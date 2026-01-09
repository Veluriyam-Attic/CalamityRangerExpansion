using CalamityMod.Items.TreasureBags.MiscGrabBags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Terraria.GameContent.ItemDropRules;

namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.TheGoldenFire
{
    internal class NeverGonnaGiveYouTheGoldenFire : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            // 确保只针对 StarterBag 生效
            // 记得&& lateInstantiation，以确保在物品实例化后应用
            return entity.type == ModContent.ItemType<StarterBag>() && lateInstantiation;
        }


        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            base.ModifyItemLoot(item, itemLoot);

            // 检查是否是 StarterBag
            if (item.type == ModContent.ItemType<StarterBag>())
            {
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<TheGoldenFire>(), 1));
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<TheLetter>(), 1));
                itemLoot.Add(ItemDropRule.Common(ItemID.Gel, 1,9999,9999));
            }
        }

    }
}