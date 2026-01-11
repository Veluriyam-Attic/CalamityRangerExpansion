namespace CalamityRangerExpansion.Content.Ammunition.APreHardMode.AerialiteBullet
{
    internal class AerialiteBulletPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectile.APreHardMode";
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
                    Color color = Color.Lerp(Color.LightYellow, Color.LightCyan, colorInterpolation) * 0.4f;
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
            Projectile.MaxUpdates = 6;
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
                if (Main.rand.NextBool(3)) // 1/3 概率生成粒子
                {
                    Dust dust = Dust.NewDustPerfect(
                        Projectile.Center,
                        Main.rand.NextBool() ? DustID.Skyware : DustID.Cloud,
                        -Projectile.velocity.RotatedByRandom(0.1f) * Main.rand.NextFloat(0.1f, 0.3f)
                    );
                    dust.noGravity = true; // 无重力
                    dust.scale = Main.rand.NextFloat(0.7f, 1.1f); // 随机大小
                }
            }
              

        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source); // 保留基类行为

            Player player = Main.player[Projectile.owner]; // 获取发射弹幕的玩家

            // 玩家速度的单位是像素/帧，而 1 mph 对应的速度约为 0.022352 像素/帧
            // 因此，65 mph 转换为像素/ 帧的速度上限为 65 * 0.022352 ≈ 1.45388 像素 / 帧

            // 定义速度上
            float speedLimit = 65f * 0.022352f * 5;

            // 检查玩家当前速度是否超过上限
            if (player.velocity.Length() > speedLimit)
            {
                return; // 如果超过上限，则不施加后坐力
            }

            // 给玩家一个与弹幕速度相反的力，力道为弹幕速度的15%
            Vector2 recoilForce = -Projectile.velocity * 0.15f;
            player.velocity += recoilForce;
        }


        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {

        }
        public override void OnKill(int timeLeft)
        {

        }
    }
}
