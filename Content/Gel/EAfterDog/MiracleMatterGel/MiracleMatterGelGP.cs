namespace CalamityRangerExpansion.Content.Gel.EAfterDog.MiracleMatterGel
{
    public class MiracleMatterGelGP : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool IsMiracleMatterGelInfused = false;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_ItemUse_WithAmmo ammoSource && ammoSource.AmmoItemIdUsed == ModContent.ItemType<MiracleMatterGel>())
            {
                IsMiracleMatterGelInfused = true;
                projectile.damage = (int)(projectile.damage * 0.2f); // 减少 95% 伤害
                projectile.netUpdate = true;
            }
            base.OnSpawn(projectile, source);
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (IsMiracleMatterGelInfused && target.active && !target.friendly)
            {
                // 检查场上是否已存在 MiracleMatterGelLighting 弹幕
                bool lightningExists = Main.projectile.Any(p => p.active && p.type == ModContent.ProjectileType<MiracleMatterGelLighting>());

                if (!lightningExists)
                {
                    // 设置闪电生成位置
                    Vector2 lightningSpawnPosition = projectile.Center - Vector2.UnitY * Main.rand.NextFloat(960f, 1020f);

                    // 设置闪电飞行方向为目标被命中的位置
                    Vector2 lightningTargetPosition = target.Center;
                    Vector2 lightningShootVelocity = (lightningTargetPosition - lightningSpawnPosition).SafeNormalize(Vector2.Zero) * 14f;

                    // 创建闪电弹幕
                    int lightning = Projectile.NewProjectile(
                        projectile.GetSource_FromThis(),
                        lightningSpawnPosition,
                        lightningShootVelocity,
                        ModContent.ProjectileType<MiracleMatterGelLighting>(),
                        (int)(0.8 * projectile.damage / 0.2 ),
                        0f,
                        projectile.owner
                    );

                    if (Main.projectile.IndexInRange(lightning))
                    {
                        Main.projectile[lightning].ai[0] = lightningShootVelocity.ToRotation(); // 设置闪电方向
                        Main.projectile[lightning].ai[1] = Main.rand.Next(100); // 随机参数（可选）
                    }

                    // 播放音效
                    SoundEngine.PlaySound(SoundID.Item92, projectile.position);
                }
            }
        }
    }
}
