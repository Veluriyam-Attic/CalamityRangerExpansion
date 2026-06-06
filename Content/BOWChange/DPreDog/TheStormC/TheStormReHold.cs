using CalamityMod;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.DPreDog.TheStormC
{
    public class TheStormReHold : BaseGunHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.DPreDog";
        public override string Texture => "CalamityRangerExpansion/Content/BOWChange/DPreDog/TheStormC/TheStormRe";
        public override int AssociatedItemID => ModContent.ItemType<TheStormRe>();

        public override float MaxOffsetLengthFromArm => 15f;

        public override bool? CanDamage() =>
            Main.player[Projectile.owner]
                .GetModPlayer<OpenBowDamagePlayer>()
                .OpenBowDamage;

        public override Vector2 GunTipPosition =>
            Projectile.Center +
            Vector2.UnitX.RotatedBy(Projectile.rotation) *
            (Projectile.width * 0.5f + 10f);

        private const int MaxChargeTime = 120;

        private int chargeTimer;
        private bool chargeReadyFXPlayed;
        private bool canRelease;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 9;
        }

        public override void AI()
        {
            KeepRefreshingLifetime = true;
            base.AI();
        }

        public override void HoldoutAI()
        {
            Player player = Owner;

            if (player.channel && !canRelease)
            {
                chargeTimer = Math.Min(chargeTimer + 1, MaxChargeTime);

                BowChangeVFX.SpawnCharge(Projectile, GunTipPosition, BowChangeTheme.Storm, chargeTimer / (float)MaxChargeTime, 0.65f);
                SpawnChargeFX_EveryFrame();

                if (chargeTimer >= MaxChargeTime)
                {
                    BowChangeVFX.SpawnCharge(Projectile, GunTipPosition, BowChangeTheme.Storm, 1f, 0.55f);
                    SpawnChargeFX_Complete();

                    if (!chargeReadyFXPlayed)
                    {
                        chargeReadyFXPlayed = true;
                        BowChangeVFX.SpawnReadyBurst(Projectile, GunTipPosition, BowChangeTheme.Storm, 1f);
                        SpawnChargeReadyOnceFX();
                    }
                }

                return;
            }

            if (!player.channel && chargeTimer < MaxChargeTime)
            {
                Projectile.Kill();
                return;
            }

            if (!player.channel && chargeTimer >= MaxChargeTime)
            {
                canRelease = true;
            }
        }

        public override void KillHoldoutLogic()
        {
            if (!canRelease || !Owner.CantUseHoldout())
                return;

            FireProjectile();
            Projectile.Kill();
        }

        private void SpawnChargeFX_EveryFrame()
        {
            float angle = Main.GameUpdateCount * 0.12f;
            float radius = 10f + chargeTimer * 0.05f;

            for (int i = 0; i < 2; i++)
            {
                Vector2 offset =
                    (angle + MathHelper.Pi * i).ToRotationVector2() * radius;

                Dust d = Dust.NewDustPerfect(
                    GunTipPosition + offset,
                    DustID.Cloud,
                    offset.SafeNormalize(Vector2.Zero) * 0.8f,
                    120,
                    new Color(180, 200, 255),
                    1.1f
                );
                d.noGravity = true;
            }
        }

        private void SpawnChargeFX_Complete()
        {
            if (Main.rand.NextBool(3))
            {
                Dust d = Dust.NewDustPerfect(
                    GunTipPosition,
                    DustID.Electric,
                    Main.rand.NextVector2Circular(2f, 2f),
                    100,
                    Color.White,
                    1.3f
                );
                d.noGravity = true;
            }
        }

        private void SpawnChargeReadyOnceFX()
        {
            SoundEngine.PlaySound(SoundID.Item122, Projectile.Center);

            for (int i = 0; i < 24; i++)
            {
                Vector2 v =
                    (MathHelper.TwoPi * i / 24f).ToRotationVector2() * 6f;

                Dust d = Dust.NewDustPerfect(
                    GunTipPosition,
                    DustID.Electric,
                    v,
                    80,
                    Color.White,
                    1.6f
                );
                d.noGravity = true;
            }
        }

        private void FireProjectile()
        {
            Player player = Owner;

            if (!player.HasAmmo(player.HeldItem))
                return;

            if (!player.PickAmmo(player.HeldItem, out int _, out _, out int damage, out float knockback, out _))
                return;

            damage = (int)(damage * 3f);

            for (int i = 0; i < Main.rand.Next(3, 5); i++)
            {
                Vector2 spawnPosition =
                    Projectile.Center +
                    new Vector2(Main.rand.NextFloat(-15f, 15f) * 16f, -50f * 16f);

                Vector2 velocity =
                    (Main.MouseWorld - spawnPosition)
                        .SafeNormalize(Vector2.Zero) * 16f;

                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    spawnPosition,
                    velocity,
                    ModContent.ProjectileType<TheStromReCloud>(),
                    damage,
                    knockback,
                    player.whoAmI
                );
            }
            BowChangeVFX.SpawnMuzzle(Projectile, GunTipPosition, Projectile.velocity.SafeNormalize(Vector2.UnitX * player.direction) * 16f, BowChangeTheme.Storm, 0.9f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            int frameCount = Main.projFrames[Projectile.type];
            int frameHeight = texture.Height / frameCount;
            int frame = (int)(Main.GameUpdateCount / 6 % frameCount);

            Rectangle sourceRectangle =
                new Rectangle(0, frame * frameHeight, texture.Width, frameHeight);

            Vector2 origin = sourceRectangle.Size() * 0.5f;

            SpriteEffects effects =
                Projectile.spriteDirection == -1
                    ? SpriteEffects.FlipVertically
                    : SpriteEffects.None;

            Main.spriteBatch.Draw(
                texture,
                Projectile.Center - Main.screenPosition,
                sourceRectangle,
                lightColor,
                Projectile.rotation,
                origin,
                Projectile.scale,
                effects,
                0f
            );

            return false;
        }
    }
}
