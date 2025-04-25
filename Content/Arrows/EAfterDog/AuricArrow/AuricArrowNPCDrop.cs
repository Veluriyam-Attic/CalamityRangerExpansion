//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Terraria.ID;
//using Terraria.ModLoader;
//using Terraria;

//namespace CalamityRangerExpansion.Content.Arrows.EAfterDog.AuricArrow
//{
//    public class AuricArrowNPCDrop : ModItem
//    {

//        public override void SetStaticDefaults()
//        {
//            // 设置物品不会受到重力影响
//            ItemID.Sets.ItemNoGravity[Item.type] = true;
//        }

//        public override void SetDefaults()
//        {
//            Item.width = 20; // 物品宽度
//            Item.height = 20; // 物品高度
//            Item.maxStack = 1; // 最大堆叠数量
//            Item.value = 1000; // 物品价值
//            Item.rare = 3; // 物品稀有度
//        }

//        public override bool OnPickup(Player player)
//        {
//            // 给玩家施加增益效果，持续3秒
//            player.AddBuff(ModContent.BuffType<AuricArrowPBuff>(), 180); // 3秒 = 180帧
//            return true; // 允许拾取
//        }
//    }
//}




