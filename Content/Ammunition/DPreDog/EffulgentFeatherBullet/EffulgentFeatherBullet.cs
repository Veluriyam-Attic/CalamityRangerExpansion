namespace CalamityRangerExpansion.Content.Ammunition.DPreDog.EffulgentFeatherBullet
{
    internal class EffulgentFeatherBullet : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Ammunition.DPreDog";


        public override void SetDefaults()
        {
            Item.width = 8;
            Item.height = 18;
            Item.damage = 25;
            Item.DamageType = DamageClass.Ranged;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.knockBack = 3f;
            Item.value = Item.sellPrice(copper: 12);
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<EffulgentFeatherBulletPROJ>();
            Item.shootSpeed = 9f;
            Item.ammo = AmmoID.Bullet;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(333);
            recipe.AddRecipeGroup("CalamityRangerExpansion:RecipeGroupBullet", 333);
            recipe.AddIngredient(ItemID.SoulofFlight, 3);
            recipe.AddIngredient<EffulgentFeather>(1);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
    }
}
