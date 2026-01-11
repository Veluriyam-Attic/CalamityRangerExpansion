namespace CalamityRangerExpansion.Content.Arrows.APreHardMode.AerialiteArrow
{
    public class AerialiteArrowWIND : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectile.APreHardMode";
        public override void SetStaticDefaults()
        {
            // 设置拖尾效果和长度
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            // 基础属性设置
            Projectile.width = 56;
            Projectile.height = 56;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 300; // 弹幕存活时间
            Projectile.extraUpdates = 2;
            Projectile.penetrate = 2; // 可以击中两个敌人
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
            Projectile.tileCollide = false; // 不与地形碰撞
        }

        public override void AI()
        {
            // 控制弹幕旋转和透明度
            Projectile.rotation += 2.5f; // 持续旋转
            Projectile.alpha -= 5; // 渐渐显示弹幕
            if (Projectile.alpha < 50)
            {
                Projectile.alpha = 50;

                // 寻找并追踪最近的敌人
                NPC target = FindClosestNPC(1800f); // 设定x像素的最大追踪范围
                if (target != null && target.active)
                {
                    // 如果速度小于最大速度，逐渐加速
                    float maxSpeed = 10f; // 最大速度设为10
                    float acceleration = 0.2f; // 加速度为0.2
                    if (Projectile.velocity.Length() < maxSpeed)
                    {
                        Projectile.velocity += Projectile.DirectionTo(target.Center) * acceleration;
                        if (Projectile.velocity.Length() > maxSpeed) // 确保速度不会超过最大值
                        {
                            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * maxSpeed;
                        }
                    }
                }

                // 生成灰白色的尘埃特效
                if (Projectile.ai[1] >= 15)
                {
                    // 检查是否启用了特效
                    if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
                    {
                        for (int i = 1; i <= 6; i++)
                        {
                            Vector2 dustspeed = new Vector2(3f, 3f).RotatedBy(MathHelper.ToRadians(60 * i));
                            int d = Dust.NewDust(Projectile.Center, Projectile.width / 2, Projectile.height / 2, 31, dustspeed.X, dustspeed.Y, 200, Color.LightGray, 1.3f);
                            Main.dust[d].noGravity = true;
                            Main.dust[d].velocity = dustspeed;
                        }
                    }

                    Projectile.ai[1] = 0;
                }
            }

            // 增加AI计数器
            Projectile.ai[1]++;
        }

        // 寻找最近的敌人
        private NPC FindClosestNPC(float maxRange)
        {
            NPC closestNPC = null;
            float closestDistance = maxRange;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy(Projectile))
                {
                    float distance = Vector2.Distance(Projectile.Center, npc.Center);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestNPC = npc;
                    }
                }
            }

            return closestNPC;
        }


        // 修改为灰白色的残影效果
        public override bool PreDraw(ref Color lightColor)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], Color.LightGray, 2);
                return false;
            }
            return true;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item60 with { Volume = SoundID.Item60.Volume * 0.6f }, Projectile.Center);

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 在弹幕死亡时生成大量尘埃
                for (int i = 0; i <= 360; i += 3)
                {
                    Vector2 dustspeed = new Vector2(3f, 3f).RotatedBy(MathHelper.ToRadians(i));
                    int d = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, 31, dustspeed.X, dustspeed.Y, 200, Color.AliceBlue, 1.4f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].position = Projectile.Center;
                    Main.dust[d].velocity = dustspeed;
                }
            }
        }
    }
}