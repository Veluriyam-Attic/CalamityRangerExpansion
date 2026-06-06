using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace CalamityRangerExpansion.Content.BOWChange
{
    public static class CCBLightingBoltsSystem
    {
        public static void Spawn_DaemonsFlameBurst(Vector2 position)
        {
            for (int i = 0; i < 18; i++)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(1.5f, 6f);
                GeneralParticleHandler.SpawnParticle(new AltSparkParticle(position, velocity, false, 18, 1.2f, Color.HotPink * 0.45f));

                Dust dust = Dust.NewDustPerfect(position, Main.rand.NextBool() ? DustID.PinkTorch : DustID.Shadowflame, velocity * 0.7f, 100, Color.HotPink, Main.rand.NextFloat(1f, 1.5f));
                dust.noGravity = true;
            }
        }

        public static void Phangasm_TrueArrowHitEffect(Vector2 position, Vector2 shootDirection)
        {
            Vector2 direction = shootDirection.SafeNormalize(Vector2.UnitX);
            for (int i = 0; i < 16; i++)
            {
                Vector2 velocity = direction.RotatedByRandom(MathHelper.ToRadians(28f)) * Main.rand.NextFloat(2f, 7f);
                GeneralParticleHandler.SpawnParticle(new SparkParticle(position, velocity, false, 32, 1.1f, Color.Cyan));

                Dust dust = Dust.NewDustPerfect(position, DustID.TintableDustLighted, velocity * 0.5f, 100, Color.Lerp(Color.Cyan, Color.Teal, Main.rand.NextFloat()), Main.rand.NextFloat(0.8f, 1.2f));
                dust.noGravity = true;
            }
        }

        public static void Phangasm_IllusionArrowHitEffect(Vector2 position)
        {
            for (int i = 0; i < 20; i++)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(1f, 5f);
                Color color = Color.Lerp(Color.LightBlue, Color.LightGreen, Main.rand.NextFloat());
                GeneralParticleHandler.SpawnParticle(new GenericSparkle(position, velocity * 0.2f, color, Color.White, Main.rand.NextFloat(1.1f, 1.8f), 6, Main.rand.NextFloat(-0.02f, 0.02f), 1.4f));

                Dust dust = Dust.NewDustPerfect(position, DustID.TintableDustLighted, velocity, 100, color, Main.rand.NextFloat(0.7f, 1.1f));
                dust.noGravity = true;
            }
        }
    }
}
