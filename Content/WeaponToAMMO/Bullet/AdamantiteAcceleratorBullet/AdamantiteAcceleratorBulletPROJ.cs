using CalamityMod;
using CalamityRangerExpansion.CREConfigs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.Particles;

namespace CalamityRangerExpansion.Content.WeaponToAMMO.Bullet.AdamantiteAcceleratorBullet
{
    internal class AdamantiteAcceleratorBulletPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "WeaponToAMMO.Bullet.AdamantiteAcceleratorBullet";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 2; // 根据模式设置穿透次数
            Projectile.timeLeft = 160;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 11;
        }
        private Vector2 initialVelocity; // 记录初始速度
        private float maxSpeedMultiplier = 5f; // 速度最大倍率
        public override void OnSpawn(IEntitySource source)
        {
            // 在弹幕生成时，将速度固定为 Xf
            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 2.1f;
            initialVelocity = Projectile.velocity; // 记录初始速度
        }

        public override void AI()
        {
            // 让弹幕旋转方向与速度一致
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            // 添加光源效果
            Lighting.AddLight(Projectile.Center, Color.Red.ToVector3() * 0.55f);

            // **始终生成一个红色 SparkParticle**
            //SparkParticle tip = new SparkParticle(
            //    Projectile.Center,
            //    Vector2.Zero, // 粒子静止
            //    false,
            //    20,
            //    0.8f,
            //    Color.Red
            //);
            //GeneralParticleHandler.SpawnParticle(tip);

            // **每 3 帧才执行一次粒子生成**
            if (Projectile.ai[0] % 3 == 0)
            {
                // **在四个方向生成 SquareParticle**
                Vector2[] offsets = new Vector2[]
                {
            new Vector2(-16, 0), // 左
            new Vector2(16, 0),  // 右
            new Vector2(0, -16), // 上
            new Vector2(0, 16)   // 下
                };

                foreach (Vector2 offset in offsets)
                {
                    SquareParticle trail = new SquareParticle(
                        Projectile.Center + offset,
                        new Vector2(0, -0.1f), // 粒子向上移动
                        false,
                        3, // 粒子存活时间
                        1.5f,
                        Color.Red
                    );
                    GeneralParticleHandler.SpawnParticle(trail);
                }
            }

            // **更新弹幕速度**
            Projectile.velocity *= 1.02f; // 线性加速
            if (Projectile.velocity.Length() >= initialVelocity.Length() * maxSpeedMultiplier)
            {
                ResetVelocity(); // 速度达到上限后重置
            }
        }
        private void ResetVelocity()
        {
            Projectile.velocity = initialVelocity; // 重置速度

            // **释放十字形 LineParticle**
            for (int i = 0; i < 4; i++)
            {
                Vector2 direction = Vector2.UnitX.RotatedBy(MathHelper.PiOver2 * i) * 4f; // 4个方向
                LineParticle crossEffect = new LineParticle(
                    Projectile.Center,
                    direction,
                    false,
                    20,
                    1.0f,
                    Color.Red
                );
                GeneralParticleHandler.SpawnParticle(crossEffect);
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // 根据弹幕当前速度计算伤害倍率
            float damageMultiplier = 1.0f + Projectile.velocity.Length() / 4f;
            modifiers.FinalDamage *= damageMultiplier;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SoundID.Item10); // 播放音效

            // 计算生成的红色光环特效数量
            int particleCount = 3 + (int)(Projectile.velocity.Length() / 1f);

            // 在敌人周围生成粒子特效
            for (int i = 0; i < particleCount; i++)
            {
                Color bloom = Color.Red;
                Vector2 randomDirection = Main.rand.NextVector2Circular(1f, 1f).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(8f, 17f);

                GeneralParticleHandler.SpawnParticle(new CritSpark(
                    target.Center,
                    randomDirection,
                    Color.IndianRed,
                    bloom,
                    0.7f + Main.rand.NextFloat(0f, 0.6f), // 粒子大小
                    30, // 粒子存活时间
                    0.4f, 0.6f // 初始和最终透明度
                ));
            }


            // 添加红色光环特效
            int auraCount = 3 + (int)(Projectile.velocity.Length() / 4f);
            for (int i = 0; i < auraCount; i++)
            {
                float polarity = Main.rand.NextFloat(-1f, 1f); // 随机方向
                Color pulseColor = Color.Red;
                AuraPulseRing auraPulse = new AuraPulseRing(
                    pulseColor,
                    new Vector2(Math.Max(target.width / 156f * 1.1f, 0.25f), 0.3f),
                    new Vector2(Math.Max(target.width / 156f * 1.5f, 0.4f), 0.01f),
                    40, // 持续时间
                    target
                );
                GeneralParticleHandler.SpawnParticle(auraPulse);
            }
        }

    }
}