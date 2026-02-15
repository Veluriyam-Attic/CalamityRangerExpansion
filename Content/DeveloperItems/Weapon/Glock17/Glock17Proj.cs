using System.Timers;

namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.Glock17
{
    internal class Glock17Proj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.Glock17";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
                return false;
            }
            return true;
        }
        // 子弹行为阶段（默认1：普通模式，2：困难，3：月后）
        public int Stage => NPC.downedMoonlord ? 3 : (Main.hardMode ? 2 : 1);

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 4;
            Projectile.timeLeft = 300;
            Projectile.MaxUpdates = 3;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
        }

        public override void AI()
        {
            // 由于我们是水平贴图，因此什么也不需要转动
            Projectile.rotation = Projectile.velocity.ToRotation();

            // 添加光效
            Lighting.AddLight(Projectile.Center, Color.Lerp(Color.Orange, Color.Red, 0.5f).ToVector3() * 0.55f);

            // 子弹在出现之后很短一段时间会变得可见
            if (Projectile.timeLeft == 296)
                Projectile.alpha = 0;
        }


        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // 混合银光喷射特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                int count = 8 + Stage * 3;
                for (int i = 0; i < count; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(1f, 3f + Stage);
                    Color c = Color.Lerp(Color.White, Color.Silver, Main.rand.NextFloat());
                    PointParticle spark = new PointParticle(Projectile.Center, velocity, false, 12, 0.9f, c);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }

            // 🔥 Stage 2+：附加 Glock17EDebuff
            if (Stage >= 2)
            {
                target.GetGlobalNPC<VeluriyamGlobalNPC>().Effects["YCRE:Glock17"] = B.Weapons.Glock17.EffectTimeSecond * 60;

                if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
                {
                    for (double i = 0; i < 2 * Math.PI; i += Math.PI / 60)
                    {
                        var d = Dust.NewDust(Projectile.position, 1, 1, DustID.PurificationPowder, 15 * (float)Math.Cos(i), 15 * (float)Math.Sin(i), 255, Color.Red);
                    }
                }
            }
            
            if(Stage >= 3)
            {
                modifiers.FinalDamage.Flat += B.Weapons.Glock17.StageThirdExtraDamage;
            }
        }
    }
}
