namespace CalamityRangerExpansion.Content.Arrows.EAfterDog.AuricArrow
{
    public class AuricArrow : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Arrows.EAfterDog";
        public override void SetDefaults()
        {
            Item.damage = 35;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 14;
            Item.height = 32;
            Item.maxStack = 9999;
            Item.consumable = true; // 弹药是消耗品
            Item.knockBack = 3.5f;
            Item.value = 10;
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<AuricArrowPROJ>();
            Item.shootSpeed = 9f;
            Item.ammo = AmmoID.Arrow; // 这是箭矢类型的弹药
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(999);
            recipe.AddRecipeGroup("CalamityRangerExpansion:RecipeGroupArrow", 999);
            recipe.AddIngredient<AuricBar>(1);
            recipe.AddTile<CosmicAnvil>();
            recipe.Register();
        }
    }
}
