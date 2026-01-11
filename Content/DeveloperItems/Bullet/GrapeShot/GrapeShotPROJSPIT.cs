namespace CalamityRangerExpansion.Content.DeveloperItems.Bullet.GrapeShot
{
    public class GrapeShotPROJSPIT : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.GrapeShot";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 画残影效果
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
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
            Projectile.penetrate = 3; // 穿透力为6，击中6个敌人后消失
            Projectile.timeLeft = 300; // 弹幕存在时间为300帧
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
            Projectile.ignoreWater = true; // 弹幕不受水影响
            Projectile.arrow = true;
            Projectile.extraUpdates = 1;
            Projectile.aiStyle = ProjAIStyleID.Arrow; // 受重力影响
        }

        public override void AI()
        {
            // 调整弹幕的旋转，使其在飞行时保持水平
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;

            // 添加光源，紫色光照
            Lighting.AddLight(Projectile.Center, Color.Purple.ToVector3() * 0.49f);
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 在路径上生成紫色 Dust 粒子特效
                if (Main.rand.NextBool(3))
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.position, DustID.PurpleTorch);
                    dust.noGravity = true;
                    dust.scale = 1.2f;
                    dust.velocity *= 0.3f;
                }
            }

        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // 随机改变反弹角度
            float randomAngle = MathHelper.ToRadians(Main.rand.Next(360));
            Vector2 newVelocity = oldVelocity.RotatedBy(randomAngle);
            Projectile.velocity = newVelocity;

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 在碰撞点生成几个紫色粒子特效
                for (int i = 0; i < 5; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.position, DustID.PurpleTorch);
                    dust.noGravity = true;
                    dust.scale = 1.5f;
                    dust.velocity = newVelocity * 0.5f;
                }
            }
            return false; // 反弹而不销毁
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 随机改变反弹角度
            float randomAngle = MathHelper.ToRadians(Main.rand.Next(360));
            Projectile.velocity = Projectile.velocity.RotatedBy(randomAngle);
        }

        public override void OnKill(int timeLeft)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 在消失时释放紫色粒子特效
                for (int i = 0; i < 10; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.position, DustID.PurpleTorch);
                    dust.noGravity = true;
                    dust.scale = 1.8f;
                    dust.velocity *= 1.2f;
                }
            }
        }




    }
}