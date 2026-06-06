using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using CalamityMod.Particles;

namespace CalamityRangerExpansion.Content.BOWChange.DPreDog.DaemonsFlameC
{
    public class DaemonsFlameReEffect : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool IsDaemonsFlameReArrow = false;

        public override void AI(Projectile projectile)
        {
            if (!IsDaemonsFlameReArrow)
                return;

            // 每帧粉红色 dust
            if (Main.rand.NextBool(2))
                Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.PinkTorch, Scale: 1.1f);

            // 每帧 Pink spark
            if (Main.rand.NextBool(4))
            {
                AltSparkParticle spark = new AltSparkParticle(
                    projectile.Center - projectile.velocity * 1.5f,
                    projectile.velocity * 0.01f,
                    false,
                    10,
                    1.3f,
                    Color.Pink * 0.15f
                );
                GeneralParticleHandler.SpawnParticle(spark);
            }

           



        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!IsDaemonsFlameReArrow)
                return;
            // 释放特效光点
            CCBLightingBoltsSystem.Spawn_DaemonsFlameBurst(projectile.Center);

            // 白鬼召唤逻辑 ??
            if (Main.myPlayer == projectile.owner && target.active && !target.friendly)
            {
                Vector2 circleCenter = target.Center + new Vector2(0f, target.height / 2 + 16f * 16f); // 敌人正下方 16 格处
                int ghostCount = 6;

                for (int i = 0; i < ghostCount; i++)
                {
                    // ?? 生成点：以 circleCenter 为圆心，半径 128px 的圆内随机分布
                    Vector2 spawnPos = circleCenter + Main.rand.NextVector2Circular(128f, 128f);

                    // ?? 方向：正上方 ±10° 偏移
                    float angle = -MathHelper.PiOver2 + Main.rand.NextFloat(-MathHelper.ToRadians(10f), MathHelper.ToRadians(10f));
                    Vector2 velocity = angle.ToRotationVector2() * Main.rand.NextFloat(8f, 11f);

                    // ?? 发射白鬼弹幕
                    Projectile.NewProjectile(
                        projectile.GetSource_FromThis(),
                        spawnPos,
                        velocity,
                        ModContent.ProjectileType<DaemonsFlameProjWhite>(),
                        projectile.damage / 2,
                        0f,
                        projectile.owner,
                        ai0: target.whoAmI // 传入目标 NPC ID
                    );
                }
            }


            // ?? TODO: 在这里添加命中特效（爆炸、粒子、debuff 等）
        }

        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            if (!IsDaemonsFlameReArrow)
                return true;

            // ?? TODO: 如需自定义绘制可写在这里
            return true;
        }

        public override void OnKill(Projectile projectile, int timeLeft)
        {
            if (!IsDaemonsFlameReArrow)
                return;

            // ?? TODO: 死亡时的粒子或音效处理
        }
    }
}
