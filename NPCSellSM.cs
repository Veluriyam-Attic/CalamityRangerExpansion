using CalamityRangerExpansion.Content.DeveloperItems.Weapon.HD2.LAS17;
using CalamityRangerExpansion.Content.DeveloperItems.Weapon.HD2.R36;
using CalamityRangerExpansion.Content.DeveloperItems.Weapon.HD2.SG225IE;
using CalamityRangerExpansion.Content.DeveloperItems.Weapon.HD2.VG70;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityRangerExpansion
{
    internal class NPCSellSM : GlobalNPC
    {
        public override void ModifyShop(NPCShop shop)
        {
            // 只作用于军火商
            if (shop.NpcType != NPCID.ArmsDealer)
                return;

            // =========================
            // 击败克苏鲁之眼 → SG225IE（30 金）
            // =========================
            Condition downedEoC = new Condition(
                "DownedEyeOfCthulhu",
                () => NPC.downedBoss1
            );

            AddWithCustomValue(
                shop,
                ModContent.ItemType<SG225IE>(),
                Item.buyPrice(gold: 30),
                downedEoC
            );

            // =========================
            // 击败血肉之墙 → R36（50 金）
            // =========================
            Condition downedWoF = new Condition(
                "DownedWallOfFlesh",
                () => Main.hardMode
            );

            AddWithCustomValue(
                shop,
                ModContent.ItemType<R36>(),
                Item.buyPrice(gold: 50),
                downedWoF
            );

            // =========================
            // 击败世纪之花 → LAS17（70 金）
            // =========================
            Condition downedPlantera = new Condition(
                "DownedPlantera",
                () => NPC.downedPlantBoss
            );

            AddWithCustomValue(
                shop,
                ModContent.ItemType<LAS17>(),
                Item.buyPrice(gold: 70),
                downedPlantera
            );

            // =========================
            // 击败拜月邪教徒 → VG70（100 金）
            // =========================
            Condition downedCultist = new Condition(
                "DownedCultist",
                () => NPC.downedAncientCultist
            );

            AddWithCustomValue(
                shop,
                ModContent.ItemType<VG70>(),
                Item.buyPrice(gold: 100),
                downedCultist
            );
        }

        // =============================
        // ★ 本地版 AddWithCustomValue
        // （与你示范文件完全一致）
        // =============================
        private NPCShop AddWithCustomValue(
            NPCShop shop,
            int itemType,
            int customValue,
            params Condition[] conditions
        )
        {
            Item item = new Item();
            item.SetDefaults(itemType);
            item.shopCustomPrice = customValue;

            shop.Add(item, conditions);
            return shop;
        }

        // 泛型版本（可选，但保留，和你示范一致）
        private NPCShop AddWithCustomValue<T>(
            NPCShop shop,
            int customValue,
            params Condition[] conditions
        ) where T : ModItem
        {
            return AddWithCustomValue(
                shop,
                ModContent.ItemType<T>(),
                customValue,
                conditions
            );
        }
    }
}
