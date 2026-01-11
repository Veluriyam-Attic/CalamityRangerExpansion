namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.HD2.VG70
{
    public class VG70Hold : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.VG70";
        public override string Texture => "CalamityRangerExpansion/Content/DeveloperItems/Weapon/HD2/VG70/VG70";

        // =========================
        // 状态机
        // =========================
        private const int State_Normal = 0;
        private const int State_Burst = 1;
        private const int State_Cooldown = 2;

        private int state = State_Normal;

        // =========================
        // 计时器
        // =========================
        private int warmupTimer = 10;
        private int fireTimer;
        private int burstShotCounter;
        private int burstDelay;
        private int burstCooldown;

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 2;
        }

        public Vector2 GunTipPosition =>
            Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * (Projectile.width * 1.05f + 5f);
        
        // =========================
        // AI 主逻辑
        // =========================
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            Projectile.timeLeft = 2;
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            Vector2 dir = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX);

            int faceDir = dir.X >= 0f ? 1 : -1;
            player.ChangeDir(faceDir);
            Projectile.direction = faceDir;
            Projectile.spriteDirection = faceDir;

            Projectile.rotation = dir.ToRotation();
            Projectile.Center = player.MountedCenter + dir * 72f;

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);

            // =========================
            // 预热
            // =========================
            if (warmupTimer > 0)
            {
                if (warmupTimer == 10)
                    SoundEngine.PlaySound(SoundID.Item15, Projectile.Center);

                warmupTimer--;
                return;
            }

            if (state != State_Cooldown && burstCooldown > 0)
                burstCooldown--;

            // =========================
            // 右键触发双管喷
            // =========================
            if (state == State_Normal && burstCooldown <= 0 && player.Calamity().mouseRight)
            {
                state = State_Burst;
                burstShotCounter = 0;
                burstDelay = 0;
            }

            // =========================
            // 双管喷（不可中断）
            // =========================
            if (state == State_Burst)
            {
                burstDelay++;

                // 间隔翻倍
                if (burstDelay >= 9)
                {
                    burstDelay = 0;
                    FireBurst(player, dir);
                    burstShotCounter++;

                    if (burstShotCounter >= 2)
                    {
                        state = State_Cooldown;
                        burstCooldown = 150;
                    }
                }

                return;
            }

            // =========================
            // 冷却阶段（强制存在）
            // =========================
            if (state == State_Cooldown)
            {
                burstCooldown--;

                if (burstCooldown <= 0)
                    state = State_Normal;

                return;
            }

            // =========================
            // 普通加特林
            // =========================
            fireTimer++;
            if (fireTimer >= 5)
            {
                fireTimer = 0;
                FireNormal(player, dir);
            }

            if (!player.channel)
                Projectile.Kill();
        }

        // =========================
        // 普通射击
        // =========================
        private void FireNormal(Player player, Vector2 dir)
        {
            if (!player.PickAmmo(player.ActiveItem(), out int projType, out float speed, out int damage, out float kb, out int ammoType))
                return;

            Vector2 muzzle = GunTipPosition;
            Vector2 spawnPos = muzzle + Main.rand.NextVector2Circular(6f, 6f);

            if (ammoType == ItemID.MusketBall)
                projType = ModContent.ProjectileType<VG70Proj>();

            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                spawnPos,
                dir * speed * 2.2f,
                projType,
                damage,
                kb,
                Projectile.owner
            );

            SoundEngine.PlaySound(
                new SoundStyle("CalamityRangerExpansion/Content/DeveloperItems/Weapon/HD2/VG70/VG70变量普通开火") with { Volume = 3.0f, Pitch = 0.0f },
                Projectile.Center
            );
        }

        // =========================
        // 双管喷射击（强化版）
        // =========================
        private void FireBurst(Player player, Vector2 dir)
        {
            if (!player.PickAmmo(player.ActiveItem(), out int projType, out float speed, out int damage, out float kb, out int ammoType))
                return;

            Vector2 muzzle = GunTipPosition;

            // 📸 强力屏幕震动
            if (Main.myPlayer == player.whoAmI)
            {
                float shakePower = 20f;
                float distFactor = Utils.GetLerpValue(1000f, 0f, Projectile.Distance(Main.LocalPlayer.Center), true);
                Main.LocalPlayer.Calamity().GeneralScreenShakePower =
                    Math.Max(Main.LocalPlayer.Calamity().GeneralScreenShakePower, shakePower * distFactor);
            }


            player.Hurt(
                PlayerDeathReason.ByCustomReason(
                    NetworkText.FromLiteral($"{player.name} was liberated by excessive firepower.")
                ),
                10,
                0,
                dodgeable: false,
                armorPenetration: int.MaxValue
            );


            // 💥 后坐力
            player.velocity += -dir * 5.8f;

            {
                // =========================
                // 枪口特效（极端夸张版）
                // =========================

                // 灰色烟雾 · 巨量喷射
                for (int i = 0; i < 180; i++)
                {
                    Vector2 vel = dir.RotatedByRandom(1.2f) * Main.rand.NextFloat(4f, 14f);

                    Dust smoke = Dust.NewDustPerfect(
                        muzzle,
                        DustID.Smoke,
                        vel,
                        80,
                        Color.Gray,
                        Main.rand.NextFloat(2.5f, 4.2f)
                    );
                    smoke.noGravity = true;
                }

                // 火焰 · 核爆级喷口焰
                for (int i = 0; i < 140; i++)
                {
                    Vector2 vel = dir.RotatedByRandom(0.9f) * Main.rand.NextFloat(10f, 26f);

                    Dust flame = Dust.NewDustPerfect(
                        muzzle,
                        DustID.Torch,
                        vel,
                        0,
                        Color.OrangeRed,
                        Main.rand.NextFloat(2.8f, 5.0f)
                    );
                    flame.noGravity = true;
                }

                // 高速火焰拖尾（线性粒子，数量翻倍）
                for (int i = 0; i < 12; i++)
                {
                    Particle trail = new SparkParticle(
                        muzzle,
                        dir.RotatedByRandom(0.15f) * Main.rand.NextFloat(6f, 12f),
                        false,
                        80,
                        Main.rand.NextFloat(1.6f, 2.4f),
                        Color.Orange
                    );
                    GeneralParticleHandler.SpawnParticle(trail);
                }

                // 椭圆冲击波 · 加粗 + 更猛
                for (int i = 0; i < 3; i++)
                {
                    Particle pulse = new DirectionalPulseRing(
                        muzzle,
                        dir * Main.rand.NextFloat(0.8f, 1.4f),
                        Color.OrangeRed,
                        new Vector2(1.2f, 3.5f),
                        Projectile.rotation + Main.rand.NextFloat(-0.2f, 0.2f),
                        0.45f,
                        0.05f,
                        26
                    );
                    GeneralParticleHandler.SpawnParticle(pulse);
                }
            }

            // =========================
            // 30 发散射
            // =========================
            for (int i = 0; i < 30; i++)
            {
                float spread = MathHelper.ToRadians(Main.rand.NextFloat(-5f, 5f));
                Vector2 shootDir = dir.RotatedBy(spread);
                Vector2 spawnPos = muzzle + Main.rand.NextVector2Circular(10f, 10f);

                int finalProj = projType;
                if (ammoType == ItemID.MusketBall)
                    finalProj = ModContent.ProjectileType<VG70Proj>();

                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    spawnPos,
                    shootDir * speed * 2.2f,
                    finalProj,
                    (int)(damage * 1.2f),
                    kb,
                    Projectile.owner
                );
            }

            SoundEngine.PlaySound(
                new SoundStyle("CalamityRangerExpansion/Content/DeveloperItems/Weapon/HD2/VG70/VG70变量连射开火") with { Volume = 3.0f, Pitch = 0.0f },
                Projectile.Center
            );


        }

        // =========================
        // 冷却条绘制
        // =========================
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Player player = Main.player[Projectile.owner];

            SpriteEffects effects =
                (Projectile.spriteDirection * player.gravDir == -1f)
                    ? SpriteEffects.FlipHorizontally
                    : SpriteEffects.None;

            float rotation =
                Projectile.rotation +
                (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f);

            Main.EntitySpriteDraw(
                tex,
                Projectile.Center - Main.screenPosition,
                null,
                lightColor,
                rotation,
                tex.Size() * 0.5f,
                1f,
                effects,
                0f
            );

            if (state == State_Cooldown)
            {
                var barBG = ModContent.Request<Texture2D>("CalamityMod/UI/MiscTextures/GenericBarBack").Value;
                var barFG = ModContent.Request<Texture2D>("CalamityMod/UI/MiscTextures/GenericBarFront").Value;

                float ratio = burstCooldown / 150f;

                Vector2 pos = player.Center - Main.screenPosition + new Vector2(0, -56f) - barBG.Size() * 0.5f;
                Rectangle crop = new Rectangle(0, 0, (int)(barFG.Width * ratio), barFG.Height);

                Main.spriteBatch.Draw(barBG, pos, null, Color.DarkRed, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(barFG, pos, crop, Color.Red, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }

            return false;
        }
    }
}
