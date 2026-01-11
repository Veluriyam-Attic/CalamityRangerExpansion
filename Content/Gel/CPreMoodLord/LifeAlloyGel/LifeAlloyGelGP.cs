namespace CalamityRangerExpansion.Content.Gel.CPreMoodLord.LifeAlloyGel
{
    internal class LifeAlloyGelGP : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool IsLifeAlloyGelInfused = false;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_ItemUse_WithAmmo ammoSource && ammoSource.AmmoItemIdUsed == ModContent.ItemType<LifeAlloyGel>())
            {
                IsLifeAlloyGelInfused = true;
                projectile.netUpdate = true;
                projectile.damage = (int)(projectile.damage * 0.80f); // 减少 20% 伤害
            }
            base.OnSpawn(projectile, source);
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (IsLifeAlloyGelInfused && target.active && !target.friendly)
            {
                // 随机生成 2 到 8 个 HyperiusSplit 弹幕
                int splitCount = Main.rand.Next(2, 9);
                for (int b = 0; b < splitCount; b++)
                {
                    Projectile.NewProjectile(
                        projectile.GetSource_FromThis(),
                        projectile.Center,
                        projectile.velocity.RotatedByRandom(0.5f) * 0.2f,
                        ModContent.ProjectileType<HyperiusSplit>(),
                        (int)(projectile.damage * 0.05f), // 伤害
                        0f,
                        projectile.owner,
                        ai0: 0f,
                        ai1: 0f,
                        ai2: Main.rand.Next(0, 5) // 随机生成 0 到 4 的数值，决定颜色
                    );
                }

                #region 旧代码
                //// 熔渣
                //// 启用 ScoriaGelGN 的标记和计时器
                //target.GetGlobalNPC<ScoriaGelGN>().IsMarkedByScoriaGel = true;
                //target.GetGlobalNPC<ScoriaGelGN>().MarkDuration = 300; // 持续 5 秒

                //// 永恒
                //// 给所有玩家添加 1200 帧的 PerennialGelPBuff
                //foreach (Player player in Main.player)
                //{
                //    if (player.active)
                //    {
                //        player.AddBuff(ModContent.BuffType<PerennialGelPBuff>(), 1200);
                //    }
                //}

                //// 寒元
                //// 施加 CryonicGelEDebuff，持续 300 帧（5 秒）
                //target.AddBuff(ModContent.BuffType<CryonicGelEDebuff>(), 300);
                #endregion
            }
        }
        #region 旧代码
        // 寒元
        //public override void ModifyDamageHitbox(Projectile projectile, ref Rectangle hitbox)
        //{
        //    if (IsLifeAlloyGelInfused)
        //    {
        //        foreach (Projectile enemyProjectile in Main.projectile)
        //        {
        //            if (enemyProjectile.active && enemyProjectile.hostile && projectile.Hitbox.Intersects(enemyProjectile.Hitbox))
        //            {
        //                enemyProjectile.velocity *= 0.6f; // 减速 40%

        //                // 如果需要 5 秒效果，则可以使用 AI 或额外逻辑追踪时间
        //                enemyProjectile.localAI[0] = 300; // 标记剩余帧数
        //            }
        //        }
        //    }
        //}

        // 寒元
        //public override void AI(Projectile projectile)
        //{
        //    foreach (Projectile enemyProjectile in Main.projectile)
        //    {
        //        if (enemyProjectile.active && enemyProjectile.hostile && enemyProjectile.localAI[0] > 0)
        //        {
        //            enemyProjectile.localAI[0]--;
        //            if (enemyProjectile.localAI[0] <= 0)
        //            {
        //                //enemyProjectile.velocity /= 0.6f; // 恢复速度
        //            }
        //        }
        //    }
        //}

        // 寒元（新逻辑会导致卡顿）
        //public override void AI(Projectile projectile)
        //{
        //    // 遍历所有的敌方弹幕
        //    for (int i = 0; i < Main.maxProjectiles; i++)
        //    {
        //        Projectile enemyProjectile = Main.projectile[i];

        //        // 检查条件：敌方弹幕（非友方）且尚未被减速（通过 ai[1] 标记）且仍活跃
        //        if (enemyProjectile.active && !enemyProjectile.friendly && enemyProjectile.ai[1] != 1)
        //        {
        //            // 再次遍历所有我方弹幕
        //            for (int j = 0; j < Main.maxProjectiles; j++)
        //            {
        //                Projectile friendlyProjectile = Main.projectile[j];

        //                // 确保是被 IsLifeAlloyGelInfused 加持的我方弹幕，且仍活跃
        //                if (friendlyProjectile.active && friendlyProjectile.friendly && friendlyProjectile.GetGlobalProjectile<LifeAlloyGelGP>().IsLifeAlloyGelInfused)
        //                {
        //                    // 检测碰撞
        //                    if (Collision.CheckAABBvAABBCollision(
        //                            new Vector2(friendlyProjectile.Hitbox.X, friendlyProjectile.Hitbox.Y),
        //                            new Vector2(friendlyProjectile.Hitbox.Width, friendlyProjectile.Hitbox.Height),
        //                            new Vector2(enemyProjectile.Hitbox.X, enemyProjectile.Hitbox.Y),
        //                            new Vector2(enemyProjectile.Hitbox.Width, enemyProjectile.Hitbox.Height)))
        //                    {
        //                        // 处理敌方弹幕减速
        //                        enemyProjectile.velocity *= 0.6f; // 减速为原来的 60%
        //                        enemyProjectile.ai[1] = 1; // 标记为已减速，避免重复触发
        //                        enemyProjectile.netUpdate = true; // 同步网络数据
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}
        #endregion
    }
}
