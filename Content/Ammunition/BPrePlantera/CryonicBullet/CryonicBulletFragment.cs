namespace CalamityRangerExpansion.Content.Ammunition.BPrePlantera.CryonicBullet
{
    internal class CryonicBulletFragment : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectile.BPrePlantera";
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
            Projectile.timeLeft = 360;
            Projectile.MaxUpdates = 3;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;
        }

        public override void AI()
        {
            // 使用主弹幕传递的减速度因子
            float decelerationFactor = Projectile.ai[0];

            // 每帧速度逐渐降低
            Projectile.velocity *= decelerationFactor;

            // 每帧旋转
            Projectile.rotation += 0.6f;

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 飞行期间释放粒子特效
                if (Main.rand.NextBool(2)) // 每帧 50% 概率生成粒子
                {
                    Dust dust = Dust.NewDustPerfect(
                        Projectile.Center,
                        185, // 粒子 ID
                        Main.rand.NextVector2Circular(1f, 1f), // 随机方向速度
                        150, // 透明度
                        Color.White, // 粒子颜色
                        Main.rand.NextFloat(0.8f, 1.2f) // 粒子大小
                    );
                    dust.noGravity = true; // 粒子不受重力影响
                }

            }
         
            Time++;
        }
        public ref float Time => ref Projectile.ai[1];

        public override bool? CanDamage() => Time >= 5f; // 初始的时候不会造成伤害，直到x为止

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // 反弹逻辑：遵循入射角等于出射角
            if (Projectile.velocity.X != oldVelocity.X) // X 方向反转
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y) // Y 方向反转
                Projectile.velocity.Y = -oldVelocity.Y;

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 在原地生成水晶粒子爆炸效果
                for (int i = 0; i < 20; i++)
                {
                    Dust dust = Dust.NewDustPerfect(
                        Projectile.Center,
                        67, // 粒子 ID
                        Main.rand.NextVector2Circular(3f, 3f), // 随机方向速度
                        100, // 透明度
                        Color.Cyan, // 粒子颜色
                        Main.rand.NextFloat(1.2f, 1.8f) // 粒子大小
                    );
                    dust.noGravity = true; // 粒子不受重力影响
                }
            }
        

            return false; // 阻止弹幕被销毁
        }

        public override void OnSpawn(IEntitySource source)
        {

        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }
        public override void OnKill(int timeLeft)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 随机生成 20 到 30 个冰晶粒子
                int particleCount = Main.rand.Next(20, 31);

                for (int i = 0; i < particleCount; i++)
                {
                    // 随机方向
                    Vector2 velocity = Main.rand.NextVector2Circular(3f, 3f); // 随机半径为3的圆形方向和速度

                    // 创建冰晶粒子特效
                    Dust dust = Dust.NewDustPerfect(
                        Projectile.Center,          // 粒子生成位置
                        DustID.SnowflakeIce,        // 粒子类型
                        velocity,                   // 粒子速度
                        100,                        // 粒子透明度
                        Color.White,                // 粒子颜色
                        Main.rand.NextFloat(1f, 1.5f) // 粒子大小
                    );
                    dust.noGravity = true; // 粒子不受重力影响
                    dust.fadeIn = 0.5f;    // 快速淡入效果
                }
            }         
        }

    }
}
