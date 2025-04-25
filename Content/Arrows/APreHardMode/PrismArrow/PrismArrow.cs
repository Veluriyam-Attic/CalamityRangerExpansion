using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Materials;
namespace CalamityRangerExpansion.Content.Arrows.APreHardMode.PrismArrow
{
    internal class PrismArrow : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Arrows.APreHardMode";

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
            Item.shoot = ModContent.ProjectileType<PrismArrowPROJ>();
            Item.shootSpeed = 8f;
            Item.ammo = AmmoID.Arrow; // 这是箭矢类型的弹药
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(150);
            recipe.AddRecipeGroup("CalamityRangerExpansion:RecipeGroupArrow", 150);
            recipe.AddIngredient<SeaPrism>(1);
            recipe.AddIngredient<PearlShard>(1);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
