namespace CalamityRangerExpansion.Content.DeveloperItems.Bullet.GrapeShot
{
    public class GrapeShotPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.GrapeShot";
        private int projectileCount; // 用于存储传递的x值

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
            Projectile.timeLeft = 300; // 弹幕存在时间为600帧
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

            if (Projectile.localAI[0] == 0) // 刚出现时调用逻辑
            {
                GrapeShotPlayer player = Main.player[Projectile.owner].GetModPlayer<GrapeShotPlayer>();
                player.IncrementGrapeShotCounter(); // 计数器加 1
                projectileCount = player.GetGrapeShotX(); // 获取当前 x 值
                Projectile.localAI[0] = 1; // 防止重复调用
            }


            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 路径上的紫色 Dust 特效
                if (Main.rand.NextBool(3))
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.position, DustID.PurpleTorch);
                    dust.noGravity = true;
                    dust.scale = 1.2f;
                    dust.velocity *= 0.3f;
                }
            }
  
        }

        public override void OnKill(int timeLeft)
        {
            // 限制分裂弹幕的数量
            int maxProjectiles = 10;
            int limitedProjectileCount = Math.Min(projectileCount, maxProjectiles); // 如果 projectileCount 超过 10，则强制设置为 10

            // 随机释放 limitedProjectileCount 个 GrapeShotPROJSPIT
            for (int i = 0; i < limitedProjectileCount; i++)
            {
                float angle = MathHelper.ToRadians(Main.rand.Next(360));
                Vector2 velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 10f;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<GrapeShotPROJSPIT>(), (int)(Projectile.damage * 0.15f), Projectile.knockBack, Projectile.owner);
            }

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 在消失时释放对应数量的粒子特效
                for (int i = 0; i < projectileCount; i++)
                {
                    float angle = MathHelper.ToRadians(Main.rand.Next(360));
                    Vector2 trailPos = Projectile.Center;
                    Vector2 trailVelocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 10f * 0.2f;
                    Particle trail = new SparkParticle(trailPos, trailVelocity, false, 60, 1.0f, Color.Purple);
                    GeneralParticleHandler.SpawnParticle(trail);
                }
            }
        }

        // 传递 projectileCount 的值
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
            projectileCount = (int)(Projectile.ai[0]); // 假设 x 作为 ai[0] 传递
        }

    }
}