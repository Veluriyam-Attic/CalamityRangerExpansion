namespace CalamityRangerExpansion.Content.DeveloperItems.Bullet.Pao
{
    public class Pao : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.ChineseChess.Pao";
        public override void SetDefaults()
        {
            Item.damage = 13;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 14;
            Item.height = 32;
            Item.maxStack = 9999;
            Item.consumable = true; // 弹药是消耗品
            Item.knockBack = 3.5f;

            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.Calamity().devItem = true;
            Item.shoot = ModContent.ProjectileType<PaoPROJ>();
            Item.shootSpeed = 6f;
            Item.ammo = AmmoID.Bullet; // 这是子弹类型的弹药
        }

        public override void AddRecipes()
        {
            Recipe recipe1 = CreateRecipe(333);
            recipe1.AddIngredient(ItemID.AlphabetStatueP, 1);
            recipe1.AddIngredient(ItemID.AlphabetStatueA, 1);
            recipe1.AddIngredient(ItemID.AlphabetStatueO, 1);
            recipe1.AddIngredient(ItemID.ExplodingBullet, 333);
            recipe1.AddIngredient<ScoriaBar>(1);
            recipe1.AddTile(TileID.Anvils);
            recipe1.Register();
        }

    }
}
