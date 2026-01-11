namespace CalamityRangerExpansion.Content.Ammunition.EAfterDog.AuricBulet
{
    public class AuricBuletPROJ : ModProjectile, ILocalizedModType
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public new string LocalizationCategory => "Projectile.EAfterDog";
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
            Projectile.timeLeft = 600;
            Projectile.MaxUpdates = 7;
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
            if (Projectile.timeLeft == 595)
                Projectile.alpha = 0;

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 只有在 timeLeft <= 590 时生成金色粒子
                if (Projectile.timeLeft <= 595)
                {
                    float positionVariation = 5f; // 位置偏移范围，可根据需求调整
                    LineParticle spark = new LineParticle(
                        Projectile.Center + Main.rand.NextVector2Circular(positionVariation, positionVariation),
                        -Projectile.velocity * Main.rand.NextFloat(0.3f, 1.1f),
                        false,
                        4,
                        1.45f,
                        Main.rand.NextBool() ? (Projectile.timeLeft < 570 ? Color.Goldenrod : Color.OrangeRed) : (Projectile.timeLeft > 590 ? Color.Red : Color.DarkGoldenrod)
                    );
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }



        }

        public override void OnSpawn(IEntitySource source)
        {

        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 保留电击效果
            target.AddBuff(BuffID.Electrified, 300);

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 生成两个金色和蓝色的火花特效
                for (int i = 0; i < 2; i++) // 翻倍生成数量
                {
                    GenericSparkle sparker = new GenericSparkle(
                        Projectile.Center,
                        Vector2.Zero,
                        Color.Gold,
                        Color.Cyan,
                        Main.rand.NextFloat(1.8f, 2.5f), // 大小保持不变
                        5,
                        Main.rand.NextFloat(-0.01f, 0.01f), // 旋转速度保持不变
                        1.68f
                    );
                    GeneralParticleHandler.SpawnParticle(sparker);
                }

                // 生成火花特效，数量和速度翻倍
                for (int i = 0; i < 2; i++) // 翻倍数量
                {
                    Vector2 bloodSpawnPosition = target.Center + Main.rand.NextVector2Circular(target.width, target.height) * 0.04f;
                    Vector2 splatterDirection = (Projectile.Center - bloodSpawnPosition).SafeNormalize(Vector2.UnitY);
                    Vector2 sparkVelocity = splatterDirection.RotatedByRandom(0.6f) * Main.rand.NextFloat(10f, 30f); // 随机速度值
                    sparkVelocity.Y -= 12f; // 翻倍Y方向的速度影响

                    SparkParticle spark = new SparkParticle(target.Center, sparkVelocity, false, Main.rand.Next(9, 12), Main.rand.NextFloat(0.9f, 1.3f) * 0.85f, Color.Lerp(Color.DarkGoldenrod, Color.Gold, Main.rand.NextFloat(0.7f)));
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }

        }

        public override void OnKill(int timeLeft)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                for (int i = 0; i <= 6; i++)
                {
                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center, 226, new Vector2(2, 2).RotatedByRandom(100f) * Main.rand.NextFloat(0.1f, 2.9f));
                    dust2.noGravity = false;
                    dust2.scale = Main.rand.NextFloat(0.3f, 0.9f);
                }
            }
            SoundStyle sound = new SoundStyle("CalamityMod/Sounds/Item/AuricBulletHit")
            {
                Volume = 0.4f // 将音量设置为 x%
            };
            SoundEngine.PlaySound(sound, Projectile.position);


            int existingBalls = Main.projectile.Count(p => p.active && p.type == ModContent.ProjectileType<AuricBuletBALL>());
            if (existingBalls >= 150)
                return; // 如果已经存在150个，不再生成


            int spawnCount = Main.getGoodWorld ? Main.rand.Next(3, 9) : Main.rand.Next(2, 4); // 生成2~3个，如果是getGoodWorld 那么生成3~8个
            for (int i = 0; i < spawnCount; i++)
            {
                // 生成一个随机的半径（3到6个方块）
                float radius = Main.rand.Next(3, 7) * 16f;

                // 随机生成角度用于偏移位置
                float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                Vector2 spawnPosition = Projectile.Center + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius;

                // 生成AuricBuletBALL
                int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPosition, Vector2.Zero, ModContent.ProjectileType<AuricBuletBALL>(), (int)(Projectile.damage * 0.3f), Projectile.knockBack, Main.myPlayer);
                // 生成AuricBuletBALL并通过ai参数传递公转半径
                //int proj = Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPosition, Vector2.Zero, ModContent.ProjectileType<AuricBuletBALL>(), (int)(Projectile.damage * 1.25f), Projectile.knockBack, Main.myPlayer, radius);

                // 访问生成的投射物，并设置其公转半径
                if (Main.projectile[proj].ModProjectile is AuricBuletBALL auricBall)
                {
                    //auricBall.GetType().GetField("X").SetValue(auricBall, radius); // 设置AuricBuletBALL的公转半径X
                }
            }
        }
    }
}
