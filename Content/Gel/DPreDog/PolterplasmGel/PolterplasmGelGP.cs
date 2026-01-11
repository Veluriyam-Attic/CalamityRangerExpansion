namespace CalamityRangerExpansion.Content.Gel.DPreDog.PolterplasmGel
{
    internal class PolterplasmGelGP : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool IsPolterplasmGelInfused = false;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_ItemUse_WithAmmo ammoSource && ammoSource.AmmoItemIdUsed == ModContent.ItemType<PolterplasmGel>())
            {
                IsPolterplasmGelInfused = true;
                projectile.netUpdate = true;
                projectile.damage = (int)(projectile.damage * 0.8f); // 减少 20% 伤害
            }
            base.OnSpawn(projectile, source);
        }
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (IsPolterplasmGelInfused && target.active && !target.friendly)
            {
                // 禁用追踪逻辑
                projectile.ai[2] = 1f; // 标记不再追踪

            }
        }
        public override void AI(Projectile projectile)
        {
            if (IsPolterplasmGelInfused)
            {
                if (projectile.ai[1] <= 1)
                {
                    projectile.ai[1]++;
                    return;
                }

                // 只有在 `ai[2] == 0` 时才追踪，确保击中敌人后不会继续追踪
                if (projectile.ai[2] == 0)
                {
                    // 查找目标
                    NPC target = projectile.Center.ClosestNPCAt(1050); // 在1050像素范围内寻找最近的敌人
                    if (target != null)
                    {
                        Vector2 direction = (target.Center - projectile.Center).SafeNormalize(Vector2.Zero); // 目标方向
                        float desiredRotation = direction.ToRotation(); // 目标方向的旋转角度
                        float currentRotation = projectile.velocity.ToRotation(); // 当前速度方向的旋转角度
                        float rotationDifference = MathHelper.WrapAngle(desiredRotation - currentRotation); // 封装角度差到[-π, π]

                        // 每帧最大旋转角度限制为 1~4 度
                        float rotationAmount = MathHelper.ToRadians(Main.rand.Next((int)1, (int)2)); // 随机生成 1~4 度
                        if (Math.Abs(rotationDifference) < rotationAmount)
                        {
                            rotationAmount = rotationDifference; // 如果差值小于限制，则直接旋转到目标方向
                        }

                        // 根据角度差的正负值决定旋转方向
                        projectile.velocity = projectile.velocity.RotatedBy(rotationAmount * Math.Sign(rotationDifference));
                    }
                }
            }
        }
    }
}
