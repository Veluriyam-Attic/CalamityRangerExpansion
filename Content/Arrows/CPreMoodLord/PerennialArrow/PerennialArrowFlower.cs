namespace CalamityRangerExpansion.Content.Arrows.CPreMoodLord.PerennialArrow
{
    public class PerennialArrowFlower : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectile.CPreMoodLord";
        private bool stuck = false; // 标志弹幕是否已经粘附在目标身上
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 1;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
                return false;
            }
            return true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.timeLeft = 300; // 5秒
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
        }

        public override void AI()
        {
            // 定义粒子的 ID，可以根据需要调整
            int[] dustTypes = { DustID.Grass, DustID.PinkTorch, DustID.GreenFairy, DustID.Plantera_Green };

            // 生成随机数量的粒子
            int numberOfParticles = Main.rand.Next(1, 4); // 随机生成 1 到 3 个粒子
            for (int i = 0; i < numberOfParticles; i++)
            {
                // 随机化位置偏移
                Vector2 offset = new Vector2(Main.rand.NextFloat(-16f, 16f), Main.rand.NextFloat(-16f, 16f));

                // 生成粒子
                int dustIndex = Dust.NewDust(Projectile.Center + offset, 0, 0, dustTypes[Main.rand.Next(dustTypes.Length)]);
                Dust dust = Main.dust[dustIndex];
                dust.noGravity = true; // 无重力效果
                dust.scale = Main.rand.NextFloat(1f, 2f); // 随机化粒子大小
                dust.velocity = new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f)); // 随机化速度
                dust.fadeIn = Main.rand.NextFloat(0.5f, 1.5f); // 淡入效果
            }

            #region 旧代码

            //// 获取目标 NPC
            //NPC target = Main.npc[(int)Projectile.ai[0]];

            //if (target.active)
            //{
            //    // 如果弹幕还未粘附
            //    if (!stuck)
            //    {
            //        // 快速追踪目标
            //        float speed = 15f; // 设置一个较快的追踪速度
            //        Vector2 direction = target.Center - Projectile.Center;
            //        direction.Normalize();
            //        Projectile.velocity = direction * speed;

            //        // 如果与目标距离足够近，开始粘附
            //        if (Projectile.Distance(target.Center) < 30f)
            //        {
            //            // 开启粘附逻辑，并设置速度为零
            //            stuck = true;
            //            Projectile.velocity = Vector2.Zero; // 停止运动
            //            Projectile.StickyProjAI(10); // 粘附在目标上
            //        }
            //    }
            //    else
            //    {
            //        // 如果已经粘附，则跟随目标
            //        Projectile.Center = target.Center;
            //    }
            //}
            //else
            //{
            //    Projectile.Kill(); // 如果目标消失，弹幕也消失
            //}

            #endregion

            //Sticky Behaviour
            Projectile.StickyProjAI(15);
            if (Projectile.ai[0] == 2f)
            {
                Projectile.velocity *= 0f;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // 调用原始的ModifyHitNPCSticky方法，确保粘附逻辑正常
            Projectile.ModifyHitNPCSticky(20);            
        }



        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.ai[0] = 2f;
            Projectile.timeLeft = 300;
            return false;
        }


        public override void OnKill(int timeLeft)
        {
            // 播放击杀音效
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            // 恢复玩家生命值
            Main.player[Projectile.owner].statLife += 30;
            Main.player[Projectile.owner].HealEffect(30);
        }
    }
}