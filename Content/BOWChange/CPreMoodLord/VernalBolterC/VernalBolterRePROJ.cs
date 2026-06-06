using CalamityMod;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.CPreMoodLord.VernalBolterC
{
    internal class VernalBolterRePROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.CPreMoodLord";
        //public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private static Color ShaderColorOne = Color.Green;
        private static Color ShaderColorTwo = Color.Lime;
        private static Color ShaderEndColor = Color.LightGreen;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 25;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        // 轨迹宽度/颜色（保留）
        private float PrimitiveWidthFunction(float completionRatio, Vector2 vertexPosition)
        {
            float arrowheadCutoff = 0.36f;
            float width = 24f;
            float minHeadWidth = 0.03f;
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
            Color startingColor = Color.Lerp(ShaderColorOne, ShaderColorTwo, startingInterpolant * colorLerpFactor);
            return Color.Lerp(startingColor, ShaderEndColor, MathHelper.SmoothStep(0f, 1f, Utils.GetLerpValue(0f, endFadeRatio, completionRatio, true)));
        }
        public override bool PreDraw(ref Color lightColor)
        {
            GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/SylvestaffStreak"));
            Vector2 overallOffset = Projectile.Size * 0.5f;
            overallOffset += Projectile.velocity * 1.4f;
            int numPoints = 46;
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(PrimitiveWidthFunction, PrimitiveColorFunction, (_, _) => overallOffset, shader: GameShaders.Misc["CalamityMod:TrailStreak"]), numPoints);
            return false;
        }

        // ========= 基础属性（保留） =========
        public override void SetDefaults()
        {
            Projectile.width = 11;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 1200;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;
            Projectile.ignoreWater = true;
            Projectile.arrow = true;
            Projectile.extraUpdates = 5;
        }

        // ========= 追踪?随机游走 状态机 =========
        // 0 = 追踪阶段，1 = 游走阶段
        private int phase = 0;
        // 本阶段还需飞行的“距离门槛”（像素）。用距离而非帧数，避免 extraUpdates 的不确定性。
        private float phaseDistLeft = 0f;
        // 游走阶段的目标速度（方向随机），逐帧逼近
        private Vector2 wanderVel = Vector2.Zero;

        // 参数（可按需微调）
        private const float HomingSpeed = 12f;           // 追踪时的目标速度标量
        private const float HomingLerp = 0.08f;         // 追踪插值（越大越黏）
        private const float WanderLerp = 0.10f;         // 游走插值（越大越快贴近随机向量）

        private const float TrackDistMin = 60f;         // 每轮追踪阶段累计距离下限
        private const float TrackDistMax = 140f;        // 上限
        private const float WanderDistMin = 80f;         // 每轮游走阶段累计距离下限
        private const float WanderDistMax = 180f;        // 上限
        private const float WanderAngleMaxDeg = 42f;     // 游走偏航最大角度
        private const float WanderSpeedMulMin = 0.90f;   // 游走速度倍率范围
        private const float WanderSpeedMulMax = 1.15f;

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.velocity *= 0.85f;
            StartTrackPhase(); // 开局先小段追踪
        }



        // ========= AI =========
        public override void AI()
        {
            // 姿态与发光（保留）
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Lighting.AddLight(Projectile.Center, Color.LightSkyBlue.ToVector3() * 0.49f);
            BowChangeVFX.SpawnTrail(Projectile, BowChangeTheme.Vernal, phase == 0 ? 0.75f : 0.5f);

            // 启动延迟：前X帧只做左右扭动，不追踪
            if (Projectile.timeLeft > 1000)
            {
                float angleOffset = (float)Math.Sin(Main.GameUpdateCount * 0.4f) * MathHelper.ToRadians(10f);
                Projectile.velocity = Projectile.velocity.RotatedBy(angleOffset * 0.2f);
                return;
            }

            // 每帧扣减本阶段剩余距离
            phaseDistLeft -= Projectile.velocity.Length();

            if (phase == 0)
            {
                // 追踪阶段：朝最近敌人缓和转向
                NPC target = Projectile.Center.ClosestNPCAt(9800);
                if (target != null)
                {
                    Vector2 dir = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);

                    // 先生成正常的追踪速度
                    Vector2 desired = dir * HomingSpeed;

                    // 在追踪的基础上叠加一个小小的正弦扰动（螺旋感）
                    float spiralStrength = 0.25f; // 螺旋半径系数
                    float spiralSpeed = 0.3f;     // 螺旋频率
                    float angleOffset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * spiralSpeed * MathHelper.TwoPi) * spiralStrength;

                    // 将方向旋转一个小角度
                    Vector2 spiralDesired = desired.RotatedBy(angleOffset);

                    // 平滑插值靠近目标
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, spiralDesired, HomingLerp);
                }

                // 到达门槛，切换到“随机游走”
                if (phaseDistLeft <= 0f)
                    StartWanderPhase();
            }
            else
            {
                // 游走阶段：沿随机向量平滑飞行
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, wanderVel, WanderLerp);

                // 到达门槛，回到“追踪”
                if (phaseDistLeft <= 0f)
                    StartTrackPhase();
            }
        }




        // 开始追踪阶段：重置距离门槛
        private void StartTrackPhase()
        {
            phase = 0;
            phaseDistLeft = Main.rand.NextFloat(TrackDistMin, TrackDistMax);
        }

        // 开始游走阶段：随机一个偏航与速度倍率，设置门槛和 wanderVel
        private void StartWanderPhase()
        {
            phase = 1;
            phaseDistLeft = Main.rand.NextFloat(WanderDistMin, WanderDistMax);

            float ang = MathHelper.ToRadians(Main.rand.NextFloat(-WanderAngleMaxDeg, WanderAngleMaxDeg));
            float mul = Main.rand.NextFloat(WanderSpeedMulMin, WanderSpeedMulMax);
            wanderVel = Projectile.velocity.RotatedBy(ang);
            float v = wanderVel.Length();
            if (v <= 0.001f) wanderVel = Vector2.UnitX * mul * HomingSpeed;
            else wanderVel = wanderVel.SafeNormalize(Vector2.UnitX) * (v * mul);
        }

        // ========= 伤害与特效（原样保留，不改你喜欢的特效） =========
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.timeLeft >= 300)
                modifiers.FinalDamage *= 2;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            BowChangeVFX.SpawnImpact(Projectile, BowChangeTheme.Vernal, 0.95f);
            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 seedVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY)
                        .RotatedBy(Main.rand.NextFloat(-0.7f, 0.7f))
                        * Main.rand.NextFloat(4f, 7f);
                    Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        Projectile.Center,
                        seedVelocity,
                        ModContent.ProjectileType<VernalBolterReBrambleSeed>(),
                        Math.Max(1, (int)(Projectile.damage * 0.35f)),
                        Projectile.knockBack * 0.3f,
                        Projectile.owner);
                }
            }

            if (Projectile.timeLeft >= 300)
                CreateComplexMagicCircle(target.Center);
        }

        private void CreateComplexMagicCircle(Vector2 center)
        {
            // =========================
            // 参数区（可快速微调）
            // =========================
            const int ringCount = 2;                 // 有序环数量（少，防堆）
            const int pointsPerRing = 10;             // 每环点数（低频）
            const float ringRadiusStep = 36f;         // 环半径步进

            const int chaosDustCount = 14;            // 无序 Dust 数量（少而密）
            const int stabCount = 4;                  // 点刺粒子数量（节拍感）

            // =========================
            // ① 有序层：低频魔法环（Dust 107）
            // =========================
            float baseRot = Main.rand.NextFloat(MathHelper.TwoPi); // 每次命中旋转不同，避免完全重叠

            for (int r = 0; r < ringCount; r++)
            {
                float radius = (r + 1) * ringRadiusStep;

                for (int i = 0; i < pointsPerRing; i++)
                {
                    float angle = baseRot + MathHelper.TwoPi * i / pointsPerRing;
                    Vector2 dir = angle.ToRotationVector2();
                    Vector2 pos = center + dir * radius;

                    Dust d = Dust.NewDustPerfect(pos, 107);
                    d.velocity = dir * 2.2f; // 稳定外扩
                    d.color = Color.Lerp(Color.LimeGreen, Color.White, 0.4f);
                    d.scale = 1.05f;
                    d.noGravity = true;
                }
            }

            // =========================
            // ② 无序层：自然魔法爆散（Dust 107）
            // =========================
            for (int i = 0; i < chaosDustCount; i++)
            {
                Vector2 dir = Main.rand.NextVector2Unit();
                float speed = Main.rand.NextFloat(1.8f, 4.6f);
                float dist = Main.rand.NextFloat(6f, 22f);

                Dust d = Dust.NewDustPerfect(center + dir * dist, 107);
                d.velocity = dir * speed;
                d.color = Color.Lerp(Color.GreenYellow, Color.Lime, Main.rand.NextFloat());
                d.scale = Main.rand.NextFloat(0.85f, 1.15f);
                d.noGravity = true;
            }

            // =========================
            // ③ 点刺节拍：PointParticle（抑制“糊成一团”）
            // =========================
            Vector2 v = Projectile.velocity.SafeNormalize(Vector2.UnitY);

            for (int i = 0; i < stabCount; i++)
            {
                Vector2 offset = Main.rand.NextVector2Circular(6f, 6f);
                Vector2 vel = -v.RotatedBy(Main.rand.NextFloat(-0.25f, 0.25f)) * Main.rand.NextFloat(0.4f, 0.7f);

                PointParticle stab = new PointParticle(
                    center + offset,
                    vel,
                    false,
                    14,
                    1.05f,
                    Color.Orange
                );

                GeneralParticleHandler.SpawnParticle(stab);
            }
        }

        public override void OnKill(int timeLeft)
        {
            // 保留：可选收尾
        }
    }
}
