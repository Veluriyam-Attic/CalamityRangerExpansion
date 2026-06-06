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

namespace CalamityRangerExpansion.Content.BOWChange.BPrePlantera.CorrodedCaustibowC
{
    public class CorrodedCaustibowReHold : BaseGunHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.BPrePlantera";

        public override string Texture => "CalamityRangerExpansion/Content/BOWChange/BPrePlantera/CorrodedCaustibowC/CorrodedCaustibowRe";

        public override int AssociatedItemID => ModContent.ItemType<CorrodedCaustibowRe>();

        public override float MaxOffsetLengthFromArm => 15f; // 后坐力偏移

        public override bool? CanDamage() => Main.player[Projectile.owner].GetModPlayer<OpenBowDamagePlayer>().OpenBowDamage;

        public override Vector2 GunTipPosition =>
            Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * (Projectile.width * 0.5f + 10f);

        private ref float ChargeFrames => ref Projectile.ai[0];
        private ref float ShotsRemaining => ref Projectile.ai[1];
        private int readyDustTicker = 0;
        private bool hasTriggeredReadyFX = false;

        // 记录魔法阵阶段是否触发
        private bool triggered50 = false;
        private bool triggered70 = false;
        private bool triggered100 = false;

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

                // 三段式阶段提示（毒爆环）
                if (ChargeFrames >= 50 && !triggered50)
                {
                    triggered50 = true;
                    BowChangeVFX.SpawnReadyBurst(Projectile, GunTipPosition, BowChangeTheme.Caustic, 0.35f);
                    SoundEngine.PlaySound(SoundID.Item29, player.Center);
                }
                if (ChargeFrames >= 70 && !triggered70)
                {
                    triggered70 = true;
                    BowChangeVFX.SpawnReadyBurst(Projectile, GunTipPosition, BowChangeTheme.Caustic, 0.45f);
                    SoundEngine.PlaySound(SoundID.Item29, player.Center);
                }
                if (ChargeFrames >= 100 && !triggered100)
                {
                    triggered100 = true;
                    BowChangeVFX.SpawnReadyBurst(Projectile, GunTipPosition, BowChangeTheme.Caustic, 0.55f);
                    SoundEngine.PlaySound(SoundID.Item29, player.Center);
                }

                // 普通蓄力阶段
                if (ChargeFrames < 150)
                {
                    BowChangeVFX.SpawnCharge(Projectile, GunTipPosition, BowChangeTheme.Caustic, ChargeFrames / 150f, 0.75f);
                    SpawnChargeFX_EveryFrame(GunTipPosition);
                }
                else
                {
                    readyDustTicker++;
                    if (readyDustTicker % 3 == 0 && ShotsRemaining <= 0)
                        //SpawnReadyIdleDust(GunTipPosition);

                    SpawnChargeFX_Complete(GunTipPosition);
                    BowChangeVFX.SpawnCharge(Projectile, GunTipPosition, BowChangeTheme.Caustic, 1f, 0.65f);

                    if (!hasTriggeredReadyFX)
                    {
                        BowChangeVFX.SpawnReadyBurst(Projectile, GunTipPosition, BowChangeTheme.Caustic, 1f);
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
                    ShotsRemaining = 1; // 只打一发
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
            Vector2 shootVel = Projectile.velocity.SafeNormalize(Vector2.UnitX * player.direction) * 14f;

            // 判定当前阶段
            int stage = 3; // 默认第三阶段
            if (triggered100) stage = 3;
            else if (triggered70) stage = 2;
            else if (triggered50) stage = 1;

            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                GunTipPosition,
                shootVel,
                ModContent.ProjectileType<CorrodedCaustibowRePROJ>(),
                Projectile.damage,
                Projectile.knockBack,
                player.whoAmI,
                ai0: stage
            );

            SpawnPerShotFX(GunTipPosition, shootVel);
            BowChangeVFX.SpawnMuzzle(Projectile, GunTipPosition, shootVel, BowChangeTheme.Caustic, 0.95f);
            SoundEngine.PlaySound(SoundID.Item72, player.Center);

            // 屏幕震动效果
            float shakePower = 5f;
            float distanceFactor = Utils.GetLerpValue(1000f, 0f, Projectile.Distance(Main.LocalPlayer.Center), true);
            Main.LocalPlayer.Calamity().GeneralScreenShakePower =
                Math.Max(Main.LocalPlayer.Calamity().GeneralScreenShakePower, shakePower * distanceFactor);
        }





        // ===== 特效函数 =====
        // === 1) 蓄力持续（剧毒汇聚：酸雾 + 电弧） ===
        private void SpawnChargeFX_EveryFrame(Vector2 pos)
        {
            // 毒气聚集：亮绿雾向内旋吸
            for (int i = 0; i < 4; i++)
            {
                Vector2 jitter = Main.rand.NextVector2Circular(10f, 10f);
                Vector2 vel = -jitter.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.2f, 0.6f);
                Dust d = Dust.NewDustPerfect(pos + jitter, DustID.Poisoned, vel, 150,
                    Color.Lerp(Color.LimeGreen, Color.Chartreuse, Main.rand.NextFloat()), Main.rand.NextFloat(0.8f, 1.2f));
                d.noGravity = true;
            }

            // 随机电弧状腐蚀线（AltSpark）
            if (Main.rand.NextBool(1, 3))
            {
                var spark = new AltSparkParticle(
                    pos + Main.rand.NextVector2Circular(4f, 4f),
                    Main.rand.NextVector2Circular(0.1f, 0.1f),
                    false,
                    12,
                    1.2f,
                    Color.Lime * 0.3f
                );
                GeneralParticleHandler.SpawnParticle(spark);
            }

