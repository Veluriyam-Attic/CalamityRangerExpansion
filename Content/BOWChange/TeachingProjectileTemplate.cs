using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityRangerExpansion.Content.BOWChange
{
    internal class TeachingProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.Misc";

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
        }

        public override void AI()
        {
            Vector2 forward = Projectile.velocity.SafeNormalize(Vector2.UnitX);

            Dust dust = Dust.NewDustPerfect(
                Projectile.Center + Main.rand.NextVector2Circular(4f, 4f),
                DustID.GoldFlame,
                -forward.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.6f, 1.6f),
                120,
                Color.Gold,
                Main.rand.NextFloat(0.8f, 1.2f));
            dust.noGravity = true;

            if (Main.rand.NextBool(4))
            {
                GeneralParticleHandler.SpawnParticle(new SparkParticle(
                    Projectile.Center,
                    Projectile.velocity * 0.08f,
                    false,
                    18,
                    0.9f,
                    Color.Orange));
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            BowChangeVFX.SpawnImpact(Projectile, BowChangeTheme.Ultima, 0.8f);
        }

        public override void OnKill(int timeLeft)
        {
            BowChangeVFX.SpawnImpact(Projectile, BowChangeTheme.Ultima, 1.1f);
        }
    }
}
