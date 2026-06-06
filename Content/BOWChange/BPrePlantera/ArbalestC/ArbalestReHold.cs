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
using CalamityMod;

namespace CalamityRangerExpansion.Content.BOWChange.BPrePlantera.ArbalestC
{
    public class ArbalestReHold : BaseGunHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.BPrePlantera";

        public override string Texture => "CalamityRangerExpansion/Content/BOWChange/BPrePlantera/ArbalestC/ArbalestRe";

        public override int AssociatedItemID => ModContent.ItemType<ArbalestRe>();

        public override float MaxOffsetLengthFromArm => 15f; // 后坐力偏移
        public override bool? CanDamage() => Main.player[Projectile.owner].GetModPlayer<OpenBowDamagePlayer>().OpenBowDamage;

        public override Vector2 GunTipPosition =>
            Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * (Projectile.width * 0.5f + 10f); // width+x像素 = 往前握持x像素



        // =================== 可调参数 ===================
        private const int MaxChargeTime = 120;      // 蓄力最大帧数
        private const int ChargeSoundInterval = 10; // 蓄力每隔多少帧播放一次音效/特效
        private const int ReadyDustEveryFrames = 3; // 满蓄力后长按时，枪口上飘dust的频率（帧）
        private const int MaxRedShots = 12;         // 红色模式最大弹幕数
        private const int MaxGreenShots = 8;        // 绿色模式最大弹幕数
        // ================================================

        private ref float ChargeFrames => ref Projectile.ai[0];   // 当前蓄力帧数
        private ref float ShotsRemaining => ref Projectile.ai[1]; // 剩余要打出的弹数

        private bool redMode = false;   // true = 红色（B弹），false = 绿色（A弹）
        private bool modeChosen = false;
        private int silverShotsFired = 0;

        // 满蓄力时用于节流上飘dust的本地计时器
        private int readyDustTicker = 0;

        public override void HoldoutAI()
        {
            Player player = Owner;

            if (!modeChosen)
            {
                redMode = Main.rand.NextBool(); // 随机选择模式
                modeChosen = true;
                ShotsRemaining = 0;
                silverShotsFired = 0;
                readyDustTicker = 0;
            }

            // —— 蓄力阶段 —— //
            if (player.channel)
            {
                if (ChargeFrames < MaxChargeTime)
                {
                    ChargeFrames++;

                    // 每隔 10 帧：播放音效 + 魔法阵聚拢特效（枪口）
                    if (ChargeFrames % ChargeSoundInterval == 0)
                    {
                        SoundEngine.PlaySound(
                            new SoundStyle("CalamityRangerExpansion/Sound/SSL/空中分裂")
                            {
                                Volume = 1.2f,
                                Pitch = 0f
                            },
                            Projectile.Center
                        );

                        SpawnChargeCircle(GunTipPosition);
                    }
                }
                else
                {
                    // —— 满蓄力且仍然长按：枪口持续向“正上方”飘散 dust（进入攻击前的待机反馈）——
                    readyDustTicker++;
                    if (readyDustTicker % ReadyDustEveryFrames == 0 && ShotsRemaining <= 0)
                        SpawnReadyIdleDust(GunTipPosition);
                }
            }
        }

        public override void KillHoldoutLogic()
        {
            Player player = Owner;

            // 松手 → 进入逐帧连发阶段
            if (Owner.CantUseHoldout())
            {
                // 首次松手时，根据蓄力决定本次总发射数
                if (ShotsRemaining <= 0 && ChargeFrames > 0)
                {
                    float chargeRatio = MathHelper.Clamp(ChargeFrames / MaxChargeTime, 0f, 1f);
                    int maxShots = redMode ? MaxRedShots : MaxGreenShots;
                    ShotsRemaining = (int)MathF.Max(1, maxShots * chargeRatio);
                }

                if (ShotsRemaining > 0)
                {
                    FireNextProjectile(player);
                    ShotsRemaining--;

                    if (ShotsRemaining <= 0)
                        Projectile.Kill();
                }
            }
        }

