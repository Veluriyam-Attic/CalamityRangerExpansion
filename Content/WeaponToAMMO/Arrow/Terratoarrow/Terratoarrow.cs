namespace CalamityRangerExpansion.Content.WeaponToAMMO.Arrow.Terratoarrow
{
    public class Terratoarrow : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "WeaponToAMMO.Arrow.Terratoarrow";
        public override void SetDefaults()
        {
            Item.damage = 8;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 14;
            Item.height = 32;
            Item.maxStack = 1;
            Item.consumable = false;
            Item.knockBack = 3.5f;

            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.shoot = ModContent.ProjectileType<TerratoarrowPROJ>();
            Item.shootSpeed = 10f;
            Item.ammo = AmmoID.Arrow; // 这是箭矢类型的弹药
        }

        //public override void AddRecipes()
        //{
        //    Recipe recipe = CreateRecipe(333);
        //    recipe.AddIngredient<SproutingArrow>(333);
        //    recipe.AddIngredient<UelibloomBar>(1);
        //    recipe.AddTile(TileID.Anvils);
        //    recipe.Register();
        //}


        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(1);
            recipe.AddIngredient<Terratomere>(1);
            recipe.AddCondition(Condition.NearShimmer);
            //recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }

    }
}