            // 酸气雾化轻烟
            if (Main.rand.NextBool(1, 5))
            {
                var smoke = new HeavySmokeParticle(
                    pos + Main.rand.NextVector2Circular(6f, 6f),
                    new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(-0.4f, 0f)),
                    Color.GreenYellow * 0.6f,
                    Main.rand.Next(10, 18),
                    Main.rand.NextFloat(0.45f, 0.75f),
                    0.3f,
                    Main.rand.NextFloat(-1f, 1f),
                    false
                );
                GeneralParticleHandler.SpawnParticle(smoke);
            }
        }



        // === 2) 满蓄持续（酸雾外泄：毒爆前兆） ===
        private void SpawnChargeFX_Complete(Vector2 pos)
        {
            for (int i = 0; i < 6; i++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(1.5f, 1.5f);
                Dust d = Dust.NewDustPerfect(pos, DustID.Venom, vel, 120,
                    Color.Lerp(Color.Lime, Color.YellowGreen, Main.rand.NextFloat(0.3f, 0.9f)), Main.rand.NextFloat(0.9f, 1.3f));
                d.noGravity = true;
            }

            // 持续小电弧乱闪
            if (Main.rand.NextBool(1, 2))
            {
                var spark = new AltSparkParticle(
                    pos + Main.rand.NextVector2Circular(8f, 8f),
                    Main.rand.NextVector2Circular(0.5f, 0.5f),
                    false,
                    15,
                    1.4f,
                    Color.GreenYellow * 0.35f
                );
                GeneralParticleHandler.SpawnParticle(spark);
            }

            // 腐蚀烟气上飘
            if (Main.rand.NextBool(1, 2))
            {
                var smoke = new HeavySmokeParticle(
                    pos + Main.rand.NextVector2Circular(12f, 12f),
                    new Vector2(Main.rand.NextFloat(-0.3f, 0.3f), Main.rand.NextFloat(-0.8f, -0.2f)),
                    Color.Lerp(Color.YellowGreen, Color.Olive, 0.4f),
                    20,
                    Main.rand.NextFloat(0.55f, 0.8f),
                    0.25f,
                    Main.rand.NextFloat(-0.8f, 0.8f),
                    false
                );
                GeneralParticleHandler.SpawnParticle(smoke);
            }
        }



        // === 3) 满蓄瞬间（酸腐爆裂：毒雨 + 烟 + 电弧） ===
        private void SpawnChargeReadyOnceFX(Vector2 pos)
        {
            // 毒尘环爆：放射 + 翻滚烟气
            int dustCount = 100;
            for (int i = 0; i < dustCount; i++)
            {
                Vector2 dir = Main.rand.NextVector2CircularEdge(1f, 1f);
                Vector2 vel = dir * Main.rand.NextFloat(3f, 8f);
                Dust d = Dust.NewDustPerfect(pos, DustID.Poisoned, vel, 100,
                    Color.Lerp(Color.LimeGreen, Color.Chartreuse, Main.rand.NextFloat()), Main.rand.NextFloat(1.0f, 1.4f));
                d.noGravity = true;
            }

            // 酸爆电弧——混乱的酸电
            int sparks = 24;
            for (int i = 0; i < sparks; i++)
            {
                Vector2 dir = Main.rand.NextVector2Circular(1f, 1f);
                var spark = new AltSparkParticle(
                    pos + dir * Main.rand.NextFloat(0f, 12f),
                    dir * Main.rand.NextFloat(1.5f, 3f),
                    false,
                    Main.rand.Next(18, 25),
                    Main.rand.NextFloat(1.2f, 1.8f),
                    Color.LimeGreen * 0.4f
                );
                GeneralParticleHandler.SpawnParticle(spark);
            }

            // 酸雾烟气覆盖全场
            int smoke = 28;
            for (int i = 0; i < smoke; i++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(1.2f, 1.2f);
                var s = new HeavySmokeParticle(
                    pos + Main.rand.NextVector2Circular(20f, 20f),
                    vel * 0.6f,
                    Color.Lerp(Color.GreenYellow, Color.DarkOliveGreen, Main.rand.NextFloat(0.3f, 0.8f)),
                    Main.rand.Next(18, 30),
                    Main.rand.NextFloat(0.65f, 1.0f),
                    0.3f,
                    Main.rand.NextFloat(-1f, 1f),
                    false
                );
                GeneralParticleHandler.SpawnParticle(s);
            }
        }



        // === 4) 发射瞬间（箭矢出膛：酸腐尾迹） ===
        private void SpawnPerShotFX(Vector2 pos, Vector2 vel)
        {
            // 尾迹酸气粒子
            for (int i = 0; i < 12; i++)
            {
                Vector2 v = vel.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.5f, 1.0f);
                Dust d = Dust.NewDustPerfect(pos, DustID.Venom, v, 120, Color.LimeGreen, Main.rand.NextFloat(0.8f, 1.2f));
                d.noGravity = true;
            }

            // 尾部电弧痕迹
            if (Main.rand.NextBool(1, 2))
            {
                var spark = new AltSparkParticle(
                    pos,
                    vel * 0.05f,
                    false,
                    10,
                    1.2f,
                    Color.GreenYellow * 0.4f
                );
                GeneralParticleHandler.SpawnParticle(spark);
            }
        }

    }
}
