namespace CalamityRangerExpansion.Content.DeveloperItems.Bullet.ShadowsBullet
{
    public class ShadowsBullet : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.ShadowsBullet";
        public override void SetDefaults()
        {
            Item.damage = 69;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 14;
            Item.height = 32;
            Item.maxStack = 1;
            Item.consumable = false; // 弹药是消耗品
            Item.knockBack = 3.5f;

            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.Calamity().devItem = true;
            Item.shoot = ModContent.ProjectileType<ShadowsBulletPROJ>();
            Item.shootSpeed = 9f;
            Item.ammo = AmmoID.Bullet; // 这是子弹类型的弹药
        }

        //public override bool Shoot(Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        //{
        //    // 获取玩家主背包右下角的格子
        //    int lastIndex = player.inventory.Length - 1;
        //    Item ammoSlot = player.inventory[lastIndex];

        //    // 如果格子为空或不子弹弹药，则什么都不射出
        //    if (ammoSlot == null || ammoSlot.ammo != AmmoID.Bullet)
        //    {
        //        return false; // 不发射任何弹幕
        //    }

        //    // 设定箭矢的属性
        //    Vector2 newVelocity = velocity; // 保持原速度

        //    Projectile.NewProjectile(
        //        source,
        //        position,
        //        newVelocity,
        //        ammoSlot.shoot, // 使用右下角格子的弹药类型
        //        150, // 强制面板伤害
        //        knockback,
        //        player.whoAmI
        //    );

        //    return false; // 不使用默认发射逻辑
        //}

        public override void AddRecipes()
        {
            Recipe recipe1 = CreateRecipe(1);
            recipe1.AddIngredient(ItemID.MusketBall, 1);
            recipe1.AddIngredient<ShadowspecBar>(5);
            recipe1.AddTile(TileID.Anvils);
            recipe1.Register();
        }

    }
}
