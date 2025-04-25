using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace CalamityRangerExpansion.CREConfigs
{
    public class WeaponGol : GlobalItem
    {
        //// 修改基础伤害
        //public override void SetDefaults(Item item)
        //{
        //    // 检查物品是否是原版武器
        //    if (item.type == ItemID.WoodenSword) // 木剑
        //    {
        //        item.damage = 20; // 将木剑的基础伤害改为 20
        //    }
        //}

        //// 修改暴击率
        //public override void ModifyWeaponCrit(Item item, Player player, ref float crit)
        //{
        //    // 检查物品是否是原版武器
        //    if (item.type == ItemID.CopperShortsword) // 铜短剑
        //    {
        //        crit += 5; // 给铜短剑增加 5% 暴击率
        //    }
        //}

        //// 修改其他属性，例如攻击速度
        //public override void ModifyWeaponKnockback(Item item, Player player, ref StatModifier knockback)
        //{
        //    // 检查物品是否是原版武器
        //    if (item.type == ItemID.WoodenBow) // 木弓
        //    {
        //        knockback *= 1.5f; // 将木弓的击退效果提升 50%
        //    }
        //}

        //public override void AddRecipes()
        //{
        //    // 创建一个配方
        //    Recipe recipe = Recipe.Create(ItemID.WoodenSword); // 目标物品：木剑

        //    // 添加材料：泥土块和腐肉
        //    recipe.AddIngredient(ItemID.DirtBlock, 10); // 需要 10 个泥土块
        //    recipe.AddIngredient(ItemID.RottenChunk, 1); // 需要 1 个腐肉

        //    // 添加制作环境：工作台
        //    recipe.AddTile(TileID.WorkBenches);

        //    // 注册配方
        //    recipe.Register();
        //}
    }
}
