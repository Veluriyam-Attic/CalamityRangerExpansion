using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityMod;
using CalamityMod.CalPlayer.Dashes;
using Terraria;
using Terraria.ModLoader;

namespace CalamityRangerExpansion.Content.Ammunition.DPreDog.PolterplasmBullet
{
    public class DashManager : ModPlayer
    {
        public override void ResetEffects()
        {
            if (Player.GetModPlayer<PolterplasmBulletDASH>().canDash)
            {
                Player.Calamity().DashID = PolterplasmBulletDASH.ID;
            }
        }

        #region 旧代码


        //// 检查是否启用了 PolterplasmBulletDASH
        //public static bool IsPolterplasmDashEnabled(Player player)
        //{
        //    return player.GetModPlayer<PolterplasmBulletDASH>().canDash;
        //}

        //// 获取当前可用的冲刺效果
        //public static PlayerDashEffect GetCurrentDashEffect(Player player)
        //{
        //    if (IsPolterplasmDashEnabled(player))
        //    {
        //        return new PolterplasmBulletDASH();
        //    }

        //    // 默认返回 DefaultDash
        //    return new DefaultDash();
        //}
        #endregion
    }
}
