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
using Terraria.Audio;

namespace CalamityRangerExpansion.Content.BOWChange.BPrePlantera.CorrodedCaustibowC
{
    public class CorrodedCaustibowReC : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.BPrePlantera";

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
            Projectile.penetrate = 4; // 可造成多少次伤害然后消失
            Projectile.extraUpdates = 1; // 每次额外更新多少次数，值越大弹幕就越快
            Projectile.timeLeft = 120; // 剩余时间
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
            Projectile.velocity *= 0.8f;
        }


        // AI函数，弹幕在飞行期间每一帧都会调用一次
        // 如果想要调用的没那么快，那么在里面添加一个计时器即可实现每隔多少帧调用一次
        private ref float CooldownTimer => ref Projectile.ai[0]; // 用于追踪冷却（击中敌人后冷却 N 帧）
        private const int TrackDelay = 30; // 初始延迟（帧数）
        private const int ReacquireDelay = 60; // 再次追踪前的冷却时间

        private NPC target;
        private float orbitRadius = 200f;
        private float orbitAngle = 0f;
        private Vector2 orbitCenter;

        private ref float Time => ref Projectile.ai[1];

        public override void AI()
        {
            Time++;
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Lighting.AddLight(Projectile.Center, new Color(100, 255, 100).ToVector3() * 0.7f);

            // ?? 寻找目标
            if (target == null || !target.active || !target.CanBeChasedBy(Projectile))
            {
                float closestDist = 1000f;
                foreach (NPC npc in Main.npc)
                {
                    if (!npc.CanBeChasedBy(Projectile)) continue;
                    float dist = Vector2.Distance(Projectile.Center, npc.Center);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        target = npc;
                    }
                }

                if (target != null)
                {
                    orbitCenter = target.Center;
                }
            }

            if (target != null && target.active)
            {
                orbitCenter = target.Center;

                // 半径缩小
                orbitRadius = MathHelper.Lerp(orbitRadius, 20f, 0.015f); // 越来越近

                // 自转角度推进
                orbitAngle += 0.15f;

                // 新位置 = 目标中心 + 旋转半径向量
                Vector2 offset = orbitAngle.ToRotationVector2() * orbitRadius;
                Vector2 desiredPos = orbitCenter + offset;

                // 设置速度为“追向下一个目标位置”
                Vector2 desiredVel = (desiredPos - Projectile.Center).SafeNormalize(Vector2.UnitY) * 12f;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVel, 0.1f);

                // 旋转朝向
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }

            // ??飞行特效 - 亮绿色爆裂感 Dust 环绕
            for (int i = 0; i < 2; i++)
            {
                float spiralAngle = Main.rand.NextFloat(MathHelper.TwoPi);
                Vector2 offset = spiralAngle.ToRotationVector2() * Main.rand.NextFloat(3f, 6f);
                Dust d = Dust.NewDustPerfect(Projectile.Center + offset, DustID.TerraBlade,
                    offset.RotatedBy(MathHelper.PiOver2) * 0.5f, 150,
                    Color.Lerp(Color.Lime, Color.GreenYellow, 0.5f), Main.rand.NextFloat(1.2f, 1.6f));
                d.noGravity = true;
            }
        }



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
            // 播放爆炸音效
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

        


            // 生成毒气粒子，但是有独特的形状
            //for (int i = 0; i < 5; i++)
            //{
            //    float angle = MathHelper.Pi * 1.5f - i * MathHelper.TwoPi / 5f;
            //    float nextAngle = MathHelper.Pi * 1.5f - (i + 2) * MathHelper.TwoPi / 5f;
            //    Vector2 start = angle.ToRotationVector2();
            //    Vector2 end = nextAngle.ToRotationVector2();

            //    for (int j = 0; j < 40; j++)
            //    {
            //        Dust starDust = Dust.NewDustPerfect(Projectile.Center, DustID.PoisonStaff);
            //        starDust.scale = 2.5f;
            //        starDust.velocity = Vector2.Lerp(start, end, j / 40f) * 16f;
            //        starDust.color = Color.DarkGreen;
            //        starDust.noGravity = true;
            //    }
            //}


            int lineSegments = 40; // 每条直线的分段数
            float lineLength = 50f; // 直线的总长度（直线半径的两倍）
            float radius = lineLength / 2f; // 圆的半径为直线的一半

            for (int i = 0; i < 3; i++) // 三条直线
            {
                float angle = i * MathHelper.TwoPi / 3f; // 每条直线的角度间隔120度
                Vector2 direction = angle.ToRotationVector2();

                // 生成直线粒子
                for (int j = 0; j <= lineSegments; j++)
                {
                    float progress = j / (float)lineSegments;
                    Vector2 position = Projectile.Center + direction * (progress - 0.5f) * lineLength; // 直线从 -0.5 到 0.5 绘制

                    Dust lineDust = Dust.NewDustPerfect(position, DustID.PoisonStaff);
                    lineDust.scale = 2.0f;
                    lineDust.velocity = Vector2.Zero; // 静止的粒子
                    lineDust.color = Color.Green;
                    lineDust.noGravity = true;
                }

                // 在直线的中点生成一个圆
                Vector2 circleCenter = Projectile.Center + direction * (0.5f * lineLength - radius);
                for (int j = 0; j < 360; j += 10) // 圆的分段数
                {
                    float circleAngle = MathHelper.ToRadians(j);
                    Vector2 circleOffset = circleAngle.ToRotationVector2() * radius;

                    Dust circleDust = Dust.NewDustPerfect(circleCenter + circleOffset, DustID.PoisonStaff);
                    circleDust.scale = 1.8f;
                    circleDust.velocity = Vector2.Zero;
                    circleDust.color = Color.Green;
                    circleDust.noGravity = true;
                }
            }



            //// 爆炸时生成有毒和恶心的粒子
            //for (int i = 0; i < 30; i++)
            //{
            //    Dust toxicDust = Dust.NewDustPerfect(
            //        Projectile.Center,
            //        DustID.PoisonStaff, // 使用类似毒素的粒子
            //        Main.rand.NextVector2Circular(3f, 3f), // 随机方向
            //        150, // 粒子透明度
            //        Main.rand.NextBool() ? Color.DarkGreen : Color.YellowGreen, // 深绿色或黄绿色
            //        Main.rand.NextFloat(1.2f, 2f) // 粒子缩放大小
            //    );
            //    toxicDust.noGravity = true;
            //}
        }


        // OnHitNPC函数，弹幕在击中敌人造成一次伤害的时候调用一次，击中多次就会调用多次
        // 用于创建命中敌人时的相应特效产生额外弹幕或者给敌人施加debuff
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            CooldownTimer = ReacquireDelay; // 60 帧冷却不追踪
        }

        // ModifyHitNPC函数，弹幕在击中敌人的时候由玩家而不是弹幕本体调用
        // 主要用于修改在击中敌人后自身或者敌人的数值等等
        // 对于特效或产生额外弹幕，请移步至OnHitNPC
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {

        }

        // ModifyDamageHitbox函数，弹幕在击中敌人或者玩家的时候调用一次
        // 主要用于更改碰撞箱，比如临时扩大之类（一般不会用这个）
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {

        }


    }
}