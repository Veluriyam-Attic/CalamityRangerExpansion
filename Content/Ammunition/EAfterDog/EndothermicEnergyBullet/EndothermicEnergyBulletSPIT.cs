namespace CalamityRangerExpansion.Content.Ammunition.EAfterDog.EndothermicEnergyBullet
{
    public class EndothermicEnergyBulletSPIT : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectile.EAfterDog";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
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
            Projectile.timeLeft = 300;
            Projectile.MaxUpdates = 6;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;
        }

        public override void AI()
        {
            // 由于我们是水平贴图，因此什么也不需要转动
            Projectile.rotation = Projectile.velocity.ToRotation();

            // 添加光效
            Lighting.AddLight(Projectile.Center, Color.Lerp(Color.Blue, Color.AliceBlue, 0.5f).ToVector3() * 0.49f);

            // 子弹在出现之后很短一段时间会变得可见
            if (Projectile.timeLeft == 296)
                Projectile.alpha = 0;

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                if (Projectile.timeLeft % 3 == 0)
                {
                    int particleCount = Main.rand.Next(1, 3); // 生成1到2个Dust粒子
                    for (int i = 0; i < particleCount; i++)
                    {
                        Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SnowflakeIce, -Projectile.velocity.X * 0.5f, -Projectile.velocity.Y * 0.5f, 150, default, 1.2f);
                        dust.noGravity = true; // 让粒子无重力效果
                        dust.scale *= 0.9f;    // 调整粒子大小
                    }
                }
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                int particleCount = Main.rand.Next(15, 25); // 随机生成15到25个粒子
                for (int i = 0; i < particleCount; i++)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SnowflakeIce, Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f), 150, default, 1.5f);
                    dust.noGravity = true; // 让粒子无重力效果
                    dust.scale *= 1.1f;    // 调整粒子大小
                }
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }
        public override void OnKill(int timeLeft)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                int particleCount = Main.rand.Next(15, 25); // 随机生成15到25个粒子
                for (int i = 0; i < particleCount; i++)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.SnowflakeIce, Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f), 150, default, 1.5f);
                    dust.noGravity = true; // 让粒子无重力效果
                    dust.scale *= 0.7f;    // 调整粒子大小
                }
            }
        }
    }
}
