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

namespace CalamityRangerExpansion.Content.BOWChange.CPreMoodLord.MalevolenceC
{
    internal class MalevolenceReARROW : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.CPreMoodLord";

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
            Projectile.penetrate = 1; // 可造成多少次伤害然后消失
            Projectile.extraUpdates = 1; // 每次额外更新多少次数，值越大弹幕就越快
            Projectile.timeLeft = 420; // 剩余时间
            Projectile.ignoreWater = true; // 是否无视水体的影响
            Projectile.tileCollide = true; // 是否与方块发生碰撞后消除自己
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
            Projectile.aiStyle = ProjAIStyleID.Arrow; // 让弹幕受到重力影响（这个可选）
        }

        // OnSpawn函数，弹幕在刚出现的时候仅调用一次
        // 用于创建出现时的一些特效或者修改一些数值
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.velocity *= 1.8f;
        }
        private bool hasDetectedTarget = false;
        private int trackingCountdown = 0;
        private bool activatedTracking = false;

        // AI函数，弹幕在飞行期间每一帧都会调用一次
        // 如果想要调用的没那么快，那么在里面添加一个计时器即可实现每隔多少帧调用一次
        public override void AI()
        {
            Time++;

            // === 保持方向（不会永远摆正）===
            if (!activatedTracking)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            else
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            // === 轻微扭动飞行（前期）===
            if (!activatedTracking)
            {
                float waveStrength = 0.35f;
                float frequency = 0.12f;
                Vector2 baseVel = Projectile.velocity.SafeNormalize(Vector2.UnitY);
                Projectile.velocity = baseVel.RotatedBy(Math.Sin(Time * frequency) * waveStrength) * Projectile.velocity.Length();
            }

            // === 稍微绿色 Dust 特效（不喧宾夺主）===
            if (Main.rand.NextBool(3))
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Grass, -Projectile.velocity * 0.2f, 100, Color.GreenYellow, Main.rand.NextFloat(0.6f, 0.9f));
                d.noGravity = true;
            }

            // === 检测敌人延迟追踪 ===
            if (!activatedTracking)
            {
                if (!hasDetectedTarget)
                {
                    NPC nearby = Projectile.Center.ClosestNPCAt(240f);
                    if (nearby != null)
                    {
                        hasDetectedTarget = true;
                        trackingCountdown = 40;
                    }
                }
                else if (trackingCountdown > 0)
                {
                    trackingCountdown--;
                    if (trackingCountdown == 0)
                    {
                        // ??触发爆炸特效
                        TriggerTrackEffect();
                        activatedTracking = true;
                    }
                }
            }
            else
            {
                // === 丝滑强力追踪 ===
                NPC target = Projectile.Center.ClosestNPCAt(1600f);
                if (target != null && target.CanBeChasedBy())
                {
                    Vector2 desiredVelocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitX) * 16f;
                    float inertia = 15f;
                    Projectile.velocity = (Projectile.velocity * (inertia - 1f) + desiredVelocity) / inertia;
                }
            }
        }
        private void TriggerTrackEffect()
        {
            Vector2 center = Projectile.Center;

            // 主爆炸 Dust
            for (int i = 0; i < 40; i++)
            {
                Dust d = Dust.NewDustPerfect(center, DustID.Venom, Main.rand.NextVector2Circular(6f, 6f), 0, Color.LimeGreen, Main.rand.NextFloat(1.2f, 1.8f));
                d.noGravity = true;
            }

            // 爆裂光点
            for (int i = 0; i < 8; i++)
            {
                Particle spark = new SparkParticle(
                    center,
                    Main.rand.NextVector2Circular(4f, 4f),
                    false,
                    20,
                    1.2f,
                    Color.LightGreen
                );
                GeneralParticleHandler.SpawnParticle(spark);
            }

            // 小烟雾
            for (int i = 0; i < 6; i++)
            {
                Particle smoke = new HeavySmokeParticle(
                    center,
                    Main.rand.NextVector2Circular(2f, 2f),
                    Color.GreenYellow * 0.5f,
                    50,
                    Main.rand.NextFloat(0.8f, 1.4f),
                    0.3f,
                    0f,
                    true
                );
                GeneralParticleHandler.SpawnParticle(smoke);
            }
        }

        public ref float Time => ref Projectile.ai[1];

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

        // OnHitNPC函数，弹幕在击中敌人造成一次伤害的时候调用一次，击中多次就会调用多次
        // 用于创建命中敌人时的相应特效产生额外弹幕或者给敌人施加debuff
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 生成轻型烟雾特效
            for (int i = 0; i < 5; i++) // 生成 5 个粒子
            {
                Vector2 smokeVelocity = Projectile.velocity * Main.rand.NextFloat(0.5f, 1.0f) + Main.rand.NextVector2Circular(1f, 1f); // 随机速度
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Smoke, smokeVelocity, 150, Color.OrangeRed, Main.rand.NextFloat(1.2f, 1.8f));
                dust.noGravity = true; // 无重力效果
            }
        }



    }
}