using CalamityMod;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework.Graphics;
using CalamityRangerExpansion.LightingBolts;

namespace CalamityRangerExpansion.Content.WeaponToAMMO.Bullet.TheEmpty
{
    public class TheEmptyPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "WeaponToAMMO.Bullet.TheEmpty";
        private float rotationAngle = 0f; // 用于粒子旋转的角度
        private const float rotationSpeed = 0.02f; // 自转速度，较慢
        private float pulseTimer = 0f; // 用于脉冲变大小
        private float randomMoveTimer = 0f; // 随机运动时间
        private bool isTracking = false; // 是否开始追踪
        private Vector2 randomVelocity; // 用于存储随机运动的速度
        public override string Texture => "CalamityMod/Particles/Sparkle";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float scaleMultiplier = 1f + 0.15f * (float)Math.Sin(pulseTimer); // 脉冲缩放效果
            lightColor = Color.Cyan * 0.75f; // 幽灵蓝色

            Main.EntitySpriteDraw(
                Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value,
                Projectile.Center - Main.screenPosition,
                null,
                lightColor,
                Projectile.rotation,
                new Vector2(Projectile.width / 2, Projectile.height / 2), // 居中绘制
                scaleMultiplier,
                SpriteEffects.None,
                0
            );
            return false;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = Main.getGoodWorld ? -1 : 1;
            Projectile.timeLeft = 600;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;
        }

        public override void AI()
        {
            Projectile.rotation += rotationSpeed; // 持续自转
            pulseTimer += 0.05f; // 控制脉冲效果
            Lighting.AddLight(Projectile.Center, Color.Cyan.ToVector3() * 0.55f);

            if (!isTracking)
            {
                // 进入随机运动模式
                randomMoveTimer += 1f;
                if (randomMoveTimer > 60f) // 每 1 秒重新生成一个随机方向
                {
                    randomMoveTimer = 0f;
                    randomVelocity = Main.rand.NextVector2Circular(3f, 3f); // 生成随机速度
                }
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, randomVelocity, 0.1f);

                if (Projectile.timeLeft < 500) // 100 帧后开始追踪
                {
                    isTracking = true;
                }
            }
            else
            {
                // 追踪逻辑
                float trackingRange = Main.getGoodWorld ? 3800f : 1800f;
                float trackingSpeed = Main.getGoodWorld ? 15f : 12f;

                NPC target = Projectile.Center.ClosestNPCAt(trackingRange);
                if (target != null)
                {
                    Vector2 direction = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * trackingSpeed, 0.08f);
                }
            }

            // 粒子特效，往正后方喷射
            float dustSpeed = -Projectile.velocity.Length() * 1.2f; // 速度比弹幕本身快一点
            Vector2 dustSpawnPos = Projectile.Center + Main.rand.NextVector2Circular(1.5f * 16f, 1.5f * 16f); // 以弹幕中心为中心，半径 1.5 × 16 的随机点
            int dust = Dust.NewDust(dustSpawnPos, 0, 0, DustID.UltraBrightTorch, 0f, 0f, 100, default, Main.rand.NextFloat(0.5f, 1.5f));
            Main.dust[dust].noGravity = true;
            Main.dust[dust].velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * dustSpeed;
        }

        // 保留命中后大小变化的逻辑
        private Dictionary<int, bool> sizeChangeDirection = new Dictionary<int, bool>();
        private Dictionary<int, float> originalScale = new Dictionary<int, float>();

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!sizeChangeDirection.ContainsKey(target.whoAmI))
            {
                sizeChangeDirection[target.whoAmI] = Main.rand.NextBool();
                originalScale[target.whoAmI] = target.scale;
            }
            float scaleChangeFactor = sizeChangeDirection[target.whoAmI] ? 1.01f : 0.99f;
            target.scale *= scaleChangeFactor;
            target.width = (int)(originalScale[target.whoAmI] * target.width * scaleChangeFactor);
            target.height = (int)(originalScale[target.whoAmI] * target.height * scaleChangeFactor);
            target.netUpdate = true;

            LightingBoltsSystem.Spawn_GhostlyImpact(Projectile.Center);

        }
        public override bool? CanDamage()
        {
            return isTracking ? base.CanDamage() : false;
        }

    }
}
