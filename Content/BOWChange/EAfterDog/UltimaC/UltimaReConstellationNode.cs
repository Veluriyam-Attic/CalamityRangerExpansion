using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityRangerExpansion.Content.BOWChange.EAfterDog.UltimaC
{
    internal class UltimaReConstellationNode : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.EAfterDog";
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.StarWrath}";

        private int Phase => (int)MathHelper.Clamp(Projectile.ai[0], 1f, 3f);

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 150;
            Projectile.alpha = 255;
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public override void AI()
        {
            Projectile.localAI[0]++;
            Projectile.velocity *= 0.965f;
            Projectile.rotation += 0.08f * Phase;
            Lighting.AddLight(Projectile.Center, Color.Lerp(Color.Gold, Color.Cyan, Phase / 3f).ToVector3() * 0.8f);

            int fireRate = Phase switch
            {
                1 => 28,
                2 => 22,
                _ => 16
            };

            if (Projectile.owner == Main.myPlayer && Projectile.localAI[0] % fireRate == 0f)
                FireConstellationVolley();

            if (Main.dedServ)
                return;

            int count = 5 + Phase * 2;
            for (int i = 0; i < count; i++)
            {
                float angle = Projectile.rotation + MathHelper.TwoPi * i / count;
                Vector2 offset = angle.ToRotationVector2() * (22f + Phase * 8f);
                Dust dust = Dust.NewDustPerfect(
                    Projectile.Center + offset,
                    Main.rand.NextBool() ? DustID.GoldFlame : DustID.WhiteTorch,
                    offset.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero) * 0.8f,
                    100,
                    Color.Lerp(Color.Gold, Color.White, Main.rand.NextFloat()),
                    Main.rand.NextFloat(0.75f, 1.2f));
                dust.noGravity = true;
            }

            if (Phase >= 3 && Projectile.localAI[0] % 10f == 0f)
                BowChangeVFX.SpawnCharge(Projectile, Projectile.Center, BowChangeTheme.Ultima, 1f, 0.65f);
        }

        private void FireConstellationVolley()
        {
            NPC target = Projectile.Center.ClosestNPCAt(1800f);
            Vector2 direction = target == null
                ? Projectile.velocity.SafeNormalize(Vector2.UnitY)
                : (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitY);

            int shots = Phase switch
            {
                1 => 1,
                2 => 3,
                _ => 5
            };

            for (int i = 0; i < shots; i++)
            {
                float spread = shots == 1 ? 0f : MathHelper.Lerp(-0.26f, 0.26f, i / (float)(shots - 1));
                int type = Phase switch
                {
                    1 => ModContent.ProjectileType<AstralStar>(),
                    2 => i == 1 ? ModContent.ProjectileType<UltimaSpark>() : ModContent.ProjectileType<StarfleetStar>(),
                    _ => i == shots / 2 ? ModContent.ProjectileType<UltimaRay>() : ModContent.ProjectileType<UltimaSpark>()
                };

                float speed = type == ModContent.ProjectileType<UltimaRay>() ? 14f : 28f;
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center + direction.RotatedBy(MathHelper.PiOver2) * (i - shots / 2f) * 12f,
                    direction.RotatedBy(spread) * speed,
                    type,
                    (int)(Projectile.damage * (Phase >= 3 ? 0.55f : 0.38f)),
                    Projectile.knockBack,
                    Projectile.owner);
            }

            BowChangeVFX.SpawnMuzzle(Projectile, Projectile.Center, direction * 20f, BowChangeTheme.Ultima, 0.65f + Phase * 0.15f);
        }
    }
}
