using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.HD2.LAS17
{
    internal class LAS17Hold : BaseGunHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.LAS17";
        public override string Texture => "CalamityRangerExpansion/Content/DeveloperItems/Weapon/HD2/LAS17/LAS17";
        public override int AssociatedItemID => ModContent.ItemType<LAS17>();
        public override Vector2 GunTipPosition => Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * (Projectile.width * 0.5f + 10f);
        public override float MaxOffsetLengthFromArm => 55f;

        private int frameCounter = 0;
        private int stage = 0;
        private int stageTimer = 0;
        private const int MaxStage = 5;
        private const int FireInterval = 4;
        private const int StageUpTime = 180; // 每180帧（3秒）升级一次
        private const int StageCooldownSpeed = 12; // 冷却时每帧倒退多少tick
        private float chargeProgress = 0f;

        public override void HoldoutAI()
        {
            Player player = Main.player[Projectile.owner];

            // 若未持有该武器，则重置
            if (player.HeldItem.type != AssociatedItemID)
            {
                ResetStage();
                return;
            }

            frameCounter++;
            stageTimer++;

            // 射击逻辑
            if (frameCounter >= FireInterval)
            {
                Fire(player);
                frameCounter = 0;
            }

            // 强化逻辑：3秒一级，最多5级
            if (stage < MaxStage && stageTimer >= StageUpTime)
            {
                stage++;
                stageTimer = 0;
                TriggerStageEffect(player);
            }

            // 后坐力模拟（简单推后）
            OffsetLengthFromArm -= 2f;

            // 更新阶段充能进度（满级后直接保持满值）
            if (stage < MaxStage)
                chargeProgress = stageTimer / (float)StageUpTime;
            else
                chargeProgress = 1f;

        }
        public override bool PreDraw(ref Color lightColor)
        {
            // 👉我们只要绘制阶段充能条，不影响主视觉
            if (Main.myPlayer == Projectile.owner && stage < MaxStage)
            {
                var barBG = ModContent.Request<Texture2D>("CalamityMod/UI/MiscTextures/GenericBarBack").Value;
                var barFG = ModContent.Request<Texture2D>("CalamityMod/UI/MiscTextures/GenericBarFront").Value;

                Vector2 drawPos = Owner.Center - Main.screenPosition + new Vector2(0, -56f) - barBG.Size() / 1.5f;
                Rectangle frameCrop = new Rectangle(0, 0, (int)(barFG.Width * chargeProgress), barFG.Height);

                float opacity = 1f;
                Color color = Color.Orange;

                Main.spriteBatch.Draw(barBG, drawPos, null, color * opacity, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(barFG, drawPos, frameCrop, color * opacity * 0.8f, 0f, Vector2.Zero, 1.5f, SpriteEffects.None, 0f);
            }

            return base.PreDraw(ref lightColor);
        }


        public override void OnKill(int timeLeft)
        {
            ResetStage();
        }

        private void ResetStage()
        {
            stage = 0;
            stageTimer = 0;
        }

        private void Fire(Player player)
        {
            //Vector2 direction = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitX);// 瞄准鼠标，这不对
            Vector2 direction =
                Vector2.UnitX.RotatedBy(Projectile.rotation)
                    .RotatedBy(Main.rand.NextFloat(
                        -MathHelper.ToRadians(2f),
                         MathHelper.ToRadians(2f)
                    ));

            int proj = ModContent.ProjectileType<LAS17Proj>();
            int damage = Projectile.damage;
            float knockback = Projectile.knockBack;

            // 创建子弹并传入当前阶段
            int index = Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                GunTipPosition,
                direction * 25f,
                proj,
                damage,
                knockback,
                player.whoAmI
            );

            if (Main.projectile.IndexInRange(index))
            {
                Projectile p = Main.projectile[index];
                if (p.ModProjectile is LAS17Proj spike)
                {
                    spike.WeaponStage = stage;
                }
            }

            // 播放基础音效
            SoundEngine.PlaySound(SoundID.Item5, Projectile.Center);

            // 3级及以上 → 在玩家身上制造橙色旋转重烟，模拟灼烧
            if (stage >= 3)
            {
                for (int i = 0; i < 2; i++)
                {
                    Particle burnSmoke = new HeavySmokeParticle(
                        player.Center + new Vector2(0, -6),
                        new Vector2(0, -1).RotatedByRandom(MathHelper.ToRadians(45f)) * Main.rand.NextFloat(3f, 6f),
                        Color.Orange,
                        30,
                        Main.rand.NextFloat(0.9f, 1.4f),
                        1f,
                        MathHelper.ToRadians(Main.rand.NextFloat(-3f, 3f)),
                        true
                    );
                    GeneralParticleHandler.SpawnParticle(burnSmoke);
                }
            }

            if (stage >= 5)
            {
                LAS17PDebuff.FireMode = 2;
                player.AddBuff(ModContent.BuffType<LAS17PDebuff>(), 300); // 5秒刷新
            }
            else if (stage >= 3)
            {
                LAS17PDebuff.FireMode = 1;
                player.AddBuff(ModContent.BuffType<LAS17PDebuff>(), 300); // 5秒刷新
            }


            // 5级 → 加强开火视觉（10倍爆量的橙红粒子）
            if (stage >= 5)
            {
                for (int i = 0; i < 30; i++)
                {
                    Dust dust = Dust.NewDustPerfect(
                        GunTipPosition,
                        DustID.Torch,
                        direction.RotatedByRandom(0.6f) * Main.rand.NextFloat(2f, 6f),
                        150,
                        Color.OrangeRed,
                        Main.rand.NextFloat(1.2f, 2f)
                    );
                    dust.noGravity = true;
                }
            }

            // 后坐力模拟（更强烈）
            OffsetLengthFromArm -= stage >= 5 ? 4f : 2f;
        }

        private void TriggerStageEffect(Player player)
        {
            Vector2 fireDirection = Vector2.UnitX.RotatedBy(Projectile.rotation);

            // 🔥1. 火把 Dust 粒子：喷射向前
            for (int i = 0; i < 25; i++)
            {
                Vector2 velocity = fireDirection.RotatedByRandom(MathHelper.ToRadians(15)) * Main.rand.NextFloat(6f, 12f);
                Dust dust = Dust.NewDustPerfect(GunTipPosition, DustID.Torch, velocity, 150, Color.Orange, Main.rand.NextFloat(1.2f, 2f));
                dust.noGravity = true;
            }

            // ⚡2. Spark 橙色粒子：速度更快更亮
            for (int i = 0; i < 8; i++)
            {
                Vector2 sparkVel = fireDirection.RotatedByRandom(MathHelper.ToRadians(10)) * Main.rand.NextFloat(8f, 16f);
                Particle spark = new SparkParticle(
                    GunTipPosition,
                    sparkVel,
                    false,
                    30,
                    1.2f,
                    Color.Orange
                );
                GeneralParticleHandler.SpawnParticle(spark);
            }

            // 💥3. 音效更清脆强烈（替代旧烟雾音）
            SoundEngine.PlaySound(SoundID.Item14.WithPitchOffset(0.15f), GunTipPosition);
        }












    }
}
