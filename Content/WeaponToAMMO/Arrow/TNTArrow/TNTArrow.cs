namespace CalamityRangerExpansion.Content.WeaponToAMMO.Arrow.TNTArrow
{
    internal class TNTArrow : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "WeaponToAMMO.Arrow.TNTArrow";
        public override void SetDefaults()
        {
            Item.damage = 10;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 14;
            Item.height = 32;
            Item.maxStack = 1;
            Item.consumable = false; // 弹药是消耗品
            Item.knockBack = 3.5f;

            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.shoot = ModContent.ProjectileType<TNTArrowPROJ>();
            Item.shootSpeed = 15f;
            Item.ammo = AmmoID.Arrow; // 这是箭矢类型的弹药
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(1);
            recipe.AddIngredient<BlastBarrel>(1);
            recipe.AddCondition(Condition.NearShimmer);
            recipe.Register();
        }
    }
}
