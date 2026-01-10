namespace CalamityRangerExpansion.Content.WeaponToAMMO.Arrow.Terratoarrow
{
    internal class TerratoarrowPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "WeaponToAMMO.Arrow.Terratoarrow";
        public override string Texture => "CalamityRangerExpansion/Content/WeaponToAMMO/Arrow/Terratoarrow/Terratoarrow";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 画残影效果
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
                return false;
            }
            return true;
        }
        public override void SetDefaults()
        {
            // 设置弹幕的基础属性
            Projectile.width = 11; // 弹幕宽度
            Projectile.height = 24; // 弹幕高度
            Projectile.friendly = true; // 对敌人有效
            Projectile.DamageType = DamageClass.Ranged; // 远程伤害类型
            Projectile.penetrate = 1; // 穿透力为1，击中一个敌人就消失
            Projectile.timeLeft = 20; // 弹幕存在时间为x帧
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
            Projectile.ignoreWater = true; // 弹幕不受水影响
            Projectile.arrow = true;
            Projectile.extraUpdates = 4;
        }

        public override void AI()
        {
            // 调整弹幕的旋转，使其在飞行时保持水平
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;
            Lighting.AddLight(Projectile.Center, Color.Green.ToVector3() * 0.49f);
            if (Projectile.ai[0] == 0)
            {
                //Projectile.damage = (int)(Projectile.damage * 0.3f);
                Projectile.velocity *= 0.5f;
                LineParticle spark = new LineParticle(Projectile.Center + Projectile.velocity * 4, Projectile.velocity * 4.95f, false, 9, 2.4f, Color.LimeGreen);
                GeneralParticleHandler.SpawnParticle(spark);
            }
            Projectile.ai[0]++;

            if (Projectile.timeLeft == 1)
            {
                // 基础方向为弹幕当前的朝向
                Vector2 baseDirection = Projectile.velocity.SafeNormalize(Vector2.UnitX);

                // 动态调整发射数量
                int arrowCount = Main.getGoodWorld ? 9 : 3; // getGoodWorld 启用时 9 发，否则 3 发

                // 动态计算夹角范围
                float maxAngle = MathHelper.ToRadians(5); // 最大偏移角度（5度）
                float minAngle = -maxAngle; // 最小偏移角度（-5度）

                // 释放多个弹幕
                for (int i = 0; i < arrowCount; i++)
                {
                    // 在角度范围内随机抽取
                    float randomAngle = Main.rand.NextFloat(minAngle, maxAngle);
                    Vector2 randomDirection = baseDirection.RotatedBy(randomAngle);

                    // 随机速度，浮动范围为原速度的 0.5 ~ 1.75 倍
                    float randomSpeedMultiplier = Main.rand.NextFloat(0.5f, 1.75f);
                    Vector2 randomVelocity = randomDirection * (Projectile.velocity.Length() * randomSpeedMultiplier);

                    // 创建新的弹幕
                    Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        Projectile.Center,         // 起始位置
                        randomVelocity,            // 随机方向和速度
                        ModContent.ProjectileType<TerratoarrowSPIT>(),
                        (int)(Projectile.damage * 0.75), // 伤害倍率为 0.75 倍
                        Projectile.knockBack,      // 保持相同的击退效果
                        Projectile.owner           // 投射物归属
                    );
                }

                // 生成粒子效果
                for (int i = 0; i < arrowCount; i++)
                {
                    PointParticle spark = new PointParticle(
                        Projectile.Center,
                        baseDirection * Main.rand.NextFloat(1f, 2f), // 粒子速度稍微随机化
                        false,
                        5,
                        1.1f,
                        Color.LimeGreen
                    );
                    GeneralParticleHandler.SpawnParticle(spark);
                }

                // 手动删除弹幕
                Projectile.Kill();
            }




        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // 检查弹幕的剩余时间
            // if (Projectile.timeLeft >= 10)
            {
                // 增加 2.5 倍伤害
                modifiers.FinalDamage *= 3.5f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            LightingBoltsSystem.Spawn_AncientForestWisdom(Projectile.Center);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            // 消亡时释放X色爆炸特效
            Particle blastRing = new CustomPulse(
                Projectile.Center, Vector2.Zero, Color.LimeGreen,
                "CalamityMod/Particles/FlameExplosion",
                Vector2.One * 0.22f, Main.rand.NextFloat(-10f, 10f),
                0.07f, 0.33f, 30
            );
            GeneralParticleHandler.SpawnParticle(blastRing);
        }







    }
}