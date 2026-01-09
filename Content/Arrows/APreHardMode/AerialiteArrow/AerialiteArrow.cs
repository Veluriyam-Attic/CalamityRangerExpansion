using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;



namespace CalamityRangerExpansion.Content.Arrows.APreHardMode.AerialiteArrow
{
    public class AerialiteArrow : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Arrows.APreHardMode";
        public override void SetDefaults()
        {
            Item.damage = B.Arrows.AerialiteDamage;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 14;
            Item.height = 32;
            Item.maxStack = 9999;
            Item.consumable = true; // 弹药是消耗品
            Item.knockBack = B.Arrows.AerialiteKnockback;
            Item.value = 10;
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<AerialiteArrowPROJ>();
            Item.shootSpeed = B.Arrows.AerialiteShootspeed;
            Item.ammo = AmmoID.Arrow; // 这是箭矢类型的弹药
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(200);
            recipe.AddRecipeGroup("CalamityRangerExpansion:RecipeGroupArrow", 200);
            recipe.AddIngredient<AerialiteBar>(1);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
