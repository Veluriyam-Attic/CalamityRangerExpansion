using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.BPrePlantera.DarkechoGreatbowC
{
    internal class DarkechoGreatbowReEchoShard : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.BPrePlantera";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 90;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
        }

        public override bool? CanDamage() => Projectile.localAI[0] > 18f;

        public override void AI()
        {
            Projectile.localAI[0]++;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (Projectile.localAI[0] < 18f)
            {
                Projectile.velocity *= 0.92f;
            }
            else
            {
                NPC target = Projectile.Center.ClosestNPCAt(900f);
                if (target != null)
                {
                    Vector2 desired = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitY) * 13f;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desired, 0.11f);
                }
                else
                {
                    Projectile.velocity *= 1.02f;
                }
            }

            Lighting.AddLight(Projectile.Center, Color.MediumPurple.ToVector3() * 0.35f);
            if (Main.rand.NextBool(2))
            {
                GeneralParticleHandler.SpawnParticle(new SparkParticle(
                    Projectile.Center + Main.rand.NextVector2Circular(4f, 4f),
                    -Projectile.velocity.SafeNormalize(Vector2.UnitY) * Main.rand.NextFloat(0.4f, 1.6f),
                    false,
                    18,
                    Main.rand.NextFloat(0.7f, 1.1f),
                    Main.rand.NextBool() ? Color.HotPink : Color.LightBlue));
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Frostburn2, 150);
            BowChangeVFX.SpawnImpact(Projectile, BowChangeTheme.Darkecho, 0.8f);
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    target.Center,
                    Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedBy(MathHelper.PiOver2) * 0.01f,
                    ModContent.ProjectileType<DarkechoGreatbowReSlash>(),
                    Math.Max(1, Projectile.damage / 2),
                    Projectile.knockBack,
                    Projectile.owner);
            }
        }
    }
}
