using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace CalamityRangerExpansion.Content
{
    public class TheLetter : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 34;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.Master;
            Item.value = Item.sellPrice(100,0,0,0);
        }
    }
}