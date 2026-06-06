using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityRangerExpansion.Content.BOWChange.DPreDog.PlanetaryAnnihilationC
{
    internal class PlanetaryAnnihilationReGravityWell : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.DPreDog";
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.RainbowCrystalExplosion}";

        public override void SetDefaults()
        {
            Projectile.width = 172;
            Projectile.height = 172;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 96;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
        }

        public override bool? CanDamage() => Projectile.localAI[0] % 16f <= 3f;

        public override bool PreDraw(ref Color lightColor) => false;

        public override void AI()
        {
            Projectile.localAI[0]++;
            float progress = Projectile.localAI[0] / 96f;
            float radius = MathHelper.Lerp(360f, 90f, progress);
            Lighting.AddLight(Projectile.Center, Color.Lerp(Color.Gold, Color.Cyan, progress).ToVector3() * 0.7f);

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.life <= 0 || !npc.CanBeChasedBy(Projectile))
                    continue;

                Vector2 toCenter = Projectile.Center - npc.Center;
                float distance = toCenter.Length();
                if (distance > radius || distance < 10f)
                    continue;

                float pull = MathHelper.Lerp(0.42f, 0.08f, distance / radius);
                npc.velocity += toCenter.SafeNormalize(Vector2.Zero) * pull;
                npc.velocity *= 0.985f;
            }

            if (Projectile.owner == Main.myPlayer && Projectile.localAI[0] % 16f == 0f)
                Projectile.Damage();

            if (Main.dedServ)
                return;

            int ringDust = Projectile.localAI[0] % 8f == 0f ? 28 : 5;
            for (int i = 0; i < ringDust; i++)
            {
                float angle = MathHelper.TwoPi * i / ringDust + Projectile.localAI[0] * 0.08f;
                Vector2 offset = angle.ToRotationVector2() * MathHelper.Lerp(20f, 72f, i / (float)ringDust);
                if (ringDust <= 5)
                    offset = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(34f, 82f);

                Dust dust = Dust.NewDustPerfect(
                    Projectile.Center + offset,
                    Main.rand.NextBool() ? DustID.RainbowMk2 : DustID.GoldFlame,
                    offset.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.4f, 1.6f) - offset * 0.018f,
                    110,
                    Color.Lerp(Color.Cyan, Color.Gold, Main.rand.NextFloat()),
                    Main.rand.NextFloat(0.9f, 1.45f));
                dust.noGravity = true;
            }

            if (Projectile.localAI[0] % 24f == 0f)
                BowChangeVFX.SpawnImpact(Projectile, BowChangeTheme.Planetary, 0.75f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Slow, 90);
            target.AddBuff(BuffID.OnFire3, 60);
        }
    }
}
