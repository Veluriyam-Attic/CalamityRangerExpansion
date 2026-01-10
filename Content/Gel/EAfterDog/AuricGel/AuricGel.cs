namespace CalamityRangerExpansion.Content.Gel.EAfterDog.AuricGel
{
    public class AuricGel : ModItem, ILocalizedModType
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            Main.RegisterItemAnimation(Type, new DrawAnimationVertical(5, 14));
        }
        public new string LocalizationCategory => "Gel.EAfterDog";
        public override void SetDefaults()
        {
            //Item.damage = 85;
            Item.width = 12;
            Item.height = 18;
            Item.consumable = true;
            Item.ammo = AmmoID.Gel;
            Item.maxStack = 9999;
        }

        public override void OnConsumedAsAmmo(Item weapon, Player player)
        {
            // 附魔效果，标记弹幕使用了 AuricGel
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.owner == player.whoAmI)
                {
                    proj.GetGlobalProjectile<AuricGelGP>().IsAuricGelInfused = true;
                }
            }
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(500);
            recipe.AddRecipeGroup("CalamityRangerExpansion:RecipeGroupGel", 500);
            recipe.AddIngredient<AuricBar>(1);
            recipe.AddTile<CosmicAnvil>();
            recipe.Register();
        }
    }
}
