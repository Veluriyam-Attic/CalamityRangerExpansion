namespace CalamityRangerExpansion.Content.WeaponToAMMO.Bullet.NorthStar
{
    internal class TheNorthStar : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "WeaponToAMMO.Bullet.NorthStar";
        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 18;
            Item.damage = 12;
            Item.DamageType = DamageClass.Ranged;
            Item.maxStack = 1;
            Item.consumable = false;
            Item.knockBack = 3f;

            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.shoot = ModContent.ProjectileType<PolarStarO>();
            Item.shootSpeed = 10f;
            Item.ammo = AmmoID.Bullet;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // 获取 PPPlayer 实例
            PPPlayer modPlayer = player.GetModPlayer<PPPlayer>();

            if (modPlayer.polarisBoostThree) //Homes in and explodes
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<PolarStarO>(), damage, knockback, player.whoAmI, 0f, 2f);
                return false;
            }
            else if (modPlayer.polarisBoostTwo) //Splits on enemy or tile hits
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<PolarStarO>(), (int)(damage * 1.25), knockback, player.whoAmI, 0f, 1f);
                return false;
            }
            return true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(1);
            recipe.AddIngredient<PolarisParrotfish>(1);
            recipe.AddCondition(Condition.NearShimmer);
            //recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }

    }
}
