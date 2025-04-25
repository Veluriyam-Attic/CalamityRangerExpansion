using CalamityMod.CalPlayer.Dashes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CalamityRangerExpansion.Content.Ammunition.DPreDog.PolterplasmBullet
{
    public class PolterplasmBulletFlowerPBuff : ModBuff, ILocalizedModType
    {
        public new string LocalizationCategory => "Buffs";

        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false; // 显示剩余时间
            Main.debuff[Type] = false;           // 不是负面效果
        }

        public override void Update(Player player, ref int buffIndex)
        {
            // 打开冲刺功能
            player.GetModPlayer<PolterplasmBulletDASH>().canDash = true;
        }
    }
}