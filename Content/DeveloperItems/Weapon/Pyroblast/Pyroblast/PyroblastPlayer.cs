using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.Pyroblast
{
    public class PyroblastPlayer : ModPlayer
    {
        public override void PostUpdate()
        {
            // 检查玩家是否手持 Pyroblast 武器
            if (Player.HeldItem.type != ModContent.ItemType<Pyroblast>())
            {
                // 获取玩家当前持有的 Pyroblast 弹幕
                var pyroblastProjectile = Player.heldProj != -1 ? Main.projectile[Player.heldProj].ModProjectile as PyroblastHoldOut : null;

                // 如果弹幕存在，重置等级到 1
                if (pyroblastProjectile != null)
                {
                    pyroblastProjectile.upgradeLevel = 1;
                }
            }
        }
    }
}
