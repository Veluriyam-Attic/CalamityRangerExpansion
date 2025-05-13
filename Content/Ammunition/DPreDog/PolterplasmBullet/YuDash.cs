using CalamityMod.CalPlayer.Dashes;
using CalamityMod.Enums;
using Terraria;

namespace CalamityRangerExpansion.Content.Ammunition.DPreDog.PolterplasmBullet
{

    public class YuDash : PlayerDashEffect
    {
        public static new string ID = "Test Dash";

        public override DashCollisionType CollisionType => DashCollisionType.NoCollision;

        public override bool IsOmnidirectional => true;

        public override float CalculateDashSpeed(Player player) => 20f;

        public override void OnDashEffects(Player player)
        {
            //player.immuneTime = 90;
        }

        public override void MidDashEffects(Player player, ref float dashSpeed, ref float dashSpeedDecelerationFactor, ref float runSpeedDecelerationFactor)
        {

        }
    }
}