using CalamityMod.Items.Materials;
using CalamityRangerExpansion.Content.Arrows.CPreMoodLord.PlagueArrow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityRangerExpansion.Content.Arrows.CPreMoodLord.PerennialArrow;

namespace CalamityRangerExpansion.Content.Arrows.DPreDog.ToothArrow
{
    internal class ToothArrow : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Arrows.DPreDog";
        public override void SetDefaults()
        {
            Item.damage = 24;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 14;
            Item.height = 32;
            Item.maxStack = 9999;
            Item.consumable = true; // 弹药是消耗品
            Item.knockBack = 3.5f;
            Item.value = 10;
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<ToothArrowPROJ>();
            Item.shootSpeed = 15f;
            Item.ammo = AmmoID.Arrow; // 这是箭矢类型的弹药
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(750);
            recipe.AddRecipeGroup("CalamityRangerExpansion:RecipeGroupArrow", 750);
            recipe.AddIngredient<ReaperTooth>(1);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
    }
}
