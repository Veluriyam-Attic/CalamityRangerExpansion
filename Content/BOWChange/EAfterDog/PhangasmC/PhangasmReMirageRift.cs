using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityRangerExpansion.Content.BOWChange.EAfterDog.PhangasmC
{
    internal class PhangasmReMirageRift : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.EAfterDog";
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.VortexVortexLightning}";

        private bool TrueRift => Projectile.ai[0] < 0.5f;
        private int TargetIndex => (int)Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.width = 136;
            Projectile.height = 136;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 86;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
        }

        public override bool? CanDamage() => Projectile.localAI[0] % 18f <= 3f;

        public override bool PreDraw(ref Color lightColor) => false;

        public override void AI()
        {
            Projectile.localAI[0]++;
            Color mainColor = TrueRift ? Color.Cyan : Color.GreenYellow;
            Lighting.AddLight(Projectile.Center, mainColor.ToVector3() * 0.72f);

            NPC target = null;
            if (TargetIndex >= 0 && TargetIndex < Main.maxNPCs)
            {
                NPC candidate = Main.npc[TargetIndex];
                if (candidate.active && candidate.CanBeChasedBy(Projectile))
                    target = candidate;
            }

            if (target != null)
                Projectile.Center = Vector2.Lerp(Projectile.Center, target.Center, TrueRift ? 0.22f : 0.12f);

            if (Projectile.owner == Main.myPlayer && Projectile.localAI[0] % 18f == 0f)
                Projectile.Damage();

            if (Projectile.owner == Main.myPlayer && Projectile.localAI[0] % (TrueRift ? 26f : 20f) == 0f)
                ReleaseMirage(target);

            if (Main.dedServ)
                return;

            int dustType = TrueRift ? DustID.Vortex : DustID.TintableDustLighted;
            int count = TrueRift ? 6 : 9;
            for (int i = 0; i < count; i++)
            {
                float angle = Projectile.localAI[0] * (TrueRift ? 0.15f : -0.19f) + MathHelper.TwoPi * i / count;
                Vector2 offset = angle.ToRotationVector2() * Main.rand.NextFloat(28f, 64f);
                Dust dust = Dust.NewDustPerfect(
                    Projectile.Center + offset,
                    dustType,
                    -offset.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.8f, 2.4f),
                    90,
                    Color.Lerp(mainColor, Color.White, Main.rand.NextFloat(0.1f, 0.45f)),
                    Main.rand.NextFloat(0.75f, 1.25f));
                dust.noGravity = true;
            }

            if (Projectile.localAI[0] % 16f == 0f)
            {
                GeneralParticleHandler.SpawnParticle(new GenericSparkle(
                    Projectile.Center,
                    Vector2.Zero,
                    mainColor,
                    TrueRift ? Color.White : Color.Teal,
                    TrueRift ? 1.7f : 1.25f,
                    5,
                    Main.rand.NextFloat(-0.04f, 0.04f),
                    1.7f));
            }
        }

        private void ReleaseMirage(NPC target)
        {
            if (TrueRift)
            {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    target?.Center ?? Projectile.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<PhangasmReTrueSlash>(),
                    (int)(Projectile.damage * 0.65f),
                    Projectile.knockBack,
                    Projectile.owner);
                SoundEngine.PlaySound(SoundID.Item71 with { Volume = 0.4f }, Projectile.Center);
                return;
            }

            Vector2 baseDirection = target == null ? Vector2.UnitY : (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitY);
            for (int i = -1; i <= 1; i++)
            {
                Vector2 spawn = Projectile.Center + baseDirection.RotatedBy(MathHelper.PiOver2) * i * 34f;
                Vector2 velocity = baseDirection.RotatedBy(MathHelper.ToRadians(8f * i)) * 15f;
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    spawn,
                    velocity,
                    ModContent.ProjectileType<PhangasmReGOEST>(),
                    (int)(Projectile.damage * 0.32f),
                    0f,
                    Projectile.owner);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(TrueRift ? BuffID.CursedInferno : BuffID.Venom, 90);
        }
    }
}
