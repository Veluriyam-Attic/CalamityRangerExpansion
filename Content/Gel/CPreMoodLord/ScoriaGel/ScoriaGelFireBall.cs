namespace CalamityRangerExpansion.Content.Gel.CPreMoodLord.ScoriaGel
{
    internal class ScoriaGelFireBall : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectile.CPreMoodLord";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public float Time
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override bool? CanDamage() => Time >= 12f; // 前 12 帧不造成伤害

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 300;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 0;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;
            Projectile.aiStyle = ProjAIStyleID.Bounce;
        }

        public override void AI()
        {
            // 增加时间计数器
            Time++;
            #region 旧代码
            // 弹幕保持直线运动并逐渐加速
            //Projectile.velocity *= 1.035f;

            //// 绘制轨迹粒子特效
            //for (int i = 0; i < 3; i++)
            //{
            //    Vector2 spawnPosition = Vector2.Lerp(Projectile.oldPosition, Projectile.position, i / 3f);
            //    Dust dust = Dust.NewDustPerfect(spawnPosition, DustID.CrimsonTorch);
            //    dust.color = Color.OrangeRed;
            //    dust.scale = 1.5f;
            //    dust.fadeIn = 1f;
            //    dust.noGravity = true;
            //}
            #endregion
        }

        public override void OnKill(int timeLeft)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 消失时生成一圈粒子特效
                int particleCount = 20;
                float angleIncrement = MathHelper.TwoPi / particleCount;

                for (int i = 0; i < particleCount; i++)
                {
                    Vector2 velocity = new Vector2(3f, 0f).RotatedBy(angleIncrement * i);
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.CrimsonTorch, velocity, 0, Color.OrangeRed);
                    dust.scale = 1.2f;
                    dust.noGravity = true;
                }
            }
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 计算反弹的方向，遵循入射角等于出射角的原则
            Vector2 reflectDirection = Vector2.Reflect(Projectile.velocity, Vector2.Normalize(target.Center - Projectile.Center));
            Projectile.velocity = reflectDirection;

            target.AddBuff(BuffID.OnFire3, 300); // 原版的狱炎效果
            target.AddBuff(BuffID.OnFire, 300); // 原版的着火效果
        }

        #region 旧代码
        //public override bool OnTileCollide(Vector2 oldVelocity)
        //{
        //    // 碰撞物块时反弹
        //    if (Projectile.velocity.X != oldVelocity.X) Projectile.velocity.X = -oldVelocity.X;
        //    if (Projectile.velocity.Y != oldVelocity.Y) Projectile.velocity.Y = -oldVelocity.Y;

        //    // 每次反弹减少一次穿透次数
        //    Projectile.penetrate--;

        //    return false;
        //}
        #endregion
    }
}