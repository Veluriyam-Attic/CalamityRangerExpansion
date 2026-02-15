namespace CalamityRangerExpansion.Content.Ammunition.EAfterDog.MiracleMatterBullet
{
    public class MiracleMatterBulletPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectile.EAfterDog";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public static int MaxUpdate = 5; // 定义一个静态变量，表示弹幕每次更新的最大次数

        private int Lifetime = 1600; // 定义弹幕的生命周期为x

        // 更改颜色：深绿色、黑色、另一种深绿色
        private static Color ShaderColorOne = Color.Cyan; // 着色器颜色1，设置为深绿色
        private static Color ShaderColorTwo = Color.Lime; // 着色器颜色2，设置为黑色
        private static Color ShaderEndColor = Color.White; // 着色器结束颜色，设置为森林绿色（另一种深绿色）

        private Vector2 altSpawn; // 定义一个备用生成位置向量

        public override void SetStaticDefaults() // 设置弹幕的静态默认值
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // 设置拖尾模式为2
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 40; // 设置拖尾缓存长度为x
        }

        public override void SetDefaults() // 设置弹幕的默认值
        {
            Projectile.width = Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = Main.getGoodWorld ? 8000 : Lifetime;
            Projectile.MaxUpdates = MaxUpdate;
            Projectile.penetrate = 201;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = Main.getGoodWorld ? 1 : -1;
            Projectile.arrow = Main.getGoodWorld;
        }

        private bool collidedWithNPC = false; // 是否与敌人发生过碰撞
        private int collisionTimer = 0; // 碰撞后的计时器
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.velocity *= 1.5f;
        }


        public override void AI()
        {
            Projectile.ai[0]++; // 弹幕AI计数器递增

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                if (Projectile.timeLeft <= 5)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(9, 9) - Projectile.velocity * 5, DustID.GemDiamond, Projectile.velocity * 30 * Main.rand.NextFloat(0.1f, 0.95f));
                    dust.noGravity = true;
                    dust.scale = Main.rand.NextFloat(0.9f, 1.45f);
                    dust.alpha = 235;
                    dust.color = Color.White;
                }
            }
            Player player = Main.player[Projectile.owner];
            Projectile.localAI[0] += 1f / (Projectile.extraUpdates + 1);

            // 默认直线飞行
            if (!collidedWithNPC)
            {
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 7.5f;
            }

            // 检测与敌人的碰撞
            if (!collidedWithNPC)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.active && !npc.friendly && npc.CanBeChasedBy() &&
                        Projectile.Hitbox.Intersects(npc.Hitbox))
                    {
                        collidedWithNPC = true; // 标记碰撞
                        collisionTimer = (int)Main.GameUpdateCount; // 记录当前帧数
                        // 检查是否启用了特效
                        if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
                        {
                            ReleaseSquareParticles(); // 释放粒子效果
                        }
                        break;
                    }
                }
            }

            // 左右偏转逻辑（碰撞后时间参数动态调整）
            int maxOffsetTime = Main.getGoodWorld ? 20 : 60; // 动态调整时间参数

            if (collidedWithNPC && (int)Main.GameUpdateCount - collisionTimer <= maxOffsetTime)
            {
                bool turnLeft = Main.rand.NextBool();
                float turnIncrement = MathHelper.ToRadians(1.5f); // 每帧转向增量
                float currentTurn = turnLeft ? -turnIncrement : turnIncrement; // 随机方向
                Projectile.velocity = Projectile.velocity.RotatedBy(currentTurn);
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 7.5f;
            }

            // 追踪模式逻辑（碰撞后超过动态调整时间参数）
            if (collidedWithNPC && (int)Main.GameUpdateCount - collisionTimer > maxOffsetTime)
            {
                NPC target = Projectile.Center.ClosestNPCAt(8848); // 查找范围内最近的敌人
                if (target != null)
                {
                    Vector2 direction = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * 14f, 0.11f); // 平滑追踪
                }
            }

            if(Main.getGoodWorld)
            {
                if (Projectile.ai[1] > 60)
                {
                    NPC target = Projectile.Center.ClosestNPCAt(88888); // 查找范围内最近的敌人
                    if (target != null)
                    {
                        Vector2 direction = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * 20f, 0.1f);
                    }
                }
                else
                {
                    Projectile.ai[1]++;
                }
            }

            // 碰撞后伤害逻辑
            if (Projectile.penetrate < 200)
            {
                if (Projectile.timeLeft > (Main.getGoodWorld ? 180 : 60))
                {
                    Projectile.timeLeft = Main.getGoodWorld ? 180 : 60;
                }
                Projectile.velocity *= 0.88f;
            }

        }

        // 释放正方形粒子特效
        private void ReleaseSquareParticles()
        {
            // 随机生成正方形核心点
            Vector2 randomCore = Projectile.Center + new Vector2(
                Main.rand.Next(-100, 101), // 左右随机偏移范围
                Main.rand.Next(-100, 101) // 上下随机偏移范围
            );

            for (int i = 0; i < 4; i++)
            {
                // 计算正方形的四个顶点角度，每个顶点相隔90度
                float angle = MathHelper.PiOver4 + i * MathHelper.PiOver2;
                float nextAngle = MathHelper.PiOver4 + (i + 1) * MathHelper.PiOver2;

                // 调整正方形的边长为核心点周围的正方形
                Vector2 start = angle.ToRotationVector2() * (16f / 2f); // 缩小边长一半
                Vector2 end = nextAngle.ToRotationVector2() * (16f / 2f);

                for (int j = 0; j < 40; j++)
                {
                    // 在两个顶点之间插值生成粒子
                    Dust squareDust = Dust.NewDustPerfect(randomCore, 267);
                    squareDust.scale = 2.5f;
                    squareDust.velocity = Vector2.Lerp(start, end, j / 40f) * 0.8f; // 初始速度降低60%
                    squareDust.color = CalamityUtils.MulticolorLerp(j / 40f, CalamityUtils.ExoPalette); // 使用前后半圆颜色逻辑
                    squareDust.noGravity = true;
                }
            }
        }


        public override bool? CanDamage()
        {
            // 如果未进入追踪模式或未经过延迟，则不允许造成伤害
            return collidedWithNPC && (int)Main.GameUpdateCount - collisionTimer >= 10 ? (bool?)true : (bool?)false;
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 300);
            //Projectile.ai[1] = 1; // 标记追踪逻辑开启
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.ai[1] = 1; // 标记追踪逻辑开启
            return base.OnTileCollide(oldVelocity);
        }


        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.FinalDamage *= 1.25f;
        }


        public override void OnKill(int timeLeft)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 在原地生成一个随机方向的旋转粒子特效
                for (int i = 0; i < 2; i++) // 生成 8 个粒子
                {
                    // 随机偏移量
                    Vector2 sparkOffset = Main.rand.NextVector2Circular(10f, 10f); // 随机半径为10像素的圆形偏移

                    // 随机初始颜色和结束颜色
                    Color startColor = Color.LightBlue * 0.5f; // 初始颜色（亮度减半）
                    Color endColor = Color.White * 0.5f;      // 结束颜色（亮度减半）

                    // 创建旋转粒子特效
                    GenericSparkle sparker = new GenericSparkle(
                        Projectile.Center + sparkOffset,       // 粒子起始位置
                        Vector2.Zero,                          // 粒子初速度
                        startColor,                            // 起始颜色
                        endColor,                              // 结束颜色
                        Main.rand.NextFloat(2.5f, 2.9f),       // 粒子大小
                        14,                                    // 生命周期
                        Main.rand.NextFloat(-0.01f, 0.01f),    // 随机旋转速度
                        2.5f                                   // 粒子亮度
                    );

                    GeneralParticleHandler.SpawnParticle(sparker); // 生成粒子
                }
            }
        }


        private float PrimitiveWidthFunction(float completionRatio, Vector2 vertexPosition)
        {
            float arrowheadCutoff = 0.36f;
            float width = 24f;
            float minHeadWidth = 0.03f;
            float maxHeadWidth = width;
            if (completionRatio <= arrowheadCutoff)
                width = MathHelper.Lerp(minHeadWidth, maxHeadWidth, Utils.GetLerpValue(from: 0f, arrowheadCutoff, completionRatio, true));
            return width;
        }

        private Color PrimitiveColorFunction(float completionRatio, Vector2 vertexPosition)
        {
            float endFadeRatio = 0.41f;
            float completionRatioFactor = 2.7f;
            float globalTimeFactor = 5.3f;
            float endFadeFactor = 3.2f;
            float endFadeTerm = Utils.GetLerpValue(0f, endFadeRatio * 0.5f, completionRatio, true) * endFadeFactor;
            float cosArgument = completionRatio * completionRatioFactor - Main.GlobalTimeWrappedHourly * globalTimeFactor + endFadeTerm;
            float startingInterpolant = (float)Math.Cos(cosArgument) * 0.5f + 0.5f;

            float colorLerpFactor = 0.6f;
            Color startingColor = Color.Lerp(ShaderColorOne, ShaderColorTwo, startingInterpolant * colorLerpFactor);
            return Color.Lerp(startingColor, ShaderEndColor, MathHelper.SmoothStep(0f, 1f, Utils.GetLerpValue(0f, endFadeRatio, completionRatio, true)));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/SylvestaffStreak"));
                Vector2 overallOffset = Projectile.Size * 0.5f;
                overallOffset += Projectile.velocity * 1.4f;
                int numPoints = 46;
                PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(PrimitiveWidthFunction, PrimitiveColorFunction, (_, _) => overallOffset, shader: GameShaders.Misc["CalamityMod:TrailStreak"]), numPoints);
                return false;
            }
            return false;
        }
    }
}
