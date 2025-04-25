using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace CalamityRangerExpansion.Content.Gel.APreHardMode.AerialiteGel
{
    public class AerialiteGelPlayer : ModPlayer
    {
        private const int RecoilCooldownTime = 300; // 5 秒，60 帧每秒
        private int recoilCooldownTimer = 0;

        public bool RecoilCooldownActive => recoilCooldownTimer > 0;

        public override void ResetEffects()
        {
            if (recoilCooldownTimer > 0)
            {
                recoilCooldownTimer--;
            }
        }

        public void StartRecoilCooldown()
        {
            recoilCooldownTimer = RecoilCooldownTime;
        }
    }

}
