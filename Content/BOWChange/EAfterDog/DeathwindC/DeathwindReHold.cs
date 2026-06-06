 
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Particles;
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.EAfterDog.DeathwindC
{
    internal class DeathwindReHold : BaseGunHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.EAfterDog";

        public override string Texture => "CalamityRangerExpansion/Content/BOWChange/EAfterDog/DeathwindC/DeathwindRe";

        public override int AssociatedItemID => ModContent.ItemType<DeathwindRe>();

        public override bool? CanDamage() => Main.player[Projectile.owner].GetModPlayer<OpenBowDamagePlayer>().OpenBowDamage;

        public override Vector2 GunTipPosition => Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * (Projectile.width * 0.5f + 10f);

        public override float MaxOffsetLengthFromArm => 15f; // 后坐力偏移


        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            if (Projectile.owner != Main.myPlayer)
                return true;

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int frameCount = Main.projFrames[Projectile.type];
            int frameHeight = texture.Height / frameCount;
            int currentFrame = (int)(Main.GameUpdateCount / 6 % frameCount);
            Rectangle sourceRectangle = new Rectangle(0, currentFrame * frameHeight, texture.Width, frameHeight);

            Vector2 drawOrigin = new Vector2(texture.Width / 2, frameHeight / 2);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
            SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;

            spriteBatch.Draw(texture, drawPosition, sourceRectangle, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0f);

            // === 绘制激光线 ===
            Texture2D laserTex = ModContent.Request<Texture2D>("CalamityRangerExpansion/texture/Weapon/TheScope").Value;

            float chargeProgress = MathHelper.Clamp(chargeTimer / MaxChargeTime, 0f, 1f);
            float offsetAngle = MathHelper.Lerp(MathHelper.ToRadians(25f), 0f, chargeProgress);
            Vector2 beamScale = new Vector2(1f, 2f);
            Vector2 origin = GunTipPosition;
            Color beamColor = Color.Lerp(Color.White, Color.GhostWhite, chargeProgress);

            Vector2 baseDir = Projectile.rotation.ToRotationVector2().RotatedBy(-MathHelper.PiOver2);
            Vector2 dir1 = baseDir.RotatedBy(offsetAngle);
            Vector2 dir2 = baseDir.RotatedBy(-offsetAngle);

            spriteBatch.Draw(laserTex, origin - Main.screenPosition, null, beamColor * 0.9f, dir1.ToRotation(), new Vector2(laserTex.Width / 2f, 0), beamScale, SpriteEffects.FlipVertically, 0f);
            spriteBatch.Draw(laserTex, origin - Main.screenPosition, null, beamColor * 0.9f, dir2.ToRotation(), new Vector2(laserTex.Width / 2f, 0), beamScale, SpriteEffects.FlipVertically, 0f);

            // === 绘制中央子弹展示 ===
            string texPath = currentColorVariant == 0
                ? "CalamityRangerExpansion/Content/BOWChange/EAfterDog/DeathwindC/DeathwindRePROJPurple"
                : "CalamityRangerExpansion/Content/BOWChange/EAfterDog/DeathwindC/DeathwindRePROJBlue";

            Texture2D bulletTex = ModContent.Request<Texture2D>(texPath).Value;
            float bulletScale = 0.6f + chargeProgress * 0.4f;
            Vector2 bulletCenter = GunTipPosition;
            Color bulletColor = Color.White * 0.9f;

            spriteBatch.Draw(bulletTex, bulletCenter - Main.screenPosition, null, bulletColor, Projectile.rotation + MathHelper.PiOver2, bulletTex.Size() * 0.5f, bulletScale, SpriteEffects.None, 0f);

            return false;
        }





        private const float BaseShootSpeed = 16f;
        private const float MaxChargeTime = 60f;
        private float chargeTimer = 0f;
        private bool chargeSoundPlayed = false;

        private int currentColorVariant = 0; // 0 = 蓝色，1 = 紫色

        public override void HoldoutAI()
        {
            chargeTimer = Math.Min(chargeTimer + 1f, MaxChargeTime);
            BowChangeVFX.SpawnCharge(Projectile, GunTipPosition, BowChangeTheme.Deathwind, chargeTimer / MaxChargeTime, 0.65f);

            // 随机决定本轮颜色
            if (chargeTimer == 1f)
                currentColorVariant = Main.rand.NextBool() ? 0 : 1;

            // ?蓄满音效 + 爆破特效（粉色）
            if (!chargeSoundPlayed && chargeTimer >= MaxChargeTime)
            {
                chargeSoundPlayed = true;
                BowChangeVFX.SpawnReadyBurst(Projectile, GunTipPosition, BowChangeTheme.Deathwind, 0.9f);

                SoundEngine.PlaySound(SoundID.Item4 with { Volume = 0.6f }, Projectile.Center);
                SoundEngine.PlaySound(SoundID.Item23 with { Volume = 0.5f }, Projectile.Center);
                SoundEngine.PlaySound(SoundID.Item94 with { Volume = 0.4f }, Projectile.Center);

                for (int i = 0; i < 12; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(6f, 6f).SafeNormalize(Vector2.UnitY).RotatedByRandom(0.4f) * Main.rand.NextFloat(4f, 8f);
                    CritSpark spark = new CritSpark(
                        GunTipPosition,
                        velocity,
                        Color.Fuchsia,
                        Color.HotPink,
                        1.6f,
                        24
                    );
                    GeneralParticleHandler.SpawnParticle(spark);
                }

                for (int i = 0; i < 10; i++)
                {
                    int dust = Dust.NewDust(GunTipPosition - Vector2.One * 8f, 16, 16, DustID.PinkTorch, 0f, 0f, 100, Color.HotPink, 1.7f);
                    Main.dust[dust].velocity *= 2.5f;
                    Main.dust[dust].noGravity = true;
                }
            }

            // ??持续吸收
            if (chargeTimer < MaxChargeTime)
                CreateAbsorbEffect();
        }

        public override void OnKill(int timeLeft)
        {
            float progress = MathHelper.Clamp(chargeTimer / MaxChargeTime, 0f, 1f);
            if (progress <= 0.5f)
                return;

            Player player = Main.player[Projectile.owner];
            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.UnitX * player.direction);
            BowChangeVFX.SpawnMuzzle(Projectile, GunTipPosition, direction * BaseShootSpeed, BowChangeTheme.Deathwind, 0.9f);

            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                GunTipPosition,
                direction * BaseShootSpeed,
                ModContent.ProjectileType<DeathwindRePROJ>(),
                (int)(Projectile.damage * MathHelper.Lerp(0.5f, 1f, progress)),
                Projectile.knockBack * progress,
                player.whoAmI,
                ai0: 0f,
                ai1: 0f,
                ai2: currentColorVariant // ??传递蓝/紫配色给弹幕
            );
        }

        private void CreateAbsorbEffect()
        {
            Vector2 center = GunTipPosition;

            // 根据当前颜色选择不同 dust 类型和颜色（已修正）
            bool isBlue = currentColorVariant == 0;
            int dustType = isBlue ? DustID.BlueTorch : DustID.PurpleTorch;
            Color dustColor = isBlue ? Color.Cyan : Color.MediumPurple;

            for (int i = 0; i < 4; i++)
            {
                Vector2 pos = center + Main.rand.NextVector2Circular(32f, 32f);
                Vector2 vel = (center - pos).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.4f, 0.9f);

                Dust dust = Dust.NewDustPerfect(
                    pos,
                    dustType,
                    vel,
                    100,
                    dustColor,
                    Main.rand.NextFloat(0.85f, 1.2f)
                );
                dust.noGravity = true;
                dust.fadeIn = 0.3f;
            }
        }


    }
}
