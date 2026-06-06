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
using CalamityMod.Projectiles.Ranged;
using CalamityMod;
using System;

namespace CalamityRangerExpansion.Content.BOWChange.CPreMoodLord.BlossomFluxC
{
    internal class BlossomFluxReHold : BaseGunHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.CPreMoodLord";

        public override string Texture => "CalamityRangerExpansion/Content/BOWChange/CPreMoodLord/BlossomFluxC/BlossomFluxRe";

        public override int AssociatedItemID => ModContent.ItemType<BlossomFluxRe>();

        public override bool? CanDamage() => Main.player[Projectile.owner].GetModPlayer<OpenBowDamagePlayer>().OpenBowDamage;

        public override Vector2 GunTipPosition => Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * (Projectile.width * 0.5f + 10f);

        public override float MaxOffsetLengthFromArm => 15f; // 后坐力偏移



        // =================== 可调参数 ===================
        private const int MaxChargeTime = 120;     // 蓄力最大帧数（不到不发叶子）
        private const int BombInterval = 7;       // （保留常量，充能期间不再使用）
        private const int BombMin = 2;       // 炸弹最少数量（在发射阶段按周期触发）
        private const int BombMax = 4;       // 炸弹最多数量（在发射阶段按周期触发）
        private const float BombSpeed = 7.5f;    // 炸弹速度
        private const float BombSpreadDeg = 35f;     // 炸弹基础散射角（度）

        private const int LeafTotal = 54;      // 叶子总数（松手连发）
        private const int LeafInterval = 2;       // 叶子发射间隔（帧）
        private const float LeafSpeed = 16f;     // 叶子速度
        private const float LeafMaxSpread = 10f;     // 叶子最大散射角（度），从0线性涨到这个值

        // 颜色：蓄力=吸附/聚拢；发射=喷射/外放（同色系不同气质）
        private static readonly Color ChargeColorA = new Color(120, 255, 160);
        private static readonly Color ChargeColorB = new Color(160, 255, 220);
        private static readonly Color FireColorA = new Color(180, 255, 120);
        private static readonly Color FireColorB = new Color(110, 235, 255);
        // ================================================

        // 状态量（用 ai/localAI 便于网络同步）
        private ref float ChargeFrames => ref Projectile.ai[0];    // 已蓄力帧数
        private ref float ShotsRemaining => ref Projectile.ai[1];    // 剩余叶子数（>0 代表正在连发）
        private ref float LeafCooldown => ref Projectile.localAI[0]; // 叶子冷却计时

        private bool fullyCharged = false; // 是否已达到 MaxChargeTime
        private bool pingFullOnce = false; // 蓄满提示是否已播
        private bool hasTriggeredReadyFX = false;

        private int bombIndex = -1;

        public override void HoldoutAI()
        {
            Player player = Owner;

            if (ChargeFrames == 1 && bombIndex == -1 && Projectile.owner == Main.myPlayer)
            {
                bombIndex = Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    GunTipPosition,
                    Vector2.Zero,
                    ModContent.ProjectileType<BlossomFluxReBOMB>(),
                    Projectile.damage,
                    Projectile.knockBack,
                    Projectile.owner,
                    ai0: 0f,
                    ai1: 0f
                );
            }
            if (bombIndex >= 0 && Main.projectile[bombIndex].active)
            {
                Main.projectile[bombIndex].ai[1] =
                    MathHelper.Clamp(ChargeFrames / MaxChargeTime, 0f, 1f);
            }


            // —— 阶段1：蓄力 —— //
            if (!fullyCharged && player.channel)
            {
                if (ChargeFrames < MaxChargeTime)
                {
                    ChargeFrames++;

                    // 蓄力期间：森林花粉/叶屑上浮 + 极少量藤脉能量线（AltSpark）
                    SpawnChargeFX_EveryFrame(GunTipPosition);
                }
                else
                {
                    // 满蓄持续：改为“外泄式”的绿雾叶粉（Dust 为主），表现与蓄力相反的动势
                    SpawnChargeFX_Complete(GunTipPosition);

                    // 满蓄瞬间：只触发一次“林海绽放”
                    if (!hasTriggeredReadyFX && ShotsRemaining <= 0)
                    {
                        SpawnChargeReadyOnceFX(GunTipPosition);
                        hasTriggeredReadyFX = true;
                    }

                    fullyCharged = true; // ? 标记已满蓄
                }
            }

            // —— 阶段2：松手后攻击 —— //
            if (!player.channel)
            {
                if (bombIndex >= 0 && Main.projectile[bombIndex].active)
                {
                    Main.projectile[bombIndex].ai[0] = 1f; // 发射指令
                }
                Projectile.Kill();
            }

