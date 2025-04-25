using Terraria;
using Terraria.ModLoader;

namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.TenderElegy
{
    public class TenderElegyBeilGNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        public bool hasBell = false;

        public override void ResetEffects(NPC npc)
        {
            // **不自动重置 hasBell，确保铃铛弹幕不会重复生成**
        }

        public override void OnKill(NPC npc)
        {
            hasBell = false; // **敌人死亡时，重置标记**
        }
    }
}
