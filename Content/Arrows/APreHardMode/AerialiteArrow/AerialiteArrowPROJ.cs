


namespace CalamityRangerExpansion.Content.Arrows.APreHardMode.AerialiteArrow
{
    public class AerialiteArrowPROJ : ModProjectile, ILocalizedModType
    {
        public override string Texture => "CalamityRangerExpansion/Content/Arrows/APreHardMode/AerialiteArrow/AerialiteArrow";

        public new string LocalizationCategory => "Projectile.APreHardMode";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            // 设置弹幕的基础属性
            Projectile.width = 11; // 弹幕宽度
            Projectile.height = 24; // 弹幕高度
            Projectile.friendly = true; // 对敌人有效
            Projectile.DamageType = DamageClass.Ranged; // 远程伤害类型
            Projectile.penetrate = 1; // 穿透力为1，击中一个敌人就消失
            Projectile.timeLeft = 600; // 弹幕存在时间为600帧
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
            Projectile.ignoreWater = true; // 弹幕不受水影响
            Projectile.arrow = true;
            Projectile.extraUpdates = 1;
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.velocity *= 0.7f;
        }
        public override void AI()
        {
            // 调整弹幕的旋转，使其在飞行时保持水平
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;

            // Lighting - 添加天蓝色光源，光照强度为 0.49
            Lighting.AddLight(Projectile.Center, Color.LightSkyBlue.ToVector3() * 0.49f);

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 使用sin函数生成粒子特效，数量较少且粒子会弹开并旋转消失
                if (Projectile.timeLeft % 2 == 0) // 每2帧生成粒子
                {
                    float sineValue = (float)Math.Sin(Projectile.timeLeft * 0.1f);
                    for (int i = 0; i < 2; i++) // 每次生成两个粒子
                    {
                        Vector2 offset = new Vector2(10 * sineValue, 0).RotatedBy(Projectile.velocity.ToRotation() + MathHelper.PiOver2);
                        Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, 135, Projectile.velocity.RotatedByRandom(MathHelper.PiOver4) * 0.5f, 100, Color.LightSkyBlue, 1f);
                        dust.noGravity = true;
                        dust.velocity *= 0.75f;
                        dust.fadeIn = 0.2f;
                    }
                }
            }
        }


        public override void OnKill(int timeLeft)
        {
            // 在消失的地方为圆心，在半径35格的范围内生成一个AerialiteArrowWIND弹幕
            for (int i = 0; i < 1; i++)
            {
                // 随机生成点在半径为35格的圆形区域内，指向消失点
                Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(35 * 16, 35 * 16);
                Vector2 velocity = (Projectile.Center - spawnPos).SafeNormalize(Vector2.Zero) * 15;

                // 生成AerialiteArrowWIND弹幕
                //Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPos, velocity,
                //    ModContent.ProjectileType<AerialiteArrowWIND>(), (int)(Projectile.damage * 0.3f), 0f, Projectile.owner);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPos, Vector2.Zero, 
                    ModContent.ProjectileType<AerialiteArrowWIND>(), (int)(Projectile.damage * 0.1f), 0f, Projectile.owner);// 初始速度设置为0


                // 检查是否启用了特效
                if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
                {
                    // 在生成点生成粒子特效
                    for (int j = 0; j < 360; j += 10)
                    {
                        Vector2 offset = new Vector2(5, 5).RotatedBy(MathHelper.ToRadians(j));
                        Dust dust = Dust.NewDustPerfect(spawnPos + offset, 135, Vector2.Zero, 100, Color.LightSkyBlue, 1.2f);
                        dust.noGravity = true; // 粒子无重力
                        dust.fadeIn = 0.5f; // 粒子淡出效果
                    }
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {

                // 获取 SpriteBatch 和投射物纹理
                SpriteBatch spriteBatch = Main.spriteBatch;
                Texture2D lightTexture = ModContent.Request<Texture2D>("CalamityRangerExpansion/Content/Arrows/APreHardMode/AerialiteArrow/AerialiteArrow").Value;

                // 遍历投射物的旧位置数组，绘制光学拖尾效果
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    // 计算颜色插值值，使颜色在旧位置之间平滑过渡
                    float colorInterpolation = (float)Math.Cos(Projectile.timeLeft / 32f + Main.GlobalTimeWrappedHourly / 20f + i / (float)Projectile.oldPos.Length * MathHelper.Pi) * 0.5f + 0.5f;

                    // 使用天蓝色渐变
                    Color color = Color.Lerp(Color.LightSkyBlue, Color.Cyan, colorInterpolation) * 0.4f;
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
    }
}