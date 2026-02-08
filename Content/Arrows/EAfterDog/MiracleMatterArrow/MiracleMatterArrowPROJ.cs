#region using 太多了
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Ranged;
using Terraria.DataStructures;
using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Particles;
using CalamityRangerExpansion.CREConfigs;
using Terraria.Audio;
using CalamityMod.Graphics.Primitives;
using Terraria.Graphics.Shaders;
using CalamityMod.Projectiles.Rogue;
#endregion

namespace CalamityRangerExpansion.Content.Arrows.EAfterDog.MiracleMatterArrow
{
    public class MiracleMatterArrowPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectile.EAfterDog";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            // 设置弹幕的基础属性
            Projectile.width = Projectile.height = 160; // 弹幕宽高
            Projectile.friendly = true; // 对敌人有效
            Projectile.DamageType = DamageClass.Ranged; // 远程伤害类型
            Projectile.penetrate = 1; // 穿透力为1，击中一个敌人就消失
            Projectile.timeLeft = 600; // 弹幕存在时间为600帧
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
            Projectile.ignoreWater = true; // 弹幕不受水影响
            Projectile.arrow = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            // 调整旋转
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            #region 旧代码

            //// 计算正前方三格（48个像素）的位置
            //Vector2 forwardPosition = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 48f;

            //// 检测该位置是否存在敌人
            //foreach (NPC npc in Main.npc)
            //{
            //    if (npc.active && !npc.friendly && npc.CanBeChasedBy(this) && npc.Hitbox.Contains(forwardPosition.ToPoint()))
            //    {
            //        // 如果正前方三格处有敌人，销毁自身
            //        Projectile.Kill();
            //        return; // 确保方法中止，避免执行后续代码
            //    }
            //}

            #endregion

            // 检测范围，以当前弹幕为圆心，半径为 5 × 16 像素（80 像素）
            float detectionRadius = 80f;

