namespace CalamityRangerExpansion.Content.Arrows.APreHardMode.WulfrimArrow
{
    internal class WulfrimArrowPROJ : ModProjectile, ILocalizedModType
    {
        public override string Texture => "CalamityRangerExpansion/Content/Arrows/APreHardMode/WulfrimArrow/WulfrimArrow";
        public new string LocalizationCategory => "Projectile.CPreMoodLord";
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
            Projectile.timeLeft = 300; // 弹幕存在时间为x帧
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
            Projectile.ignoreWater = true; // 弹幕不受水影响
            Projectile.arrow = true;
            Projectile.extraUpdates = 1;
            Projectile.aiStyle = ProjAIStyleID.Arrow; // 受重力影响
        }

        public override void AI()
        {
            // 调整弹幕的旋转，使其在飞行时保持水平
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;

            // Lighting - 添加天蓝色光源，光照强度为 0.49
            Lighting.AddLight(Projectile.Center, Color.LightSkyBlue.ToVector3() * 0.49f);

            #region 旧代码

            //// 生成双螺旋粒子效果
            //float sine = (float)Math.Sin(Projectile.timeLeft * 0.1875f / MathHelper.Pi); // 绘制速度减半
            //Vector2 offset = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2) * sine * 32f; // 振动幅度扩大 100%

            //// Electric 粒子特效 (白蓝色)
            //Dust electricDust = Dust.NewDustPerfect(Projectile.Center + offset, 226, Vector2.Zero);
            //electricDust.color = Color.LightBlue;
            //electricDust.noGravity = true;

            //// WulfrumBolt 粒子特效 (保持原色)
            //Dust wulfrumDust = Dust.NewDustPerfect(Projectile.Center - offset, 267, Vector2.Zero);
            //wulfrumDust.noGravity = true;

            #endregion

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 释放亮绿色粒子特效
                if (Main.rand.NextBool(5))
                {
                    //  直接在弹幕中心生成粒子，没有左右偏移
                    Vector2 trailPos = Projectile.Center;

                    float trailScale = Main.rand.NextFloat(0.8f, 1.2f); // 维持粒子的缩放效果
                    Color trailColor = Color.LimeGreen; // 固定颜色为亮绿色

                    //  创建粒子
                    Particle trail = new SparkParticle(trailPos, Projectile.velocity * 0.2f, false, 60, trailScale, trailColor);
                    GeneralParticleHandler.SpawnParticle(trail);
                }
            }
            #region 旧代码

            //// 产生DNA形状的粒子特效
            //float frequency = 30f;  // 30帧一个回合
            //float amplitude = 20f;  // 振动幅度

            //// 左侧和右侧的偏移计算
            //Vector2 leftOffset = new Vector2(-amplitude * (float)Math.Sin(Projectile.ai[0] * MathHelper.TwoPi / frequency), 0);
            //Vector2 rightOffset = new Vector2(amplitude * (float)Math.Sin(Projectile.ai[0] * MathHelper.TwoPi / frequency), 0);

            //if (Projectile.ai[0] % 2 == 0)  // 每两帧产生一次粒子
            //{
            //    // Electric 粒子特效 (白蓝色)
            //    Dust.NewDustPerfect(Projectile.Center + leftOffset, 226, Vector2.Zero, 0, Color.LightBlue, 1.2f).noGravity = true;

            //    // WulfrumBolt 粒子特效 (保持原色)
            //    Dust.NewDustPerfect(Projectile.Center + rightOffset, 267, Vector2.Zero, 0, default(Color), 1.2f).noGravity = true;
            //}

            //// 更新ai，两倍的绘制速度
            //Projectile.ai[0] += 1f;
            #endregion
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<WulfrimArrowEDebuff>(), 75);
        }
        public override void OnKill(int timeLeft)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 生成大量向上抛射的电能粒子特效
                for (int i = 0; i < 10; i++) // 生成 10 个粒子
                {
                    Vector2 velocity = new Vector2(0, -1f).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(5f, 10f); // 随机角度和速度
                    Dust electricDust = Dust.NewDustPerfect(Projectile.Center, 226, velocity); // 226 为 Electric 粒子特效的编号
                    electricDust.color = Color.LightGreen; // 设定颜色为浅绿色
                    electricDust.noGravity = true; // 无重力效果，使其向上抛射
                    electricDust.scale = Main.rand.NextFloat(1.2f, 1.8f); // 随机缩放
                }
            }
        }







    }
}