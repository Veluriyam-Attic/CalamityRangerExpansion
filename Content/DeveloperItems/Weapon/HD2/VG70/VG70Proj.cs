namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.HD2.VG70
{
    internal class VG70Proj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.VG70";

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
            Projectile.penetrate = 4; // 可击中次数
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 4; // 可调节飞行平滑度
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {



        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {



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
            Projectile.rotation = Projectile.velocity.ToRotation();

            // 轻微减速（每帧一次）
            if (mainFrame)
                Projectile.velocity *= 0.99f;

            // 方向基：前向 / 后向（用于“半球体”喷发）
            Vector2 forward = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            Vector2 back = -forward;

            {
                // =========================
                // 可见性：出生几帧后显现
                // =========================
                if (Projectile.timeLeft == 298)
                    Projectile.alpha = 0;

                // =========================
                // 基础灰色拖尾（稳定线条）
                // =========================
                if (Main.rand.NextBool(3))
                {
                    Dust core = Dust.NewDustPerfect(
                        Projectile.Center,
                        DustID.Smoke, // 灰色烟尘
                        -Projectile.velocity * Main.rand.NextFloat(0.15f, 0.35f),
                        140,
                        Color.Gray,
                        Main.rand.NextFloat(0.5f, 0.8f)
                    );
                    core.noGravity = true;
                    core.fadeIn = 0.4f;
                }

                // =========================
                // DNA 双螺旋（有序数学美感）
                // =========================
                if (Projectile.numUpdates == 0) // 只在主帧生成，避免过密
                {
                    // 时间参数：随 timeLeft 变化即可，稳定又确定
                    float t = (300 - Projectile.timeLeft) * 0.35f;

                    // 垂直方向（相对弹道）
                    Vector2 perp = forward.RotatedBy(MathHelper.PiOver2);

                    // 螺旋半径
                    float radius = 6f;

                    // 两条螺旋：相位差 π
                    for (int i = 0; i < 2; i++)
                    {
                        float phase = t + (i == 0 ? 0f : MathHelper.Pi);

                        Vector2 offset =
                            perp * (float)Math.Sin(phase) * radius;

                        Dust helix = Dust.NewDustPerfect(
                            Projectile.Center + offset,
                            DustID.Smoke,
                            back * 0.25f,
                            160,
                            Color.DarkGray,
                            0.65f
                        );
                        helix.noGravity = true;
                    }
                }

            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {


        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // =========================
            // 灰色命中特效：有序脊线 + 无序尘喷（只用 Dust）
            // =========================
            Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            Vector2 back = -dir;
            Vector2 perp = dir.RotatedBy(MathHelper.PiOver2);

            // 1) 有序：沿反方向“脊线”喷一串（像被刮出的金属粉）
            int spineCount = 7;
            for (int i = 0; i < spineCount; i++)
            {
                float k = i / (float)(spineCount - 1); // 0..1
                Vector2 pos = Projectile.Center + back * (6f + 14f * k) + perp * (2f * (k - 0.5f));

                Dust spine = Dust.NewDustPerfect(
                    pos,
                    31, // 安全 DustID：31（烟/灰系）
                    back * (2.2f + 2.8f * k),
                    140,
                    Color.DarkGray,
                    0.8f - 0.25f * k
                );
                spine.noGravity = true;
            }

            // 2) 无序：扇形喷射（角度集中在后方，带随机）
            int sprayCount = 10;
            float sprayHalfAngle = MathHelper.ToRadians(28f); // 扇形半角
            for (int i = 0; i < sprayCount; i++)
            {
                Vector2 v = back.RotatedBy(Main.rand.NextFloat(-sprayHalfAngle, sprayHalfAngle)) *
                            Main.rand.NextFloat(1.6f, 4.8f);

                Dust spray = Dust.NewDustPerfect(
                    Projectile.Center,
                    31,
                    v,
                    150,
                    Color.Gray,
                    Main.rand.NextFloat(0.75f, 1.15f)
                );
                spray.noGravity = true;
            }

            // 3) 点睛：2~3 个“更亮的碎屑点”（像火花冷却后的灰白亮点）
            int sparkleCount = Main.rand.Next(2, 4);
            for (int i = 0; i < sparkleCount; i++)
            {
                Vector2 v = back.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)) * Main.rand.NextFloat(3.5f, 6.5f);

                Dust sparkle = Dust.NewDustPerfect(
                    Projectile.Center,
                    267, // 安全且偏亮的点刺质感（你前面也用过）
                    v,
                    120,
                    Color.Silver,
                    Main.rand.NextFloat(0.7f, 1.0f)
                );
                sparkle.noGravity = true;
            }
        }



        public override void OnKill(int timeLeft)
        {
         
        }





    }
}
