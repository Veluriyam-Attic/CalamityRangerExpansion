namespace CalamityRangerExpansion.Content.Arrows.CPreMoodLord.PlagueArrow
{
    internal class PlagueArrowPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectile.CPreMoodLord";
        public override string Texture => "CalamityRangerExpansion/Content/Arrows/CPreMoodLord/PlagueArrow/PlagueArrow";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            // 设置弹幕的基础属性
            Projectile.width = 11; // 弹幕宽度
            Projectile.height = 24; // 弹幕高度
            Projectile.friendly = true; // 对敌人有效
            Projectile.DamageType = DamageClass.Ranged; // 远程伤害类型
            Projectile.penetrate = 1; // 穿透力为1，击中一个敌人就消失
            Projectile.timeLeft = 300; // 弹幕存在时间为x帧
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
            Projectile.ignoreWater = true; // 弹幕不受水影响
            Projectile.arrow = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            // 调整弹幕的旋转，使其在飞行时保持水平
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;

            // Lighting - 添加天蓝色光源，光照强度为 0.49
            Lighting.AddLight(Projectile.Center, Color.LightSkyBlue.ToVector3() * 0.49f);



            // 检查场上是否存在 WitherBlossom 弹幕
            bool hasWitherBlossom = Main.projectile.Any(p => p.active && p.type == ModContent.ProjectileType<WitherBlossom>());

            if (hasWitherBlossom)
            {
                // 前30帧不追踪，之后开始追踪敌人
                if (Projectile.ai[1] > 30)
                {
                    NPC target = Projectile.Center.ClosestNPCAt(2800f); // 查找1800范围内最近的敌人
                    if (target != null)
                    {
                        Vector2 direction = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * 18f, 0.08f); // 追踪速度
                    }
                }
                else
                {
                    Projectile.ai[1]++;
                }
            }
            else
            {
                // 没有 WitherBlossom 弹幕时直接直线飞行
                Projectile.ai[1]++;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }

        // 更改颜色：深绿色、黑色、另一种深绿色
        private static Color ShaderColorOne = Color.DarkGreen; // 着色器颜色1，设置为深绿色
        private static Color ShaderColorTwo = Color.Black; // 着色器颜色2，设置为黑色
        private static Color ShaderEndColor = Color.ForestGreen; // 着色器结束颜色，设置为森林绿色（另一种深绿色）
        private float PrimitiveWidthFunction(float completionRatio, Vector2 vexpos)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                float arrowheadCutoff = 0.36f;
                float width = 24f;
                float minHeadWidth = 0.03f;
                float maxHeadWidth = width;
                if (completionRatio <= arrowheadCutoff)
                    width = MathHelper.Lerp(minHeadWidth, maxHeadWidth, Utils.GetLerpValue(0f, arrowheadCutoff, completionRatio, true));
                return width;
            }

            // 如果没有启用特效，返回默认宽度
            return 24f;
        }

        private Color PrimitiveColorFunction(float completionRatio, Vector2 vexpos)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
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

            // 如果没有启用特效，返回默认颜色
            return Color.White;
        }


        public override bool PreDraw(ref Color lightColor)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);

                // 渲染带有粉红色渐变效果的光学尾迹
                GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/SylvestaffStreak"));
                Vector2 overallOffset = Projectile.Size * 0.5f;
                overallOffset += Projectile.velocity * 1.4f;
                int numPoints = 46;
                PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(PrimitiveWidthFunction, PrimitiveColorFunction, (_, _) => overallOffset, shader: GameShaders.Misc["CalamityMod:TrailStreak"]), numPoints);
                return false;
            }
            return true;

        }

        public override void OnKill(int timeLeft)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 检查场上是否存在 WitherBlossom 弹幕
                bool hasWitherBlossom = Main.projectile.Any(p => p.active && p.type == ModContent.ProjectileType<WitherBlossom>());

                int numParticles = hasWitherBlossom ? 40 : 14; // 如果存在 WitherBlossom，则生成更多粒子

                // 改为 PlagueTaintedDrone 的粒子特效，但保留原逻辑
                for (int i = 0; i < numParticles; i++)
                {
                    float speedFactor = hasWitherBlossom ? 5f + i * 0.4f : 3f + i * 0.2f; // 控制速度
                    int dustType = hasWitherBlossom ? DustID.GreenTorch : DustID.WhiteTorch;

                    // 向上发射
                    Vector2 velocityUp = new Vector2(0, -speedFactor);
                    Dust dustUp = Dust.NewDustPerfect(Projectile.Center, dustType, velocityUp, 0, Color.DarkGreen, 1.5f);
                    dustUp.noGravity = true;

                    // 向下发射
                    Vector2 velocityDown = new Vector2(0, speedFactor);
                    Dust dustDown = Dust.NewDustPerfect(Projectile.Center, dustType, velocityDown, 0, Color.DarkGreen, 1.5f);
                    dustDown.noGravity = true;
                }

                for (int i = 0; i < numParticles; i++)
                {
                    float speedFactor = hasWitherBlossom ? 2f + i * 0.3f : 1f + i * 0.2f; // 控制速度
                    int dustType = hasWitherBlossom ? DustID.GreenTorch : DustID.WhiteTorch;

                    // 向左发射
                    Vector2 velocityLeft = new Vector2(-speedFactor, 0);
                    Dust dustLeft = Dust.NewDustPerfect(Projectile.Center, dustType, velocityLeft, 0, Color.DarkGreen, 1.5f);
                    dustLeft.noGravity = true;

                    // 向右发射
                    Vector2 velocityRight = new Vector2(speedFactor, 0);
                    Dust dustRight = Dust.NewDustPerfect(Projectile.Center, dustType, velocityRight, 0, Color.DarkGreen, 1.5f);
                    dustRight.noGravity = true;
                }


                Color explosionColor = Color.DarkGreen;
                Particle blastRing = new CustomPulse(
                    Projectile.Center, Vector2.Zero, explosionColor,
                    "CalamityRangerExpansion/texture/BiologicalHazards",
                    Vector2.One * 0.33f, Main.rand.NextFloat(-10f, 10f),
                    0.075f, 0.40f, 30
                );
                GeneralParticleHandler.SpawnParticle(blastRing);                
            }
        }
    }
}