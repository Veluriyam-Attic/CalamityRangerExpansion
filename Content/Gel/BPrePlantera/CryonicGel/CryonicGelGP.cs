namespace CalamityRangerExpansion.Content.Gel.BPrePlantera.CryonicGel
{
    public class CryonicGelGP : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool IsCryonicGelInfused = false;
        public bool IsSlowedByCryonicGel = false; // 标记弹幕是否已经被减速

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_ItemUse_WithAmmo ammoSource && ammoSource.AmmoItemIdUsed == ModContent.ItemType<CryonicGel>())
            {
                IsCryonicGelInfused = true; // 标记为附魔状态
                projectile.netUpdate = true;
                //projectile.damage = (int)(projectile.damage * 0.8f); // 减少 20% 伤害
            }
            base.OnSpawn(projectile, source);
        }  
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (IsCryonicGelInfused && target.active && !target.friendly)
            {
                // 用的是反器材大狙弹幕（AMRShot）那里的代码
                target.Calamity().miscDefenseLoss = 15;
            }
        }

        #region 旧代码
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

        //                // 确保是被 IsCryonicGelInfused 加持的我方弹幕，且仍活跃
        //                if (friendlyProjectile.active && friendlyProjectile.friendly && friendlyProjectile.GetGlobalProjectile<CryonicGelGP>().IsCryonicGelInfused)
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
