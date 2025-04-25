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
using CalamityRangerExpansion.CREConfigs;

namespace CalamityRangerExpansion.Content.Gel.DPreDog.BloodstoneCoreGel
{
    internal class BloodstoneCoreGelGP : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool IsBloodstoneCoreGelInfused = false;

        // 添加全局冷却计时器
        private static int globalHealCooldown = 0;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_ItemUse_WithAmmo ammoSource && ammoSource.AmmoItemIdUsed == ModContent.ItemType<BloodstoneCoreGel>())
            {
                IsBloodstoneCoreGelInfused = true;
                projectile.netUpdate = true;
                projectile.damage = (int)(projectile.damage * 0.65f); // 减少 85% 伤害
            }
            base.OnSpawn(projectile, source);
        }

        public override void AI(Projectile projectile)
        {
            // 更新全局冷却计时器
            if (globalHealCooldown > 0)
            {
                globalHealCooldown--;
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (IsBloodstoneCoreGelInfused && target.active && !target.friendly)
            {
                // 如果全局冷却未结束，则跳过回血逻辑
                if (globalHealCooldown > 0)
                {
                    return;
                }

                // 根据伤害计算回血量
                int healAmount = Main.rand.Next((int)(damageDone * 0.01f), (int)(damageDone * 0.05f) + 1);

                // 恢复所有玩家的血量
                foreach (Player player in Main.player)
                {
                    if (player.active)
                    {
                        player.statLife += healAmount;
                        player.HealEffect(healAmount);
                    }
                }

                // 施加全局冷却：30 帧（0.5 秒）
                globalHealCooldown = 30;

                // 施加 BurningBlood Buff，持续 10 秒（600 帧）
                target.AddBuff(ModContent.BuffType<BurningBlood>(), 600);

                // 检查是否启用了特效
                if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
                {
                    // 在原地生成血爆特效
                    Particle bloodsplosion2 = new CustomPulse(
                        projectile.Center,
                        Vector2.Zero,
                        new Color(255, 32, 32),
                        "CalamityMod/Particles/DustyCircleHardEdge",
                        Vector2.One * 0.5f, // 缩小至原大小的 50%
                        Main.rand.NextFloat(-15f, 15f),
                        0.03f,
                        0.155f,
                        40
                    );
                    GeneralParticleHandler.SpawnParticle(bloodsplosion2);
                }
            }
        }
    }

}