        private void FireNextProjectile(Player player)
        {
            // 从枪口射出（注意不是玩家中心）
            Vector2 shootVel = Projectile.velocity.SafeNormalize(Vector2.UnitX * player.direction) * 14f;
            int arbalestType = redMode ? 1 : 0; // 红=1(B弹)；绿=0(A弹)

            // 用玩家自身的箭（ammo）发射，但用 ArbalestReEffect 标记附魔类型
            if (player.PickAmmo(player.HeldItem, out int ammoProj, out float shootSpeed, out int damage, out float knockback, out int ammoType))
            {
                int proj = Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    GunTipPosition,
                    shootVel,
                    ammoProj,
                    damage,
                    knockback,
                    player.whoAmI,
                    ai0: 0f,
                    ai1: arbalestType
                );

                if (proj >= 0 && proj < Main.maxProjectiles)
                    Main.projectile[proj].GetGlobalProjectile<ArbalestReEffect>().ArbalestType = arbalestType;

                // 发射点击音
                SoundEngine.PlaySound(
                    new SoundStyle("CalamityRangerExpansion/Sound/Others/crossbow-firing-95020")
                    {
                        Volume = 1.2f,
                        Pitch = 0f
                    },
                    GunTipPosition
                );
            }

            // 在发射过程插入两发银色穿刺矛（按剩余数分段触发）
            if ((redMode && (ShotsRemaining == MaxRedShots * 2 / 3 || ShotsRemaining == MaxRedShots / 3)) ||
                (!redMode && (ShotsRemaining == MaxGreenShots * 2 / 3 || ShotsRemaining == MaxGreenShots / 3)))
            {
                if (silverShotsFired < 2)
                {
                    Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        GunTipPosition,
                        shootVel,
                        ModContent.ProjectileType<ArbalestSilverPROJ>(),
                        Projectile.damage,
                        Projectile.knockBack,
                        player.whoAmI
                    );
                    SoundEngine.PlaySound(SoundID.Item12, GunTipPosition);
                    silverShotsFired++;
                }
            }
        }

        // —— 满蓄力待机：枪口向正上方飘散 —— //
        private void SpawnReadyIdleDust(Vector2 pos)
        {
            if (Main.dedServ) return;

            // 基础向上（Terraria坐标：Y 轴向下，所以“上方”为负 Y）
            Vector2 up = new Vector2(0f, -1f);

            // 1) 少量 Dust 作为主体上飘
            for (int i = 0; i < 2; i++)
            {
                Dust d = Dust.NewDustPerfect(
                    pos + Main.rand.NextVector2Circular(2f, 2f),
                    DustID.MagicMirror,
                    up.RotatedBy(Main.rand.NextFloat(-0.15f, 0.15f)) * Main.rand.NextFloat(0.9f, 1.5f) + Owner.velocity * 0.05f,
                    140,
                    redMode ? new Color(255, 180, 140) : new Color(150, 255, 210),
                    Main.rand.NextFloat(0.9f, 1.2f)
                );
                d.noGravity = true;
            }

            // 2) 极少量线性能量丝（SparkParticle）向上漂移，强调“能量蒸腾”
            if (Main.rand.NextBool(3))
            {
                var trail = new CalamityMod.Particles.SparkParticle(
                    pos + Main.rand.NextVector2Circular(1.5f, 1.5f),
                    up.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)) * Main.rand.NextFloat(0.8f, 1.3f),
                    false,
                    20,
                    0.9f,
                    redMode ? new Color(255, 210, 160) : new Color(170, 255, 230)
                );
                CalamityMod.Particles.GeneralParticleHandler.SpawnParticle(trail);
            }

            // 3) 偶尔一个小十字星（CritSpark）微量上浮，作为高光
            if (Main.rand.NextBool(6))
            {
                var star = new CalamityMod.Particles.CritSpark(
                    pos + Main.rand.NextVector2Circular(1f, 1f),
                    up * Main.rand.NextFloat(0.7f, 1.1f),
                    Color.White,
                    redMode ? new Color(255, 170, 120) : new Color(140, 255, 200),
                    0.9f,
                    14
                );
                CalamityMod.Particles.GeneralParticleHandler.SpawnParticle(star);
            }
        }

        // —— 蓄力中的“聚拢魔法阵” —— //
        private void SpawnChargeCircle(Vector2 pos)
        {
            if (Main.dedServ) return;

            // 数学控制参数
            int points = 22;
            float t = MathHelper.Clamp(ChargeFrames / MaxChargeTime, 0f, 1f);
            float baseRadius = MathHelper.Lerp(18f, 36f, t);
            float inwardSpeed = MathHelper.Lerp(1.0f, 2.6f, t);
            float swirlSpeed = MathHelper.Lerp(0.6f, 1.8f, t);
            float sparkScale = MathHelper.Lerp(0.7f, 1.0f, t);
            int sparkLife = (int)MathHelper.Lerp(10f, 26f, t);
            int starEvery = 4;

            Color c1 = redMode ? new Color(255, 120, 80) : new Color(100, 255, 170);
            Color c2 = redMode ? new Color(255, 200, 140) : new Color(170, 255, 220);

            float timePhase = Main.GlobalTimeWrappedHourly * 4f;
            Vector2 forward = Projectile.velocity.SafeNormalize(Vector2.UnitX * Owner.direction);

            for (int i = 0; i < points; i++)
            {
                float ang = MathHelper.TwoPi * i / points;
                float rJitter = baseRadius + (float)Math.Sin(ang * 2f + timePhase) * MathHelper.Lerp(1.5f, 3.0f, t);

                Vector2 outDir = ang.ToRotationVector2();
                Vector2 tangent = new Vector2(-outDir.Y, outDir.X);
                Vector2 onCircle = pos + outDir * rJitter;

                Vector2 vIn = -outDir * inwardSpeed;
                Vector2 vTwirl = tangent * (swirlSpeed * (0.8f + 0.4f * (float)Math.Sin(ang + timePhase)));
                Vector2 vFwd = forward * MathHelper.Lerp(0.2f, 0.8f, t);
                Vector2 v0 = vIn + vTwirl + vFwd;

                var trail = new CalamityMod.Particles.SparkParticle(
                    onCircle,
                    v0,
                    false,
                    sparkLife,
                    sparkScale,
                    Color.Lerp(c1, c2, 0.25f)
                );
                CalamityMod.Particles.GeneralParticleHandler.SpawnParticle(trail);

                if (i % starEvery == 0)
                {
                    Vector2 starVel = v0 + outDir.RotatedBy(Main.rand.NextFloat(-0.35f, 0.35f)) * MathHelper.Lerp(4f, 6.5f, t);
                    var star = new CalamityMod.Particles.CritSpark(
                        onCircle,
                        starVel + Owner.velocity * 0.2f,
                        Color.White,
                        redMode ? new Color(255, 140, 90) : new Color(140, 255, 210),
                        MathHelper.Lerp(0.9f, 1.1f, t),
                        (int)MathHelper.Lerp(12f, 20f, t)
                    );
                    CalamityMod.Particles.GeneralParticleHandler.SpawnParticle(star);
                }
            }

            // 中心轻微外喷，增强“向内聚拢”的对比
            for (int k = 0; k < 3; k++)
            {
                var core = new CalamityMod.Particles.SparkParticle(
                    pos,
                    forward.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f)) * MathHelper.Lerp(1.2f, 2.2f, t),
                    false,
                    (int)MathHelper.Lerp(28f, 40f, t),
                    MathHelper.Lerp(0.8f, 1.0f, t),
                    c2
                );
                CalamityMod.Particles.GeneralParticleHandler.SpawnParticle(core);
            }
        }








    }
}
