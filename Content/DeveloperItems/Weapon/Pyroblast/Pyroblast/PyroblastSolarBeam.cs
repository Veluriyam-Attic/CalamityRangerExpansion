using CalamityMod;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.Pyroblast
{
    public class PyroblastSolarBeam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.Pyroblast";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public static bool IsEnhanced = false; // 是否被强化

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = IsEnhanced ? 15 : 1; // 判断是否强化
            Projectile.extraUpdates = 100;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;
        }

        public override void AI()
        {
            Projectile.localAI[0] += 1f;

            // 释放金色粒子特效
            if (IsEnhanced && Projectile.localAI[0] % 3 == 0)
            {
                Vector2 particleDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);
                PointParticle spark = new PointParticle(
                    Projectile.Center,
                    particleDirection * 5f, // 固定方向与速度
                    false,
                    20,
                    1.3f,
                    Color.Gold // 颜色改为金色
                );
                GeneralParticleHandler.SpawnParticle(spark);
            }

            if (Projectile.localAI[0] > 4f)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 projPos = Projectile.position;
                    projPos -= Projectile.velocity * (i * 0.25f);
                    Projectile.alpha = 255;
                    int heatGold = Dust.NewDust(projPos, 1, 1, DustID.GoldCoin, 0f, 0f, 0, default, 1f);
                    Main.dust[heatGold].position = projPos;
                    Main.dust[heatGold].scale = Main.rand.Next(70, 110) * 0.013f;
                    Main.dust[heatGold].velocity *= 0.2f;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Daybreak, 180);
            if (Projectile.owner == Main.myPlayer)
            {
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FuckYou>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack, Projectile.owner, 0f, 0.85f + Main.rand.NextFloat() * 1.15f);
                if (proj.WithinBounds(Main.maxProjectiles))
                    Main.projectile[proj].DamageType = DamageClass.Magic;
            }
        }
    }
}
