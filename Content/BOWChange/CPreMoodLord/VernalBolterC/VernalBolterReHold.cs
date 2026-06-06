using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityRangerExpansion.Content.BOWChange.CPreMoodLord.VernalBolterC;
using System.Collections.Generic;
using CalamityRangerExpansion.Content.BOWChange;
using CalamityRangerExpansion.Content.BOWChange.APreHardMode.LunarianBowC;
using CalamityMod;
using CalamityMod.Particles;

namespace CalamityRangerExpansion.Content.BOWChange.CPreMoodLord.VernalBolterC
{
    public class VernalBolterReHold : BaseGunHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.CPreMoodLord";

        public override string Texture => "CalamityRangerExpansion/Content/BOWChange/CPreMoodLord/VernalBolterC/VernalBolterRe";
        public override int AssociatedItemID => ModContent.ItemType<VernalBolterRe>();
        public override float MaxOffsetLengthFromArm => 20f;

        public override Vector2 GunTipPosition =>
            Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * (Projectile.width * 0.5f + 10f);

        private ref float ChargeFrames => ref Projectile.ai[0];
        private ref float ShotsRemaining => ref Projectile.ai[1];

        private int shotTimer = 0;
        private int readyDustTicker = 0;
        private const int shotCooldown = 3;

        private List<float> fanAngles = new();
        private int currentShotIndex = 0;

        SoundStyle fireSound = new SoundStyle("CalamityRangerExpansion/Sound/SSL/空中分裂") { Volume = 1.2f };

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
                    if (ChargeFrames % 10 == 0)
                        SpawnChargeCircle(GunTipPosition);

