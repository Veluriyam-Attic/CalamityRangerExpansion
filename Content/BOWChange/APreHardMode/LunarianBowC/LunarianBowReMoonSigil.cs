using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.APreHardMode.LunarianBowC
{
    internal class LunarianBowReMoonSigil : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.APreHardMode";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 92;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 72;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 18;
        }

        public override bool? CanDamage() => Projectile.localAI[0] > 8f && Projectile.localAI[0] % 12f <= 2f;

        public override void AI()
        {
            Projectile.localAI[0]++;
            Projectile.rotation += 0.045f;

            float pulse = 0.5f + 0.5f * (float)System.Math.Sin(Projectile.localAI[0] * 0.18f);
            Lighting.AddLight(Projectile.Center, Color.LightSkyBlue.ToVector3() * (0.25f + pulse * 0.18f));

            int points = 18;
            float radius = 28f + pulse * 16f;
            for (int i = 0; i < points; i++)
            {
                float phase = Projectile.rotation + MathHelper.Pi * i / (points - 1);
                Vector2 pos = Projectile.Center + phase.ToRotationVector2() * radius;
                Dust d = Dust.NewDustPerfect(pos, DustID.WhiteTorch, Vector2.Zero, 100, Color.Lerp(Color.White, Color.LightSkyBlue, pulse), 0.85f);
                d.noGravity = true;
            }

            if (Projectile.localAI[0] % 12f == 1f)
            {
                Projectile.Damage();
                GeneralParticleHandler.SpawnParticle(new DirectionalPulseRing(
                    Projectile.Center,
                    Vector2.Zero,
                    Color.White,
                    new Vector2(1f, 1.4f),
                    Projectile.rotation,
                    0.08f,
                    0.035f,
                    16));
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Slow, 90);
            BowChangeVFX.SpawnImpact(Projectile, BowChangeTheme.Lunar, 0.7f);
        }
    }
}
