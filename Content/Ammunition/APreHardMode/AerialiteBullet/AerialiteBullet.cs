namespace CalamityRangerExpansion.Content.Ammunition.APreHardMode.AerialiteBullet
{
    internal class AerialiteBullet : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Ammunition.APreHardMode";


        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 18;
            Item.damage = 9;
            Item.DamageType = DamageClass.Ranged;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.knockBack = 3f;
            Item.value = Item.sellPrice(copper: 12);
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<AerialiteBulletPROJ>();
            Item.shootSpeed = 9f;
            Item.ammo = AmmoID.Bullet;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(250);
            recipe.AddRecipeGroup("CalamityRangerExpansion:RecipeGroupBullet", 250);
            recipe.AddIngredient<AerialiteBar>(1);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
