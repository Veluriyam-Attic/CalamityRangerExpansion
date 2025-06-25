using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using System;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Particles;
using CalamityMod;
using CalamityRangerExpansion.LightingBolts.Shader;
using Terraria.DataStructures;
using Terraria.GameContent;
using Microsoft.CodeAnalysis.Text;
using CalamityRangerExpansion.LightingBolts.Particles;

namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.Pyroblast
{
    internal class PyroblastRocketScorpio : ModProjectile
    {
        private int chosenForm;
        private bool trackingActivated = false; // 是否进入追踪模式

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 11;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;
            Projectile.ignoreWater = true;
            Projectile.arrow = true;
            Projectile.extraUpdates = 1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            // **随机选择形态（只执行一次）**
            chosenForm = Main.rand.Next(3);
        }

        public override void AI()
        {
            // **旋转调整**
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            // **每帧在弹幕尾部释放粒子**
            if (Main.rand.NextBool(2)) // 50% 概率生成粒子，防止太密集
            {
                Color particleColor = Color.Lerp(Color.LightBlue, Color.DarkBlue, Main.rand.NextFloat());
                EnhancementParticle particle = new EnhancementParticle(
                    Projectile.Center - Projectile.velocity * 0.5f, // 让粒子生成在弹幕后方
                    -Projectile.velocity * 0.2f + Main.rand.NextVector2Circular(1f, 1f), // 让粒子略微扩散
                    false, // 不受重力影响
                    Main.rand.Next(25, 40), // 粒子存活时间
                    Main.rand.NextFloat(0.6f, 1f), // 粒子大小
                    particleColor, // 颜色在青蓝色-深蓝色间浮动
                    0.97f, // 逐渐缩小
                    Main.rand.NextFloat(-0.1f, 0.1f) // 旋转速度
                );
                GeneralParticleHandler.SpawnParticle(particle);
            }

            // **加速飞行**
            if (Projectile.timeLeft > 270)
            {
                Projectile.velocity *= 1.02f; // 逐渐加速
                if (Projectile.velocity.Length() > 16f)
                    Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 16f; // 速度限制
            }
            else
            {
                // **进入追踪模式**
                NPC target = Projectile.Center.ClosestNPCAt(5800);
                if (target != null)
                {
                    Vector2 direction = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * 10f, 0.08f);
                }

                // **刚进入追踪模式时，释放橙色特效**
                if (!trackingActivated)
                {
                    trackingActivated = true;
                    for (int i = 0; i < 10; i++)
                    {
                        Particle nanoDust = new NanoParticle(Projectile.Center, -Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)), Color.Orange, Main.rand.NextFloat(0.65f, 0.9f), Main.rand.Next(15, 20 + 1), Main.rand.NextBool(), true);
                        GeneralParticleHandler.SpawnParticle(nanoDust);
                    }
                }
            }
        }

 

    }
}
