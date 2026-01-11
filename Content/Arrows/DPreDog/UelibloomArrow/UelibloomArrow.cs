namespace CalamityRangerExpansion.Content.Arrows.DPreDog.UelibloomArrow
{
    public class UelibloomArrow : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Arrows.DPreDog";
        public override void SetDefaults()
        {
            Item.damage = 32;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 14;
            Item.height = 32;
            Item.maxStack = 9999;
            Item.consumable = true; // 弹药是消耗品
            Item.knockBack = 3.5f;
            Item.value = 10;
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<UelibloomArrowPROJ>();
            Item.shootSpeed = 15f;
            Item.ammo = AmmoID.Arrow; // 这是箭矢类型的弹药
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(333);
            recipe.AddRecipeGroup("CalamityRangerExpansion:RecipeGroupArrow", 333);
            recipe.AddIngredient<UelibloomBar>(1);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
    }
}
