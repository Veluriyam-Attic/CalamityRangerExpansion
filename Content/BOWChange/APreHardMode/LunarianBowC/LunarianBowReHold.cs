using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityRangerExpansion.Content.BOWChange;
using CalamityRangerExpansion.Content.BOWChange.APreHardMode.LunarianBowC;
using CalamityMod;
using System.Collections.Generic;
using CalamityMod.Particles;

namespace CalamityRangerExpansion.Content.BOWChange.APreHardMode.LunarianBowC
{
    public class LunarianBowReHold : BaseGunHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.APreHardMode";

        public override string Texture => "CalamityRangerExpansion/Content/BOWChange/APreHardMode/LunarianBowC/LunarianBowRe";
        public override int AssociatedItemID => ModContent.ItemType<LunarianBowRe>();
        public override float MaxOffsetLengthFromArm => 20f;

        public override Vector2 GunTipPosition => Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * (Projectile.width * 0.5f + 10f);


        private int readyDustTicker = 0;
        private int shotTimer = 0;
        private const int shotCooldown = 3;

        private int currentShotIndex = 0;
        private List<float> shotAngles = new();

        private SoundStyle fireSound = SoundID.Item5;
        protected ref float ChargeFrames => ref Projectile.ai[0];
        protected ref float ShotsRemaining => ref Projectile.ai[1];

        public override void AI()
        {
            KeepRefreshingLifetime = true;
            base.AI();
        }

        public override void HoldoutAI()
        {
            Player player = Owner;

            if (player.channel)
            {
                if (ChargeFrames < 90)
                {
                    ChargeFrames++;
                    BowChangeVFX.SpawnCharge(Projectile, GunTipPosition, BowChangeTheme.Lunar, ChargeFrames / 90f, 0.7f);
                    SpawnChargeFX_EveryFrame(GunTipPosition);
                }
                else
                {
                    readyDustTicker++;

                    // 第一次满蓄 → 一次性魔法阵特效
                    if (readyDustTicker == 1)
                    {
                        BowChangeVFX.SpawnReadyBurst(Projectile, GunTipPosition, BowChangeTheme.Lunar, 0.85f);
                        SpawnChargeReadyOnceFX(GunTipPosition);
                    }

                    BowChangeVFX.SpawnCharge(Projectile, GunTipPosition, BowChangeTheme.Lunar, 1f, 0.55f);
                    SpawnChargeFX_Complete(GunTipPosition);
                }
            }
        }

        public override void KillHoldoutLogic()
        {
            Player player = Owner;

            if (Owner.CantUseHoldout())
            {
                // 防止太短的蓄力导致的快速连射[这个几乎是必要的]
                if (ChargeFrames < 90)
                {
                    Projectile.Kill();
                    return;
                }

                if (ShotsRemaining <= 0 && ChargeFrames > 0)
                {
                    float chargeRatio = MathHelper.Clamp(ChargeFrames / 90f, 0f, 1f);
                    ShotsRemaining = (chargeRatio >= 1f) ? 7 : 1;

                    // ?? 准备好扇形角度序列
                    shotAngles.Clear();
                    float baseAngle = -6f;   // 从 -6° 开始
                    float step = 2f;         // 每发只差 2°
                    for (int i = 0; i < ShotsRemaining; i++)
                        shotAngles.Add(baseAngle + step * i);

                    currentShotIndex = 0;
                }

                shotTimer++;
                if (ShotsRemaining > 0 && shotTimer >= shotCooldown)
                {
                    FireNextProjectile(player);
                    shotTimer = 0;
                    ShotsRemaining--;

                    if (ShotsRemaining <= 0)
                        Projectile.Kill();
                }
            }
        }

        private void FireNextProjectile(Player player)
        {
            float angle = shotAngles.Count > currentShotIndex ? shotAngles[currentShotIndex] : 0f;
            currentShotIndex++;

            Vector2 baseDir = Projectile.velocity.SafeNormalize(Vector2.UnitX * player.direction);
            Vector2 rotatedVel = baseDir.RotatedBy(MathHelper.ToRadians(angle)) * 12f;

            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                GunTipPosition,
                rotatedVel,
                ModContent.ProjectileType<LunarianBowRePROJ>(),
                Projectile.damage,
                Projectile.knockBack,
                player.whoAmI
            );

