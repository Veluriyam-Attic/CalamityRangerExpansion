namespace CalamityRangerExpansion.Content.WeaponToAMMO.Bullet.OrichalcumBullet
{
    internal class OrichalcumBullet : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "WeaponToAMMO.Bullet.OrichalcumBullet";
        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 18;
            Item.damage = 2;
            Item.DamageType = DamageClass.Ranged;
            Item.maxStack = 1;
            Item.consumable = false;
            Item.knockBack = 3f;

            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.shoot = ModContent.ProjectileType<OrichalcumBulletPROJ>();
            Item.shootSpeed = 10f;
            Item.ammo = AmmoID.Bullet;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(1);
            recipe.AddIngredient<OrichalcumSpikedGemstone>(3996);
            recipe.AddCondition(Condition.NearShimmer);
            //recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }

    }
}
