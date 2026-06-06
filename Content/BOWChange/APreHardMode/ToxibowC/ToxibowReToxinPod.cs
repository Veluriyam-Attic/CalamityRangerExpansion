using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.APreHardMode.ToxibowC
{
    internal class ToxibowReToxinPod : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.APreHardMode";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 96;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override bool? CanDamage() => Projectile.localAI[0] > 10f && Projectile.localAI[0] % 16f <= 3f;

        public override void AI()
        {
            Projectile.localAI[0]++;
            float grow = Utils.GetLerpValue(96f, 28f, Projectile.timeLeft, true);
            Projectile.Resize((int)MathHelper.Lerp(44f, 118f, grow), (int)MathHelper.Lerp(44f, 118f, grow));
            Projectile.rotation += 0.02f;
            Lighting.AddLight(Projectile.Center, Color.GreenYellow.ToVector3() * 0.35f);

            for (int i = 0; i < 5; i++)
            {
                Vector2 offset = Main.rand.NextVector2Circular(Projectile.width * 0.45f, Projectile.height * 0.45f);
                Particle smoke = new HeavySmokeParticle(
                    Projectile.Center + offset,
                    offset.SafeNormalize(Vector2.UnitY) * Main.rand.NextFloat(0.15f, 0.75f),
                    Main.rand.NextBool() ? Color.YellowGreen : Color.DarkOliveGreen,
                    24,
                    Main.rand.NextFloat(0.65f, 1.25f),
                    0.35f,
                    Main.rand.NextFloat(-0.05f, 0.05f),
                    false);
                GeneralParticleHandler.SpawnParticle(smoke);
            }

            if (Projectile.localAI[0] % 16f == 1f)
                Projectile.Damage();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Poisoned, 180);
            target.AddBuff(BuffID.Venom, 120);
            target.AddBuff(ModContent.BuffType<Plague>(), 90);
        }
    }
}
