namespace CalamityRangerExpansion.Content.Gel.APreHardMode.AerialiteGel
{
    internal class AerialiteGelGP : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool IsAerialiteGelInfused = false;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            // 检查弹幕是否由 AerialiteGel 弹药生成
            if (source is EntitySource_ItemUse_WithAmmo ammoSource && ammoSource.AmmoItemIdUsed == ModContent.ItemType<AerialiteGel>())
            {
                // 标记为已附魔
                IsAerialiteGelInfused = true;

                // 检查场上是否已存在 AerialiteGelCloud 弹幕
                bool cloudExists = false;
                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.active && proj.type == ModContent.ProjectileType<AerialiteGelCloud>())
                    {
                        cloudExists = true;
                        break;
                    }
                }

                // 如果不存在 AerialiteGelCloud，则生成一个新的
                if (!cloudExists)
                {
                    Projectile.NewProjectile(
                        projectile.GetSource_FromThis(),
                        projectile.Center,
                        Vector2.Zero, // 无速度
                        ModContent.ProjectileType<AerialiteGelCloud>(),
                        (int)(projectile.damage * 12.0f),
                        projectile.knockBack,
                        projectile.owner
                    );
                }

                // 对玩家施加反作用力（增加速度上限逻辑和计时器逻辑）
                Player player = Main.player[projectile.owner];

                // 定义速度上限
                float speedLimit = 65f * 0.022352f * 5;

                // 检查玩家当前速度是否超过上限
                if (player.velocity.Length() > speedLimit)
                {
                    return; // 如果超过上限，则不施加后坐力
                }

                if (!player.GetModPlayer<AerialiteGelPlayer>().RecoilCooldownActive)
                {
                    Vector2 recoilForce = -projectile.velocity * 1.5f;
                    player.velocity += recoilForce;

                    // 激活冷却计时器
                    player.GetModPlayer<AerialiteGelPlayer>().StartRecoilCooldown();
                }

                // 同步网络状态
                projectile.netUpdate = true;

                projectile.damage = (int)(projectile.damage * 0.2f); // 减少 80% 伤害

            }

            base.OnSpawn(projectile, source);
        }


        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 检查弹幕是否附魔，目标是否为非友方 NPC 且仍存活
            if (IsAerialiteGelInfused && target.active && !target.friendly)
            {
                // 为目标添加 x，持续 300 帧（约 5 秒）
                //target.AddBuff(ModContent.BuffType<XXX>(), 300);
            }
        }

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(projectile, target, ref modifiers);
        }


        public override void OnKill(Projectile projectile, int timeLeft)
        {
            base.OnKill(projectile, timeLeft);
        }
    }
}

