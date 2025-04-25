using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Particles;

namespace CalamityRangerExpansion.Content.Gel.BPrePlantera.StarblightSootGel
{
    internal class StarblightSootGelGP : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool IsStarblightSootGelInfused = false;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_ItemUse_WithAmmo ammoSource && ammoSource.AmmoItemIdUsed == ModContent.ItemType<StarblightSootGel>())
            {
                IsStarblightSootGelInfused = true;
                projectile.netUpdate = true;
            }
            base.OnSpawn(projectile, source);
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (IsStarblightSootGelInfused && target.active && !target.friendly)
            {
                // 施加 AstralInfectionDebuff，持续 300 帧（5 秒）
                target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 300);

                // 通知 StarblightSootGelPlayer
                Main.player[projectile.owner].GetModPlayer<StarblightSootGelPlayer>().NotifyHit();

                // 释放五角星特效
                for (int i = 0; i < 5; i++)
                {
                    float angle = MathHelper.ToRadians(360f / 5 * i);
                    Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 5f;
                    PointParticle spark = new PointParticle(
                        projectile.Center,
                        direction,
                        false,
                        15,
                        1.1f,
                        Color.LightBlue
                    );
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }
        }
    }
}

