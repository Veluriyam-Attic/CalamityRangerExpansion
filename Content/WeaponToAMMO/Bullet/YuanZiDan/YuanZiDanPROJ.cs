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
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using CalamityRangerExpansion.CREConfigs;

namespace CalamityRangerExpansion.Content.WeaponToAMMO.Bullet.YuanZiDan
{
    public class YuanZiDanPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "WeaponToAMMO.Bullet.YuanZiDan";

        private NPC currentTarget = null; // Current target
        private float damageMultiplier = 1.0f; // Damage multiplier after each bounce
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 获取 SpriteBatch 和投射物纹理
                SpriteBatch spriteBatch = Main.spriteBatch;
                Texture2D lightTexture = ModContent.Request<Texture2D>("CalamityRangerExpansion/Content/WeaponToAMMO/Bullet/YuanZiDan/YuanZiDanPROJ").Value;

                // 遍历投射物的旧位置数组，绘制光学拖尾效果
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    // 计算颜色插值值，使颜色在旧位置之间平滑过渡
                    float colorInterpolation = (float)Math.Cos(Projectile.timeLeft / 32f + Main.GlobalTimeWrappedHourly / 20f + i / (float)Projectile.oldPos.Length * MathHelper.Pi) * 0.5f + 0.5f;

                    // 使用天蓝色渐变
                    Color color = Color.Lerp(Color.IndianRed, Color.Red, colorInterpolation) * 0.4f;
                    color.A = 0;

                    // 计算绘制位置，将位置调整到碰撞箱的中心
                    Vector2 drawPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

                    // 计算外部和内部的颜色
                    Color outerColor = color;
                    Color innerColor = color * 0.5f;

                    // 计算强度，使拖尾逐渐变弱
                    float intensity = 0.9f + 0.15f * (float)Math.Cos(Main.GlobalTimeWrappedHourly % 60f * MathHelper.TwoPi);
                    intensity *= MathHelper.Lerp(0.15f, 1f, 1f - i / (float)Projectile.oldPos.Length);
                    if (Projectile.timeLeft <= 60)
                    {
                        intensity *= Projectile.timeLeft / 60f; // 如果弹幕即将消失，则拖尾也逐渐消失
                    }

                    // 计算外部和内部的缩放比例，使拖尾具有渐变效果
                    Vector2 outerScale = new Vector2(2f) * intensity;
                    Vector2 innerScale = new Vector2(2f) * intensity * 0.7f;
                    outerColor *= intensity;
                    innerColor *= intensity;

                    // 绘制外部的拖尾效果，并应用旋转
                    Main.EntitySpriteDraw(lightTexture, drawPosition, null, outerColor, Projectile.rotation, lightTexture.Size() * 0.5f, outerScale * 0.6f, SpriteEffects.None, 0);

