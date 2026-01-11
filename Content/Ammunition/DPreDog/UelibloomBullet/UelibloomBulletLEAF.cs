namespace CalamityRangerExpansion.Content.Ammunition.DPreDog.UelibloomBullet
{
    internal class UelibloomBulletLEAF : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectile.DPreDog";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public bool ableToHit = true;
        public NPC target;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
        }
        public ref float Time => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 200;
            Projectile.extraUpdates = 7;
            Projectile.timeLeft = 60000;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 50;
            Projectile.DamageType = DamageClass.Ranged;
        }
        public override bool? CanDamage() => Time >= 80f; // 初始的时候不会造成伤害，直到x为止


        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.localAI[0] += 1f / (Projectile.extraUpdates + 1);

            if (Projectile.localAI[0] < 65f) // 前 x 帧速度逐渐降低
            {
                Projectile.velocity *= 0.985f; // 每帧速度减少
            }
            #region 旧代码
            //else if (Projectile.localAI[0] < 120f) // 前 80-120 帧执行旋转逻辑
            //{
            //    if (Projectile.localAI[0] % 3 == 0) // 每 3 帧旋转
            //    {
            //        Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(7));
            //    }
            //    Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 0.5f;
            //}
            #endregion
            else // 使用追踪逻辑
            {
                if (!Projectile.localAI[1].Equals(1f)) // 切换到追踪状态时释放特效
                {

                    // 检查是否启用了特效
                    if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
                    {
                        ReleaseLeafEffect();
                    }
                    Projectile.localAI[1] = 1f; // 标记追踪逻辑已启动
                }

                NPC target = Projectile.Center.ClosestNPCAt(5800);
                if (target != null)
                {
                    Vector2 direction = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * 10f, 0.08f);
                }
            }

            if (Projectile.penetrate < 200) // 如果弹幕已经击中敌人，停止追踪能力
            {
                if (Projectile.timeLeft > 60)
                {
                    Projectile.timeLeft = 60; // 弹幕开始缩小并减速
                }
                Projectile.velocity *= 0.88f;
            }

            if (Projectile.timeLeft <= 20) // 弹幕即将消失时停止造成伤害
            {
                ableToHit = false;
            }
            Time++;
        }


        private void ReleaseLeafEffect()
        {
            // 创建叶子形状的粒子特效
            for (int i = -1; i <= 1; i += 2) // 两条曲线，正方向和反方向
            {
                for (int j = 0; j < 20; j++) // 每条曲线 20 个粒子
                {
                    float progress = j / 20f; // 进度 0 到 1
                    float angle = progress * MathHelper.PiOver2 * i; // 曲线弯曲程度
                    Vector2 direction = Projectile.velocity.RotatedBy(angle).SafeNormalize(Vector2.UnitY) * (2f + progress * 5f); // 曲线方向和速度

                    Dust dust = Dust.NewDustPerfect(
                        Projectile.Center, // 粒子生成位置
                        DustID.Plantera_Green, // 157号粒子
                        direction, // 速度
                        100, // 透明度
                        Color.ForestGreen, // 深绿色
                        Main.rand.NextFloat(1.2f, 1.8f) // 粒子大小
                    );
                    dust.noGravity = true; // 粒子不受重力影响
                    dust.fadeIn = 0.1f; // 快速淡入效果
                }
            }
        }



        public void KillTheThing(NPC npc)
        {
            //Projectile.velocity = Projectile.SuperhomeTowardsTarget(npc, 50f / (Projectile.extraUpdates + 1), 60f / (Projectile.extraUpdates + 1), 1f / (Projectile.extraUpdates + 1));
        }
        public override bool PreDraw(ref Color lightColor)
        {

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                Texture2D lightTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/SmallGreyscaleCircle").Value;
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    float colorInterpolation = (float)Math.Cos(Projectile.timeLeft / 32f + Main.GlobalTimeWrappedHourly / 20f + i / (float)Projectile.oldPos.Length * MathHelper.Pi) * 0.5f + 0.5f;
                    Color color = Color.Lerp(Color.Green, Color.DarkGreen, colorInterpolation) * 0.4f; // 改为绿色和深绿色的渐变
                    color.A = 0;
                    Vector2 drawPosition = Projectile.oldPos[i] + lightTexture.Size() * 0.5f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(-28f, -28f);
                    Color outerColor = color;
                    Color innerColor = color * 0.5f;
                    float intensity = 0.9f + 0.15f * (float)Math.Cos(Main.GlobalTimeWrappedHourly % 60f * MathHelper.TwoPi);
                    intensity *= MathHelper.Lerp(0.15f, 1f, 1f - i / (float)Projectile.oldPos.Length);
                    if (Projectile.timeLeft <= 60) // 弹幕即将消失时缩小
                    {
                        intensity *= Projectile.timeLeft / 60f;
                    }
                    Vector2 outerScale = new Vector2(1f) * intensity;
                    Vector2 innerScale = new Vector2(1f) * intensity * 0.7f;
                    outerColor *= intensity;
                    innerColor *= intensity;
                    Main.EntitySpriteDraw(lightTexture, drawPosition, null, outerColor, 0f, lightTexture.Size() * 0.5f, outerScale * 0.6f, SpriteEffects.None, 0);
                    Main.EntitySpriteDraw(lightTexture, drawPosition, null, innerColor, 0f, lightTexture.Size() * 0.5f, innerScale * 0.6f, SpriteEffects.None, 0);
                }
                return false;
            }
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //target.AddBuff(ModContent.BuffType<MiracleBlight>(), 300); // 超位崩解
        }
    }
}
