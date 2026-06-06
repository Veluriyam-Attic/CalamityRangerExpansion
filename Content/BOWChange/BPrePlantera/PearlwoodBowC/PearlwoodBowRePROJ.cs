using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod;
using CalamityMod.Particles;

namespace CalamityRangerExpansion.Content.BOWChange.BPrePlantera.PearlwoodBowC
{
    internal class PearlwoodBowRePROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.BPrePlantera";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        // ▼▼▼ 拖尾着色调成浅粉红系 ▼▼▼
        private static readonly Color SoftPinkA = new Color(255, 180, 215);   // 主色
        private static readonly Color SoftPinkB = new Color(255, 225, 245);   // 次色
        private static readonly Color SoftPinkEnd = new Color(255, 248, 252); // 尾部

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        // ▼ 将“粗细”调细：减少宽度，使拖尾更纤细
        private float PrimitiveWidthFunction(float completionRatio, Vector2 vertexPosition)
        {
            float arrowheadCutoff = 0.36f;
            float width = 14f;                 // 原 24f → 14f，更“细”
            float minHeadWidth = 0.02f;
            float maxHeadWidth = width;
            if (completionRatio <= arrowheadCutoff)
                width = MathHelper.Lerp(minHeadWidth, maxHeadWidth, Utils.GetLerpValue(0f, arrowheadCutoff, completionRatio, true));
            return width;
        }

        private Color PrimitiveColorFunction(float completionRatio, Vector2 vertexPosition)
        {
            float endFadeRatio = 0.41f;
            float completionRatioFactor = 2.7f;
            float globalTimeFactor = 5.3f;
            float endFadeFactor = 3.2f;
            float endFadeTerm = Utils.GetLerpValue(0f, endFadeRatio * 0.5f, completionRatio, true) * endFadeFactor;
            float cosArgument = completionRatio * completionRatioFactor - Main.GlobalTimeWrappedHourly * globalTimeFactor + endFadeTerm;
            float startingInterpolant = (float)Math.Cos(cosArgument) * 0.5f + 0.5f;

            float colorLerpFactor = 0.6f;
            Color startingColor = Color.Lerp(SoftPinkA, SoftPinkB, startingInterpolant * colorLerpFactor);
            return Color.Lerp(startingColor, SoftPinkEnd, MathHelper.SmoothStep(0f, 1f, Utils.GetLerpValue(0f, endFadeRatio, completionRatio, true)));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            GameShaders.Misc["CalamityMod:TrailStreak"]
                .SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/SylvestaffStreak"));
            Vector2 overallOffset = Projectile.Size * 0.5f;
            overallOffset += Projectile.velocity * 1.4f;
            int numPoints = 46;
            PrimitiveRenderer.RenderTrail(
                Projectile.oldPos,
                new(PrimitiveWidthFunction, PrimitiveColorFunction, (_, _) => overallOffset, shader: GameShaders.Misc["CalamityMod:TrailStreak"]),
                numPoints
            );
            return false; // 不执行默认绘制
        }

