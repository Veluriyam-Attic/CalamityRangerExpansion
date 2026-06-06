using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.CPreMoodLord.VernalBolterC
{
    internal class VernalBolterReBrambleSeed : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.CPreMoodLord";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
        }

        public override bool? CanDamage() => Projectile.localAI[0] > 12f;

        public override void AI()
        {
            Projectile.localAI[0]++;
            Projectile.rotation += 0.18f;

            NPC target = Projectile.Center.ClosestNPCAt(740f);
            if (target != null)
            {
                Vector2 desired = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitY) * MathHelper.Lerp(6f, 12f, Utils.GetLerpValue(12f, 70f, Projectile.localAI[0], true));
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desired, 0.075f);
            }

            if (Projectile.localAI[0] % 5f == 0f)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.TerraBlade, -Projectile.velocity * 0.12f, 110, Color.GreenYellow, 1f);
                d.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Poisoned, 180);
            BowChangeVFX.SpawnImpact(Projectile, BowChangeTheme.Vernal, 0.8f);
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<VernalBolterReBloom>(), Math.Max(1, Projectile.damage / 2), 0f, Projectile.owner);
            }
        }
    }

    internal class VernalBolterReBloom : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.CPreMoodLord";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 118;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 45;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override bool? CanDamage() => Projectile.localAI[0] % 15f <= 2f;

        public override void AI()
        {
            Projectile.localAI[0]++;
            if (Projectile.localAI[0] % 15f == 1f)
                Projectile.Damage();

            int petals = 12;
            for (int i = 0; i < petals; i++)
            {
                float angle = Projectile.rotation + MathHelper.TwoPi * i / petals;
                Vector2 pos = Projectile.Center + angle.ToRotationVector2() * Main.rand.NextFloat(18f, 56f);
                Dust d = Dust.NewDustPerfect(pos, DustID.Grass, angle.ToRotationVector2() * 0.8f, 100, Color.SpringGreen, 0.9f);
                d.noGravity = true;
            }
            Projectile.rotation += 0.08f;
        }
    }
}
