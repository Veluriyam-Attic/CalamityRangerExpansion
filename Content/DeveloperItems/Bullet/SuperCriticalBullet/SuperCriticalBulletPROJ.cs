//using CalamityMod.Particles;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Terraria.DataStructures;
//using Terraria.ID;
//using Terraria.ModLoader;
//using Terraria;

//namespace CalamityRangerExpansion.Content.DeveloperItems.Bullet.SuperCriticalBullet
//{
//    public class SuperCriticalBulletPROJ : ModProjectile, ILocalizedModType
//    {
//        public new string LocalizationCategory => "DeveloperItems.SuperCriticalBullet";

//        private NPC currentTarget = null; // Current target
//        private float damageMultiplier = 1.0f; // Damage multiplier after each bounce
//        public override void SetStaticDefaults()
//        {
//            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
//            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
//        }
//        public override bool PreDraw(ref Color lightColor)
//        {
//            // 获取 SpriteBatch 和投射物纹理
//            SpriteBatch spriteBatch = Main.spriteBatch;
//            Texture2D lightTexture = ModContent.Request<Texture2D>("CalamityRangerExpansion/Content/DeveloperItems/Bullet/SuperCriticalBullet/SuperCriticalBulletPROJ").Value;

//            // 遍历投射物的旧位置数组，绘制光学拖尾效果
//            for (int i = 0; i < Projectile.oldPos.Length; i++)
//            {
//                // 计算颜色插值值，使颜色在旧位置之间平滑过渡
//                float colorInterpolation = (float)Math.Cos(Projectile.timeLeft / 32f + Main.GlobalTimeWrappedHourly / 20f + i / (float)Projectile.oldPos.Length * MathHelper.Pi) * 0.5f + 0.5f;

//                // 使用天蓝色渐变
//                Color color = Color.Lerp(Color.Black, Color.DarkGray, colorInterpolation) * 0.4f;
//                color.A = 0;

//                // 计算绘制位置，将位置调整到碰撞箱的中心
//                Vector2 drawPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

//                // 计算外部和内部的颜色
//                Color outerColor = color;
//                Color innerColor = color * 0.5f;

//                // 计算强度，使拖尾逐渐变弱
//                float intensity = 0.9f + 0.15f * (float)Math.Cos(Main.GlobalTimeWrappedHourly % 60f * MathHelper.TwoPi);
//                intensity *= MathHelper.Lerp(0.15f, 1f, 1f - i / (float)Projectile.oldPos.Length);
//                if (Projectile.timeLeft <= 60)
//                {
//                    intensity *= Projectile.timeLeft / 60f; // 如果弹幕即将消失，则拖尾也逐渐消失
//                }

//                // 计算外部和内部的缩放比例，使拖尾具有渐变效果
//                Vector2 outerScale = new Vector2(2f) * intensity;
//                Vector2 innerScale = new Vector2(2f) * intensity * 0.7f;
//                outerColor *= intensity;
//                innerColor *= intensity;

//                // 绘制外部的拖尾效果，并应用旋转
//                Main.EntitySpriteDraw(lightTexture, drawPosition, null, outerColor, Projectile.rotation, lightTexture.Size() * 0.5f, outerScale * 0.6f, SpriteEffects.None, 0);

//                // 绘制内部的拖尾效果，并应用旋转
//                Main.EntitySpriteDraw(lightTexture, drawPosition, null, innerColor, Projectile.rotation, lightTexture.Size() * 0.5f, innerScale * 0.6f, SpriteEffects.None, 0);
//            }

//            // 绘制默认的弹幕，并应用旋转
//            Main.EntitySpriteDraw(lightTexture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, lightColor, Projectile.rotation, lightTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
//            return false;
//        }
//        public override void SetDefaults()
//        {
//            Projectile.width = 4;
//            Projectile.height = 4;
//            Projectile.friendly = true;
//            Projectile.DamageType = DamageClass.Ranged;
//            Projectile.tileCollide = true;
//            Projectile.ignoreWater = true;
//            Projectile.penetrate = -1;
//            Projectile.timeLeft = 1200;
//            Projectile.MaxUpdates = 6;
//            Projectile.alpha = 255;
//            Projectile.usesLocalNPCImmunity = true;
//            Projectile.localNPCHitCooldown = 2;
//        }

//        public override void AI()
//        {
//            // 由于我们是水平贴图，因此什么也不需要转动
//            Projectile.rotation = Projectile.velocity.ToRotation();

//            // 添加光效
//            Lighting.AddLight(Projectile.Center, Color.Lerp(Color.Black, Color.Black, 0.5f).ToVector3() * 0.49f);

//            // 子弹在出现之后很短一段时间会变得可见
//            if (Projectile.timeLeft == 1118)
//                Projectile.alpha = 0;

//            // 添加飞行粒子特效
//            if (Main.rand.NextBool(3)) // 1/3 概率生成粒子
//            {
//                Dust dust = Dust.NewDustPerfect(
//                    Projectile.Center,
//                    Main.rand.NextBool() ? DustID.ShadowbeamStaff : DustID.Smoke, // 使用黑色主题的 Dust
//                    -Projectile.velocity.RotatedByRandom(0.1f) * Main.rand.NextFloat(0.1f, 0.3f)
//                );
//                dust.noGravity = true; // 无重力
//                dust.scale = Main.rand.NextFloat(0.7f, 1.1f); // 随机大小
//            }


//        }

//        public override void OnSpawn(IEntitySource source)
//        {

//        }
//        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
//        {
//            if (Main.rand.NextBool(10000)) // 1/10000 概率
//            {
//                // 造成 100 倍伤害
//                modifiers.FinalDamage *= 100;

//                // 释放黑色冲击波特效
//                Particle pulse = new DirectionalPulseRing(
//                    Projectile.Center,
//                    Vector2.Zero, // 冲击波无速度
//                    Color.Black, // 使用深黑色
//                    new Vector2(1.5f, 3.5f), // 扩散尺寸
//                    Projectile.rotation,
//                    0.3f, // 效果渐隐速度
//                    0.05f,
//                    30
//                );
//                GeneralParticleHandler.SpawnParticle(pulse);
//            }
//        }

//        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
//        {
//            int particleCount = Main.rand.Next(3, 6); // 随机生成 3~5 个粒子
//            for (int i = 0; i < particleCount; i++)
//            {
//                Vector2 direction = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(15)) * Main.rand.NextFloat(0.5f, 1.5f); // 稍微随机偏移

//                // 生成黑色粒子
//                Dust dust = Dust.NewDustPerfect(
//                    Projectile.Center,
//                    DustID.ShadowbeamStaff, // 黑色主题粒子
//                    direction,
//                    0, // 无透明度
//                    Color.Black,
//                    Main.rand.NextFloat(1f, 1.5f) // 随机缩放
//                );
//                dust.noGravity = true; // 无重力
//            }
//        }

//        public override void OnKill(int timeLeft)
//        {
           
//        }
//    }
//}
