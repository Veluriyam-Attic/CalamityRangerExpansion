using CalamityMod.Particles;
using CalamityMod;
using CalamityRangerExpansion.Content.BOWChange.DPreDog.NettlevineGreatbowC;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CalamityMod.CalamityUtils;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace CalamityRangerExpansion.Content.BOWChange.DPreDog.ArterialAssaultC
{
    internal class ArterialAssaultReRightPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.DPreDog";

        public override string Texture => "CalamityRangerExpansion/Content/BOWChange/DPreDog/ArterialAssaultC/ArterialAssaultRe";

        // === 自定义字段 ===
        private Player Owner => Main.player[Projectile.owner];
        private int Time;
        private bool Launched;
        private NPC LatchedTarget;
        private const int ChargeupTime = 30;
        private const int Lifetime = 300;
        private int ShootTimer;

        // 手臂动画（拉弓动作）
        private CurveSegment pullback = new(EasingType.PolyOut, 0f, 0f, -MathHelper.PiOver4 * 1.1f, 2);
        private CurveSegment throwout = new(EasingType.PolyOut, 0.5f, -MathHelper.PiOver4 * 1.1f, MathHelper.PiOver4 * 1.3f + MathHelper.PiOver2, 3);
        private float ChargeProgress => 1f - (Projectile.timeLeft - Lifetime) / (float)ChargeupTime;
        private float ArmAnim() => PiecewiseAnimation(ChargeProgress, [pullback, throwout]);


        public override void SetStaticDefaults()
        {
            // 设置弹幕拖尾长度和模式
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // 绘制控制函数，可用于绘制自定义贴图、添加发光效果、叠加特效等
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 3; // 可击中次数
            Projectile.timeLeft = ChargeupTime + Lifetime;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false; // 初期不碰撞
            Projectile.extraUpdates = 1; // 可调节飞行平滑度
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = 0f;
        }

        public override void AI()
        {
            Time++;
            Projectile.spriteDirection = Owner.direction;

            if (!Launched && ChargeProgress < 1f)
            {
                // 后仰蓄力阶段
                float armRot = ArmAnim() * Owner.direction;
                Owner.ChangeDir(Math.Sign(Main.MouseWorld.X - Owner.Center.X));
                Owner.heldProj = Projectile.whoAmI;
                Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, MathHelper.Pi + armRot);

                Projectile.Center = Owner.MountedCenter
                    + Vector2.UnitY.RotatedBy(armRot * Owner.gravDir) * -32f * Owner.gravDir // 这里的数字代表偏移跟武器大小有关
                    + new Vector2(Owner.direction == 1 ? 10 : 3, 0);

                Projectile.rotation = (-MathHelper.PiOver4 * Projectile.direction + armRot) * Owner.gravDir;
                return;
            }

            if (!Launched)
            {
                // 抛出瞬间
                Launched = true;
                Projectile.tileCollide = true;
                Vector2 launchDir = (Main.MouseWorld - Owner.Center).SafeNormalize(Vector2.UnitX * Owner.direction);
                Projectile.velocity = launchDir * 12f;
                Projectile.netUpdate = true;
            }

            // 自旋旋转
            Projectile.rotation += 0.35f * Projectile.direction;

            // 已扎入
            if (LatchedTarget != null && LatchedTarget.active)
            {
                Projectile.Center = LatchedTarget.Center;
                Projectile.velocity = Vector2.Zero;

                // 每隔?帧召唤1发光弹
                ShootTimer++;
                if (ShootTimer >= 5)
                {
                    ShootTimer = 0;
                    Vector2 circlePos = Projectile.Center + Main.rand.NextVector2Unit() * 160f;
                    Vector2 dirToSelf = (Projectile.Center - circlePos).SafeNormalize(Vector2.UnitY);

                    Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        circlePos,
                        dirToSelf * 8f,
                        ModContent.ProjectileType<NettlevineGreatbowReLIGHT>(),
                        (int)(Projectile.damage * 0.5f),
                        0f,
                        Projectile.owner
                    );
                }
                return;
            }

            // 飞行期间释放粒子
            if (Time % 5 == 0)
            {
                GeneralParticleHandler.SpawnParticle(new SparkParticle(Projectile.Center, Main.rand.NextVector2Circular(2f, 2f), false, 20, 1f, Color.ForestGreen));
            }
            if (Time % 7 == 0)
            {
                GeneralParticleHandler.SpawnParticle(new AltSparkParticle(Projectile.Center, Main.rand.NextVector2Circular(1.5f, 1.5f), false, 24, 0.9f, Color.LimeGreen));
            }
            if (Time % 9 == 0)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Grass, Main.rand.NextVector2Circular(1.2f, 1.2f), 100, Color.GreenYellow, 1.1f);
                d.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 记录扎入状态
            if (LatchedTarget == null)
            {
                LatchedTarget = target;
                Projectile.velocity = Vector2.Zero;
                Projectile.netUpdate = true;
                Projectile.timeLeft = 300;
            }

            Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.UnitY);

            // 1?? 有序特效：前方喷射藤蔓能量（Spark）
            for (int i = 0; i < 5; i++)
            {
                Vector2 spread = dir.RotatedByRandom(MathHelper.ToRadians(20f)) * Main.rand.NextFloat(3f, 6f);
                Particle spark = new SparkParticle(
                    Projectile.Center,
                    spread,
                    false,
                    20,
                    1.1f,
                    Color.LimeGreen
                );
                GeneralParticleHandler.SpawnParticle(spark);
            }

            // 2?? 有序特效：淡绿色 AltSpark 尾迹花瓣
            for (int i = 0; i < 3; i++)
            {
                Vector2 swirl = dir.RotatedByRandom(MathHelper.ToRadians(40f)) * Main.rand.NextFloat(1f, 3f);
                Particle alt = new AltSparkParticle(
                    Projectile.Center,
                    swirl,
                    false,
                    24,
                    0.9f,
                    new Color(120, 255, 120)
                );
                GeneralParticleHandler.SpawnParticle(alt);
            }

            // 3?? 无序特效：Dust 弥散
            for (int i = 0; i < 18; i++)
            {
                Vector2 burst = Main.rand.NextVector2Circular(5f, 5f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Grass, burst, 100, Color.ForestGreen, Main.rand.NextFloat(1.0f, 1.4f));
                d.noGravity = true;
            }


            // 5?? 辅助轻型烟雾
            if (Main.rand.NextBool(2))
            {
                Particle smoke = new HeavySmokeParticle(
                    Projectile.Center,
                    dir * 0.5f + Main.rand.NextVector2Circular(0.3f, 0.3f),
                    new Color(80, 255, 80),
                    24,
                    1.2f,
                    0.35f,
                    0.02f,
                    false
                );
                GeneralParticleHandler.SpawnParticle(smoke);
            }
        }






    }
}