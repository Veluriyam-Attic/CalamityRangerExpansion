using Terraria;
using Terraria.ModLoader;

namespace CalamityRangerExpansion.Content.BOWChange.EAfterDog.UltimaC
{
    internal class UltimaRePlayer : ModPlayer
    {
        public bool IsRightClicking { get; private set; } // 用于存储右键按下状态

        public override void ResetEffects()
        {
            IsRightClicking = false; // 每帧重置状态
        }

        public override void ProcessTriggers(Terraria.GameInput.TriggersSet triggersSet)
        {
            if (Main.mouseRight) // 检测玩家是否按下右键
            {
                IsRightClicking = true;
            }
        }
    }
}
