using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Ranged;
using Terraria.DataStructures;
using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using CalamityRangerExpansion.CREConfigs;
using CalamityMod.Particles;

namespace CalamityRangerExpansion.Content.Arrows.DPreDog.EffulgentFeatherArrow
{
    public class EffulgentFeatherArrowPROJ : ModProjectile, ILocalizedModType
    {
        public override string Texture => "CalamityRangerExpansion/Content/Arrows/DPreDog/EffulgentFeatherArrow/EffulgentFeatherArrow";
        public new string LocalizationCategory => "Projectile.DPreDog";
        private float rotationSpeed = 0.05f; // 设置旋转速度
        private float rotationAngle = 0f;    // 初始化旋转角度

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override bool PreDraw(ref Color lightColor)
        {           
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {

                // 获取 SpriteBatch 和投射物纹理
                SpriteBatch spriteBatch = Main.spriteBatch;
                Texture2D lightTexture = ModContent.Request<Texture2D>("CalamityRangerExpansion/Content/Arrows/DPreDog/EffulgentFeatherArrow/EffulgentFeatherArrow").Value;

                // 遍历投射物的旧位置数组，绘制光学拖尾效果
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    // 计算颜色插值值，使颜色在旧位置之间平滑过渡
                    float colorInterpolation = (float)Math.Cos(Projectile.timeLeft / 32f + Main.GlobalTimeWrappedHourly / 20f + i / (float)Projectile.oldPos.Length * MathHelper.Pi) * 0.5f + 0.5f;

                    // 使用浅橙红色渐变
                    Color color = Color.Lerp(Color.LightSalmon, Color.OrangeRed, colorInterpolation) * 0.4f;
                    color.A = 0;

                    // 计算绘制位置，将位置调整到碰撞箱的中心
                    Vector2 drawPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

                    // 计算外部和内部的颜色
                    Color outerColor = color;
                    Color innerColor = color * 0.5f;

                    // 计算强度，使拖尾逐渐变弱
                    float intensity = 0.9f + 0.15f * (float)Math.Cos(Main.GlobalTimeWrappedHourly % 60f * MathHelper.TwoPi);
                    intensity *= MathHelper.Lerp(0.15f, 1f, 1f - i / (float)Projectile.oldPos.Length);
                    if (Projectile.timeLeft <= 60)
                    {
                        intensity *= Projectile.timeLeft / 60f; // 如果弹幕即将消失，则拖尾也逐渐消失
                    }

                    // 计算外部和内部的缩放比例，使拖尾具有渐变效果
                    Vector2 outerScale = new Vector2(2f) * intensity;
                    Vector2 innerScale = new Vector2(2f) * intensity * 0.7f;
                    outerColor *= intensity;
                    innerColor *= intensity;

                    // 绘制外部的拖尾效果，并应用旋转
                    Main.EntitySpriteDraw(lightTexture, drawPosition, null, outerColor, Projectile.rotation, lightTexture.Size() * 0.5f, outerScale * 0.6f, SpriteEffects.None, 0);

                    // 绘制内部的拖尾效果，并应用旋转
                    Main.EntitySpriteDraw(lightTexture, drawPosition, null, innerColor, Projectile.rotation, lightTexture.Size() * 0.5f, innerScale * 0.6f, SpriteEffects.None, 0);
                }

                // 绘制默认的弹幕，并应用旋转
                Main.EntitySpriteDraw(lightTexture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, lightColor, Projectile.rotation, lightTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

                return false;
            }
            return true;
        }
        public override void SetDefaults()
        {
            // 设置弹幕的基础属性
            Projectile.width = 11; // 弹幕宽度
            Projectile.height = 24; // 弹幕高度
            Projectile.friendly = true; // 对敌人有效
            Projectile.DamageType = DamageClass.Ranged; // 远程伤害类型
            Projectile.penetrate = -1; // 穿透力为x
            Projectile.timeLeft = 80; // 弹幕存在时间
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
            Projectile.ignoreWater = true; // 弹幕不受水影响
            Projectile.arrow = true;
            Projectile.extraUpdates = 3;
            Projectile.tileCollide = false;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.velocity *= 0.6f;
        }
        public override void AI()
        {
            // 调整弹幕的旋转，使其在飞行时保持水平
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;

            // Lighting - 添加浅橙色光源，光照强度为 0.49
            Lighting.AddLight(Projectile.Center, Color.Lerp(Color.LightSalmon, Color.OrangeRed, 0.5f).ToVector3() * 0.49f);

            // 维持 EffulgentFeatherArrowAura 的存在
            if (Projectile.localAI[0] == 0f)
            {
                // 在第一次执行时生成 Aura 弹幕，并记录其 ID
                int auraProjectileID = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<EffulgentFeatherArrowAura>(), (int)(Projectile.damage * 0.15f), 0, Projectile.owner, Projectile.whoAmI);
                Projectile.localAI[0] = auraProjectileID + 1; // 存储 Aura 的 ID，+1 以防止冲突
            }
            else
            {
                // 确保 Aura 与箭头保持同步
                int auraProjectileID = (int)(Projectile.localAI[0] - 1);
                if (Main.projectile.IndexInRange(auraProjectileID))
                {
                    Projectile auraProjectile = Main.projectile[auraProjectileID];
                    if (auraProjectile.active && auraProjectile.type == ModContent.ProjectileType<EffulgentFeatherArrowAura>() && auraProjectile.ai[0] == Projectile.whoAmI)
                    {
                        // 将 Aura 位置设置为与箭头匹配
                        auraProjectile.Center = Projectile.Center;
                        auraProjectile.damage = (int)((Projectile.damage) * 0.15);
                        auraProjectile.velocity = Vector2.Zero; // 确保 Aura 不移动
                    }
                    else
                    {
                        // 如果 Aura 不存在或失效，则重新生成
                        Projectile.localAI[0] = 0f;
                    }
                }
            }


            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 更新旋转角度，控制粒子公转
                rotationAngle += rotationSpeed;
                if (rotationAngle > MathHelper.TwoPi)
                {
                    rotationAngle -= MathHelper.TwoPi;
                }

