//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Terraria.ID;
//using Terraria.ModLoader;
//using Terraria;
//using CalamityMod.Items.Accessories;

//namespace CalamityRangerExpansion.Content.DeveloperItems.Others.NecromancerQuiver
//{
//    public class NecromancerQuiver : ModItem
//    {
//        public override void SetStaticDefaults()
//        {
//            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<QuiverofNihility>();
//        }


//        public override void SetDefaults()
//        {
//            Item.width = 32;
//            Item.height = 32;
//            Item.accessory = true;
//            Item.rare = ItemRarityID.Lime;
//            Item.value = Item.buyPrice(0, 30, 0, 0);
//        }

//        public override void UpdateAccessory(Player player, bool hideVisual)
//        {
//            player.GetModPlayer<NecromancerQuiverPlayer>().hasNecromancerQuiver = true;
//        }
//    }
//}