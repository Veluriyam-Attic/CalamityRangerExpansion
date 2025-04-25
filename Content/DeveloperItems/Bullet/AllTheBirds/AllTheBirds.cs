using CalamityRangerExpansion.Content.Arrows.CPreMoodLord.CoreofCalamityArrow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.Items;
using CalamityMod.Rarities;
using CalamityMod;

namespace CalamityRangerExpansion.Content.DeveloperItems.Bullet.AllTheBirds
{
    public class AllTheBirds : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.AllTheBirds";
        public override void SetDefaults()
        {
            Item.damage = 3;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 14;
            Item.height = 32;
            Item.maxStack = 9999;
            Item.consumable = true; // 弹药是消耗品
            Item.knockBack = 3.5f;
            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.Calamity().donorItem = true;
            Item.shoot = ModContent.ProjectileType<AllTheBirdsPROJ>();
            Item.shootSpeed = 1f;
            Item.ammo = AmmoID.Bullet; // 这是子弹类型的弹药
        }

        public override void AddRecipes()
        {
            // 第一个配方：使用 YellowCockatiel 制作
            Recipe recipe1 = CreateRecipe(333);
            recipe1.AddIngredient(ItemID.YellowCockatiel, 1);
            recipe1.AddTile(TileID.Anvils);
            recipe1.Register();

            // 第二个配方：使用 GrayCockatiel 制作
            Recipe recipe2 = CreateRecipe(333);
            recipe2.AddIngredient(ItemID.GrayCockatiel, 1);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }

    }
}