            // 遍历所有敌人
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && !npc.friendly && npc.CanBeChasedBy(this))
                {
                    // 计算敌人与弹幕的距离
                    float distanceToNPC = Vector2.Distance(Projectile.Center, npc.Center);

                    // 如果敌人在检测范围内，销毁弹幕
                    if (distanceToNPC <= detectionRadius)
                    {
                        //Projectile.Kill(); // 销毁弹幕（不再有这个效果，而是通过直接扩大碰撞箱的逻辑来实现）
                        return; // 终止后续逻辑
                    }
                }
            }

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                Vector2 armPosition = Projectile.Center;
                Vector2 tipPosition = armPosition + Projectile.velocity * Projectile.width * 0.45f;

                // 发光效果
                Color energyColor = Color.Orange;
                Vector2 verticalOffset = Vector2.UnitY.RotatedBy(Projectile.rotation) * 8f;
                if (Math.Cos(Projectile.rotation) < 0f)
                    verticalOffset *= -1f;

                // 发射橙色光粒子
                if (Main.rand.NextBool(4))
                {
                    SquishyLightParticle exoEnergy = new(tipPosition + verticalOffset, -Vector2.UnitY.RotatedByRandom(0.39f) * Main.rand.NextFloat(0.4f, 1.6f), 0.28f, energyColor, 25);
                    GeneralParticleHandler.SpawnParticle(exoEnergy);
                }

                // 增加透明度渐变
                Projectile.Opacity = Utils.GetLerpValue(0f, 3f, Projectile.timeLeft, true);

                // 添加光照
                DelegateMethods.v3_1 = energyColor.ToVector3();
                Utils.PlotTileLine(tipPosition - verticalOffset, tipPosition + verticalOffset, 10f, DelegateMethods.CastLightOpen);
                Lighting.AddLight(tipPosition, energyColor.ToVector3());

                #region 旧代码

                //// 添加从暗红色到亮白色的粒子效果
                //if (Main.rand.NextBool(5))
                //{
                //    // 颜色渐变：从深红色逐渐变成亮白色
                //    float brightness = (float)Main.rand.NextDouble(); // 随机亮度值
                //    Color smokeColor = Color.Lerp(Color.GhostWhite, Color.White, brightness); // 插值计算
                //    Particle smoke = new HeavySmokeParticle(Projectile.Center, Projectile.velocity * 0.5f, smokeColor, 20, Projectile.scale * Main.rand.NextFloat(0.6f, 1.2f), 0.8f, MathHelper.ToRadians(3f), required: true);
                //    GeneralParticleHandler.SpawnParticle(smoke);
                //}

                #endregion

                // 充能完成时的特效
                if (Projectile.timeLeft == 550) // 示例触发条件
                {
                    #region 旧代码
                    //SoundEngine.PlaySound(SoundID.Item158 with { Volume = 1.6f }, Projectile.Center);
                    //for (int i = 0; i < 75; i++)
                    //{
                    //    float offsetAngle = MathHelper.TwoPi * i / 75f;
                    //    float unitOffsetX = (float)Math.Pow(Math.Cos(offsetAngle), 3D);
                    //    float unitOffsetY = (float)Math.Pow(Math.Sin(offsetAngle), 3D);

                    //    Vector2 puffDustVelocity = new Vector2(unitOffsetX, unitOffsetY) * 5f;
                    //    Dust magic = Dust.NewDustPerfect(tipPosition, 267, puffDustVelocity);
                    //    magic.scale = 1.8f;
                    //    magic.fadeIn = 0.5f;
                    //    magic.color = CalamityUtils.MulticolorLerp(i / 75f, CalamityUtils.ExoPalette);
                    //    magic.noGravity = true;
                    //}

                    #endregion
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 获取纹理资源和位置
                Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
                Texture2D textureGlow = ModContent.Request<Texture2D>("CalamityRangerExpansion/Content/Arrows/EAfterDog/MiracleMatterArrow/MiracleMatterArrowPROJ").Value;
                Vector2 origin = texture.Size() * 0.5f;
                Vector2 drawPosition = Projectile.Center - Main.screenPosition;

                // 计算颜色渐变，用于动态的尾迹效果
                float localIdentityOffset = Projectile.identity * 0.1372f;
                Color mainColor = CalamityUtils.MulticolorLerp((Main.GlobalTimeWrappedHourly * 2f + localIdentityOffset) % 1f, Color.Cyan, Color.Lime, Color.GreenYellow, Color.Goldenrod, Color.Orange);
                Color secondaryColor = CalamityUtils.MulticolorLerp((Main.GlobalTimeWrappedHourly * 2f + localIdentityOffset + 0.2f) % 1f, Color.Cyan, Color.Lime, Color.GreenYellow, Color.Goldenrod, Color.Orange);

                // 混合颜色，进一步控制白色和渐变色的比例
                mainColor = Color.Lerp(Color.White, mainColor, 0.85f);
                secondaryColor = Color.Lerp(Color.White, secondaryColor, 0.85f);

                // 背光效果部分，增加充能效果的光晕
                float chargeOffset = 3f; // 控制充能效果扩散的偏移量
                Color chargeColor = Color.Lerp(Color.Lime, Color.Cyan, (float)Math.Cos(Main.GlobalTimeWrappedHourly * 7.1f) * 0.5f + 0.5f) * 0.6f;
                chargeColor.A = 0; // 透明度

                // 设置固定的旋转角度，使弹幕始终保持一个方向
                Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
                float rotation = Projectile.rotation; // 使用Projectile.rotation作为旋转值

                // 去除原有的方向翻转逻辑
                SpriteEffects direction = SpriteEffects.None; // 保持默认值，不进行水平翻转

                // 绘制充能效果 - 圆周上绘制多个充能光效
                for (int i = 0; i < 8; i++)
                {
                    Vector2 drawOffset = (MathHelper.TwoPi * i / 8f).ToRotationVector2() * chargeOffset;
                    Main.spriteBatch.Draw(texture, drawPosition + drawOffset, null, chargeColor, rotation, origin, Projectile.scale, direction, 0f);
                }

                // 使用自定义着色器应用尾迹效果
                GameShaders.Misc["CalamityMod:HeavenlyGaleTrail"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/EternityStreak"));
                GameShaders.Misc["CalamityMod:HeavenlyGaleTrail"].UseImage2("Images/Extra_189");
                GameShaders.Misc["CalamityMod:HeavenlyGaleTrail"].UseColor(mainColor);
                GameShaders.Misc["CalamityMod:HeavenlyGaleTrail"].UseSecondaryColor(secondaryColor);
                GameShaders.Misc["CalamityMod:HeavenlyGaleTrail"].Apply();

                // 渲染尾迹，通过存储的弹幕位置数据渲染弹幕移动的尾巴
                PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(PrimitiveWidthFunction, PrimitiveColorFunction, (_, _) => Projectile.Size * 0.5f, shader: GameShaders.Misc["CalamityMod:HeavenlyGaleTrail"]), 53);

                // 渲染实际的投射物本体
                Main.spriteBatch.Draw(texture, drawPosition, null, Projectile.GetAlpha(lightColor), rotation, origin, Projectile.scale, direction, 0f);

                // 渲染发光的投射物效果
                Main.spriteBatch.Draw(textureGlow, drawPosition, null, Color.White, rotation, origin, Projectile.scale, direction, 0f);

                return false;
            }
            else
            {
                return true;
            }
        }


        public float PrimitiveWidthFunction(float completionRatio, Vector2 vexpos) => Projectile.scale * 30f;

        public Color PrimitiveColorFunction(float _, Vector2 vexpos) => Color.Lime * Projectile.Opacity; // 在方法传入参数中 _ 并不是丢弃，而是一个合法的参数名称

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnKill(int timeLeft)
        {
            #region 旧代码

            //// 计算随机初始角度
            //float initialAngle = Main.rand.NextFloat(0f, MathHelper.TwoPi); // 在 0 到 2π 之间随机选择一个角度
            //float angleIncrement = MathHelper.ToRadians(120f); // 每个角度之间的夹角为 120 度

            //// 计算并发射三个弹幕
            //for (int i = 0; i < 3; i++)
            //{
            //    float currentAngle = initialAngle + i * angleIncrement; // 计算当前的角度
            //    Vector2 newVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(currentAngle) * 10f; // 旋转基础速度并保持大小
            //    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, newVelocity, ModContent.ProjectileType<MiracleMatterArrowFire>(), (int)(Projectile.damage * 1.0f), Projectile.knockBack, Projectile.owner);
            //}

            #endregion

            // 计算并发射三个弹幕
            {
                // 计算初始方向角度
                float forwardAngle = Projectile.velocity.ToRotation(); // 当前速度的方向角
                float backwardAngle = forwardAngle + MathHelper.Pi; // 相反方向的角度

                // 定义前后角度范围（±15度）
                float angleRange = MathHelper.ToRadians(15f);

                // 存储所有可能的角度
                List<float> possibleAngles = new List<float>
                {
                    forwardAngle - angleRange, // 正前方左15度
                    forwardAngle + angleRange, // 正前方右15度
                    backwardAngle - angleRange, // 正后方左15度
                    backwardAngle + angleRange  // 正后方右15度
                };

                // 随机选择三个角度
                List<float> selectedAngles = possibleAngles.OrderBy(_ => Main.rand.Next()).Take(3).ToList();

                // 发射三个弹幕
                foreach (float angle in selectedAngles)
                {
                    // 计算新速度（当前速度的1/4）
                    //Vector2 newVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(angle) * (Projectile.velocity.Length() / 4f);

                    // 设置固定初始速度（长度为 xf）
                    Vector2 newVelocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 1.4f;

                    if (Main.zenithWorld)
                    {
                        // 直接发射超新星爆炸
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, newVelocity, ModContent.ProjectileType<SupernovaStealthBoom>(), (int)(Projectile.damage * 20f), Projectile.knockBack, Projectile.owner);
                    }
                    else
                    {
                        // 发射天堂之火
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, newVelocity, ModContent.ProjectileType<MiracleMatterArrowFire>(), (int)(Projectile.damage * 1.3f), Projectile.knockBack, Projectile.owner);
                    }

                }
            }




            Vector2 armPosition = Projectile.Center;
            Vector2 tipPosition = armPosition + Projectile.velocity * Projectile.width * 0.45f;

            // 释放原有的菱形粒子效果
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                SoundEngine.PlaySound(SoundID.Item158 with { Volume = 1.6f }, Projectile.Center);

                #region 旧代码

                //for (int i = 0; i < 75; i++)
                //{
                //    float offsetAngle = MathHelper.TwoPi * i / 75f;
                //    float unitOffsetX = (float)Math.Pow(Math.Cos(offsetAngle), 3D);
                //    float unitOffsetY = (float)Math.Pow(Math.Sin(offsetAngle), 3D);

                //    Vector2 puffDustVelocity = new Vector2(unitOffsetX, unitOffsetY) * 5f;
                //    Dust magic = Dust.NewDustPerfect(tipPosition, 267, puffDustVelocity);
                //    magic.scale = 1.8f;
                //    magic.fadeIn = 0.5f;
                //    magic.color = CalamityUtils.MulticolorLerp(i / 75f, CalamityUtils.ExoPalette);
                //    magic.noGravity = true;
                //}





                //Vector2 frontCenter = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 48f; // 正前方三个方块处
                //Vector2 backCenter = Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.Zero) * 96f;  // 正后方六个方块处（间隔3个方块）

                //int halfCircleParticles = 38; // 每个半圆生成的粒子数量
                //float particleSpeed = 6f; // 粒子速度增量
                //float radius = 24f; // 半圆半径增大，形状更夸张

                //for (int i = 0; i < halfCircleParticles; i++)
                //{
                //    // 计算角度范围 (-90 度到 90 度)，并根据弹幕朝向旋转
                //    float angle = MathHelper.Pi * (i / (float)(halfCircleParticles - 1)) - MathHelper.PiOver2;
                //    Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero); // 弹幕朝向

                //    // 计算正前方半圆的粒子
                //    Vector2 frontOffset = Vector2.Transform(new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius,
                //                                            Matrix.CreateRotationZ(direction.ToRotation()));
                //    Dust frontMagic = Dust.NewDustPerfect(frontCenter + frontOffset, 267, frontOffset.SafeNormalize(Vector2.Zero) * particleSpeed);
                //    frontMagic.scale = 2.2f; // 增大粒子缩放
                //    frontMagic.fadeIn = 0.5f;
                //    frontMagic.color = CalamityUtils.MulticolorLerp(i / (float)halfCircleParticles, CalamityUtils.ExoPalette);
                //    frontMagic.noGravity = true;

                //    // 计算正后方半圆的粒子（反转角度）
                //    float reversedAngle = -angle; // 反转角度
                //    Vector2 backOffset = Vector2.Transform(new Vector2((float)Math.Cos(reversedAngle), (float)Math.Sin(reversedAngle)) * radius,
                //                                           Matrix.CreateRotationZ(direction.ToRotation()));
                //    Dust backMagic = Dust.NewDustPerfect(backCenter + backOffset, 267, backOffset.SafeNormalize(Vector2.Zero) * particleSpeed);
                //    backMagic.scale = 2.2f; // 增大粒子缩放
                //    backMagic.fadeIn = 0.5f;
                //    backMagic.color = CalamityUtils.MulticolorLerp(i / (float)halfCircleParticles, CalamityUtils.ExoPalette);
                //    backMagic.noGravity = true;
                //}

                #endregion

                Vector2 center = Projectile.Center; // 粒子生成的中心点
                int totalParticles = 76; // 圆形粒子的总数量（可以调整密度）
                float particleSpeed = 6f; // 粒子速度增量
                float radius = 24f; // 圆形半径

                for (int i = 0; i < totalParticles; i++)
                {
                    // 计算每个粒子的角度（0 到 2 * PI，即 360°）
                    float angle = MathHelper.TwoPi * (i / (float)totalParticles);
                    Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius; // 偏移量

                    // 生成粒子
                    Dust magicDust = Dust.NewDustPerfect(center + offset, 267, offset.SafeNormalize(Vector2.Zero) * particleSpeed);
                    magicDust.scale = 2.2f; // 增大粒子缩放
                    magicDust.fadeIn = 0.5f;
                    magicDust.color = CalamityUtils.MulticolorLerp(i / (float)totalParticles, CalamityUtils.ExoPalette); // 色彩过渡
                    magicDust.noGravity = true; // 取消重力效果
                }
            }
        }
    }
}