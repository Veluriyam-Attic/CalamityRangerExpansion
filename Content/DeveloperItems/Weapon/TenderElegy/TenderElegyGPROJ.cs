using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Terraria.GameContent;
using CalamityMod.Graphics.Primitives;
using System;

namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.TenderElegy
{
    public class TenderElegyGPROJ : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool isSpecialArrow = false;
        private float breathEffect = 0f; // 呼吸灯效果
        private bool increasing = true; // 控制呼吸灯增减
        private int homingStartTimer = 0; // **追踪启动计时器**
        private bool isHoming = false; // **记录是否进入追踪模式**

        public override void AI(Projectile projectile)
        {
            if (isSpecialArrow)
            {
                // 让呼吸灯效果进行动态变化
                //if (increasing)
                //{
                //    breathEffect += 0.02f;
                //    if (breathEffect >= 1f) increasing = false;
                //}
                //else
                //{
                //    breathEffect -= 0.02f;
                //    if (breathEffect <= 0f) increasing = true;
                //}
                if (projectile.ai[0] == 0)
                {
                    projectile.ai[0] = projectile.Center.X; // **记录初始 X 坐标**
                    projectile.ai[1] = projectile.Center.Y; // **记录初始 Y 坐标**
                }

                float distanceX = projectile.Center.X - projectile.ai[0];
                float distanceY = projectile.Center.Y - projectile.ai[1];
                float totalDistance = (float)Math.Sqrt(distanceX * distanceX + distanceY * distanceY);

                // 玩家的屏幕为120×60格，半个屏幕的大小就是60×30格
                // 根据毕达哥拉斯定理
                // sqrt[60^2 + 30^2] *16 = 1073.28
                // **如果飞行距离超过 1073.28f，禁用原 AI，并强制进入追踪**
                if (totalDistance >= 1073.28f) // **飞行超过半个屏幕对角线**
                {
                    if (!isHoming)
                    {
                        isHoming = true; // **标记进入追踪模式**

                        // **隐藏绘制 5 帧**
                        hideFrames = 10; // **5 帧前 + 5 帧后**

                        // **寻找最近的敌人**
                        NPC target = projectile.Center.ClosestNPCAt(8848);
                        if (target != null)
                        {
                            // **计算一个随机的偏移量，让弹幕出现在敌人半径 10×16 的圆圈上**
                            float angle = Main.rand.NextFloat(0, MathHelper.TwoPi);
                            Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * (10 * 16);

                            // **传送到目标附近**
                            projectile.Center = target.Center + offset;

                            // **调整速度，让它砸向目标**
                            Vector2 direction = (target.Center - projectile.Center).SafeNormalize(Vector2.Zero);
                            projectile.velocity = direction * 30f;

                            // **调整 rotation，让弹幕朝向目标**
                            projectile.rotation = projectile.velocity.ToRotation();
                        }
                    }

                    // **强制追踪**
                    CalamityUtils.HomeInOnNPC(projectile, true, 8848f, 40f, 1f);
                }

                // **减少 hideFrames 计时**
                if (hideFrames > 0)
                    hideFrames--;
            }
        }
        public override void SetDefaults(Projectile projectile)
        {
            if (isSpecialArrow)
            {
                // 强制修改拖尾缓存长度和模式
                ProjectileID.Sets.TrailCacheLength[projectile.type] = 155;
                ProjectileID.Sets.TrailingMode[projectile.type] = 0;
                projectile.width = 30;
                projectile.height = 30;
                projectile.tileCollide = false;
                projectile.extraUpdates = 1;



            }
        }
        public override void PostDraw(Projectile projectile, Color lightColor)
        {
            if (isSpecialArrow)
            {
                // **禁用所有原始 `PostDraw` 逻辑**
                return;
            }
        }
        private float ImpFlameWidthFunction(float completionRatio, Vector2 vexpos)
        {
            float baseWidth = 30f; // **稍微加宽拖尾**
            float taper = MathHelper.Lerp(1f, 0.2f, completionRatio); // **让拖尾逐渐变细**
            return baseWidth * taper;
        }
        private Color ImpFlameColorFunction(float completionRatio, Vector2 vexpos)
        {
            float localIdentityOffset = Main.rand.NextFloat(0.1f, 0.2f); // **每个弹幕稍微不同**

            // **让颜色动态渐变**
            Color baseColor = CalamityUtils.MulticolorLerp(
                (Main.GlobalTimeWrappedHourly * 2f + localIdentityOffset) % 1f,
                Color.Cyan, Color.Lime, Color.GreenYellow, Color.Goldenrod, Color.Orange
            );

            return Color.Lerp(baseColor, Color.Transparent, completionRatio);
        }
        private int hideFrames = 0; // **传送前/后的无绘制计时器**

        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            if (isSpecialArrow)
            {
                if (hideFrames > 0)
                {
                    return false; // **禁用弹幕绘制**
                }

                Texture2D texture = ModContent.Request<Texture2D>("CalamityRangerExpansion/Content/DeveloperItems/Weapon/TenderElegy/TenderElegyPROJ").Value;
                Vector2 drawPosition = projectile.Center - Main.screenPosition;

                // **应用 HeavenlyGaleTrail Shader**
                GameShaders.Misc["CalamityMod:HeavenlyGaleTrail"].SetShaderTexture(
                    ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/EternityStreak")
                );
                GameShaders.Misc["CalamityMod:HeavenlyGaleTrail"].UseImage2("Images/Extra_189");

                // **计算动态颜色**
                float localIdentityOffset = projectile.identity * 0.1372f;
                Color mainColor = CalamityUtils.MulticolorLerp(
                    (Main.GlobalTimeWrappedHourly * 2f + localIdentityOffset) % 1f,
                    Color.Cyan, Color.Lime, Color.GreenYellow, Color.Goldenrod, Color.Orange
                );
                Color secondaryColor = CalamityUtils.MulticolorLerp(
                    (Main.GlobalTimeWrappedHourly * 2f + localIdentityOffset + 0.2f) % 1f,
                    Color.Cyan, Color.Lime, Color.GreenYellow, Color.Goldenrod, Color.Orange
                );

                GameShaders.Misc["CalamityMod:HeavenlyGaleTrail"].UseColor(mainColor);
                GameShaders.Misc["CalamityMod:HeavenlyGaleTrail"].UseSecondaryColor(secondaryColor);
                GameShaders.Misc["CalamityMod:HeavenlyGaleTrail"].Apply();

                // **提高拖尾采样点，保证平滑度**
                int numPoints = 73;
                PrimitiveRenderer.RenderTrail(
                    projectile.oldPos,
                    new(
                        ImpFlameWidthFunction,
                        ImpFlameColorFunction,
                        (_, _) => projectile.Size * 0.5f + new Vector2(0, 0), // 后面的值是左右偏移，前面的是前后
                        shader: GameShaders.Misc["CalamityMod:HeavenlyGaleTrail"]
                    ),
                    numPoints
                );

                // 应用光学染色效果
                //float colorInterpolation = (float)Math.Cos(Main.GlobalTimeWrappedHourly / 2f + breathEffect * MathHelper.Pi) * 0.5f + 0.5f;
                //Color glowColor = Color.Lerp(Color.LightSkyBlue, Color.Cyan, colorInterpolation) * 0.6f;

                //Main.EntitySpriteDraw(
                //    texture,
                //    drawPosition,
                //    null,
                //    glowColor, // 使用动态颜色
                //    projectile.rotation,
                //    texture.Size() / 2,
                //    projectile.scale,
                //    SpriteEffects.None,
                //    0
                //);

                // 暂时先禁用光学染色
                Main.EntitySpriteDraw(
                    texture,
                    drawPosition,
                    null,
                    lightColor, // **直接使用环境光颜色**
                    projectile.rotation,
                    texture.Size() / 2,
                    projectile.scale,
                    SpriteEffects.None,
                    0
                );

                return false; // **完全禁用原本的绘制**
            }
            return true;
        }


        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (isSpecialArrow)
            {
                var npcGlobal = target.GetGlobalNPC<TenderElegyBeilGNPC>();

                // 先尝试生成铃铛弹幕
                if (!npcGlobal.hasBell)
                {
                    Vector2 spawnPosition = target.Center + new Vector2(0, -30);
                    int proj = Projectile.NewProjectile(
                        projectile.GetSource_FromThis(),
                        spawnPosition, Vector2.Zero,
                        ModContent.ProjectileType<TenderElegyBeil>(),
                        (int)(projectile.damage * 0.5f),
                        projectile.knockBack,
                        projectile.owner,
                        target.whoAmI // 绑定目标敌人 ID
                    );

                    Main.projectile[proj].ai[0] = target.whoAmI; // 存储目标敌人 ID
                    npcGlobal.hasBell = true; // **然后** 标记敌人已拥有铃铛
                }
            }
        }
    }
}


