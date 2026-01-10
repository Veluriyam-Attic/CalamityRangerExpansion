namespace CalamityRangerExpansion.Content.Gel.EAfterDog.CosmosGel
{
    public class CosmosGelGP : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool IsCosmosGelInfused = false;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_ItemUse_WithAmmo ammoSource && ammoSource.AmmoItemIdUsed == ModContent.ItemType<CosmosGel>())
            {
                IsCosmosGelInfused = true;
                projectile.netUpdate = true;
                projectile.damage = (int)(projectile.damage * 0.70f); // 减少 30% 伤害
                projectile.Kill(); // 直接删除自己


                // **检查场上已存在的 `CosmosGelEater1Head` 数量**
                int wormHeadCount = 0;
                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.active && proj.type == ModContent.ProjectileType<CosmosGelEater1Head>())
                    {
                        wormHeadCount++;
                        if (wormHeadCount >= 7)
                            return;
                    }
                }

                // **生成一条完整的蠕虫**
                SummonCosmosGelWorm(projectile);

            }
            base.OnSpawn(projectile, source);
        }

        private void SummonCosmosGelWorm(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            Vector2 spawnPos = player.Center; // 以玩家中心为起点

            // **生成头部**
            int prev = Projectile.NewProjectile(
                projectile.GetSource_FromThis(),
                spawnPos,
                Vector2.Zero, // **初始速度为 0**
                ModContent.ProjectileType<CosmosGelEater1Head>(),
                (int)(projectile.damage / 0.7 * 0.45f), // 伤害调整
                projectile.knockBack,
                projectile.owner
            );

            // **生成 6 段身体**
            for (int i = 0; i < 6; i++)
            {
                prev = Projectile.NewProjectile(
                    projectile.GetSource_FromThis(),
                    spawnPos,
                    Vector2.Zero, // **初始速度为 0**
                    ModContent.ProjectileType<CosmosGelEater2Body>(),
                    (int)(projectile.damage / 0.7 * 0.45f),
                    projectile.knockBack,
                    projectile.owner,
                    prev // 这里确保 `ai[0]` 继承前一部分的 UUID
                );
            }

            // **生成尾巴**
            Projectile.NewProjectile(
                projectile.GetSource_FromThis(),
                spawnPos,
                Vector2.Zero, // **初始速度为 0**
                ModContent.ProjectileType<CosmosGelEater3Tail>(),
                (int)(projectile.damage / 0.7 * 0.45f),
                projectile.knockBack,
                projectile.owner,
                prev // 连接最后一节身体
            );
        }


        public override void Unload()
        {



        }
  

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (IsCosmosGelInfused && target.active && !target.friendly)
            {
                // 施加 GodSlayerInferno 和 CosmosGelEDebuff
                target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 300); // 5 秒
                target.AddBuff(ModContent.BuffType<CosmosGelEDebuff>(), 90); // 1.5 秒

                // 调整伤害为原来的 70%（穿透衰减）
                //projectile.damage = (int)(projectile.damage * 0.7f);
            }
        }
    }
}
