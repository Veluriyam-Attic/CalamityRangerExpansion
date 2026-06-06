using CalamityRangerExpansion.Content.BOWChange.BPrePlantera.PearlwoodBowC;
using CalamityMod;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityRangerExpansion.Content.BOWChange.BPrePlantera.PearlwoodBowC
{
    /// <summary>
    /// 【原·珍珠木弓 | 群体-直线】
    /// 逻辑：A 全转化；长按左键蓄力 → 松手后仅发射 1 发“巨型圣光箭矢（纯特效）”
    /// 说明：本文件仅为 HoldOut；Proj/Effect 以后你说“有书”再写。
    /// 约束：禁止使用 Projectile.localAI[*]，计数统一走类字段/ai。
    /// </summary>
    public class PearlwoodBowReHold : BaseGunHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.BPrePlantera";
        public override string Texture => "CalamityRangerExpansion/Content/BOWChange/BPrePlantera/PearlwoodBowC/PearlwoodBowRe";
        public override int AssociatedItemID => ModContent.ItemType<PearlwoodBowRe>();
        public override float MaxOffsetLengthFromArm => 20f;
        public override Vector2 GunTipPosition =>
            Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * (Projectile.width * 0.5f + 10f);
        // =========（以上为统一区）==================================================

        // ====================【武器专属常量】====================
        private const int MinChargeFrames = 18;
        private const int FullChargeFrames = 54;
        private const int HardCapChargeFrames = 90;

        private const float BaseArrowSpeed = 22f;
        private const float MaxArrowSpeedBonus = 8f;
        private const float BaseDamageMul = 1.0f;
        private const float FullChargeDamageBonus = 1.85f;

        private const float ShootShake = 5f; // 屏幕震动强度（发射时的全屏反馈，保留）

        // ====================【AI 槽位映射】====================
        private ref float ChargeFrames => ref Projectile.ai[0];
        private ref float ShotsRemaining => ref Projectile.ai[1];

        // ====================【自定义字段】====================
        private int readyDustTicker;     // 节拍（保留，但本版不再用于间歇 FX）
        private bool chargedOnce;        // 是否已触发一次性满蓄魔法阵
        private bool fired;              // 是否已发射
        private int fxTick;              // 特效计时

        // 调色板（浅粉 + 月白）
        private static readonly Color MoonWhite = new Color(245, 245, 255);
        private static readonly Color SoftPink = new Color(255, 210, 230);
        private static readonly Color MistBlue = new Color(200, 225, 255); // 水雾点缀用

        public override void OnSpawn(Terraria.DataStructures.IEntitySource source)
        {
            ShotsRemaining = 1;
            SoundEngine.PlaySound(SoundID.Item7 with { Volume = 0.6f, Pitch = -0.05f }, Projectile.Center);
            // Projectile.scale = 3f; // 如需整体放大，可开启
        }

        public override void AI()
        {
            KeepRefreshingLifetime = true;
            base.AI();
            fxTick++;
        }

        // ==================== 蓄力阶段 ====================
        public override void HoldoutAI()
        {
            Player player = Owner;

            if (player.channel)
            {
                if (ChargeFrames < HardCapChargeFrames)
                    ChargeFrames++;

                float t = Utils.GetLerpValue(0f, FullChargeFrames, ChargeFrames, true); // 0~1

                // ——持续型：蓄力期每帧释放（数学感小环绕）——
                if (ChargeFrames < FullChargeFrames)
                {
                    SpawnChargeFX_EveryFrame(GunTipPosition, t);
                }
                else
                {
                    // 满蓄：首次进入触发一次性魔法阵
                    if (!chargedOnce)
                    {
                        SpawnChargeReadyOnceFX(GunTipPosition);
                        chargedOnce = true;
                        UpdateChargeFXControlled(GunTipPosition);

                    }

                    // ——持续型：满蓄后每帧释放（双轨细丝带向前铺展）——
                    SpawnChargeFX_Complete(GunTipPosition);
                }

            }
        }

        // ==================== 松手逻辑（保留原样，仅移除发射时特效函数的调用） ====================
        public override void KillHoldoutLogic()
        {
            Player player = Owner;

            if (player.CantUseHoldout())
            {
                if (ChargeFrames < FullChargeFrames)
                {
                    Projectile.Kill();
                    return;
                }

                if (!fired)
                {
                    if (ChargeFrames >= MinChargeFrames && ShotsRemaining > 0)
                    {
                        FireNextProjectile(player);
                        ShotsRemaining--;
                        fired = true;
                    }
                }

                if (ShotsRemaining <= 0)
                    Projectile.Kill();
            }
        }

        // ==================== 发射一发（保留原逻辑；移除“发射时特效函数”的调用） ====================
        private void FireNextProjectile(Player player)
        {
            Vector2 shootDir = Projectile.velocity.LengthSquared() > 0.0001f
                ? Projectile.velocity.SafeNormalize(Vector2.UnitX * player.direction)
                : (Main.MouseWorld - player.MountedCenter).SafeNormalize(Vector2.UnitX * player.direction);

            float chargeLerp = Utils.GetLerpValue(0f, FullChargeFrames, ChargeFrames, true);
            float speed = BaseArrowSpeed + MaxArrowSpeedBonus * chargeLerp;
            Vector2 shootVel = shootDir * speed;

            int baseDamage = Projectile.damage > 0 ? Projectile.damage : player.GetWeaponDamage(player.HeldItem);
            float damageMul = BaseDamageMul + FullChargeDamageBonus * chargeLerp;
            int finalDamage = (int)(baseDamage * damageMul);

            int type = ModContent.ProjectileType<PearlwoodBowRePROJ>(); // 纯特效箭，具体效果等你“有书”后实现

            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                GunTipPosition,
                shootVel,
                type,
                finalDamage,
                player.HeldItem.knockBack,
                player.whoAmI,
                ai0: MathHelper.Lerp(0.6f, 1.0f, chargeLerp),
                ai1: 0f
            );

            // 屏幕震动（保留）
            float shakePower = ShootShake;
            float distanceFactor = Utils.GetLerpValue(1000f, 0f, Projectile.Distance(Main.LocalPlayer.Center), true);
            Main.LocalPlayer.Calamity().GeneralScreenShakePower =
                System.Math.Max(Main.LocalPlayer.Calamity().GeneralScreenShakePower, shakePower * distanceFactor);

            // 发射声（满蓄更低沉）
            SoundEngine.PlaySound((chargeLerp >= 0.999f ? SoundID.Item96 : SoundID.Item5) with { Volume = 0.9f }, GunTipPosition);
        }

        private int backArmStretchTimer;

        public override void ManageHoldout()
        {
            // ===== 完整复制 BaseGunHoldoutProjectile.ManageHoldout =====

            Vector2 armPosition = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            Vector2 ownerToMouse = Owner.Calamity().mouseWorld - armPosition;

            float holdoutDirection = Projectile.velocity.ToRotation();
            float proximityLookingUpwards = Vector2.Dot(
                ownerToMouse.SafeNormalize(Vector2.Zero),
                -Vector2.UnitY * Owner.gravDir
            );

            int direction = MathF.Sign(ownerToMouse.X);

            Vector2 lengthOffset = Projectile.rotation.ToRotationVector2() * OffsetLengthFromArm;
            Vector2 armOffset = new Vector2(
                Utils.Remap(
                    MathF.Abs(proximityLookingUpwards),
                    0f, 1f,
                    0f,
                    proximityLookingUpwards > 0f ? OffsetXUpwards : OffsetXDownwards
                ) * direction,
                BaseOffsetY * Owner.gravDir +
                Utils.Remap(
                    MathF.Abs(proximityLookingUpwards),
                    0f, 1f,
                    0f,
                    proximityLookingUpwards > 0f ? OffsetYUpwards : OffsetYDownwards
                ) * Owner.gravDir
            );

            Projectile.Center = armPosition + lengthOffset + armOffset;
            Projectile.velocity = holdoutDirection
                .AngleTowards(ownerToMouse.ToRotation(), 0.2f)
                .ToRotationVector2();
            Projectile.rotation = holdoutDirection;

            Projectile.spriteDirection = direction;
            Owner.ChangeDir(direction);

            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = Owner.itemAnimation = 2;
            Owner.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();

            // ====== 手部动画修改区（改的是【另一只手】）======

            backArmStretchTimer++;

            const int X = 10; // 帧数写死，方便你快速改

            if (backArmStretchTimer < X)
                FrontArmStretch = Player.CompositeArmStretchAmount.Full;
            else if (backArmStretchTimer < X * 2)
                FrontArmStretch = Player.CompositeArmStretchAmount.ThreeQuarters;
            else
                FrontArmStretch = Player.CompositeArmStretchAmount.Quarter;

            // BackArmStretch 完全不动，维持原本行为

            // ====== 原本的手臂设置 ======

            float armRotation =
                (Projectile.rotation - MathHelper.PiOver2) * Owner.gravDir +
                (Owner.gravDir == -1 ? MathHelper.Pi : 0f);

            Owner.SetCompositeArmFront(
                true,
                FrontArmStretch,
                armRotation + ExtraFrontArmRotation * direction
            );

            Owner.SetCompositeArmBack(
                true,
                BackArmStretch,
                armRotation + ExtraBackArmRotation * direction
            );

            if (KeepRefreshingLifetime)
                Projectile.timeLeft = 2;

            if (OffsetLengthFromArm != MaxOffsetLengthFromArm)
                OffsetLengthFromArm = MathHelper.Lerp(
                    OffsetLengthFromArm,
                    MaxOffsetLengthFromArm,
                    RecoilResolveSpeed
                );

            Projectile.netUpdate = true;
            Projectile.netSpam = 0;
        }







        // SpawnChargeFX_EveryFrame 专用：被接管的蓄力粒子
        private List<(Particle particle, Vector2 localOffset, Vector2 velocity)>
            chargeFXControlled = new();

        private void UpdateChargeFXControlled(Vector2 anchorPos)
        {
            for (int i = chargeFXControlled.Count - 1; i >= 0; i--)
            {
                var t = chargeFXControlled[i];
                Particle p = t.particle;

                if (p.Time >= p.Lifetime)
                {
                    chargeFXControlled.RemoveAt(i);
                    continue;
                }

                // ——前进（你说的“强行介入”就在这）——
                t.localOffset += t.velocity;

                // 轻微衰减，避免无限飞
                t.velocity *= 0.96f;

                // ——关键：以“当前枪口”为坐标系中心——
                p.Position = anchorPos + t.localOffset;

                chargeFXControlled[i] = t;
            }
        }

        // ========================================================================
        // ========================   仅保留三大特效函数   =========================
        // ========================================================================

        // 1）【蓄力期间·持续释放】浅粉亮白·半圆收缩（神圣感）
        //    思路：以正前方为主轴，生成半圆上的点，每帧半径收缩+随机扰动
        //          粒子呈现“从前方半弧汇聚到枪口”的效果，突出圣洁感
        private void SpawnChargeFX_EveryFrame(Vector2 pos, float t)
        {
            Vector2 f = Projectile.velocity.LengthSquared() > 1e-4f
                ? Vector2.Normalize(Projectile.velocity)
                : Vector2.UnitX.RotatedBy(Projectile.rotation);
            Vector2 n = new Vector2(-f.Y, f.X);

            float time = Main.GlobalTimeWrappedHourly;

            int points = 8;
            float baseRadius = 24f;
            float shrink = 1f - 0.5f * (float)Math.Sin(time * 2.2f);

            for (int i = 0; i < points; i++)
            {
                float angle = MathHelper.Lerp(
                    -MathHelper.PiOver2,
                     MathHelper.PiOver2,
                    i / (float)(points - 1)
                );

                Vector2 dir = f.RotatedBy(angle);
                float radius = baseRadius * shrink + Main.rand.NextFloat(-2f, 2f);
                Vector2 worldPos = pos + dir * radius;

                // ========= EXO 光粒 =========
                var exo = new SquishyLightParticle(
                    worldPos,
                    Vector2.Zero, // ?? 速度清零，完全由我们接管
                    MathHelper.Lerp(0.2f, 0.3f, t),
                    Color.Lerp(SoftPink, MoonWhite, 0.5f + 0.5f * t),
                    2,
                    opacity: 1f,
                    squishStrenght: 1.1f,
                    maxSquish: 2.8f
                );
                GeneralParticleHandler.SpawnParticle(exo);

                //// ——立刻收编进“介质”——
                //Vector2 localOffset = worldPos - pos;
                //Vector2 vel = -dir * Main.rand.NextFloat(0.6f, 1.0f);

                //chargeFXControlled.Add((exo, localOffset, vel));

                // ========= GlowOrb =========
                if (Main.rand.NextBool(3))
                {
                    var orb = new GlowOrbParticle(
                        worldPos,
                        Vector2.Zero,
                        false,
                        5,
                        0.85f,
                        Color.Lerp(MoonWhite, SoftPink, 0.2f),
                        true, false, true
                    );
                    GeneralParticleHandler.SpawnParticle(orb);

                    //chargeFXControlled.Add((
                    //    orb,
                    //    worldPos - pos,
                    //    -dir * Main.rand.NextFloat(0.4f, 0.8f)
                    //));
                }

                // ========= Water Mist =========
                if (Main.rand.NextBool(2))
                {
                    // 从枪口 pos → 当前生成点 worldPos 的方向
                    Vector2 radialDir = (worldPos - pos).SafeNormalize(Vector2.UnitX);

                    var mist = new WaterFlavoredParticle(
                        worldPos,
                        radialDir * Main.rand.NextFloat(0.6f, 1.4f), // ? 辐射向外的初速度
                        false,
                        Main.rand.Next(1, 2),
                        0.8f + Main.rand.NextFloat(0.3f),
                        Color.Lerp(MoonWhite, SoftPink, 0.3f)
                    );
                    GeneralParticleHandler.SpawnParticle(mist);

                    //chargeFXControlled.Add((
                    //    mist,
                    //    worldPos - pos,
                    //    -dir * 0.5f
                    //));
                }
            }

            // 脉冲环（不接管，瞬时型，留给系统）
            if (fxTick % 20 == 0)
            {
                Particle pulse = new DirectionalPulseRing(
                    pos,
                    f * 0.6f,
                    Color.Lerp(MoonWhite, SoftPink, 0.6f),
                    new Vector2(0.8f, 2.4f),
                    Projectile.rotation,
                    0.15f,
                    0.03f,
                    22
                );
                GeneralParticleHandler.SpawnParticle(pulse);
            }
        }






        // 2）【满蓄后·持续释放】浅粉月白·双轨细丝带（强）
        //    思路：以 f 为主轴，在 ±n 的两条“细轨道”上铺设高速光粒，带相位起伏（正弦）
        private void SpawnChargeFX_Complete(Vector2 pos)
        {
            Vector2 f = Projectile.velocity.LengthSquared() > 1e-4f
                ? Vector2.Normalize(Projectile.velocity)
                : Vector2.UnitX.RotatedBy(Projectile.rotation);
            Vector2 n = new Vector2(-f.Y, f.X);

            float time = (float)Main.GameUpdateCount * 0.08f;
            float railOffset = 6f;         // 两条轨道的法线偏移（越小越细）
            float seg = 12f;               // 轨道段距
            int count = 8;               // 每帧沿前方铺设的段数

            for (int i = 0; i < count; i++)
            {
                float t = i / (float)count;
                float wave = (float)System.Math.Sin(time + i * 0.7f) * 0.9f; // 细小相位波动

                // 两条轨：+n 与 -n
                Vector2 baseP = pos + f * (i * seg + wave * 2.5f);

                for (int side = -1; side <= 1; side += 2)
                {
                    Vector2 p = baseP + n * (railOffset * side);

                    //// EXO：沿 f 方向的细长亮点（核心）
                    //var exo = new SquishyLightParticle(
                    //    p,
                    //    f * Main.rand.NextFloat(0.4f, 1.2f) + n * Main.rand.NextFloat(-0.15f, 0.15f),
                    //    0.28f,
                    //    Color.Lerp(MoonWhite, SoftPink, 0.35f),
                    //    22,
                    //    opacity: 1f,
                    //    squishStrenght: 1.25f,
                    //    maxSquish: 3.4f, // 更细更长
                    //    hueShift: 0f
                    //);
                    //GeneralParticleHandler.SpawnParticle(exo);

                    //// GlowOrb：低频点亮
                    //if (Main.rand.NextBool(4))
                    //{
                    //    var orb = new GlowOrbParticle(
                    //        p,
                    //        Vector2.Zero,
                    //        false,
                    //        7,
                    //        0.8f,
                    //        Color.Lerp(MoonWhite, SoftPink, 0.15f),
                    //        true, false, true
                    //    );
                    //    GeneralParticleHandler.SpawnParticle(orb);
                    //}

                    //// 水雾：贴轨道向后散（极少）
                    //if (Main.rand.NextBool(10))
                    //{
                    //    var mist = new WaterFlavoredParticle(
                    //        p,
                    //        -f * Main.rand.NextFloat(0.6f, 1.2f),
                    //        false,
                    //        Main.rand.Next(16, 22),
                    //        0.9f + Main.rand.NextFloat(0.3f),
                    //        Color.Lerp(MistBlue, MoonWhite, 0.6f)
                    //    );
                    //    GeneralParticleHandler.SpawnParticle(mist);
                    //}
                }
            }

            // 低频的椭圆冲击波（提升存在感，但不瞎眼）
            if (fxTick % 3 == 0)
            {
                Particle pulse = new DirectionalPulseRing(
                    pos + f * 6f,
                    f * 0.7f,
                    Color.Lerp(SoftPink, MoonWhite, 0.85f),
                    new Vector2(0.7f, 2.6f),
                    Projectile.rotation,
                    0.18f,
                    0.03f,
                    22
                );
                GeneralParticleHandler.SpawnParticle(pulse);
            }
        }


        // 3）【满蓄瞬间·一次性】浅粉月白·新月魔法阵（极度扩散，大面积）
        //    思路：以“正前方”为半圆中心角，环阵+半月权重；Dust/Orb/冲击波/少量强 Bloom
        private void SpawnChargeReadyOnceFX(Vector2 pos)
        {
            Vector2 f = Projectile.velocity.LengthSquared() > 1e-4f
                ? Vector2.Normalize(Projectile.velocity)
                : Vector2.UnitX.RotatedBy(Projectile.rotation);

            float forwardAngle = f.ToRotation();

            // ——Dust：大扩散（PinkTorch + WhiteTorch 混合）——
            for (int i = 0; i < 320; i++) // >= 200，增强“宏伟”
            {
                // 以正前方为中心的半圆角度（±90°），并叠加轻权重，让“前侧”更密集
                float a = forwardAngle + Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);
                float rad = Main.rand.NextFloat(16f, 64f);
                Vector2 vel = a.ToRotationVector2() * rad;

                int type = (Main.rand.NextBool(3) ? DustID.PinkTorch : DustID.WhiteTorch);
                Dust d = Dust.NewDustPerfect(pos, type, vel, 120,
                    Color.Lerp(SoftPink, MoonWhite, Main.rand.NextFloat(0.2f, 0.8f)),
                    Main.rand.NextFloat(1.0f, 1.6f));
                d.noGravity = true;
            }

            // ——GlowOrb 魔法阵：双环（外稀内密），半月权重（前方更亮）——
            int inner = 32, outer = 40;
            float rInner = 56f, rOuter = 92f;

            for (int ring = 0; ring < 2; ring++)
            {
                int count = ring == 0 ? inner : outer;
                float r = ring == 0 ? rInner : rOuter;

                for (int i = 0; i < count; i++)
                {
                    // 半圆分布在 forwardAngle ±90°
                    float ang = forwardAngle - MathHelper.PiOver2 + MathHelper.Pi * i / (count - 1);
                    // 新月：用余弦权重压低后半边
                    float weight = 0.5f + 0.5f * (float)System.Math.Cos(ang - forwardAngle);
                    Vector2 offset = ang.ToRotationVector2() * r;

                    var orb = new GlowOrbParticle(
                        pos + offset,
                        Vector2.Zero,
                        false,
                        10,
                        MathHelper.Lerp(0.7f, 1.0f, weight),
                        Color.Lerp(MoonWhite, SoftPink, 0.15f + 0.5f * weight),
                        true, false, true
                    );
                    GeneralParticleHandler.SpawnParticle(orb);
                }
            }

            // ——椭圆冲击波：连发两道（极短命），强调“瞬时爆闪”——
            for (int k = 0; k < 2; k++)
            {
                Particle pulse = new DirectionalPulseRing(
                    pos + f * (k * 6f),
                    f * (0.8f + 0.2f * k),
                    Color.Lerp(SoftPink, MoonWhite, 0.85f),
                    new Vector2(0.85f, 2.8f),
                    Projectile.rotation - MathHelper.PiOver4,
                    0.22f,
                    0.04f,
                    18
                );
                GeneralParticleHandler.SpawnParticle(pulse);
            }

            //// ——StrongBloom：极少量（超亮，谨慎）——
            //for (int i = 0; i < 2; i++)
            //{
            //    var strong = new StrongBloom(
            //        pos + f * Main.rand.NextFloat(4f, 12f),
            //        Vector2.Zero,
            //        Color.Lerp(MoonWhite, SoftPink, 0.25f),
            //        2.0f,
            //        42
            //    );
            //    GeneralParticleHandler.SpawnParticle(strong);
            //}
        }
    }
}
