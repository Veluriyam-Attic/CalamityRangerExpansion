using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;

namespace CalamityRangerExpansion.Content.Gel.APreHardMode.HurricaneGel
{
    internal class HurricaneGelPlayer : ModPlayer
    {
        private int hurricaneGelAttackSpeedTimer = 0;

        public override void ResetEffects()
        {
            if (hurricaneGelAttackSpeedTimer > 0)
            {
                Player.GetAttackSpeed(DamageClass.Generic) -= 0.1f;
                hurricaneGelAttackSpeedTimer--;
            }
        }

        public void ActivateHurricaneGelEffect()
        {
            hurricaneGelAttackSpeedTimer = 300; // 持续 300 帧（5 秒）
        }
    }
}
