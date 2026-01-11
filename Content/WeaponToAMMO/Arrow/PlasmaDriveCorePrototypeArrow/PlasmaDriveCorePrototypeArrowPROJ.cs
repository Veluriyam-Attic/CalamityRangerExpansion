namespace CalamityRangerExpansion.Content.WeaponToAMMO.Arrow.PlasmaDriveCorePrototypeArrow
{
    public class PlasmaDriveCorePrototypeArrowPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "WeaponToAMMO.Arrow.PlasmaDriveCorePrototypeArrow";
        // 定义初始速度、减速速度和减速时间
        public const float InitialSpeed = 30f;
        public const float SlowdownSpeed = 7f;
        public const int SlowdownTime = 50;
        public static readonly float SlowdownFactor = (float)Math.Pow(SlowdownSpeed / InitialSpeed, 1f / SlowdownTime);

        // 使用 ai[0] 来记录时间
        public ref float Time => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = SlowdownTime;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.Opacity = 1f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;
            Projectile.arrow = true;

            //// 确保初始速度被设置正确
            //if (Projectile.velocity == Vector2.Zero)
            //{
            //    Projectile.velocity = new Vector2(InitialSpeed, 0f).RotatedByRandom(MathHelper.TwoPi); // 随机方向发射
            //}
        }




        public override void AI()
        {
            // 控制旋转方向
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            // Lighting - 添加亮白色光源，光照强度为 0.55
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.55f);


            // Very rapidly slow down and fade out, transforming into light.
            if (Time <= SlowdownTime)
            {
                Projectile.Opacity = (float)Math.Pow(1f - Time / SlowdownTime, 2D);
                Projectile.velocity *= SlowdownFactor;

                // 检查是否启用了特效
                if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
                {
                    int lightDustCount = (int)MathHelper.Lerp(8f, 1f, Projectile.Opacity);
                    for (int i = 0; i < lightDustCount; i++)
                    {
                        Vector2 dustSpawnPosition = Projectile.Center + Main.rand.NextVector2Unit() * (1f - Projectile.Opacity) * 45f;
                        Dust light = Dust.NewDustPerfect(dustSpawnPosition, 267);
                        light.color = Color.Lerp(Color.Gold, Color.White, Main.rand.NextFloat(0.5f, 1f));
                        light.velocity = Main.rand.NextVector2Circular(10f, 10f);
                        light.scale = MathHelper.Lerp(1.3f, 0.8f, Projectile.Opacity) * Main.rand.NextFloat(0.8f, 1.2f);
                        light.noGravity = true;
                    }
                }
            }

            Time++;
        }

        private void CreateLightEffect()
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 发光粒子效果仿照SeraphimProjectile
                int lightDustCount = (int)MathHelper.Lerp(8f, 1f, Projectile.Opacity);
                for (int i = 0; i < lightDustCount; i++)
                {
                    Vector2 dustSpawnPosition = Projectile.Center + Main.rand.NextVector2Unit() * (1f - Projectile.Opacity) * 45f;
                    Dust light = Dust.NewDustPerfect(dustSpawnPosition, 267);
                    light.color = Color.Lerp(Color.Gold, Color.White, Main.rand.NextFloat(0.5f, 1f));
                    light.velocity = Main.rand.NextVector2Circular(10f, 10f);
                    light.scale = MathHelper.Lerp(1.3f, 0.8f, Projectile.Opacity) * Main.rand.NextFloat(0.8f, 1.2f);
                    light.noGravity = true;
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            // 在弹幕消失时，释放SHPExplosion
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PlasmaDriveCorePrototypeArrowEXP>(), (int)(Projectile.damage * 0.25f), Projectile.knockBack, Projectile.owner);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 模仿SeraphimProjectile的PreDraw方法
                Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
                Vector2 origin = texture.Size() * 0.5f;
                Vector2 baseDrawPosition = Projectile.Center - Main.screenPosition;

                // 逐渐消失的光效
                float endFade = Utils.GetLerpValue(0f, 12f, Projectile.timeLeft, true);
                Color mainColor = Color.White * Projectile.Opacity * endFade * 1.5f;
                mainColor.A = (byte)(255 - Projectile.alpha);

                Color afterimageLightColor = Color.White * endFade;
                afterimageLightColor.A = (byte)(255 - Projectile.alpha);

                // 绘制多个逐渐淡出的光影
                for (int i = 0; i < 18; i++)
                {
                    Vector2 drawPosition = baseDrawPosition + (MathHelper.TwoPi * i / 18f).ToRotationVector2() * (1f - Projectile.Opacity) * 16f;
                    Main.EntitySpriteDraw(texture, drawPosition, null, afterimageLightColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
                }

                // 绘制特殊的残影效果
                for (int i = 0; i < 8; i++)
                {
                    Vector2 drawPosition = baseDrawPosition - Projectile.velocity * i * 0.3f;
                    Color afterimageColor = mainColor * (1f - i / 8f);
                    Main.EntitySpriteDraw(texture, drawPosition, null, afterimageColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
                }

                return false; // 不使用默认绘制
            }
            return true;
        }
    }
}