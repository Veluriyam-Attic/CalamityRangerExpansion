namespace CalamityRangerExpansion.Content.Gel.EAfterDog.AuricGel
{
    public class AuricGelGP : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool IsAuricGelInfused = false;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_ItemUse_WithAmmo ammoSource && ammoSource.AmmoItemIdUsed == ModContent.ItemType<AuricGel>())
            {
                IsAuricGelInfused = true;
                projectile.netUpdate = true;
                projectile.damage = (int)(projectile.damage * 0.25f); // 减少 75% 伤害
            }
            base.OnSpawn(projectile, source);
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (IsAuricGelInfused && target.active && !target.friendly)
            {
                // 检查是否已有 AuricGelLighting 存在
                if (!Main.projectile.Any(p => p.active && p.type == ModContent.ProjectileType<AuricGelLighting>()))
                {
                    // 获取屏幕边界，随机生成 10 个位置
                    Player player = Main.player[projectile.owner];
                    Rectangle screenBounds = new Rectangle((int)(player.Center.X - Main.screenWidth / 2), (int)(player.Center.Y - Main.screenHeight / 2), Main.screenWidth, Main.screenHeight);

                    for (int i = 0; i < 10; i++)
                    {
                        Vector2 spawnPosition;
                        if (Main.rand.NextBool()) // 随机选择水平或垂直边界
                        {
                            spawnPosition = new Vector2(
                                Main.rand.Next(screenBounds.Left, screenBounds.Right), // X 轴随机
                                Main.rand.NextBool() ? screenBounds.Top : screenBounds.Bottom // 顶部或底部
                            );
                        }
                        else
                        {
                            spawnPosition = new Vector2(
                                Main.rand.NextBool() ? screenBounds.Left : screenBounds.Right, // 左侧或右侧
                                Main.rand.Next(screenBounds.Top, screenBounds.Bottom) // Y 轴随机
                            );
                        }

                        Vector2 velocity = (target.Center - spawnPosition).SafeNormalize(Vector2.Zero) * 12f; // 指向目标的速度

                        int lightningProjectile = Projectile.NewProjectile(
                            projectile.GetSource_FromThis(),
                            spawnPosition,
                            velocity,
                            ModContent.ProjectileType<AuricGelLighting>(),
                            (int)(projectile.damage / 0.25 * 0.75f), // 75% 的伤害
                            0f,
                            projectile.owner,
                            velocity.ToRotation() // 将目标位置方向传递为初始旋转角度
                        );

                        // 设置属性
                        Projectile proj = Main.projectile[lightningProjectile];
                        proj.friendly = true;
                        proj.hostile = false;
                        proj.penetrate = -1;
                        proj.localNPCHitCooldown = 60;
                        proj.usesLocalNPCImmunity = true;
                        proj.DamageType = DamageClass.Ranged; // 改为射手伤害类型
                    }
                    SoundEngine.PlaySound(CommonCalamitySounds.LightningSound, projectile.Center);
                }
            }
        }
    }
}
