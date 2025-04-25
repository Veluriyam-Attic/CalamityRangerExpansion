using CalamityMod.Particles;
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
using Terraria.Audio;
using CalamityRangerExpansion.CREConfigs;

namespace CalamityRangerExpansion.Content.WeaponToAMMO.Bullet.DestructionBullet
{
    public class DestructionBulletPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "WeaponToAMMO.Bullet.DestructionBullet";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // 获取 SpriteBatch 和投射物纹理
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D lightTexture = ModContent.Request<Texture2D>("CalamityRangerExpansion/Content/WeaponToAMMO/Bullet/DestructionBullet/DestructionBulletPROJ").Value;

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
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 450;
            Projectile.MaxUpdates = 6;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;
        }

        public override void AI()
        {
            // 由于我们是水平贴图，因此什么也不需要转动
            Projectile.rotation = Projectile.velocity.ToRotation();


            // 子弹在出现之后很短一段时间会变得可见
            if (Projectile.timeLeft == 445)
                Projectile.alpha = 0;

            // 添加光效
            Lighting.AddLight(Projectile.Center, Color.Lerp(Color.Yellow, Color.LightGoldenrodYellow, 0.6f).ToVector3() * 0.6f);

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 添加随机粒子特效
                //if (Main.rand.NextBool(3)) // 随机 1/3 概率
                {
                    int randomDustID = Main.rand.Next(1, 324); // 随机选择 1 到 323 的 DustID
                    Dust dust = Dust.NewDustPerfect(
                        Projectile.Center,
                        randomDustID, // 使用随机 DustID
                        -Projectile.velocity.RotatedByRandom(0.1f) * Main.rand.NextFloat(0.01f, 0.3f) // 粒子速度
                    );
                    dust.noGravity = true; // 无重力
                    dust.scale = Main.rand.NextFloat(0.6f, 1.0f); // 随机大小
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
            //// 命中时释放明黄色粒子效果
            //Particle hitEffect = new CustomPulse(
            //    Projectile.Center, Vector2.Zero, Color.LightYellow,
            //    "CalamityMod/Particles/DustyCircleHardEdge",
            //    Vector2.One, Main.rand.NextFloat(-10f, 10f),
            //    0.03f, 0.15f, 30
            //);
            //GeneralParticleHandler.SpawnParticle(hitEffect);
        }

        public override void OnKill(int timeLeft)
        {
            // 消亡时释放额外弹幕
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<DestructionBulletEXP>(),
                    (int)(Projectile.damage * 0.5f), Projectile.knockBack,
                    Projectile.owner
                );
            }

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 消亡时释放爆炸特效
                //Particle bloodsplosion2 = new CustomPulse(
                //    Projectile.Center,
                //    Vector2.Zero,
                //    new Color(Main.rand.Next(256), Main.rand.Next(256), Main.rand.Next(256)), // 随机生成 RGB 颜色
                //    "CalamityMod/Particles/DustyCircleHardEdge",
                //    Vector2.One * 0.7f,
                //    Main.rand.NextFloat(-15f, 15f),
                //    0.03f,
                //    0.155f,
                //    40
                //);
                //GeneralParticleHandler.SpawnParticle(bloodsplosion2);

                // 消亡时释放爆炸特效
                Particle blastRing = new CustomPulse(
                    Projectile.Center, Vector2.Zero, new Color(Main.rand.Next(256), Main.rand.Next(256), Main.rand.Next(256)), // 随机生成 RGB 颜色
                    "CalamityRangerExpansion/texture/ChemicalPollution",
                    Vector2.One * 0.33f, Main.rand.NextFloat(-10f, 10f),
                    0.25f, 0.57f, 30
                );
                GeneralParticleHandler.SpawnParticle(blastRing);
            }
            SoundEngine.PlaySound(SoundID.Item92, Projectile.Center);
        }
    }
}
