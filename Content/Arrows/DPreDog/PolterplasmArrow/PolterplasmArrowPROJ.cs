namespace CalamityRangerExpansion.Content.Arrows.DPreDog.PolterplasmArrow
{
    public class PolterplasmArrowPROJ : ModProjectile, ILocalizedModType
    {
        public override string Texture => "CalamityRangerExpansion/Content/Arrows/DPreDog/PolterplasmArrow/PolterplasmArrow";
        public new string LocalizationCategory => "Projectile.DPreDog";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 获取 SpriteBatch 和投射物纹理
                SpriteBatch spriteBatch = Main.spriteBatch;
                Texture2D lightTexture = ModContent.Request<Texture2D>("CalamityRangerExpansion/Content/Arrows/DPreDog/PolterplasmArrow/PolterplasmArrow").Value;

                // 遍历投射物的旧位置数组，绘制光学拖尾效果
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    // 计算颜色插值值，使颜色在旧位置之间平滑过渡
                    float colorInterpolation = (float)Math.Cos(Projectile.timeLeft / 32f + Main.GlobalTimeWrappedHourly / 20f + i / (float)Projectile.oldPos.Length * MathHelper.Pi) * 0.5f + 0.5f;

                    // 使用浅粉红色渐变
                    Color color = Color.Lerp(Color.LightPink, Color.Pink, colorInterpolation) * 0.4f;
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

        public override void AI()
        {
            // 调整弹幕的旋转，使其在飞行时保持水平
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;

            // Lighting - 添加淡粉色光源，光照强度为 0.55
            Lighting.AddLight(Projectile.Center, Color.LightPink.ToVector3() * 0.55f);

            // 如果是强追踪，为何不在短暂飞行后缓慢暂停，随后在原地引发虚渊之锋同款的小爆炸，然后释放一道激光飞过去？



        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 1% 的概率为敌人施加 1 秒钟的 PolterplasmArrowEDeBuff
            if (Main.rand.NextFloat() < 0.01f)
            {
                // 在主弹幕当前位置生成 PolterplasmArrowINV
                Vector2 spawnPosition0 = Projectile.Center; // 使用主弹幕的当前位置
                Vector2 velocity = Projectile.velocity; // 初始速度与主弹幕一致

                // 生成 PolterplasmArrowINV
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(), // 弹幕来源
                    spawnPosition0, // 生成位置为主弹幕当前位置
                    velocity, // 初始速度与主弹幕一致
                    ModContent.ProjectileType<PolterplasmArrowINV>(), // 生成的弹幕类型
                    1, // 伤害设置为1
                    0f, // 击退为0
                    Main.myPlayer // 所属玩家
                );

                target.AddBuff(ModContent.BuffType<PolterplasmArrowEDeBuff2>(), 300); // 300 帧相当于 5 秒钟

                // 在原地往周围 360 度随机选择三个角度释放 PolterplasmArrowsSoul 弹幕 (一倍伤害)
                for (int i = 0; i < 3; i++)
                {
                    float randomAngle = Main.rand.NextFloat(MathHelper.TwoPi); // 360 度随机角度
                    Vector2 direction = new Vector2((float)Math.Cos(randomAngle), (float)Math.Sin(randomAngle)) * 10f; // 自定义速度
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, direction, ModContent.ProjectileType<PolterplasmArrowsSoul>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }

                // 在半径为 20 个方块的圆环上随机选择 6 个点并向中心发射 PolterplasmArrowsSoul 弹幕 (5 倍伤害)
                float radius = 20 * 16f; // 20 个方块的半径 (20 * 16 像素)
                for (int i = 0; i < 6; i++)
                {
                    float randomAngle = Main.rand.NextFloat(MathHelper.TwoPi); // 360 度随机角度
                    Vector2 spawnPosition = Projectile.Center + new Vector2((float)Math.Cos(randomAngle), (float)Math.Sin(randomAngle)) * radius; // 计算圆周上的点
                    Vector2 directionToCenter = Vector2.Normalize(Projectile.Center - spawnPosition) * 10f; // 方向向内，速度自定义
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPosition, directionToCenter, ModContent.ProjectileType<PolterplasmArrowsSoul>(), (int)(Projectile.damage * 5), Projectile.knockBack, Projectile.owner);
                }
            }
        }
        public override void OnKill(int timeLeft)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 2添加规则扩散的粉红色粒子特效（特殊粒子）
                int points = 25;
                float radians = MathHelper.TwoPi / points;
                Vector2 spinningPoint = Vector2.Normalize(new Vector2(-1f, -1f));
                float rotRando = Main.rand.NextFloat(0.1f, 2.5f);
                for (int k = 0; k < points; k++)
                {
                    Vector2 velocity = spinningPoint.RotatedBy(radians * k).RotatedBy(-0.45f * rotRando);
                    LineParticle subTrail = new LineParticle(Projectile.Center + velocity * 20.5f, velocity * 15, false, 30, 0.75f, Color.LightPink); // 将颜色改为淡粉色
                    GeneralParticleHandler.SpawnParticle(subTrail);
                }


                // 随机扩散的淡粉色粒子特效，反向抛射
                int numRandomParticles = 20;
                for (int i = 0; i < numRandomParticles; i++)
                {
                    // 粒子的反方向速度，带有随机性
                    Vector2 randomVelocity = -Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(30)) * Main.rand.NextFloat(0.5f, 2f);
                    Dust randomDust = Dust.NewDustPerfect(Projectile.Center, DustID.PinkTorch, randomVelocity, 0, Color.LightPink, Main.rand.NextFloat(1.0f, 2.5f));
                    randomDust.noGravity = true;
                    randomDust.fadeIn = 1f;
                    randomDust.scale = Main.rand.NextFloat(1f, 2.5f); // 随机大小
                    randomDust.rotation = Main.rand.NextFloat(MathHelper.TwoPi); // 随机旋转
                    randomDust.velocity *= Main.rand.NextFloat(0.8f, 1.5f); // 随机速度
                }
            }
             
        }
    }
}