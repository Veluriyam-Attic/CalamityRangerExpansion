using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.CPreMoodLord.MalevolenceC
{
    internal class MalevolenceRePlagueNest : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.CPreMoodLord";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 76;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 22;
        }

        public override bool? CanDamage() => Projectile.localAI[0] % 30f <= 3f;

        public override void AI()
        {
            Projectile.localAI[0]++;
            Projectile.rotation += 0.025f;
            Projectile.velocity *= 0.96f;
            Lighting.AddLight(Projectile.Center, Color.GreenYellow.ToVector3() * 0.28f);

            for (int i = 0; i < 4; i++)
            {
                Vector2 offset = Main.rand.NextVector2Circular(Projectile.width * 0.42f, Projectile.height * 0.42f);
                Dust d = Dust.NewDustPerfect(Projectile.Center + offset, Main.rand.NextBool() ? DustID.PoisonStaff : DustID.Smoke, offset.SafeNormalize(Vector2.UnitY) * 0.35f, 130, Color.Lerp(Color.DarkOliveGreen, Color.LimeGreen, Main.rand.NextFloat()), Main.rand.NextFloat(0.75f, 1.25f));
                d.noGravity = true;
            }

            if (Projectile.localAI[0] % 30f == 1f)
            {
                Projectile.Damage();
                GeneralParticleHandler.SpawnParticle(new DirectionalPulseRing(Projectile.Center, Vector2.Zero, Color.GreenYellow, new Vector2(1f, 1.7f), Projectile.rotation, 0.1f, 0.035f, 18));
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Venom, 180);
            target.AddBuff(ModContent.BuffType<Plague>(), 180);
        }
    }
}
