namespace CalamityRangerExpansion.Content.DeveloperItems.Arrow.ExplodingRabbit
{
    public class ExplodingRabbitPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectile.DPreDog";
        public override string Texture => "CalamityRangerExpansion/Content/DeveloperItems/Arrow/ExplodingRabbit/ExplodingRabbit";
        private int bounceCount = 0;

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
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 900;
            Projectile.MaxUpdates = 3;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            // 默认状态
            Projectile.localNPCHitCooldown = 15;
            Projectile.penetrate = 2;

            #region 旧代码

            //// 根据模式动态调整
            //if (Main.getGoodWorld)
            //{
            //    Projectile.localNPCHitCooldown = 1;
            //    Projectile.penetrate = -1; // 无限穿透
            //}

            #endregion

            Projectile.aiStyle = ProjAIStyleID.Arrow; // 让弹幕受到重力影响
            Projectile.arrow = true;
        }
        private int goodWorldTimer = 0; // 新增计时器变量

        public override void AI()
        {
            // 由于我们是水平贴图，因此什么也不需要转动
            Projectile.rotation = Projectile.velocity.ToRotation();

            // 添加光效
            Lighting.AddLight(Projectile.Center, Color.Lerp(Color.White, Color.WhiteSmoke, 0.5f).ToVector3() * 0.49f);

            // 子弹在出现之后很短一段时间会变得可见
            if (Projectile.timeLeft == 898)
                Projectile.alpha = 0;

            #region 旧代码

            //// 如果 getGoodWorld 启用，每 5 帧生成一个爆炸弹幕
            //if (Main.getGoodWorld)
            //{
            //    goodWorldTimer++;
            //    if (goodWorldTimer >= 5)
            //    {
            //        goodWorldTimer = 0; // 重置计时器

            //        // 创建爆炸弹幕
            //        int projID = Projectile.NewProjectile(
            //            Projectile.GetSource_FromThis(),
            //            Projectile.Center,
            //            Vector2.Zero,
            //            ModContent.ProjectileType<FuckYou>(), // 替换为实际爆炸弹幕的类型
            //            (int)(Projectile.damage * 2.0f),
            //            Projectile.knockBack,
            //            Projectile.owner
            //        );
            //        Main.projectile[projID].scale = 2.0f; // 调整爆炸弹幕的大小
            //    }

            //    // 前15帧不追踪，之后开始追踪敌人
            //    if (Projectile.ai[1] > 15)
            //    {
            //        NPC target = Projectile.Center.ClosestNPCAt(8800); // 查找范围内最近的敌人
            //        if (target != null)
            //        {
            //            Vector2 direction = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
            //            Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * 18f, 0.08f); // 追踪速度为18f
            //        }
            //    }
            //    else
            //    {
            //        Projectile.ai[1]++;
            //    }
            //}

            #endregion
        }
        public override void OnKill(int timeLeft)
        {
            #region 旧代码
            //// 创建爆炸弹幕
            //int projID = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FuckYou>(), (int)(Projectile.damage * 2.0f), Projectile.knockBack, Projectile.owner);
            //// 修改爆炸弹幕的大小为原来的两倍
            //Main.projectile[projID].scale = 2.0f;
            //// Smoke effect
            //CreateSmokeEffect();

            //// Square particle effect
            //CreateSquareParticleEffect();

            #endregion
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 创建爆炸弹幕
            int projID = Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                Projectile.Center,
                Vector2.Zero,
                ModContent.ProjectileType<FuckYou>(),
                (int)(Projectile.damage * 3.5f),
                Projectile.knockBack,
                Projectile.owner
            );
            Main.projectile[projID].scale = 3.5f;


            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 调用烟雾和正方形粒子效果
                CreateSmokeEffect();
                CreateSquareParticleEffect();
            }



            // 增加反弹逻辑
            if (bounceCount >= 2)
            {
                Projectile.Kill(); // 超过反弹次数则销毁
                return;
            }

            bounceCount++;

            // 获取当前速度方向，并反射
            if (Projectile.velocity.X != 0)
                Projectile.velocity.X = -Projectile.velocity.X;

            if (Projectile.velocity.Y != 0)
                Projectile.velocity.Y = -Projectile.velocity.Y;

            // 播放反弹音效
            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (bounceCount >= 2)
                return true;

            bounceCount++;
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X; // Reflect X velocity

            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y; // Reflect Y velocity

            // Smoke effect
            CreateSmokeEffect();

            // Square particle effect
            CreateSquareParticleEffect();

            return false; // Prevent the projectile from being destroyed
        }

        private void CreateSmokeEffect()
        {
            for (int i = 0; i < 4; i++)
            {
                int type = Main.rand.Next(61, 64); // Random smoke gore
                Vector2 velocity = new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));
                Gore gore = Main.gore[Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, velocity, type, 1f)];
                gore.velocity *= 0.5f; // Adjust velocity multiplier
            }
        }

        private void CreateSquareParticleEffect()
        {
            int particleCount = 100; // 粒子总数量
            float squareSize = 100f; // 正方形的最大扩散范围（像素）

            for (int i = 0; i < particleCount; i++)
            {
                // 随机选择粒子类型 31 或 35
                int type = Main.rand.Next(new int[] { 31, 35 });

                // 生成粒子的初始速度，速度范围内生成随机值
                Vector2 velocity = new Vector2(
                    Main.rand.NextFloat(-squareSize, squareSize),
                    Main.rand.NextFloat(-squareSize, squareSize)
                ).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(2f, 5f);

                // 创建粒子
                Dust dust = Dust.NewDustDirect(Projectile.Center, 0, 0, type);
                dust.velocity = velocity;
                dust.scale = 1.5f; // 粒子的缩放比例
                dust.noGravity = true; // 让粒子不受重力影响
            }
        }

        public override void OnSpawn(IEntitySource source)
        {

        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {

        }
    }
}
