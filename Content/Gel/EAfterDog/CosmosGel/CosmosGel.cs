using CalamityMod.Items;
using CalamityMod;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Pets;
using CalamityMod.Rarities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityRangerExpansion.Content.DeveloperItems.Weapon.TheGoldenFire;
using CalamityMod.Tiles.Furniture.CraftingStations;

namespace CalamityRangerExpansion.Content.Gel.EAfterDog.CosmosGel
{
    public class CosmosGel : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Gel.EAfterDog";
        public override void SetDefaults()
        {
            //Item.damage = 50;
            Item.width = 12;
            Item.height = 18;
            Item.consumable = true;
            Item.ammo = AmmoID.Gel;
            Item.maxStack = 9999;
        }


        public override void OnConsumedAsAmmo(Item weapon, Player player)
        {
            // 获取当前玩家最后生成的弹幕
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.owner == player.whoAmI && proj.timeLeft == proj.MaxUpdates)
                {
                    proj.GetGlobalProjectile<CosmosGelGP>().IsCosmosGelInfused = true;
                }
            }
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(500);
            recipe.AddRecipeGroup("CalamityRangerExpansion:RecipeGroupGel", 500);
            recipe.AddIngredient<CosmiliteBar>(1);
            recipe.AddTile<CosmicAnvil>();
            recipe.Register();
        }
    }
}
