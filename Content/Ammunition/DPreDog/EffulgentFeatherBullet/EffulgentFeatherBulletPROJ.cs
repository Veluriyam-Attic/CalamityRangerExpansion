using CalamityRangerExpansion.Content.Ammunition.DPreDog.DivineGeodeBullet;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityRangerExpansion.CREConfigs;
using Terraria.GameContent;

namespace CalamityRangerExpansion.Content.Ammunition.DPreDog.EffulgentFeatherBullet
{
    internal class EffulgentFeatherBulletPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectile.DPreDog";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 获取 SpriteBatch 和投射物纹理
                SpriteBatch spriteBatch = Main.spriteBatch;
                Texture2D lightTexture = TextureAssets.Projectile[Projectile.type].Value;

                // 遍历投射物的旧位置数组，绘制光学拖尾效果
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    // 计算颜色插值值，使颜色在旧位置之间平滑过渡
                    float colorInterpolation = (float)Math.Cos(Projectile.timeLeft / 32f + Main.GlobalTimeWrappedHourly / 20f + i / (float)Projectile.oldPos.Length * MathHelper.Pi) * 0.5f + 0.5f;

                    // 使用天蓝色渐变
                    Color color = Color.Lerp(Color.Yellow, Color.LightGoldenrodYellow, colorInterpolation) * 0.4f;
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
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 420;
            Projectile.MaxUpdates = 6;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 2;
        }

        public override void AI()
        {
            // 由于我们是水平贴图，因此什么也不需要转动
            Projectile.rotation = Projectile.velocity.ToRotation();

            // 添加光效
            Lighting.AddLight(Projectile.Center, Color.Lerp(Color.Black, Color.Black, 0.5f).ToVector3() * 0.49f);

            // 子弹在出现之后很短一段时间会变得可见
            if (Projectile.timeLeft == 418)
                Projectile.alpha = 0;




            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 添加飞行粒子特效
                if (Main.rand.NextBool(3)) // 1/3 概率生成粒子
                {
                    Dust dust = Dust.NewDustPerfect(
                        Projectile.Center,
                        Main.rand.NextBool() ? DustID.YellowTorch : DustID.RedTorch,
                        -Projectile.velocity.RotatedByRandom(0.1f) * Main.rand.NextFloat(0.1f, 0.3f)
                    );
                    dust.noGravity = true; // 无重力
                    dust.scale = Main.rand.NextFloat(0.7f, 1.1f); // 随机大小
                }
            }

        }

        public override void OnSpawn(IEntitySource source)
        {

        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 1. 给玩家添加 Buff
            Player player = Main.player[Projectile.owner];

            // **检查玩家是否已有 Buff**
            if (!player.HasBuff(ModContent.BuffType<EffulgentFeatherBulletPBuff>()))
            {
                player.AddBuff(ModContent.BuffType<EffulgentFeatherBulletPBuff>(), 300); // 5 秒
            }

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                //// ⚡ 在目标位置生成一条随机方向的闪电线
                //// 设置闪电的起点
                //Vector2 startPoint = target.Center;

                //// 随机生成方向和长度
                //float randomDirection = Main.rand.NextFloat(0f, MathHelper.TwoPi); // 随机生成一个 0 到 2π 的方向
                //float randomLength = Main.rand.NextFloat(10f, 20f) * 16f; // 随机长度为 10 到 20 tile
                //Vector2 endPoint = startPoint + new Vector2((float)Math.Cos(randomDirection), (float)Math.Sin(randomDirection)) * randomLength;

                //// 在起点到终点之间生成粒子效果
                //int particleCount = Main.rand.Next(10, 15); // 随机粒子数量
                //for (int j = 0; j < particleCount; j++)
                //{
                //    // 根据进度插值计算每个粒子的位置，增加随机偏移
                //    float progress = j / (float)(particleCount - 1);
                //    Vector2 position = Vector2.Lerp(startPoint, endPoint, progress) + Main.rand.NextVector2Circular(4f, 4f); // 增加随机偏移

                //    // 随机选择粒子类型
                //    int dustType = Main.rand.NextBool() ? DustID.Electric : DustID.BlueTorch;

                //    // 创建粒子特效
                //    Dust dust = Dust.NewDustPerfect(
                //        position,
                //        dustType,
                //        Main.rand.NextVector2Circular(1f, 1f), // 粒子微小速度偏移
                //        150,
                //        default,
                //        Main.rand.NextFloat(1.2f, 1.8f) // 粒子大小随机
                //    );
                //    dust.noGravity = true; // 禁用重力
                //}
            }

            // 检查当前场上的 EffulgentFeatherBulletAREA 数量
            int existingProjectileCount = 0;
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.type == ModContent.ProjectileType<EffulgentFeatherBulletAREA>())
                {
                    existingProjectileCount++;
                    if (existingProjectileCount >= 2)
                    {
                        return; // 如果已经存在两个，则不再生成新弹幕
                    }
                }
            }

            // 3. 在玩家位置释放 EffulgentFeatherBulletAREA
            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                player.Center,
                Vector2.Zero,
                ModContent.ProjectileType<EffulgentFeatherBulletAREA>(),
                (int)(Projectile.damage * 0.5f), // 伤害倍率为 0.5
                0f,
                Projectile.owner
            );
        }


        public override void OnKill(int timeLeft)
        {

        }
    }
}
