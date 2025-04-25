using CalamityMod.Particles;
using CalamityMod;
using CalamityRangerExpansion.Content.Ammunition.CPreMoodLord.PerennialBullet;
using CalamityRangerExpansion.CREConfigs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityRangerExpansion.Content.Ammunition.CPreMoodLord.PlagueBullet
{
    internal class PlagueBulletPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectile.CPreMoodLord";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
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
            Projectile.MaxUpdates = 5;
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
            if (Projectile.timeLeft == 296)
                Projectile.alpha = 0;

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 添加飞行期间的粒子特效
                if (Main.rand.NextBool(3)) // 随机1/3概率生成
                {
                    Dust dust = Dust.NewDustPerfect(
                        Projectile.Center,
                        Main.rand.NextBool() ? 89 : 6, // 改为PlagueTaintedDrone的粒子类型
                        -Projectile.velocity.RotatedByRandom(0.1f) * Main.rand.NextFloat(0.01f, 0.3f) // 随机化速度
                    );
                    dust.noGravity = true; // 粒子无重力
                    dust.scale = Main.rand.NextFloat(0.5f, 0.8f); // 调整随机大小范围
                }
            }
        }

        public override void OnSpawn(IEntitySource source)
        {

        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.HasBuff(ModContent.BuffType<Plague>())) // 检查是否有瘟疫debuff
            {
                //modifiers.FinalDamage *= 1.25f; // 增加伤害
                //modifiers.SourceDamage *= 1.25f; // 增加伤害
                //modifiers.ScalingBonusDamage += 4f; // 额外增加 4 点伤害
                //modifiers.FlatBonusDamage += 10f; // 固定增加 10 点伤害
                //modifiers.TargetDamageMultiplier *= 1.25f; // 让目标承受 25% 额外伤害
                //modifiers.DamageVariationScale = 0f; // 让伤害不浮动
                modifiers.CritDamage *= 1.25f; // 让暴击额外增加 25% 伤害
            }
            else
            {
                //modifiers.FinalDamage *= 0.75f; // 减少伤害
                //modifiers.SourceDamage *= 0.75f; // 增加伤害
                modifiers.CritDamage *= 0.75f;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 判断敌人是否有瘟疫效果，并调整特效参数
                int particleCount = target.HasBuff(ModContent.BuffType<Plague>()) ? 15 : 5; // 瘟疫时特效更华丽
                float baseLength = 4 * 16f; // 基础弧线长度
                float arcLength = target.HasBuff(ModContent.BuffType<Plague>()) ? baseLength * 1.5f : baseLength; // 根据瘟疫效果调整长度
                float velocityMultiplier = target.HasBuff(ModContent.BuffType<Plague>()) ? 4.5f : 1.5f;

                // 每两条弧线之间的夹角为 120 度
                for (int arcIndex = 0; arcIndex < 3; arcIndex++)
                {
                    float baseAngle = MathHelper.TwoPi / 3 * arcIndex; // 每条弧线的起始角度

                    // 生成弧线粒子
                    for (int i = 0; i < particleCount; i++)
                    {
                        // 弧线内粒子的角度分布
                        float angle = baseAngle + (-MathF.PI / 6f) + (MathF.PI / 6f) * (i / (float)(particleCount - 1));
                        Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)).SafeNormalize(Vector2.Zero);

                        // 粒子生成位置
                        Vector2 position = Projectile.Center + direction * (arcLength / particleCount * i);

                        // 创建粒子特效
                        Dust plague = Dust.NewDustDirect(
                            position,
                            0, 0, // 粒子生成范围为 1×1 像素
                            DustID.TerraBlade // 粒子特效编号
                        );
                        plague.velocity = direction * velocityMultiplier + Main.rand.NextVector2Circular(2f, 2f); // 粒子速度
                        plague.color = Color.DarkGreen; // 粒子颜色
                        plague.noGravity = true; // 无重力
                        plague.scale = Main.rand.NextFloat(1f, 1.5f); // 粒子大小
                    }
                }
            }
        }


        public override void OnKill(int timeLeft)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 1. 生成五边形的 DirectionalPulseRing 特效
                float radius = 2 * 16f; // 五边形距离中心点的半径
                int pulseCount = 5; // 五边形的顶点数量
                float baseAngle = Main.rand.NextFloat(0, MathF.PI * 2); // 随机生成基准角度

                for (int i = 0; i < pulseCount; i++)
                {
                    // 计算每个顶点的位置
                    float angle = baseAngle + MathF.PI * 2 / pulseCount * i; // 每个顶点的角度，加上基准角度
                    Vector2 position = Projectile.Center + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius;

                    // 创建 DirectionalPulseRing
                    float pulseScale = Main.rand.NextFloat(0.3f, 0.4f);
                    DirectionalPulseRing pulse = new DirectionalPulseRing(
                        position,
                        new Vector2(2, 2).RotatedByRandom(MathF.PI * 2) * Main.rand.NextFloat(0.2f, 1.1f),
                        (Main.rand.NextBool(3) ? Color.LimeGreen : Color.Green) * 0.8f,
                        new Vector2(1, 1),
                        pulseScale - 0.25f,
                        pulseScale,
                        0f,
                        15
                    );
                    GeneralParticleHandler.SpawnParticle(pulse);
                }
            }
        }
    }
}
