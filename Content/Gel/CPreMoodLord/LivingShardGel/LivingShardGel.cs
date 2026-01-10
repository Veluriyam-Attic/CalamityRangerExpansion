namespace CalamityRangerExpansion.Content.Gel.CPreMoodLord.LivingShardGel
{
    internal class LivingShardGel : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Gel.CPreMoodLord";
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
            // 附魔效果，标记弹幕使用了 XX 凝胶
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.owner == player.whoAmI)
                {
                    proj.GetGlobalProjectile<LivingShardGelGP>().IsLivingShardGelInfused = true;
                }
            }
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(250);
            recipe.AddRecipeGroup("CalamityRangerExpansion:RecipeGroupGel", 250);
            recipe.AddIngredient<LivingShard>(1);
            recipe.AddTile<StaticRefiner>();
            recipe.Register();
        }
    }
}