                    BowChangeVFX.SpawnCharge(Projectile, GunTipPosition, BowChangeTheme.Vernal, ChargeFrames / 90f, 0.65f);
                    SpawnChargeFX_EveryFrame(GunTipPosition);
                }
                else
                {
                    // 第一次进入满蓄时释放一次性大特效
                    if (readyDustTicker == 0 && ShotsRemaining <= 0)
                    {
                        BowChangeVFX.SpawnReadyBurst(Projectile, GunTipPosition, BowChangeTheme.Vernal, 0.85f);
                        SpawnChargeReadyOnceFX(GunTipPosition);
                    }

                    readyDustTicker++;
                    if (readyDustTicker % 3 == 0 && ShotsRemaining <= 0)
                        SpawnReadyIdleDust(GunTipPosition);

                    BowChangeVFX.SpawnCharge(Projectile, GunTipPosition, BowChangeTheme.Vernal, 1f, 0.55f);
                    SpawnChargeFX_Complete(GunTipPosition);
                }
            }
        }

        public override void KillHoldoutLogic()
        {
            Player player = Owner;

            if (Owner.CantUseHoldout())
            {
                // 防止太短的蓄力导致的快速连胜
                if (ChargeFrames < 90)
                {
                    Projectile.Kill();
                    return;
                }


                if (ShotsRemaining <= 0 && ChargeFrames > 0)
                {
                    float chargeRatio = MathHelper.Clamp(ChargeFrames / 90f, 0f, 1f);

                    if (chargeRatio >= 1f)
                    {
                        ShotsRemaining = 14;
                        fanAngles.Clear();

                        // 双螺旋交错
                        fanAngles.AddRange(new float[] { -28f, -20f, -12f, -4f, 4f, 12f, 20f }); // 螺旋1
                        fanAngles.AddRange(new float[] { -24f, -16f, -8f, 0f, 8f, 16f, 24f });   // 螺旋2
                    }
                    else
                    {
                        ShotsRemaining = 1;
                        fanAngles.Clear();
                        fanAngles.Add(0f);
                    }

                    currentShotIndex = 0;
                }

                // 每几帧发一发
                shotTimer++;
                if (ShotsRemaining > 0 && shotTimer >= shotCooldown)
                {
                    shotTimer = 0;
                    FireNextProjectile(player);
                    ShotsRemaining--;

                    if (ShotsRemaining <= 0)
                        Projectile.Kill();
                }
            }
        }

        private void FireNextProjectile(Player player)
        {
            if (currentShotIndex >= fanAngles.Count)
                return;

            float angle = fanAngles[currentShotIndex];
            currentShotIndex++;

            Vector2 baseDir = Projectile.velocity.SafeNormalize(Vector2.UnitX * player.direction);
            Vector2 rotatedVel = baseDir.RotatedBy(MathHelper.ToRadians(angle)) * 9f;

            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                GunTipPosition,
                rotatedVel,
                ModContent.ProjectileType<VernalBolterRePROJ>(),
                Projectile.damage,
                Projectile.knockBack,
                player.whoAmI
            );

            SpawnPerShotFX(GunTipPosition, rotatedVel);
            BowChangeVFX.SpawnMuzzle(Projectile, GunTipPosition, rotatedVel, BowChangeTheme.Vernal, 0.8f);
            SoundEngine.PlaySound(fireSound, GunTipPosition);
        }







        // ?? 每一帧蓄力特效（翠绿色主题，翻倍狂野）
        private void SpawnChargeFX_EveryFrame(Vector2 pos)
        {
            // 基础绿 Dust
            for (int i = 0; i < 2; i++)
            {
                Dust d = Dust.NewDustPerfect(pos, DustID.UnusedWhiteBluePurple, Main.rand.NextVector2Circular(2.4f, 2.4f));
                d.noGravity = true;
                d.scale = 1.8f; // 比之前大一倍
            }

            // 荧光辉光球
            if (Main.rand.NextBool(1))
            {
                var orb = new GlowOrbParticle(
                    pos,
                    Vector2.Zero,
                    false,
                    10,
                    0.7f,
                    Color.LimeGreen, // 翠绿色
                    true,
                    false,
                    true
                );
                GeneralParticleHandler.SpawnParticle(orb);
            }
        }

        // ?? 达到满蓄后，每帧环绕能量（翻倍范围）
        private void SpawnChargeFX_Complete(Vector2 pos)
        {
            int points = 12;
            float radius = 24f; // 原来 12f，翻倍
            for (int i = 0; i < points; i++)
            {
                float angle = MathHelper.TwoPi * i / points;
                Vector2 offset = angle.ToRotationVector2() * radius;
                Dust d = Dust.NewDustPerfect(pos + offset, DustID.UnusedWhiteBluePurple, -offset.SafeNormalize(Vector2.UnitY) * 3f);
                d.noGravity = true;
                d.scale = 2.2f; // 更大
            }
        }

        // ?? 满蓄未发射时偶尔提示粒子（翠绿闪烁）
        private void SpawnReadyIdleDust(Vector2 pos)
        {
            Vector2 jitter = Main.rand.NextVector2Circular(2.4f, 2.4f); // 抖动范围翻倍
            Dust d = Dust.NewDustPerfect(pos + jitter, DustID.UnusedWhiteBluePurple);
            d.noGravity = true;
            d.scale = 1.4f;
            d.fadeIn = 1.5f;

            if (Main.rand.NextBool(4))
            {
                var orb = new GlowOrbParticle(
                    pos + jitter,
                    Vector2.Zero,
                    false,
                    12,
                    0.6f,
                    Color.SpringGreen,
                    true,
                    false,
                    true
                );
                GeneralParticleHandler.SpawnParticle(orb);
            }
        }

        // ?? 每次发射弹幕时的视觉特效（冲击波+更猛的爆裂）
        private void SpawnPerShotFX(Vector2 pos, Vector2 vel)
        {
            Vector2 dir = vel.SafeNormalize(Vector2.UnitX);

            // 椭圆冲击波（更大更亮）
            Particle pulse = new DirectionalPulseRing(
                pos,
                dir * 1.2f,
                Color.LightGreen,
                new Vector2(1.2f, 3.5f),
                Projectile.rotation,
                0.25f,
                0.05f,
                28
            );
            GeneralParticleHandler.SpawnParticle(pulse);

            // Dust 爆裂
            for (int i = 0; i < 12; i++)
            {
                Vector2 v = dir.RotatedByRandom(0.45f) * Main.rand.NextFloat(2f, 4f);
                Dust d = Dust.NewDustPerfect(pos, DustID.UnusedWhiteBluePurple, v);
                d.noGravity = true;
                d.scale = 1.8f;
            }
        }

        // ?? 蓄力阶段环形能量脉冲（翻倍规模，翠绿）
        private void SpawnChargeCircle(Vector2 pos)
        {
            int points = 16;
            float radius = 28f; // 原来 14f，翻倍
            for (int i = 0; i < points; i++)
            {
                float angle = MathHelper.TwoPi * i / points + Main.GlobalTimeWrappedHourly * 1.5f;
                Vector2 offset = angle.ToRotationVector2() * radius;
                Dust d = Dust.NewDustPerfect(pos + offset, DustID.UnusedWhiteBluePurple);
                d.noGravity = true;
                d.scale = 1.8f;
                d.fadeIn = 1.6f;
            }
        }


        // ?? 满蓄瞬间一次性特效：翠绿魔法阵 + 大范围扩散
        private void SpawnChargeReadyOnceFX(Vector2 pos)
        {
            // 大量翠绿 Dust（TerraBlade）
            for (int i = 0; i < 180; i++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(18f, 18f);
                Dust d = Dust.NewDustPerfect(pos, DustID.TerraBlade, vel, 150, Color.GreenYellow, Main.rand.NextFloat(1.2f, 1.8f));
                d.noGravity = true;
            }

            // 翠绿色 GlowOrb 魔法阵
            int orbCount = 24;
            float radius = 64f;
            for (int i = 0; i < orbCount; i++)
            {
                float angle = MathHelper.TwoPi * i / orbCount;
                Vector2 offset = angle.ToRotationVector2() * radius;
                var orb = new GlowOrbParticle(
                    pos + offset,
                    Vector2.Zero,
                    false,
                    14,
                    0.9f,
                    Color.LimeGreen,
                    true,
                    false,
                    true
                );
                GeneralParticleHandler.SpawnParticle(orb);
            }

            // 数学感线性粒子：正弦波形扩散
            for (int i = 0; i < 60; i++)
            {
                float angle = MathHelper.TwoPi * i / 60f;
                Vector2 dir = angle.ToRotationVector2();
                Vector2 offset = dir * (40f + (float)System.Math.Sin(angle * 6f) * 12f); // 加入波动
                Particle trail = new SparkParticle(
                    pos + offset,
                    dir * 2.5f,
                    false,
                    70,
                    1.1f,
                    Color.SpringGreen
                );
                GeneralParticleHandler.SpawnParticle(trail);
            }
        }





    }
}
