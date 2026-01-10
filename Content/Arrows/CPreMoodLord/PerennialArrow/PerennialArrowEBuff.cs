namespace CalamityRangerExpansion.Content.Arrows.CPreMoodLord.PerennialArrow
{
    public class PerennialArrowEBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<PerennialArrowGlobalNPC>().damageMultiplier = 1.1f;

            // 每帧有一定几率生成粒子特效
            if (Main.rand.NextBool(5)) // 20% 概率生成粒子
            {
                // 粒子的生成位置在敌人中心附近随机偏移
                Vector2 dustPosition = npc.Center + new Vector2(Main.rand.NextFloat(-npc.width / 2, npc.width / 2), Main.rand.NextFloat(-npc.height / 2, npc.height / 2));

                // 创建绿色的粒子特效
                Dust dust = Dust.NewDustPerfect(dustPosition, DustID.Grass, Main.rand.NextVector2Circular(1f, 1f) * 2f, 100, Color.Green, Main.rand.NextFloat(1f, 1.5f));
                dust.noGravity = false; // 受重力影响
                dust.velocity *= 0.5f;  // 随机的速度
                dust.scale = Main.rand.NextFloat(0.8f, 1.2f); // 粒子的随机大小
                dust.noLight = true; // 不发光
            }
        }
    }
}