namespace CalamityRangerExpansion.Content.Gel.CPreMoodLord.PlagueGel
{
    internal class PlagueGel : ModItem, ILocalizedModType
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
            // 附魔效果，标记弹幕使用了 CosmosGel
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.owner == player.whoAmI)
                {
                    proj.GetGlobalProjectile<PlagueGelGP>().IsPlagueGelInfused = true;
                }
            }
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(150);
            recipe.AddRecipeGroup("CalamityRangerExpansion:RecipeGroupGel", 250);
            recipe.AddIngredient<PlagueCellCanister>(3);
            recipe.AddIngredient<InfectedArmorPlating>(1);
            recipe.AddTile<StaticRefiner>();
            recipe.Register();
        }
    }
}
