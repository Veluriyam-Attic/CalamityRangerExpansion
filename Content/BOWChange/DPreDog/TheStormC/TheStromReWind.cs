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
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.DPreDog.TheStormC
{
    internal class TheStromReWind : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.DPreDog";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 6;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // 绘制拖尾效果
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 42;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true; // 允许与方块碰撞
            Projectile.extraUpdates = 1; // 额外更新次数
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
            //Projectile.timeLeft = Lifetime;
            //Projectile.Opacity = 0f;
        }

        public override void AI()
        {
            // 帧动画切换
            Projectile.frameCounter++;
            Projectile.frame = Projectile.frameCounter / 5 % Main.projFrames[Type];

            // 天蓝色粒子效果
            BowChangeVFX.SpawnTrail(Projectile, BowChangeTheme.Storm, 0.55f);
            if (Main.rand.NextBool(5))
            {
                int d = Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 136, new Color(172, 238, 255), 1.4f);
                Main.dust[d].noGravity = true;
                Main.dust[d].fadeIn = 1.5f;
                Main.dust[d].velocity = -Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(30)) * 0.2f;
            }

            // 前 30 帧不追踪，保持旋转效果
            if (Projectile.ai[1] <= 30)
            {
                Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(-2)); // 每帧左转2度
                Projectile.ai[1]++;
            }
            else // 开始追踪最近敌人
            {
                NPC target = Projectile.Center.ClosestNPCAt(2800); // 查找xx范围内最近的敌人
                if (target != null)
                {
                    Vector2 direction = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * 15f, 0.08f); // 追踪速度为xf
                }
            }

            Time++;
        }
        public ref float Time => ref Projectile.localAI[0];

        public override bool? CanDamage() => Time >= 12f; // 初始的时候不会造成伤害，直到x为止


        public override void OnKill(int timeLeft)
        {
            BowChangeVFX.SpawnImpact(Projectile, BowChangeTheme.Storm, 0.8f);

            // 原地扩散 50 个粒子
            for (int i = 0; i < 50; i++)
            {
                Vector2 dustVelocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(1f, 2.6f); // 初始速度随机
                Color smokeColor = Color.White; // 粒子颜色为白色
                Particle smoke = new HeavySmokeParticle(
                    Projectile.Center,
                    dustVelocity,
                    smokeColor,
                    18, // 粒子大小
                    Main.rand.NextFloat(0.9f, 1.6f), // 缩放
                    0.35f, // 不透明度
                    Main.rand.NextFloat(-1, 1), // 随机旋转
                    true // 粒子是否跟随重力
                );
                GeneralParticleHandler.SpawnParticle(smoke);
            }
        }


    }
}
