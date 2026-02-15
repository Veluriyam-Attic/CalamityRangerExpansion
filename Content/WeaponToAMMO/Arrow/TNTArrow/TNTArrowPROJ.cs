namespace CalamityRangerExpansion.Content.WeaponToAMMO.Arrow.TNTArrow
{
    internal class TNTArrowPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "WeaponToAMMO.Arrow.TNTArrow";
        public override string Texture => "CalamityRangerExpansion/Content/WeaponToAMMO/Arrow/TNTArrow/TNTArrow";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
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
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.arrow = true;
            Projectile.penetrate = 6;
            Projectile.timeLeft = 180;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
        }
        private bool hasHitEnemy = false; // 标记是否击中过敌人
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 6, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            }

        }

        public override void OnKill(int timeLeft)
        {
            Projectile.ExpandHitboxBy(32);
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            for (int j = 0; j < 5; j++)
            {
                int fire = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 2f);
                Main.dust[fire].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[fire].scale = 0.5f;
                    Main.dust[fire].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int k = 0; k < 10; k++)
            {
                int fire = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 3f);
                Main.dust[fire].noGravity = true;
                Main.dust[fire].velocity *= 5f;
                fire = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 2f);
                Main.dust[fire].velocity *= 2f;
            }

            if (Main.netMode != NetmodeID.Server)
            {
                Vector2 goreSource = Projectile.Center;
                int goreAmt = 3;
                Vector2 source = new Vector2(goreSource.X - 24f, goreSource.Y - 24f);
                for (int goreIndex = 0; goreIndex < goreAmt; goreIndex++)
                {
                    float velocityMult = 0.33f;
                    if (goreIndex < goreAmt / 3)
                    {
                        velocityMult = 0.66f;
                    }
                    if (goreIndex >= 2 * goreAmt / 3)
                    {
                        velocityMult = 1f;
                    }
                    //Mod mod = ModContent.GetInstance<CalamityMod>();
                    int type = Main.rand.Next(61, 64);
                    int smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    Gore gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X += 1f;
                    gore.velocity.Y += 1f;
                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X -= 1f;
                    gore.velocity.Y += 1f;
                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X += 1f;
                    gore.velocity.Y -= 1f;
                    type = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= velocityMult;
                    gore.velocity.X -= 1f;
                    gore.velocity.Y -= 1f;
                }
            }

            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f, 0.1f);
                    int flames = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<TotalityFire>(), (int)(Projectile.damage * 0.3), 0f, Projectile.owner);
                    if (flames.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[flames].DamageType = DamageClass.Ranged;
                        Main.projectile[flames].penetrate = 3;
                        Main.projectile[flames].usesLocalNPCImmunity = false;
                        Main.projectile[flames].usesIDStaticNPCImmunity = true;
                        Main.projectile[flames].idStaticNPCHitCooldown = 10;
                    }
                }
            }
        }
        private int totalityFireCount = 3; // 初始发射 3 个火焰弹幕

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // 反弹逻辑
            if (Projectile.velocity.X != oldVelocity.X) Projectile.velocity.X = -oldVelocity.X; // 反弹 X 方向
            if (Projectile.velocity.Y != oldVelocity.Y) Projectile.velocity.Y = -oldVelocity.Y; // 反弹 Y 方向

            // 每次反弹减少穿透次数
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill(); // 穿透次数耗尽则销毁
                return false;
            }

            // 播放爆炸音效
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            // 在原地生成一颗手榴弹（保持原属性）
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ProjectileID.Grenade, (int)(Projectile.damage * 0.3), 0f, Projectile.owner);

            // 生成 TotalityFire
            for (int i = 0; i < totalityFireCount; i++)
            {
                Vector2 fireVelocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(2f, 5f); // 随机方向
                int flames = Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    fireVelocity,
                    ModContent.ProjectileType<TotalityFire>(),
                    (int)(Projectile.damage * 0.3),
                    0f,
                    Projectile.owner
                );

                if (flames.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[flames].DamageType = DamageClass.Ranged;
                    Main.projectile[flames].penetrate = 3;
                    Main.projectile[flames].usesLocalNPCImmunity = false;
                    Main.projectile[flames].usesIDStaticNPCImmunity = true;
                    Main.projectile[flames].idStaticNPCHitCooldown = 10;
                }
            }

            // 每次反弹增加 TotalityFire 发射数量
            totalityFireCount++;

            return false; // 不销毁弹幕，而是让其继续存活
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }
    }
}
