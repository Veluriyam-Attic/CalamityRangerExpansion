namespace CalamityRangerExpansion.Content.Arrows.CPreMoodLord.ScoriaArrow
{
    public class ScoriaArrowFireball : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectile.CPreMoodLord";
        private bool hasLockedOn = false;  // 是否已经开始追踪
        private NPC target;  // 被追踪的目标

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4; // 保留4帧动画
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.aiStyle = 0; // 自定义AI
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true; // 无视水
        }

        public override void AI()
        {
            // 调整弹幕的旋转，使其在飞行时保持水平
            Projectile.rotation = Projectile.velocity.ToRotation();

            // 处理动画帧切换
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }

            // 如果没有锁定目标，则以固定速度飞行，并寻找敌人
            if (!hasLockedOn)
            {
                NPC closestNPC = FindClosestNPC(1600f); // 寻找1600像素范围内的敌人
                if (closestNPC != null && Projectile.ai[0] > 30f) // 在飞行超过30帧后开始追踪
                {
                    hasLockedOn = true;
                    target = closestNPC;
                }
            }

            // 如果已经锁定目标，则开始平滑追踪
            if (hasLockedOn && target != null && target.active)
            {
                // 使用类似PlagueTaintedDrone的平滑追踪机制
                Projectile.velocity = Projectile.SuperhomeTowardsTarget(target, 12f, 15f); // 平滑调整速度和方向
                Projectile.rotation = Projectile.velocity.ToRotation(); // 调整朝向
            }

            #region 旧代码

            //// 在飞行过程中留下橘黄色粒子
            //if (Main.rand.NextBool(3))
            //{
            //    Vector2 dustPosition = Projectile.Center;
            //    Dust dust = Dust.NewDustDirect(dustPosition, 1, 1, DustID.Torch, 0f, 0f, 150, default, 1.5f);
            //    dust.noGravity = true;
            //    dust.velocity *= 0.5f;
            //}

            // 增加计时器，逐步进入锁定阶段

            #endregion
            Projectile.ai[0]++;
        }

        // 寻找最近的敌人
        private NPC FindClosestNPC(float maxRange)
        {
            NPC closestNPC = null;
            float closestDistance = maxRange;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.CanBeChasedBy(Projectile))
                {
                    float distance = Vector2.Distance(Projectile.Center, npc.Center);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestNPC = npc;
                    }
                }
            }

            return closestNPC;
        }

        public override void OnKill(int timeLeft)
        {
            // 弹幕消失时生成火焰爆炸效果
            for (int k = 0; k < 10; k++)
            {
                int volcano = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.InfernoFork, 0f, 0f);
                Main.dust[volcano].noGravity = true;
                Main.dust[volcano].velocity *= 0f;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 120); // 命中目标时附加火焰Debuff
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, Main.DiscoG, 53, Projectile.alpha); // 自定义颜色效果
        }
    }
}
