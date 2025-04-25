//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Terraria;
//using Terraria.ModLoader;

//namespace CalamityRangerExpansion.Content.Arrows.EAfterDog.AuricArrow
//{
//    public class AuricArrowNPCDropPLAYER : ModPlayer
//    {     
//        public override void PostUpdate()
//        {
//            // 遍历玩家背包中的所有物品
//            for (int i = 0; i < Player.inventory.Length; i++)
//            {
//                // 检测是否为 AuricArrowNPCDrop
//                if (Player.inventory[i].type == ModContent.ItemType<AuricArrowNPCDrop>())
//                {
//                    // 直接删除背包中的该物品
//                    Player.inventory[i].SetDefaults(0); // 设置为空物品
//                }
//            }
//        }
//    }
//}
