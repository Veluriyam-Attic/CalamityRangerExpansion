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
using CalamityMod.Particles;

namespace CalamityRangerExpansion.Content.BOWChange.DPreDog.PlanetaryAnnihilationC.SolarSystemPROJ
{
    internal class PEJupiter : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.DPreDog";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // 画残影效果
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
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
            Projectile.ignoreWater = true; // 弹幕不受水影响
            Projectile.arrow = true;
            Projectile.extraUpdates = 1;
        }
        // 木星颜色随机池
        private readonly Color[] jupiterColors = new Color[]
        {
    new Color(176, 123, 89), // 浅棕色，代表木星表面云层
    new Color(242, 66, 54),  // 红色，代表木星的大红斑
    new Color(245, 197, 128) // 浅橙色，代表木星带状云
        };

        public override void AI()
        {
            // 调整弹幕的旋转，使其在飞行时保持水平
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;

            // Lighting - 添加天蓝色光源，光照强度为 0.49
            Lighting.AddLight(Projectile.Center, Color.LightSkyBlue.ToVector3() * 0.49f);


            // 每隔 x 帧生成一次
            if (Projectile.localAI[0] % 5 == 0) 
            {
                // 随机选择颜色
                Color smokeColor = jupiterColors[Main.rand.Next(jupiterColors.Length)];

                // 创建轻型烟雾粒子
                Vector2 dustVelocity = Main.rand.NextVector2Circular(1f, 1f); // 粒子随机速度
                Particle smoke = new HeavySmokeParticle(
                    Projectile.Center,
                    dustVelocity * Main.rand.NextFloat(1f, 2.6f), // 随机速度放大
                    smokeColor, // 使用随机颜色
                    18, // 粒子尺寸
                    Main.rand.NextFloat(0.9f, 1.6f), // 缩放范围
                    0.35f, // 不透明度
                    Main.rand.NextFloat(-1, 1), // 随机旋转
                    true // 粒子是否随重力运动
                );

                // 生成粒子
                GeneralParticleHandler.SpawnParticle(smoke);
            }
               
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 计算上圆心和下圆心的位置
            Vector2 topCircleCenter = Projectile.Center + new Vector2(0, -50 * 16);
            Vector2 bottomCircleCenter = Projectile.Center + new Vector2(0, 50 * 16);

            // 生成两个圆圈的弹幕
            for (int i = 0; i < 2; i++) // 两个圆圈
            {
                Vector2 circleCenter = i == 0 ? topCircleCenter : bottomCircleCenter;

                // 在圆圈内生成弹幕
                int projectileCount = Main.rand.Next(1, 3); // 随机生成 1~2 发弹幕
                for (int j = 0; j < projectileCount; j++)
                {
                    // 随机选择一个点
                    float angle = Main.rand.NextFloat(0, MathHelper.TwoPi);
                    float radius = Main.rand.NextFloat(0, 20 * 16);
                    Vector2 spawnPosition = circleCenter + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius;

                    // 计算速度指向目标 NPC 的位置
                    Vector2 velocity = (target.Center - spawnPosition).SafeNormalize(Vector2.Zero) * 10f;

                    // 生成弹幕
                    Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        spawnPosition,
                        velocity,
                        ModContent.ProjectileType<PEJupiterSPIT>(),
                        (int)(damageDone * 0.45f), // 伤害倍率为 0.45
                        Projectile.knockBack,
                        Projectile.owner
                    );
                }
            }
        }
        public override void OnKill(int timeLeft)
        {

        }







    }
}