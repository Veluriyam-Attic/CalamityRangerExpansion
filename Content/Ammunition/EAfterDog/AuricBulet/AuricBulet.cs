using CalamityMod.Items.Materials;
using CalamityRangerExpansion.Content.Ammunition.BPrePlantera.CryonicBullet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.Tiles.Furniture.CraftingStations;

namespace CalamityRangerExpansion.Content.Ammunition.EAfterDog.AuricBulet
{
    public class AuricBulet : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Ammunition.EAfterDog";

        public override void SetDefaults()
        {
            Item.width = 8;
            Item.height = 18;
            Item.damage = 42;
            Item.DamageType = DamageClass.Ranged;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.knockBack = 3f;
            Item.value = Item.sellPrice(copper: 12);
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<AuricBuletPROJ>();
            Item.shootSpeed = 7f;
            Item.ammo = AmmoID.Bullet;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(500);
            recipe.AddRecipeGroup("CalamityRangerExpansion:RecipeGroupBullet", 500);
            recipe.AddIngredient<AuricBar>(1);
            recipe.AddTile<CosmicAnvil>();
            recipe.Register();
        }
    }
}
