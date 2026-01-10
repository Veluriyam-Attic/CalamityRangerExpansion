namespace CalamityRangerExpansion.Content.Ammunition.DPreDog.DivineGeodeBullet
{
    public class DivineGeodeBulletPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectile.DPreDog";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 获取 SpriteBatch 和投射物纹理
                SpriteBatch spriteBatch = Main.spriteBatch;
                Texture2D lightTexture = TextureAssets.Projectile[Projectile.type].Value;

                // 遍历投射物的旧位置数组，绘制光学拖尾效果
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    // 计算颜色插值值，使颜色在旧位置之间平滑过渡
                    float colorInterpolation = (float)Math.Cos(Projectile.timeLeft / 32f + Main.GlobalTimeWrappedHourly / 20f + i / (float)Projectile.oldPos.Length * MathHelper.Pi) * 0.5f + 0.5f;

                    // 使用天蓝色渐变
                    Color color = Color.Lerp(Color.Yellow, Color.LightGoldenrodYellow, colorInterpolation) * 0.4f;
                    color.A = 0;

                    // 计算绘制位置，将位置调整到碰撞箱的中心
                    Vector2 drawPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

                    // 计算外部和内部的颜色
                    Color outerColor = color;
                    Color innerColor = color * 0.5f;

                    // 计算强度，使拖尾逐渐变弱
                    float intensity = 0.9f + 0.15f * (float)Math.Cos(Main.GlobalTimeWrappedHourly % 60f * MathHelper.TwoPi);
                    intensity *= MathHelper.Lerp(0.15f, 1f, 1f - i / (float)Projectile.oldPos.Length);
                    if (Projectile.timeLeft <= 60)
                    {
                        intensity *= Projectile.timeLeft / 60f; // 如果弹幕即将消失，则拖尾也逐渐消失
                    }

                    // 计算外部和内部的缩放比例，使拖尾具有渐变效果
                    Vector2 outerScale = new Vector2(2f) * intensity;
                    Vector2 innerScale = new Vector2(2f) * intensity * 0.7f;
                    outerColor *= intensity;
                    innerColor *= intensity;

                    // 绘制外部的拖尾效果，并应用旋转
                    Main.EntitySpriteDraw(lightTexture, drawPosition, null, outerColor, Projectile.rotation, lightTexture.Size() * 0.5f, outerScale * 0.6f, SpriteEffects.None, 0);

                    // 绘制内部的拖尾效果，并应用旋转
                    Main.EntitySpriteDraw(lightTexture, drawPosition, null, innerColor, Projectile.rotation, lightTexture.Size() * 0.5f, innerScale * 0.6f, SpriteEffects.None, 0);
                }

                // 绘制默认的弹幕，并应用旋转
                Main.EntitySpriteDraw(lightTexture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, lightColor, Projectile.rotation, lightTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
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
            Projectile.timeLeft = 420;
            Projectile.MaxUpdates = 1;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 2;
        }

        public override void AI()
        {
            // 由于我们是水平贴图，因此什么也不需要转动
            Projectile.rotation = Projectile.velocity.ToRotation();

            // 添加光效
            Lighting.AddLight(Projectile.Center, Color.Lerp(Color.Black, Color.Black, 0.5f).ToVector3() * 0.49f);

            // 子弹在出现之后很短一段时间会变得可见
            if (Projectile.timeLeft == 418)
                Projectile.alpha = 0;

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 添加飞行粒子特效
                if (Main.rand.NextBool(2)) // 1/3 概率生成粒子
                {
                    Dust dust = Dust.NewDustPerfect(
                        Projectile.Center,
                        Main.rand.NextBool() ? DustID.ShadowbeamStaff : DustID.Smoke, // 使用黑色主题的 Dust
                        -Projectile.velocity.RotatedByRandom(0.1f) * Main.rand.NextFloat(0.1f, 0.3f)
                    );
                    dust.noGravity = true; // 无重力
                    dust.scale = Main.rand.NextFloat(0.7f, 1.1f); // 随机大小
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
            // 5% 概率恢复敌人 200 点生命值（仅在专家模式下）
            if (Main.expertMode && Main.rand.NextFloat() < 0.05f)
            {
                target.life += 200;
                target.HealEffect(200); // 显示治疗效果
            }

            // 施加 DivineGeodeBulletEDebuff，持续 600 帧
            //target.AddBuff(ModContent.BuffType<DivineGeodeBulletEDebuff>(), 600);

            target.Calamity().miscDefenseLoss = 10;
        }

        public override void OnKill(int timeLeft)
        {
            #region 旧代码
            //if (Main.rand.NextBool(3))
            //{
            //    for (int i = 0; i < 36; i++) // 36 个点形成魔法阵
            //    {
            //        float angle = MathHelper.TwoPi / 36 * i; // 均匀分布
            //        Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 10; // 魔法阵半径为10

            //        Dust dust = Dust.NewDustPerfect(
            //            Projectile.Center + offset,
            //            Main.rand.NextBool() ? 262 : 87, // 粒子特效编号
            //            offset.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.5f, 1.5f), // 向外速度
            //            0,
            //            Color.Lime, // 粒子颜色
            //            Main.rand.NextFloat(0.8f, 1.2f) // 粒子大小
            //        );
            //        dust.noGravity = true;
            //    }
            //}

            //if (Main.rand.NextBool(15))
            //{
            //    for (int i = 0; i < 10; i++) // 中心区域附加特效
            //    {
            //        float angle = MathHelper.TwoPi / 10 * i;
            //        Vector2 velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * Main.rand.NextFloat(1f, 2f);

            //        Dust dust = Dust.NewDustPerfect(
            //            Projectile.Center,
            //            278, // 粒子特效编号
            //            velocity, // 随机偏移
            //            0,
            //            Color.Orange, // 粒子颜色
            //            Main.rand.NextFloat(0.6f, 1f) // 粒子大小
            //        );
            //        dust.noGravity = true;
            //    }
            //}
            #endregion

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 触发 IonizingRadiation 特效
                LightingBoltsSystem.Spawn_IonizingRadiation(Projectile.Center);

                //// 保留现有的小圆圈效果
                //for (int i = 0; i < 36; i++) // 36 个点形成魔法阵
                //{
                //    float angle = MathHelper.TwoPi / 36 * i; // 均匀分布
                //    Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 10; // 魔法阵半径为10

                //    Dust dust = Dust.NewDustPerfect(
                //        Projectile.Center + offset,
                //        Main.rand.NextBool() ? 262 : 87, // 粒子特效编号
                //        offset.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.5f, 1.5f), // 向外速度
                //        0,
                //        default, // 默认颜色
                //        Main.rand.NextFloat(0.8f, 1.2f) // 粒子大小
                //    );
                //    dust.noGravity = true;
                //}


                // 绘制三个扇形的粒子特效
                float radius = 7 * 16f; // 扇形的半径
                float arcAngle = MathHelper.Pi / 3; // 每个扇形的弧度（60 度）
                float baseOffsetAngle = -MathHelper.PiOver2; // 起始角度偏移（从正上方开始）

                // 每个扇形的中心角度
                float[] arcBaseAngles = new float[]
                {
                baseOffsetAngle - MathHelper.Pi / 3, // 左上角
                baseOffsetAngle + MathHelper.Pi / 3, // 右上角
                baseOffsetAngle + MathHelper.Pi // 正下方
                };

                foreach (float arcBaseAngle in arcBaseAngles)
                {
                    for (int i = 0; i < 24; i++) // 每个扇形生成 24 个粒子
                    {
                        float angle = arcBaseAngle + (-arcAngle / 2) + (arcAngle / 24) * i; // 每个粒子的角度
                        Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

                        Vector2 particlePosition = Projectile.Center + direction * radius; // 粒子的位置
                        Dust dust = Dust.NewDustPerfect(
                            particlePosition,
                            Main.rand.NextBool() ? 262 : 87, // 粒子特效编号
                            direction * Main.rand.NextFloat(1.5f, 2.5f), // 粒子向外辐射的速度
                            0,
                            default, // 默认颜色
                            Main.rand.NextFloat(1.5f, 1.65f) // 粒子大小
                        );
                        dust.noGravity = true; // 无重力
                    }
                }
            }

       
        }
    }
}
