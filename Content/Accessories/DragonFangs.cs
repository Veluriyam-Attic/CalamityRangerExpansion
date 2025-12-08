using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Accessories;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityRangerExpansion.Content.Accessories
{
    public class DragonFangs : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 54;
            Item.accessory = true;
            Item.rare = ItemRarityID.Master;
            Item.value = Item.sellPrice(1, 0, 0, 0);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(1)
            .AddIngredient<AuricBar>(5)
            .AddIngredient<YharonSoulFragment>(3)
            .AddIngredient<ReaperToothNecklace>(1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetArmorPenetration<GenericDamageClass>() += 100f;
            FangsPlayer.EquipedFangs = true;
        }
    }

    public class FangsPlayer : ModPlayer
    {
        public static bool EquipedFangs = false;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (EquipedFangs)
            {
                target.AddBuff(ModContent.BuffType<Dragonfire>(), 120, false);
            }
        }
        public override void ResetEffects()
        {
            EquipedFangs = false;
        }
    }
}