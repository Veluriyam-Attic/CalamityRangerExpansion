using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;

namespace CalamityRangerExpansion.Content.Arrows.EAfterDog.AuricArrow
{ 
    public class AuricArrowPBuff : ModBuff
    {

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true; // Buff在玩家退出后不会保存
            Main.debuff[Type] = false; // 这是一个增益效果，而不是减益
        }

        public override void Update(Player player, ref int buffIndex)
        {
            // 提升？%的全职业伤害
            player.GetDamage(DamageClass.Generic) *= 1.4f;
            // player.GetDamage(DamageClass.Generic) += 0.1f;
        }
    }
}
