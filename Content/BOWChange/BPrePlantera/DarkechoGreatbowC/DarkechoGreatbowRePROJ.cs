using CalamityMod;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.Particles;
using CalamityMod.Sounds;
using Terraria.Audio;
using CalamityRangerExpansion.Content.BOWChange;
 

namespace CalamityRangerExpansion.Content.BOWChange.BPrePlantera.DarkechoGreatbowC
{
    internal class DarkechoGreatbowRePROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.BPrePlantera";
        //public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        // 这里设定多帧图和拖尾类型和长度
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        // 这里套用拖尾模板，可调整是否发光
        // 有的时候会调用更加复杂的情况，比如发光描边等等
        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }
        public override void SetDefaults()
        {
            Projectile.arrow = true; // 他一定是箭，所以这句话一定要加
            Projectile.width = Projectile.height = 10; // 弹幕宽高
            Projectile.friendly = true; // 当然他是我方弹幕
            Projectile.DamageType = DamageClass.Ranged; // 远程伤害
            Projectile.penetrate = 5; // 可造成多少次伤害然后消失
            Projectile.extraUpdates = 5; // 每次额外更新多少次数，值越大弹幕就越快
            Projectile.timeLeft = 420; // 剩余时间
            Projectile.ignoreWater = true; // 是否无视水体的影响
            Projectile.tileCollide = true; // 是否与方块发生碰撞后消除自己
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
            //Projectile.aiStyle = ProjAIStyleID.Arrow; // 让弹幕受到重力影响（这个可选）
        }
        public override void OnSpawn(IEntitySource source)
        {
            // 减速启动
            Projectile.velocity *= 1.5f;

            // 记录传入参数（ai[0] = 蓄力等级, ai[1] = 弓箭弹药类型）
            chargeStrength = MathHelper.Clamp(Projectile.ai[0], 0f, 1f);
            arrowType = (int)Projectile.ai[1];
        }
        private float chargeStrength = 0f;
        private int arrowType = ProjectileID.WoodenArrowFriendly;
        private Vector2? trailSnapshot = null;

        public override void AI()
        {
            // 逐帧增加速度，每帧 * 1.0x
            Projectile.velocity *= 1.01f;

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;

            // 弹幕高速旋转
            //Projectile.rotation += MathHelper.ToRadians(4f);

            if (Projectile.numUpdates % 3 == 0)
            {
                Color outerSparkColor = new Color(173, 216, 230); // 淡蓝色的冰元素颜色
                float scaleBoost = MathHelper.Clamp(Projectile.ai[0] * 0.005f, 0f, 2f);
                float outerSparkScale = 1.2f + scaleBoost;
                SparkParticle spark = new SparkParticle(Projectile.Center, Projectile.velocity, false, 7, outerSparkScale, outerSparkColor);
                GeneralParticleHandler.SpawnParticle(spark);
            }

            // 在飞行路径上留下冰元素粒子特效
            if (Main.rand.NextBool(4)) // 控制粒子生成频率（1/4 概率）
            {
                Dust iceDust = Dust.NewDustPerfect(Projectile.Center, DustID.SnowflakeIce, Projectile.velocity * 0.5f, 150, Color.LightBlue, 1.2f);
                iceDust.noGravity = true; // 使粒子无重力
                iceDust.velocity *= 0.3f; // 调整粒子速度
                iceDust.fadeIn = 1.5f; // 使粒子有一个渐入效果
            }

            // ??粉色能量火花
            if (Projectile.numUpdates % 2 == 0)
            {
                SparkParticle spark = new SparkParticle(
                    Projectile.Center,
                    Projectile.velocity.RotatedByRandom(0.2f) * 0.15f,
                    false,
                    10,
                    1.2f + chargeStrength * 0.6f,
                    Color.HotPink * 0.85f
                );
                GeneralParticleHandler.SpawnParticle(spark);
            }

            // ???柔和能量尘埃
            if (Main.rand.NextBool(3))
            {
                Dust d = Dust.NewDustPerfect(
                    Projectile.Center + Main.rand.NextVector2Circular(4f, 4f),
                    DustID.PinkTorch,
                    Projectile.velocity * 0.2f,
                    100,
                    Color.HotPink,
                    Main.rand.NextFloat(1f, 1.4f)
                );
                d.noGravity = true;
                d.fadeIn = 1.3f;
            }

            BowChangeVFX.SpawnTrail(Projectile, BowChangeTheme.Darkecho, 0.45f + chargeStrength * 0.35f);

            // 记录弹幕飞行中的一个历史位置（每 4 帧更新一次）
            if (Time % 4 == 0)
            {
                trailSnapshot = Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.Zero) * 48f; // 距离自身后方约3格
            }
            Time++;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(CommonCalamitySounds.SwiftSliceSound with { Volume = 0.5f }, Projectile.Center);
            BowChangeVFX.SpawnImpact(Projectile, BowChangeTheme.Darkecho, 1f + chargeStrength * 0.4f);

            Vector2 center = Projectile.Center;

            // ===== 命中后：等分散射 6 个分裂水晶镖 =====
            int dartCount = 6;
            float speed = 8f;

            for (int i = 0; i < dartCount; i++)
            {
                float angle = MathHelper.TwoPi * i / dartCount;
                Vector2 velocity = angle.ToRotationVector2() * speed;

                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    velocity,
                    ModContent.ProjectileType<DarkechoGreatbowReDarts>(),
                    Projectile.damage,          // ? 100% 伤害
                    Projectile.knockBack,
                    Projectile.owner
                );
            }

            for (int i = 0; i < 2 + (int)(chargeStrength * 2f); i++)
            {
                Vector2 echoVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY)
                    .RotatedBy(Main.rand.NextFloat(-0.85f, 0.85f))
                    * Main.rand.NextFloat(4f, 7f);
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    trailSnapshot ?? Projectile.Center,
                    echoVelocity,
                    ModContent.ProjectileType<DarkechoGreatbowReEchoShard>(),
                    Math.Max(1, (int)(Projectile.damage * 0.45f)),
                    Projectile.knockBack * 0.5f,
                    Projectile.owner);
            }


            // ??视觉爆发特效（粒子爆炸 + 能量圈）
            for (int i = 0; i < 8; i++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(5f, 5f);
                SparkParticle spark = new SparkParticle(center, vel, false, 18, 1.4f + chargeStrength, Color.Fuchsia);
                GeneralParticleHandler.SpawnParticle(spark);
            }

            for (int i = 0; i < 3; i++)
            {
                Dust d = Dust.NewDustPerfect(center, DustID.PinkTorch, Main.rand.NextVector2Circular(3f, 3f), 100, Color.HotPink, 1.5f);
                d.noGravity = true;
                d.fadeIn = 1.6f;
            }

            Particle ring = new DirectionalPulseRing(
                center,
                Vector2.Zero,
                Color.Pink,
                new Vector2(1f, 1.8f),
                0f,
                0.2f,
                3.5f,
                20
            );
            GeneralParticleHandler.SpawnParticle(ring);

            {
                // 如果 trailSnapshot 有记录，就从那里发射箭矢（否则仍使用中心）
                Vector2 spawnOrigin = trailSnapshot ?? center;

                // ??从旧位置散射原始箭矢 x3（方向依然指向目标）
                for (int i = 0; i < 3; i++)
                {
                    Vector2 direction = (target.Center - spawnOrigin).SafeNormalize(Vector2.UnitY)
                                        .RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f)); // 加点散射角度
                    Vector2 velocity = direction * 7f;

                    Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        spawnOrigin,
                        velocity,
                        arrowType,
                        (int)(Projectile.damage * 0.6f),
                        Projectile.knockBack * 0.6f,
                        Projectile.owner
                    );
                }

            }

            // ??叠加debuff
            target.AddBuff(BuffID.Frostburn2, 300);
            target.AddBuff(BuffID.Chilled, 300);
        }


        public ref float Time => ref Projectile.localAI[0];

        // 初始的时候不会造成伤害，直到5为止
        // 这是为了防止弹幕在突然出现的时候就造成伤害，一般都会加上
        public override bool? CanDamage() => Time >= 5f;

        // 当与方块发生碰撞时会发生什么
        // 注意这前提是必须Projectile.tileCollide = true;
        // 如果这个弹幕是一个钻头，可以破坏方块的话那么就在这里面实现相关操作
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return base.OnTileCollide(oldVelocity);
        }

        // OnKill函数，弹幕在死亡的时候仅调用一次
        // 用于创建死亡时释放的额外弹幕或者死亡时的特效
        public override void OnKill(int timeLeft)
        {
            // 可以选择在这里播放一个原版音效
            // SoundEngine.PlaySound(SoundID.xxx, Projectile.position);




        }

     

    }
}
