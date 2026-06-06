using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.DPreDog.NettlevineGreatbowC
{
    internal class NettlevineGreatbowReRootSnare : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.DPreDog";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 96;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 24;
        }

        public override bool? CanDamage() => Projectile.localAI[0] % 24f <= 3f;

        public override void AI()
        {
            Projectile.localAI[0]++;
            Projectile.rotation += 0.035f;
            if (Projectile.localAI[0] % 24f == 1f)
                Projectile.Damage();

            for (int i = 0; i < 6; i++)
            {
                float angle = Projectile.rotation + MathHelper.TwoPi * i / 6f;
                Vector2 root = Projectile.Center + angle.ToRotationVector2() * Main.rand.NextFloat(18f, 48f);
                Dust d = Dust.NewDustPerfect(root, DustID.JungleGrass, -angle.ToRotationVector2() * 0.6f, 110, Color.ForestGreen, Main.rand.NextFloat(0.9f, 1.4f));
                d.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Poisoned, 180);
            target.velocity *= 0.92f;
        }
    }
}
