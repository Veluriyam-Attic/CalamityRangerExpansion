using CalamityMod;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;
using CalamityMod.Particles;

namespace CalamityRangerExpansion.Content.BOWChange.BPrePlantera.DarkechoGreatbowC
{
    public class DarkechoGreatbowReDarts : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.BPrePlantera";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        private static Color ShaderColorOne = Color.DarkSlateGray; // 着色器颜色1，深灰色，象征黑暗
        private static Color ShaderColorTwo = Color.MidnightBlue; // 着色器颜色2，深蓝色，象征寒冷
        private static Color ShaderEndColor = Color.Purple; // 着色器结束颜色，暗紫色，象征邪恶
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
            Projectile.penetrate = 1; // 穿透力为1，击中一个敌人就消失
            Projectile.timeLeft = 450; // 弹幕存在时间为x帧
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
            Projectile.ignoreWater = true; // 弹幕不受水影响
            Projectile.arrow = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            // 初始定位和方向调整
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;

                Vector2 vectorToPlayer = player.Center - Projectile.Center;
                Projectile.rotation = vectorToPlayer.ToRotation();
                Projectile.velocity = vectorToPlayer.SafeNormalize(Vector2.Zero) * 28f; // 固定速度
            }

            // 追踪玩家
            float speed = 28f;
            Vector2 directionToPlayer = (player.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * speed;
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, directionToPlayer, 0.1f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            // 添加光效
            Lighting.AddLight(Projectile.Center, Color.LightSkyBlue.ToVector3() * 0.49f);

            // 与玩家重叠时销毁并恢复固定生命值
            if (Projectile.Hitbox.Intersects(player.Hitbox))
            {
                player.statLife += 2;
                player.HealEffect(2);
                Projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Frostburn2, 300); // 原版的冻伤效果
        }
        public override void OnKill(int timeLeft)
        {
            // 在死亡时释放紫色粒子效果
            for (int i = 0; i < 20; i++)
            {
                Vector2 randomDirection = Main.rand.NextVector2Circular(1f, 1f) * Main.rand.NextFloat(2f, 4f);
                Particle electricParticle = new SparkParticle(
                    Projectile.Center,
                    randomDirection,
                    false,
                    60,
                    Main.rand.NextFloat(0.8f, 1.2f),
                    Color.Purple
                );
                GeneralParticleHandler.SpawnParticle(electricParticle);
            }
        }






    }
}
