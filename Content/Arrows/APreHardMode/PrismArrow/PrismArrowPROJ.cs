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
using CalamityRangerExpansion.CREConfigs;
using Terraria.DataStructures;
using Terraria.WorldBuilding;
using CalamityMod.Particles;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;

namespace CalamityRangerExpansion.Content.Arrows.APreHardMode.PrismArrow
{
    internal class PrismArrowPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectile.APreHardMode";
        public override string Texture => "CalamityRangerExpansion/Content/Arrows/APreHardMode/PrismArrow/PrismArrow";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
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
            // 设置弹幕的基础属性
            Projectile.width = 11; // 弹幕宽度
            Projectile.height = 24; // 弹幕高度
            Projectile.friendly = true; // 对敌人有效
            Projectile.DamageType = DamageClass.Ranged; // 远程伤害类型
            Projectile.penetrate = 1; // 穿透力为1，击中一个敌人就消失
            Projectile.timeLeft = 300; // 弹幕存在时间为x帧
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
            Projectile.ignoreWater = false; // 弹幕不受水影响
            Projectile.arrow = true;
            Projectile.extraUpdates = 0;
            // 检测是否在液体中，并调整 aiStyle
            if (Projectile.wet) // 在水中时
            {
                Projectile.aiStyle = -1; // 不会重力影响
            }
            else // 不在水中
            {
                Projectile.aiStyle = ProjAIStyleID.Arrow; // 受到重力影响
            }
        }
        public override void OnSpawn(IEntitySource source)
        {
            // 检测是否在液体中，并调整 aiStyle
            if (Projectile.wet) // 在水中时
            {
                Projectile.aiStyle = -1; // 不会重力影响
            }
            else // 不在水中
            {
                Projectile.aiStyle = ProjAIStyleID.Arrow; // 受到重力影响
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.wet)
            {
                // 如果在水中，造成 1.45 倍伤害
                modifiers.SourceDamage *= 1.45f;
            }
            else
            {
                // 不在水中，造成 1 倍伤害
                modifiers.SourceDamage *= 1f;
            }
        }

        public override void OnKill(int timeLeft)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                if (Projectile.wet)
                {
                    // 如果在水中，生成五边形扩散粒子
                    for (int i = 0; i < 25; i++)
                    {
                        float angle = MathHelper.TwoPi / 5 * i; // 计算五边形的角度
                        Vector2 particleVelocity = angle.ToRotationVector2() * Main.rand.NextFloat(0.5f, 1.5f);

                        // 创建粒子特效
                        Dust waterDust = Dust.NewDustPerfect(Projectile.Center, DustID.Water, particleVelocity, 0, Color.LightSkyBlue, 2f);
                        waterDust.noGravity = true; // 无重力效果
                        waterDust.fadeIn = 1.5f; // 亮度增加效果
                    }
                }
                else
                {
                    // 保留现有的粒子特效
                    for (int i = 0; i < 10; i++)
                    {
                        Vector2 particleVelocity = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(10)) * Main.rand.NextFloat(0.5f, 1.5f);

                        Dust waterDust = Dust.NewDustPerfect(Projectile.Center, DustID.Water, particleVelocity, 0, Color.LightSkyBlue, 2f);
                        waterDust.noGravity = true;
                        waterDust.fadeIn = 1.5f;
                    }
                }
            }
        }

        public override void AI()
        {
            // 调整弹幕的旋转
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;

            // 在水中每帧增加速度
            if (Projectile.wet)
            {
                Projectile.velocity *= 1.01f;
                if (Projectile.numUpdates % 3 == 0)
                {
                    // 将火花的颜色改为水元素的颜色
                    Color outerSparkColor = new Color(0, 105, 148);
                    float scaleBoost = MathHelper.Clamp(Projectile.ai[0] * 0.005f, 0f, 2f);
                    float outerSparkScale = 0.7f + scaleBoost;
                    SparkParticle spark = new SparkParticle(Projectile.Center, Projectile.velocity, false, 7, outerSparkScale, outerSparkColor);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }
            else
            {
                // 不在水中，增加重力效果
                Projectile.velocity.Y += 0.15f; // 每帧增加一个小值，模拟更强的重力
            }

            // 添加天蓝色光源
            Lighting.AddLight(Projectile.Center, Color.LightSkyBlue.ToVector3() * 0.49f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Eutrophication>(), 300); // 富营养化
        }
    }
}