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
    }

    public class YuDashPlayer : ModPlayer
    {
        public readonly static int YuDashCD = 20;

        public static int YuLastDash = 0;

        public override void PostUpdateEquips()
        {
            Player player = Main.LocalPlayer;
            if (player.HasBuff<PolterplasmBulletFlowerPBuff>() && YuLastDash == 0)
            {
                CalamityPlayer modplayer = player.Calamity();
                modplayer.DashID = YuDash.ID;
            }
        }
    }
}