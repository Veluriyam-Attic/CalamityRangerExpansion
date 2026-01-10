namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.HD2.SG225IE
{
    public class SG225IEProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.SG225IE";

        //public override string Texture => "CalamityMod/Projectiles/InvisibleProj"; // 使用完全透明贴图
        public override void SetStaticDefaults()
        {
            // 设置弹幕拖尾长度和模式
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 60, 60, Projectile.alpha);
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 15;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1; // 可击中次数
            Projectile.timeLeft = 100;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 4; // 可调节飞行平滑度
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            // 原版与模组灼烧
            target.AddBuff(BuffID.OnFire, 300);          // 燃烧
            target.AddBuff(BuffID.CursedInferno, 300);   // 咒火
            target.AddBuff(BuffID.Daybreak, 300);        // 破晓
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 300); // 神圣之火
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            // 直接将本次命中伤害限制为 1
            modifiers.SetMaxDamage(1);
        }
        public override void OnSpawn(IEntitySource source)
        {

        }


        public override void AI()
        {
            // 玩家伤害判定窗口
            Projectile.hostile = Projectile.timeLeft <= 270;

            // 额外更新时，AI 每帧会被调用多次；这里用 numUpdates 只在“主帧”做一次衰减和重特效
            bool mainFrame = Projectile.numUpdates == 0;

            // 调整旋转方向
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            // 轻微减速（每帧一次）
            if (mainFrame)
                Projectile.velocity *= 0.99f;

            // 方向基：前向 / 后向（用于“半球体”喷发）
            Vector2 forward = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            Vector2 back = -forward;

            // -------------------------
            // 主体：高科技火焰滞留（四方粒子 + 高密度轻烟）
            // -------------------------
            if (mainFrame)
            {
                // 四方粒子数量（每帧 2~3 个）
                int squareCount = Main.rand.Next(2, 4);
                for (int i = 0; i < squareCount; i++)
                {
                    // 在“后向半球”取样：角度围绕 back，范围 ±90°
                    float ang = back.ToRotation() + Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);
                    Vector2 hemiDir = ang.ToRotationVector2();
                    float radius = Main.rand.NextFloat(2f, 10f);
                    Vector2 offset = hemiDir * radius;

                    // 颜色：橙火为主，少量偏青的科技感（别太蓝）
                    Color squareColor = Color.Lerp(Color.OrangeRed, Color.Cyan, 0.18f) * 1.25f;

                    // 速度：整体跟随弹幕，但更“滞留”，并向后外扩一点点
                    Vector2 pVel = Projectile.velocity * 0.12f + hemiDir * Main.rand.NextFloat(0.15f, 0.9f);

                    SquareParticle squareParticle = new SquareParticle(
                        Projectile.Center + offset,
                        pVel,
                        false,
                        30,
                        1.4f + Main.rand.NextFloat(0.7f),
                        squareColor
                    );
                    GeneralParticleHandler.SpawnParticle(squareParticle);
                }

                //// 轻型烟雾：小一点、密一点（每帧 1~2 个）
                //int smokeCount = Main.rand.Next(1, 3);
                //for (int i = 0; i < smokeCount; i++)
                //{
                //    float ang = back.ToRotation() + Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);
                //    Vector2 hemiDir = ang.ToRotationVector2();
                //    Vector2 offset = hemiDir * Main.rand.NextFloat(1f, 6f);

                //    Particle smokeL = new HeavySmokeParticle(
                //        Projectile.Center + offset,
                //        Projectile.velocity * 0.10f + hemiDir * Main.rand.NextFloat(0.05f, 0.35f),
                //        Color.WhiteSmoke,
                //        Main.rand.Next(14, 20),
                //        Main.rand.NextFloat(0.55f, 0.95f),
                //        0.28f,
                //        Main.rand.NextFloat(-1f, 1f),
                //        false
                //    );
                //    GeneralParticleHandler.SpawnParticle(smokeL);
                //}
            }

            //// -------------------------
            //// 点缀：线性火花 + 少量 Dust 尖刺感
            //// -------------------------
            //if (Main.rand.NextBool(4))
            //{
            //    // 线性粒子：更像“拉丝”的能量火痕
            //    Particle trail = new SparkParticle(
            //        Projectile.Center,
            //        Projectile.velocity * 0.18f + Main.rand.NextVector2Circular(0.25f, 0.25f),
            //        false,
            //        50,
            //        Main.rand.NextFloat(0.75f, 1.1f),
            //        Color.Orange
            //    );
            //    GeneralParticleHandler.SpawnParticle(trail);
            //}

            if (Main.rand.NextBool(1))
            {
                // 少量 Dust：尖刺型点缀（别太多）
                float ang = back.ToRotation() + Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);
                Vector2 hemiDir = ang.ToRotationVector2();
                Vector2 pos = Projectile.Center + hemiDir * Main.rand.NextFloat(0f, 8f);

                Dust flame = Dust.NewDustPerfect(
                    pos,
                    DustID.Torch,
                    Projectile.velocity * 0.05f + hemiDir * Main.rand.NextFloat(0.2f, 1.0f),
                    120,
                    Color.OrangeRed,
                    Main.rand.NextFloat(0.9f, 1.35f)
                );
                flame.noGravity = true;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // ✂️ 每次命中目标后永久降低当前弹幕的伤害乘数
            Projectile.damage = (int)(Projectile.damage * 0.3f);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 施加 / 刷新专属灼烧
            target.AddBuff(ModContent.BuffType<SG225IEEDebuff>(), 240);

            // 叠层（实时影响每帧掉血速度）
            SG225IEEGlobalNPC g = target.GetGlobalNPC<SG225IEEGlobalNPC>();
            g.ApplyStack(240);

            // 命中瞬间的“喷射 + 火花”冲击感
            // 火花：少量尖刺型点缀
            for (int i = 0; i < 3; i++)
            {
                Particle spark = new SparkParticle(
                    target.Center + Main.rand.NextVector2Circular(8f, 10f),
                    new Vector2(0f, -1f).RotatedBy(Main.rand.NextFloat(-0.55f, 0.55f)) * Main.rand.NextFloat(1.5f, 3.2f),
                    false,
                    22,
                    Main.rand.NextFloat(0.8f, 1.2f),
                    Color.OrangeRed
                );
                GeneralParticleHandler.SpawnParticle(spark);
            }

        
        }


        public override void OnKill(int timeLeft)
        {
            // 🔥火焰尘埃四散
            for (int i = 0; i < 12; i++)
            {
                Dust.NewDust(
                    Projectile.Center,
                    10, 10,
                    DustID.Torch,
                    Main.rand.NextFloat(-2.4f, 2.4f),
                    Main.rand.NextFloat(-2.4f, 2.4f));
            }

            // 🌫️外圈大烟雾（大而稀疏）
            for (int i = 0; i < 5; i++)
            {
                Vector2 offset = Main.rand.NextVector2CircularEdge(12f, 12f);
                Particle outerSmoke = new HeavySmokeParticle(
                    Projectile.Center + offset,
                    offset * 0.08f,
                    Color.WhiteSmoke,
                    20,
                    Main.rand.NextFloat(1.2f, 1.8f),
                    0.3f,
                    Main.rand.NextFloat(-1f, 1f),
                    false
                );
                GeneralParticleHandler.SpawnParticle(outerSmoke);
            }

            // 🌫️内圈小烟雾（紧贴弹幕中心）
            for (int i = 0; i < 4; i++)
            {
                Vector2 offset = Main.rand.NextVector2Circular(5f, 5f);
                Particle innerSmoke = new HeavySmokeParticle(
                    Projectile.Center + offset,
                    offset * 0.05f,
                    Color.WhiteSmoke,
                    18,
                    Main.rand.NextFloat(0.9f, 1.3f),
                    0.35f,
                    Main.rand.NextFloat(-1f, 1f),
                    false
                );
                GeneralParticleHandler.SpawnParticle(innerSmoke);
            }

        }





    }
}
