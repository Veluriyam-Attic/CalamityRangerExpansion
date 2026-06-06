// 完整文件：BarinauticalReHold.cs（含五个特效函数）
// ?? 保持原有攻击结构不变，仅补充标准视觉特效封装
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityRangerExpansion.Content.BOWChange.BPrePlantera.ArbalestC;
using CalamityRangerExpansion.Content.BOWChange;
using CalamityMod;
using CalamityMod.Particles;
using System;

namespace CalamityRangerExpansion.Content.BOWChange.BPrePlantera.BarinauticalC
{
    public class BarinauticalReHold : BaseGunHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.BPrePlantera";
        public override string Texture => "CalamityRangerExpansion/Content/BOWChange/BPrePlantera/BarinauticalC/BarinauticalRe";
        public override int AssociatedItemID => ModContent.ItemType<BarinauticalRe>();
        public override float MaxOffsetLengthFromArm => 15f;

        public override bool? CanDamage() => Main.player[Projectile.owner].GetModPlayer<OpenBowDamagePlayer>().OpenBowDamage;

        public override Vector2 GunTipPosition => Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * (Projectile.width * 0.5f + 10f);

        public ref float ChargeFrames => ref Projectile.ai[0];
        public ref float ShotsRemaining => ref Projectile.ai[1];

        private bool hasFired = false;
        private int frameCounter = 0;
        private int patternIndex = 0;
        private readonly int[] attackPattern = { 1, 1, 1, 1, 0, 0, 0 };

        public override void AI()
        {
            KeepRefreshingLifetime = true;
            base.AI();
        }

        public override void HoldoutAI()
        {
            if (Owner.channel)
            {
                ChargeFrames++;

                if (ChargeFrames < 90f)
                {
                    BowChangeVFX.SpawnCharge(Projectile, GunTipPosition, BowChangeTheme.Ocean, ChargeFrames / 90f, 0.7f);
                    SpawnChargeFX_EveryFrame(GunTipPosition);
                }
                else
                {
                    BowChangeVFX.SpawnCharge(Projectile, GunTipPosition, BowChangeTheme.Ocean, 1f, 0.65f);
                    SpawnChargeFX_Complete(GunTipPosition);

                    // 第一次进入满蓄 → 一次性爆发提示
                    if (!hasFired)
                    {
                        BowChangeVFX.SpawnReadyBurst(Projectile, GunTipPosition, BowChangeTheme.Ocean, 0.95f);
                        SpawnChargeReadyOnceFX(GunTipPosition);
                        hasFired = true;
                    }
                }

                frameCounter++;
            }
        }



        public override void KillHoldoutLogic()
        {
            Player player = Owner;

            if (Owner.CantUseHoldout())
            {
                if (ChargeFrames < 90)
                {
                    Projectile.Kill();
                    return;
                }

                if (ShotsRemaining <= 0 && ChargeFrames > 0)
                {
                    ShotsRemaining = attackPattern.Length;
                    patternIndex = 0;
                }


                if (ShotsRemaining > 0)
                {
                    FireNextProjectile(player);
                    ShotsRemaining--;

                    if (ShotsRemaining <= 0)
                        Projectile.Kill();
                }
            }
        }
        private void FireNextProjectile(Player player)
        {
            if (patternIndex < 0 || patternIndex >= attackPattern.Length)
            {
                Projectile.Kill();
                return;
            }

            int mode = attackPattern[patternIndex];
            FireProjectile(mode);
            patternIndex++;
        }


        private void FireProjectile(int mode)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 shootDirection = Projectile.velocity.SafeNormalize(Vector2.UnitX * player.direction);
            Item heldItem = player.HeldItem;

