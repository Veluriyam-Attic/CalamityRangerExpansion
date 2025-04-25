using CalamityMod.Items.Ammo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.Items.Weapons.Ranged;
using CalamityRangerExpansion.Content.DeveloperItems.Weapon.DiffuseNovaArc;

namespace CalamityRangerExpansion
{
    internal class RedoCALAItem : GlobalItem // 继承全局物品类
    {
        public override void AddRecipes()
        {
            // 迫击炮
            Recipe recipe1 = Recipe.Create(ModContent.ItemType<MortarRound>());
            recipe1.AddIngredient(ItemID.Dynamite, 1);
            recipe1.AddTile(TileID.WorkBenches);
            recipe1.Register();
        }

        public override void SetStaticDefaults()
        {
            // 删除迫击炮的微光转化
            if (ModLoader.HasMod("CalamityMod"))
            {
                // 获取 迫击炮 MortarRound 的 ItemType
                int arcNovaDiffuserType = ModContent.ItemType<MortarRound>();

                // 动态修改 迫击炮 MortarRound 的 Shimmer 转化目标为 一根雷管 Dynamite
                ItemID.Sets.ShimmerTransformToItem[arcNovaDiffuserType] = ItemID.Dynamite;
            }
        }

        public override void SetDefaults(Item item)
        {
            // 检查是否是 迫击炮 或 橡胶迫击炮
            if (item.type == ModContent.ItemType<MortarRound>() || item.type == ModContent.ItemType<RubberMortarRound>())
            {
                item.damage = 1; // 将基础面板伤害降为 1
            }

            // 检查是否是 满弹霰弹枪
            if (item.type == ModContent.ItemType<BulletFilledShotgun>())
            {
                item.useTime = 30;
                item.useAnimation = 30;
            }

            // 检查是否是 绝路prime
            if (item.type == ModContent.ItemType<RubicoPrime>())
            {
                item.useAnimation = 30;
            }



        }
    }
}
