using CalamityMod.Particles;
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
using CalamityMod;

namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.BlindBirdCry
{
    internal class BlindBirdCryCrods : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.BlindBirdCry";
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
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 7;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 50;
            Projectile.DamageType = DamageClass.Ranged;
        }


        public override void OnSpawn(IEntitySource source)
        {
            // 在弹幕出现时生成黑色椭圆形光圈
            Particle pulse = new DirectionalPulseRing(
                Projectile.Center, // 生成位置
                Projectile.velocity * 0.75f, // 初始速度，朝正前方
                Color.Black, // 光圈颜色
                new Vector2(1f, 2.5f), // 椭圆比例
                Projectile.rotation, // 旋转角度
                0.2f, // 初始缩放
                0.03f, // 最终缩放
                20 // 持续时间
            );
            GeneralParticleHandler.SpawnParticle(pulse);
        }

        public bool ableToHit = true;
        public ref float Time => ref Projectile.ai[1];
        public override bool? CanDamage() => Time >= 80f; // 初始的时候不会造成伤害，直到x为止

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


            //if (Projectile.localAI[0] < 60) // 前 1 秒飞行
            //{
            //    if (Projectile.localAI[0] % 3 == 0) // 每 x 帧
            //    {
            //        //Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.PiOver2);
            //        //Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(4));
            //        Projectile.velocity *= 0.85f;
            //    }
            //    //Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 1.0f;
            //}
            //else // 使用新的追踪逻辑
            //{
            //    NPC target = Projectile.Center.ClosestNPCAt(5800);
            //    if (target != null)
            //    {
            //        Vector2 direction = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
            //        Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * 10f, 0.08f);
            //    }
            //}

            if (Projectile.timeLeft <= 20) // 弹幕即将消失时停止造成伤害
            {
                ableToHit = false;
            }
            Time++;


            // 前x帧不追踪，之后开始追踪敌人
            if (Projectile.ai[1] > 60)
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



                }
            }
            else
            {
                Projectile.ai[1]++;
            }


            // 每帧生成 186 号 Dust 粒子特效，形成旋转圆圈
            for (int i = 0; i < 3; i++)
            {
                float angle = MathHelper.TwoPi / 3 * i + Projectile.ai[1] * 0.1f; // 三个规则点，每帧旋转
                Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 16f * 1; // 半径为 2×16
                Vector2 position = Projectile.Center + offset;

                Dust dust = Dust.NewDustPerfect(position, 186, Vector2.Zero, 0, Color.Black, 1.2f);
                dust.noGravity = true; // 粒子无重力
            }

        }
        public override void OnKill(int timeLeft)
        {
            // 正前方、左偏 20 度、右偏 20 度方向
            float[] angles = { 0f, -MathHelper.ToRadians(20f), MathHelper.ToRadians(20f) };
            int particleCount = Main.rand.Next(15, 21); // 每个方向 15~20 个轻型烟雾

            foreach (float angle in angles)
            {
                for (int i = 0; i < particleCount; i++)
                {
                    Vector2 dustVelocity = Projectile.velocity.RotatedBy(angle) * Main.rand.NextFloat(1f, 2.6f);
                    Particle smoke = new HeavySmokeParticle(
                        Projectile.Center,
                        dustVelocity, // 初始速度
                        Color.Black, // 烟雾颜色
                        18, // 粒子寿命
                        Main.rand.NextFloat(0.9f, 1.6f), // 缩放比例
                        0.35f, // 透明度
                        Main.rand.NextFloat(-1, 1), // 旋转速度
                        true // 无重力
                    );
                    GeneralParticleHandler.SpawnParticle(smoke);
                }
            }
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<BlindBirdCryEDebuff>(), 1200);
        }



    }


}