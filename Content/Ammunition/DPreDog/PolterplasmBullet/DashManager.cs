using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityMod;
using CalamityMod.CalPlayer;
using CalamityMod.CalPlayer.Dashes;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace CalamityRangerExpansion.Content.Ammunition.DPreDog.PolterplasmBullet
{
    public class DashManager : ModSystem
    {

        public override void Load()
        {
            
            PlayerDashManager.TryAddDash(new YuDash());
        }
    }
}
