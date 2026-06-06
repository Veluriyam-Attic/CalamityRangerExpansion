using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.DPreDog.ArterialAssaultC
{
    internal class ArterialAssaultReBloodSiphon : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.DPreDog";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 150;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 16;
        }

        public override bool? CanDamage() => Projectile.localAI[0] > 10f;

        public override void AI()
        {
            Projectile.localAI[0]++;
            Projectile.rotation += 0.11f;

            Player player = Main.player[Projectile.owner];
            if (Projectile.localAI[0] > 45f && player.active)
            {
                Vector2 desired = (player.Center - Projectile.Center).SafeNormalize(Vector2.UnitY) * 15f;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desired, 0.08f);
                if (Projectile.Hitbox.Intersects(player.Hitbox) && player.statLife < player.statLifeMax2)
                {
                    int heal = 2;
                    player.statLife += heal;
                    player.HealEffect(heal);
                    Projectile.Kill();
                }
            }
            else
            {
                NPC target = Projectile.Center.ClosestNPCAt(620f);
                if (target != null)
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitY) * 10f, 0.06f);
            }

            Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(4f, 4f), DustID.Blood, -Projectile.velocity * 0.08f, 100, Color.DarkRed, 1.1f);
            d.noGravity = true;
            if (Main.rand.NextBool(4))
                GeneralParticleHandler.SpawnParticle(new GlowOrbParticle(Projectile.Center, Vector2.Zero, false, 8, 0.7f, Color.Red, true, false, true));
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Bleeding, 180);
            BowChangeVFX.SpawnImpact(Projectile, BowChangeTheme.Arterial, 0.75f);
            Projectile.velocity = -Projectile.velocity.SafeNormalize(Vector2.UnitY) * 8f;
            Projectile.localAI[0] = 50f;
        }
    }
}
