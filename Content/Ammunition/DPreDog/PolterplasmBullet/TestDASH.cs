﻿using CalamityMod;
using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityRangerExpansion.Content.Ammunition.DPreDog.PolterplasmBullet
{
    public class TestDASH : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.accessory = true; // 设置为饰品
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modplayer = player.Calamity();

            modplayer.DashID = YuDash.ID;
        }
    }
}
