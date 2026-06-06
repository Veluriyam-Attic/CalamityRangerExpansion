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
using Terraria.Audio;
using Terraria.DataStructures;
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.APreHardMode.LunarianBowC
{
    public class LunarianBowRePROJ : ModProjectile, ILocalizedModType
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public new string LocalizationCategory => "Weapons.APreHardMode";
        private static Color ShaderColorOne = Color.WhiteSmoke; // 着色器颜色1，设置为深绿色
        private static Color ShaderColorTwo = Color.White; // 着色器颜色2，设置为白色
        private static Color ShaderEndColor = Color.GhostWhite; // 着色器结束颜色，设置为浅绿色

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        private float PrimitiveWidthFunction(float completionRatio, Vector2 vertexPosition) // 计算弹幕宽度变化
        {
            float arrowheadCutoff = 0.36f; // 箭头部分的截止点
            float width = 24f; // 设置默认宽度为24
            float minHeadWidth = 0.03f; // 设置最小宽度为0.03
            float maxHeadWidth = width; // 最大宽度为24
            if (completionRatio <= arrowheadCutoff) // 如果进度比小于截止点
                width = MathHelper.Lerp(minHeadWidth, maxHeadWidth, Utils.GetLerpValue(0f, arrowheadCutoff, completionRatio, true)); // 计算渐变宽度
            return width; // 返回计算后的宽度
        }

        private Color PrimitiveColorFunction(float completionRatio, Vector2 vertexPosition) // 计算弹幕颜色变化
        {
            float endFadeRatio = 0.41f; // 结束渐变比例
            float completionRatioFactor = 2.7f; // 完成比例因子
            float globalTimeFactor = 5.3f; // 全局时间因子
            float endFadeFactor = 3.2f; // 结束渐变因子
            float endFadeTerm = Utils.GetLerpValue(0f, endFadeRatio * 0.5f, completionRatio, true) * endFadeFactor; // 结束渐变项
            float cosArgument = completionRatio * completionRatioFactor - Main.GlobalTimeWrappedHourly * globalTimeFactor + endFadeTerm; // 计算余弦参数
            float startingInterpolant = (float)Math.Cos(cosArgument) * 0.5f + 0.5f; // 计算颜色插值

            float colorLerpFactor = 0.6f; // 颜色渐变因子
            Color startingColor = Color.Lerp(ShaderColorOne, ShaderColorTwo, startingInterpolant * colorLerpFactor); // 计算起始颜色
            return Color.Lerp(startingColor, ShaderEndColor, MathHelper.SmoothStep(0f, 1f, Utils.GetLerpValue(0f, endFadeRatio, completionRatio, true))); // 返回渐变后的颜色
        }

        public override bool PreDraw(ref Color lightColor) // 在绘制前的操作
        {
            GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/SylvestaffStreak")); // 设置着色器的纹理
            Vector2 overallOffset = Projectile.Size * 0.5f; // 计算整体偏移量
            overallOffset += Projectile.velocity * 1.4f; // 调整偏移量
            int numPoints = 46; // 点的数量
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(PrimitiveWidthFunction, PrimitiveColorFunction, (_, _) => overallOffset, shader: GameShaders.Misc["CalamityMod:TrailStreak"]), numPoints); // 渲染拖尾效果
            return false; // 不执行默认绘制
        }
        public override void SetDefaults()
        {
            // 设置弹幕的基础属性
            Projectile.width = 11; // 弹幕宽度
            Projectile.height = 24; // 弹幕高度
            Projectile.friendly = true; // 对敌人有效
            Projectile.DamageType = DamageClass.Ranged; // 远程伤害类型
            Projectile.penetrate = 2; // 穿透力
            Projectile.timeLeft = 300; // 弹幕存在时间为x帧
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
            Projectile.ignoreWater = true; // 弹幕不受水影响
            Projectile.arrow = true;
            Projectile.extraUpdates = 4;
        }
        private int curveTimer = 0;     // 曲线阶段计时器
        private Vector2 curveVel;       // 曲线飞行方向

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.velocity *= 1f;
        }
        public override void AI()
        {
            // 调整弹幕的旋转，使其在飞行时保持水平
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;

            // Lighting - 添加天蓝色光源，光照强度为 0.49
            Lighting.AddLight(Projectile.Center, Color.LightSkyBlue.ToVector3() * 0.49f);
            BowChangeVFX.SpawnTrail(Projectile, BowChangeTheme.Lunar, 0.4f);



            if (curveTimer > 0)
            {
                curveTimer--;

                // 曲线：轻微摇摆
                Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(0.8f)) * 0.99f;

                if (curveTimer == 0)
                {
                    // 曲线结束 → 开始重新追踪
                    NPC closestNPC = Main.npc
                        .Where(npc => npc.active && !npc.friendly && npc.life > 0)
                        .OrderBy(npc => Vector2.Distance(npc.Center, Projectile.Center))
                        .FirstOrDefault();

                    if (closestNPC != null)
                    {
                        Vector2 direction = closestNPC.Center - Projectile.Center;
                        Projectile.velocity = Vector2.Normalize(direction) * Projectile.velocity.Length();
                    }
                }
            }

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            BowChangeVFX.SpawnImpact(Projectile, BowChangeTheme.Lunar, 0.75f);
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    target.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<LunarianBowReMoonSigil>(),
                    Math.Max(1, (int)(Projectile.damage * 0.45f)),
                    Projectile.knockBack * 0.25f,
                    Projectile.owner);
            }

            // 瞄准最近的敌人并调整弹幕方向
            NPC closestNPC = Main.npc
                .Where(npc => npc.active && !npc.friendly && npc.life > 0 && npc.whoAmI != target.whoAmI)
                .OrderBy(npc => Vector2.Distance(npc.Center, Projectile.Center))
                .FirstOrDefault();


            if (closestNPC != null)
            {
                // 命中后先折射，偏移 ±10°
                float offset = Main.rand.NextBool() ? MathHelper.ToRadians(10f) : MathHelper.ToRadians(-10f);
                Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(offset);
                curveVel = dir * Projectile.velocity.Length();

                // 开始进入曲线阶段
                curveTimer = 30; // 例如持续 30 帧
                Projectile.velocity = curveVel;
            }

        }
        public override void OnKill(int timeLeft)
        {
            Vector2 center = Projectile.Center; // 特效中心位置
            int numParticles = 60; // 月牙的粒子数
            float radius = 60f; // 月牙的半径

            // 随机选择月牙的半圆方向
            float startAngle = Main.rand.NextFloat(MathHelper.TwoPi); // 随机起始角度
            float endAngle = startAngle + MathHelper.Pi; // 半圆范围

            // 绘制月牙的粒子
            for (int i = 0; i < numParticles; i++)
            {
                float angle = MathHelper.TwoPi * i / numParticles; // 每个粒子的位置角度
                if (angle < startAngle || angle > endAngle)
                {
                    // 跳过不属于随机半圆的部分
                    continue;
                }

                Vector2 position = center + angle.ToRotationVector2() * radius;
                Dust moonDust = Dust.NewDustPerfect(position, 63); // 使用63号WhiteTorch粒子
                moonDust.velocity = Vector2.Zero; // 静止粒子
                moonDust.color = Color.Lerp(Color.LightBlue, Color.White, Main.rand.NextFloat()); // 浅蓝到白色渐变
                moonDust.scale = 1.5f;
                moonDust.noGravity = true;
            }

            // 绘制月亮环的粒子
            for (int i = 0; i < 36; i++)
            {
                float angle = MathHelper.TwoPi * i / 36f;
                Vector2 position = center + angle.ToRotationVector2() * (radius + 20f);

                Dust ringDust = Dust.NewDustPerfect(position, 63); // 使用63号WhiteTorch粒子
                ringDust.velocity = angle.ToRotationVector2() * 2f; // 环绕旋转的速度
                ringDust.color = Color.White; // 纯白色
                ringDust.scale = 1.2f;
                ringDust.noGravity = true;
            }

            // 播放音效
            //SoundEngine.PlaySound(SoundID.Item29, Projectile.position);
        }





    }
}
