using CalamityRangerExpansion.Content.DeveloperItems.Arrow.MaoMaoChong;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Items.Materials;
using CalamityMod.Items;
using CalamityMod.Rarities;
using CalamityMod;

namespace CalamityRangerExpansion.Content.DeveloperItems.Arrow.ExplodingRabbit
{
    public class ExplodingRabbit : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.ExplodingRabbit";
        public override void SetDefaults()
        {
            Item.damage = 30;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 14;
            Item.height = 32;
            Item.maxStack = 9999;
            Item.consumable = true; // 弹药是消耗品
            Item.knockBack = 3.5f;

            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.Calamity().devItem = true;

            Item.shoot = ModContent.ProjectileType<ExplodingRabbitPROJ>();
            Item.shootSpeed = 15f;
            Item.ammo = AmmoID.Arrow; // 这是箭矢类型的弹药
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(99);
            recipe.AddIngredient(ItemID.Bunny, 1);
            recipe.AddIngredient(ItemID.Dynamite, 999);
            recipe.AddIngredient<CosmiliteBar>(1);
            recipe.AddCondition(Condition.NotForTheWorthy);
            recipe.AddTile<CosmicAnvil>();
            recipe.Register();

            Recipe recipe2 = CreateRecipe(999);
            recipe2.AddIngredient(ItemID.Bunny, 1);
            recipe2.AddIngredient(ItemID.Dynamite, 999);
            recipe2.AddCondition(Condition.ForTheWorthyWorld);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }

    }
}
