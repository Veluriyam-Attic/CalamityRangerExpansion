using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.BPrePlantera.BarinauticalC
{
    internal class BarinauticalReDepthCharge : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.BPrePlantera";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 34;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 72;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 18;
        }

        public override bool? CanDamage() => Projectile.localAI[0] > 28f;

        public override void AI()
        {
            Projectile.localAI[0]++;
            Projectile.velocity *= 0.94f;
            Projectile.rotation += Projectile.velocity.X * 0.02f + 0.06f;

            float arm = 18f + Projectile.localAI[0] * 0.25f;
            for (int i = 0; i < 4; i++)
            {
                Vector2 offset = (Projectile.rotation + MathHelper.PiOver2 * i).ToRotationVector2() * arm;
                Dust d = Dust.NewDustPerfect(Projectile.Center + offset, DustID.Electric, -offset.SafeNormalize(Vector2.UnitY) * 0.6f, 120, Color.Cyan, 1.05f);
                d.noGravity = true;
            }

            if (Projectile.localAI[0] == 30f)
            {
                Projectile.Resize(118, 118);
                Projectile.Damage();
                BowChangeVFX.SpawnImpact(Projectile, BowChangeTheme.Ocean, 1.1f);
                for (int i = 0; i < 18; i++)
                {
                    Vector2 vel = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(3f, 7f);
                    GeneralParticleHandler.SpawnParticle(new SparkParticle(Projectile.Center, vel, false, 28, 1.1f, Color.LightBlue));
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Electrified, 120);
        }
    }
}
