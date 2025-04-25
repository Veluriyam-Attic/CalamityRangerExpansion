using Terraria;
using Terraria.ModLoader;

namespace CalamityRangerExpansion.Content.Arrows.DPreDog.DivineGeodeArrow
{
    public class DivineGeodeArrowPlayer : ModPlayer
    {
        public bool HasDivineGeodeBuff { get; set; } = false; // 开关状态

        public override void ResetEffects()
        {
            // 每帧重置开关状态，由 Buff 更新
            HasDivineGeodeBuff = false;
        }

        public override void PostUpdateMiscEffects()
        {
            // 如果玩家拥有 Buff 或开关被激活
            if (HasDivineGeodeBuff)
            {
                // 增加翅膀飞行时间
                if (Player.wingTimeMax > 0) // 确保玩家装备了翅膀
                {
                    Player.wingTimeMax = (int)(Player.wingTimeMax * 1.125f); // 增加 12.5% 的最大飞行时间
                }
            }
        }
    }
}
