using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Particles;
using CalamityRangerExpansion.Content.BOWChange;
using CalamityRangerExpansion.Content.BOWChange.EAfterDog.PhangasmC;
using System;

namespace CalamityRangerExpansion.Content.BOWChange.EAfterDog.PhangasmC
{
    public class PhangasmReHold : BaseGunHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.EAfterDog";

        public override string Texture => "CalamityRangerExpansion/Content/BOWChange/EAfterDog/PhangasmC/PhangasmRe";
        public override int AssociatedItemID => ModContent.ItemType<PhangasmRe>();

        public override float MaxOffsetLengthFromArm => 15f;
        public override Vector2 GunTipPosition =>
            Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * (Projectile.width * 0.5f + 10f);

        private ref float ChargeFrames => ref Projectile.ai[0];
        private ref float ShotsRemaining => ref Projectile.ai[1];
        private int readyDustTicker = 0;
        private int fireTimer = 0;
        private int fireStep = 0;
        private int fireMode = 0; // 0 = true箭, 1 = false箭
        private bool hasTriggeredReadyFX = false; // 一次性爆发提示

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
                    BowChangeVFX.SpawnCharge(Projectile, GunTipPosition, BowChangeTheme.Phangasm, ChargeFrames / 90f, 0.6f);
                    if (ChargeFrames % 10 == 0)
                        SpawnChargeCircle(GunTipPosition); // 每10帧一次
                }
                else
                {
                    BowChangeVFX.SpawnCharge(Projectile, GunTipPosition, BowChangeTheme.Phangasm, 1f, 0.55f);
                    SpawnChargeFX_Complete(GunTipPosition); // 满蓄持续释放

                    if (!hasTriggeredReadyFX)
                    {
                        BowChangeVFX.SpawnReadyBurst(Projectile, GunTipPosition, BowChangeTheme.Phangasm, 0.9f);
                        SpawnChargeReadyOnceFX(GunTipPosition); // 一次性爆发
                        hasTriggeredReadyFX = true;
                    }
                }
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

                if (ShotsRemaining <= 0 && ChargeFrames >= 90)
                {
                    fireTimer = 0;
                    fireStep = 0;
                    fireMode = Main.rand.NextBool() ? 0 : 1;
                    ShotsRemaining = 3;
                }

                if (ShotsRemaining > 0)
                {
                    fireTimer++;
                    if (fireTimer >= 7)
                    {
                        FireNextProjectile(player);
                        fireTimer = 0;
                        ShotsRemaining--;

                        if (ShotsRemaining <= 0)
                            Projectile.Kill();
                    }
                }
            }
        }

        private void FireNextProjectile(Player player)
        {
            Vector2 shootVel = Projectile.velocity.SafeNormalize(Vector2.UnitX * player.direction) * 24f;
            Item heldItem = player.HeldItem;

            if (!player.HasAmmo(heldItem))
                return;

            if (player.PickAmmo(heldItem, out int ammoProjectile, out float _, out int damage, out float knockback, out int _))
            {
                for (int i = 0; i < 6; i++)
                {
                    Vector2 offset = new Vector2(Main.rand.NextFloat(-8f, 8f), Main.rand.NextFloat(-4f, 4f));
                    int proj = Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        GunTipPosition + offset,
                        shootVel,
                        ammoProjectile,
                        damage,
                        knockback,
                        player.whoAmI
                    );

                    if (proj.WithinBounds(Main.maxProjectiles))
                        Main.projectile[proj].GetGlobalProjectile<PhangasmReEffect>().PhangasmArrowType = fireMode;
                }

                SoundEngine.PlaySound(SoundID.Item5 with { Volume = 1f }, player.Center);
            }

            SpawnPerShotFX(GunTipPosition, shootVel);
            BowChangeVFX.SpawnMuzzle(Projectile, GunTipPosition, shootVel, BowChangeTheme.Phangasm, 0.85f);
        }

        // 【蓄力期?冲突函数】持续释放（默认未使用）
        private void SpawnChargeFX_EveryFrame(Vector2 pos)
        {
            // TODO: 如果切换到持续模式，这里实现火花类特效
        }

        // 【蓄力期?默认选择】每10帧一次的能量环
        private void SpawnChargeCircle(Vector2 pos)
        {
            int points = 14;
            float radius = 14f;
            for (int i = 0; i < points; i++)
            {
                float ang = MathHelper.TwoPi * i / points;
                Vector2 offset = ang.ToRotationVector2() * radius;
                Dust d = Dust.NewDustPerfect(pos + offset, DustID.Electric, offset * 0.05f, 120, Color.Cyan, 1.2f);
                d.noGravity = true;
            }
        }

        // 【满蓄后?冲突函数】间隔提示（默认未使用）
        private void SpawnReadyIdleDust(Vector2 pos)
        {
            // TODO: 如果切换到间隔模式，这里实现闲置 Dust 提示
        }

        // 【满蓄后?默认选择】持续释放：Dust + GlowOrb + SquareParticle + ConstellationRing
        private void SpawnChargeFX_Complete(Vector2 pos)
        {
            float time = (float)Main.GameUpdateCount * 0.05f; // 随帧增长的时间因子
            int arms = 3; // 3条旋臂
            float baseRadius = 12f + (float)Math.Sin(time * 2f) * 4f; // 半径呼吸变化

            for (int i = 0; i < arms; i++)
            {
                float angle = time + MathHelper.TwoPi * i / arms; // 三条旋臂相位错开
                Vector2 offset = angle.ToRotationVector2() * baseRadius;
                Vector2 spawnPos = pos + offset;

                // Dust：环绕溢出
                Vector2 vel = offset.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.6f, 1.2f);
                int d = Dust.NewDust(spawnPos, 0, 0, DustID.UnusedWhiteBluePurple, vel.X, vel.Y);
                Main.dust[d].noGravity = true;
                Main.dust[d].scale = Main.rand.NextFloat(0.8f, 1.2f);

                // GlowOrb：小球在旋臂上漂浮
                if (Main.rand.NextBool(3))
                {
                    GlowOrbParticle orb = new GlowOrbParticle(
                        spawnPos,
                        Vector2.Zero,
                        false,
                        6,
                        0.6f,
                        Color.LightCyan,
                        true,
                        false,
                        true
                    );
                    GeneralParticleHandler.SpawnParticle(orb);
                }

                // Square：顺着旋臂喷出
                if (Main.rand.NextBool(2))
                {
                    SquareParticle square = new SquareParticle(
                        spawnPos,
                        offset.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.5f, 1.2f),
                        false,
                        25,
                        1.2f + Main.rand.NextFloat(0.4f),
                        Color.Cyan * 1.2f
                    );
                    GeneralParticleHandler.SpawnParticle(square);
                }
            }

            // 星座环
            ConstellationRingVFX ring = new ConstellationRingVFX(
                pos,
                Color.GreenYellow * 0.8f,
                Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi),
                1.2f,
                new Vector2(1f, 1f),
                0.9f,
                5,
                1.5f,
                0.06f,
                false
            );
            GeneralParticleHandler.SpawnParticle(ring);
        }

        // 【满蓄第一次?必有】一次性爆发：Dust + Sparkle + Bloom 大规模
        private void SpawnChargeReadyOnceFX(Vector2 pos)
        {
            // 大量 Dust：扩散范围更大，速度更快
            for (int i = 0; i < 100; i++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(40f, 40f); // 半径扩散 ×10
                int d = Dust.NewDust(pos, 0, 0, DustID.FireworkFountain_Red, vel.X, vel.Y);
                Main.dust[d].noGravity = true;
                Main.dust[d].scale = Main.rand.NextFloat(1f, 1.6f);
            }

            // 十字星：速度、扩散放大 ×10
            for (int i = 0; i < 18; i++)
            {
                GenericSparkle sparkle = new GenericSparkle(
                    pos,
                    Main.rand.NextVector2Circular(10f, 10f),      // 扩散范围放大
                    Color.Gold,
                    Color.Cyan,
                    Main.rand.NextFloat(1.2f, 1.8f),
                    8,
                    Main.rand.NextFloat(-0.3f, 0.3f),            // 旋转幅度也放大
                    1.6f
                );
                GeneralParticleHandler.SpawnParticle(sparkle);
            }


            // Bloom 光晕
            for (int i = 0; i < 4; i++)
            {
                GenericBloom bloom = new GenericBloom(
                    pos,
                    Vector2.Zero,
                    Color.GreenYellow,
                    1.5f,
                    40
                );
                GeneralParticleHandler.SpawnParticle(bloom);
            }
        }

        // 【发射时?必有】枪口喷射
        private void SpawnPerShotFX(Vector2 pos, Vector2 vel)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust d = Dust.NewDustPerfect(pos, DustID.Flare, vel.RotatedByRandom(0.4f) * 0.6f, 120, Color.Cyan, 1.2f);
                d.noGravity = true;
            }
        }
    }
}