            if (player.HasAmmo(heldItem))
            {
                if (player.PickAmmo(heldItem, out int ammoProj, out float shootSpeed, out int damage, out float knockback, out int _))
                {
                    Vector2 spawnPos = Projectile.Center + shootDirection * 4f;
                    Vector2 perturbed = shootDirection.RotatedByRandom(MathHelper.ToRadians(1.5f));
                    float speed = Main.rand.NextFloat(14f, 17f);

                    int proj = Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        spawnPos,
                        perturbed * speed,
                        ModContent.ProjectileType<BarinauticalRePROJ>(),
                        damage,
                        knockback,
                        player.whoAmI
                    );

                    if (proj.WithinBounds(Main.maxProjectiles))
                        Main.projectile[proj].localAI[0] = mode; // 0=A，1=B
                }
            }

            SoundEngine.PlaySound(SoundID.Item93 with { Volume = 0.7f }, player.Center);
            SpawnPerShotFX(GunTipPosition, shootDirection * 16f);
            BowChangeVFX.SpawnMuzzle(Projectile, GunTipPosition, shootDirection * 16f, BowChangeTheme.Ocean, 0.8f);
        }

        // =========================
        // 以下是五个标准特效函数（水电主题）
        // =========================


        // === 蓄力期间·持续特效（海潮聚能：水汽上浮 + 微电火花 + 细泡亮点） ===
        // 目标：更干净的层级与动势，减少杂乱；以“向上漂+微电跃迁”为主基调
        private void SpawnChargeFX_EveryFrame(Vector2 pos)
        {
            // ① 方框水汽（WaterCandle Dust）：柔和上浮，像近口的潮湿水雾
            Vector2 boxCenter = pos + new Vector2(0f, 6f);
            float halfW = 9f, halfH = 3.5f;
            int mistCount = Main.rand.Next(5, 8); // 中等密度（保持整洁）
            for (int i = 0; i < mistCount; i++)
            {
                Vector2 spawn = boxCenter + new Vector2(Main.rand.NextFloat(-halfW, halfW), Main.rand.NextFloat(-halfH, halfH));
                Vector2 vel = new Vector2(Main.rand.NextFloat(-0.18f, 0.18f), Main.rand.NextFloat(-1.25f, -0.8f));
                int d = Dust.NewDust(spawn, 0, 0, DustID.WaterCandle, vel.X, vel.Y);
                Main.dust[d].noGravity = true;
                Main.dust[d].scale = Main.rand.NextFloat(0.85f, 1.05f);
                Main.dust[d].color = Color.Lerp(Color.CornflowerBlue, Color.LightBlue, Main.rand.NextFloat(0.35f, 0.85f));
            }

            // ② 低频小电花（CritSpark）：轻触即逝，避免视觉噪声
            if (Main.rand.NextBool(1, 5))
            {
                // 小幅抖动方向，速度偏低，制造“电晕”感
                Vector2 j = Main.rand.NextVector2Circular(0.6f, 0.6f);
                CritSpark spark = new CritSpark(
                    pos + j,                                   // 位置略抖动
                    j * Main.rand.NextFloat(2.0f, 3.2f),       // 速度很小
                    Color.White,                               // 起始白亮
                    Color.LightBlue,                           // 过渡水蓝
                    Main.rand.NextFloat(0.8f, 1.05f),          // 缩放很小
                    Main.rand.Next(10, 16)                     // 寿命较短
                );
                GeneralParticleHandler.SpawnParticle(spark);
            }

            // ③ 微亮泡沫（GlowOrbParticle）：极低频闪一下，像泡沫折光
            if (Main.rand.NextBool(1, 8))
            {
                GlowOrbParticle orb = new GlowOrbParticle(
                    pos + Main.rand.NextVector2Circular(4f, 4f),
                    Vector2.Zero,
                    false,
                    6,
                    0.85f,
                    Color.Lerp(Color.LightCyan, Color.White, 0.5f),
                    true,   // additive
                    false,
                    true
                );
                GeneralParticleHandler.SpawnParticle(orb);
            }
        }



        private void SpawnChargeFX_Complete(Vector2 pos)
        {
            // 旋涡参数：环绕 + 轻微脉动
            float t = Main.GlobalTimeWrappedHourly;
            float baseAngle = t * MathHelper.TwoPi * 0.9f;        // 环绕速度
            float radius = 12f + (float)Math.Sin(t * 4.5f) * 2f;  // 半径轻微呼吸

            // 环绕小漩涡（Dust）：水电混合，贴切向滑动
            for (int i = 0; i < 3; i++)
            {
                float ang = baseAngle + i * (MathHelper.TwoPi / 3f);
                Vector2 ringPos = pos + ang.ToRotationVector2() * radius;
                Vector2 tangent = ang.ToRotationVector2().RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(0.3f, 0.8f);

                int type = (i % 2 == 0) ? DustID.UnusedWhiteBluePurple : DustID.Electric;
                int d = Dust.NewDust(ringPos, 0, 0, type, tangent.X, tangent.Y);
                Main.dust[d].noGravity = true;
                Main.dust[d].scale = Main.rand.NextFloat(0.9f, 1.25f);
            }

            // 内核上浮（Dust，低频，柔和）
            if (Main.rand.NextBool(3))
            {
                Vector2 vel = new Vector2(0f, -0.35f) + Main.rand.NextVector2Circular(0.2f, 0.2f);
                int d2 = Dust.NewDust(pos, 0, 0, DustID.UnusedWhiteBluePurple, vel.X, vel.Y);
                Main.dust[d2].noGravity = true;
                Main.dust[d2].scale = Main.rand.NextFloat(1.0f, 1.3f);
            }

            // 十字星电花（CritSpark）：沿切向轻拂，频率适中更和谐
            if (Main.rand.NextBool(3))
            {
                float ang = baseAngle + Main.rand.NextFloat(-0.15f, 0.15f);
                Vector2 ringPos = pos + ang.ToRotationVector2() * (radius + Main.rand.NextFloat(-2f, 2f));
                Vector2 tangent = ang.ToRotationVector2().RotatedBy(MathHelper.PiOver2);

                const float maxAngle = MathHelper.Pi / 8f; // 22.5°
                Vector2 sparkVel = tangent.RotatedBy(Main.rand.NextFloat(-maxAngle, maxAngle)) * Main.rand.NextFloat(3.2f, 5.0f);

                CritSpark spark = new CritSpark(
                    ringPos,
                    sparkVel,
                    Color.White,
                    Color.LightBlue,
                    Main.rand.NextFloat(0.9f, 1.2f),
                    Main.rand.Next(18, 26)
                );
                GeneralParticleHandler.SpawnParticle(spark);
            }
        }


        // === 满蓄瞬间·一次性爆发（海电共鸣：椭圆冲击 + 泡沫光环 + 水尘扇弧 + 电火花脉冲） ===
        // 结构分层：
        // A 冲击波（DirectionalPulseRing）→ 给“主语”；
        // B 泡沫光环（GlowOrb）→ 给“几何锚点”；
        // C 水尘扇弧 + 电尘尖束（Dust）→ 给“体量与动势”；
        // D 电火花（CritSpark）→ 给“锐度与脉冲”。
        // 这样 Dust 不再是一锅乱，而是按扇弧/锥束组织，Spark/Orb/Pulse起到结构化作用。
        private void SpawnChargeReadyOnceFX(Vector2 pos)
        {
            // 基向量
            Vector2 f = Projectile.velocity.LengthSquared() > 1e-4f
                ? Vector2.Normalize(Projectile.velocity)
                : Vector2.UnitX.RotatedBy(Projectile.rotation);
            Vector2 n = new Vector2(-f.Y, f.X);

            // === A) 椭圆冲击波（定向） ===
            // 轻微拉长并按武器朝向旋转，颜色是蓝白电光
            Particle pulse = new DirectionalPulseRing(
                pos,
                f * 0.75f,                                       // 传播方向/速度
                Color.Lerp(Color.White, Color.Cyan, 0.65f),      // 蓝白电光
                new Vector2(1.15f, 2.4f),                        // 椭圆纵向更长
                Projectile.rotation - MathHelper.PiOver4,        // 让椭圆略倾斜，更“电”
                0.22f,                                           // 初始尺度
                0.04f,                                           // 扩张速率
                28                                               // 寿命
            );
            GeneralParticleHandler.SpawnParticle(pulse);

            // === B) 泡沫光环（椭圆环的“几何锚点”） ===
            // 椭圆参数（与A一致但略小），让结构更可读
            int orbSeg = 20;
            float a = 46f, b = 22f; // 椭圆长短轴
            for (int i = 0; i < orbSeg; i++)
            {
                float t = MathHelper.TwoPi * i / orbSeg;
                Vector2 off = f * (a * (float)Math.Cos(t)) + n * (b * (float)Math.Sin(t));
                GlowOrbParticle orb = new GlowOrbParticle(
                    pos + off,
                    Vector2.Zero,
                    false,
                    8,
                    Main.rand.NextFloat(0.9f, 1.2f),
                    Color.Lerp(Color.LightCyan, Color.White, Main.rand.NextFloat(0.25f, 0.65f)),
                    true,
                    false,
                    true
                );
                GeneralParticleHandler.SpawnParticle(orb);
            }

            // === C1) 水尘扇弧（前向“水花扇面”） ===
            // 两翼扇弧（±24°），只发水系 Dust，速度成扇形外抛，减少全向随机带来的噪声
            int waterCount = 120;
            float arc = MathHelper.ToRadians(24f);
            for (int i = 0; i < waterCount; i++)
            {
                // 左/右扇弧各一半
                float side = (i % 2 == 0) ? -1f : 1f;
                float ang = side * Main.rand.NextFloat(0f, arc);
                Vector2 dir = f.RotatedBy(ang);
                Vector2 vel = dir * Main.rand.NextFloat(7f, 13f) + n * Main.rand.NextFloat(-0.6f, 0.6f);

                int d = Dust.NewDust(pos, 0, 0, DustID.WaterCandle, vel.X, vel.Y);
                Dust dd = Main.dust[d];
                dd.noGravity = true;
                dd.scale = Main.rand.NextFloat(1.0f, 1.35f);
                dd.color = Color.Lerp(Color.CornflowerBlue, Color.LightBlue, Main.rand.NextFloat(0.35f, 0.9f));
            }

            // === C2) 电尘尖束（夹在扇弧内的“电尖”） ===
            // 更窄角度（±12°），速度更快一点，和水尘形成动势对比
            int elecCount = 60;
            float narrow = MathHelper.ToRadians(12f);
            for (int i = 0; i < elecCount; i++)
            {
                float ang = Main.rand.NextFloat(-narrow, narrow);
                Vector2 dir = f.RotatedBy(ang);
                Vector2 vel = dir * Main.rand.NextFloat(9f, 15f);

                int d = Dust.NewDust(pos, 0, 0, DustID.Electric, vel.X, vel.Y);
                Dust ed = Main.dust[d];
                ed.noGravity = true;
                ed.scale = Main.rand.NextFloat(0.95f, 1.25f);
                // Electric 自带色彩，这里不额外上色以免发白糊
            }

            // === D) 电火花脉冲（CritSpark，沿椭圆切向） ===
            // 挑若干点，速度沿椭圆切向发散，像“电在环上跳跃”
            int sparkCount = 18;
            for (int i = 0; i < sparkCount; i++)
            {
                float t = MathHelper.TwoPi * i / sparkCount + Main.rand.NextFloat(-0.08f, 0.08f);
                // 椭圆上一点
                Vector2 p = pos + f * (a * (float)Math.Cos(t)) + n * (b * (float)Math.Sin(t));
                // 椭圆切向（对 cos/sin 求导后的方向，再正交化）：(-a sin t, b cos t) 映射到 f/n 基
                Vector2 tangent = f * (-a * (float)Math.Sin(t)) + n * (b * (float)Math.Cos(t));
                tangent = tangent.SafeNormalize(Vector2.UnitX) * Main.rand.NextFloat(3.2f, 5.0f);

                CritSpark spark = new CritSpark(
                    p,
                    tangent,
                    Color.White,
                    Color.LightCyan,
                    Main.rand.NextFloat(0.9f, 1.15f),
                    Main.rand.Next(14, 20)
                );
                GeneralParticleHandler.SpawnParticle(spark);
            }
        }











        private void SpawnPerShotFX(Vector2 pos, Vector2 vel)
        {
            // 基础参数
            Vector2 dir = vel.SafeNormalize(Vector2.UnitX);
            Vector2 backDir = -dir; // 反方向

            // 反方向释放裂纹闪电
            CrackParticle crack = new CrackParticle(
                pos,
                backDir * 2f,                                // 反方向轻微漂移
                Color.Cyan,                                  // 电光蓝色
                new Vector2(0.5f, 0.5f),                     // 更小的缩放比例
                Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi),
                0.35f,                                       // 初始很小
                0.8f,                                        // 最终也小
                20                                           // 生命周期缩短
            );
            GeneralParticleHandler.SpawnParticle(crack);

            // 反方向喷射的电光 Dust
            for (int i = 0; i < 4; i++)
            {
                Vector2 dustVel = backDir.RotatedByRandom(0.35f) * Main.rand.NextFloat(1.2f, 2.5f);
                int d = Dust.NewDust(pos, 0, 0, DustID.Electric, dustVel.X, dustVel.Y);
                Dust dust = Main.dust[d];
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.6f, 0.9f); // 很小的 Dust
            }
        }


      




    }
}
