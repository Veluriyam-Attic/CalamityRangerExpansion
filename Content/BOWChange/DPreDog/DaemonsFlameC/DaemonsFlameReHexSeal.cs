using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.DPreDog.DaemonsFlameC
{
    internal class DaemonsFlameReHexSeal : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.DPreDog";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 104;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 66;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 18;
        }

        public override bool? CanDamage() => Projectile.localAI[0] % 22f <= 3f;

        public override void AI()
        {
            Projectile.localAI[0]++;
            Projectile.rotation += 0.07f;

            for (int i = 0; i < 3; i++)
            {
                float angle = Projectile.rotation + MathHelper.TwoPi * i / 3f;
                Vector2 a = Projectile.Center + angle.ToRotationVector2() * 46f;
                Vector2 b = Projectile.Center + (angle + MathHelper.TwoPi / 3f).ToRotationVector2() * 46f;
                Dust d = Dust.NewDustPerfect(Vector2.Lerp(a, b, Main.rand.NextFloat()), DustID.Shadowflame, Vector2.Zero, 120, Color.Lerp(Color.HotPink, Color.MediumPurple, Main.rand.NextFloat()), 1.15f);
                d.noGravity = true;
            }

            if (Projectile.localAI[0] % 22f == 1f)
            {
                Projectile.Damage();
                GeneralParticleHandler.SpawnParticle(new DirectionalPulseRing(Projectile.Center, Vector2.Zero, Color.HotPink, new Vector2(1f, 1.5f), Projectile.rotation, 0.12f, 0.035f, 18));
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), 150);
        }
    }
}
