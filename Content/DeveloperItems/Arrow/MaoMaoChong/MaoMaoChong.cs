namespace CalamityRangerExpansion.Content.DeveloperItems.Arrow.MaoMaoChong
{
    public class MaoMaoChong : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.MaoMaoChong";
        public override void SetDefaults()
        {
            Item.damage = 15;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 14;
            Item.height = 32;
            Item.maxStack = 9999;
            Item.consumable = true; // 弹药是消耗品
            Item.knockBack = 3.5f; 
            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.Calamity().donorItem = true;
            Item.shoot = ModContent.ProjectileType<MaoMaoChongPROJ>();
            Item.shootSpeed = 15f;
            Item.ammo = AmmoID.Arrow; // 这是箭矢类型的弹药
        }

        // 猫咪许可证+蠕虫+999木箭=999咖波 且只限月后合成（如果实现不了就在材料里加5夜明锭吧）
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(999);
            recipe.AddRecipeGroup("CalamityRangerExpansion:RecipeGroupArrow", 999);
            recipe.AddIngredient(ItemID.LunarBar, 1);
            recipe.AddIngredient(ItemID.LicenseCat, 1);
            recipe.AddCondition(Condition.DownedMoonLord);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }

    }
}
