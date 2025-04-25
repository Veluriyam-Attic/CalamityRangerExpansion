using CalamityMod;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
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
using Terraria.Audio;
using CalamityRangerExpansion.CREConfigs;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using CalamityMod.Graphics.Primitives;

namespace CalamityRangerExpansion.Content.DeveloperItems.Arrow.MaoMaoChong
{
    public class MaoMaoChongPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.MaoMaoChong";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 36; // 设置较长的拖尾长度
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2; // 模式2拖尾
            //Main.projFrames[Projectile.type] = 2;
        }
        internal float WidthFunction(float completionRatio) => (1f - completionRatio) * Projectile.scale * 9f;

        internal Color ColorFunction(float completionRatio)
        {
            float hue = 0.5f + 0.5f * completionRatio * MathF.Sin(Main.GlobalTimeWrappedHourly * 5f);
            Color trailColor = Main.hslToRgb(hue, 1f, 0.8f);
            return trailColor * Projectile.Opacity;
        }

        public override void PostDraw(Color lightColor)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 使用与 MeowCreature 相同的拖尾渲染逻辑
                PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(WidthFunction, ColorFunction, (_) => Projectile.Size * 0.5f), 30);

                // 绘制弹幕本体的发光效果
                Texture2D glow = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
                Main.EntitySpriteDraw(glow, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, glow.Size() * 0.5f, Projectile.scale, SpriteEffects.None);
            }
            Texture2D glow2 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Main.EntitySpriteDraw(glow2, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, glow2.Size() * 0.5f, Projectile.scale, SpriteEffects.None);
        }

        // 可以去参考一下原版的502号弹幕逻辑（在弹幕文件里面）
        public override bool PreDraw(ref Color lightColor)
        {
            #region 旧代码

            // 取消默认的拖尾绘制逻辑
            // CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);

            //// 使用彩虹色拖尾逻辑进行替换
            //float num105 = Main.DiscoR / 255f; // 获取红色成分并进行归一化
            //float num106 = Main.DiscoG / 255f; // 获取绿色成分并进行归一化
            //float num107 = Main.DiscoB / 255f; // 获取蓝色成分并进行归一化

            //Lighting.AddLight(Projectile.Center, num105, num106, num107);

            //// 绘制更大的拖尾效果，通过遍历弹幕历史位置
            //Texture2D value84 = TextureAssets.Projectile[Projectile.type].Value; // 获取弹幕的纹理
            //Vector2 origin22 = new Vector2(value84.Width / 2, 0f); // 设定纹理的绘制原点
            //Vector2 vector75 = new Vector2(Projectile.width, Projectile.height) / 2f; // 计算弹幕的中心位置
            //Color white3 = Color.White;
            //white3.A = 200; // 设置更高的透明度，保持拖尾更鲜明

            //// 遍历历史位置以生成拖尾
            //for (int num316 = Projectile.oldPos.Length - 1; num316 > 0; num316--)
            //{
            //    Vector2 vector76 = Projectile.oldPos[num316] + vector75; // 计算历史位置的中心
            //    if (!(vector76 == vector75)) // 确保绘制位置正确
            //    {
            //        Vector2 vector77 = Projectile.oldPos[num316 - 1] + vector75; // 获取下一个历史位置
            //                                                                     // 调整旋转角度，增加或减少 90 度
            //        float rotation27 = (vector77 - vector76).ToRotation(); // 原始旋转角度
            //        rotation27 += MathHelper.PiOver2; // 额外旋转 90 度

            //        Vector2 scale7 = new Vector2(2f, Vector2.Distance(vector76, vector77) / value84.Height * 1.5f); // 增大缩放比例，使拖尾更粗更长
            //        Color color82;

            //        // 使用 4 种不同的颜色进行循环变化
            //        switch (num316 % 4)
            //        {
            //            case 0:
            //                color82 = Color.Red;
            //                break;
            //            case 1:
            //                color82 = Color.Green;
            //                break;
            //            case 2:
            //                color82 = Color.Blue;
            //                break;
            //            default:
            //                color82 = Color.Yellow;
            //                break;
            //        }
            //        color82 *= (1f - (float)num316 / Projectile.oldPos.Length); // 逐渐降低透明度，生成渐变效果
            //        Main.EntitySpriteDraw(value84, vector76 - Main.screenPosition, null, color82, rotation27, origin22, scale7, SpriteEffects.None, 0);
            //    }
            //}

            #endregion

            return false; // 不使用默认的绘制逻辑
        }


        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 3; // 允许x次伤害
            Projectile.timeLeft = 600;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true; // 允许与方块碰撞
            Projectile.extraUpdates = 1; // 额外更新次数
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
            Projectile.aiStyle = ProjAIStyleID.Arrow; // 让弹幕受到重力影响
            Projectile.arrow = true;
        }

        public override void AI()
        {
            // 保持弹幕旋转（对于倾斜走向的弹幕而言）
            Projectile.rotation = Projectile.velocity.ToRotation();

            // Lighting - 将光源颜色更改为偏黑的深蓝色，光照强度为 0.55
            Lighting.AddLight(Projectile.Center, new Vector3(0.1f, 0.1f, 0.5f) * 0.55f);

            #region 旧代码

            // 动画处理
            //Projectile.frameCounter++;
            //if (Projectile.frameCounter > 4) // 控制帧切换速度
            //{
            //    Projectile.frame++;
            //    Projectile.frameCounter = 0;
            //}
            //if (Projectile.frame >= Main.projFrames[Projectile.type]) // 循环到第一帧
            //{
            //    Projectile.frame = 0;
            //}

            //    // 检查是否启用了特效
            //    if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            //    {
            //        // 彩虹颜色数组，包含 7 种颜色
            //        Color[] rainbowColors = new Color[]
            //        {
            //Color.Red,
            //Color.Orange,
            //Color.Yellow,
            //Color.Green,
            //Color.Blue,
            //Color.Indigo,
            //Color.Violet
            //        };

            //        // 在飞行路径上生成白色电能粒子特效
            //        int particleCount = 1; // 控制每帧生成的粒子数量
            //        for (int i = 0; i < particleCount; i++)
            //        {
            //            float angle = Main.rand.NextFloat(0, MathHelper.TwoPi); // 随机角度
            //            float distance = Main.rand.NextFloat(2f, 10f); // 控制粒子生成的随机偏移范围
            //            Vector2 spawnPosition = Projectile.Center + distance * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
            //            Vector2 velocity = Vector2.Normalize(spawnPosition - Projectile.Center) * Main.rand.NextFloat(0.5f, 1.5f); // 随机速度

            //            // 随机选择一个彩虹颜色
            //            Color randomRainbowColor = rainbowColors[Main.rand.Next(rainbowColors.Length)];

            //            // 创建电能粒子特效并染色为随机彩虹颜色
            //            Dust dust = Dust.NewDustPerfect(spawnPosition, DustID.Electric, velocity, 150, randomRainbowColor, Main.rand.NextFloat(0.8f, 1.5f));
            //            dust.noGravity = true; // 使粒子不受重力影响
            //            dust.fadeIn = 1f;
            //        }
            //    }

            #endregion

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 计算反弹的方向，遵循入射角等于出射角的原则
            Vector2 reflectDirection = Vector2.Reflect(Projectile.velocity, Vector2.Normalize(target.Center - Projectile.Center));
            Projectile.velocity = reflectDirection;

            // 播放随机音效（Item57 或 Item58）
            SoundStyle[] sounds = { SoundID.Item57, SoundID.Item58 };
            SoundEngine.PlaySound(sounds[Main.rand.Next(sounds.Length)], Projectile.position);

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 生成彩虹电能粒子特效
                GenerateRainbowElectricParticles(Projectile.Center, 15);
            }
        }


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // 碰撞物块时反弹
            if (Projectile.velocity.X != oldVelocity.X) Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y) Projectile.velocity.Y = -oldVelocity.Y;

            // 播放喵喵叫音效
            // 播放随机音效（Item57 或 Item58）
            SoundStyle[] sounds = { SoundID.Item57, SoundID.Item58 };
            SoundEngine.PlaySound(sounds[Main.rand.Next(sounds.Length)], Projectile.position);

            // 每次反弹减少一次穿透次数
            Projectile.penetrate--;

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 生成彩虹电能粒子特效
                GenerateRainbowElectricParticles(Projectile.Center, 5);
            }


            return false;
        }

        public override void OnKill(int timeLeft)
        {
            #region 旧代码

            //// 在弹幕消失时释放深蓝色粒子特效
            //for (int i = 0; i < 15; i++)
            //{
            //    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BlueTorch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 150, default, 2.2f);
            //}

            #endregion
        }

        private void GenerateRainbowElectricParticles(Vector2 position, int particleCount)
        {
            // 彩虹颜色数组，包含 7 种颜色
            Color[] rainbowColors = new Color[]
            {
                Color.Red,
                Color.Orange,
                Color.Yellow,
                Color.Green,
                Color.Blue,
                Color.Indigo,
                Color.Violet
            };

            for (int i = 0; i < particleCount; i++)
            {
                float angle = Main.rand.NextFloat(0, MathHelper.TwoPi); // 随机角度
                float distance = Main.rand.NextFloat(5f, 50f); // 控制粒子生成的随机偏移范围
                Vector2 spawnPosition = position + distance * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                Vector2 velocity = Vector2.Normalize(spawnPosition - position) * Main.rand.NextFloat(1f, 3f); // 随机速度

                // 随机选择一个彩虹颜色
                Color randomRainbowColor = rainbowColors[Main.rand.Next(rainbowColors.Length)];

                // 创建电能粒子特效并染色为随机彩虹颜色
                Dust dust = Dust.NewDustPerfect(spawnPosition, DustID.Electric, velocity, 150, randomRainbowColor, Main.rand.NextFloat(1f, 2f));
                dust.noGravity = true; // 使粒子不受重力影响
                dust.fadeIn = 1f;
            }
        }

    }
}