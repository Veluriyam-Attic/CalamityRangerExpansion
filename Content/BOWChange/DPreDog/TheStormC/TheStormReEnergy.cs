using CalamityMod;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.Buffs.StatDebuffs;
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.DPreDog.TheStormC
{
    internal class TheStormReEnergy : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.DPreDog";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.alpha = 255;
            Projectile.penetrate = 10;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Projectile.velocity *= 1.015f;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            BowChangeVFX.SpawnTrail(Projectile, BowChangeTheme.Storm, 0.65f);
            if (Projectile.timeLeft % 30 == 0)
            {
                for (int l = 0; l < 12; l++)
                {
                    Vector2 rotate = Vector2.UnitX * (float)-(float)Projectile.width / 2f;
                    rotate += -Vector2.UnitY.RotatedBy((double)((float)l * 3.14159274f / 6f), default) * new Vector2(8f, 16f);
                    rotate = rotate.RotatedBy((double)(Projectile.rotation - 1.57079637f), default);
                    int blueDust = Dust.NewDust(Projectile.Center, 0, 0, DustID.FireworkFountain_Blue, 0f, 0f, 160, default, 1f);
                    Main.dust[blueDust].scale = 1.1f;
                    Main.dust[blueDust].noGravity = true;
                    Main.dust[blueDust].position = Projectile.Center + rotate;
                    Main.dust[blueDust].velocity = Projectile.velocity * 0.1f;
                    Main.dust[blueDust].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[blueDust].position) * 1.25f;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.timeLeft < 85)
            {
                byte b2 = (byte)(Projectile.timeLeft * 3);
                byte a2 = (byte)(100f * ((float)b2 / 255f));
                return new Color((int)b2, (int)b2, (int)b2, (int)a2);
            }
            return new Color(255, 255, 255, 100);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Electrified, 180);
            target.AddBuff(ModContent.BuffType<GalvanicCorrosion>(), 6);
            BowChangeVFX.SpawnImpact(Projectile, BowChangeTheme.Storm, 0.8f);

            if (Projectile.owner == Main.myPlayer && Main.rand.NextBool(3))
            {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    target.Center,
                    Projectile.velocity.SafeNormalize(Vector2.UnitY) * 1.5f,
                    ModContent.ProjectileType<TheStormReStaticField>(),
                    (int)(Projectile.damage * 0.45f),
                    Projectile.knockBack,
                    Projectile.owner);
            }
        }
        public override void OnKill(int timeLeft)
        {
            BowChangeVFX.SpawnImpact(Projectile, BowChangeTheme.Storm, 0.65f);
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 32;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
            int rando = Main.rand.Next(4, 8);
            for (int i = 0; i < rando; i++)
            {
                int dust = Dust.NewDust(Projectile.Center - Projectile.velocity / 2f, 0, 0, DustID.IceTorch, 0f, 0f, 100, default, 2f);
                Main.dust[dust].velocity *= 2f;
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
