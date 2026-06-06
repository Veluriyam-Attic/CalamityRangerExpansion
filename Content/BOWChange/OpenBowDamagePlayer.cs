using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace CalamityRangerExpansion.Content.BOWChange
{
    internal class OpenBowDamagePlayer : ModPlayer
    {
        public bool OpenBowDamage = true;
        public override void ResetEffects()
        {
            OpenBowDamage = true;
        }
    }
}
