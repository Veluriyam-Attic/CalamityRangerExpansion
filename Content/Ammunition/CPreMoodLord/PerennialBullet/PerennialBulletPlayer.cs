namespace CalamityRangerExpansion.Content.Ammunition.CPreMoodLord.PerennialBullet
{
    public class PerennialBulletPlayer : ModPlayer
    {
      

        public int StackCount; // 防御力堆叠层数
        private int disableBuffTimer = 0; // 禁用计时器

        public override void ResetEffects()
        {
            // 每帧减少计时器
            if (disableBuffTimer > 0)
            {
                disableBuffTimer--;
            }
        }


        private int increaseStackCountCalls = 0; // 追踪调用次数

        public void IncreaseStackCount()
        {
            // 如果禁用计时器生效，直接返回
            if (disableBuffTimer > 0) return;

            increaseStackCountCalls++; // 每次调用增加计数

            if (increaseStackCountCalls >= 50) // 当调用次数达到 50 时触发效果
            {
                if (StackCount < 10) // 等级最多提升至 10
                {
                    StackCount++; // 提升一级
                    int maxHealthIncrease = Main.getGoodWorld ? 100 : 10; // 每级增加的生命值
                    //Player.statLifeMax2 += maxHealthIncrease; // 提升最大生命值
                }
                Player.AddBuff(ModContent.BuffType<PerennialBulletPBuff>(), 600); // 刷新 10 秒的 Buff

                // 生成心形扩散粒子效果
                for (int i = 0; i < 40; i++) // 总粒子数 40
                {
                    double angle = Math.PI * 2 * i / 40; // 计算心形的点位置
                    float x = (float)(16 * Math.Sin(angle) * Math.Sin(angle) * Math.Sin(angle)); // x 坐标公式
                    float y = (float)(13 * Math.Cos(angle) - 5 * Math.Cos(2 * angle) - 2 * Math.Cos(3 * angle) - Math.Cos(4 * angle)); // y 坐标公式
                    Vector2 particlePosition = new Vector2(x, y) * 0.5f; // 缩放心形大小

                    Vector2 particleVelocity = particlePosition * Main.rand.NextFloat(0.5f, 1.5f); // 速度沿心形方向扩散
                    Dust dust = Dust.NewDustPerfect(
                        Player.Center + particlePosition, // 以玩家为中心
                        DustID.GreenFairy,                // 绿色粒子
                        particleVelocity,                 // 粒子速度
                        150,                              // 透明度
                        Color.LightGreen,                 // 粒子颜色
                        Main.rand.NextFloat(1.2f, 1.6f)   // 粒子大小
                    );
                    dust.noGravity = true; // 粒子不受重力影响
                }
                LightingBoltsSystem.Spawn_FlowerPattern(Player.Center);

                // 重置调用计数器
                increaseStackCountCalls = 0;
            }
        }


        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            ClearBuffOnHit();
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            ClearBuffOnHit();
        }

        private void ClearBuffOnHit()
        {
            // 清除 Buff 和堆叠层数
            StackCount = 0;
            Player.ClearBuff(ModContent.BuffType<PerennialBulletPBuff>());

            // 启动禁用计时器，5秒 = 300帧
            disableBuffTimer = 300;
        }
    }
}
