using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using CalamityMod;
using Terraria.ID;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.NPCs.ExoMechs; // 引用 CalamityMod

namespace CalamityRangerExpansion.Content.Arrows.EAfterDog.MiracleMatterArrow
{
    public class MiracleMatterArrowPlayer : ModPlayer
    {        
        // 玩家是否启用了 MiracleMatterArrow 的附魔效果
        public bool IsMiracleMatterArrowActive = false;
        // 附魔效果剩余持续时间（以帧为单位，5秒 = 300帧）
        private int MiracleMatterArrowTimer = 0;
        public override void PostUpdate()
        {
            // 如果附魔计时器大于 0，递减计时器
            if (MiracleMatterArrowTimer > 0)
            {
                MiracleMatterArrowTimer--;

                // 如果计时器归零，关闭附魔状态
                if (MiracleMatterArrowTimer == 0)
                {
                    IsMiracleMatterArrowActive = false;
                }
            }
            // 如果至尊灾厄在这个世界上还没有被击败
            if (!DownedBossSystem.downedCalamitas)
            {
                // 获取玩家当前持有的武器和箭矢
                Item heldItem = Player.HeldItem;

                // 检查玩家是否持有 HeavenlyGale 武器，并且背包中有 MiracleMatterArrow 弹药
                if (heldItem != null && heldItem.type == ModContent.ItemType<HeavenlyGale>() &&
                    Player.inventory.Any(item => item.type == ModContent.ItemType<MiracleMatterArrow>() && item.stack > 0))
                {
                    if(IsMiracleMatterArrowActive)
                    {
                        // 降低远程伤害为 25%
                        Player.GetDamage(DamageClass.Ranged) *= 0.25f;
                    }
                }
            }

            // 备注:
            // 我真的是c了，开启无限弹药后这套逻辑直接不管用
            // 千万别开无限弹药，目前已确认的模组有：Tos QOL
        }
    }
}