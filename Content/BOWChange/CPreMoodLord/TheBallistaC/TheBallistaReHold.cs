using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityRangerExpansion.Content.BOWChange.BPrePlantera.ArbalestC;
using CalamityMod.Particles;
using System;
using CalamityMod;
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.CPreMoodLord.TheBallistaC
{
    internal class TheBallistaReHold : BaseGunHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.CPreMoodLord";

        public override string Texture => "CalamityRangerExpansion/Content/BOWChange/CPreMoodLord/TheBallistaC/TheBallistaRe";

        public override int AssociatedItemID => ModContent.ItemType<TheBallistaRe>();

        public override bool? CanDamage() => false;

        public override Vector2 GunTipPosition => Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * (Projectile.width * 0.5f + 10f);

        public override float MaxOffsetLengthFromArm => 15f;



        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            // 只在本地玩家绘制瞄准效果
            if (Projectile.owner != Main.myPlayer)
                return true;

            {
                SpriteBatch spriteBat1ch = Main.spriteBatch;
                Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

                // 计算当前动画帧
                int frameCount = Main.projFrames[Projectile.type]; // 获取帧数
                int frameHeight = texture.Height / frameCount; // 每帧的高度
                int currentFrame = (int)(Main.GameUpdateCount / 6 % frameCount); // 每 6 帧切换一次帧
                Rectangle sourceRectangle = new Rectangle(0, currentFrame * frameHeight, texture.Width, frameHeight);

                // 设置绘制的原点和位置
                Vector2 drawOrigin = new Vector2(texture.Width / 2, frameHeight / 2);
                Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

                // 检查是否需要翻转图片
                SpriteEffects spriteEffects = SpriteEffects.None;
                if (Projectile.spriteDirection == -1)
                {
                    spriteEffects = SpriteEffects.FlipVertically;
                }

                // 绘制当前帧
                spriteBat1ch.Draw(texture, drawPosition, sourceRectangle, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0f);

            }



            // === 1. 绘制激光线（左右两条） ===
            Texture2D laserTex = ModContent.Request<Texture2D>("CalamityRangerExpansion/texture/Weapon/TheScope").Value;

            float chargeProgress = MathHelper.Clamp(chargeTimer / MaxChargeTime, 0f, 1f);
            float maxOffsetAngle = MathHelper.ToRadians(25f);
            float offsetAngle = MathHelper.Lerp(maxOffsetAngle, 0f, chargeProgress);

            Vector2 beamScale = new Vector2(1f, 2f); // 宽度保持1，高度拉长2倍
            float beamLength = 1200f;
            Vector2 origin = GunTipPosition;

            // 颜色从白色 → GhostWhite（淡蓝白）
            Color beamColor = Color.Lerp(Color.White, Color.GhostWhite, chargeProgress);

            Vector2 baseDir = Projectile.rotation.ToRotationVector2().RotatedBy(-MathHelper.PiOver2);
            Vector2 dir1 = baseDir.RotatedBy(offsetAngle);
            Vector2 dir2 = baseDir.RotatedBy(-offsetAngle);

            //spriteBatch.Draw(laserTex, origin - Main.screenPosition, null, beamColor * 0.9f, dir1.ToRotation(), new Vector2(laserTex.Width / 2f, 0), beamScale, SpriteEffects.FlipVertically, 0f);
            //spriteBatch.Draw(laserTex, origin - Main.screenPosition, null, beamColor * 0.9f, dir2.ToRotation(), new Vector2(laserTex.Width / 2f, 0), beamScale, SpriteEffects.FlipVertically, 0f);



            // === 2. 中央子弹展示（随蓄力变大） ===
            Texture2D bulletTex = ModContent.Request<Texture2D>("CalamityRangerExpansion/Content/BOWChange/CPreMoodLord/TheBallistaC/TheBallistaRePROJ").Value;
            float bulletScale = 0.6f + chargeProgress * 0.4f;
            Vector2 bulletCenter = GunTipPosition;
            Color bulletColor = Color.White * 0.9f;

            spriteBatch.Draw(bulletTex, bulletCenter - Main.screenPosition, null, bulletColor, Projectile.rotation + MathHelper.PiOver2, bulletTex.Size() * 0.5f, bulletScale, SpriteEffects.FlipVertically, 0f);


            //// === 绘制弓弦线（上端 → 中心 & 下端 → 中心） ===【暂时封存，因为暂时用不到】
            //Vector2 topOfBow = Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation + TopStringOffset.ToRotation()) * TopStringOffset.Length();
            //Vector2 bottomOfBow = Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation + BottomStringOffset.ToRotation()) * BottomStringOffset.Length();

            //// 弦中点（随蓄力拉远）
            //Vector2 endOfString = Projectile.Center - Projectile.rotation.ToRotationVector2().RotatedBy(-MathHelper.PiOver2) * (25f + 40f * chargeProgress);

            //Color stringColor = Color.White * 0.6f;
            //Main.spriteBatch.DrawLineBetter(topOfBow, endOfString, stringColor, 2f);
            //Main.spriteBatch.DrawLineBetter(bottomOfBow, endOfString, stringColor, 2f);


            return false; // 拒绝系统绘制本体贴图
        }
        //// === 临时定义弓弦的上下锚点（之后改精确值） ===
        //private Vector2 TopStringOffset => new Vector2(-30f, -40f);    // ← 你要自己调这两个数
        //private Vector2 BottomStringOffset => new Vector2(-30f, 40f);  // ← 推荐照贴图上锚点定





        private const float BaseShootSpeed = 14f; // 保留原速度设定
        private const float MaxChargeTime = 60f;  // 总蓄力时间
        private float chargeTimer = 0f;           // 当前蓄力进度
        private bool chargeSoundPlayed = false;   // 音效播放标志

        public override void HoldoutAI()
        {
            // 线性蓄力，每帧累加
            chargeTimer = Math.Min(chargeTimer + 1f, MaxChargeTime);
            BowChangeVFX.SpawnCharge(Projectile, GunTipPosition, BowChangeTheme.Ballista, chargeTimer / MaxChargeTime, 0.65f);

            // 播放蓄满音效（仅一次）
            if (!chargeSoundPlayed && chargeTimer >= MaxChargeTime)
            {
                chargeSoundPlayed = true;
                BowChangeVFX.SpawnReadyBurst(Projectile, GunTipPosition, BowChangeTheme.Ballista, 0.9f);
                SoundEngine.PlaySound(SoundID.Item4 with { Volume = 0.6f }, Projectile.Center);
                SoundEngine.PlaySound(SoundID.Item23 with { Volume = 0.5f }, Projectile.Center);
                SoundEngine.PlaySound(SoundID.Item94 with { Volume = 0.4f }, Projectile.Center);

                // ? 蓄满爆破特效（极短高亮爆炸 + 电光）
                for (int i = 0; i < 12; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(6f, 6f);
                    CritSpark spark = new CritSpark(
                        GunTipPosition,
                        velocity,
                        Color.Yellow,
                        Color.OrangeRed,
                        1.4f,
                        24
                    );
                    GeneralParticleHandler.SpawnParticle(spark);
                }

                for (int i = 0; i < 10; i++)
                {
                    int dust = Dust.NewDust(GunTipPosition - Vector2.One * 8f, 16, 16, DustID.Electric, 0f, 0f, 100, default, 1.5f);
                    Main.dust[dust].velocity *= 2f;
                    Main.dust[dust].noGravity = true;
                }
            }

            // 吸收特效
            if (chargeTimer < MaxChargeTime)
                CreateAbsorbEffect(); // 蓄满后关闭吸收特效

            //{
            //    // === 动态拉弓手臂动作同步 ===[暂时封存，因为暂时不用]
            //    Vector2 top = Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation + TopStringOffset.ToRotation()) * TopStringOffset.Length();
            //    Vector2 bot = Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation + BottomStringOffset.ToRotation()) * BottomStringOffset.Length();
            //    float stringHalfHeight = Math.Abs(top.Y - bot.Y) * 0.5f;
            //    float pullBackDist = 25f + 40f * chargeTimer / MaxChargeTime;

            //    float frontArmRotation = (float)Math.Atan(stringHalfHeight / Math.Max(pullBackDist, 0.001f)) * 0.5f;
            //    frontArmRotation += Projectile.rotation + MathHelper.Pi + Main.player[Projectile.owner].direction * MathHelper.PiOver2 + 0.12f;

            //    Player owner = Main.player[Projectile.owner];
            //    owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, frontArmRotation);
            //    owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() - MathHelper.PiOver2);
            //}
        }

        public override void OnKill(int timeLeft)
        {
            float progress = MathHelper.Clamp(chargeTimer / MaxChargeTime, 0f, 1f);
            if (progress <= 0.5f) // 小于百分之多少的蓄力，拒绝发射，防止白嫖
                return;

            Player player = Main.player[Projectile.owner];
            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.UnitX * player.direction);
            BowChangeVFX.SpawnMuzzle(Projectile, GunTipPosition, direction * BaseShootSpeed, BowChangeTheme.Ballista, 1f);

            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                GunTipPosition,
                direction * BaseShootSpeed,
                ModContent.ProjectileType<TheBallistaRePROJ>(),
                (int)(Projectile.damage * MathHelper.Lerp(0.5f, 1f, progress)),
                Projectile.knockBack * progress,
                player.whoAmI,
                ai0: progress // 使用 progress 作为传参
            );
        }
        private void CreateAbsorbEffect()
        {
            Vector2 center = GunTipPosition;

            for (int i = 0; i < 4; i++)
            {
                Vector2 pos = center + Main.rand.NextVector2Circular(2*16f, 2*16f); // ??范围极小
                Vector2 vel = (center - pos).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.4f, 0.9f); // ??速度也极低

                Dust dust = Dust.NewDustPerfect(
                    pos,
                    DustID.Torch, // ??粒子类型：橘黄土色
                    vel,
                    100,
                    new Color(120, 100, 60), // 土黄偏橄榄色
                    Main.rand.NextFloat(0.73f, 0.95f) // ??非常小的尺寸
                );
                dust.noGravity = true;
                dust.fadeIn = 0.2f;
            }
        }



    }
}
