using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.Projectiles.BaseProjectiles;
using System;
using CalamityMod;
using CalamityMod.Particles;
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.APreHardMode.ToxibowC
{
    internal class ToxibowReHold : BaseGunHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.APreHardMode";

        public override string Texture => "CalamityRangerExpansion/Content/BOWChange/APreHardMode/ToxibowC/ToxibowRe";

        public override int AssociatedItemID => ModContent.ItemType<ToxibowRe>();

        public override bool? CanDamage() => Main.player[Projectile.owner].GetModPlayer<OpenBowDamagePlayer>().OpenBowDamage;

        public override Vector2 GunTipPosition =>
            Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * (Projectile.width * 0.5f + 10f);

        public override float MaxOffsetLengthFromArm => 15f;

        private ref float ChargeFrames => ref Projectile.ai[0];
        private ref float ShotsRemaining => ref Projectile.ai[1];
        private int readyDustTicker = 0;
        private bool hasTriggeredReadyFX = false;

        private bool triggered50 = false;
        private bool triggered70 = false;
        private int shotsPlanned = 0;

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
                ChargeFrames++;

                if (ChargeFrames >= 50 && !triggered50)
                {
                    triggered50 = true;
                    BowChangeVFX.SpawnReadyBurst(Projectile, GunTipPosition, BowChangeTheme.Toxic, 0.35f);
                    SoundEngine.PlaySound(SoundID.Item29, player.Center);
                }
                if (ChargeFrames >= 70 && !triggered70)
                {
                    triggered70 = true;
                    BowChangeVFX.SpawnReadyBurst(Projectile, GunTipPosition, BowChangeTheme.Toxic, 0.45f);
                    SoundEngine.PlaySound(SoundID.Item29, player.Center);
                }

                if (ChargeFrames < 150)
                {
                    if (ChargeFrames % 10 == 0)
                        //SpawnChargeCircle(GunTipPosition);

                    BowChangeVFX.SpawnCharge(Projectile, GunTipPosition, BowChangeTheme.Toxic, ChargeFrames / 150f, 0.75f);
                    SpawnChargeFX_EveryFrame(GunTipPosition);
                }
                else
                {
                    readyDustTicker++;
                    if (readyDustTicker % 3 == 0 && ShotsRemaining <= 0)
                        //SpawnReadyIdleDust(GunTipPosition);

                    SpawnChargeFX_Complete(GunTipPosition);
                    BowChangeVFX.SpawnCharge(Projectile, GunTipPosition, BowChangeTheme.Toxic, 1f, 0.6f);

                    if (!hasTriggeredReadyFX)
                    {
                        BowChangeVFX.SpawnReadyBurst(Projectile, GunTipPosition, BowChangeTheme.Toxic, 1f);
                        SpawnChargeReadyOnceFX(GunTipPosition);
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
                if (ChargeFrames < 50)
                {
                    Projectile.Kill();
                    return;
                }

                if (ShotsRemaining <= 0 && ChargeFrames > 0)
                {
                    shotsPlanned = ChargeFrames >= 150 ? 3 : ChargeFrames >= 90 ? 2 : 1;
                    ShotsRemaining = shotsPlanned;
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

        private void FireStageProjectile(Player player, int stage)
        {
            Vector2 shootVel = Projectile.velocity.SafeNormalize(Vector2.UnitX * player.direction) * 12f;

            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                GunTipPosition,
                shootVel,
                ModContent.ProjectileType<ToxibowRePROJ>(),
                Projectile.damage,
                Projectile.knockBack,
                player.whoAmI,
                ai0: stage
            );

            SpawnPerShotFX(GunTipPosition, shootVel);
        }

        private void FireNextProjectile(Player player)
        {
            int shotIndex = shotsPlanned - (int)ShotsRemaining;
            float spread = shotsPlanned <= 1 ? 0f : MathHelper.Lerp(-5f, 5f, shotIndex / (float)(shotsPlanned - 1));
            int stage = ChargeFrames >= 150 ? 3 : ChargeFrames >= 90 ? 2 : 1;
            Vector2 shootVel = Projectile.velocity.SafeNormalize(Vector2.UnitX * player.direction).RotatedBy(MathHelper.ToRadians(spread)) * (13f + stage * 1.5f);

            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                GunTipPosition,
                shootVel,
                ModContent.ProjectileType<ToxibowRePROJ>(),
                Projectile.damage,
                Projectile.knockBack,
                player.whoAmI,
                ai0: stage
            );

            SpawnPerShotFX(GunTipPosition, shootVel);
            BowChangeVFX.SpawnMuzzle(Projectile, GunTipPosition, shootVel, BowChangeTheme.Toxic, 0.9f);
            SoundEngine.PlaySound(SoundID.Item72, player.Center);

            // 屏幕震动效果
            float shakePower = 5f;
            float distanceFactor = Utils.GetLerpValue(1000f, 0f, Projectile.Distance(Main.LocalPlayer.Center), true);
            Main.LocalPlayer.Calamity().GeneralScreenShakePower =
                Math.Max(Main.LocalPlayer.Calamity().GeneralScreenShakePower, shakePower * distanceFactor);
        }

        // ===== 特效函数 =====

        private void SpawnChargeFX_EveryFrame(Vector2 pos)
        {
            // ① 缓慢渗出的毒液雾气
            for (int i = 0; i < 3; i++)
            {
                Vector2 jitter = Main.rand.NextVector2Circular(4f, 4f);
                Vector2 vel = new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(-0.9f, -0.4f));
                Dust d = Dust.NewDustPerfect(pos + jitter, DustID.Poisoned, vel, 130,
                    Color.Lerp(Color.YellowGreen, Color.Lime, Main.rand.NextFloat(0.4f, 0.8f)), Main.rand.NextFloat(0.8f, 1.1f));
                d.noGravity = true;
            }

            // ② 偶发细长毒线（AltSpark）——电流状毒丝
            if (Main.rand.NextBool(1, 5))
            {
                var spark = new AltSparkParticle(
                    pos + Main.rand.NextVector2Circular(3f, 3f),
                    Main.rand.NextVector2Circular(0.2f, 0.2f),
                    false,
                    14,
                    1.1f,
                    Color.LimeGreen * 0.35f
                );
                GeneralParticleHandler.SpawnParticle(spark);
            }

            // ③ 轻型酸雾（HeavySmoke，小体积）
            if (Main.rand.NextBool(1, 8))
            {
                var smoke = new HeavySmokeParticle(
                    pos + Main.rand.NextVector2Circular(6f, 6f),
                    new Vector2(Main.rand.NextFloat(-0.1f, 0.1f), Main.rand.NextFloat(-0.5f, -0.2f)),
                    Color.Lerp(Color.GreenYellow, Color.DarkOliveGreen, 0.4f),
                    15,
                    Main.rand.NextFloat(0.35f, 0.6f),
                    0.25f,
                    Main.rand.NextFloat(-0.8f, 0.8f),
                    false
                );
                GeneralParticleHandler.SpawnParticle(smoke);
            }
        }



        private void SpawnChargeFX_Complete(Vector2 pos)
        {
            // 毒尘向外散（数量较少）
            for (int i = 0; i < 3; i++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(0.8f, 0.8f);
                Dust d = Dust.NewDustPerfect(pos, DustID.Poisoned, vel, 120,
                    Color.Lerp(Color.Chartreuse, Color.LimeGreen, Main.rand.NextFloat()), Main.rand.NextFloat(0.9f, 1.2f));
                d.noGravity = true;
            }

            // 小量毒丝电弧
            if (Main.rand.NextBool(1, 3))
            {
                var spark = new AltSparkParticle(
                    pos + Main.rand.NextVector2Circular(5f, 5f),
                    Main.rand.NextVector2Circular(0.1f, 0.1f),
                    false,
                    12,
                    1.2f,
                    Color.YellowGreen * 0.4f
                );
                GeneralParticleHandler.SpawnParticle(spark);
            }

            // 低频轻烟
            if (Main.rand.NextBool(1, 5))
            {
                var smoke = new HeavySmokeParticle(
                    pos + Main.rand.NextVector2Circular(8f, 8f),
                    new Vector2(Main.rand.NextFloat(-0.15f, 0.15f), Main.rand.NextFloat(-0.6f, -0.3f)),
                    Color.Lerp(Color.YellowGreen, Color.Olive, 0.3f),
                    18,
                    Main.rand.NextFloat(0.45f, 0.7f),
                    0.25f,
                    Main.rand.NextFloat(-0.5f, 0.5f),
                    false
                );
                GeneralParticleHandler.SpawnParticle(smoke);
            }
        }



        private void SpawnChargeReadyOnceFX(Vector2 pos)
        {
            // A) 毒液尘环（放射状）——视觉主体
            int dusts = 100;
            for (int i = 0; i < dusts; i++)
            {
                Vector2 dir = Main.rand.NextVector2CircularEdge(1f, 1f);
                Vector2 vel = dir * Main.rand.NextFloat(2.5f, 6f);
                Dust d = Dust.NewDustPerfect(pos, DustID.Poisoned, vel, 100,
                    Color.Lerp(Color.LimeGreen, Color.Chartreuse, Main.rand.NextFloat()), Main.rand.NextFloat(1.0f, 1.4f));
                d.noGravity = true;
            }

            // B) 电弧状酸丝（AltSpark，少但高亮）
            int sparks = 20;
            for (int i = 0; i < sparks; i++)
            {
                Vector2 dir = Main.rand.NextVector2Circular(1f, 1f);
                var spark = new AltSparkParticle(
                    pos + dir * Main.rand.NextFloat(0f, 10f),
                    dir * Main.rand.NextFloat(1.0f, 2.0f),
                    false,
                    Main.rand.Next(15, 22),
                    Main.rand.NextFloat(1.2f, 1.5f),
                    Color.Lime * 0.45f
                );
                GeneralParticleHandler.SpawnParticle(spark);
            }

            // C) 轻型酸雾（环状扩散，缓缓上飘）
            for (int i = 0; i < 24; i++)
            {
                Vector2 offset = Vector2.UnitY.RotatedBy(MathHelper.TwoPi * i / 24f) * 18f;
                var smoke = new HeavySmokeParticle(
                    pos + offset,
                    new Vector2(Main.rand.NextFloat(-0.4f, 0.4f), Main.rand.NextFloat(-0.5f, 0f)),
                    Color.Lerp(Color.GreenYellow, Color.OliveDrab, 0.5f),
                    20,
                    Main.rand.NextFloat(0.45f, 0.75f),
                    0.25f,
                    Main.rand.NextFloat(-1f, 1f),
                    false
                );
                GeneralParticleHandler.SpawnParticle(smoke);
            }
        }

        private void SpawnPerShotFX(Vector2 pos, Vector2 vel)
        {
            // 毒气尘迹
            for (int i = 0; i < 10; i++)
            {
                Vector2 v = vel.RotatedByRandom(0.2f) * Main.rand.NextFloat(0.5f, 1f);
                Dust d = Dust.NewDustPerfect(pos, DustID.PoisonStaff, v, 120,
                    Color.Lerp(Color.LimeGreen, Color.YellowGreen, Main.rand.NextFloat()), Main.rand.NextFloat(0.8f, 1.2f));
                d.noGravity = true;
            }

            // 藤状电弧线
            if (Main.rand.NextBool(1, 3))
            {
                var spark = new AltSparkParticle(
                    pos,
                    vel * 0.05f,
                    false,
                    10,
                    1.1f,
                    Color.YellowGreen * 0.35f
                );
                GeneralParticleHandler.SpawnParticle(spark);
            }
        }






    }
}
