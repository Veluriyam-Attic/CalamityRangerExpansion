using CalamityMod;
using Microsoft.Xna.Framework;
using System;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.Particles;
using CalamityMod.Buffs.StatDebuffs;
using Terraria.Audio;
using CalamityRangerExpansion.CREConfigs;

namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.Pyroblast
{
    internal class PyroblastRocket : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.Pyroblast";

        // 是否启用特殊能力
        public static bool EnableSpecialAbility = false;

        private int phase = 1; // 阶段标识
        private int frameCounter = 0; // 帧计数器

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
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
            Projectile.timeLeft = 300; // 弹幕存在时间为x帧
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
            Projectile.ignoreWater = true; // 弹幕不受水影响
            Projectile.arrow = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;

            frameCounter++;

            // 在路径上生成 Lava 和 Smoke 粒子
            if (frameCounter % 2 == 0) // 每两帧触发一次
            {
                int particleCount = Main.rand.Next(4, 8); // 每次随机生成 4 到 7 个粒子
                for (int i = 0; i < particleCount; i++)
                {
                    // 随机选择 Lava 或 Smoke 粒子
                    int dustType = Main.rand.NextBool() ? DustID.Lava : DustID.Smoke;
                    Dust trailDust = Dust.NewDustPerfect(
                        Projectile.Center,
                        dustType,
                        Main.rand.NextVector2Circular(1f, 1f), // 粒子速度
                        150,
                        Color.WhiteSmoke,
                        Main.rand.NextFloat(1.1f, 1.5f) // 粒子大小
                    );
                    trailDust.noGravity = true; // 无重力效果
                }
            }

            // 第1阶段逻辑
            if (phase == 1)
            {
                Projectile.velocity *= 0.995f; // 每帧速度减缓

                if (frameCounter >= 40) // 进入下一阶段
                {
                    phase = 2;
                    frameCounter = 0;

                    // 创建橙色圆圈粒子特效
                    for (int i = 0; i < 3; i++)
                    {
                        Particle pulse = new DirectionalPulseRing(
                            Projectile.Center,
                            Projectile.velocity * 0.75f,
                            Color.Orange,
                            new Vector2(1f, 2.5f),
                            Projectile.rotation + MathHelper.PiOver2 + MathHelper.Pi,
                            0.2f,
                            0.03f,
                            20
                        );
                        GeneralParticleHandler.SpawnParticle(pulse);
                    }

                }
            }
            // 第2阶段逻辑
            else if (phase == 2 && EnableSpecialAbility)
            {
                NPC target = Projectile.Center.ClosestNPCAt(5000); // 寻找最近的敌人
                if (target != null)
                {
                    Vector2 direction = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                    float desiredRotation = direction.ToRotation(); // 目标方向的旋转角度
                    float currentRotation = Projectile.velocity.ToRotation(); // 当前弹幕速度的旋转角度
                    float rotationDifference = MathHelper.WrapAngle(desiredRotation - currentRotation); // 计算旋转角度差并封装到[-π, π]
                    float rotationAmount = MathHelper.ToRadians(Main.rand.Next(1, 10)); // 每次调整的角度为1~9度的随机值

                    // 限制旋转幅度
                    if (Math.Abs(rotationDifference) < rotationAmount)
                    {
                        rotationAmount = rotationDifference; // 如果差值小于随机值，则直接旋转到目标方向
                    }

                    // 调整速度方向
                    Projectile.velocity = Projectile.velocity.RotatedBy(rotationAmount);
                }
            }

            // 添加光效
            Lighting.AddLight(Projectile.Center, Color.OrangeRed.ToVector3() * 0.49f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 施加多个Debuff
            target.AddBuff(ModContent.BuffType<MarkedforDeath>(), 300); // 死亡标记
            target.AddBuff(BuffID.CursedInferno, 300); // 诅咒狱火
            target.AddBuff(BuffID.BetsysCurse, 300); // 双足翼龙之怒火
            target.AddBuff(BuffID.OnFire, 300); // 着火
            target.AddBuff(BuffID.OnFire3, 300); // 狱火
        }

        public override void OnKill(int timeLeft)
        {
            // 扩展性爆炸特效
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 64; // 扩大爆炸范围
            Projectile.position -= new Vector2(Projectile.width / 2, Projectile.height / 2);

            // 播放爆炸音效
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            // 生成浓烟粒子
            for (int i = 0; i < 30; i++)
            {
                Dust smoke = Dust.NewDustPerfect(Projectile.Center, DustID.Smoke, Main.rand.NextVector2Circular(3f, 3f), 150, Color.Black, Main.rand.NextFloat(2.5f, 4f));
                smoke.noGravity = true;
            }

            for (int i = 0; i < 30; i++)
            {
                Dust ash = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, Main.rand.NextVector2Circular(4f, 4f), 150, Color.DarkOrange, Main.rand.NextFloat(2f, 3.5f));
                ash.noGravity = true;
            }

            // 生成Gore效果
            if (Main.netMode != NetmodeID.Server)
            {
                for (int i = 0; i < 4; i++)
                {
                    Gore gore = Gore.NewGoreDirect(Projectile.GetSource_Death(), Projectile.Center, Main.rand.NextVector2Circular(3f, 3f), Main.rand.Next(61, 64));
                    gore.velocity *= 2f;
                }
            }

            // 消亡时释放额外弹幕
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<PyroblastRocketEXP>(),
                    (int)(Projectile.damage * 0.5f), Projectile.knockBack,
                    Projectile.owner
                );
            }
        }
    }
}
