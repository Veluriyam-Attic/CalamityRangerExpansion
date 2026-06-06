using CalamityMod.Particles;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityRangerExpansion.Content.BOWChange.EAfterDog.CondemnationC
{
    internal class CondemnationReJudgementBrand : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.EAfterDog";
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.DD2ExplosiveTrapT3Explosion}";

        private int Stage => (int)MathHelper.Clamp(Projectile.ai[0], 0f, 5f);
        private int TargetIndex => (int)Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.width = 112;
            Projectile.height = 112;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 94;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
        }

        public override bool? CanDamage() => Projectile.localAI[0] % PulseRate <= 3f;

        private int PulseRate => System.Math.Max(8, 18 - Stage * 2);

        public override bool PreDraw(ref Color lightColor) => false;

        public override void AI()
        {
            Projectile.localAI[0]++;
            Projectile.width = Projectile.height = 112 + Stage * 18;
            Projectile.position = Projectile.Center - Projectile.Size * 0.5f;

            if (TargetIndex >= 0 && TargetIndex < Main.maxNPCs)
            {
                NPC target = Main.npc[TargetIndex];
                if (target.active && !target.friendly && target.life > 0)
                    Projectile.Center = Vector2.Lerp(Projectile.Center, target.Center, 0.38f);
            }

            Lighting.AddLight(Projectile.Center, Color.Lerp(Color.OrangeRed, Color.Gold, Stage / 5f).ToVector3() * 0.85f);

            if (Projectile.owner == Main.myPlayer && Projectile.localAI[0] % PulseRate == 0f)
                Projectile.Damage();

            if (Projectile.owner == Main.myPlayer && Stage >= 4 && Projectile.localAI[0] == 54f)
            {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<FuckYou>(),
                    (int)(Projectile.damage * (Stage >= 5 ? 1.35f : 0.85f)),
                    0f,
                    Projectile.owner,
                    0.75f + Stage * 0.1f);
            }

            if (Main.dedServ)
                return;

            if (Projectile.localAI[0] % 6f == 0f)
            {
                int count = 12 + Stage * 4;
                float rotation = Projectile.localAI[0] * 0.07f;
                for (int i = 0; i < count; i++)
                {
                    float angle = rotation + MathHelper.TwoPi * i / count;
                    Vector2 offset = angle.ToRotationVector2() * (32f + Stage * 7f);
                    Dust dust = Dust.NewDustPerfect(
                        Projectile.Center + offset,
                        Main.rand.NextBool(3) ? DustID.GoldFlame : DustID.Torch,
                        offset.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero) * 1.4f,
                        90,
                        Color.Lerp(Color.OrangeRed, Color.Gold, Main.rand.NextFloat()),
                        Main.rand.NextFloat(1f, 1.45f));
                    dust.noGravity = true;
                }
            }

            if (Projectile.localAI[0] % PulseRate == 0f)
            {
                GeneralParticleHandler.SpawnParticle(new GenericSparkle(
                    Projectile.Center,
                    Vector2.Zero,
                    Color.Gold,
                    Color.OrangeRed,
                    1.2f + Stage * 0.18f,
                    6,
                    Main.rand.NextFloat(-0.03f, 0.03f),
                    1.8f));
                SoundEngine.PlaySound(SoundID.Item74 with { Volume = 0.35f, Pitch = 0.15f }, Projectile.Center);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 120 + Stage * 20);
        }
    }
}
