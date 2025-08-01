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
using CalamityMod.Projectiles.Typeless;

namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.R36
{
    internal class R36Proj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.R36";

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj"; // 使用完全透明贴图
        public override void SetStaticDefaults()
        {
            // 设置弹幕拖尾长度和模式
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // 绘制控制函数，可用于绘制自定义贴图、添加发光效果、叠加特效等
            // 若不需要可返回 true 使用默认绘制【很不推荐】
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 36;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1; // 可击中次数
            Projectile.timeLeft = 150;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 4; // 可调节飞行平滑度
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
        }

        // 储存弹药类型
        public int StoredAmmoType;
        public override void OnSpawn(IEntitySource source)
        {
            StoredAmmoType = (int)Projectile.ai[1]; // 在生成时把 ai[1] 的值记录下来

            // 💥生成瞬间的前向喷射特效（Spark + Dust）
            for (int i = 0; i < 20; i++)
            {
                Vector2 offset = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(0.35f) * Main.rand.NextFloat(3f, 7f);
                Vector2 spawnPos = Projectile.Center + offset * 1.5f;

                // 🔶橙色火花（线性粒子）
                LineParticle spark = new LineParticle(
                    spawnPos,
                    offset * 0.4f,
                    false,
                    25,
                    1.3f,
                    Color.Orange * 0.9f
                );
                GeneralParticleHandler.SpawnParticle(spark);

                // 🔸橙黄 Dust 特效
                Dust dust = Dust.NewDustPerfect(spawnPos, DustID.Torch, offset * 0.3f);
                dust.scale = Main.rand.NextFloat(1.1f, 1.6f);
                dust.noGravity = true;
            }
        }


        public override void AI()
        {
            // 调整旋转方向
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            if (Projectile.timeLeft < 148)
            {
                // ✨LineParticle 闪光拖尾（暗金调，夸张数量）
                for (int i = 0; i < 2; i++)
                {
                    Vector2 posOffset = Main.rand.NextVector2Circular(8f, 8f);
                    Vector2 vel = -Projectile.velocity * Main.rand.NextFloat(0.2f, 0.9f);
                    Color color = Main.rand.NextBool() ? Color.DarkGoldenrod : Color.Orange * 0.8f;
                    LineParticle spark = new LineParticle(Projectile.Center + posOffset, vel, false, 5, 1.8f, color);
                    GeneralParticleHandler.SpawnParticle(spark);
                }

                // 🔥SparkParticle 能量轨迹（类似巨龙之火风格）
                if (Main.rand.NextBool(1))
                {
                    SparkParticle trail = new SparkParticle(
                        Projectile.Center,
                        Projectile.velocity * 0.2f,
                        false,
                        60,
                        1.2f,
                        Color.OrangeRed * 0.8f
                    );
                    GeneralParticleHandler.SpawnParticle(trail);
                }

                // 🌪Dust 特效（具有数学规律的环绕轨迹）
                float angleOffset = Main.GlobalTimeWrappedHourly * 10f;
                for (int i = 0; i < 2; i++)
                {
                    float angle = angleOffset + MathHelper.TwoPi * i / 2f;
                    Vector2 offset = angle.ToRotationVector2() * 12f;
                    Vector2 dustPos = Projectile.Center + offset;
                    Dust dust = Dust.NewDustPerfect(dustPos, DustID.Torch, -offset.SafeNormalize(Vector2.Zero) * 0.3f);
                    dust.scale = 1.3f;
                    dust.noGravity = true;
                }
            }
        
        }


        public override void OnKill(int timeLeft)
        {
            // 💥生成爆炸弹幕（FuckYou）
            int explosionID = Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                Projectile.Center,
                Vector2.Zero,
                ModContent.ProjectileType<FuckYou>(),
                (int)(Projectile.damage * 1.8f),
                0f,
                Projectile.owner
            );

            if (Main.projectile.IndexInRange(explosionID))
            {
                var p = Main.projectile[explosionID];
                p.friendly = true;
                p.hostile = true;
                p.scale = 2.5f;
            }


            StoredAmmoType = (int)Projectile.ai[1];

            //Main.NewText($"[R36] StoredAmmoType = {StoredAmmoType}", Color.OrangeRed);

            // 💣发射 7 枚仁慈破片（使用传入弹药类型 StoredAmmoType）
            int shardType = StoredAmmoType;
            Vector2 toPlayer = Main.LocalPlayer.Center - Projectile.Center;
            float playerAngle = toPlayer.ToRotation();
            int playerSector = (int)(MathHelper.WrapAngle(playerAngle) * 180f / MathF.PI / 10f + 36) % 36; // 玩家所在扇区（0~35）

            int shardCount = Main.rand.Next(3, 11); // 生成 3~10 之间的随机数量（含3，含10）
            for (int i = 0; i < shardCount; i++)
            {
                float chosenAngle;

                if (Main.rand.NextFloat() < 0.6f) // 60% 仁慈破片
                {
                    // 尝试避开玩家所在扇区
                    int tries = 0;
                    do
                    {
                        float randomAngle = Main.rand.NextFloat(MathHelper.TwoPi);
                        int testSector = (int)(MathHelper.WrapAngle(randomAngle) * 180f / MathF.PI / 10f + 36) % 36;
                        if (testSector != playerSector || tries++ > 10) // 超过10次就不强求
                        {
                            chosenAngle = randomAngle;
                            break;
                        }
                    } while (true);
                }
                else // 40% 普通破片
                {
                    chosenAngle = Main.rand.NextFloat(MathHelper.TwoPi);
                }

                // 添加轻微扰动
                chosenAngle += Main.rand.NextFloat(-0.3f, 0.3f);

                Vector2 shardVelocity = chosenAngle.ToRotationVector2() * Main.rand.NextFloat(14f, 17f);
                int shardID = Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    shardVelocity,
                    shardType,
                    (int)(Projectile.damage * 1.5f),
                    2f,
                    Projectile.owner
                );

                if (Main.projectile.IndexInRange(shardID))
                {
                    var shard = Main.projectile[shardID];
                    shard.friendly = true;
                    shard.hostile = true;
                    shard.tileCollide = false;

                    // ✅ 标记为 R36 特制破片，启用延迟伤害机制
                    shard.GetGlobalProjectile<R36ProjChange>().IsR36Shard = true;
                }
            }





            {
                // 🌟R36死亡特效：中心有序 → 外部混乱
                for (int i = 0; i < 12; i++)
                {
                    float angle = MathHelper.TwoPi * i / 12f + Main.rand.NextFloat(-0.15f, 0.15f);
                    Vector2 dir = angle.ToRotationVector2();
                    Vector2 basePos = Projectile.Center + dir * Main.rand.NextFloat(4f, 12f);
                    Vector2 velocity = dir * Main.rand.NextFloat(1f, 4f);

                    // 1. 🌕GlowOrb：代表中心的有序与控制
                    for (int j = 0; j < 3; j++)
                    {
                        Vector2 orbOffset = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(0f, 6f);
                        GlowOrbParticle orb = new GlowOrbParticle(
                            Projectile.Center + orbOffset,
                            orbOffset.SafeNormalize(Vector2.Zero) * 0.2f,
                            false,
                            10,
                            1.1f,
                            Color.White * 1.2f,
                            true,
                            false,
                            true
                        );
                        GeneralParticleHandler.SpawnParticle(orb);
                    }

                    // 2. 🔥SparkParticle：乱流感拖尾
                    SparkParticle trail = new SparkParticle(
                        basePos,
                        velocity,
                        false,
                        Main.rand.Next(30, 45),
                        Main.rand.NextFloat(0.9f, 1.3f),
                        Color.Orange * 0.85f
                    );
                    GeneralParticleHandler.SpawnParticle(trail);

                    // 3. 💉PointParticle：爆裂感针刺
                    PointParticle spark = new PointParticle(
                        basePos,
                        -velocity * 0.4f,
                        false,
                        15,
                        1.0f,
                        Color.Goldenrod
                    );
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }


            {
                // 🌑 添加一个半径为 5×15 的 Spark 椭圆粒子圈
                int points = 50;
                float a = 55f; // 横向半轴
                float b = 55f;  // 纵向半轴

                for (int i = 0; i < points; i++)
                {
                    float t = MathHelper.TwoPi / points * i;

                    // 计算椭圆上当前点的位置
                    Vector2 pos = Projectile.Center + new Vector2(a * (float)Math.Cos(t), b * (float)Math.Sin(t));

                    // 椭圆切线方向（导数方向）
                    Vector2 tangent = new Vector2(-a * (float)Math.Sin(t), b * (float)Math.Cos(t));
                    Vector2 dir = tangent.SafeNormalize(Vector2.UnitX); // 标准化方向作为粒子运动方向

                    // 🌟 添加粒子
                    Particle p = new SparkParticle(
                        pos,
                        dir * 2.5f, // 粒子速度
                        false,      // 不受重力
                        45,         // 生命周期
                        1.1f,       // 缩放
                        Color.Lerp(Color.Orange, Color.Goldenrod, Main.rand.NextFloat(0.2f, 0.6f))
                    );
                    GeneralParticleHandler.SpawnParticle(p);
                }

            }


        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 弹幕命中 NPC 时执行，可用于生成击中特效、播放音效、回复血量等
        }




    }
}
