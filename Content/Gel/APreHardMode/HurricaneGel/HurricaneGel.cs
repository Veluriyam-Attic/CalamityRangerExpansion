using CalamityRangerExpansion.Content.Gel.CPreMoodLord.AstralGel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Materials;

namespace CalamityRangerExpansion.Content.Gel.APreHardMode.HurricaneGel
{
    public class HurricaneGel : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Gel.APreHardMode";
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
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.owner == player.whoAmI)
                {
                    proj.GetGlobalProjectile<HurricaneGelGP>().IsHurricaneGelInfused = true;
                }
            }

            // 通知 HurricaneGelPlayer 激活攻速降低效果
            player.GetModPlayer<HurricaneGelPlayer>().ActivateHurricaneGelEffect();
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(250);
            recipe.AddRecipeGroup("CalamityRangerExpansion:RecipeGroupGel", 250);
            recipe.AddIngredient<SeaPrism>(10);
            recipe.AddIngredient<PearlShard>(1);
            recipe.AddTile(TileID.Solidifier);
            recipe.Register();
        }
    }
}