                    // 绘制内部的拖尾效果，并应用旋转
                    Main.EntitySpriteDraw(lightTexture, drawPosition, null, innerColor, Projectile.rotation, lightTexture.Size() * 0.5f, innerScale * 0.6f, SpriteEffects.None, 0);
                }

                // 绘制默认的弹幕，并应用旋转
                Main.EntitySpriteDraw(lightTexture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, lightColor, Projectile.rotation, lightTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
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
            Projectile.penetrate = 2;
            Projectile.timeLeft = 450;
            Projectile.MaxUpdates = 6;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;
        }

        public override void AI()
        {
            // 由于我们是水平贴图，因此什么也不需要转动
            Projectile.rotation = Projectile.velocity.ToRotation();

            // 添加光效
            Lighting.AddLight(Projectile.Center, Color.Lerp(Color.Blue, Color.AliceBlue, 0.5f).ToVector3() * 0.49f);

            // 子弹在出现之后很短一段时间会变得可见
            if (Projectile.timeLeft == 445)
                Projectile.alpha = 0;

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // Add flying particles
                if (Main.rand.NextBool(3)) // Randomly 1/3 chance
                {
                    Dust dust = Dust.NewDustPerfect(
                        Projectile.Center,
                        Main.rand.NextBool() ? DustID.SomethingRed : DustID.CrimsonTorch,
                        -Projectile.velocity.RotatedByRandom(0.1f) * Main.rand.NextFloat(0.01f, 0.3f)
                    );
                    dust.noGravity = true; // Particle with no gravity
                    dust.scale = Main.rand.NextFloat(0.5f, 0.9f); // Random size
                }

            }

            // Homing logic if target exists
            if (currentTarget != null && currentTarget.active)
            {
                Vector2 direction = currentTarget.Center - Projectile.Center;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction.SafeNormalize(Vector2.Zero) * Projectile.velocity.Length(), 0.1f);
            }
        }

        public override void OnSpawn(IEntitySource source)
        {

        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // 找到距离最近的敌人
            NPC closestTarget = FindClosestTarget(target);

            if (closestTarget != null)
            {
                // 计算新的方向
                Vector2 newDirection = (closestTarget.Center - Projectile.Center).SafeNormalize(Vector2.Zero);

                // 保持原始速度，但调整方向
                Projectile.velocity = newDirection * Projectile.velocity.Length();

                // 播放音效
                SoundEngine.PlaySound(SoundID.Item115, Projectile.position);

                // 检查是否启用了特效
                if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
                {
                    // 生成收缩的白色圆圈特效
                    Particle pulse = new DirectionalPulseRing(
                        Projectile.Center,
                        Projectile.velocity * 0.75f,
                        Color.IndianRed,
                        new Vector2(1f, 2.5f),
                        Projectile.rotation,
                        0.2f,
                        0.03f,
                        20
                    );
                    GeneralParticleHandler.SpawnParticle(pulse);
                }



                // 提升x%的伤害
                Projectile.damage = Main.getGoodWorld
                    ? (int)(Projectile.damage * 1.75f) // getGoodWorld 启用时，伤害乘以 X
                    : (int)(Projectile.damage * 1.15f); // 否则，伤害乘以 x
            }
        }

        // 寻找最近的敌人（排除当前目标）
        private NPC FindClosestTarget(NPC excludeTarget)
        {
            NPC closest = null;
            float minDistance = float.MaxValue;

            foreach (NPC npc in Main.npc)
            {
                if (npc.active && !npc.friendly && npc.lifeMax > 5 && npc != excludeTarget)
                {
                    float distance = Vector2.Distance(Projectile.Center, npc.Center);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closest = npc;
                    }
                }
            }

            return closest;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Find the next closest target
            //currentTarget = FindClosestTarget();

            // Play sound effect
            SoundEngine.PlaySound(SoundID.Item115, Projectile.position);

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // Create shrinking white circle effect
                Particle pulse = new DirectionalPulseRing(Projectile.Center, Projectile.velocity * 0.75f, Color.IndianRed, new Vector2(1f, 2.5f), Projectile.rotation, 0.2f, 0.03f, 20);
                GeneralParticleHandler.SpawnParticle(pulse);
            }



            // Increase damage by 20%
            //damageMultiplier *= 1.2f;
            //Projectile.damage = (int)(Projectile.damage * 1.2f);
        }

        public override void OnKill(int timeLeft)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                int particleCount = Main.rand.Next(5, 11); // 随机生成5到10个粒子

                for (int i = 0; i < particleCount; i++)
                {
                    // 随机选择粒子类型：Electrict 或 HallowSpray
                    int dustType = Main.rand.NextBool() ? DustID.SomethingRed : DustID.CrimsonTorch;

                    // 计算抛射方向，基于投射物当前方向并加上随机角度偏移
                    Vector2 velocity = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(45))
                                       * Main.rand.NextFloat(1f, 3f); // 随机速度范围

                    // 创建粒子
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, dustType, velocity, 0, default, 1.5f);
                    dust.noGravity = true; // 粒子不受重力影响
                    dust.scale = Main.rand.NextFloat(1f, 1.5f); // 粒子大小随机
                }
            }

        }

    }
}
