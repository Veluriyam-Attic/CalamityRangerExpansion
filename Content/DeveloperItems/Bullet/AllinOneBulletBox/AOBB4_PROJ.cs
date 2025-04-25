using Microsoft.Xna.Framework;
using System;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.DataStructures;

namespace CalamityRangerExpansion.Content.DeveloperItems.Bullet.AllinOneBulletBox
{
    internal class AOBB4_PROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.AllinOneBulletBox";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj"; // 透明贴图

        private int[] dustArray = new int[] { DustID.Electric, DustID.BlueTorch, DustID.WhiteTorch, DustID.Vortex }; // 增强粒子效果
        private float rotationOffset;

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 2;
        }

        public override void OnSpawn(IEntitySource source)
        {
            rotationOffset = Main.rand.NextFloat(MathHelper.TwoPi);
        }

        public override void AI()
        {
            CreateFlyingDust();
            CreateEnergyRingEffect();
        }

        private void CreateFlyingDust()
        {
            if (Main.rand.NextBool(2)) // 1/2 概率生成基本粒子
            {
                int dustType = dustArray[Main.rand.Next(dustArray.Length)];
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, dustType);
                dust.velocity *= 0.2f;
                dust.scale = Main.rand.NextFloat(1.3f, 1.8f);
                dust.noGravity = true;
            }
        }

        private void CreateEnergyRingEffect()
        {
            for (int i = 0; i < 3; i++) // 3 层环绕特效
            {
                float angle = (Projectile.ai[0] * 0.2f + i * MathHelper.PiOver2) + rotationOffset;
                Vector2 offset = new Vector2(4f * (i + 1), 0).RotatedBy(angle);
                int dustType = dustArray[i % dustArray.Length];
                Dust ringDust = Dust.NewDustDirect(Projectile.Center + offset, 1, 1, dustType);
                ringDust.velocity *= 0.1f;
                ringDust.scale = 1.2f;
                ringDust.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            GenerateLaserAttacks();
            CreateExplosionEffect();
            SoundEngine.PlaySound(SoundID.Item91, Projectile.position);
            Projectile.Kill();
        }

        private void GenerateLaserAttacks()
        {
            int laserCount = Main.rand.Next(3, 5); // 生成 3~4 个激光
            Vector2 ellipseCenter = Projectile.Center + new Vector2(0, 30 * 16);

            for (int i = 0; i < laserCount; i++)
            {
                float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                float xOffset = (float)Math.Cos(angle) * (15 * 16);
                float yOffset = (float)Math.Sin(angle) * (4 * 16);
                Vector2 laserSpawnPos = ellipseCenter + new Vector2(xOffset, yOffset);

                Vector2 laserDirection = Vector2.Normalize(Projectile.Center - laserSpawnPos) * 10f;

                Projectile.NewProjectile(Projectile.GetSource_FromThis(), laserSpawnPos, laserDirection, ModContent.ProjectileType<AOBB4_Lazer>(), (int)(Projectile.damage * 0.33f), 0f, Projectile.owner);
            }
        }

        private void CreateExplosionEffect()
        {
            for (int i = 0; i < 30; i++)
            {
                int dustType = dustArray[Main.rand.Next(dustArray.Length)];
                Dust dust = Dust.NewDustDirect(Projectile.Center, 10, 10, dustType);
                dust.velocity = Main.rand.NextVector2Circular(7f, 7f);
                dust.scale = Main.rand.NextFloat(2.0f, 2.8f);
                dust.noGravity = true;
            }
        }
    }
}
