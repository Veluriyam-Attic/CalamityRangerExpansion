namespace CalamityRangerExpansion.Content.Arrows.CPreMoodLord.PerennialArrow
{
    public class PerennialArrowPROJ : ModProjectile, ILocalizedModType
    {
        public override string Texture => "CalamityRangerExpansion/Content/Arrows/CPreMoodLord/PerennialArrow/PerennialArrow";

        public new string LocalizationCategory => "Projectile.CPreMoodLord";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.arrow = true;
            Projectile.extraUpdates = 2;
            Projectile.aiStyle = ProjAIStyleID.Arrow; // 让弹幕受到重力影响
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
        }

        public override void AI()
        {
            // 调整弹幕的旋转，使其在飞行时保持水平
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 释放绿红色粒子特效，类似于EclipsesStealth的效果
                if (Main.rand.NextBool(7))
                {
                    // 随机生成左右偏移量
                    float sideOffset = Main.rand.NextFloat(-1f, 1f); // 随机左右偏移的距离

                    // 计算 trailPos，添加左右偏移
                    Vector2 trailPos = Projectile.Center + Vector2.UnitY.RotatedBy(Projectile.rotation) + Projectile.velocity.RotatedBy(MathHelper.PiOver2) * sideOffset;

                    float trailScale = Main.rand.NextFloat(0.8f, 1.2f);
                    Color trailColor = Main.rand.NextBool() ? Color.Green : Color.Lime;

                    // 创建粒子
                    Particle trail = new SparkParticle(trailPos, Projectile.velocity * 0.2f, false, 60, trailScale, trailColor);
                    GeneralParticleHandler.SpawnParticle(trail);
                }
            }

        }

        public override void OnKill(int timeLeft)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 生成绿色烟雾特效，类似CinderArrowProj的效果
                int Dusts = 15;
                float radians = MathHelper.TwoPi / Dusts;
                Vector2 spinningPoint = Vector2.Normalize(new Vector2(-1f, -1f));
                for (int i = 0; i < Dusts; i++)
                {
                    Vector2 dustVelocity = spinningPoint.RotatedBy(radians * i).RotatedBy(0.5f) * 6.5f;
                    Particle smoke = new HeavySmokeParticle(Projectile.Center, dustVelocity * Main.rand.NextFloat(1f, 2.6f), Color.Green, 18, Main.rand.NextFloat(0.9f, 1.6f), 0.35f, Main.rand.NextFloat(-1, 1), true);
                    GeneralParticleHandler.SpawnParticle(smoke);
                }
            }
 

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 检查当前世界中是否已经存在至少一个 PerennialArrowFlower
            bool hasExistingFlowerInWorld = Main.projectile.Any(proj => proj.active && proj.type == ModContent.ProjectileType<PerennialArrowFlower>());

            // 如果当前世界中不存在 PerennialArrowFlower，则生成新的
            if (!hasExistingFlowerInWorld)
            {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    target.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<PerennialArrowFlower>(),
                    (int)(damageDone * 0.15f),
                    Projectile.knockBack,
                    Projectile.owner,
                    target.whoAmI
                );
            }
        }



        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // 确保弹幕可以黏附并且在敌人身上触发效果
            //Projectile.ModifyHitNPCSticky(1);

            // 为敌人添加10秒的增伤Buff
            target.AddBuff(ModContent.BuffType<PerennialArrowEBuff>(), 600);

            // 为玩家回复生命
            //Main.player[Projectile.owner].statLife += 2;
            //Main.player[Projectile.owner].HealEffect(2);

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


    }
}