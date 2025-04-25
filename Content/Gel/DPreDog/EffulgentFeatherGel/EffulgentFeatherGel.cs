using CalamityRangerExpansion.Content.Gel.APreHardMode.HurricaneGel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.Items.Materials;
using Terraria.DataStructures;
using CalamityMod.Tiles.Furniture.CraftingStations;

namespace CalamityRangerExpansion.Content.Gel.DPreDog.EffulgentFeatherGel
{
    internal class EffulgentFeatherGel : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Gel.DPreDog";
        #region 旧代码
        //public override void SetStaticDefaults()
        //{
        //    ItemID.Sets.AnimatesAsSoul[Type] = true;
        //    Main.RegisterItemAnimation(Type, new DrawAnimationVertical(5, 10));
        //}
        #endregion
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
                    proj.GetGlobalProjectile<EffulgentFeatherGelGP>().IsEffulgentFeatherGelInfused = true;
                }
            }
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(333);
            recipe.AddRecipeGroup("CalamityRangerExpansion:RecipeGroupGel", 333);
            recipe.AddIngredient(ItemID.SoulofFlight, 5);
            recipe.AddIngredient<EffulgentFeather>(1);
            recipe.AddTile<StaticRefiner>();
            recipe.Register();
        }
    }
}
