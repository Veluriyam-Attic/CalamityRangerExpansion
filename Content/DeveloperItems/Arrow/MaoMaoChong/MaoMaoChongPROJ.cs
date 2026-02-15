namespace CalamityRangerExpansion.Content.DeveloperItems.Arrow.MaoMaoChong
{
    public class MaoMaoChongPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.MaoMaoChong";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 36; // 设置较长的拖尾长度
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // 模式2拖尾
            //Main.projFrames[Projectile.type] = 2;
        }
        internal float WidthFunction(float completionRatio, Vector2 vexpos) => (1f - completionRatio) * Projectile.scale * 9f;

        internal Color ColorFunction(float completionRatio, Vector2 vexpos)
        {
            float hue = 0.5f + 0.5f * completionRatio * MathF.Sin(Main.GlobalTimeWrappedHourly * 5f);
            Color trailColor = Main.hslToRgb(hue, 1f, 0.8f);
            return trailColor * Projectile.Opacity;
        }

        public override void PostDraw(Color lightColor)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 使用与 MeowCreature 相同的拖尾渲染逻辑
                PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(WidthFunction, ColorFunction, (_, _) => Projectile.Size * 0.5f), 30);

                // 绘制弹幕本体的发光效果
                Texture2D glow = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
                Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, glow.Size() * 0.5f, Projectile.scale, SpriteEffects.None);
            }
            Texture2D glow2 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Main.EntitySpriteDraw(glow2, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, glow2.Size() * 0.5f, Projectile.scale, SpriteEffects.None);
        }

        // 可以去参考一下原版的502号弹幕逻辑（在弹幕文件里面）
        public override bool PreDraw(ref Color lightColor)
        {

            return false; // 不使用默认的绘制逻辑
        }


        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 3; // 允许x次伤害
            Projectile.timeLeft = 600;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true; // 允许与方块碰撞
            Projectile.extraUpdates = 1; // 额外更新次数
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
            Projectile.aiStyle = ProjAIStyleID.Arrow; // 让弹幕受到重力影响
            Projectile.arrow = true;
        }

        public override void AI()
        {
            // 保持弹幕旋转（对于倾斜走向的弹幕而言）
            Projectile.rotation = Projectile.velocity.ToRotation();

            // Lighting - 将光源颜色更改为偏黑的深蓝色，光照强度为 0.55
            Lighting.AddLight(Projectile.Center, new Vector3(0.1f, 0.1f, 0.5f) * 0.55f);


        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 计算反弹的方向，遵循入射角等于出射角的原则
            Vector2 reflectDirection = Vector2.Reflect(Projectile.velocity, Vector2.Normalize(target.Center - Projectile.Center));
            Projectile.velocity = reflectDirection;

            // 播放随机音效（Item57 或 Item58）
            SoundStyle[] sounds = { SoundID.Item57, SoundID.Item58 };
            SoundEngine.PlaySound(sounds[Main.rand.Next(sounds.Length)], Projectile.position);

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 生成彩虹电能粒子特效
                GenerateRainbowElectricParticles(Projectile.Center, 15);
            }
        }


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // 碰撞物块时反弹
            if (Projectile.velocity.X != oldVelocity.X) Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y) Projectile.velocity.Y = -oldVelocity.Y;

            // 播放喵喵叫音效
            // 播放随机音效（Item57 或 Item58）
            SoundStyle[] sounds = { SoundID.Item57, SoundID.Item58 };
            SoundEngine.PlaySound(sounds[Main.rand.Next(sounds.Length)], Projectile.position);

            // 每次反弹减少一次穿透次数
            Projectile.penetrate--;

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 生成彩虹电能粒子特效
                GenerateRainbowElectricParticles(Projectile.Center, 5);
            }


            return false;
        }


        private void GenerateRainbowElectricParticles(Vector2 position, int particleCount)
        {
            // 彩虹颜色数组，包含 7 种颜色
            Color[] rainbowColors = new Color[]
            {
                Color.Red,
                Color.Orange,
                Color.Yellow,
                Color.Green,
                Color.Blue,
                Color.Indigo,
                Color.Violet
            };

            for (int i = 0; i < particleCount; i++)
            {
                float angle = Main.rand.NextFloat(0, MathHelper.TwoPi); // 随机角度
                float distance = Main.rand.NextFloat(5f, 50f); // 控制粒子生成的随机偏移范围
                Vector2 spawnPosition = position + distance * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                Vector2 velocity = Vector2.Normalize(spawnPosition - position) * Main.rand.NextFloat(1f, 3f); // 随机速度

                // 随机选择一个彩虹颜色
                Color randomRainbowColor = rainbowColors[Main.rand.Next(rainbowColors.Length)];

                // 创建电能粒子特效并染色为随机彩虹颜色
                Dust dust = Dust.NewDustPerfect(spawnPosition, DustID.Electric, velocity, 150, randomRainbowColor, Main.rand.NextFloat(1f, 2f));
                dust.noGravity = true; // 使粒子不受重力影响
                dust.fadeIn = 1f;
            }
        }

    }
}