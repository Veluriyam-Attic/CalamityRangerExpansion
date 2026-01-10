namespace CalamityRangerExpansion.Content.Arrows.EAfterDog.AuricArrow
{
    internal class DropThing : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectile.CPreMoodLord";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // 画残影效果
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
        public override void SetDefaults()
        {
            // 设置弹幕的基础属性
            Projectile.width = 11; // 弹幕宽度
            Projectile.height = 24; // 弹幕高度
            Projectile.friendly = true; // 对敌人有效
            Projectile.DamageType = DamageClass.Ranged; // 远程伤害类型
            Projectile.penetrate = 1; // 穿透力为1，击中一个敌人就消失
            Projectile.timeLeft = 36000; // 弹幕存在时间为x帧
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
            Projectile.ignoreWater = true; // 弹幕不受水影响
            Projectile.arrow = true;
            Projectile.extraUpdates = 1;
        }
        private int phase = 1; // 当前阶段
        private int phaseTimer = 0; // 阶段计时器

        public override void AI()
        {
            Player targetPlayer = Main.player[Projectile.owner];

            switch (phase)
            {
                case 1: // 第一阶段：速度逐渐减慢
                    Projectile.velocity *= 0.975f;
                    phaseTimer++;
                    if (phaseTimer >= 40)
                    {
                        phase = 2;
                        phaseTimer = 0;
                    }
                    break;

                case 2: // 第二阶段：停留
                    Projectile.velocity = Vector2.Zero;
                    phaseTimer++;
                    if (phaseTimer >= 10)
                    {
                        phase = 3;
                        phaseTimer = 0;
                    }
                    break;

                case 3: // 第三阶段：追踪玩家
                    Vector2 direction = Vector2.Normalize(targetPlayer.Center - Projectile.Center);
                    float speed = 28f;
                    Projectile.velocity = direction * speed;

                    phaseTimer++;
                    if (phaseTimer >= 15)
                    {
                        phase = 4;
                        phaseTimer = 0;
                    }
                    break;

                case 4: // 第四阶段：永久停留
                    Projectile.velocity = Vector2.Zero;
                    break;
            }

            // 检测与玩家的碰撞
            if (Projectile.Hitbox.Intersects(targetPlayer.Hitbox))
            {
                // 玩家损失10点生命值
                targetPlayer.statLife -= 10;
                targetPlayer.HealEffect(-10, true); // 显示扣血效果

                // 给予玩家Buff，持续10秒
                targetPlayer.AddBuff(ModContent.BuffType<AuricArrowPBuff>(), 600);

                // 消除弹幕
                Projectile.Kill();
                return;
            }
        }
        #region 旧代码

        //public override void AI()
        //{
        //    Player targetPlayer = Main.player[Projectile.owner];

        //    // 计算向玩家移动的方向
        //    Vector2 direction = Vector2.Normalize(targetPlayer.Center - Projectile.Center);
        //    float speed = 28f; // 设置固定速度
        //    Projectile.velocity = direction * speed;

        //    // 检测与玩家的碰撞
        //    if (Projectile.Hitbox.Intersects(targetPlayer.Hitbox))
        //    {
        //        // 玩家损失10点生命值
        //        targetPlayer.statLife -= 10;
        //        targetPlayer.HealEffect(-10, true); // 显示扣血效果

        //        // 给予玩家Buff，持续10秒
        //        targetPlayer.AddBuff(ModContent.BuffType<AuricArrowPBuff>(), 600);

        //        // 消除弹幕
        //        Projectile.Kill();
        //        return;
        //    }
        //}

        #endregion
    }
}