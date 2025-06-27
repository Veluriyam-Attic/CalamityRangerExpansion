using CalamityMod.Particles;
using CalamityMod.Projectiles.Typeless;
using CalamityMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using CalamityRangerExpansion.CREConfigs;

namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.Pyroblast
{
    internal class PyroblastPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.Pyroblast";
        private static int killCounter = 0; // 计数器


        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        //public override bool PreDraw(ref Color lightColor)
        //{
        //    // 获取 SpriteBatch 和投射物纹理
        //    SpriteBatch spriteBatch = Main.spriteBatch;
        //    Texture2D lightTexture = ModContent.Request<Texture2D>("CalamityRangerExpansion/Content/DeveloperItems/Weapon/Pyroblast/PyroblastPROJ").Value;

        //    // 遍历投射物的旧位置数组，绘制光学拖尾效果
        //    for (int i = 0; i < Projectile.oldPos.Length; i++)
        //    {
        //        // 计算颜色插值值，使颜色在旧位置之间平滑过渡
        //        float colorInterpolation = (float)Math.Cos(Projectile.timeLeft / 32f + Main.GlobalTimeWrappedHourly / 20f + i / (float)Projectile.oldPos.Length * MathHelper.Pi) * 0.5f + 0.5f;

        //        // 使用天蓝色渐变
        //        Color color = Color.Lerp(Color.OrangeRed, Color.Orange, colorInterpolation) * 0.4f;
        //        color.A = 0;

        //        // 计算绘制位置，将位置调整到碰撞箱的中心
        //        Vector2 drawPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

        //        // 计算外部和内部的颜色
        //        Color outerColor = color;
        //        Color innerColor = color * 0.5f;

        //        // 计算强度，使拖尾逐渐变弱
        //        float intensity = 0.9f + 0.15f * (float)Math.Cos(Main.GlobalTimeWrappedHourly % 60f * MathHelper.TwoPi);
        //        intensity *= MathHelper.Lerp(0.15f, 1f, 1f - i / (float)Projectile.oldPos.Length);
        //        if (Projectile.timeLeft <= 60)
        //        {
        //            intensity *= Projectile.timeLeft / 60f; // 如果弹幕即将消失，则拖尾也逐渐消失
        //        }

        //        // 计算外部和内部的缩放比例，使拖尾具有渐变效果
        //        Vector2 outerScale = new Vector2(2f) * intensity;
        //        Vector2 innerScale = new Vector2(2f) * intensity * 0.7f;
        //        outerColor *= intensity;
        //        innerColor *= intensity;

        //        // 绘制外部的拖尾效果，并应用旋转
        //        Main.EntitySpriteDraw(lightTexture, drawPosition, null, outerColor, Projectile.rotation, lightTexture.Size() * 0.5f, outerScale * 0.6f, SpriteEffects.None, 0);

        //        // 绘制内部的拖尾效果，并应用旋转
        //        Main.EntitySpriteDraw(lightTexture, drawPosition, null, innerColor, Projectile.rotation, lightTexture.Size() * 0.5f, innerScale * 0.6f, SpriteEffects.None, 0);
        //    }

        //    // 绘制默认的弹幕，并应用旋转
        //    Main.EntitySpriteDraw(lightTexture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, lightColor, Projectile.rotation, lightTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
        //    return false;
        //}
        public override bool PreDraw(ref Color lightColor)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 画残影效果
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
                return false;
            }
            return true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.MaxUpdates = 4;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;
        }
        public static bool EnableHoming = false; // 控制是否启用追踪逻辑
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;

            // 添加光效
            Lighting.AddLight(Projectile.Center, Color.Lerp(Color.Orange, Color.Red, 0.5f).ToVector3() * 0.55f);

            // 子弹在出现之后很短一段时间会变得可见
            if (Projectile.timeLeft == 296)
                Projectile.alpha = 0;

            // 添加飞行期间的橙色粒子特效
            if (Main.rand.NextBool(3)) // 随机1/3概率生成
            {
                Dust dust = Dust.NewDustPerfect(
                    Projectile.Center,
                    Main.rand.NextBool() ? 130 : 60, // 随机选择两种粒子类型
                    -Projectile.velocity.RotatedByRandom(0.1f) * Main.rand.NextFloat(0.01f, 0.3f) // 轻微随机速度
                );
                dust.noGravity = true; // 粒子无重力
                dust.scale = Main.rand.NextFloat(0.5f, 0.9f); // 随机大小
                if (dust.type == 130)
                    dust.scale = Main.rand.NextFloat(0.35f, 0.55f); // 特殊类型的粒子缩小
            }

            // 初始飞行阶段
            if (Projectile.ai[1] <= 30)
            {
                Projectile.ai[1]++; // 递增计数器
            }
            else
            {
                // 根据是否启用追踪逻辑决定飞行行为
                if (EnableHoming)
                {
                    StrongHoming();
                }
            }

            // 添加光效与粒子
            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3() * 0.8f);
            if (Main.rand.NextBool(3))
            {
                Dust dust = Dust.NewDustPerfect(
                    Projectile.Center,
                    DustID.Torch,
                    Projectile.velocity * -0.5f,
                    100,
                    Color.OrangeRed,
                    1.2f
                );
                dust.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 240); // 添加“烈焰”效果，持续 4 秒
            target.AddBuff(BuffID.Daybreak, 240); // 添加“日曜”效果，持续 4 秒        

            // 抛射橙色粒子特效
            int particleCount = Main.rand.Next(10, 16); // 随机生成 10 到 15 个粒子
            for (int i = 0; i < particleCount; i++)
            {
                Color particleColor = Main.rand.NextBool() ? Color.OrangeRed : Color.Orange; // 随机颜色
                Vector2 velocity = new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-5f, -1f)); // 向上抛射
                PointParticle spark = new PointParticle(Projectile.Center, velocity, false, 10, 0.8f, particleColor);
                GeneralParticleHandler.SpawnParticle(spark);
            }

            // 普通爆炸
            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                Projectile.Center,
                Vector2.Zero,
                ModContent.ProjectileType<FuckYou>(), // 替换为普通爆炸的弹幕类型
                (int)(Projectile.damage * 0.5f),
                Projectile.knockBack,
                Projectile.owner
            );
          
        }


        private void StrongHoming()
        {
            NPC target = Projectile.Center.ClosestNPCAt(1000); // 寻找最近的敌人
            if (target != null)
            {
                Vector2 direction = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * 20f, 0.1f);
            }
        }


        public override void OnKill(int timeLeft)
        {
            // 计数器加 1
            killCounter++;

            // 如果计数器达到 25，释放 SubductionFlameburst 弹幕
            if (killCounter >= 25)
            {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<ScoriaBulletFlameburst>(), // 释放的弹幕
                    (int)(Projectile.damage * 5.0f),                   // 伤害倍率
                    Projectile.knockBack,
                    Projectile.owner
                );
                killCounter = 0; // 重置计数器
            }



        }
    }
}