                // 更新旋转角度，控制粒子公转（速度提高 2.5 倍）
                rotationAngle += rotationSpeed * 2.35f;
                if (rotationAngle > MathHelper.TwoPi)
                {
                    rotationAngle -= MathHelper.TwoPi;
                }

                int particleCount = 1; // 每次生成 1 个粒子
                float[] angles = { 0f, MathHelper.Pi }; // 粒子间隔 180 度

                foreach (float initialAngle in angles)
                {
                    // 计算粒子的当前角度和位置，公转半径缩小为 X 格（X*16 像素）
                    float angle = initialAngle + rotationAngle;
                    Vector2 position = Projectile.Center + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 0.75f * 16f;

                    // 创建粒子特效
                    for (int j = 0; j < 1; j++)
                    {
                        int dustType = Main.rand.NextBool() ? DustID.RedTorch : DustID.YellowTorch; // 随机粒子类型
                        Dust dust = Dust.NewDustPerfect(position, dustType);
                        dust.noGravity = true; // 粒子无重力效果
                        dust.scale = 1.25f; // 粒子大小
                        dust.fadeIn = 1.2f; // 粒子淡入效果
                        dust.rotation = Main.rand.NextFloat(MathHelper.TwoPi); // 随机初始旋转
                        dust.velocity = Vector2.Zero; // 初始速度为零
                    }
                }

                if (Projectile.numUpdates % 12 == 0) // 频率
                {
                    // 在后方 2X 度范围内随机生成粒子
                    // 定义弧度范围，便于随时调整
                    float angleRange = MathHelper.ToRadians(25f); // X 度
                    // 在指定范围内随机生成角度
                    float randomAngle = Main.rand.NextFloat(-angleRange, angleRange); // 后方 -angleRange 到 +angleRange
                    Vector2 particleVelocity = Projectile.velocity.RotatedBy(MathHelper.Pi + randomAngle).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(2f, 4f); // 粒子速度

                    // 随机选择浅黄色或浅红色
                    Color particleColor = Main.rand.NextBool() ? Color.LightYellow : Color.LightSalmon;

                    float randomScale = Main.rand.NextFloat(0.85f, 1.25f); // 随机大小
                    Particle bolt = new CrackParticle(
                        Projectile.Center, // 粒子生成位置为弹幕中心
                        particleVelocity, // 粒子速度
                        particleColor * 0.65f, // 设置粒子的颜色和透明度
                        Vector2.One * randomScale, // 设置粒子大小
                        0, // 初始旋转速度
                        0, // 初始角度偏移
                        randomScale, // 粒子缩放比例
                        11 // 粒子类型编号
                    );
                    GeneralParticleHandler.SpawnParticle(bolt);
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 在结束时释放几个卡其色的小圆圈特效
                for (int i = 0; i < 3; i++)
                {
                    //Particle pulse = new DirectionalPulseRing(Projectile.Center, Projectile.velocity * 0.75f, Color.OrangeRed, new Vector2(1f, 2.5f), Projectile.rotation, 0.2f, 0.03f, 20);
                    //GeneralParticleHandler.SpawnParticle(pulse);
                }
            }
    
        }
    }
}