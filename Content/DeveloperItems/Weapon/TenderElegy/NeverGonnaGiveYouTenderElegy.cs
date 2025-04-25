using CalamityMod;
using CalamityMod.Systems;
using Terraria.ModLoader;
using CalamityRangerExpansion.Content.DeveloperItems.Weapon.TenderElegy;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.UI.CalamitasEnchants;

namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.TenderElegy
{
    public class NeverGonnaGiveYouTenderElegy : ModSystem
    {
        public override void PostSetupContent()
        {
            // 确保 CalamityMod 存在，避免崩溃
            if (ModLoader.TryGetMod("CalamityMod", out Mod calamityMod))
            {
                // 确保主模组的升级关系表已初始化
                if (EnchantmentManager.ItemUpgradeRelationship != null)
                {
                    // 只有击败了 ExoMechs 和 Calamitas 之后，才能将 Phangasm 转化为 TenderElegy
                    //if (DownedBossSystem.downedExoMechs && DownedBossSystem.downedCalamitas)
                    //if (DownedBossSystem.downedExoMechs)
                    {
                            EnchantmentManager.ItemUpgradeRelationship[ModContent.ItemType<TenderElegyi>()] = ModContent.ItemType<TenderElegy>();
                    }
                }
            }
        }
    }
}
