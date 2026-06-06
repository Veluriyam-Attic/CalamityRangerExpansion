using CalamityRangerExpansion.Content.BOWChange.BPrePlantera.ArbalestC;
using CalamityMod;
using CalamityRangerExpansion.Content.BOWChange;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityRangerExpansion.Content.BOWChange.DPreDog.DaemonsFlameC
{
    internal class DaemonsFlameReHold : BaseGunHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.DPreDog";
        public override string Texture => "CalamityRangerExpansion/Content/BOWChange/DPreDog/DaemonsFlameC/DaemonsFlameRe";
        public override int AssociatedItemID => ModContent.ItemType<DaemonsFlameRe>();

        public override bool? CanDamage() => false;
        public override float MaxOffsetLengthFromArm => 15f;

        public override Vector2 GunTipPosition =>
            Projectile.Center + Projectile.rotation.ToRotationVector2() * (Projectile.width * 0.5f + 10f);

        // ================= 参数 =================
        private const int MaxChargeTime = 180;

        private int chargeTimer;
        private bool chargeReadyFXPlayed;
        private bool canRelease;

        private int releaseTimer;
        private int releaseStep;

        // ==================================================
        // Static：多帧贴图声明
        // ==================================================
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

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

            if (player.channel && !canRelease)
            {
                chargeTimer = Math.Min(chargeTimer + 1, MaxChargeTime);

                BowChangeVFX.SpawnCharge(Projectile, GunTipPosition, BowChangeTheme.DaemonsFlame, chargeTimer / (float)MaxChargeTime, 0.65f);
                SpawnChargeFX_EveryFrame();

                if (chargeTimer >= MaxChargeTime)
                {
                    BowChangeVFX.SpawnCharge(Projectile, GunTipPosition, BowChangeTheme.DaemonsFlame, 1f, 0.55f);
                    SpawnChargeFX_Complete();

                    if (!chargeReadyFXPlayed)
                    {
                        chargeReadyFXPlayed = true;
                        BowChangeVFX.SpawnReadyBurst(Projectile, GunTipPosition, BowChangeTheme.DaemonsFlame, 1f);
                        SpawnChargeReadyOnceFX();
                    }
                }

                return;
            }

            // 未满蓄松手
            if (!player.channel && chargeTimer < MaxChargeTime)
            {
                Projectile.Kill();
                return;
            }

            // 满蓄松手 → 进入清算阶段
            if (!player.channel && chargeTimer >= MaxChargeTime)
            {
                canRelease = true;
            }
        }

        // ==================================================
        // KillHoldoutLogic：结束清算（原有释放逻辑）
        // ==================================================
        public override void KillHoldoutLogic()
        {
            if (!canRelease || !Owner.CantUseHoldout())
                return;

            releaseTimer++;

            if (releaseStep < 3 && releaseTimer % 8 == 0)
            {
                FireSequenceStep();
                releaseStep++;
            }

            if (releaseStep >= 3)
                Projectile.Kill();
        }

        // ==================================================
        // 蓄力期间持续特效
        // ==================================================
        private void SpawnChargeFX_EveryFrame()
        {
            if (chargeTimer % 6 == 0)
            {
                Dust d = Dust.NewDustPerfect(
                    GunTipPosition,
                    DustID.PinkTorch,
                    Main.rand.NextVector2Circular(2.5f, 2.5f),
                    150,
                    Color.Magenta,
                    1.2f
                );
                d.noGravity = true;
            }
        }

        // ==================================================
        // 蓄力完成后的持续特效
        // ==================================================
        private void SpawnChargeFX_Complete()
        {
            if (Main.rand.NextBool(4))
            {
                Dust d = Dust.NewDustPerfect(
                    GunTipPosition,
                    DustID.PinkTorch,
                    Main.rand.NextVector2Circular(1.5f, 1.5f),
                    150,
                    Color.Magenta,
                    1.4f
                );
                d.noGravity = true;
            }
        }

        // ==================================================
        // 蓄力完成瞬间爆发特效
        // ==================================================
        private void SpawnChargeReadyOnceFX()
        {
            SoundEngine.PlaySound(SoundID.NPCHit36, Projectile.Center);

            for (int i = 0; i < 30; i++)
            {
                Dust d = Dust.NewDustPerfect(
                    GunTipPosition,
                    DustID.PinkTorch,
                    Main.rand.NextVector2Circular(6f, 6f),
                    150,
                    Color.Magenta,
                    1.5f
                );
                d.noGravity = true;
            }
        }

        // ==================================================
        // 原有单轮发射逻辑（未改）
        // ==================================================
        private void FireSequenceStep()
        {
            Player player = Owner;

            Vector2 forward = Projectile.rotation.ToRotationVector2();
            Vector2 left = forward.RotatedBy(-MathHelper.PiOver2);
            Vector2 right = forward.RotatedBy(MathHelper.PiOver2);

            // 玩家弹幕：5 排平行
            if (player.HasAmmo(player.HeldItem) &&
                player.PickAmmo(player.HeldItem, out int ammoProj, out _, out int damage, out float kb, out _))
            {
                for (int i = -2; i <= 2; i++)
                {
                    Vector2 offset = left * (i * 10f);
                    int p = Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        GunTipPosition + offset,
                        forward * 14f,
                        ammoProj,
                        damage,
                        kb,
                        player.whoAmI
                    );

                    if (p.WithinBounds(Main.maxProjectiles))
                        Main.projectile[p]
                            .GetGlobalProjectile<DaemonsFlameReEffect>()
                            .IsDaemonsFlameReArrow = true;
                }
            }

            // 红鬼：左右
            for (int i = 0; i < 2; i++)
            {
                Vector2 dir = (i == 0) ? left : right;

                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    GunTipPosition,
                    dir * 8f,
                    ModContent.ProjectileType<DaemonsFlameProjRed>(),
                    (int)(Projectile.damage * 0.8f),
                    2f,
                    player.whoAmI
                );
            }

            // 蓝鬼：正前
            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                GunTipPosition,
                forward * 9f,
                ModContent.ProjectileType<DaemonsFlameProjBlue>(),
                (int)(Projectile.damage * 0.6f),
                1f,
                player.whoAmI
            );

            SoundEngine.PlaySound(SoundID.Item5 with { Volume = 0.8f }, Projectile.Center);
            BowChangeVFX.SpawnMuzzle(Projectile, GunTipPosition, forward * 14f, BowChangeTheme.DaemonsFlame, 0.9f);
        }

        // ==================================================
        // 多帧图绘制（保持原样）
        // ==================================================
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch sb = Main.spriteBatch;
            Texture2D tex = TextureAssets.Projectile[Projectile.type].Value;

            int frameHeight = tex.Height / Main.projFrames[Projectile.type];
            int frame = (int)(Main.GameUpdateCount / 6 % Main.projFrames[Projectile.type]);
            Rectangle src = new Rectangle(0, frame * frameHeight, tex.Width, frameHeight);

            sb.Draw(
                tex,
                Projectile.Center - Main.screenPosition,
                src,
                lightColor,
                Projectile.rotation,
                src.Size() / 2f,
                Projectile.scale,
                Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None,
                0f
            );

            return false;
        }
    }
}
