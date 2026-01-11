namespace CalamityRangerExpansion.Content.WeaponToAMMO.Bullet.TheEmpty
{
    public class TheEmpty : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "WeaponToAMMO.Bullet.TheEmpty";
        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 18;
            Item.damage = 7;
            Item.DamageType = DamageClass.Ranged;
            Item.maxStack = 1;
            Item.consumable = false;
            Item.knockBack = 3f;

            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.shoot = ModContent.ProjectileType<TheEmptyPROJ>();
            Item.shootSpeed = 6f;
            Item.ammo = AmmoID.Bullet;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(1);
            recipe.AddIngredient<NullificationPistol>(1);
            recipe.AddCondition(Condition.NearShimmer);
            //recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
