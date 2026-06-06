using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace CalamityRangerExpansion.Content.BOWChange.Global
{
    internal class CBRKeys : ModSystem
    {
        public static ModKeybind ChangeBow { get; private set; }

        public override void Load()
        {
            ChangeBow = KeybindLoader.RegisterKeybind(Mod, "替换手上武器", "P");
        }

        public override void Unload()
        {
            ChangeBow = null;
        }
    }
}