namespace CalamityRangerExpansion.Content.Arrows.EAfterDog.EndothermicEnergyArrow
{
    public class EndothermicEnergyArrowPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectile.EAfterDog";
        public override string Texture => "CalamityRangerExpansion/Content/Arrows/EAfterDog/EndothermicEnergyArrow/EndothermicEnergyArrow";


        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
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
            Projectile.penetrate = 1; // 穿透力为1，击中一个敌人就消失
            Projectile.timeLeft = 600; // 弹幕存在时间为600帧
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
            Projectile.ignoreWater = true; // 弹幕不受水影响
            Projectile.arrow = true;
            Projectile.extraUpdates = 2;
            Projectile.velocity *= 0.6f;
            //Projectile.aiStyle = ProjAIStyleID.Arrow; // 让弹幕受到重力影响
        }

        private bool isAscending = false; // 标记弹幕是否处于向上飞行阶段
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 变成大冰锥并向上飞行
            Projectile.width = 20;
            Projectile.height = 40;
            Projectile.velocity = new Vector2(0, -8f); // 向上飞行
            Projectile.timeLeft = 60; // 设定寿命为短暂时间内销毁自己
            Projectile.penetrate = -1; // 不再穿透

            // 禁用原始的碰撞逻辑
            Projectile.tileCollide = false;

            // 设置为向上飞行阶段
            isAscending = true;
        }
        public override bool? CanDamage()
        {
            // 如果弹幕处于向上飞行阶段，则不造成伤害
            return !isAscending;
        }
        public override void AI()
        {
            // 调整弹幕的旋转，使其在飞行时保持水平
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;

            // Lighting - 添加天蓝色光源，光照强度为 0.49
            Lighting.AddLight(Projectile.Center, Color.LightSkyBlue.ToVector3() * 0.49f);


            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                SparkParticle Visual = new SparkParticle(Projectile.Center, Projectile.velocity * 0.1f, false, 2, 1.2f, Color.SkyBlue);
                GeneralParticleHandler.SpawnParticle(Visual);
                if (Projectile.localAI[0] % 3 == 0)
                {
                    LineParticle subTrail = new LineParticle(Projectile.Center, Projectile.velocity * 0.01f, false, 4, 1.1f, Color.SkyBlue);
                    GeneralParticleHandler.SpawnParticle(subTrail);
                }
            }

            // 检查是否即将销毁
            if (Projectile.timeLeft <= 10)
            {

                // 动态调整发射数量
                int arrowCount = Main.getGoodWorld ? 9 : 5; // 根据条件调整发射数量
                Vector2 baseDirection = Vector2.UnitY; // 绝对正下方方向

                // 动态调整每两个弹幕之间的夹角
                float angleStep = MathHelper.ToRadians(3); // 每两发之间的固定角度（原先是3度）

                // 动态生成弹幕
                for (int i = -(arrowCount / 2); i <= arrowCount / 2; i++) // 从负到正，确保发射对称
                {
                    float offsetAngle = i * angleStep; // 根据索引计算偏移角度
                    Vector2 direction = baseDirection.RotatedBy(offsetAngle); // 相对绝对正下方生成新的方向
                    direction *= 8f; // 设定飞行速度

                    // 生成新的弹幕
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, direction, ModContent.ProjectileType<EndothermicEnergyArrowSPLIT>(), (int)(Projectile.damage * 0.35f), Projectile.knockBack, Projectile.owner);
                }
                // 销毁自身
                Projectile.Kill();
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item30, Projectile.position);


            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                int Dusts = 10;
                float radians = MathHelper.TwoPi / Dusts;
                Vector2 spinningPoint = Vector2.Normalize(new Vector2(-1f, -1f));
                for (int i = 0; i < Dusts; i++)
                {
                    Vector2 dustVelocity = spinningPoint.RotatedBy(radians * i).RotatedBy(0.5f) * 6.5f;
                    Particle smoke = new HeavySmokeParticle(Projectile.Center, dustVelocity * Main.rand.NextFloat(1f, 2.6f), Color.WhiteSmoke, 18, Main.rand.NextFloat(0.9f, 1.6f), 0.35f, Main.rand.NextFloat(-1, 1), true);
                    GeneralParticleHandler.SpawnParticle(smoke);
                }
            }
        }
    }
}