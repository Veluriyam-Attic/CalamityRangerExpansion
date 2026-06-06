using CalamityMod.Particles;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.CPreMoodLord.TheBallistaC
{
    internal class TheBallistaReSiegeCrater : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.CPreMoodLord";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 140;
            Projectile.height = 58;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 54;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 18;
        }

        public override bool? CanDamage() => Projectile.localAI[0] > 4f && Projectile.localAI[0] % 18f <= 2f;

        public override void AI()
        {
            Projectile.localAI[0]++;
            Projectile.velocity *= 0.84f;
            Lighting.AddLight(Projectile.Center, Color.Gold.ToVector3() * 0.28f);

            for (int i = 0; i < 10; i++)
            {
                Vector2 offset = new Vector2(Main.rand.NextFloat(-Projectile.width * 0.45f, Projectile.width * 0.45f), Main.rand.NextFloat(-Projectile.height * 0.35f, Projectile.height * 0.35f));
                Dust d = Dust.NewDustPerfect(Projectile.Center + offset, DustID.Dirt, new Vector2(Main.rand.NextFloat(-0.7f, 0.7f), -Main.rand.NextFloat(0.4f, 1.8f)), 100, Color.SandyBrown, Main.rand.NextFloat(0.9f, 1.6f));
                d.noGravity = true;
            }

            if (Projectile.localAI[0] % 18f == 1f)
            {
                Projectile.Damage();
                if (Projectile.owner == Main.myPlayer)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Vector2 vel = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(5f, 8f);
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, vel, ModContent.ProjectileType<FossilShard>(), Math.Max(1, Projectile.damage / 3), Projectile.knockBack * 0.35f, Projectile.owner);
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Crumbling>(), 180);
        }
    }
}
