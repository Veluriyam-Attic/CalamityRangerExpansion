namespace CalamityRangerExpansion.Content.Arrows.DPreDog.DivineGeodeArrow
{
    internal class DivineGeodeArrowPBuff : ModBuff, ILocalizedModType
    {
        public new string LocalizationCategory => "Buffs";
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false; // Buff 显示时间
        }

        public override void Update(Player player, ref int buffIndex)
        {
            // 增加飞行时间
            //player.wingTimeMax = (int)(player.wingTimeMax * 1.125f); // 增加 12.5% 的最大飞行时间
            player.GetModPlayer<DivineGeodeArrowPlayer>().HasDivineGeodeBuff = true;

            // 每帧生成金黄色粒子特效
            if (Main.rand.NextBool(1)) // x% 概率生成粒子
            {
                GenericSparkle impactParticle = new GenericSparkle(
                    player.Center + Main.rand.NextVector2Circular(20f, 20f), // 随机偏移，围绕玩家
                    Vector2.Zero, // 粒子无初始速度
                    Color.Goldenrod, // 金黄色
                    Color.White, // 白色渐变
                    Main.rand.NextFloat(0.7f, 1.2f), // 粒子大小
                    8 // 粒子生命周期
                );
                //GeneralParticleHandler.SpawnParticle(impactParticle); // 生成粒子
            }
        }
    }
}
