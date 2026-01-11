namespace CalamityRangerExpansion.Content.Gel.DPreDog.UelibloomGel
{
    public class UelibloomGelGP : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool IsUelibloomGelInfused = false;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_ItemUse_WithAmmo ammoSource && ammoSource.AmmoItemIdUsed == ModContent.ItemType<UelibloomGel>())
            {
                IsUelibloomGelInfused = true;
                projectile.damage = (int)(projectile.damage * 0.75f); // 减少 25% 伤害
                projectile.netUpdate = true;
            }
            base.OnSpawn(projectile, source);
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (IsUelibloomGelInfused && target.active && !target.friendly)
            {
                // 在敌人四面八方随机生成 UelibloomArrowLight 弹幕
                for (int i = 0; i < 2; i++)
                {
                    // 随机选择一个方向
                    float randomAngle = MathHelper.ToRadians(Main.rand.Next(0, 360));
                    Vector2 spawnPosition = target.Center + new Vector2((float)Math.Cos(randomAngle), (float)Math.Sin(randomAngle)) * 30 * 16f;
                    Vector2 directionToTarget = Vector2.Normalize(target.Center - spawnPosition); // 改为指向命中敌人的位置

                    // 创建 UelibloomArrowLight 弹幕
                    Projectile.NewProjectile(
                        projectile.GetSource_FromThis(),
                        spawnPosition,
                        directionToTarget * 10f, // 设置飞行速度
                        ModContent.ProjectileType<UelibloomArrowLight>(),
                        (int)(projectile.damage * 0.4f),
                        projectile.knockBack,
                        projectile.owner
                    );
                }
            }
        }
    }
}
