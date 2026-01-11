namespace CalamityRangerExpansion.Content.Ammunition.CPreMoodLord.ScoriaBullet
{
    public class ScoriaBulletPROJ : ModProjectile, ILocalizedModType
    {
        private static int killCounter = 0; // 计数器

        public new string LocalizationCategory => "Projectile.CPreMoodLord";

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
            Lighting.AddLight(Projectile.Center, Color.Lerp(Color.Orange, Color.Red, 0.5f).ToVector3() * 0.55f);

            // 子弹在出现之后很短一段时间会变得可见
            if (Projectile.timeLeft == 296)
                Projectile.alpha = 0;

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 添加飞行期间的橙色粒子特效
                if (Main.rand.NextBool(3)) // 随机1/3概率生成
                {
                    Dust dust = Dust.NewDustPerfect(
                        Projectile.Center,
                        Main.rand.NextBool() ? 130 : 60, // 随机选择两种粒子类型
                        -Projectile.velocity.RotatedByRandom(0.1f) * Main.rand.NextFloat(0.01f, 0.3f) // 轻微随机速度
                    );
                    dust.noGravity = true; // 粒子无重力
                    dust.scale = Main.rand.NextFloat(0.5f, 0.9f); // 随机大小
                    if (dust.type == 130)
                        dust.scale = Main.rand.NextFloat(0.35f, 0.55f); // 特殊类型的粒子缩小
                }
            }
          

        }

        public override void OnSpawn(IEntitySource source)
        {
            #region 旧代码
            //// 在弹幕刚出现时释放三个橙色粒子特效
            //for (int i = -1; i <= 1; i++) // 循环 -1, 0, 1
            //{
            //    float angle = MathHelper.ToRadians(45 * i); // -45, 0, 45度
            //    Vector2 direction = Projectile.velocity.RotatedBy(angle).SafeNormalize(Vector2.UnitY);
            //    PointParticle spark = new PointParticle(
            //        Projectile.Center - direction * 10f, // 起始位置
            //        direction * 0.5f,                    // 粒子速度
            //        false,
            //        15,                                  // 生命周期
            //        1.1f,                                // 缩放比例
            //        Color.Orange                         // 颜色
            //    );
            //    GeneralParticleHandler.SpawnParticle(spark);
            //}
            #endregion

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            Player player = Main.player[Projectile.owner];
            if (player.TryGetModPlayer<ScoriaBulletPlayer>(out var modPlayer))
            {
                modPlayer.IncreaseStackCount(); // 增加层数
            }

            // 给予 ScoriaBulletPBuff，每次命中都会刷新 5 秒
            player.AddBuff(ModContent.BuffType<ScoriaBulletPBuff>(), 300);


            target.AddBuff(BuffID.OnFire3, 240);
            target.AddBuff(BuffID.Daybreak, 240);

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                //LightingBoltsSystem.Spawn_ExpandingOrangeRing(Projectile.Center);

                // 抛射橙色粒子特效
                int particleCount = Main.rand.Next(10, 16); // 随机生成 10 到 15 个粒子
                for (int i = 0; i < particleCount; i++)
                {
                    Color particleColor = Main.rand.NextBool() ? Color.OrangeRed : Color.Orange; // 随机颜色
                    Vector2 velocity = new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-5f, -1f)); // 向上抛射
                    PointParticle spark = new PointParticle(Projectile.Center, velocity, false, 10, 0.8f, particleColor);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }


            // 在原地释放爆炸弹幕
            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                Projectile.Center,
                Vector2.Zero,
                ModContent.ProjectileType<FuckYou>(), // 爆炸弹幕
                (int)(Projectile.damage * 0.5f),      // 伤害倍率
                Projectile.knockBack,
                Projectile.owner
            );
        }




        public override void OnKill(int timeLeft)
        {

        }
    }
}
