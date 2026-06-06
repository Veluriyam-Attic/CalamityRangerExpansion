using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using System;

namespace CalamityRangerExpansion.Content.BOWChange.CPreMoodLord.BlossomFluxC
{
    internal class BlossomFluxReBOMB : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.CPreMoodLord";

        // ================= 状态 =================
        private const int State_Attached = 0;
        private const int State_Launched = 1;

        private ref float State => ref Projectile.ai[0];
        private ref float ChargeProgress => ref Projectile.ai[1]; // 0~1

        private int launchTimer = 0;

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.scale = 1f;
        }

        // ================= AI =================
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!owner.active || owner.dead)
            {
                Projectile.Kill();
                return;
            }

            if (State == State_Attached)
            {
                // 永远跟随枪口
                if (owner.HeldItem.type != ModContent.ItemType<BlossomFluxRe>())
                {
                    Projectile.Kill();
                    return;
                }

                // 枪口位置（跟 Hold 完全一致）
                Vector2 armPos = owner.RotatedRelativePoint(owner.MountedCenter, true);
                Vector2 aimDir = (Main.MouseWorld - armPos).SafeNormalize(Vector2.UnitX * owner.direction);
                Vector2 gunTip = armPos + aimDir * 32f;
                Projectile.Center = gunTip;
                Projectile.velocity = Vector2.Zero;

                // 蓄力缩放：50% → 300%
                float baseScale = MathHelper.Lerp(0.5f, 3.0f, MathHelper.Clamp(ChargeProgress, 0f, 1f));

                // 小幅呼吸脉动
                float pulse = 1f + (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f) * 0.05f;
                Projectile.scale = baseScale * pulse;

                Projectile.alpha = 0;
                return;
            }

            // ================= 发射阶段 =================
            if (State == State_Launched)
            {
                launchTimer++;

                if (launchTimer == 1)
                {
                    Vector2 armPos = owner.RotatedRelativePoint(owner.MountedCenter, true);
                    Vector2 aimDir = (Main.MouseWorld - armPos).SafeNormalize(Vector2.UnitX * owner.direction);

                    Projectile.velocity = aimDir * 18f;
                    SoundEngine.PlaySound(SoundID.Item62, Projectile.Center);
                }

                // 每帧减速
                Projectile.velocity *= 0.99f;

                // 20 帧后爆炸
                if (launchTimer >= 20)
                {
                    Explode();
                }
            }
        }

        // ================= 爆炸 =================
        private void Explode()
        {
            if (Projectile.owner == Main.myPlayer)
            {
                // 创建真正的爆炸弹幕（特效不在这里）
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<BlossomFluxReEXP>(),
                    (int)(Projectile.damage * MathHelper.Lerp(0.3f, 1f, ChargeProgress)),
                    Projectile.knockBack,
                    Projectile.owner
                );

                // 叶子四散
                int leafCount = (int)MathHelper.Lerp(1f, 24f, ChargeProgress);
                for (int i = 0; i < leafCount; i++)
                {
                    Vector2 v = Main.rand.NextVector2Unit() * Main.rand.NextFloat(8f, 14f);
                    Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        Projectile.Center,
                        v,
                        ModContent.ProjectileType<BlossomFluxReLEAF>(),
                        Projectile.damage,
                        Projectile.knockBack,
                        Projectile.owner
                    );
                }
            }

            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            Projectile.Kill();
        }

        // ================= 绘制 =================
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = tex.Size() * 0.5f;

            Main.spriteBatch.Draw(
                tex,
                Projectile.Center - Main.screenPosition,
                null,
                Color.White,
                0f,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0f
            );
            return false;
        }
    }
}
