namespace CalamityRangerExpansion.Content.Gel.CPreMoodLord.LivingShardGel
{
    internal class LivingShardGelGP : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool IsLivingShardGelInfused = false;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_ItemUse_WithAmmo ammoSource && ammoSource.AmmoItemIdUsed == ModContent.ItemType<LivingShardGel>())
            {
                IsLivingShardGelInfused = true;
                projectile.netUpdate = true;
            }
            base.OnSpawn(projectile, source);
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (IsLivingShardGelInfused && target.active && !target.friendly)
            {
                // 检查场上是否已有超过 1 个 某种 弹幕
                int sparkCount = 0;
                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.active && proj.type == ModContent.ProjectileType<LivingShardGelHealPROJ>())
                    {
                        sparkCount++;
                        if (sparkCount >= 1)
                            return; // 如果已存在 1 个 某种 弹幕，则不释放新的
                    }
                }

                // 2% 概率释放 LivingShardGelHealPROJ
                if (Main.rand.NextFloat() <= 0.02f)
                {
                    Vector2 randomDirection = Main.rand.NextVector2CircularEdge(1f, 1f).SafeNormalize(Vector2.Zero) * 10f;
                    Projectile.NewProjectile(
                        projectile.GetSource_FromThis(),
                        projectile.Center,
                        randomDirection,
                        ModContent.ProjectileType<LivingShardGelHealPROJ>(),
                        (int)(projectile.damage * 2.5f), // 250% 伤害
                        projectile.knockBack,
                        projectile.owner
                    );
                }
            }
        }
    }
}