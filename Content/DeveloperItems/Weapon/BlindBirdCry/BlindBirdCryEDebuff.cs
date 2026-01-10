namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.BlindBirdCry
{
    internal class BlindBirdCryEDebuff : ModBuff, ILocalizedModType
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj"; // 使用透明贴图
        public new string LocalizationCategory => "Buffs";
        private int summonCounter = 0;
        private int baseDamage;

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            // 敌人攻击力
            npc.damage = (int)(npc.damage * 0.85);       
        }    
    }
}
