using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityRangerExpansion.Content.BOWChange.EAfterDog.DeathwindC
{
    internal class DeathwindReColorPrism : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.EAfterDog";
        public override string Texture => "CalamityMod/Projectiles/LaserProj";

        private bool IsBlue => Projectile.ai[0] < 0.5f;
        private int TargetIndex => (int)Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.width = 96;
            Projectile.height = 96;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 76;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override bool? CanDamage() => Projectile.localAI[0] % 16f <= 2f;

        public override bool PreDraw(ref Color lightColor) => false;

        public override void AI()
        {
            Projectile.localAI[0]++;
            Color prismColor = IsBlue ? Color.Cyan : Color.MediumPurple;
            Lighting.AddLight(Projectile.Center, prismColor.ToVector3() * 0.75f);

            if (TargetIndex >= 0 && TargetIndex < Main.maxNPCs)
            {
                NPC target = Main.npc[TargetIndex];
                if (target.active && target.CanBeChasedBy(Projectile))
                {
                    Vector2 desired = target.Center + new Vector2(0f, -44f).RotatedBy(Projectile.localAI[0] * 0.05f);
                    Projectile.Center = Vector2.Lerp(Projectile.Center, desired, IsBlue ? 0.08f : 0.05f);
                }
            }

            if (Projectile.owner == Main.myPlayer && Projectile.localAI[0] % 16f == 0f)
                Projectile.Damage();

            if (Projectile.owner == Main.myPlayer && Projectile.localAI[0] == 34f)
                FirePrismPattern();

            if (Main.dedServ)
                return;

            int dustType = IsBlue ? DustID.Electric : DustID.PurpleTorch;
            for (int i = 0; i < 4; i++)
            {
                float angle = Projectile.localAI[0] * (IsBlue ? 0.18f : -0.14f) + MathHelper.TwoPi * i / 4f;
                Vector2 offset = angle.ToRotationVector2() * (22f + i * 8f);
                Dust dust = Dust.NewDustPerfect(
                    Projectile.Center + offset,
                    dustType,
                    offset.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero) * 1.1f,
                    80,
                    prismColor,
                    Main.rand.NextFloat(1f, 1.55f));
                dust.noGravity = true;
            }

            if (Projectile.localAI[0] % 12f == 0f)
            {
                GeneralParticleHandler.SpawnParticle(new CritSpark(
                    Projectile.Center,
                    Main.rand.NextVector2CircularEdge(1f, 1f) * 2f,
                    IsBlue ? Color.White : Color.HotPink,
                    prismColor,
                    1.1f,
                    18));
            }
        }

        private void FirePrismPattern()
        {
            int beams = IsBlue ? 5 : 3;
            float baseAngle = Main.rand.NextFloat(MathHelper.TwoPi);
            for (int i = 0; i < beams; i++)
            {
                float angle = baseAngle + MathHelper.TwoPi * i / beams;
                Vector2 spawn = Projectile.Center + angle.ToRotationVector2() * (IsBlue ? 460f : 180f);
                Vector2 velocity = IsBlue
                    ? (Projectile.Center - spawn).SafeNormalize(Vector2.UnitY) * 28f
                    : angle.ToRotationVector2() * 24f;

                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    spawn,
                    velocity,
                    ModContent.ProjectileType<DeathwindCJB>(),
                    (int)(Projectile.damage * (IsBlue ? 0.42f : 0.6f)),
                    Projectile.knockBack,
                    Projectile.owner);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (IsBlue)
                target.AddBuff(BuffID.Electrified, 80);
            else
                target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 100);
        }
    }
}
