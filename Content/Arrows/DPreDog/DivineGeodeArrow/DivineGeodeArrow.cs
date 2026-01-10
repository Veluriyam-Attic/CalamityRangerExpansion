namespace CalamityRangerExpansion.Content.Arrows.DPreDog.DivineGeodeArrow
{
    public class DivineGeodeArrow : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Arrows.DPreDog";
        public override void SetDefaults()
        {
            Item.damage = 25;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 14;
            Item.height = 32;
            Item.maxStack = 9999;
            Item.consumable = true; // 弹药是消耗品
            Item.knockBack = 3.5f;
            Item.value = 10;
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<DivineGeodeArrowPROJ>();
            Item.shootSpeed = 15f;
            Item.ammo = AmmoID.Arrow; // 这是箭矢类型的弹药
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(200);
            recipe.AddRecipeGroup("CalamityRangerExpansion:RecipeGroupArrow", 200);
            recipe.AddIngredient<DivineGeode>(1);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
    }
}
