namespace CalamityRangerExpansion.Content.Gel.BPrePlantera.StarblightSootGel
{
    public class StarblightSootGelPlayer : ModPlayer
    {
        private int hitCount = 0;
        private int damageTier = 0;
        private int lastHitTimer = 300; // 5 秒计时器

        private Item lastWeapon; // 记录上一把武器

        // 只要玩家切换的武器，无论切换了什么都会直接清空
        public override void UpdateEquips()
        {
            // 检查玩家手中的武器是否发生变化
            if (Player.HeldItem != lastWeapon)
            {
                lastWeapon = Player.HeldItem; // 更新记录的武器
                ClearDamageTiers(); // 清空等级
            }
        }

        private void ClearDamageTiers()
        {
            // 清空所有等级
            hitCount = 0;
            damageTier = 0;
            lastHitTimer = 0; // 取消计时
        }

        public override void ResetEffects()
        {
            if (lastHitTimer > 0)
            {
                lastHitTimer--;
            }
            else if (damageTier > 0)
            {
                DecreaseDamageTier(); // 如果连续5秒没有击中敌人，直接清空所有等级
            }
        }

        public void NotifyHit()
        {
            hitCount++;
            lastHitTimer = 300; // 重置计时器

            if (hitCount >= 20)
            {
                hitCount = 0;
                IncreaseDamageTier();
            }
        }

        public override void ModifyWeaponDamage(Item item, ref StatModifier damage)
        {
            // 根据当前层级动态增加远程伤害
            if (item.DamageType == DamageClass.Ranged && damageTier > 0)
            {
                //damage *= (1+(damageTier * 0.1f)); // 每层级增加 10% 远程伤害+
                // 每层级远程伤害 乘以1.1倍
                damage *= (float)Math.Pow(1.1f, damageTier);
            }
        }

        private void IncreaseDamageTier()
        {
            if (damageTier < 6)
            {
                damageTier++;
                // 显示向上箭头粒子
                // CreateParticleArrow(true); 
            }
        }

        private void DecreaseDamageTier()
        {
            if (damageTier > 0)
            {
                // 显示向下箭头粒子
                // CreateParticleArrow(false);

                // 直接清空所有等级
                damageTier = 0;
            }
        }

        private void CreateParticleArrow(bool upward)
        {
            Color particleColor = Color.Cyan;
            Vector2 playerCenter = Player.Center;
            int particleCount = 30; // 增加粒子数量
            float lengthMultiplier = 15f; // 加长粒子线段

            for (int i = 0; i < particleCount; i++)
            {
                float angle = upward
                    ? (i < particleCount / 2 ? MathHelper.ToRadians(-105) : MathHelper.ToRadians(-165))
                    : (i < particleCount / 2 ? MathHelper.ToRadians(105) : MathHelper.ToRadians(165));

                Vector2 offset = new Vector2(i % (particleCount / 2), i % (particleCount / 2))
                                 * Main.rand.NextFloat(0.5f, 1.5f) * lengthMultiplier;
                Vector2 spawnPos = playerCenter + offset.RotatedBy(angle);
                Vector2 velocity = new Vector2(0, upward ? -1f : 1f); // 上升或下降速度

                Dust dust = Dust.NewDustPerfect(spawnPos, DustID.Electric, velocity, 100, particleColor, 1.2f);
                dust.noGravity = true;
                dust.fadeIn = 0.1f;
                dust.scale = Main.rand.NextFloat(0.8f, 1.2f);
            }
        }
    }

}
