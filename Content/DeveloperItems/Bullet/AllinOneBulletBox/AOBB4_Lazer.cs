using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;
using Terraria.Audio;
using Terraria.DataStructures;

namespace CalamityRangerExpansion.Content.DeveloperItems.Bullet.AllinOneBulletBox
{
    public class AOBB4_Lazer : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.AllinOneBulletBox";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private int hitCount = 0;
        private const int MaxHitBonus = 10;
        private float baseDamage;
        private float rotationOffset;
        private int[] dustArray = new int[4] { DustID.Electric, DustID.BlueTorch, DustID.WhiteTorch, DustID.Vortex }; // 添加更多粒子

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = 60;
            Projectile.timeLeft = 240;
            Projectile.scale = 2.5f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        public override void OnSpawn(IEntitySource source)
        {
            baseDamage = Projectile.damage;
            rotationOffset = Main.rand.NextFloat(MathHelper.TwoPi);
        }

        public override void AI()
        {
            CreateFlyingEffect();
            CreateEnergyRingEffect();
        }

        private void CreateFlyingEffect()
        {
            if (Main.rand.NextBool(2))
            {
                int dustType = Main.rand.Next(dustArray.Length);
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, dustArray[dustType]);
                dust.velocity *= 0.2f;
                dust.scale = Main.rand.NextFloat(1.5f, 2.2f);
                dust.noGravity = true;
            }
        }

        private void CreateEnergyRingEffect()
        {
            for (int i = 0; i < 3; i++) // 3 层环绕特效
            {
                float angle = (Projectile.ai[0] * 0.15f + i * MathHelper.PiOver2) + rotationOffset;
                Vector2 offset = new Vector2(6f * (i + 1), 0).RotatedBy(angle);
                int dustType = dustArray[i % dustArray.Length];
                Dust ringDust = Dust.NewDustDirect(Projectile.Center + offset, 1, 1, dustType);
                ringDust.velocity *= 0.1f;
                ringDust.scale = 1.2f;
                ringDust.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CreateExplosionEffect();
            SoundEngine.PlaySound(SoundID.Item94, Projectile.position);
            if (hitCount < MaxHitBonus)
            {
                hitCount++;
                Projectile.damage = (int)(baseDamage * (1f + 0.1f * hitCount));
            }
        }

        private void CreateExplosionEffect()
        {
            for (int i = 0; i < 40; i++)
            {
                int dustType = dustArray[Main.rand.Next(dustArray.Length)];
                Dust dust = Dust.NewDustDirect(Projectile.position, 10, 10, dustType);
                dust.velocity = Main.rand.NextVector2Circular(6f, 6f);
                dust.scale = Main.rand.NextFloat(1.8f, 2.5f);
                dust.noGravity = true;
            }
        }
    }
}
