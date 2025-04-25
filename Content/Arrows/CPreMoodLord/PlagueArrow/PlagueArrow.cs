using CalamityRangerExpansion.Content.Arrows.CPreMoodLord.PerennialArrow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;

namespace CalamityRangerExpansion.Content.Arrows.CPreMoodLord.PlagueArrow
{
    internal class PlagueArrow : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Arrows.CPreMoodLord";
        public override void SetDefaults()
        {
            Item.damage = 18;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 14;
            Item.height = 32;
            Item.maxStack = 9999;
            Item.consumable = true; // 弹药是消耗品
            Item.knockBack = 3.5f;
            Item.value = 10;
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<PlagueArrowPROJ>();
            Item.shootSpeed = 15f;
            Item.ammo = AmmoID.Arrow; // 这是箭矢类型的弹药
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(250);
            recipe.AddRecipeGroup("CalamityRangerExpansion:RecipeGroupArrow", 250);
            recipe.AddIngredient<PlagueCellCanister>(3);
            recipe.AddIngredient<InfectedArmorPlating>(1);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}
