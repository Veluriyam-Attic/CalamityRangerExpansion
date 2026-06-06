using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.ID;
using CalamityMod.Particles;
using Terraria.DataStructures;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod;
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.CPreMoodLord.MalevolenceC
{
    public class MalevolenceReHold : BaseGunHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.CPreMoodLord";

        public override string Texture => "CalamityRangerExpansion/Content/BOWChange/CPreMoodLord/MalevolenceC/MalevolenceRe";

        public override int AssociatedItemID => ModContent.ItemType<MalevolenceRe>();

        public override bool? CanDamage() => Main.player[Projectile.owner].GetModPlayer<OpenBowDamagePlayer>().OpenBowDamage;

        public override Vector2 GunTipPosition => Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * (Projectile.width * 0.5f + 5f);
        public override float MaxOffsetLengthFromArm => 15f;


        private ref float ChargeFrames => ref Projectile.ai[0];
        private ref float ShotsRemaining => ref Projectile.ai[1];
        private int readyDustTicker = 0;

        //public override void AI()
        //{
        //    base.AI();
        //}

        public override void HoldoutAI()
        {
            Player player = Owner;
            KeepRefreshingLifetime = true;

            if (player.channel)
            {
                if (ChargeFrames < 90)
                {
                    ChargeFrames++;
                    if (ChargeFrames % 10 == 0)
                        //SpawnChargeCircle(GunTipPosition);

                    BowChangeVFX.SpawnCharge(Projectile, GunTipPosition, BowChangeTheme.Malevolence, ChargeFrames / 90f, 0.75f);
                    SpawnChargeFX_EveryFrame(GunTipPosition);
                }
                else
                {
                    // 第一次进入满蓄时释放一次性大特效
                    if (readyDustTicker == 0 && ShotsRemaining <= 0)
                    {
                        BowChangeVFX.SpawnReadyBurst(Projectile, GunTipPosition, BowChangeTheme.Malevolence, 0.9f);
                        SpawnChargeReadyOnceFX(GunTipPosition);
                    }

                    readyDustTicker++;
                    BowChangeVFX.SpawnCharge(Projectile, GunTipPosition, BowChangeTheme.Malevolence, 1f, 0.6f);
                    SpawnChargeFX_Complete(GunTipPosition);
                }
            }
        }

        public override void KillHoldoutLogic()
        {
            Player player = Owner;

            // 玩家已经松手
            if (!player.channel)
            {
                // 情况 1：蓄力足够，允许结算发射
                if (ShotsRemaining <= 0 && ChargeFrames >= 20)
                    ShotsRemaining = 1;

                if (ShotsRemaining > 0)
                {
                    FireVolley(player);
                    Projectile.Kill();
                    return;
                }

                // 情况 2：快速点按 / 未达发射条件 —— 直接兜底死亡
                Projectile.Kill();
            }
        }


        private void FireVolley(Player player)
        {
            Vector2 baseDir = Projectile.velocity.SafeNormalize(Vector2.UnitX * player.direction);
            float baseSpeed = 33f;
            int shots = 6;


            float clampedCharge = MathHelper.Clamp(ChargeFrames, 20f, 150f);
            float t = Utils.GetLerpValue(20f, 92f, clampedCharge, true); // [0,1]
            float maxAngle = MathHelper.Lerp(MathHelper.ToRadians(180f), MathHelper.ToRadians(1f), t);

            int effectMode = Main.rand.NextBool() ? 0 : 1;

            for (int i = 0; i < shots; i++)
            {
                // 核心改动：均匀分布角度 + 小扰动，更具视觉规律
                float interp = (shots == 1) ? 0f : i / (float)(shots - 1);
                float baseAngle = MathHelper.Lerp(-maxAngle / 2f, maxAngle / 2f, interp);
                float wobble = Main.rand.NextFloatDirection() * maxAngle * 0.1f; // 最多 ±10% 扰动
                float finalAngle = baseAngle + wobble;

                Vector2 perturbedDir = baseDir.RotatedBy(finalAngle);

                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    GunTipPosition,
                    perturbedDir * baseSpeed,
                    ModContent.ProjectileType<MalevolenceRePROJ>(),
                    Projectile.damage,
                    Projectile.knockBack,
                    player.whoAmI,
                    ai0: 0f,
                    ai1: effectMode
                );
            }

            SpawnPerShotFX(GunTipPosition, baseDir * baseSpeed);
            BowChangeVFX.SpawnMuzzle(Projectile, GunTipPosition, baseDir * baseSpeed, BowChangeTheme.Malevolence, 0.9f);
            SoundEngine.PlaySound(SoundID.Item72, player.Center);
        }




        // —— 放在类中（字段区）新增：机械蜂毒配色 ——
        // 军绿色 / 墨绿色 / 深灰 + 少量荧光绿点缀
        private static readonly Color ArmyGreen = new Color(88, 102, 74);   // 军绿基底
        private static readonly Color DarkOlive = new Color(40, 48, 38);  // 墨绿偏灰（机械感）
        private static readonly Color SmokeGray = new Color(60, 62, 60);  // 深灰烟
        private static readonly Color NeonLime = new Color(160, 255, 80);  // 少量荧光绿点缀


        // === 蓄力期·瘟疫气体方框上升（加强版） ===
        // 使用 DustID 163（DemonTorch），高频率+快速上升
        private void SpawnChargeFX_EveryFrame(Vector2 pos)
        {
            // 定义矩形区域（宽20，高6），中心在枪口位置下方一点
            Vector2 boxCenter = pos + new Vector2(0f, 6f);
            float halfWidth = 10f;
            float halfHeight = 3f;

            // 每帧生成更多颗粒
            int count = Main.rand.Next(6, 11);
            for (int i = 0; i < count; i++)
            {
                // 随机取方框中的一点
                float offsetX = Main.rand.NextFloat(-halfWidth, halfWidth);
                float offsetY = Main.rand.NextFloat(-halfHeight, halfHeight);
                Vector2 spawnPos = boxCenter + new Vector2(offsetX, offsetY);

                // 向正上方漂浮，速度更快，带轻微扰动
                Vector2 vel = new Vector2(Main.rand.NextFloat(-0.25f, 0.25f), Main.rand.NextFloat(-1.8f, -1.2f));

                // 163号 Dust，颜色偏绿，随机轻微变动
                Color c = Color.Lerp(ArmyGreen, NeonLime, Main.rand.NextFloat(0.4f, 0.7f));

                Dust d = Dust.NewDustPerfect(spawnPos, 163, vel, 120, c, Main.rand.NextFloat(0.8f, 1.1f));
                d.noGravity = true;
            }
        }

        // === 满蓄期·病毒外溢（双螺旋喷射，雨刷式摆动） ===
        private void SpawnChargeFX_Complete(Vector2 pos)
        {
            // 时间因子，用于摆动
            float time = (float)Main.GameUpdateCount * 0.12f;

            // 双螺旋的两个基准角度，随时间摆动（雨刷式）
            float baseAngle1 = (float)Math.Sin(time) * 0.6f;   // 左右来回摆动
            float baseAngle2 = baseAngle1 + MathHelper.Pi;     // 相反方向

            // 每次喷射的 Dust 数量
            int countPerHelix = 6;

            // 两条螺旋：分别从枪口向外喷
            for (int helix = 0; helix < 2; helix++)
            {
                float baseAngle = (helix == 0) ? baseAngle1 : baseAngle2;

                for (int i = 0; i < countPerHelix; i++)
                {
                    // 沿着螺旋递进（半径和角度变化）
                    float progress = i / (float)countPerHelix;
                    float spiralAngle = baseAngle + progress * MathHelper.TwoPi * 0.5f; // 半圈螺旋
                    float radius = 12f + progress * 18f; // 从近到远

                    // 计算生成点和方向
                    Vector2 dir = spiralAngle.ToRotationVector2();
                    Vector2 spawnPos = pos + dir * radius;

                    // Dust 向外喷射
                    Vector2 vel = dir * Main.rand.NextFloat(1.2f, 2.2f);

                    // Dust 类型：163号，颜色毒绿偏差
                    Dust d = Dust.NewDustPerfect(spawnPos, 163, vel, 140,
                        Color.Lerp(ArmyGreen, NeonLime, Main.rand.NextFloat(0.5f, 0.9f)),
                        Main.rand.NextFloat(0.8f, 1.2f));
                    d.noGravity = true;
                }
            }
        }





        // === 3) 满蓄瞬间·一次性大爆发（瘟疫病毒专属） ===
        // 结构：Dust(40%) + 烟雾(雾化弥散) + AltSparkParticle(细长病毒丝)
        // 形状：不规则尖刺扩散，避免圆润，表现病毒刺突和瘟疫侵蚀
        private void SpawnChargeReadyOnceFX(Vector2 pos)
        {
            // ① 病毒爆裂：大量深绿色 Dust，占比核心 40%
            for (int i = 0; i < 250; i++)
            {
                // 尖锐：不均匀角度 + 随机加权，形成“病毒刺突”
                float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                if (Main.rand.NextBool(3)) // 少部分尖刺角度加倍
                    angle += Main.rand.NextFloat(-0.15f, 0.15f);

                Vector2 dir = angle.ToRotationVector2();
                Vector2 vel = dir * Main.rand.NextFloat(3f, 9f);

                Color c = Color.Lerp(DarkOlive, ArmyGreen, Main.rand.NextFloat(0.3f, 0.7f));
                if (Main.rand.NextBool(1, 6)) // 少量荧光绿突显毒性
                    c = Color.Lerp(c, NeonLime, 0.25f);

                Dust d = Dust.NewDustPerfect(pos, 163, vel, 140, c, Main.rand.NextFloat(0.8f, 1.2f));
                d.noGravity = true;
            }

            // ② 病毒烟雾：外圈弥散，不均匀，增加侵蚀氛围
            for (int i = 0; i < 60; i++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(2f, 2f);
                Vector2 spawnPos = pos + Main.rand.NextVector2Circular(24f, 24f);

                Dust smoke = Dust.NewDustPerfect(spawnPos, DustID.Smoke, vel, 120,
                    Color.Lerp(SmokeGray, DarkOlive, Main.rand.NextFloat(0.4f, 0.9f)),
                    Main.rand.NextFloat(0.9f, 1.3f));
                smoke.noGravity = true;
            }

            // ③ 病毒尖刺丝：细长线性粒子，不成圆环，而是向外呈放射性锐角
            int spikes = 48;
            for (int i = 0; i < spikes; i++)
            {
                float angle = MathHelper.TwoPi * i / spikes + Main.rand.NextFloat(-0.05f, 0.05f);
                Vector2 dir = angle.ToRotationVector2();

                AltSparkParticle spike = new AltSparkParticle(
                    pos + dir * Main.rand.NextFloat(12f, 20f), // 稍微偏外生成
                    dir * 0.1f,                                // 极低速度，几乎静止
                    false,
                    Main.rand.Next(10, 16),                    // 存活时间
                    Main.rand.NextFloat(1.1f, 1.6f),           // 长度大小
                    Color.Lerp(ArmyGreen, NeonLime, Main.rand.NextFloat(0.1f, 0.3f)) * 0.9f
                );
                GeneralParticleHandler.SpawnParticle(spike);
            }

            // ④ 毒孢子尖环：不规则外环，形成病菌团簇的视觉
            int spores = 20;
            float radius = 72f;
            for (int i = 0; i < spores; i++)
            {
                float angle = MathHelper.TwoPi * i / spores + Main.rand.NextFloat(-0.15f, 0.15f);
                Vector2 offset = angle.ToRotationVector2() * (radius + Main.rand.NextFloat(-8f, 8f));

                Dust spore = Dust.NewDustPerfect(pos + offset, DustID.Poisoned, Vector2.Zero, 150,
                    Color.Lerp(DarkOlive, NeonLime, 0.6f), Main.rand.NextFloat(1.0f, 1.4f));
                spore.noGravity = true;
            }
        }







        // === 4) 发射瞬间·机械碎屑 + 毒火花（方向性） ===
        // 方向对齐弹道，深灰机械碎烟为主，夹少量荧光绿火花，体现“未来机械 + 瘟疫”
        private void SpawnPerShotFX(Vector2 pos, Vector2 vel)
        {
            Vector2 dir = vel.SafeNormalize(Vector2.UnitX);

            // 机械碎烟（深灰/墨绿），贴地冲散
            for (int i = 0; i < 12; i++)
            {
                Vector2 v = dir.RotatedBy(Main.rand.NextFloat(-0.35f, 0.35f)) * Main.rand.NextFloat(0.3f, 1.1f);
                Color c = Main.rand.NextBool()
                    ? Color.Lerp(SmokeGray, DarkOlive, Main.rand.NextFloat(0.4f, 0.9f))
                    : Color.Lerp(DarkOlive, ArmyGreen, Main.rand.NextFloat(0.2f, 0.8f));

                Dust d = Dust.NewDustPerfect(pos, DustID.Smoke, v, 120, c, Main.rand.NextFloat(0.7f, 1.0f));
                d.noGravity = true;
            }

            // 荧光绿毒火花（少量、高速）
            for (int i = 0; i < 4; i++)
            {
                Vector2 v = dir.RotatedBy(Main.rand.NextFloat(-0.25f, 0.25f)) * Main.rand.NextFloat(1.2f, 2.2f);
                Dust g = Dust.NewDustPerfect(pos, DustID.GreenTorch, v, 140, NeonLime, Main.rand.NextFloat(0.9f, 1.2f));
                g.noGravity = true;
            }

            // 外圈“蜂群散射”小点（呈八方齿轮感），速度较低，增加机械秩序感
            for (int k = 0; k < 8; k++)
            {
                float angle = MathHelper.TwoPi * k / 8f;
                Vector2 v = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * Main.rand.NextFloat(0.6f, 1.0f);
                Dust ring = Dust.NewDustPerfect(pos, DustID.Poisoned, v, 110,
                    Color.Lerp(ArmyGreen, DarkOlive, Main.rand.NextFloat(0.5f, 0.9f)), Main.rand.NextFloat(0.55f, 0.85f));
                ring.noGravity = true;
            }
        }

   


    }
}
