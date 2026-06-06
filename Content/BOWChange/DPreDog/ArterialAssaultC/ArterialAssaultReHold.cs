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
using System;
using CalamityMod.Particles;
using CalamityRangerExpansion.Content.BOWChange.CPreMoodLord.TheBallistaC;
using CalamityRangerExpansion.Content.BOWChange;
using System.Collections.Generic;
using CalamityMod.Items.Ammo;

namespace CalamityRangerExpansion.Content.BOWChange.DPreDog.ArterialAssaultC
{
    internal class ArterialAssaultReHold : BaseGunHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.DPreDog";

        public override string Texture => "CalamityRangerExpansion/Content/BOWChange/DPreDog/ArterialAssaultC/ArterialAssaultRe";

        public override int AssociatedItemID => ModContent.ItemType<ArterialAssaultRe>();

        public override bool? CanDamage() => Main.player[Projectile.owner].GetModPlayer<OpenBowDamagePlayer>().OpenBowDamage;

        public override Vector2 GunTipPosition => Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * (Projectile.width * 0.5f + 10f);

        public override float MaxOffsetLengthFromArm => 15f; // 后坐力偏移


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

            spriteBatch.Draw(laserTex, origin - Main.screenPosition, null, beamColor * 0.9f, dir1.ToRotation(), new Vector2(laserTex.Width / 2f, 0), beamScale, SpriteEffects.FlipVertically, 0f);
            spriteBatch.Draw(laserTex, origin - Main.screenPosition, null, beamColor * 0.9f, dir2.ToRotation(), new Vector2(laserTex.Width / 2f, 0), beamScale, SpriteEffects.FlipVertically, 0f);

            // === 2. 中央子弹展示（随蓄力变大） ===
            Texture2D bulletTex = ModContent.Request<Texture2D>("CalamityRangerExpansion/Content/BOWChange/DPreDog/ArterialAssaultC/ArterialAssaultRePROJ").Value;
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
        private int bloodCostTimer = 0;

        public override void HoldoutAI()
        {
            // 线性蓄力，每帧累加
            chargeTimer = Math.Min(chargeTimer + 1f, MaxChargeTime);
            BowChangeVFX.SpawnCharge(Projectile, GunTipPosition, BowChangeTheme.Arterial, chargeTimer / MaxChargeTime, 0.75f);

            // 播放蓄满音效（仅一次）
            if (!chargeSoundPlayed && chargeTimer >= MaxChargeTime)
            {
                chargeSoundPlayed = true;
                BowChangeVFX.SpawnReadyBurst(Projectile, GunTipPosition, BowChangeTheme.Arterial, 1f);
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

            {
                // 在 HoldoutAI 尾部追加（控制粒子蛇形运动）
                for (int i = bloodSparks.Count - 1; i >= 0; i--)
                {
                    SparkParticle p = bloodSparks[i];
                    if (p.Time >= p.Lifetime)
                    {
                        bloodSparks.RemoveAt(i);
                        continue;
                    }

                    // 蛇形左右摆动
                    int cycle = 10;
                    int phase = p.Time % cycle;
                    float rotateAmount = MathHelper.ToRadians(2f);

                    if (phase < 5)
                        p.Velocity = p.Velocity.RotatedBy(-rotateAmount);
                    else
                        p.Velocity = p.Velocity.RotatedBy(rotateAmount);

                    // 螺旋吸附加强（模拟“血脉吸收”）
                    p.Position += Vector2.UnitY.RotatedBy(Main.GameUpdateCount * 0.04f) * 0.2f;
                }
            }


            {
                // 蓄力通过生命换威力，但按节拍扣血，避免一帧一跳造成过高负担。
                Player player = Main.player[Projectile.owner];

                if (chargeTimer < MaxChargeTime && ++bloodCostTimer >= 8)
                {
                    bloodCostTimer = 0;
                    int damage = 3;
                    if (player.statLife > damage)
                    {
                        player.statLife -= damage;

                        CombatText.NewText(player.Hitbox, Color.Red, damage, true, false);
                    }
                    else
                    {
                        // 若血量不足，不再继续扣血，防止自杀
                        chargeTimer = MaxChargeTime; // 直接蓄满终止
                    }
                }

            }

        }
        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];

            float progress = MathHelper.Clamp(chargeTimer / MaxChargeTime, 0f, 1f); // 蓄力进度
            int count = (int)MathHelper.Lerp(1, 10, progress); // 发射数量：1~10 发
            float angleSpread = MathHelper.ToRadians(3f); // 小角度抖动
            Vector2 baseDirection = Projectile.velocity.SafeNormalize(Vector2.UnitX * player.direction);
            BowChangeVFX.SpawnMuzzle(Projectile, GunTipPosition, baseDirection * MathHelper.Lerp(9f, 15f, progress), BowChangeTheme.Arterial, 1f);

            for (int i = 0; i < count; i++)
            {
                Vector2 fireDir = baseDirection.RotatedByRandom(angleSpread);
                float speed = MathHelper.Lerp(9f, 15f, progress); // 射速随蓄力增加

                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    GunTipPosition,
                fireDir * speed,
                    ModContent.ProjectileType<ArterialAssaultRePROJ>(), // 替换为你实际的子弹类型
                    (int)(Projectile.damage * MathHelper.Lerp(0.6f, 1f, progress)), // 伤害随进度线性提升
                    Projectile.knockBack,
                    player.whoAmI,
                    progress // 传入 ai[0] 作为进度参数
                );
            }
        }

        // 添加字段（类字段区）
        private List<SparkParticle> bloodSparks = new();

        // 替换原有 CreateAbsorbEffect
        private void CreateAbsorbEffect()
        {
            Vector2 center = GunTipPosition;

            for (int i = 0; i < 2; i++)
            {
                Vector2 spawnPos = center + Main.rand.NextVector2Circular(40f, 40f);
                Vector2 velocity = (center - spawnPos).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.5f, 1.2f);

                // 血色流动粒子
                SparkParticle blood = new SparkParticle(
                    spawnPos,
                    velocity,
                    false,
                    60,
                    Main.rand.NextFloat(0.9f, 1.3f),
                    Color.Lerp(Color.DarkRed, Color.IndianRed, Main.rand.NextFloat(0.3f, 0.7f)) * 0.8f
                );

                GeneralParticleHandler.SpawnParticle(blood);
                bloodSparks.Add(blood);
            }
        }










    }
}
