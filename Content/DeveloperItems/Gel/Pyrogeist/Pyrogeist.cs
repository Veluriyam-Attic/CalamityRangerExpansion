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
using CalamityMod;

namespace CalamityRangerExpansion.Content.DeveloperItems.Gel.Pyrogeist
{
    internal class Pyrogeist : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.Pyrogeist";
        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 18;
            Item.consumable = true;
            Item.ammo = AmmoID.Gel;
            Item.maxStack = 9999;
            Item.Calamity().donorItem = true;
        }

        public override void OnConsumedAsAmmo(Item weapon, Player player)
        {
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.owner == player.whoAmI)
                {
                    proj.GetGlobalProjectile<PyrogeistGP>().IsPyrogeistInfused = true;
                }
            }
        }
        public static class GraveyardCondition
        {
            public static Condition InGraveyard = new Condition(
                "GraveyardCondition",
                () => Main.LocalPlayer.ZoneGraveyard // 检查玩家是否处于坟墓环境
            );
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(500);
            //recipe.AddIngredient<Hurrican>(1);
            recipe.AddIngredient(ItemID.Ectoplasm, 1);
            recipe.AddIngredient(ItemID.LivingFireBlock, 1);
            recipe.AddCondition(Condition.InGraveyard); // 官方坟墓环境检查（建议）
            //recipe.AddCondition(GraveyardCondition.InGraveyard); // 自制坟墓环境检查（不建议）
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}
