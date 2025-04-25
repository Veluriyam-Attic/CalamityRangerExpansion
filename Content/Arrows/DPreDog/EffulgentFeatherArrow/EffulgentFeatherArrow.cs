using System;
using System.Collections.Generic;
using System.Text;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityRangerExpansion.Content.Arrows.DPreDog.EffulgentFeatherArrow
{
    public class EffulgentFeatherArrow : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Arrows.DPreDog";
        public override void SetDefaults()
        {
            Item.damage = 36;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 14;
            Item.height = 32;
            Item.maxStack = 9999;
            Item.consumable = true; // 弹药是消耗品
            Item.knockBack = 3.5f;
            Item.value = 10;
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<EffulgentFeatherArrowPROJ>();
            Item.shootSpeed = 10f;
            Item.ammo = AmmoID.Arrow; // 这是箭矢类型的弹药
        }

        public override void AddRecipes()
        {            
            Recipe recipe = CreateRecipe(500);
            recipe.AddRecipeGroup("CalamityRangerExpansion:RecipeGroupArrow", 500);
            recipe.AddIngredient(ItemID.SoulofFlight, 5);
            recipe.AddIngredient<EffulgentFeather>(1);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
    }
}
