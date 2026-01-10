namespace CalamityRangerExpansion.Content.Gel.EAfterDog.MiracleMatterGel
{
    public class MiracleMatterGel : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Gel.EAfterDog";
        public override void SetDefaults()
        {
            //Item.damage = 1;
            Item.width = 12;
            Item.height = 18;
            Item.consumable = true;
            Item.ammo = AmmoID.Gel;
            Item.maxStack = 9999;
        }

        public override void OnConsumedAsAmmo(Item weapon, Player player)
        {
            // 附魔效果，标记弹幕使用了 MiracleMatterGel
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.owner == player.whoAmI)
                {
                    proj.GetGlobalProjectile<MiracleMatterGelGP>().IsMiracleMatterGelInfused = true;
                }
            }
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(999);
            recipe.AddRecipeGroup("CalamityRangerExpansion:RecipeGroupGel", 999);
            recipe.AddIngredient<MiracleMatter>(1);
            recipe.AddTile<DraedonsForge>();
            recipe.Register();
        }
    }
}