            // 满蓄且仍在长按 → 什么都不做（等松手触发 KillHoldoutLogic）
        }


   

        // ================= 发射逻辑（现在把炸弹逻辑放在发射阶段） =================

        /// <summary>
        /// 在叶子主流中周期性触发炸弹散射（辅助手段）
        /// 设计理念（植物学风格）：
        /// - 叶子像种子逐一外放（主流），炸弹像成熟的“种荚/孢子荚”在叶群中周期性散落；
        /// - 随着连发进行（越接近尾端），炸弹更频繁/数量更多/散射角更大，模拟“成熟阶段更多散播”；
        /// - 触发规则：每 N 发叶子（这里用 9 发作为一个周期）触发一次炸弹散射；数量 = BombMin..BombMax，再按进度线性放大到上限。
        /// </summary>
        private void FireBombs_AtFiringStage(Vector2 origin, Vector2 forward, int firedSoFar)
        {
            Player player = Owner;

            // 计算进度比（0..1），越靠后阶段，炸弹更猛烈
            float progress = MathHelper.Clamp(firedSoFar / (float)Math.Max(1, LeafTotal - 1), 0f, 1f);

            // 基础数量，并随进度微幅增长（线性）
            int baseNum = Main.rand.Next(BombMin, BombMax + 1);
            int extra = (int)Math.Floor(progress * (BombMax - BombMin)); // 进度带来的额外量
            int num = Math.Min(BombMax, baseNum + extra);

            // 散射角随着进度线性放大（基础 BombSpreadDeg → 1.5x）
            float spreadDegNow = MathHelper.Lerp(BombSpreadDeg * 0.6f, BombSpreadDeg * 1.5f, progress);

            for (int i = 0; i < num; i++)
            {
                float ang = MathHelper.ToRadians(Main.rand.NextFloat(-spreadDegNow, spreadDegNow));
                Vector2 v = forward.RotatedBy(ang) * (BombSpeed * Main.rand.NextFloat(0.85f, 1.15f));

                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    origin,
                    v,
                    ModContent.ProjectileType<BlossomFluxReBOMB>(),
                    Projectile.damage, // 使用基础伤害（可后续按progress缩放）
                    Projectile.knockBack,
                    player.whoAmI
                );
            }

            // 炸弹群发音效（原版爆裂类）
            SoundEngine.PlaySound(SoundID.Item14 with { Volume = 1.2f, Pitch = -0.0f }, Projectile.Center);

            // 额外少量 Dust 用于视觉（草绿色）
            for (int k = 0; k < Math.Clamp(num * 2, 3, 10); k++)
            {
                Dust d = Dust.NewDustPerfect(
                    origin + Main.rand.NextVector2Circular(4f, 4f),
                    DustID.Grass,
                    forward.RotatedByRandom(0.9f) * Main.rand.NextFloat(1f, 3f),
                    110,
                    Color.Lerp(new Color(100, 255, 150), new Color(150, 255, 200), Main.rand.NextFloat()),
                    Main.rand.NextFloat(0.9f, 1.4f)
                );
                d.noGravity = true;
            }
        }

        // 松手连发一片叶子（散射角随进度线性增大），并在周期点触发炸弹散射（辅助手段）
        private void FireLeaf()
        {
            Player player = Owner;
            Vector2 forward = Projectile.velocity.SafeNormalize(Vector2.UnitX * player.direction);

            int fired = LeafTotal - (int)ShotsRemaining; // 已经发出的数量（0-based）
            float ratio = LeafTotal <= 1 ? 1f : fired / (float)(LeafTotal - 1);
            float spreadNow = MathHelper.ToRadians(MathHelper.Lerp(0f, LeafMaxSpread, ratio));

            float ang = Main.rand.NextFloat(-spreadNow, spreadNow);
            Vector2 v = forward.RotatedBy(ang) * LeafSpeed;

            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                GunTipPosition,
                v,
                ModContent.ProjectileType<BlossomFluxReLEAF>(),
                Projectile.damage,
                Projectile.knockBack,
                player.whoAmI
            );

            // 叶片喷射音效（原版弓箭/轻脉冲）
            SoundEngine.PlaySound(SoundID.Item5 with { Volume = 1.2f, Pitch = -0.0f }, Projectile.Center);

            // 叶片喷射 FX：亮色线性Spark + 点状高光 + 少量草色Dust
            SpawnPerShotFX(GunTipPosition, v.SafeNormalize(Vector2.UnitX));

            // ---------- 辅助：周期性释放炸弹（植物学“种荚散播”） ----------
            // 每 9 发触发一次炸弹散射（你可以把 9 调整为别的值）
            int cycleLen = 9;
            if (fired > 0 && fired % cycleLen == 0)
            {
                // 在发射源点 origin（枪口）触发炸弹
                FireBombs_AtFiringStage(GunTipPosition, forward, fired);
            }

            // 另外：在整轮的中后段增加一次“集中爆发”的额外炸弹（例如当进度超过0.75时，额外再发一组）
            if (ratio > 0.75f && Main.rand.NextBool(6))
            {
                FireBombs_AtFiringStage(GunTipPosition, forward, fired + 1);
            }
        }








        // === 1) 蓄力期间 · 持续（林息聚拢：矩形上浮 + 少量藤脉线） ===
        // 设计：Dust 绝对主力。方框内随机生成“花粉/叶屑”缓缓上浮；极少量 AltSpark 模拟植物能量脉络。
        // 颜色：以草绿色为主，少量青绿高光作点缀。
        private void SpawnChargeFX_EveryFrame(Vector2 pos)
        {
            // 方框上浮：宽20, 高6（中心略在枪口下），像枝叶呼吸时的细粉
            Vector2 boxCenter = pos + new Vector2(0f, 6f);
            float hw = 10f, hh = 3f;

            // Dust——草系粉尘为主
            int count = Main.rand.Next(6, 10); // 密度略高但颗粒小
            for (int i = 0; i < count; i++)
            {
                Vector2 spawn = boxCenter + new Vector2(Main.rand.NextFloat(-hw, hw), Main.rand.NextFloat(-hh, hh));
                Vector2 vel = new Vector2(Main.rand.NextFloat(-0.18f, 0.18f), Main.rand.NextFloat(-1.25f, -0.7f)); // 正上方为主
                int dID = DustID.Grass; // 纯草系最安全
                int id = Dust.NewDust(spawn, 0, 0, dID, vel.X, vel.Y, 120);
                Dust d = Main.dust[id];
                d.noGravity = true;
                d.scale = Main.rand.NextFloat(0.75f, 0.95f); // 颗粒小，避免糊
                d.color = Color.Lerp(new Color(110, 255, 150), new Color(160, 255, 200), Main.rand.NextFloat(0.25f, 0.8f));
            }

            // 偶发藤脉能量线（AltSparkParticle）——极少量点缀，不能喧宾夺主
            if (Main.rand.NextBool(1, 6))
            {
                var vine = new CalamityMod.Particles.AltSparkParticle(
                    pos - Projectile.velocity * 1.5f,
                    Projectile.velocity * 0.01f, // 几乎静止
                    false,
                    10,
                    1.15f,
                    new Color(60, 220, 120) * 0.25f
                );
                CalamityMod.Particles.GeneralParticleHandler.SpawnParticle(vine);
            }
        }



        // === 2) 满蓄后 · 持续（林潮外泄：扇缘外散 + 少量下落孢子） ===
        // 设计：与蓄力反向。由“上浮吸附”→“前向外散 + 少量下落”，Dust 主体，保证阶段对比。
        // 形状：前向 ±18° 的轻扇弧 + 少量重力孢子。
        private void SpawnChargeFX_Complete(Vector2 pos)
        {
            Vector2 f = Projectile.velocity.LengthSquared() > 1e-4f
                ? Vector2.Normalize(Projectile.velocity)
                : Vector2.UnitX.RotatedBy(Projectile.rotation);

            // 前向轻扇弧：外泄
            int fan = 4;
            float arc = MathHelper.ToRadians(18f);
            for (int i = 0; i < fan; i++)
            {
                float ang = Main.rand.NextFloat(-arc, arc);
                Vector2 dir = f.RotatedBy(ang);
                Vector2 vel = dir * Main.rand.NextFloat(1.2f, 2.2f);

                int id = Dust.NewDust(pos, 0, 0, DustID.Grass, vel.X, vel.Y, 110);
                Dust d = Main.dust[id];
                d.noGravity = true;
                d.scale = Main.rand.NextFloat(0.85f, 1.05f);
                d.color = Color.Lerp(new Color(100, 235, 140), new Color(150, 255, 200), Main.rand.NextFloat(0.25f, 0.8f));
            }

            // 少量“孢子”带重力下落（与上浮形成对比）
            if (Main.rand.NextBool(1, 2))
            {
                Vector2 v = new Vector2(Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextFloat(0.8f, 1.4f));
                int id = Dust.NewDust(pos, 0, 0, DustID.Grass, v.X, v.Y, 100);
                Dust d = Main.dust[id];
                d.noGravity = false; // ? 受重力
                d.scale = Main.rand.NextFloat(0.9f, 1.15f);
                d.color = new Color(120, 210, 130);
            }
        }



        // === 3) 满蓄瞬间 · 一次性（林海绽放：花环 + 叶爆 + 藤脉短线） ===
        // 设计：Dust 占绝对体量（≥80%），AltSpark 少量“叶脉/藤纹”强化锐度。
        // 形状：不做完美圆——采用“花瓣环”（半径随角度微波动）+ 双翼扇弧爆尘。
        private void SpawnChargeReadyOnceFX(Vector2 pos)
        {
            Vector2 f = Projectile.velocity.LengthSquared() > 1e-4f
                ? Vector2.Normalize(Projectile.velocity)
                : Vector2.UnitX.RotatedBy(Projectile.rotation);
            Vector2 n = new Vector2(-f.Y, f.X);

            // A) 花瓣环（Dust 环，半径带正弦扰动）
            int petals = 20;
            float R = 46f;
            for (int i = 0; i < petals; i++)
            {
                float t = MathHelper.TwoPi * i / petals;
                float r = R + (float)Math.Sin(t * 3f) * 6f; // 三叶调制
                Vector2 p = pos + (f * (float)Math.Cos(t) + n * (float)Math.Sin(t)) * r;

                int id = Dust.NewDust(p, 0, 0, DustID.Grass, 0f, 0f, 140);
                Dust d = Main.dust[id];
                d.noGravity = true;
                d.scale = Main.rand.NextFloat(0.95f, 1.25f);
                d.color = Color.Lerp(new Color(120, 255, 160), new Color(170, 255, 210), Main.rand.NextFloat(0.3f, 0.9f));
            }

            // B) 双翼扇弧爆尘（前向 ±26°，Dust 主体体量）
            int burst = 140;
            float arc = MathHelper.ToRadians(26f);
            for (int i = 0; i < burst; i++)
            {
                float ang = Main.rand.NextFloat(-arc, arc);
                Vector2 dir = f.RotatedBy(ang);
                Vector2 vel = dir * Main.rand.NextFloat(7f, 12f) + n * Main.rand.NextFloat(-0.6f, 0.6f);

                int id = Dust.NewDust(pos, 0, 0, DustID.Grass, vel.X, vel.Y, 120);
                Dust d = Main.dust[id];
                d.noGravity = true;
                d.scale = Main.rand.NextFloat(1.0f, 1.35f);
                d.color = Color.Lerp(new Color(110, 245, 150), new Color(180, 255, 210), Main.rand.NextFloat(0.2f, 0.8f));
            }

            // C) 藤脉短线（AltSparkParticle），极少量点缀成“叶脉”
            int vines = 18;
            for (int i = 0; i < vines; i++)
            {
                float ang = Main.rand.NextFloat(-0.2f, 0.2f);
                Vector2 dir = f.RotatedBy(ang);
                var line = new CalamityMod.Particles.AltSparkParticle(
                    pos + dir * Main.rand.NextFloat(8f, 16f),
                    dir * Main.rand.NextFloat(0.4f, 0.8f),
                    false,
                    Main.rand.Next(10, 16),
                    Main.rand.NextFloat(1.0f, 1.3f),
                    new Color(70, 220, 140) * 0.4f
                );
                CalamityMod.Particles.GeneralParticleHandler.SpawnParticle(line);
            }
        }



        // === 4) 发射瞬间 · 一次性（叶片出鞘：前向小爆 + 藤脉一划） ===
        // 供 FireLeaf 调用。Dust 为主，小型 AltSpark 仅一笔。
        private void SpawnPerShotFX(Vector2 pos, Vector2 dir)
        {
            // 小爆尘（Dust 主体）
            int amt = 8;
            for (int i = 0; i < amt; i++)
            {
                Vector2 v = dir.RotatedBy(Main.rand.NextFloat(-0.22f, 0.22f)) * Main.rand.NextFloat(1.6f, 2.8f);
                int id = Dust.NewDust(pos, 0, 0, DustID.Grass, v.X, v.Y, 110);
                Dust d = Main.dust[id];
                d.noGravity = true;
                d.scale = Main.rand.NextFloat(0.9f, 1.2f);
                d.color = Color.Lerp(new Color(120, 245, 160), new Color(160, 255, 200), Main.rand.NextFloat(0.3f, 0.9f));
            }

            // 一笔藤脉（AltSpark，极少量）
            if (Main.rand.NextBool(1, 2))
            {
                var line = new CalamityMod.Particles.AltSparkParticle(
                    pos + dir * 6f,
                    dir * Main.rand.NextFloat(0.6f, 1.0f),
                    false,
                    10,
                    1.0f,
                    new Color(60, 210, 130) * 0.35f
                );
                CalamityMod.Particles.GeneralParticleHandler.SpawnParticle(line);
            }
        }






    }
}
