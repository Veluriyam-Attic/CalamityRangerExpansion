using CalamityMod;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.Audio;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;

namespace CalamityRangerExpansion.Content.BOWChange.BPrePlantera.ArbalestC
{
    internal class ArbalestSilverPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.BPrePlantera";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft > 588) // 前12帧不画
                return false;
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);

            float spiralRadius = 24f;
            float thickness = 2f;
            Color baseColor = Color.Lerp(Color.White, Color.Silver, 0.7f);

            // 多螺旋线条数量
            int strandCount = 3;
            
            // 遍历每一条螺旋
            for (int strand = 0; strand < strandCount; strand++)
            {
                float phaseOffset = MathHelper.TwoPi * strand / strandCount;
                Vector2 previous = Projectile.oldPos[0] + Projectile.Size * 0.5f;

                for (int i = 1; i < Projectile.oldPos.Length; i++)
                {
                    float completionRatio = i / (float)Projectile.oldPos.Length;
                    Vector2 basePos = Projectile.oldPos[i] + Projectile.Size * 0.5f;

                    // 以 sin 曲线进行偏移
                    float sinFactor = (float)Math.Sin(completionRatio * MathHelper.TwoPi * 2 + Main.GlobalTimeWrappedHourly * 4f + phaseOffset);
                    Vector2 perp = Projectile.velocity.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero);
                    Vector2 offset = perp * sinFactor * spiralRadius;

                    Vector2 current = basePos + offset;

                    float fade = 1f - completionRatio;
                    Color c = baseColor * fade;

                    Main.spriteBatch.DrawLineBetter(previous, current, c, thickness);
                    previous = current;
                }
            }

            return false;
        }
        public override void SetDefaults()
        {
            // 设置弹幕的基础属性
            Projectile.width = 11; // 弹幕宽度
            Projectile.height = 24; // 弹幕高度
            Projectile.friendly = true; // 对敌人有效
            Projectile.DamageType = DamageClass.Ranged; // 远程伤害类型
            Projectile.penetrate = 8; // 穿透力
            Projectile.timeLeft = 600; // 弹幕存在时间为600帧
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
            Projectile.ignoreWater = true; // 弹幕不受水影响
            Projectile.arrow = true;
            Projectile.extraUpdates = 14; 
            //Projectile.aiStyle = ProjAIStyleID.Arrow; // 让弹幕受到重力影响
        }

        public override void AI()
        {
            // 基础朝向/打光
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;
            Lighting.AddLight(Projectile.Center, Color.LightSkyBlue.ToVector3() * 0.55f);

            // —— 命中后进入追踪（保留原有逻辑，不改动）——
            if (Projectile.ai[0] == 1f)
            {
                NPC target = FindClosestTarget(Projectile.Center, 600f);
                if (target != null)
                {
                    Vector2 toTarget = target.Center - Projectile.Center;
                    float distance = toTarget.Length();
                    float targetSpeed = MathHelper.Clamp(distance * 0.018f, 5f, 10f); // 稍提速，穿刺感更强
                    Vector2 desiredVelocity = toTarget.SafeNormalize(Projectile.velocity) * targetSpeed;
                    float turnSharpness = 0.06f; // 转向更干脆
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, turnSharpness);
                }
            }

            // =================== 飞行特效：穿刺·破阵 ===================
            // 抽样到每帧（避免 extraUpdates 导致的粒子爆量）
            if (Projectile.numUpdates == 0)
            {
                Vector2 fwd = Projectile.velocity.SafeNormalize(Vector2.UnitX);
                Vector2 nrm = new Vector2(-fwd.Y, fwd.X); // 法线
                float spd = Projectile.velocity.Length();
                float thrust = MathHelper.Clamp(spd / 12f, 0.6f, 1.6f); // 推进强度系数
                Color core = Color.Lerp(Color.White, Color.Silver, 0.85f);
                Color edge = Color.Lerp(core, Color.LightSkyBlue, 0.45f);

                // ① 前锥冲击线（刺破空气）——多条向前收束能量线
                int spearRays = 3 + (Main.rand.NextBool() ? 1 : 0);
                for (int i = 0; i < spearRays; i++)
                {
                    Vector2 dir = fwd.RotatedBy(Main.rand.NextFloat(-0.12f, 0.12f)); // ±6.8°
                    var ray = new SparkParticle(
                        Projectile.Center + fwd * 6f,           // 稍微位于弹头前
                        dir * (2.8f + 1.7f * thrust),          // 越快越长
                        false,
                        10 + Main.rand.Next(6),                 // 寿命短，干脆
                        1.0f + 0.22f * thrust,                  // 亮细条
                        core
                    );
                    GeneralParticleHandler.SpawnParticle(ray);
                }

                // ② 轴向能量拖丝（高速穿刺后的尾迹）
                {
                    var tail = new SparkParticle(
                        Projectile.Center - fwd * 4f,
                        -fwd * (1.5f + 0.7f * thrust),
                        false,
                        22,
                        0.92f,
                        edge
                    );
                    GeneralParticleHandler.SpawnParticle(tail);

                    if (Main.rand.NextBool(2))
                    {
                        var tail2 = new SparkParticle(
                            Projectile.Center - fwd * 6f + nrm * Main.rand.NextFloat(-1.2f, 1.2f),
                            -fwd * (1.1f + 0.6f * thrust),
                            false,
                            18,
                            0.84f,
                            edge
                        );
                        GeneralParticleHandler.SpawnParticle(tail2);
                    }
                }

                // ③ 破阵碎片环（把能量“阵面”扯开）
                if (Main.rand.NextBool(3))
                {
                    int stars = 4; // 左右各两枚
                    for (int j = 0; j < stars; j++)
                    {
                        float sign = (j % 2 == 0) ? 1f : -1f;
                        Vector2 anchor = Projectile.Center + nrm * sign * Main.rand.NextFloat(1f, 2f);
                        Vector2 v = (nrm * sign).RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * (2.0f + 1.1f * thrust);
                        var star = new CritSpark(
                            anchor,
                            v + Main.rand.NextVector2Circular(0.3f, 0.3f), // 微抖动
                            core,                                          // 起始
                            edge,                                          // 结束
                            0.9f + 0.16f * thrust,
                            14 + Main.rand.Next(6)
                        );
                        GeneralParticleHandler.SpawnParticle(star);
                    }
                }

                // ④ 少量 Dust 衬底（别喧宾夺主）
                if (Main.rand.NextBool(4))
                {
                    Dust d = Dust.NewDustPerfect(
                        Projectile.Center + fwd * 2f,
                        Main.rand.NextBool() ? 91 : 66,                  // 银蓝系
                        fwd * 0.6f,
                        120,
                        Color.Lerp(edge, Color.White, 0.35f),
                        0.9f + Main.rand.NextFloat(-0.1f, 0.1f)
                    );
                    d.noGravity = true;
                }
            }
            // ==========================================================
        }

        private NPC FindClosestTarget(Vector2 position, float maxDistance)
        {
            NPC bestTarget = null;
            float closest = maxDistance;

            foreach (NPC npc in Main.npc)
            {
                if (npc.CanBeChasedBy() && !npc.friendly && !npc.dontTakeDamage)
                {
                    float distance = Vector2.Distance(position, npc.Center);
                    if (distance < closest && Collision.CanHitLine(position, 1, 1, npc.Center, 1, 1))
                    {
                        closest = distance;
                        bestTarget = npc;
                    }
                }
            }

            return bestTarget;
        }



        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[0] == 0f)
                Projectile.ai[0] = 1f; // 命中后进入追踪状态

            // 生成银色火花
            int sparkCount = Main.rand.Next(6, 10);
            for (int i = 0; i < sparkCount; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(4f, 4f);
                var spark = new SparkParticle(
                    Projectile.Center,
                    velocity,
                    false,
                    Main.rand.Next(8, 13),
                    Main.rand.NextFloat(1f, 1.4f),
                    Color.Silver
                );
                CalamityMod.Particles.GeneralParticleHandler.SpawnParticle(spark);
            }

            // 混合白色轻烟雾
            int smokeCount = Main.rand.Next(3, 6);
            for (int i = 0; i < smokeCount; i++)
            {
                var smoke = new HeavySmokeParticle(
                    Projectile.Center,
                    Main.rand.NextVector2Circular(1.2f, 1.2f),
                    Color.WhiteSmoke,
                    22,
                    Main.rand.NextFloat(1f, 1.5f),
                    0.4f,
                    Main.rand.NextFloat(-0.5f, 0.5f),
                    false
                );
                CalamityMod.Particles.GeneralParticleHandler.SpawnParticle(smoke);
            }
        }


        public override void OnKill(int timeLeft)
        {
            //SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/SevensStrikerCoinShot"));
         
        }




    }
}