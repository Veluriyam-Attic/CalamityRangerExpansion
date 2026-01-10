namespace CalamityRangerExpansion.Content.DeveloperItems.Arrow.ShadowsArrow
{
    internal class SAPortalPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.ShadowsArrow";
        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60; // 存在时间较短
            Projectile.tileCollide = false;
            Projectile.light = 0.75f; // 发光
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 15; // 无敌帧冷却时间为10帧
            Projectile.scale = 1.75f;
        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);

            // 生成 Metaball 粒子
            int particleCount = Main.rand.Next(10, 18); // 生成 X~Y 个粒子
            for (int i = 0; i < particleCount; i++)
            {
                // 生成粒子的位置：以传送门为中心，向四周随机偏移 0~30 像素
                Vector2 spawnPosition = Projectile.Center + Main.rand.NextVector2Circular(30f, 30f);

                // 粒子的初始速度：随机方向，速度在 1~4 之间
                Vector2 velocity = Main.rand.NextVector2Circular(1f, 4f);

                // 粒子的大小：24~48
                float size = Main.rand.NextFloat(24f, 48f);

                // 调用 Metaball 系统生成粒子
                ShadowAmmoMetaball.SpawnParticle(spawnPosition, velocity, size);
            }
        }

        public override void AI()
        {
            // 旋转效果
            Projectile.rotation += 0.1f;

            // 不断改变颜色
            // 手动实现 PingPong
            float PingPong(float value, float length)
            {
                return length - Math.Abs(value % (length * 2) - length);
            }

            // 使用时
            Color baseColor = Color.Lerp(Color.Cyan, Color.Magenta, PingPong(Main.GameUpdateCount * 0.1f, 1f));
            Lighting.AddLight(Projectile.Center, baseColor.ToVector3() * 0.75f);

            // 渐渐消失
            Projectile.alpha += 4;
            if (Projectile.alpha >= 255)
                Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // 设定黑褐色
            Color darkBrown = new Color(70, 40, 20);

            // 使用传送门的贴图
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Melee/StreamGougePortal").Value;
            Vector2 origin = texture.Size() * 0.5f;

            // 绘制时强行覆盖颜色
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, darkBrown * ((255 - Projectile.alpha) / 255f), Projectile.rotation, origin, 1f, SpriteEffects.None, 0);
            return false;
        }
    }
}