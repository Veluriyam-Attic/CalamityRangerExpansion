using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod;
using CalamityRangerExpansion.CREConfigs;
using CalamityMod.Projectiles.Typeless;

namespace CalamityRangerExpansion.Content.DeveloperItems.Bullet.Pao
{
    public class PaoPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.ChineseChess.Pao";
        public override string Texture => "CalamityRangerExpansion/Content/DeveloperItems/Bullet/Pao/Pao";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
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
            Projectile.width = 44;
            Projectile.height = 44;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 450;
            Projectile.MaxUpdates = 3;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;
        }
        private bool firstHit = true; // 用于标记是否为第一次击中

        public override void AI()
        {
            // 子弹旋转逻辑
            Projectile.rotation = Projectile.velocity.ToRotation();

            // 子弹在短时间后变得可见
            if (Projectile.timeLeft == 445)
                Projectile.alpha = 0;


            // 添加飞行粒子效果
            if (Main.rand.NextBool(2)) // 修改为 1/2 概率
            {
                int[] dustTypes = { DustID.GemTopaz, DustID.Flare, DustID.OrangeTorch };
                int selectedDust = Main.rand.Next(dustTypes); // 随机选择粒子类型
                Dust dust = Dust.NewDustPerfect(
                    Projectile.Center,
                    selectedDust, // 随机粒子
                    -Projectile.velocity.RotatedByRandom(0.1f) * Main.rand.NextFloat(0.01f, 0.3f)
                );
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.6f, 1.0f); // 保持原粒子大小
            }

            //// 检测与敌人的碰撞
            //if (!collidedWithNPC)
            //{
            //    for (int i = 0; i < Main.maxNPCs; i++)
            //    {
            //        NPC npc = Main.npc[i];
            //        if (npc.active && !npc.friendly && npc.CanBeChasedBy() &&
            //            Projectile.Hitbox.Intersects(npc.Hitbox))
            //        {
            //            collidedWithNPC = true; // 标记碰撞
            //            collisionTimer = (int)Main.GameUpdateCount; // 记录当前帧数
            //            break;
            //        }
            //    }
            //}
        }
        //public override bool? CanDamage()
        //{
        //    // 是否允许造成伤害
        //    return collidedWithNPC && (int)Main.GameUpdateCount - collisionTimer >= 20 ? (bool?)true : (bool?)false;
        //}

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (firstHit)
            {
                // 第一次击中时，仅造成 0.1 倍伤害
                modifiers.FinalDamage *= 0.1f;

                // 播放音效，音量x
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode with { Volume = 1.0f }, Projectile.Center);

                // 标记为非第一次击中
                firstHit = false;
            }
        }

        private bool firstHitEffect = true; // 标记是否是第一次调用特效逻辑

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (firstHitEffect)
            {
                firstHitEffect = false; // 标记为已调用

                // 传送到以自己当前面向方向为正方向的前方 x 像素
                Vector2 forwardDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);
                Projectile.position = Projectile.Center + forwardDirection * 300f;

                // 在传送路径上生成烟雾特效
                Vector2 start = Projectile.Center;
                Vector2 end = target.Center;
                int particleCount = 20;
                for (int i = 0; i < particleCount; i++)
                {
                    float lerpFactor = i / (float)particleCount;
                    Vector2 position = Vector2.Lerp(start, end, lerpFactor);
                    Vector2 dustVelocity = Main.rand.NextVector2Circular(1f, 1f);
                    Particle smoke = new HeavySmokeParticle(
                        position,
                        dustVelocity * Main.rand.NextFloat(1f, 2.6f),
                        Color.Yellow,
                        18,
                        Main.rand.NextFloat(0.9f, 1.6f),
                        0.35f,
                        Main.rand.NextFloat(-1, 1),
                        true
                    );
                    GeneralParticleHandler.SpawnParticle(smoke);
                }
            }

            // 其他击中逻辑（如生成粒子或触发特效）
            {
                // 获取弹幕的正前方方向
                Vector2 forwardDirection2 = Projectile.velocity.SafeNormalize(Vector2.Zero);
                int particleCount2 = 15;
                float angleRange = MathHelper.ToRadians(25);

                for (int i = 0; i < particleCount2; i++)
                {
                    float randomAngle = Main.rand.NextFloat(-angleRange, angleRange);
                    Vector2 particleDirection = forwardDirection2.RotatedBy(MathHelper.Pi + randomAngle);
                    Vector2 trailVelocity = particleDirection * Main.rand.NextFloat(2f, 5f);

                    Particle trail = new SparkParticle(
                        Projectile.Center,
                        trailVelocity,
                        false,
                        60,
                        1.0f,
                        Color.OrangeRed
                    );
                    GeneralParticleHandler.SpawnParticle(trail);
                }
            }
        }


        public override void OnKill(int timeLeft)
        {
            // 在死亡时释放一个 2.0 大小的 FuckYou 弹幕
            Projectile.NewProjectile(
                Projectile.GetSource_Death(),
                Projectile.Center,
                Vector2.Zero,
                ModContent.ProjectileType<FuckYou>(),
                (int)(Projectile.damage * 2.0f), // 伤害倍率 2.0
                Projectile.knockBack,
                Projectile.owner
            );

            // 播放爆炸音效
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);
        }
    }
}
