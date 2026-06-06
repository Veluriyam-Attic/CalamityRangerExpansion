using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityRangerExpansion.Content.BOWChange.DPreDog.TheStormC
{
    internal class TheStormReStaticField : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.DPreDog";
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.Electrosphere}";

        public override void SetDefaults()
        {
            Projectile.width = 132;
            Projectile.height = 132;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 72;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override bool? CanDamage() => Projectile.localAI[0] % 12f <= 2f;

        public override bool PreDraw(ref Color lightColor) => false;

        public override void AI()
        {
            Projectile.localAI[0]++;
            Projectile.velocity *= 0.94f;
            float pulse = 0.5f + 0.5f * (float)System.Math.Sin(Projectile.localAI[0] * 0.27f);
            Lighting.AddLight(Projectile.Center, Color.Lerp(Color.Cyan, Color.White, pulse).ToVector3() * 0.55f);

            if (Projectile.owner == Main.myPlayer && Projectile.localAI[0] % 12f == 0f)
                Projectile.Damage();

            if (Main.dedServ)
                return;

            if (Projectile.localAI[0] % 6f == 0f)
            {
                for (int i = 0; i < 10; i++)
                {
                    Vector2 edge = Main.rand.NextVector2CircularEdge(66f, 34f).RotatedBy(Projectile.localAI[0] * 0.05f);
                    Vector2 velocity = -edge.SafeNormalize(Vector2.UnitY) * Main.rand.NextFloat(1.2f, 3.8f);
                    Dust dust = Dust.NewDustPerfect(
                        Projectile.Center + edge,
                        Main.rand.NextBool() ? DustID.Electric : DustID.Cloud,
                        velocity,
                        90,
                        Color.Lerp(Color.White, Color.Cyan, Main.rand.NextFloat()),
                        Main.rand.NextFloat(1f, 1.55f));
                    dust.noGravity = true;
                }
            }

            if (Projectile.localAI[0] % 18f == 0f)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 start = Projectile.Center + Main.rand.NextVector2CircularEdge(68f, 68f);
                    Vector2 end = Projectile.Center + Main.rand.NextVector2Circular(22f, 22f);
                    Vector2 step = (end - start) / 5f;
                    for (int j = 0; j < 5; j++)
                    {
                        Dust arc = Dust.NewDustPerfect(
                            start + step * j + Main.rand.NextVector2Circular(5f, 5f),
                            DustID.Electric,
                            step.SafeNormalize(Vector2.Zero) * 1.4f,
                            80,
                            Color.White,
                            1.35f);
                        arc.noGravity = true;
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Electrified, 120);
            target.AddBuff(ModContent.BuffType<GalvanicCorrosion>(), 30);
        }
    }
}
