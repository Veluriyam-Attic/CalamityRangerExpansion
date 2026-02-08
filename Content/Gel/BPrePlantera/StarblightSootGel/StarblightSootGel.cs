using CalamityRangerExpansion.Content.Gel.APreHardMode.HurricaneGel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.FurnitureMonolith;
using CalamityMod.Tiles.FurnitureStatigel;

namespace CalamityRangerExpansion.Content.Gel.BPrePlantera.StarblightSootGel
{
    internal class StarblightSootGel : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Gel.BPrePlantera";
        public override void SetDefaults()
        {
            //Item.damage = 85;
            Item.width = 12;
            Item.height = 18;
            Item.consumable = true;
            Item.ammo = AmmoID.Gel;
            Item.maxStack = 9999;
        }

        public override void OnConsumedAsAmmo(Item weapon, Player player)
        {
            // 附魔效果，标记弹幕使用了 XX 凝胶
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.owner == player.whoAmI)
                {
                    proj.GetGlobalProjectile<StarblightSootGelGP>().IsStarblightSootGelInfused = true;
                }
            }
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(500);
            recipe.AddRecipeGroup("CalamityRangerExpansion:RecipeGroupGel", 250);
            recipe.AddIngredient<StarblightSoot>(2);
            recipe.AddIngredient<AstralMonolith>(2); //AstralMonolith
            recipe.AddTile<StaticRefiner>(); // StaticRefiner
            recipe.Register();
        }
    }
}
