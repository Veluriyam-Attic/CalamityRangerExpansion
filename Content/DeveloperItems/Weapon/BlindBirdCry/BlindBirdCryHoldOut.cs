namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.BlindBirdCry
{
    public class BlindBirdCryHoldOut : BaseGunHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.BlindBirdCry";
        public override string Texture => "CalamityRangerExpansion/Content/DeveloperItems/Weapon/BlindBirdCry/BlindBirdCry";

        public override int AssociatedItemID => ModContent.ItemType<BlindBirdCry>();

        public override float MaxOffsetLengthFromArm => 15f; // 后坐力偏移

        public override Vector2 GunTipPosition => Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * (Projectile.width * 0.5f + 10f);

        private bool hasFiredBlindBirdCryINVPROJ = false; // 确保只发射一次
        public override void HoldoutAI()
        {
            Player player = Main.player[Projectile.owner];

            if (!hasFiredBlindBirdCryINVPROJ)
            {
                // 播放音效
                SoundEngine.PlaySound(SoundID.Item92, Projectile.Center);

                // 发射弹幕
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    GunTipPosition, // 发射位置
                    Vector2.UnitX.RotatedBy(Projectile.rotation) * 15f, // 固定方向和初始速度
                    ModContent.ProjectileType<BlindBirdCryINVPROJ>(), // 弹幕类型
                    Projectile.damage, // 固定伤害倍率为 1.0
                    Projectile.knockBack, // 使用当前的击退值
                    player.whoAmI // 发射者
                );

                // 在枪口位置生成黑色粒子边框
                int particleCountPerSide = 10; // 每条边的粒子数量
                float squareSize = 30f; // 正方形的边长

                Vector2[] squareCorners = new Vector2[]
                {
                new Vector2(-squareSize / 2, -squareSize / 2), // 左上角
                new Vector2(squareSize / 2, -squareSize / 2),  // 右上角
                new Vector2(squareSize / 2, squareSize / 2),   // 右下角
                new Vector2(-squareSize / 2, squareSize / 2)   // 左下角
                };

                for (int i = 0; i < squareCorners.Length; i++)
                {
                    Vector2 startCorner = squareCorners[i];
                    Vector2 endCorner = squareCorners[(i + 1) % squareCorners.Length];
                    Vector2 direction = (endCorner - startCorner) / particleCountPerSide;

                    for (int j = 0; j < particleCountPerSide; j++)
                    {
                        Vector2 particlePosition = GunTipPosition + startCorner + direction * j;

                        Particle particle = new SparkParticle(
                            particlePosition, // 粒子位置
                            (Main.MouseWorld - GunTipPosition).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(2f, 5f), // 粒子速度，朝鼠标方向
                            false,
                            60, // 粒子寿命
                            1f, // 粒子缩放
                            Color.Black // 粒子颜色
                        );

                        GeneralParticleHandler.SpawnParticle(particle);
                    }
                }

                hasFiredBlindBirdCryINVPROJ = true; // 标记为已发射
            }

            // 如果当前弹幕即将消失，销毁所有 BlindBirdCryINVPROJ 弹幕
            if (Projectile.timeLeft <= 0 || !Projectile.active)
            {
                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.active && proj.type == ModContent.ProjectileType<BlindBirdCryINVPROJ>())
                    {
                        proj.Kill(); // 销毁弹幕
                    }
                }
            }
        }
    }
}
