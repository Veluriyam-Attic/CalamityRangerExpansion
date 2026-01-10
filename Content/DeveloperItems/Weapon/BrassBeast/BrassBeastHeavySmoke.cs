namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.BrassBeast
{
    public class BrassBeastHeavySmoke : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.BrassBeast";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj"; // 使用透明贴图

        public override void SetDefaults()
        {
            if (NPC.downedMoonlord)
            {
                if (Main.getGoodWorld)
                {
                    Projectile.width = Projectile.height = 1660;
                }
                Projectile.width = Projectile.height = 660;
            }
            else
            {
                Projectile.width = Projectile.height = 300;
            }                
            Projectile.friendly = true; // 友方投射物
            Projectile.ignoreWater = true; // 无视水
            Projectile.tileCollide = false; // 不与地形碰撞
            Projectile.DamageType = DamageClass.Ranged; // 远程伤害
            Projectile.penetrate = -1; // 无限穿透
            Projectile.MaxUpdates = 10; // 额外更新次数
            Projectile.timeLeft = 120; // 生命周期
            Projectile.usesIDStaticNPCImmunity = true; // 开启静态NPC击中冷却
            Projectile.idStaticNPCHitCooldown = 2; // 每x帧可击中一次相同NPC
        }
        private Vector2? fixedMouseDirection; // 存储固定的鼠标方向
        public override void AI()
        {
            Projectile.velocity *= 1f;

            // 第一次运行时记录鼠标方向
            if (fixedMouseDirection == null)
            {
                fixedMouseDirection = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitX); // 从弹幕指向鼠标的方向
            }

            // 喷射烟雾粒子
            for (int i = 0; i < Main.rand.Next(150, 171); i++)
            {
                float angle = MathHelper.ToRadians(-45 + Main.rand.NextFloat(90)); // 随机角度范围 -45 至 45
                Vector2 velocity = fixedMouseDirection.Value.RotatedBy(angle) * Main.rand.NextFloat(10f, 40f); // 高速粒子
                int dustType = Main.rand.NextBool() ? DustID.Torch : DustID.Smoke; // Torch 和 Smoke 随机选择

                // 创建粒子并随机化生命周期
                Dust dust = Dust.NewDustPerfect(
                    Projectile.Center,
                    dustType,
                    velocity,
                    100,
                    default,
                    Main.rand.NextFloat(3.5f, 6.0f) * Main.rand.NextFloat(0.8f, 1.35f) // scale 随机化
                );
                dust.noGravity = true;
            }


            // 实时更新鼠标位置的喷射逻辑虽然很帅，但是不太推荐
            //{
            //    // 喷射烟雾粒子
            //    Vector2 directionToMouse = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitX); // 从弹幕指向鼠标的方向
            //    for (int i = 0; i < Main.rand.Next(150, 171); i++)
            //    {
            //        float angle = MathHelper.ToRadians(-45 + Main.rand.NextFloat(90)); // 随机角度范围 -45 至 45
            //        Vector2 velocity = directionToMouse.RotatedBy(angle) * Main.rand.NextFloat(10f, 40f); // 高速粒子
            //        int dustType = Main.rand.NextBool() ? DustID.Torch : DustID.Smoke; // Torch 和 Smoke 随机选择
            //        Dust.NewDustPerfect(Projectile.Center, dustType, velocity, 100, default, Main.rand.NextFloat(3.5f, 6.0f)).noGravity = true;
            //    }
            //}
        }

        public override void OnSpawn(IEntitySource source)
        {
            // 屏幕震动效果
            float shakePower = 1.5f; // 设置震动强度
            float distanceFactor = Utils.GetLerpValue(1000f, 0f, Projectile.Distance(Main.LocalPlayer.Center), true); // 距离衰减
            Main.LocalPlayer.Calamity().GeneralScreenShakePower = Math.Max(Main.LocalPlayer.Calamity().GeneralScreenShakePower, shakePower * distanceFactor);



            base.OnSpawn(source);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 施加火焰Debuff
            target.AddBuff(BuffID.OnFire, 300); // 燃烧
            target.AddBuff(BuffID.CursedInferno, 300); // 诅咒地狱火
            if (NPC.downedMoonlord)
            {
                target.AddBuff(BuffID.Daybreak, 300); // 破晓
                target.AddBuff(ModContent.BuffType<ElementalMix>(), 300); // 元素紊乱
                target.AddBuff(ModContent.BuffType<HolyFlames>(), 300); // 神圣之火
                if(Main.getGoodWorld)
                {
                    target.AddBuff(ModContent.BuffType<Dragonfire>(), 300); // 巨龙之火
                    target.AddBuff(ModContent.BuffType<GodSlayerInferno> (), 300); // 弑神者之怒焰
                }
            }

            // 击中后释放火焰和烟雾粒子
            for (int i = 0; i < 30; i++)
            {
                float angle = MathHelper.ToRadians(-30 + Main.rand.NextFloat(60)); // 随机角度范围 -30 至 30
                Vector2 velocity = -Vector2.UnitY.RotatedBy(angle) * Main.rand.NextFloat(6f, 10f); // 高速粒子，向上
                Dust.NewDustPerfect(
                    target.Center,
                    DustID.Torch,
                    velocity,
                    100,
                    Color.OrangeRed,
                    Main.rand.NextFloat(2.5f, 3.5f)
                ).noGravity = true;
            }

            for (int i = 0; i < 30; i++)
            {
                float angle = MathHelper.ToRadians(-30 + Main.rand.NextFloat(60)); // 随机角度范围 -30 至 30
                Vector2 velocity = -Vector2.UnitY.RotatedBy(angle) * Main.rand.NextFloat(4f, 8f); // 高速粒子，向上
                Dust.NewDustPerfect(
                    target.Center,
                    DustID.Smoke,
                    velocity,
                    100,
                    Color.Gray,
                    Main.rand.NextFloat(2.5f, 3.5f)
                ).noGravity = true;
            }
        }

    }
}
