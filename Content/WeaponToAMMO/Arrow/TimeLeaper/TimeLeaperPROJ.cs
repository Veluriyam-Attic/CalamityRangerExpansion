namespace CalamityRangerExpansion.Content.WeaponToAMMO.Arrow.TimeLeaper
{
    public class TimeLeaperPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "WeaponToAMMO.Arrow.TimeLeaper";
        private int hitCounter = 0; // 计数器，记录击中敌人的次数
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
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
            Projectile.penetrate = -1; // 穿透力为1，击中一个敌人就消失
            Projectile.timeLeft = 150; // 弹幕存在时间为150帧
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
            Projectile.ignoreWater = true; // 弹幕不受水影响
            Projectile.arrow = true;
            Projectile.extraUpdates = 1;
            Projectile.arrow = true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 每次击中敌人时增加计数器
            hitCounter++;
        }

        public override void AI()
        {
            // 调整弹幕的旋转，使其在飞行时保持水平
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;

            // Lighting - 添加土黄色/卡其色光源，光照强度为 0.55
            Lighting.AddLight(Projectile.Center, new Color(189, 183, 107).ToVector3() * 0.55f);

            // 只在弹幕生成的第一次运行时进行定位
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f; // 标记为已初始化

                // 将弹幕传送到鼠标位置
                Projectile.Center = Main.MouseWorld;

                // 使弹幕头部朝向玩家自身
                Vector2 vectorToPlayer = Main.player[Projectile.owner].Center - Projectile.Center;
                Projectile.rotation = vectorToPlayer.ToRotation();
                Projectile.velocity = vectorToPlayer.SafeNormalize(Vector2.Zero) * Projectile.velocity.Length();

            }

            // 追踪玩家位置的逻辑
            Player player = Main.player[Projectile.owner];
            //Vector2 directionToPlayer = Vector2.Normalize(player.Center - Projectile.Center) * 0.5f; // 控制追踪速度，0.5f 可根据需求调整
            //Projectile.velocity = (Projectile.velocity * 10f + directionToPlayer) / 11f; // 平滑过渡以保持追踪效果


            // 计算朝向玩家的单位向量，并使用固定速度
            float speed = 28f; // 设置固定速度，可以根据需求调整
            Vector2 directionToPlayer = Vector2.Normalize(player.Center - Projectile.Center) * speed;
            Projectile.velocity = directionToPlayer;



            // 当弹幕与玩家重叠时销毁并执行回血/扣血逻辑
            if (Projectile.Hitbox.Intersects(player.Hitbox))
            {
                if (hitCounter == 0)
                {
                    // 如果没有击中任何敌人，给玩家加 1 点血
                    player.statLife += 1;
                    player.HealEffect(1);
                }
                else
                {
                    // 计算扣除或增加的血量
                    //int healthChange = -hitCounter / 2; // 每两个命中扣 1 点血，如果命中 1 次，则加 1 点
                    int healthChange = -hitCounter * 3; // 每命中 1 次扣 3 点血
                    //if (hitCounter % 2 == 1) // 奇数次命中时，增加 1 点血
                    //    healthChange++;

                    player.statLife += healthChange;
                    if (healthChange > 0)
                        player.HealEffect(healthChange);
                    else if (healthChange < 0)
                        player.Hurt(Terraria.DataStructures.PlayerDeathReason.ByCustomReason($"{player.name}，搞不明白祖父悖论"), -healthChange, 0);
                }

                Projectile.Kill(); // 销毁弹幕
                return;
            }

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 定期生成粒子效果（基于 TimeBoltKnife 的逻辑）
                if (Projectile.localAI[0] > 7f)
                {
                    int dustType = Utils.SelectRandom(Main.rand, new int[]
                    {
                226, // 粒子类型 1
                229  // 粒子类型 2
                    });
                    Vector2 center = Projectile.Center;
                    Vector2 offset = new Vector2(-4f, 4f).RotatedBy(Projectile.rotation); // 生成的粒子偏移
                    int dust = Dust.NewDust(center + offset + Vector2.One * -4f, 8, 8, dustType, 0f, 0f, 100, default, 1f);
                    Dust dustInstance = Main.dust[dust];
                    dustInstance.velocity *= 0.1f;
                    if (!Main.rand.NextBool(6))
                        dustInstance.noGravity = true;
                }

                // 刚生成时释放几个浅蓝色的小圆圈往外扩散
                if (Projectile.ai[0] == 0f)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        Particle pulse = new DirectionalPulseRing(Projectile.Center, Projectile.velocity * 0.75f, Color.AliceBlue, new Vector2(1f, 2.5f), Projectile.rotation, 0.2f, 0.03f, 20);
                        GeneralParticleHandler.SpawnParticle(pulse);
                    }

                    for (int i = 0; i <= 4; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.BlueFairy, Projectile.velocity);
                        dust.scale = Main.rand.NextFloat(1.6f, 2.5f);
                        dust.velocity = Projectile.velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.3f, 1.6f);
                        dust.noGravity = true;
                    }
                }
            }

            Projectile.localAI[0] += 1f; // 增加 localAI[0] 以确保定期生成效果
            Projectile.ai[0]++;
        }


        public override void OnKill(int timeLeft)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 释放一个浅绿色的冲击波
                Particle pulse = new DirectionalPulseRing(Projectile.Center, Vector2.Zero, Color.LightGreen, new Vector2(1.5f), Projectile.rotation, 1f, 0.1f, 30);
                GeneralParticleHandler.SpawnParticle(pulse);
            }
        }



    }
}