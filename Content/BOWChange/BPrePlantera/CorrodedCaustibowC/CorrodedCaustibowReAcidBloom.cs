using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.BPrePlantera.CorrodedCaustibowC
{
    internal class CorrodedCaustibowReAcidBloom : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.BPrePlantera";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 96;
            Projectile.height = 42;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 84;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 18;
        }

        public override bool? CanDamage() => Projectile.localAI[0] > 6f && Projectile.localAI[0] % 14f <= 2f;

        public override void AI()
        {
            Projectile.localAI[0]++;
            float squeeze = 0.75f + 0.25f * (float)System.Math.Sin(Projectile.localAI[0] * 0.22f);
            Lighting.AddLight(Projectile.Center, new Color(80, 255, 120).ToVector3() * 0.35f);

            for (int i = 0; i < 7; i++)
            {
                Vector2 offset = new Vector2(Main.rand.NextFloat(-Projectile.width * 0.5f, Projectile.width * 0.5f), Main.rand.NextFloat(-Projectile.height * 0.45f, Projectile.height * 0.45f) * squeeze);
                Dust d = Dust.NewDustPerfect(
                    Projectile.Center + offset,
                    Main.rand.NextBool() ? DustID.Venom : DustID.Demonite,
                    new Vector2(0f, -Main.rand.NextFloat(0.4f, 1.5f)),
                    110,
                    Color.Lerp(Color.LimeGreen, Color.Purple, Main.rand.NextFloat(0.2f, 0.75f)),
                    Main.rand.NextFloat(0.9f, 1.55f));
                d.noGravity = true;
            }

            if (Projectile.localAI[0] % 14f == 1f)
            {
                Projectile.Damage();
                if (Projectile.owner == Main.myPlayer)
                {
                    Vector2 vel = new Vector2(Main.rand.NextFloat(-2.2f, 2.2f), Main.rand.NextFloat(-5.5f, -3f));
                    Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        Projectile.Center + Main.rand.NextVector2Circular(34f, 12f),
                        vel,
                        ModContent.ProjectileType<CorrodedCaustibowReASpark>(),
                        Math.Max(1, Projectile.damage / 3),
                        0f,
                        Projectile.owner);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Venom, 150);
            BowChangeVFX.SpawnImpact(Projectile, BowChangeTheme.Caustic, 0.55f);
        }
    }
}
