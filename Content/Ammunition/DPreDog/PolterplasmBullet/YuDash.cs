using CalamityMod.CalPlayer.Dashes;
using CalamityMod.Enums;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Enums;

namespace CalamityRangerExpansion.Content.Ammunition.DPreDog.PolterplasmBullet
{

    public class YuDash : PlayerDashEffect
    {
        public static new string cooldownID => "YuDash";
        public static int cooldown = 30;

        public static new string ID => "Test Dash";

        public override DashCollisionType CollisionType => DashCollisionType.NoCollision;

        public override bool IsOmnidirectional => false;

        public override float CalculateDashSpeed(Player player) => 40f;


        public int Time;

        public override void OnDashEffects(Player player)
        {
            player.immuneTime = 30;
            Time = 0;
            //player.AddCooldown(cooldownID, CalamityUtils.SecondsToFrames(cooldown));
        }

        public override void MidDashEffects(Player player, ref float dashSpeed, ref float dashSpeedDecelerationFactor, ref float runSpeedDecelerationFactor)
        {
            YuDashPlayer.YuLastDash++;
            Time++;
            int DustType1 = 20;
            int DustNum = 5;
            int[] dust = new int[DustNum-1];
            for (int i = 0; i < DustNum-1; i++)
            {
                if(Time < 90)
                {
                    Dust.NewDust(player.position, player.width, player.height, DustType1, player.velocity.X / 2, player.velocity.Y / 2, 1, new Color(140, 159, 255), 1);
                }
                else
                {
                    Time = 90;
                }
            }
            if(YuDashPlayer.YuLastDash >= YuDashPlayer.YuDashCD)
            {
                YuDashPlayer.YuLastDash = 0;
            }
        }
    }
}