        public override void SetDefaults()
        {
            Projectile.width = 11;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 22;
            Projectile.timeLeft = 130;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;
            Projectile.ignoreWater = true;
            Projectile.arrow = true;
            Projectile.extraUpdates = 15;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.velocity *= 0.3f;
        }






        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, SoftPinkEnd.ToVector3() * 0.4f);

            Vector2 forward = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            Vector2 normal = new Vector2(-forward.Y, forward.X);

            float time = Main.GameUpdateCount + Projectile.whoAmI * 3f;

            // ?? 正弦扰动位移（略微波动）
            float oscillation = (float)Math.Sin(time * 0.25f) * 6f;
            Projectile.Center += normal * oscillation * 0.08f;

            // ?? 复杂双螺旋粒子（每帧4个）
            for (int i = 0; i < 2; i++)
            {
                float angle = time * 0.3f + i * MathHelper.Pi;
                float offsetAmount = (float)Math.Sin(angle) * 12f;
                Vector2 spiralOffset = normal * offsetAmount;
                Vector2 spiralPos = Projectile.Center + spiralOffset;

                GlowOrbParticle orb = new GlowOrbParticle(
                    spiralPos,
                    Vector2.Zero,
                    false,
                    7,
                    1.5f,
                    i == 0 ? SoftPinkA : SoftPinkEnd,
                    true,
                    false,
                    true
                );
                GeneralParticleHandler.SpawnParticle(orb);
            }

            // ?? 中线极粗拖尾（每帧多条）
            for (int j = 0; j < 3; j++)
            {
                Vector2 offset = Main.rand.NextVector2CircularEdge(5f, 5f);
                GlowOrbParticle core = new GlowOrbParticle(
                    Projectile.Center + offset,
                    Vector2.Zero,
                    false,
                    6,
                    1.2f,
                    Color.Lerp(SoftPinkA, SoftPinkEnd, Main.rand.NextFloat()),
                    true,
                    false,
                    true
                );
                GeneralParticleHandler.SpawnParticle(core);
            }

            // ?? 水雾拖尾（非随机，每 5 帧一条）
            if (Main.GameUpdateCount % 5 == Projectile.whoAmI % 5)
            {
                WaterFlavoredParticle mist = new WaterFlavoredParticle(
                    Projectile.Center,
                    -Projectile.velocity * 0.3f,
                    false,
                    Main.rand.Next(18, 26),
                    0.9f + Main.rand.NextFloat(0.3f),
                    Color.LightBlue * 0.9f
                );
                GeneralParticleHandler.SpawnParticle(mist);
            }

            // ?? 周期性脉冲环（每 15 帧）
            if (Main.GameUpdateCount % 15 == Projectile.whoAmI % 15)
            {
                DirectionalPulseRing ring = new DirectionalPulseRing(
                    Projectile.Center,
                    forward * 0.3f,
                    Color.Lerp(SoftPinkA, SoftPinkEnd, 0.6f),
                    new Vector2(0.8f, 2.2f),
                    Projectile.rotation,
                    0.16f,
                    0.02f,
                    22
                );
                GeneralParticleHandler.SpawnParticle(ring);
            }

            //// ?? 每 12 帧亮光核心粒子
            //if (Main.GameUpdateCount % 12 == Projectile.whoAmI % 12)
            //{
            //    StrongBloom bloom = new StrongBloom(
            //        Projectile.Center + Main.rand.NextVector2Circular(2f, 2f),
            //        Vector2.Zero,
            //        SoftPinkEnd,
            //        1.7f,
            //        12
            //    );
            //    GeneralParticleHandler.SpawnParticle(bloom);
            //}

            // 旋转更新
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }







        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // ===================== 命中魔法阵（超豪华堆叠） =====================
            Vector2 pos = Projectile.Center;
            Vector2 f = Projectile.velocity.LengthSquared() > 1e-4f ? Vector2.Normalize(Projectile.velocity) : Vector2.UnitX.RotatedBy(Projectile.rotation);
            float forwardAngle = f.ToRotation();

            //// A. 大量 Dust：浅粉+白，强扩散（半圆前向权重）
            //for (int i = 0; i < 480; i++)
            //{
            //    float a = forwardAngle + Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);
            //    float r = Main.rand.NextFloat(20f, 80f);
            //    Vector2 v = a.ToRotationVector2() * r;
            //    int type = Main.rand.NextBool(3) ? DustID.PinkTorch : DustID.WhiteTorch;
            //    Dust d = Dust.NewDustPerfect(pos, type, v, 110, Color.Lerp(SoftPinkA, SoftPinkEnd, Main.rand.NextFloat()), Main.rand.NextFloat(1.0f, 1.6f));
            //    d.noGravity = true;
            //}

            //// B. 双环 GlowOrb 魔法阵（外疏内密）
            //int inner = 48, outer = 64;
            //float rInner = 64f, rOuter = 110f;
            //for (int ring = 0; ring < 2; ring++)
            //{
            //    int count = ring == 0 ? inner : outer;
            //    float R = ring == 0 ? rInner : rOuter;
            //    for (int i = 0; i < count; i++)
            //    {
            //        float ang = forwardAngle - MathHelper.PiOver2 + MathHelper.Pi * i / (count - 1); // 半圆分布
            //        float w = 0.5f + 0.5f * (float)Math.Cos(ang - forwardAngle); // 前向权重
            //        Vector2 offset = ang.ToRotationVector2() * R;
            //        var orb = new GlowOrbParticle(
            //            pos + offset,
            //            Vector2.Zero,
            //            false,
            //            12,
            //            MathHelper.Lerp(0.75f, 1.05f, w),
            //            Color.Lerp(SoftPinkB, SoftPinkEnd, 0.2f + 0.6f * w),
            //            true, false, true
            //        );
            //        GeneralParticleHandler.SpawnParticle(orb);
            //    }
            //}

            //// C. 多道 DirectionalPulseRing（按要求用 Projectile.rotation）
            //for (int k = 0; k < 3; k++)
            //{
            //    Particle pulse = new DirectionalPulseRing(
            //        pos + f * (k * 8f),
            //        f * (0.85f + 0.2f * k),
            //        Color.Lerp(SoftPinkA, SoftPinkEnd, 0.85f),
            //        new Vector2(1.0f, 2.9f),
            //        Projectile.rotation,   // ? 使用 rotation
            //        0.24f,
            //        0.04f,
            //        22
            //    );
            //    GeneralParticleHandler.SpawnParticle(pulse);
            //}

            //// D. EXO 铺场（前方半圆扇区）
            //for (int i = 0; i < 80; i++)
            //{
            //    float ang = forwardAngle + Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);
            //    Vector2 p = pos + ang.ToRotationVector2() * Main.rand.NextFloat(18f, 80f);
            //    var exo = new SquishyLightParticle(
            //        p,
            //        Vector2.Zero,
            //        0.30f,
            //        Color.Lerp(SoftPinkEnd, SoftPinkA, Main.rand.NextFloat(0.3f, 0.7f)),
            //        26,
            //        opacity: 1f,
            //        squishStrenght: 1.15f,
            //        maxSquish: 3.2f,
            //        hueShift: 0f
            //    );
            //    GeneralParticleHandler.SpawnParticle(exo);
            //}

            // E. 少量 StrongBloom（超亮，谨慎）
            for (int i = 0; i < 3; i++)
            {
                var strong = new StrongBloom(
                    pos + f * Main.rand.NextFloat(6f, 14f),
                    Vector2.Zero,
                    Color.Lerp(SoftPinkA, SoftPinkEnd, 0.3f),
                    2.0f,
                    48
                );
                GeneralParticleHandler.SpawnParticle(strong);
            }

            // F. 屏幕震动（按你给的参数）
            float shakePower = 15f;
            float distanceFactor = Utils.GetLerpValue(1000f, 0f, Projectile.Distance(Main.LocalPlayer.Center), true);
            Main.LocalPlayer.Calamity().GeneralScreenShakePower =
                Math.Max(Main.LocalPlayer.Calamity().GeneralScreenShakePower, shakePower * distanceFactor);

            // ? 不改变弹幕飞行方向（删除原有追踪/转向逻辑）
        }

        public override void OnKill(int timeLeft)
        {
            // 保留一个较温和的收束环以呼应主题（非命中，不堆太狠）
            Vector2 center = Projectile.Center;
            for (int i = 0; i < 48; i++)
            {
                float ang = MathHelper.TwoPi * i / 48f;
                Vector2 p = center + ang.ToRotationVector2() * 56f;
                Dust d = Dust.NewDustPerfect(p, DustID.WhiteTorch, Vector2.Zero, 130, SoftPinkEnd, 1.2f);
                d.noGravity = true;
            }
        }
    }
}
