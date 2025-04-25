using CalamityMod.Items.Weapons.Ranged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.Items.DraedonMisc;
using CalamityMod.Items;
using CalamityMod.Rarities;
using CalamityMod;
using CalamityMod.Items.Weapons.Magic;

namespace CalamityRangerExpansion.Content.WeaponToAMMO.Bullet.YuanZiDan
{
    public class YuanZiDan : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "WeaponToAMMO.Bullet.YuanZiDan";
        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 18;
            Item.damage = 2;
            Item.DamageType = DamageClass.Ranged;
            Item.maxStack = 1;
            Item.consumable = false;
            Item.knockBack = 3f;

            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.shoot = ModContent.ProjectileType<YuanZiDanPROJ>();
            Item.shootSpeed = 6f;
            Item.ammo = AmmoID.Bullet;
        }

        //public override void AddRecipes()
        //{
        //    Recipe recipe = CreateRecipe(1);
        //    recipe.AddIngredient(ItemID.NanoBullet, 3996);
        //    recipe.AddIngredient<AuricQuantumCoolingCell>(1);
        //    recipe.AddTile(TileID.Anvils);
        //    recipe.Register();
        //}

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(1);
            recipe.AddIngredient<RecitationoftheBeast>(1);
            recipe.AddCondition(Condition.NearShimmer);
            //recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }


    }
}
