using CalamityRangerExpansion.Content.Gel.CPreMoodLord.AstralGel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using CalamityMod.Projectiles.Ranged;
using CalamityRangerExpansion.Content.Gel.CPreMoodLord.LivingShardGel;

namespace CalamityRangerExpansion.Content.Gel.APreHardMode.HurricaneGel
{
    public class HurricaneGelGP : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool IsHurricaneGelInfused = false;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_ItemUse_WithAmmo ammoSource && ammoSource.AmmoItemIdUsed == ModContent.ItemType<HurricaneGel>())
            {
                IsHurricaneGelInfused = true;
                projectile.netUpdate = true;
            }
            base.OnSpawn(projectile, source);
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (IsHurricaneGelInfused && target.active && !target.friendly)
            {
                // 检查场上是否已有超过 8 个 某种 弹幕
                int sparkCount = 0;
                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.active && proj.type == ModContent.ProjectileType<HurricaneGelSplit>())
                    {
                        sparkCount++;
                        if (sparkCount >= 8)
                            return; // 如果已存在 8 个 某种 弹幕，则不释放新的
                    }
                }

                // 随机生成 1-2 个额外弹幕
                int extraProjectiles = Main.rand.Next(1, 3);
                for (int i = 0; i < extraProjectiles; i++)
                {
                    // 随机生成 360 度方向
                    float randomAngle = Main.rand.NextFloat(0, MathHelper.TwoPi); // 随机角度（弧度制）
                    Vector2 velocity = randomAngle.ToRotationVector2() * 6f; // 固定初速度为 6f

                    Projectile.NewProjectile(
                        projectile.GetSource_FromThis(),
                        projectile.Center,
                        velocity,
                        ModContent.ProjectileType<HurricaneGelSplit>(),
                        (int)(projectile.damage * 0.1f), // 伤害为原来的 35%
                        projectile.knockBack,
                        projectile.owner
                    );
                }
            }
        }
    }
}