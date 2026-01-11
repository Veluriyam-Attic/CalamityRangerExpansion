namespace CalamityRangerExpansion.Content.DeveloperItems.Arrow.ShadowsArrow
{
    public class ShadowsArrowPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.ShadowsArrow";
        public override string Texture => "CalamityRangerExpansion/Content/DeveloperItems/Arrow/ShadowsArrow/ShadowsArrow";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }


        // 严 选 弹 幕 列 表   确 保 攻 守 兼 备
        private static readonly int[] VanillaProjectiles = new int[]
        {
            1, 4, 5, 41, 82, 91, 103, 117, 120, 172, 225, 282, 357, 469, 474, 485, 495, 631, 639, 932, 1006
        };

        private static readonly int[] CalamityProjectiles = new int[]
        {
            ModContent.ProjectileType<BarinadeArrow>(),
            ModContent.ProjectileType<Shell>(),
            ModContent.ProjectileType<ToxicArrow>(),
            ModContent.ProjectileType<FeatherLarge>(),
            ModContent.ProjectileType<SlimeStream>(),
            ModContent.ProjectileType<LunarBolt>(),

            ModContent.ProjectileType<FlareBat>(),
            ModContent.ProjectileType<BrimstoneBolt>(),
            ModContent.ProjectileType<BoltArrow>(),
            ModContent.ProjectileType<CorrodedShell>(),
            ModContent.ProjectileType<MistArrow>(),
            ModContent.ProjectileType<LeafArrow>(),
            ModContent.ProjectileType<SporeBomb>(),
            ModContent.ProjectileType<VernalBolt>(),
            ModContent.ProjectileType<BallistaGreatArrow>(),
            ModContent.ProjectileType<PlagueArrow>(),

            ModContent.ProjectileType<PlanetaryAnnihilationProj>(),
            ModContent.ProjectileType<AstrealArrow>(),
            ModContent.ProjectileType<PrecisionBolt>(),
            ModContent.ProjectileType<TelluricGlareArrow>(),
            ModContent.ProjectileType<Bolt>(),
            ModContent.ProjectileType<TheMaelstromShark>(),
            ModContent.ProjectileType<TyphoonArrow>(),
            ModContent.ProjectileType<MiniSharkron>(),
            ModContent.ProjectileType<DWArrow>(),
            ModContent.ProjectileType<UltimaBolt>(),
            ModContent.ProjectileType<UltimaRay>(),
            ModContent.ProjectileType<UltimaSpark>(),
            ModContent.ProjectileType<DrataliornusFlame>(),
            ModContent.ProjectileType<DrataliornusExoArrow>(),
            ModContent.ProjectileType<DrataliornusExoArrow>(),
            ModContent.ProjectileType<SkyFlareFriendly>(),
            ModContent.ProjectileType<ExoCrystalArrow>(),
            ModContent.ProjectileType<CondemnationArrow>(),
            ModContent.ProjectileType<CondemnationArrowHoming>(),
            ModContent.ProjectileType<ContagionArrow>(),

            ModContent.ProjectileType<CinderArrowProj>(),
            ModContent.ProjectileType<VeriumBoltProj>(),
            ModContent.ProjectileType<SproutingArrowMain>(),
            ModContent.ProjectileType<IcicleArrowProj>(),
            ModContent.ProjectileType<ElysianArrowProj>(),
            ModContent.ProjectileType<BloodfireArrowProj>(),
            ModContent.ProjectileType<VanquisherArrowProj>()
        };
        private static readonly string[] CustomModProjectiles = new string[]
        {
            "AerialiteArrowPROJ","BloodBeadsArrowPROJ","PrismArrowPROJ","WulfrimArrowPROJ",            
            "StarblightSootArrowPROJ",
            "AstralArrowPROJ","LifeAlloyArrowPROJ","PerennialArrowPROJ","PlagueArrowPROJ","ScoriaArrowPROJ",
            "DivineGeodeArrowPROJ","EffulgentFeatherArrowPROJ","PolterplasmArrowPROJ","ToothArrowPROJ","UelibloomArrowPROJ",
            "AuricArrowPROJ", "EndothermicEnergyArrowPROJ", "MiracleMatterArrowPROJ"
        };
        private bool hasTeleported = false; // 是否已经进行过初始传送

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 4; // 穿4
            Projectile.timeLeft = 250;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 1; // 增加更新次数
            Projectile.arrow = true;
            Projectile.alpha = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            // 调整弹幕的旋转，使其在飞行时保持朝向
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;

            if (Projectile.ai[0] >= 15 && !hasTeleported)
            {
                hasTeleported = true;
                TeleportToTarget();
            }

            {
                // 计算粒子位置，使其沿着弹幕轨迹左右各偏移一定距离，形成两条线
                Vector2 offsetRight = Projectile.velocity.RotatedBy(MathHelper.PiOver2) * 0.45f; // 右侧偏移
                Vector2 offsetLeft = -offsetRight; // 左侧偏移

                // 计算粒子的运动动量，使其具有流动性
                Vector2 dustVelocity = -Projectile.velocity * 0.3f; // 让粒子有一些反向拖尾感
                dustVelocity += Main.rand.NextVector2Circular(1f, 1f); // 让粒子有轻微的随机抖动

                // 生成右侧粒子
                Dust dustRight = Dust.NewDustPerfect(Projectile.Center + offsetRight, DustID.Smoke, dustVelocity, 0, Color.Black, Main.rand.NextFloat(0.9f, 1.6f));
                dustRight.noGravity = true;
                dustRight.fadeIn = 0.5f; // 让粒子有渐变消失效果

                // 生成左侧粒子
                Dust dustLeft = Dust.NewDustPerfect(Projectile.Center + offsetLeft, DustID.Smoke, dustVelocity, 0, Color.Black, Main.rand.NextFloat(0.9f, 1.6f));
                dustLeft.noGravity = true;
                dustLeft.fadeIn = 0.5f; // 让粒子有渐变消失效果
            }           

            // 计数器增加
            Projectile.ai[0]++;
        }

        /// <summary>
        /// 传送逻辑：如果找到目标，则围绕目标传送；否则随机传送
        /// </summary>
        private void TeleportToTarget()
        {
            // 在原地生成传送门
            CreatePortal(Projectile.Center);

            NPC target = Projectile.Center.ClosestNPCAt(2500f);
            Vector2 newPosition;

            if (target != null)
            {
                // 以目标为圆心，在半径 8×16 的圆周随机传送
                float angle = Main.rand.NextFloat(0, MathHelper.TwoPi);
                newPosition = target.Center + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 12 * 16;
            }
            else
            {
                // 如果没有找到敌人，则以玩家为圆心，在半径 8×16 的圆周随机传送
                Player player = Main.player[Projectile.owner];
                float angle = Main.rand.NextFloat(0, MathHelper.TwoPi);
                newPosition = player.Center + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 20 * 16;
            }

            // 传送弹幕
            Projectile.position = newPosition;

            // 重新调整方向指向目标
            if (target != null)
            {
                Projectile.velocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * Projectile.velocity.Length();
            }

            // 传送后在新位置生成传送门
            CreatePortal(Projectile.Center);

            // 释放额外的随机弹幕
            SpawnRandomProjectile();
        }

        /// <summary>
        /// 命中敌人时触发传送，最多传送4次
        /// </summary>
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            TeleportToTarget();            
        }

        /// <summary>
        /// 生成 SAPortal 传送门
        /// </summary>
        private void CreatePortal(Vector2 position)
        {
            int portal = Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, Vector2.Zero, ModContent.ProjectileType<SAPortalPROJ>(), (int)(Projectile.damage * 0.5f), 0, Projectile.owner);
            Main.projectile[portal].timeLeft = 60; // 让传送门存在1秒
        }

        /// <summary>
        /// 释放一个伤害倍率为1.0的随机弹幕，方向和速度与原弹幕一致
        /// </summary>
        private void SpawnRandomProjectile()
        {
            // 选择一个随机容器 (0 = 原版, 1 = 灾厄, 2 = 自定义模组)
            int containerType = Main.rand.Next(3);
            int selectedProjectile;

            if (containerType == 0)
            {
                // 原版弹幕
                selectedProjectile = VanillaProjectiles[Main.rand.Next(VanillaProjectiles.Length)];
            }
            else if (containerType == 1)
            {
                // 灾厄模组弹幕
                selectedProjectile = CalamityProjectiles[Main.rand.Next(CalamityProjectiles.Length)];
            }
            else
            {
                // 自定义模组弹幕
                string selectedProjectileName = CustomModProjectiles[Main.rand.Next(CustomModProjectiles.Length)];
                selectedProjectile = Mod.Find<ModProjectile>(selectedProjectileName).Type;
            }

            // 生成随机弹幕，方向和速度与原弹幕一致
            // 生成随机弹幕
            int spawnedProjectile = Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                Projectile.Center,
                Projectile.velocity,
                selectedProjectile,
                Projectile.damage,
                Projectile.knockBack,
                Projectile.owner
            );

            // 确保弹幕属性正确
            if (spawnedProjectile != Main.maxProjectiles) // 确保弹幕生成成功
            {
                Projectile proj = Main.projectile[spawnedProjectile];
                proj.friendly = true; // 确保不会误伤玩家
                proj.hostile = false; // 不是敌对弹幕
                proj.penetrate = 1; // 只穿透1次，避免卡顿
                proj.localNPCHitCooldown = 60; // 确保不会造成过度伤害
                proj.usesLocalNPCImmunity = true; // 让每个敌人有单独的无敌帧
            }
        }


    }
}