namespace CalamityRangerExpansion.Content.Ammunition.CPreMoodLord.ScoriaBullet
{
    public class ScoriaBulletPlayer : ModPlayer
    {
        public int StackCount = 0; // 层数
        private int disableBuffTimer = 0; // 受击冷却

        public override void ResetEffects()
        {
            if (disableBuffTimer > 0)
                disableBuffTimer--; // 受击冷却递减
        }

        private int hitCounter = 0; // 追踪命中次数

        public void IncreaseStackCount()
        {
            if (disableBuffTimer > 0) return; // 冷却期间不增加

            hitCounter++; // 记录命中次数

            if (hitCounter >= 50) // 每 50 次命中增加 1 层
            {
                if (StackCount < 10) // 最大 10 层
                {
                    StackCount++;

                    // 以玩家为中心，生成 4~6 个纯视觉弹幕
                    int projectileCount = Main.rand.Next(4, 7); // 随机 4~6 个
                    for (int i = 0; i < projectileCount; i++)
                    {
                        Vector2 randomOffset = Main.rand.NextVector2Circular(5 * 16, 5 * 16); // 半径 5×16 范围内随机点
                        Vector2 spawnPosition = Player.Center + randomOffset; // 计算生成位置

                        Projectile.NewProjectile(
                            Player.GetSource_FromThis(),
                            spawnPosition,        // 生成位置
                            Vector2.Zero,         // 无速度，静止
                            ModContent.ProjectileType<FuckYou>(), // 视觉弹幕
                            0,                    // 伤害为 0
                            0f,                   // 无击退
                            Player.whoAmI         // 设置拥有者
                        );
                    }
                }

                // 刷新 Buff 10 秒
                Player.AddBuff(ModContent.BuffType<ScoriaBulletPBuff>(), 600);
                hitCounter = 0; // 重置计数
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
            StackCount = 0; // 清空层数
            Player.ClearBuff(ModContent.BuffType<ScoriaBulletPBuff>());
            disableBuffTimer = 300; // 受击后 5 秒内不能叠加
        }
    }
}
