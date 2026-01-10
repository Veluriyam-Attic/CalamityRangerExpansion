namespace CalamityRangerExpansion.Content.WeaponToAMMO.Arrow.TheDrill
{
    public class TheDrill : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "WeaponToAMMO.Arrow.TheDrill";
        public override void SetDefaults()
        {
            Item.damage = Main.getGoodWorld ? 100 : 1; // 根据模式设置伤害
            Item.DamageType = DamageClass.Ranged;
            Item.width = 14;
            Item.height = 32;
            Item.maxStack = 1;
            Item.consumable = false; // 弹药是消耗品
            Item.knockBack = 3.5f;

            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.shoot = ModContent.ProjectileType<TheDrillPROJ>();
            Item.shootSpeed = 15f;
            Item.ammo = AmmoID.Arrow; // 这是箭矢类型的弹药
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(1);
            recipe.AddIngredient(ItemID.DrillContainmentUnit, 1); // 钻头控制装置
            //recipe.AddIngredient(ItemID.WoodenArrow, 3996);
            //recipe.AddIngredient<WulfrumDiggingTurtle>(1);
            recipe.AddCondition(Condition.NearShimmer);
            //recipe.AddTile(TileID.Anvils);
            recipe.Register();

            Recipe recipe2 = CreateRecipe(1);
            // 所有原版花前的钻头
            recipe2.AddIngredient(ItemID.CobaltDrill, 1);
            recipe2.AddIngredient(ItemID.PalladiumDrill, 1);
            recipe2.AddIngredient(ItemID.MythrilDrill, 1);
            recipe2.AddIngredient(ItemID.OrichalcumDrill, 1);
            recipe2.AddIngredient(ItemID.AdamantiteDrill, 1);
            recipe2.AddIngredient(ItemID.TitaniumDrill, 1);
            recipe2.AddIngredient(ItemID.Drax, 1);
            recipe2.AddIngredient(ItemID.ChlorophyteDrill, 1);
            // 大钻石
            recipe2.AddIngredient(ItemID.LargeDiamond, 1);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }


    }
}
