using CalamityMod;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.DPreDog.NettlevineGreatbowC
{
    internal class NettlevineGreatbowReHold : BaseGunHoldoutProjectile
    {
        public override string Texture =>
            "CalamityRangerExpansion/Content/BOWChange/DPreDog/NettlevineGreatbowC/NettlevineGreatbowRe";

        public override int AssociatedItemID =>
            ModContent.ItemType<NettlevineGreatbowRe>();

        public override bool? CanDamage() => false;

        public override Vector2 GunTipPosition =>
            Projectile.Center + Projectile.rotation.ToRotationVector2() * 18f;

        public override float MaxOffsetLengthFromArm => 14f;

        // ===================== 蓄力参数 =====================
        private const int MaxChargeTime = 180;
        private int chargeTimer;
        private bool chargeReadyFXPlayed;

        // ===================== 释放阶段 =====================
        private int releaseTimer;
        private int releaseStep;
        private const int TotalReleaseWaves = 7;

        // ==================================================
        // AI：固定结构（必须）
        // ==================================================
        public override void AI()
        {
            KeepRefreshingLifetime = true;
            base.AI();
        }

        // ==================================================
        // HoldoutAI：蓄力推进
        // ==================================================
        public override void HoldoutAI()
        {
            Player player = Owner;

            if (player.channel)
            {
                chargeTimer = Math.Min(chargeTimer + 1, MaxChargeTime);

                BowChangeVFX.SpawnCharge(Projectile, GunTipPosition, BowChangeTheme.Nettle, chargeTimer / (float)MaxChargeTime, 0.65f);
                SpawnChargeFX_EveryFrame();

                if (chargeTimer >= MaxChargeTime)
                {
                    BowChangeVFX.SpawnCharge(Projectile, GunTipPosition, BowChangeTheme.Nettle, 1f, 0.55f);
                    SpawnChargeFX_Complete();

                    if (!chargeReadyFXPlayed)
                    {
                        chargeReadyFXPlayed = true;
                        BowChangeVFX.SpawnReadyBurst(Projectile, GunTipPosition, BowChangeTheme.Nettle, 0.9f);
                        SpawnChargeReadyOnceFX();
                    }
                }

                return;
            }


        }

        // ==================================================
        // KillHoldoutLogic：释放清算
        // ==================================================
        public override void KillHoldoutLogic()
        {
            if (!Owner.CantUseHoldout())
                return;

            if (chargeTimer < MaxChargeTime && releaseStep == 0)
            {
                Projectile.Kill();
                return;
            }

            releaseTimer++;

            if (releaseStep < TotalReleaseWaves && releaseTimer % 3 == 0)
            {
                FireProjectile();
                releaseStep++;
            }

            if (releaseStep >= TotalReleaseWaves)
                Projectile.Kill();
        }

        // ==================================================
        // SpawnChargeFX_EveryFrame：蓄力持续特效
        // ==================================================
        private void SpawnChargeFX_EveryFrame()
        {
            float ratio = chargeTimer / (float)MaxChargeTime;

            if (Main.rand.NextBool(2))
            {
                Dust d = Dust.NewDustPerfect(
                    GunTipPosition,
                    DustID.Grass,
                    Main.rand.NextVector2Circular(2f, 2f),
                    120,
                    new Color(40, 120, 40),
                    1.0f + ratio * 0.8f
                );
                d.noGravity = true;
            }
        }

        // ==================================================
        // SpawnChargeFX_Complete：满蓄持续态
        // ==================================================
        private void SpawnChargeFX_Complete()
        {
            if (Main.rand.NextBool(3))
            {
                Dust d = Dust.NewDustPerfect(
                    GunTipPosition,
                    DustID.JungleGrass,
                    Main.rand.NextVector2Circular(2f, 2f),
                    100,
                    new Color(60, 160, 60),
                    1.3f
                );
                d.noGravity = true;
            }
        }

        // ==================================================
        // SpawnChargeReadyOnceFX：满蓄瞬间爆发
        // ==================================================
        private void SpawnChargeReadyOnceFX()
        {
            SoundEngine.PlaySound(
                SoundID.Grass with { Pitch = -0.2f },
                Projectile.Center
            );

            for (int i = 0; i < 24; i++)
            {
                Vector2 v =
                    (MathHelper.TwoPi * i / 24f).ToRotationVector2() * 5f;

                Dust d = Dust.NewDustPerfect(
                    GunTipPosition,
                    DustID.JungleGrass,
                    v,
                    100,
                    new Color(60, 160, 60),
                    1.4f
                );
                d.noGravity = true;
            }
        }

        // ==================================================
        // FireProjectile：具体发射逻辑（未改）
        // ==================================================
        private void FireProjectile()
        {
            Player player = Owner;
            Vector2 forward = Projectile.rotation.ToRotationVector2();
            Vector2 perp = forward.RotatedBy(MathHelper.PiOver2);

            if (!player.PickAmmo(
                player.HeldItem,
                out int projType,
                out _,
                out int damage,
                out float knockback,
                out _))
                return;

            for (int i = -1; i <= 1; i += 2)
            {
                Vector2 offset = perp * i * 12f;

                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    GunTipPosition + offset,
                    forward * 15f,
                    projType,
                    damage,
                    knockback,
                    player.whoAmI
                );
            }

            SoundEngine.PlaySound(
                SoundID.Item5 with { Volume = 0.7f },
                Projectile.Center
            );
            BowChangeVFX.SpawnMuzzle(Projectile, GunTipPosition, forward * 15f, BowChangeTheme.Nettle, 0.8f);
        }
    }
}
