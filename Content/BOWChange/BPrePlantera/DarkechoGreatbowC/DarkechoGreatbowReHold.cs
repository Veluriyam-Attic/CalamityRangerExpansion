using CalamityRangerExpansion.Content.BOWChange.BPrePlantera.DarkechoGreatbowC;
using CalamityMod;
using CalamityRangerExpansion.Content.BOWChange;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityRangerExpansion.Content.BOWChange.BPrePlantera.DarkechoGreatbowC
{
    public class DarkechoGreatbowReHold : BaseGunHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.BPrePlantera";
        public override string Texture => "CalamityRangerExpansion/Content/BOWChange/BPrePlantera/DarkechoGreatbowC/DarkechoGreatbowRe";
        public override int AssociatedItemID => ModContent.ItemType<DarkechoGreatbowRe>();

        public override float MaxOffsetLengthFromArm => 15f;
        public override bool? CanDamage() => Main.player[Projectile.owner].GetModPlayer<OpenBowDamagePlayer>().OpenBowDamage;
        public override Vector2 GunTipPosition => Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * (Projectile.width * 0.5f + 10f);

        private ref float ChargeFrames => ref Projectile.ai[0];
        private ref float ShotsRemaining => ref Projectile.ai[1];
        private int readyDustTicker = 0;
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;

            // ========= 基础数据 =========
            Vector2 center = GunTipPosition - Main.screenPosition;
            Vector2 forward = Projectile.rotation.ToRotationVector2();

            Texture2D mainCrystalTex =
                Terraria.GameContent.TextureAssets.Projectile[
                    ModContent.ProjectileType<DarkechoGreatbowRePROJ>()
                ].Value;

            Texture2D dartTex =
                Terraria.GameContent.TextureAssets.Projectile[
                    ModContent.ProjectileType<DarkechoGreatbowReDarts>()
                ].Value;

            Vector2 mainOrigin = mainCrystalTex.Size() * 0.5f;
            Vector2 dartOrigin = dartTex.Size() * 0.5f;

            // ========= 参数 =========
            const int mainLoadTime = 45;
            const int dartCount = 6;
            const int dartLoadTime = 4;
            const float mainStartDist = 36f;
            const float dartOuterRadius = 28f;

            int totalLoadTime = mainLoadTime + dartCount * dartLoadTime;

            // ========= Ease 函数（iOS 风格） =========
            float EaseOut(float t) => 1f - MathF.Pow(1f - t, 3.5f);
            float EaseInOut(float t) =>
                t < 0.5f
                    ? 0.5f * MathF.Pow(t * 2f, 2.5f)
                    : 1f - 0.5f * MathF.Pow((1f - t) * 2f, 2.5f);

            // =====================================================
            // 【阶段 A】大水晶刺装填
            Vector2 mainPos = center;

            float loadTimer = ChargeFrames; // ?? 统一使用现有的蓄力计时


            if (loadTimer < mainLoadTime)
            {
                float t = loadTimer / (float)mainLoadTime;
                float eased = EaseOut(t);
                float dist = MathHelper.Lerp(mainStartDist, 0f, eased);
                mainPos += forward * dist;
            }

            // ——主体绘制——
            sb.Draw(
                mainCrystalTex,
                mainPos,
                null,
                Color.White,
                Projectile.rotation,
                mainOrigin,
                1f,
                SpriteEffects.None,
                0f
            );

            // ——到位描边闪光——
            int outlineFlashTime = (int)(loadTimer - mainLoadTime);
            if (outlineFlashTime >= 0 && outlineFlashTime < 6)
            {
                float flashT = 1f - outlineFlashTime / 6f;
                float outlineScale = 1.15f + flashT * 0.35f;
                Color outlineColor = Color.White * flashT;
                outlineColor.A = 0;

                sb.Draw(
                    mainCrystalTex,
                    mainPos,
                    null,
                    outlineColor,
                    Projectile.rotation,
                    mainOrigin,
                    outlineScale,
                    SpriteEffects.None,
                    0f
                );
            }

            // =====================================================
            // 【阶段 B】分裂水晶镖装填
            for (int i = 0; i < dartCount; i++)
            {
                int dartStart = mainLoadTime + i * dartLoadTime;
                float localT = (loadTimer - dartStart) / (float)dartLoadTime;
                if (localT <= 0f)
                    continue;

                localT = MathHelper.Clamp(localT, 0f, 1f);

                float eased = EaseInOut(localT);
                float radius = MathHelper.Lerp(dartOuterRadius, 0f, eased);

                float angle = MathHelper.TwoPi * i / dartCount;
                Vector2 offset = angle.ToRotationVector2() * radius;

                sb.Draw(
                    dartTex,
                    mainPos + offset,
                    null,
                    Color.White,
                    Projectile.rotation,
                    dartOrigin,
                    1f,
                    SpriteEffects.None,
                    0f
                );
            }

            return false;
        }

        public override void AI()
        {
            KeepRefreshingLifetime = true;
            base.AI();
        }
        private bool hasTriggeredReadyFX = false;



        const int MainLoadTime = 45;
        const int DartCount = 6;
        const int DartInterval = 4;
        const int TotalLoadTime = MainLoadTime + DartCount * DartInterval; // 69

        public override void HoldoutAI()
        {
            Player player = Owner;

            if (!player.channel)
                return;

            // ===== 统一推进时间轴 =====
            ChargeFrames++;

            Vector2 pos = GunTipPosition;

            // =========================
            // 阶段 A：前 45 帧（持续吸收，每一帧）
            if (ChargeFrames < MainLoadTime)
            {
                BowChangeVFX.SpawnCharge(Projectile, pos, BowChangeTheme.Darkecho, ChargeFrames / TotalLoadTime, 0.65f);
                SpawnChargeFX_AbsorbSoft(pos);
                return;
            }

            // =========================
            // 阶段 B：45 ~ 69（每 4 帧一次“强吸”，共 6 次）
            if (ChargeFrames < TotalLoadTime)
            {
                BowChangeVFX.SpawnCharge(Projectile, pos, BowChangeTheme.Darkecho, ChargeFrames / TotalLoadTime, 0.75f);
                SpawnChargeFX_AbsorbSoft(pos);

                int local = (int)(ChargeFrames - MainLoadTime);

                // 每 4 帧触发一次
                if (local % DartInterval == 0)
                {
                    int index = local / DartInterval; // 0~5
                    if (index < DartCount)
                        SpawnChargeFX_AbsorbPulse(pos, index);
                }

                return;
            }

            // =========================
            // 阶段 C：只触发一次的大收尾
            if (!hasTriggeredReadyFX)
            {
                BowChangeVFX.SpawnReadyBurst(Projectile, pos, BowChangeTheme.Darkecho, 0.9f);
                SpawnChargeFX_FinalCollapse(pos);
                hasTriggeredReadyFX = true;
            }

            // 69 帧之后保持“已装填稳态”，不再刷新特效
        }

        public override void KillHoldoutLogic()
        {
            Player player = Owner;

            // 玩家已经松手（核心入口）
            if (!player.channel)
            {
                // 满蓄：允许结算发射
                if (ShotsRemaining <= 0 && ChargeFrames >= TotalLoadTime)
                {
                    float progress = MathHelper.Clamp(ChargeFrames / (float)TotalLoadTime, 0f, 1f);

                    Item heldItem = player.HeldItem;
                    if (player.HasAmmo(heldItem) &&
                        player.PickAmmo(heldItem, out int ammoProj, out _, out _, out _, out _))
                    {
                        Vector2 shootDir = Projectile.velocity.SafeNormalize(Vector2.UnitX * player.direction);

                        Projectile.NewProjectile(
                            Projectile.GetSource_FromThis(),
                            GunTipPosition,
                            shootDir * 16f,
                            ModContent.ProjectileType<DarkechoGreatbowRePROJ>(),
                            (int)(Projectile.damage * MathHelper.Lerp(0.5f, 1f, progress)),
                            Projectile.knockBack * progress,
                            player.whoAmI,
                            ai0: progress,
                            ai1: ammoProj
                        );

                        SoundEngine.PlaySound(SoundID.Item93 with { Volume = 0.7f }, player.Center);
                        SpawnPerShotFX(GunTipPosition, shootDir * 16f);
                        BowChangeVFX.SpawnMuzzle(Projectile, GunTipPosition, shootDir * 16f, BowChangeTheme.Darkecho, 0.9f);
                    }

                    Projectile.Kill();
                    return;
                }

                // 兜底：快速点按 / 未达条件 —— 直接消失
                Projectile.Kill();
            }
        }



        // 软吸收：细小晶尘被拉向核心
        private void SpawnChargeFX_AbsorbSoft(Vector2 pos)
        {
            for (int i = 0; i < 6; i++)
            {
                Vector2 from = pos + Main.rand.NextVector2Circular(22f, 12f);
                Vector2 dir = (pos - from).SafeNormalize(Vector2.Zero);

                Dust d = Dust.NewDustPerfect(
                    from,
                    DustID.PurpleCrystalShard,
                    dir * Main.rand.NextFloat(1.6f, 2.4f),
                    140,
                    Color.Lerp(Color.MediumPurple, Color.DeepSkyBlue, 0.4f),
                    Main.rand.NextFloat(0.75f, 1.0f)
                );
                d.noGravity = true;
            }
        }


        // 强吸收：一次明确的“拉扯感”
        private void SpawnChargeFX_AbsorbPulse(Vector2 pos, int index)
        {
            int count = 24 + index * 4; // 越往后越强

            for (int i = 0; i < count; i++)
            {
                float ang = MathHelper.TwoPi * i / count;
                Vector2 from = pos + ang.ToRotationVector2() * Main.rand.NextFloat(26f, 36f);
                Vector2 dir = (pos - from).SafeNormalize(Vector2.Zero);

                Dust d = Dust.NewDustPerfect(
                    from,
                    DustID.BlueTorch,
                    dir * Main.rand.NextFloat(3.5f, 6.5f),
                    120,
                    Color.Lerp(Color.LightCyan, Color.MediumPurple, 0.5f),
                    Main.rand.NextFloat(1.0f, 1.25f)
                );
                d.noGravity = true;
            }
        }
        // 最终坍缩：所有能量被瞬间压入核心
        private void SpawnChargeFX_FinalCollapse(Vector2 pos)
        {
            int ring = 48;

            for (int i = 0; i < ring; i++)
            {
                float ang = MathHelper.TwoPi * i / ring;
                Vector2 dir = ang.ToRotationVector2();

                Dust d = Dust.NewDustPerfect(
                    pos + dir * Main.rand.NextFloat(30f, 42f),
                    DustID.PurpleTorch,
                    -dir * Main.rand.NextFloat(6f, 10f),
                    90,
                    Color.White,
                    Main.rand.NextFloat(1.3f, 1.6f)
                );
                d.noGravity = true;
            }

            // 可选：这里你要不要接一个光点外包，我没强行加
        }


        private void SpawnPerShotFX(Vector2 pos, Vector2 vel)
        {
            for (int i = 0; i < 3; i++)
            {
                Vector2 spawn = pos + Main.rand.NextVector2Circular(4f, 4f);
                Vector2 v = vel.RotatedByRandom(0.1f) * Main.rand.NextFloat(0.2f, 0.6f);

                Dust d = Dust.NewDustPerfect(spawn, DustID.Electric, v, 100, Color.MediumPurple, 1.3f);
                d.noGravity = true;
            }
        }


        //// === 蓄力期·持续（紫蓝水晶·静谧聚能）===
        //// 规则：只用 Dust / GlowOrb / Crack（体积极小）；禁止调用光点外包
        //private void SpawnChargeFX_EveryFrame(Vector2 pos)
        //{
        //    // A) 近场细腻晶尘（PurpleTorch + BlueTorch）——从方框轻轻上浮
        //    Vector2 boxC = pos + new Vector2(0f, 6f);
        //    float halfW = 8f, halfH = 3f;
        //    int dustCount = Main.rand.Next(5, 8);
        //    for (int i = 0; i < dustCount; i++)
        //    {
        //        Vector2 spawn = boxC + new Vector2(Main.rand.NextFloat(-halfW, halfW), Main.rand.NextFloat(-halfH, halfH));
        //        Vector2 vel = new Vector2(Main.rand.NextFloat(-0.18f, 0.18f), Main.rand.NextFloat(-1.1f, -0.7f));
        //        int type = Main.rand.NextBool(2) ? DustID.PurpleTorch : DustID.BlueTorch;
        //        int id = Dust.NewDust(spawn, 0, 0, type, vel.X, vel.Y, 130);
        //        Dust d = Main.dust[id];
        //        d.noGravity = true;
        //        d.scale = Main.rand.NextFloat(0.85f, 1.05f);
        //        d.color = (type == DustID.PurpleTorch)
        //            ? Color.Lerp(Color.MediumPurple, Color.Violet, Main.rand.NextFloat(0.3f, 0.8f))
        //            : Color.Lerp(Color.RoyalBlue, Color.LightBlue, Main.rand.NextFloat(0.3f, 0.8f));
        //        d.fadeIn = 1.05f;
        //    }

        //    // B) 极小“裂纹电弧”偶发（CrackParticle）——体积必须很小
        //    if (Main.rand.NextBool(1, 6))
        //    {
        //        var crack = new CrackParticle(
        //            pos + Main.rand.NextVector2Circular(3f, 3f),
        //            new Vector2(0f, -0.6f),                      // 轻微上漂
        //            Color.Lerp(Color.MediumPurple, Color.DeepSkyBlue, 0.35f),
        //            new Vector2(1f, 1f),
        //            Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi),
        //            Main.rand.NextFloat(0.05f, 0.08f),          // ? 初始极小
        //            0.20f,                                      // 终值也小，避免喧宾
        //            18                                          // 短寿命
        //        );
        //        GeneralParticleHandler.SpawnParticle(crack);
        //    }

        //    // C) 点状晶光——极低频的小辉光球
        //    if (Main.rand.NextBool(1, 10))
        //    {
        //        var orb = new GlowOrbParticle(
        //            pos + Main.rand.NextVector2Circular(4f, 4f),
        //            Vector2.Zero,
        //            false,
        //            6,
        //            0.85f,
        //            Color.Lerp(Color.LightCyan, Color.MediumPurple, 0.35f),
        //            true, false, true
        //        );
        //        GeneralParticleHandler.SpawnParticle(orb);
        //    }
        //}



        //// === 满蓄期·持续（暗回响·晶流环绕）===
        //// 规则：可少量调用“光点外包”（持续）+ 少量 Dust + 小号裂纹；整体克制
        //private void SpawnChargeFX_Complete(Vector2 pos)
        //{
        //    Vector2 f = Projectile.velocity.LengthSquared() > 1e-4f
        //        ? Vector2.Normalize(Projectile.velocity)
        //        : Vector2.UnitX.RotatedBy(Projectile.rotation);
        //    Vector2 n = new Vector2(-f.Y, f.X);

        //    // A) 前向稀疏“水晶碎屑”外抛（PurpleCrystalShard）
        //    int shard = Main.rand.Next(2, 4);
        //    for (int i = 0; i < shard; i++)
        //    {
        //        float ang = Main.rand.NextFloat(-0.22f, 0.22f);
        //        Vector2 dir = f.RotatedBy(ang);
        //        Vector2 vel = dir * Main.rand.NextFloat(1.8f, 3.2f) + n * Main.rand.NextFloat(-0.4f, 0.4f);
        //        Dust d = Dust.NewDustPerfect(pos, DustID.PurpleCrystalShard, vel, 140,
        //            Color.Lerp(Color.MediumPurple, Color.RoyalBlue, Main.rand.NextFloat(0.25f, 0.7f)),
        //            Main.rand.NextFloat(1.0f, 1.25f));
        //        d.noGravity = true;
        //    }

        //    // B) 小裂纹沿切向闪一下（Crack，小体积）
        //    if (Main.rand.NextBool(1, 4))
        //    {
        //        var crack = new CrackParticle(
        //            pos + n * Main.rand.NextFloat(-6f, 6f),
        //            n * Main.rand.NextFloat(-0.4f, 0.4f),
        //            Color.Lerp(Color.DeepSkyBlue, Color.MediumPurple, 0.5f),
        //            new Vector2(1f, 1f),
        //            Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi),
        //            Main.rand.NextFloat(0.05f, 0.07f),
        //            0.18f,
        //            16
        //        );
        //        GeneralParticleHandler.SpawnParticle(crack);
        //    }

        //    // C) ?调用“光点外包”（持续漂移）：只在满蓄阶段使用，制造神秘晶光环绕
        //    CCBLightingBoltsSystem.Spawn_DarkechoSustainCrystals(pos, f, 0.9f);
        //}



        //// === 满蓄瞬间·一次性大爆发（暗之回响：晶环 + 裂闪 + 扇束）===
        //// 规则：可使用光点外包（一次性向四周扩散）；Dust 主体做体量，环形 GlowOrb 给形状，小 Crack 给锐度
        //private void SpawnChargeReadyOnceFX(Vector2 pos)
        //{
        //    Vector2 f = Projectile.velocity.LengthSquared() > 1e-4f
        //        ? Vector2.Normalize(Projectile.velocity)
        //        : Vector2.UnitX.RotatedBy(Projectile.rotation);
        //    Vector2 n = new Vector2(-f.Y, f.X);

        //    // A) 环形辉光（GlowOrb）——暗回响的“几何锚点”，两道错相细环
        //    int ringPts = 18;
        //    float a1 = 38f, b1 = 20f;     // 椭圆1
        //    float a2 = 48f, b2 = 26f;     // 椭圆2
        //    for (int i = 0; i < ringPts; i++)
        //    {
        //        float t = MathHelper.TwoPi * i / ringPts;
        //        Vector2 off1 = f * (a1 * (float)Math.Cos(t)) + n * (b1 * (float)Math.Sin(t));
        //        Vector2 off2 = f * (a2 * (float)Math.Cos(t + 0.25f)) + n * (b2 * (float)Math.Sin(t + 0.25f));

        //        var orb1 = new GlowOrbParticle(pos + off1, Vector2.Zero, false, 8,
        //            Main.rand.NextFloat(0.85f, 1.1f),
        //            Color.Lerp(Color.LightCyan, Color.MediumPurple, 0.35f), true, false, true);
        //        var orb2 = new GlowOrbParticle(pos + off2, Vector2.Zero, false, 8,
        //            Main.rand.NextFloat(0.8f, 1.0f),
        //            Color.Lerp(Color.RoyalBlue, Color.MediumPurple, 0.25f), true, false, true);
        //        GeneralParticleHandler.SpawnParticle(orb1);
        //        GeneralParticleHandler.SpawnParticle(orb2);
        //    }

        //    // B) 扇束晶尘（前向 ±24°）：主量体（PurpleCrystalShard + BlueTorch）
        //    int dustCount = 120;
        //    float arc = MathHelper.ToRadians(24f);
        //    for (int i = 0; i < dustCount; i++)
        //    {
        //        float ang = Main.rand.NextFloat(-arc, arc);
        //        Vector2 dir = f.RotatedBy(ang);
        //        Vector2 vel = dir * Main.rand.NextFloat(7f, 12f) + n * Main.rand.NextFloat(-0.6f, 0.6f);

        //        int type = (i % 3 == 0) ? DustID.PurpleCrystalShard : DustID.BlueTorch;
        //        Dust d = Dust.NewDustPerfect(pos, type, vel, 140,
        //            (type == DustID.PurpleCrystalShard)
        //                ? Color.Lerp(Color.MediumPurple, Color.Violet, Main.rand.NextFloat(0.3f, 0.8f))
        //                : Color.Lerp(Color.RoyalBlue, Color.LightBlue, Main.rand.NextFloat(0.3f, 0.8f)),
        //            Main.rand.NextFloat(1.0f, 1.35f));
        //        d.noGravity = true;
        //    }

        //    // C) 小裂纹尖束（Crack，沿前向内侧抖动）
        //    int cracks = 24;
        //    for (int i = 0; i < cracks; i++)
        //    {
        //        float ang = Main.rand.NextFloat(-0.18f, 0.18f);
        //        Vector2 dir = f.RotatedBy(ang);
        //        Vector2 p = pos + dir * Main.rand.NextFloat(8f, 18f) + n * Main.rand.NextFloat(-6f, 6f);
        //        var crack = new CrackParticle(
        //            p,
        //            dir * Main.rand.NextFloat(0.6f, 1.4f),
        //            Color.Lerp(Color.LightCyan, Color.MediumPurple, 0.5f),
        //            new Vector2(1f, 1f),
        //            Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi),
        //            Main.rand.NextFloat(0.05f, 0.08f),      // ? 极小
        //            0.22f,                                  // 终值也不大
        //            20
        //        );
        //        GeneralParticleHandler.SpawnParticle(crack);
        //    }

        //    // D) ?一次性调用“光点外包”（四周优雅扩散的神秘晶光）
        //    CCBLightingBoltsSystem.Spawn_DarkechoBurstCrystals(pos);
        //}




    }
}
