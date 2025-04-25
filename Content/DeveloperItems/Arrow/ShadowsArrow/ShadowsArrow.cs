using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityRangerExpansion.Content.DeveloperItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items;
using CalamityMod.Rarities;
using CalamityMod;
using Microsoft.Xna.Framework;
using Terraria.DataStructures;
using CalamityMod.Tiles.Furniture.CraftingStations;

namespace CalamityRangerExpansion.Content.DeveloperItems.Arrow.ShadowsArrow
{
    public class ShadowsArrow : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.ShadowsArrow";
        public override void SetDefaults()
        {
            Item.damage = 78;
            Item.DamageType = DamageClass.Ranged;
            Item.width = 14;
            Item.height = 32;
            Item.maxStack = 1;
            Item.consumable = false;
            Item.knockBack = 3.5f;

            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.Calamity().devItem = true;
            Item.shoot = ModContent.ProjectileType<ShadowsArrowPROJ>();
            Item.shootSpeed = 9f;
            Item.ammo = AmmoID.Arrow; // 这是箭矢类型的弹药
        }

        #region 旧代码

        //public override bool Shoot(Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        //{
        //    // 获取玩家主背包右下角的格子
        //    int lastIndex = player.inventory.Length - 1;
        //    Item ammoSlot = player.inventory[lastIndex];

        //    // 如果格子为空或不是箭矢弹药，则什么都不射出
        //    if (ammoSlot == null || ammoSlot.ammo != AmmoID.Arrow)
        //    {
        //        return false; // 不发射任何弹幕
        //    }

        //    // 设定箭矢的属性
        //    Vector2 newVelocity = velocity; // 保持原速度

        //    Projectile.NewProjectile(
        //        source,
        //        position,
        //        newVelocity,
        //        ammoSlot.shoot, // 使用右下角格子的弹药类型
        //        150, // 强制面板伤害
        //        knockback,
        //        player.whoAmI
        //    );

        //    return false; // 不使用默认发射逻辑
        //}

        #endregion
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(1);
            recipe.AddRecipeGroup("CalamityRangerExpansion:RecipeGroupArrow", 1);
            recipe.AddIngredient<ShadowspecBar>(5);
            recipe.AddTile<DraedonsForge>();
            recipe.Register();
        }
    }
}
