namespace CalamityRangerExpansion.Content.Gel.EAfterDog.CosmosGel
{
    public class CosmosGelEDebuff : ModBuff, ILocalizedModType
    {
        public new string LocalizationCategory => "Buffs";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true; // 定义为Debuff
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            if (npc.life > 0)
            {
                // 每帧减少1000点生命值
                npc.life -= 1000;
                if (npc.life <= 0)
                {
                    npc.life = 0;
                    npc.checkDead(); // 检查是否死亡
                }
            }
        }
    }
}
