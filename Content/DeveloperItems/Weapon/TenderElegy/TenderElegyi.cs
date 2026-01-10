namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.TenderElegy
{
    internal class TenderElegyi : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.TenderElegy";
        public override void SetStaticDefaults()
        {

        }
        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 60;
            Item.damage = 333;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = 10;
            Item.useAnimation = 7;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2f;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shootSpeed = 19f;
            Item.shoot = ProjectileID.WoodenArrowFriendly; // 仅作默认值
            Item.useAmmo = AmmoID.Arrow; // 允许使用任何箭矢

            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.Calamity().devItem = true;
        }
        public override Vector2? HoldoutOffset() => new Vector2(-17, 0);
        public override bool Shoot(Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // 计算水平偏移
            Vector2 offset = Vector2.Normalize(velocity.RotatedBy(MathHelper.PiOver2));
            position += offset * Main.rand.NextFloat(-19f, 19f);
            //position -= 3f * velocity;

            // 生成弹幕，不修改 type，保持原弹药特性
            Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);

            return false; // 阻止默认的单箭矢发射
        }
    }
}
