namespace CalamityRangerExpansion.Content.Arrows.CPreMoodLord.LifeAlloyArrow
{
    public class LifeAlloyArrow : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Arrows.CPreMoodLord";
        public override void SetDefaults()
        {
            Item.damage = 7;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 14;
            Item.height = 32;
            Item.maxStack = 9999;
            Item.consumable = true; // 弹药是消耗品
            Item.knockBack = 3.5f;
            Item.value = 10;
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<LifeAlloyArrowPROJ>();
            Item.shootSpeed = 10f;
            Item.ammo = AmmoID.Arrow; // 这是箭矢类型的弹药
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(999);
            recipe.AddIngredient<PerennialArrow.PerennialArrow>(333);
            recipe.AddIngredient<ScoriaArrow.ScoriaArrow>(333);
            recipe.AddIngredient<VeriumBolt>(333);
            recipe.AddIngredient<LifeAlloy>(1);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}
