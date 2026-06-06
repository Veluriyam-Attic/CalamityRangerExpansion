using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace CalamityRangerExpansion.Content.BOWChange
{
    internal enum BowChangeTheme
    {
        Lunar,
        Toxic,
        Ocean,
        Brimstone,
        Caustic,
        Darkecho,
        Malevolence,
        Ballista,
        Vernal,
        Arterial,
        DaemonsFlame,
        Nettle,
        Planetary,
        Storm,
        Condemnation,
        Deathwind,
        Phangasm,
        Ultima
    }

    internal readonly struct BowChangePalette
    {
        public BowChangePalette(Color primary, Color secondary, Color accent, int dustA, int dustB)
        {
            Primary = primary;
            Secondary = secondary;
            Accent = accent;
            DustA = dustA;
            DustB = dustB;
        }

        public readonly Color Primary;
        public readonly Color Secondary;
        public readonly Color Accent;
        public readonly int DustA;
        public readonly int DustB;
    }

    internal static class BowChangeVFX
    {
        public static BowChangePalette Palette(BowChangeTheme theme)
        {
            return theme switch
            {
                BowChangeTheme.Lunar => new BowChangePalette(new Color(190, 220, 255), Color.White, Color.LightSkyBlue, DustID.WhiteTorch, DustID.UnusedWhiteBluePurple),
                BowChangeTheme.Toxic => new BowChangePalette(Color.LimeGreen, Color.GreenYellow, Color.YellowGreen, DustID.Poisoned, DustID.PoisonStaff),
                BowChangeTheme.Ocean => new BowChangePalette(Color.Cyan, Color.LightBlue, Color.White, DustID.WaterCandle, DustID.Electric),
                BowChangeTheme.Brimstone => new BowChangePalette(Color.OrangeRed, Color.Orange, Color.DarkRed, DustID.Torch, DustID.Flare),
                BowChangeTheme.Caustic => new BowChangePalette(Color.Lime, Color.Purple, Color.YellowGreen, DustID.Venom, DustID.Poisoned),
                BowChangeTheme.Darkecho => new BowChangePalette(Color.MediumPurple, Color.DeepSkyBlue, Color.LightCyan, DustID.PurpleCrystalShard, DustID.BlueTorch),
                BowChangeTheme.Malevolence => new BowChangePalette(new Color(88, 102, 74), new Color(160, 255, 80), new Color(40, 48, 38), DustID.PoisonStaff, DustID.Smoke),
                BowChangeTheme.Ballista => new BowChangePalette(Color.Gold, Color.Orange, Color.White, DustID.Electric, DustID.Torch),
                BowChangeTheme.Vernal => new BowChangePalette(Color.SpringGreen, Color.GreenYellow, Color.White, DustID.TerraBlade, DustID.Grass),
                BowChangeTheme.Arterial => new BowChangePalette(Color.DarkRed, Color.IndianRed, Color.Red, DustID.Blood, DustID.LifeDrain),
                BowChangeTheme.DaemonsFlame => new BowChangePalette(Color.HotPink, Color.MediumPurple, Color.White, DustID.PinkTorch, DustID.Shadowflame),
                BowChangeTheme.Nettle => new BowChangePalette(new Color(70, 190, 70), new Color(120, 255, 120), new Color(40, 120, 40), DustID.JungleGrass, DustID.Grass),
                BowChangeTheme.Planetary => new BowChangePalette(Color.Gold, Color.Cyan, Color.White, DustID.RainbowMk2, DustID.GoldFlame),
                BowChangeTheme.Storm => new BowChangePalette(new Color(180, 210, 255), Color.White, Color.Cyan, DustID.Cloud, DustID.Electric),
                BowChangeTheme.Condemnation => new BowChangePalette(Color.OrangeRed, Color.Gold, Color.White, DustID.Torch, DustID.Flare),
                BowChangeTheme.Deathwind => new BowChangePalette(Color.MediumPurple, Color.Cyan, Color.HotPink, DustID.PurpleTorch, DustID.BlueTorch),
                BowChangeTheme.Phangasm => new BowChangePalette(Color.Cyan, Color.GreenYellow, Color.Gold, DustID.Vortex, DustID.FireworkFountain_Red),
                BowChangeTheme.Ultima => new BowChangePalette(Color.Gold, Color.White, Color.Cyan, DustID.GoldFlame, DustID.WhiteTorch),
                _ => new BowChangePalette(Color.White, Color.LightBlue, Color.Cyan, DustID.WhiteTorch, DustID.BlueTorch)
            };
        }

        public static Vector2 Forward(Projectile projectile)
        {
            if (projectile.velocity.LengthSquared() > 0.0001f)
                return projectile.velocity.SafeNormalize(Vector2.UnitX);

            return projectile.rotation.ToRotationVector2();
        }

        public static void SpawnCharge(Projectile projectile, Vector2 position, BowChangeTheme theme, float progress, float intensity = 1f)
        {
            if (Main.dedServ)
                return;

            BowChangePalette palette = Palette(theme);
            Vector2 forward = Forward(projectile);
            Vector2 normal = new Vector2(-forward.Y, forward.X);
            progress = MathHelper.Clamp(progress, 0f, 1f);
            intensity = MathHelper.Clamp(intensity, 0.2f, 3f);

            int dustCount = Math.Max(1, (int)MathF.Round(MathHelper.Lerp(2f, 6f, progress) * intensity));
            float radius = MathHelper.Lerp(34f, 12f, progress);
            float time = Main.GlobalTimeWrappedHourly * MathHelper.Lerp(3f, 7f, progress);

            for (int i = 0; i < dustCount; i++)
            {
                float phase = time + MathHelper.TwoPi * (i / (float)dustCount);
                Vector2 offset = forward * MathF.Cos(phase) * radius + normal * MathF.Sin(phase) * radius * 0.45f;
                Vector2 spawn = position + offset + Main.rand.NextVector2Circular(3f, 3f);
                Vector2 velocity = (-offset.SafeNormalize(forward) * MathHelper.Lerp(0.5f, 1.8f, progress)) + forward * (0.15f + progress * 0.5f);

                Dust dust = Dust.NewDustPerfect(
                    spawn,
                    Main.rand.NextBool(3) ? palette.DustB : palette.DustA,
                    velocity,
                    120,
                    Color.Lerp(palette.Primary, palette.Secondary, Main.rand.NextFloat(0.15f, 0.85f)),
                    Main.rand.NextFloat(0.75f, 1.1f + progress * 0.35f));
                dust.noGravity = true;
                dust.fadeIn = 0.45f + progress * 0.35f;
            }

            if (Main.rand.NextFloat() < 0.12f * intensity)
            {
                GeneralParticleHandler.SpawnParticle(new SparkParticle(
                    position + Main.rand.NextVector2Circular(5f, 5f),
                    forward.RotatedByRandom(0.45f) * Main.rand.NextFloat(0.4f, 1.3f),
                    false,
                    Main.rand.Next(14, 22),
                    0.8f + progress * 0.4f,
                    Color.Lerp(palette.Primary, palette.Accent, 0.45f)));
            }

            if (progress > 0.82f && Main.rand.NextFloat() < 0.08f * intensity)
            {
                GeneralParticleHandler.SpawnParticle(new GlowOrbParticle(
                    position + normal * Main.rand.NextFloat(-8f, 8f),
                    Vector2.Zero,
                    false,
                    8,
                    0.7f + progress * 0.35f,
                    Color.Lerp(palette.Secondary, Color.White, 0.25f),
                    true,
                    false,
                    true));
            }
        }

        public static void SpawnReadyBurst(Projectile projectile, Vector2 position, BowChangeTheme theme, float intensity = 1f)
        {
            if (Main.dedServ)
                return;

            BowChangePalette palette = Palette(theme);
            Vector2 forward = Forward(projectile);
            intensity = MathHelper.Clamp(intensity, 0.3f, 3f);

            int dustCount = Math.Max(16, (int)(58f * intensity));
            float arc = MathHelper.ToRadians(34f);
            for (int i = 0; i < dustCount; i++)
            {
                Vector2 direction = forward.RotatedBy(Main.rand.NextFloat(-arc, arc));
                Vector2 velocity = direction * Main.rand.NextFloat(4f, 10f) + Main.rand.NextVector2Circular(0.8f, 0.8f);
                Dust dust = Dust.NewDustPerfect(
                    position,
                    Main.rand.NextBool(3) ? palette.DustB : palette.DustA,
                    velocity,
                    100,
                    Color.Lerp(palette.Primary, palette.Secondary, Main.rand.NextFloat()),
                    Main.rand.NextFloat(1f, 1.45f));
                dust.noGravity = true;
            }

            GeneralParticleHandler.SpawnParticle(new DirectionalPulseRing(
                position,
                forward * 0.7f,
                Color.Lerp(palette.Secondary, Color.White, 0.35f),
                new Vector2(1f, 2.5f),
                forward.ToRotation() - MathHelper.PiOver4,
                0.18f * intensity,
                0.035f,
                22));

            int sparkleCount = Math.Max(3, (int)(8f * intensity));
            for (int i = 0; i < sparkleCount; i++)
            {
                Vector2 velocity = forward.RotatedByRandom(0.65f) * Main.rand.NextFloat(2f, 5f);
                GeneralParticleHandler.SpawnParticle(new GenericSparkle(
                    position + Main.rand.NextVector2Circular(8f, 8f),
                    velocity * 0.25f,
                    palette.Secondary,
                    palette.Accent,
                    Main.rand.NextFloat(1f, 1.55f),
                    8,
                    Main.rand.NextFloat(-0.04f, 0.04f),
                    1.45f));
            }
        }

        public static void SpawnMuzzle(Projectile projectile, Vector2 position, Vector2 velocity, BowChangeTheme theme, float intensity = 1f)
        {
            if (Main.dedServ)
                return;

            BowChangePalette palette = Palette(theme);
            Vector2 direction = velocity.SafeNormalize(Forward(projectile));
            intensity = MathHelper.Clamp(intensity, 0.3f, 3f);

            int dustCount = Math.Max(4, (int)(10f * intensity));
            for (int i = 0; i < dustCount; i++)
            {
                Vector2 dustVelocity = direction.RotatedByRandom(0.36f) * Main.rand.NextFloat(1.4f, 4f);
                Dust dust = Dust.NewDustPerfect(
                    position,
                    Main.rand.NextBool() ? palette.DustA : palette.DustB,
                    dustVelocity,
                    110,
                    Color.Lerp(palette.Primary, palette.Secondary, Main.rand.NextFloat(0.25f, 0.9f)),
                    Main.rand.NextFloat(0.85f, 1.25f));
                dust.noGravity = true;
            }

            GeneralParticleHandler.SpawnParticle(new AltSparkParticle(
                position + direction * 6f,
                direction * Main.rand.NextFloat(0.5f, 1.2f),
                false,
                10,
                1f + intensity * 0.18f,
                palette.Accent * 0.55f));
        }

        public static void SpawnTrail(Projectile projectile, BowChangeTheme theme, float intensity = 1f)
        {
            if (Main.dedServ)
                return;

            BowChangePalette palette = Palette(theme);
            intensity = MathHelper.Clamp(intensity, 0.25f, 2f);
            Vector2 velocity = -projectile.velocity.SafeNormalize(Vector2.UnitX) * Main.rand.NextFloat(0.6f, 1.8f);

            if (Main.rand.NextFloat() < 0.42f * intensity)
            {
                Dust dust = Dust.NewDustPerfect(
                    projectile.Center + Main.rand.NextVector2Circular(3f, 3f),
                    Main.rand.NextBool() ? palette.DustA : palette.DustB,
                    velocity + Main.rand.NextVector2Circular(0.35f, 0.35f),
                    120,
                    Color.Lerp(palette.Primary, palette.Secondary, Main.rand.NextFloat()),
                    Main.rand.NextFloat(0.65f, 1f));
                dust.noGravity = true;
            }

            if (Main.rand.NextFloat() < 0.09f * intensity)
            {
                GeneralParticleHandler.SpawnParticle(new SparkParticle(
                    projectile.Center - projectile.velocity.SafeNormalize(Vector2.Zero) * 6f,
                    projectile.velocity * 0.03f,
                    false,
                    12,
                    0.8f,
                    palette.Secondary * 0.8f));
            }
        }

        public static void SpawnImpact(Projectile projectile, BowChangeTheme theme, float intensity = 1f)
        {
            if (Main.dedServ)
                return;

            BowChangePalette palette = Palette(theme);
            intensity = MathHelper.Clamp(intensity, 0.3f, 3f);
            int dustCount = Math.Max(8, (int)(18f * intensity));

            for (int i = 0; i < dustCount; i++)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(1.5f, 5.5f);
                Dust dust = Dust.NewDustPerfect(
                    projectile.Center,
                    Main.rand.NextBool() ? palette.DustA : palette.DustB,
                    velocity,
                    110,
                    Color.Lerp(palette.Primary, palette.Secondary, Main.rand.NextFloat()),
                    Main.rand.NextFloat(0.9f, 1.45f));
                dust.noGravity = true;
            }

            GeneralParticleHandler.SpawnParticle(new DirectionalPulseRing(
                projectile.Center,
                Vector2.Zero,
                Color.Lerp(palette.Primary, Color.White, 0.35f),
                new Vector2(1f, 1.55f),
                Main.rand.NextFloat(MathHelper.TwoPi),
                0.14f * intensity,
                0.035f,
                18));
        }
    }
}
