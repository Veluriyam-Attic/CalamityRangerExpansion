namespace CalamityRangerExpansion.Content.Ammunition.DPreDog.ToothBullet
{
    internal class ToothBulletPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectile.DPreDog";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
                return false;
            }
            return true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.MaxUpdates = 5;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;
        }

        public override void AI()
        {
            // 由于我们是水平贴图，因此什么也不需要转动
            Projectile.rotation = Projectile.velocity.ToRotation();

            // 添加光效
            Lighting.AddLight(Projectile.Center, Color.Lerp(Color.Blue, Color.AliceBlue, 0.5f).ToVector3() * 0.49f);

            // 子弹在出现之后很短一段时间会变得可见
            if (Projectile.timeLeft == 296)
                Projectile.alpha = 0;

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 添加飞行粒子特效
                if (Main.rand.NextBool(1)) // 1/X 概率生成粒子
                {
                    Dust dust = Dust.NewDustPerfect(
                        Projectile.Center,
                        Main.rand.NextBool() ? DustID.WaterCandle : DustID.DynastyShingle_Blue,
                        -Projectile.velocity.RotatedByRandom(0.1f) * Main.rand.NextFloat(0.1f, 0.3f)
                    );
                    dust.noGravity = true; // 无重力
                    dust.scale = Main.rand.NextFloat(0.7f, 1.1f); // 随机大小
                }
            }
            #region 旧代码
            //// 检查是否启用了特效
            //if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects && Projectile.localAI[0] % 4 == 0) // 每 4 帧调用一次
            //{
            //    // 添加飞行粒子特效（尖刺型），转动角度为原来的 4 倍
            //    float oscillationAngle = MathHelper.ToRadians(15) * (float)Math.Sin((Projectile.localAI[0] / 4) * MathHelper.Pi / 180 * 4); // 左右摆动角度（加速摆动）
            //    Vector2 particleDirection = Projectile.velocity.RotatedBy(MathHelper.Pi + oscillationAngle).SafeNormalize(Vector2.Zero);

            //    PointParticle spark = new PointParticle(
            //        Projectile.Center,
            //        particleDirection * 5f, // 固定方向与速度
            //        false,
            //        20,
            //        1.3f,
            //        Color.DeepSkyBlue // 深海深渊的海蓝色
            //    );
            //    GeneralParticleHandler.SpawnParticle(spark);
            //}
            #endregion

            // 更新局部 AI 计数器
            Projectile.localAI[0]++;
        }

        public override void OnSpawn(IEntitySource source)
        {

        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // 根据敌人的防御力计算加成，每点防御增加 0.75%
            float defenseBonus = target.defense * 0.0075f;

            // 根据敌人的伤害减免（DR）计算加成，每点 DR 增加 0.25%
            float drBonus = target.Calamity().DR * 0.0025f;

            // 计算总加成，且加成不能超过 125%
            float totalBonus = Math.Min(defenseBonus + drBonus, 1.25f); // 最大加成为 125%（即 2.25 倍）

            // 应用最终伤害加成
            modifiers.SourceDamage *= 1 + totalBonus;
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 300); // 深渊水压
        }


        public override void OnKill(int timeLeft)
        {
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                int numParticles = 20; // 粒子数量
                float rectWidth = 3f; // 矩形宽度
                float rectHeight = 1f; // 矩形高度

                for (int i = 0; i < numParticles; i++)
                {
                    // 在矩形范围内生成随机位置
                    Vector2 offset = new Vector2(
                        Main.rand.NextFloat(-rectWidth / 2f, rectWidth / 2f),
                        Main.rand.NextFloat(-rectHeight / 2f, rectHeight / 2f)
                    );

                    // 生成速度，偏向正前方
                    Vector2 velocity = (Vector2.UnitX + new Vector2(
                        Main.rand.NextFloat(-0.2f, 0.2f),
                        Main.rand.NextFloat(-0.2f, 0.2f)
                    )) * Main.rand.NextFloat(1f, 3f);

                    Dust dust = Dust.NewDustPerfect(
                        Projectile.Center + offset, // 粒子生成位置
                        DustID.BlueTorch, // 原版粒子类型
                        velocity, // 速度
                        0,
                        Color.Navy, // 粒子颜色
                        Main.rand.NextFloat(1.2f, 1.8f) // 粒子大小
                    );
                    dust.noGravity = true; // 粒子无重力
                }


                // 绘制一条随机方向的虚拟线
                float randomRotation = Main.rand.NextFloat(0f, MathHelper.TwoPi);
                Vector2 lineDirection = new Vector2((float)Math.Cos(randomRotation), (float)Math.Sin(randomRotation));
                Vector2 lineStart = Projectile.Center - lineDirection * 2.5f * 16f; // 起点
                Vector2 lineEnd = Projectile.Center + lineDirection * 2.5f * 16f;  // 终点

                // 计算命中点 -> 特效方向（标准化）
                Vector2 attackDirection = (Projectile.Center - lineStart).SafeNormalize(Vector2.Zero);

                // 计算特效排列的平行方向（即和特效排列线一致）
                Vector2 parallelDirection = lineDirection; // 保持平行

                // 在线正上方和正下方生成 5 个尖刺特效
                for (int offset = -1; offset <= 1; offset += 2) // 上下两个方向
                {
                    // **设置两排特效的相反方向，并分别旋转 90°**
                    Vector2 effectDirection = (offset == 1) ?
                        attackDirection.RotatedBy(-MathHelper.PiOver2) :
                        attackDirection.RotatedBy(MathHelper.PiOver2);

                    for (int i = 0; i < 5; i++) // 每个方向生成 5 个尖刺
                    {
                        float progress = i / 4f; // 计算粒子位置比例
                        Vector2 particlePosition = Vector2.Lerp(lineStart, lineEnd, progress) + parallelDirection.RotatedBy(MathHelper.PiOver2 * offset) * 2 * 16f;

                        // **特效速度（相反方向 + 额外 90° 旋转）**
                        Vector2 particleVelocity = effectDirection * Main.rand.NextFloat(3f, 6f);

                        PointParticle spark = new PointParticle(
                            particlePosition,
                            particleVelocity, // 速度统一，但每排方向相反 + 90° 旋转
                            false,
                            30,
                            1.5f,
                            Color.DeepSkyBlue // 深海深渊的海蓝色
                        );
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                }



            }
        }

    }
}
