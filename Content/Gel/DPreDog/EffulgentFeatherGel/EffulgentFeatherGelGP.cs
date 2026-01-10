namespace CalamityRangerExpansion.Content.Gel.DPreDog.EffulgentFeatherGel
{
    internal class EffulgentFeatherGelGP : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool IsEffulgentFeatherGelInfused = false;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_ItemUse_WithAmmo ammoSource && ammoSource.AmmoItemIdUsed == ModContent.ItemType<EffulgentFeatherGel>())
            {
                IsEffulgentFeatherGelInfused = true;
                projectile.netUpdate = true;
                projectile.damage = (int)(projectile.damage * 0.95f); // 减少 5% 伤害
            }
            base.OnSpawn(projectile, source);
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (IsEffulgentFeatherGelInfused && target.active && !target.friendly)
            {
                // 施加 Electrified（带电）Buff，持续 300 帧
                target.AddBuff(BuffID.Electrified, 300);

                // 检查场上是否已有超过 6 个 Spark 弹幕
                int sparkCount = 0;
                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.active && proj.type == ModContent.ProjectileType<Spark>())
                    {
                        sparkCount++;
                        if (sparkCount >= 6)
                            return; // 如果已存在 6 个 Spark 弹幕，则不释放新的
                    }
                }

                // 随机方向释放 2 个 Spark 弹幕
                for (int i = 0; i < 2; i++)
                {
                    float randomAngle = MathHelper.ToRadians(Main.rand.Next(0, 360));
                    Vector2 velocity = new Vector2((float)Math.Cos(randomAngle), (float)Math.Sin(randomAngle)) * 6f; // 初速度为 6f

                    Projectile.NewProjectile(
                        projectile.GetSource_FromThis(),
                        projectile.Center,
                        velocity,
                        ModContent.ProjectileType<Spark>(),
                        (int)(projectile.damage / 0.95 * 0.33f), // 伤害为原弹幕的 33%
                        projectile.knockBack,
                        projectile.owner
                    );
                }
            }
        }
    }
}