            SpawnPerShotFX(GunTipPosition, rotatedVel);
            BowChangeVFX.SpawnMuzzle(Projectile, GunTipPosition, rotatedVel, BowChangeTheme.Lunar, 0.7f);
            SoundEngine.PlaySound(fireSound, GunTipPosition);
        }













        // =========================
        //  小工具：统一放 Dust 组合（只用 UnusedWhiteBluePurple）
        // =========================
        private void SpawnDust_UBP(Vector2 pos, Vector2 vel, float scale, int count)
        {
            for (int i = 0; i < count; i++)
            {
                int d = Dust.NewDust(pos, 0, 0, DustID.UnusedWhiteBluePurple, vel.X, vel.Y, 0, default, scale);
                Dust dust = Main.dust[d];
                dust.noGravity = true;           // ? 按要求默认不受重力
                dust.velocity *= 0.9f;           // 稍收敛，避免爆散
            }
        }

        // ==========================================================
        //  1) 蓄力：每帧特效（按住且未满蓄）
        //     对应调用：SpawnChargeFX_EveryFrame(GunTipPosition)
        // ==========================================================
        private void SpawnChargeFX_EveryFrame(Vector2 pos)
        {
            // ——可调参数（小）——
            const float dustScale = 0.6f;
            const int dustCount = 1;
            const float arcRadius = 28f;    // 弧度半径
            const float arcSpan = MathHelper.PiOver4; // 弧度
            const float orbScale = 0.38f;
            const int orbLife = 18;
            // ————————————————

            // Dust：以弧形往内收缩
            float baseRot = Main.rand.NextFloat(-arcSpan * 0.5f, arcSpan * 0.5f);
            Vector2 offset = baseRot.ToRotationVector2() * arcRadius;
            Vector2 targetVel = (pos - (pos + offset)) * 0.08f; // 往中心收缩
            SpawnDust_UBP(pos + offset, targetVel, dustScale, dustCount);

            // 偶发辉光球：顺着弧线在内侧闪现
            if (Main.rand.NextBool(1))
            {
                Vector2 orbPos = pos + offset * Main.rand.NextFloat(0.3f, 0.6f);
                GlowOrbParticle orb = new GlowOrbParticle(
                    orbPos,
                    Vector2.Zero,
                    false,
                    orbLife,
                    orbScale,
                    new Color(200, 220, 255), // 偏冷的月光色
                    true,
                    false,
                    true
                );
                GeneralParticleHandler.SpawnParticle(orb);
            }
        }





        // ==========================================================
        //  3) 满蓄力：持续提示（达到阈值且仍按住）
        //     对应调用：SpawnChargeFX_Complete(GunTipPosition)
        // ==========================================================
        private void SpawnChargeFX_Complete(Vector2 pos)
        {
            // ——可调参数（小）——
            const float dustScale = 0.65f;
            const int dustCount = 2;
            const float upLift = -0.45f;
            const float orbScale = 0.40f;
            const int orbLife = 8;
            // ————————————————

            Vector2 dv = new Vector2(Main.rand.NextFloat(-0.25f, 0.25f), upLift + Main.rand.NextFloat(-0.08f, 0f));
            SpawnDust_UBP(pos, dv, dustScale, dustCount);

            if (Main.rand.NextBool(2))
            {
                var orb = new GlowOrbParticle(
                    pos + Main.rand.NextVector2Circular(4f, 4f),
                    Vector2.Zero, false, orbLife, orbScale,
                    new Color(190, 220, 255), true, false, true
                );
                GeneralParticleHandler.SpawnParticle(orb);
            }
        }


        // ==========================================================
        //  5) 发射瞬间：每发一次（含椭圆冲击波）
        //     对应调用：SpawnPerShotFX(GunTipPosition, rotatedVel)
        // ==========================================================
        private void SpawnPerShotFX(Vector2 pos, Vector2 shotVelocity)
        {
            // ——可调参数（小）——
            const float dustScale = 0.7f;
            const int dustBurst = 6;
            const float sparkScale = 1.0f;
            const int sparkLife = 26;
            Color pulseColor = new Color(180, 220, 255);
            Vector2 squish = new Vector2(0.85f, 2.0f);
            float pulseAngle = Projectile.rotation;
            float originalScale = 0.12f;  // 小
            float finalScale = 0.02f;  // 更小
            int pulseLife = 18;
            // ————————————————

            Vector2 dir = shotVelocity.SafeNormalize(Vector2.UnitX);

            // 椭圆冲击波
            Particle pulse = new DirectionalPulseRing(
                pos, dir * 0.75f, pulseColor, squish, pulseAngle,
                originalScale, finalScale, pulseLife
            );
            GeneralParticleHandler.SpawnParticle(pulse);

            // 射口小爆花 Dust
            for (int i = 0; i < dustBurst; i++)
            {
                Vector2 v = dir.RotatedByRandom(0.35f) * Main.rand.NextFloat(1.2f, 2.4f);
                SpawnDust_UBP(pos, v, dustScale, 1);
            }


        }



        private void SpawnChargeReadyOnceFX(Vector2 pos)
        {
            // 弹幕正前方方向
            float forwardAngle = Projectile.velocity.ToRotation();

            // ??? 柔和的月光 Dust（半圆）
            for (int i = 0; i < 180; i++)
            {
                // 以 forwardAngle 为中心，左右各偏 90°
                float angle = forwardAngle + Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);
                Vector2 vel = angle.ToRotationVector2() * Main.rand.NextFloat(6f, 14f);

                Dust d = Dust.NewDustPerfect(
                    pos,
                    DustID.WhiteTorch,
                    vel,
                    150,
                    Color.White,
                    Main.rand.NextFloat(1.0f, 1.5f)
                );
                d.noGravity = true;
            }

            // ?? 简洁的 GlowOrb 魔法阵（以前方为中心的半圆）
            int orbCount = 16;
            float radius = 60f;
            for (int i = 0; i < orbCount; i++)
            {
                // 半圆分布在 forwardAngle ±90°
                float angle = forwardAngle - MathHelper.PiOver2 + MathHelper.Pi * i / (orbCount - 1);
                Vector2 offset = angle.ToRotationVector2() * radius;

                var orb = new GlowOrbParticle(
                    pos + offset,
                    Vector2.Zero,
                    false,
                    14,
                    0.8f,
                    new Color(230, 230, 255), // 月光白
                    true,
                    false,
                    true
                );
                GeneralParticleHandler.SpawnParticle(orb);
            }

            // ? 月光折射：少量向前方半圆散射
            for (int i = 0; i < 30; i++)
            {
                float angle = forwardAngle + MathHelper.ToRadians(Main.rand.NextFloat(-90f, 90f));
                Vector2 dir = angle.ToRotationVector2();
                Particle trail = new SparkParticle(
                    pos,
                    dir * Main.rand.NextFloat(2f, 5f),
                    false,
                    50,
                    0.9f,
                    Color.LightGray
                );
                GeneralParticleHandler.SpawnParticle(trail);
            }
        }





    }
}
