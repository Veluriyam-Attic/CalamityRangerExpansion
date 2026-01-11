namespace CalamityRangerExpansion.Content.WeaponToAMMO.Bullet.NorthStar
{
    public class PolarisBuff : ModBuff
    {
        public new string LocalizationCategory => "Buffs";

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen += 2;
            //player.endurance = 99f;
            PPPlayer modPlayer = player.GetModPlayer<PPPlayer>();
            modPlayer.polarisBoost = true;
        }
    }
}
