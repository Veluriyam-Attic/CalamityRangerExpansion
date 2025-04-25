using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CalamityRangerExpansion.Content.WeaponToAMMO.Bullet.NorthStar
{
    public class PPPlayer : ModPlayer
    {
        public bool polarisBoost = false;
        public bool polarisBoostTwo = false;
        public bool polarisBoostThree = false;
        public int polarisBoostCounter = 0; // 计数器，用于追踪击中次数
        private int lastHitTime = 0; // 上一次击中时间的计时器（以帧为单位）

        public override void ResetEffects()
        {
            //// 检查玩家是否手持PolarisParrotfish，只有在这种情况下才启用强化逻辑
            //if (Player.HeldItem.type == ModContent.ItemType<NorthStar>())
            //{
            //    // 保持状态
            //    polarisBoost = true;
            //}
            //else
            //{
            //    // 重置状态和计数器
            //    polarisBoost = false;
            //    polarisBoostTwo = false;
            //    polarisBoostThree = false;
            //    polarisBoostCounter = 0;
            //}

            // 定时清空等级逻辑
            if (Main.GameUpdateCount - lastHitTime > 600) // 超过 600 帧（10 秒）
            {
                ResetBoostLevels(); // 重置所有强化等级
            }
        }
        public void ResetBoostLevels()
        {
            polarisBoost = false;
            polarisBoostTwo = false;
            polarisBoostThree = false;
            polarisBoostCounter = 0; // 重置计数器
        }
        public void IncreaseBoostLevel()
        {
            polarisBoostCounter++;
            lastHitTime = (int)Main.GameUpdateCount; // 更新上次击中时间

            if (polarisBoostCounter >= 50) // 每 50 次击中敌人，提升一个等级
            {
                polarisBoostCounter = 0; // 计数器清零

                // 升级逻辑，等级提升是逐级存在的
                if (!polarisBoostTwo)
                {
                    polarisBoostTwo = true;
                }
                else if (!polarisBoostThree)
                {
                    polarisBoostThree = true;
                }
            }
        }

        public override void OnHurt(Player.HurtInfo hurtInfo)
        {
            if (polarisBoost)
            {
                // 受到伤害时降低等级
                polarisBoostCounter = 0; // 清零计数器
                if (polarisBoostThree)
                {
                    polarisBoostThree = false; // 降低到二级
                }
                else if (polarisBoostTwo)
                {
                    polarisBoostTwo = false; // 降低到一级
                }
            }
        }
    }
}
