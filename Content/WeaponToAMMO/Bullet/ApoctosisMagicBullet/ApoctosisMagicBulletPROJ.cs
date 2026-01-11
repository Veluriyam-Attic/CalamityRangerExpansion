namespace CalamityRangerExpansion.Content.WeaponToAMMO.Bullet.ApoctosisMagicBullet
{
    internal class ApoctosisMagicBulletPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "WeaponToAMMO.Bullet.ApoctosisMagicBullet";
        //public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 80;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public float PrimitiveWidthFunction(float completionRatio) => Projectile.scale * 15f;
        public Color PrimitiveColorFunction(float _) => Color.White * Projectile.Opacity;
        public override bool PreDraw(ref Color lightColor)
        {
            //// **确保 Shader 存在**
            //Effect shader = ShaderGames.RainbowShader;

            //// **应用 Shader 变量**
            //shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly);
            //shader.Parameters["uOpacity"].SetValue(1f);

            //// **应用 Shader**
            //Main.spriteBatch.End(); // 先关闭原有的 Begin
            //Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, shader, Main.GameViewMatrix.TransformationMatrix);



            //// **确保 Shader 存在**
            //Effect shader = ShaderGames.EdgeGlowShader;
            //if (shader == null) return true;

            //// **设置 Shader 变量**
            //shader.Parameters["uThreshold"].SetValue(0.1f); // 边缘检测阈值
            //shader.Parameters["uGlowColor"].SetValue(new Vector4(0.5f, 0.8f, 1f, 1f)); // 发光颜色

            //// **应用 Shader**
            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, shader, Main.GameViewMatrix.TransformationMatrix);



            //// **确保 Shader 存在**
            //Effect shader = ShaderGames.GlassRefractionShader;
            //if (shader == null) return true;

            //// **设置 Shader 变量**
            //shader.Parameters["uRefractStrength"].SetValue(0.05f); // 折射强度
            //shader.Parameters["uOpacity"].SetValue(0.7f); // 半透明程度

            //// **应用 Shader**
            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, shader, Main.GameViewMatrix.TransformationMatrix);




            //// **确保 Shader 存在**
            //Effect shader = ShaderGames.DistortionShader;
            //if (shader == null) return true;

            //// **设置 Shader 变量**
            //shader.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly);
            //shader.Parameters["uDistortionStrength"].SetValue(0.1f); // 扭曲强度

            //// **应用 Shader**
            //Main.spriteBatch.End();
            //Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, shader, Main.GameViewMatrix.TransformationMatrix);



            //// **绘制本体**
            //Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            //Vector2 origin = texture.Size() * 0.5f;
            //Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            //Main.spriteBatch.Draw(texture, drawPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);

            //Main.spriteBatch.End(); // 结束 Shader 渲染
            //Main.spriteBatch.Begin(); // 重新启用普通渲染

            return false;
        }



        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1; // 根据模式设置穿透次数
            Projectile.timeLeft = 200;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 33;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            if (player.statMana < 5)
            {
                Projectile.Kill(); // 阻止发射
                return;
            }

            player.statMana -= 5; // 消耗魔力值
            SoundEngine.PlaySound(SoundID.Item91); // 播放音效
        }

        public override void AI()
        {
            // 保持弹幕旋转
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;

            // Lighting - 添加深橙色光源，光照强度为 0.55
            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3() * 0.55f);

            //// 在两侧生成粒子特效
            //for (int i = -1; i <= 1; i += 2) // 循环两次，分别生成左侧和右侧的粒子
            //{
            //    Vector2 offset = Projectile.velocity.RotatedBy(MathHelper.PiOver2 * i) * 1f; // 偏移方向
            //    Dust dust = Dust.NewDustPerfect(
            //        Projectile.Center + offset,
            //        i % 2 == 0 ? 130 : 134, // 粒子特效 ID
            //        Vector2.Zero
            //    );
            //    dust.noGravity = true; // 粒子无重力
            //    dust.scale = 1.2f; // 固定大小
            //}


            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 添加粒子特效，随机 1/3 概率
                if (Main.rand.NextBool(2))
                {
                    Dust dust = Dust.NewDustPerfect(
                        Projectile.Center,
                        Main.rand.NextBool() ? 90 : 130, // 粒子特效 ID 
                        -Projectile.velocity.RotatedByRandom(0.1f) * Main.rand.NextFloat(0.01f, 0.3f)
                    );
                    dust.noGravity = true; // 粒子无重力
                    dust.scale = Main.rand.NextFloat(0.5f, 0.9f); // 随机大小
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 生成红色法阵
            //for (int i = 0; i < 36; i++) // 36 个粒子均匀分布成一个圆环
            //{
            //    float angle = MathHelper.TwoPi / 36 * i; // 计算粒子的角度
            //    Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 50f; // 粒子偏移位置
            //    Dust dust = Dust.NewDustPerfect(
            //        target.Center + offset,
            //        DustID.SomethingRed, // 粒子
            //        Vector2.Zero
            //    );
            //    dust.noGravity = true; // 粒子无重力
            //    dust.scale = 1.5f; // 粒子大小
            //}

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 定义正方形边长（等于原半径）
                float sideLength = 50f; // 原粒子圆环的半径

                // 正方形粒子特效
                for (int i = 0; i < 4; i++) // 正方形的 4 条边
                {
                    Vector2 startPoint = Projectile.Center + new Vector2(
                        (i == 1 || i == 2 ? 1 : -1) * sideLength,
                        (i == 2 || i == 3 ? 1 : -1) * sideLength
                    ); // 每条边的起点
                    Vector2 direction = new Vector2(
                        i % 2 == 0 ? 0 : (i == 1 ? -1 : 1),
                        i % 2 != 0 ? 0 : (i == 0 ? 1 : -1)
                    ); // 边的方向

                    for (int j = 0; j <= 15; j++) // 每条边生成 x 个粒子
                    {
                        Vector2 position = startPoint + direction * (j / 15f) * 2 * sideLength;
                        Dust dust = Dust.NewDustPerfect(
                            position,
                            DustID.RedTorch, // 粒子特效编号
                            Vector2.Zero
                        );
                        dust.noGravity = true;
                        dust.scale = 1.5f; // 粒子大小
                    }
                }

                // 菱形粒子特效
                float rotatedSideLength = sideLength * (float)Math.Sqrt(2) / 2; // 菱形的边长（正方形对角线）
                for (int i = 0; i < 4; i++) // 菱形的 4 条边
                {
                    Vector2 startPoint = Projectile.Center + new Vector2(
                        (i == 1 || i == 2 ? 1 : -1) * rotatedSideLength,
                        (i == 2 || i == 3 ? 1 : -1) * rotatedSideLength
                    ).RotatedBy(MathHelper.PiOver4); // 旋转 45 度
                    Vector2 direction = new Vector2(
                        i % 2 == 0 ? 0 : (i == 1 ? -1 : 1),
                        i % 2 != 0 ? 0 : (i == 0 ? 1 : -1)
                    ).RotatedBy(MathHelper.PiOver4); // 方向旋转 45 度

                    for (int j = 0; j <= 10; j++) // 每条边生成 10 个粒子
                    {
                        Vector2 position = startPoint + direction * (j / 10f) * 2 * rotatedSideLength;
                        Dust dust = Dust.NewDustPerfect(
                            position,
                            DustID.RedStarfish, // 粒子特效编号
                            Vector2.Zero
                        );
                        dust.noGravity = true;
                        dust.scale = 1.5f; // 粒子大小
                    }
                }
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Player player = Main.player[Projectile.owner];
            float manaBonus = player.statMana * 0.005f; // 每点魔力提升 0.5% 的伤害
            modifiers.FinalDamage *= 1f + manaBonus; // 应用伤害倍率
        }

        public override void OnKill(int timeLeft)
        {
          
        }
    }
}