using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Graphics.Metaballs;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityMod;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Graphics.Metaballs;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using CalamityRangerExpansion.CREConfigs;

namespace CalamityRangerExpansion.Content.DeveloperItems.Bullet.AllTheBirds
{
    public class AllTheBirdsPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.AllTheBirds";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int frameHeight = texture.Height / Main.projFrames[Projectile.type]; // 计算每帧的高度
            int frameY = Projectile.frame * frameHeight; // 获取当前帧在纹理中的起始Y坐标
            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight); // 定义绘制的源矩形
            Vector2 drawOrigin = new Vector2(texture.Width / 2, frameHeight / 2); // 计算绘制的中心点

            Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

            // 判断图像是否需要翻转
            SpriteEffects spriteEffects = Projectile.velocity.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // 绘制
            spriteBatch.Draw(texture, drawPosition, sourceRectangle, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects, 0f);
            return false;
        }


        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 1200;
            Projectile.extraUpdates = 1;
            Projectile.aiStyle = -1;
            Projectile.tileCollide = false;
        }

        private Vector2 controlPoint1;
        private Vector2 controlPoint2;

        private float sineAmplitude = 13f; // x个方块
        private float sineFrequencyOffset;

        public override void OnSpawn(IEntitySource source)
        {
            controlPoint1 = Projectile.position + new Vector2(Main.rand.NextFloat(-20f, 20f), Main.rand.NextFloat(-20f, 20f));
            controlPoint2 = Projectile.position + new Vector2(Main.rand.NextFloat(-20f, 20f), Main.rand.NextFloat(-20f, 20f));

            // / xf 代表一个周期
            float sineValue = (float)Math.Sin((Projectile.ai[0] / 60f) * MathHelper.TwoPi + sineFrequencyOffset);

        }



        // 在类中添加变量，跟踪转向总量
        private float totalLeftTurn = 0f; // 跟踪左转总弧度
        private float totalRightTurn = 0f; // 跟踪右转总弧度
        private const float maxTurnAngle = MathHelper.PiOver4; // 最大转向量为 45 度（转弧度）
        private const float turnIncrement = 0.5f; // 每次转向的弧度增量  
        public override void AI()
        {
            // 适用于六帧动图的切换逻辑
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4) // 调整这个值可以控制帧切换的速度
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 4) // 5帧动图时，最大帧数为 4
            {
                Projectile.frame = 0;
            }

            // 前40帧使用贝塞尔曲线随机运动
            //if (Projectile.ai[0] < 40)
            //{
            //    Vector2 controlPoint1 = Projectile.position + new Vector2(Main.rand.NextFloat(-20f, 20f), Main.rand.NextFloat(-20f, 20f));
            //    Vector2 controlPoint2 = Projectile.position + new Vector2(Main.rand.NextFloat(-20f, 20f), Main.rand.NextFloat(-20f, 20f));
            //    float t = Projectile.ai[0] / 40f;
            //    Projectile.position = Vector2.CatmullRom(controlPoint1, Projectile.position, controlPoint2, Projectile.position, t);
            //    Projectile.ai[0]++;
            //}
            //if (Projectile.ai[0] < 40)
            //{
            //    float t = Projectile.ai[0] / 40f;
            //    Projectile.position = Vector2.CatmullRom(controlPoint1, Projectile.position, controlPoint2, Projectile.position, t);
            //    Projectile.ai[0]++;
            //    Projectile.velocity *= 1.02f; // 逐渐加速
            //}
            //if (Projectile.ai[0] < 130)
            //{
            //    float sineValue = (float)Math.Sin((Projectile.ai[0] / 20f) * MathHelper.TwoPi + sineFrequencyOffset);
            //    float offset = sineValue * sineAmplitude;

            //    // 基于弹幕当前的移动方向，增加垂直于方向的偏移
            //    Vector2 perpendicularOffset = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * offset;
            //    Projectile.position += perpendicularOffset;

            //    Projectile.ai[0]++;

            //    // 重新规范化速度向量，使其保持恒定的4f绝对速度
            //    Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 4f;
            //}
            //if (Projectile.ai[0] < 130)
            //{
            //    float sineValue = (float)Math.Sin((Projectile.ai[0] / 20f) * MathHelper.TwoPi + sineFrequencyOffset);
            //    float offset = sineValue * sineAmplitude;

            //    // 基于弹幕当前的移动方向，增加垂直于方向的偏移
            //    Vector2 perpendicularOffset = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * offset;
            //    Projectile.position += perpendicularOffset;

            //    Projectile.ai[0]++;

            //    // 重新计算速度向量的大小，使其保持恒定为 3f
            //    Vector2 newVelocity = Projectile.velocity + perpendicularOffset;
            //    Projectile.velocity = newVelocity.SafeNormalize(Vector2.Zero) * 3f;
            //}
            if (Projectile.ai[0] < 130)
            {
                // 随机选择转向方向
                bool turnLeft = Main.rand.NextBool();

                // 检查是否需要强制改变方向
                if (turnLeft && totalLeftTurn >= maxTurnAngle)
                {
                    turnLeft = false; // 强制右转
                }
                else if (!turnLeft && totalRightTurn >= maxTurnAngle)
                {
                    turnLeft = true; // 强制左转
                }

                // 进行转向
                float rotationChange = turnIncrement * (MathHelper.Pi / 180f); // 0.5度转成弧度
                if (turnLeft)
                {
                    Projectile.velocity = Projectile.velocity.RotatedBy(-rotationChange);
                    totalLeftTurn += rotationChange;
                    totalRightTurn = Math.Max(0f, totalRightTurn - rotationChange); // 防止右转过度积累
                }
                else
                {
                    Projectile.velocity = Projectile.velocity.RotatedBy(rotationChange);
                    totalRightTurn += rotationChange;
                    totalLeftTurn = Math.Max(0f, totalLeftTurn - rotationChange); // 防止左转过度积累
                }

                // 规范化速度，保持恒定为 xf
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 7.5f;

                Projectile.ai[0]++;
            }
            else
            {
                // 前30帧不追踪，之后开始追踪敌人
                if (Projectile.ai[1] > 130)
                {
                    NPC target = Projectile.Center.ClosestNPCAt(5000); // 查找范围内最近的敌人
                    if (target != null)
                    {
                        Vector2 direction = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * 10f, 0.08f); // 追踪速度为xf
                    }

                    // 触发追踪能力激活的效果
                    if (Projectile.ai[1] == 120)
                    {
                        // 检查是否启用了特效
                        if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                float angle = MathHelper.ToRadians(120 * i);
                                Vector2 particleVelocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 3f;
                                Particle spark = new PointParticle(Projectile.Center, particleVelocity, false, 7, 1.5f, Color.LightYellow);
                                GeneralParticleHandler.SpawnParticle(spark);
                            }
                        }
                    }
                }
                else
                {
                    Projectile.ai[1]++;
                }
            }
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 在飞行期间生成浅黄色的粒子特效
                if (Main.rand.NextBool(1)) // 随机生成粒子
                {
                    Vector2 dustPosition = Projectile.position + new Vector2(Main.rand.NextFloat(Projectile.width), Main.rand.NextFloat(Projectile.height));
                    Dust dust = Dust.NewDustDirect(dustPosition, 0, 0, DustID.Torch, 0f, 0f, 100, Color.LightYellow, 1.5f);
                    dust.noGravity = true;
                }
            }

        }

        public override void OnKill(int timeLeft)
        {
            // 在死亡时向自己面向的反方向抛射出 15 个粉红色的原版粒子特效（Dust）
            for (int i = 0; i < 15; i++)
            {
                float angle = MathHelper.ToRadians(Main.rand.NextFloat(-45f, 45f)); // 在 -45 到 45 度之间随机生成角度
                Vector2 dustVelocity = Projectile.velocity.RotatedBy(angle) * -0.5f; // 反方向速度，加上随机旋转
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.PinkTorch, dustVelocity.X, dustVelocity.Y);
                dust.noGravity = true;
                dust.scale = 1.5f;
            }

            for (int i = 0; i < 3; i++)
            {
                float angle = MathHelper.ToRadians(120 * i);
                Vector2 particleVelocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 3f;
                Particle spark = new PointParticle(Projectile.Center, particleVelocity, false, 7, 1.5f, Color.Yellow);
                GeneralParticleHandler.SpawnParticle(spark);
            }

            SoundEngine.PlaySound(SoundID.Zombie118, Projectile.position);

        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {


        }



    }


}