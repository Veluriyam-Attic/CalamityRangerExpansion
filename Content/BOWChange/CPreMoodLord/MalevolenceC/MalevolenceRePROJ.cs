using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Particles;
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.CPreMoodLord.MalevolenceC
{
    internal class MalevolenceRePROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.CPreMoodLord";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public static int MaxUpdate = 3; // 定义一个静态变量，表示弹幕每次更新的最大次数
        private int Lifetime = 900; // 定义弹幕的生命周期

        // 更改颜色：深绿色、黑色、另一种深绿色
        private static Color ShaderColorOne = Color.DarkGreen; // 着色器颜色1，设置为深绿色
        private static Color ShaderColorTwo = Color.Black; // 着色器颜色2，设置为黑色
        private static Color ShaderEndColor = Color.ForestGreen; // 着色器结束颜色，设置为森林绿色（另一种深绿色）

        private Vector2 altSpawn; // 定义一个备用生成位置向量

        public override void SetStaticDefaults() // 设置弹幕的静态默认值
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // 设置拖尾模式为2
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 21; // 设置拖尾缓存长度为21
        }
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
            GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityRangerExpansion/texture/KsTexture/spark_07"));
            Vector2 overallOffset = Projectile.Size * 0.5f;
            overallOffset += Projectile.velocity * 1.4f;
            int numPoints = 46;
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(PrimitiveWidthFunction, PrimitiveColorFunction, (_, _) => overallOffset, shader: GameShaders.Misc["CalamityMod:TrailStreak"]), numPoints);



            SpriteBatch spriteBatch = Main.spriteBatch; // ??添加这一行！

            Texture2D tex = ModContent.Request<Texture2D>("CalamityRangerExpansion/Content/BOWChange/CPreMoodLord/MalevolenceC/MalevolenceRePROJ").Value;
            Vector2 pos = Projectile.Center - Main.screenPosition;

            spriteBatch.Draw(
                tex,
                pos,
                null,
                Color.White,
                Projectile.rotation,
                tex.Size() * 0.5f,
                Projectile.scale,
                SpriteEffects.None,
                0f
            );

            return false;
        }
        public override void SetDefaults() // 设置弹幕的默认值
        {
            Projectile.width = Projectile.height = 24;
            Projectile.arrow = true;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = Lifetime;
            Projectile.MaxUpdates = MaxUpdate;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        private bool stuck = false;
        private NPC stuckTarget = null;
        private Vector2 stuckOffset = Vector2.Zero;
        private int summonTimer = 0;
        private bool hasDetectedTarget = false;
        private int trackingCountdown = -1; // -1 表示未激活
        private float lockedRotation; // 用于锁定飞行角度

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            BowChangeVFX.SpawnTrail(Projectile, BowChangeTheme.Malevolence, stuck ? 0.35f : 0.75f);

            Projectile.ai[0]++; // 计时器推进

            // === 飞行特效（绿色尾迹）===
            Vector2 center = Projectile.Center;
            if (Projectile.timeLeft > 10)
            {
                Dust dust = Dust.NewDustPerfect(center, DustID.GreenFairy, -Projectile.velocity * 0.2f, 100, Color.GreenYellow, 1f);
                dust.noGravity = true;

                Particle spark = new SparkParticle(center, -Projectile.velocity * 0.1f, false, 20, 0.9f, Color.Lime * 0.8f);
                GeneralParticleHandler.SpawnParticle(spark);
            }

            // === 延迟追踪触发 ===
            if (!hasDetectedTarget)
            {
                float radius = new Vector2(5 * 16f, 10 * 16f).Length() * 0.5f;
                NPC close = Projectile.Center.ClosestNPCAt(radius);
                if (close != null)
                {
                    hasDetectedTarget = true;
                    trackingCountdown = 20;
                }
            }
            else if (trackingCountdown > 0)
            {
                trackingCountdown--;
            }
            else if (!stuck) // ??倒计时结束，且未粘附
            {
                NPC target = Projectile.Center.ClosestNPCAt(2000f); // ??更大范围
                if (target != null && target.CanBeChasedBy())
                {
                    Vector2 desiredVelocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitX) * 14f;
                    float inertia = 20f; // ??越大越丝滑

                    // Lerp 当前速度朝目标靠近
                    Projectile.velocity = (Projectile.velocity * (inertia - 1f) + desiredVelocity) / inertia;

                    // 角度朝向同步
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                }
            }


            // === 粘在敌人身上 ===
            if (stuck && stuckTarget != null && stuckTarget.active)
            {
                Projectile.Center = stuckTarget.Center + stuckOffset;
                Projectile.velocity = stuckTarget.velocity;
                Projectile.rotation = lockedRotation;
                Projectile.tileCollide = false;

                // 计算释放频率：速度越快，频率越高（限制范围在[x, y]帧）
                float targetSpeed = stuckTarget.velocity.Length();
                float rate = MathHelper.Clamp(80f - targetSpeed * 6f, 5f, 30f);

                summonTimer++;
                if (summonTimer >= rate)
                {
                    summonTimer = 0;

                    // 掉箭：从敌人周围随机生成
                    Vector2 arrowSpawn = stuckTarget.Center + Main.rand.NextVector2Circular(300f, 300f);
                    Vector2 arrowVel = (stuckTarget.Center - arrowSpawn).SafeNormalize(Vector2.UnitY) * 14f;

                    Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        arrowSpawn,
                        arrowVel,
                        ModContent.ProjectileType<MalevolenceReARROW>(),
                        (int)(Projectile.damage * 0.6f),
                        1f,
                        Projectile.owner
                    );

                    // 生成蜜蜂：从自己位置爆裂式释放
                    for (int i = 0; i < 7; i++) // 原来是2，现在 = 2 * 3.5
                    {
                        Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f).SafeNormalize(Vector2.UnitX).RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat(6f, 11f);

                        int bee = Projectile.NewProjectile(
                            Projectile.GetSource_FromThis(),
                            Projectile.Center,
                            velocity,
                            ModContent.ProjectileType<PlaguenadeBee>(),
                            (int)(Projectile.damage * 0.3f),
                            0f,
                            Projectile.owner
                        );

                        if (Main.projectile.IndexInRange(bee))
                        {
                            Projectile proj = Main.projectile[bee];
                            proj.friendly = true;
                            proj.hostile = false;
                            proj.penetrate = 1;
                            proj.usesLocalNPCImmunity = true;
                            proj.localNPCHitCooldown = 20;
                        }
                    }

                }
            }







        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!stuck)
            {
                lockedRotation = Projectile.rotation;

                stuck = true;
                stuckTarget = target;
                stuckOffset = Projectile.Center - target.Center;

                Projectile.velocity = Vector2.Zero;
                Projectile.tileCollide = false;
                Projectile.timeLeft = 300;
                Projectile.netUpdate = true;
                BowChangeVFX.SpawnImpact(Projectile, BowChangeTheme.Malevolence, 1f);

                if (Projectile.owner == Main.myPlayer)
                {
                    Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        target.Center,
                        Vector2.Zero,
                        ModContent.ProjectileType<MalevolenceRePlagueNest>(),
                        Math.Max(1, (int)(Projectile.damage * 0.42f)),
                        0f,
                        Projectile.owner);
                }
            }

            // Buff依然保留
            target.AddBuff(BuffID.Venom, 180);
            target.AddBuff(ModContent.BuffType<Plague>(), 180);
            target.AddBuff(BuffID.Poisoned, 180);
        }




















    }
}
