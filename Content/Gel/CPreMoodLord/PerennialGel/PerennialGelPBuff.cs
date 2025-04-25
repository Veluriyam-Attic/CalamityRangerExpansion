using CalamityRangerExpansion.Content.Ammunition.CPreMoodLord.PerennialBullet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;

namespace CalamityRangerExpansion.Content.Gel.CPreMoodLord.PerennialGel
{
    internal class PerennialGelPBuff : ModBuff, ILocalizedModType
    {
        public new string LocalizationCategory => "Buffs";
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true; // 不保存 Buff
            Main.debuff[Type] = false;   // Buff 是增益效果
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen += 6;
        }
    }

}
