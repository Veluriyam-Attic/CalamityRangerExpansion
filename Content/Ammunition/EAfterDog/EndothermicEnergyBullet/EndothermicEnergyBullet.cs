namespace CalamityRangerExpansion.Content.Ammunition.EAfterDog.EndothermicEnergyBullet
{
    public class EndothermicEnergyBullet : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Ammunition.EAfterDog";
        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 18;
            Item.damage = 40;
            Item.DamageType = DamageClass.Ranged;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.knockBack = 3f;

            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            //Item.Calamity().devItem = true;
            Item.shoot = ModContent.ProjectileType<EndothermicEnergyBulletPROJ>();
            Item.shootSpeed = 19f;
            Item.ammo = AmmoID.Bullet;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(500);
            recipe.AddRecipeGroup("CalamityRangerExpansion:RecipeGroupBullet", 500);
            recipe.AddIngredient<EndothermicEnergy>(3);
            recipe.AddTile<CosmicAnvil>();
            recipe.Register();
        }
    }
}
