using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace CalamityRangerExpansion.Content.DeveloperItems.Bullet.AllinOneBulletBox
{
    internal class AOBB5_PROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.AllinOneBulletBox";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj"; // 透明贴图

        private int oscillationCounter = 0; // 记录摆动次数
        private bool movingLeft = true; // 记录当前方向
        private int hitCount = 0; // 记录命中次数

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1; // 无限穿透
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 2;
        }

        public override void AI()
        {
            // **左右摆动逻辑**
            if (oscillationCounter % 2 == 0) // 每 2 帧执行一次
            {
                if (movingLeft)
                    Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(-1));
                else
                    Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(1));

                oscillationCounter++;

                if (oscillationCounter >= 15) // 左摆 15° 后，换方向
                {
                    movingLeft = !movingLeft;
                    oscillationCounter = 0; // 重新计数
                }
            }

            CreateFlyingEffect();
        }

        private void CreateFlyingEffect()
        {
            if (Main.rand.NextBool(2)) // 1/2 概率生成粒子
            {
                int dustType = Main.rand.NextBool() ? DustID.PurpleTorch : DustID.Vortex; // 变成紫色系
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, dustType);
                dust.velocity *= 0.3f;
                dust.scale = Main.rand.NextFloat(1.6f, 2.2f);
                dust.noGravity = true;
            }

            // **三角形分布的环绕光效**
            for (int i = 0; i < 3; i++) // 三角形顶点
            {
                float angle = MathHelper.TwoPi / 3 * i + Projectile.ai[0] * 0.1f;
                Vector2 offset = new Vector2(6f, 0).RotatedBy(angle);
                int dustType = i == 0 ? DustID.PurpleTorch : DustID.Vortex;
                Dust ringDust = Dust.NewDustDirect(Projectile.Center + offset, 1, 1, dustType);
                ringDust.velocity *= 0.1f;
                ringDust.scale = 1.3f;
                ringDust.noGravity = true;
            }

            // **紫色电弧轨迹**
            if (Main.rand.NextBool(6))
            {
                Vector2 arcOffset = new Vector2(5, 0).RotatedByRandom(MathHelper.TwoPi);
                Dust arcDust = Dust.NewDustDirect(Projectile.position + arcOffset, 1, 1, DustID.PurpleTorch);
                arcDust.noGravity = true;
                arcDust.scale = 1.4f;
            }
        }

        private void CreateExplosionEffect()
        {
            for (int i = 0; i < 30; i++) // 增加爆炸粒子数量
            {
                int dustType = Main.rand.NextBool() ? DustID.PurpleTorch : DustID.Vortex;
                Dust dust = Dust.NewDustDirect(Projectile.Center, 10, 10, dustType);
                dust.velocity = Main.rand.NextVector2Circular(7f, 7f); // 更快的扩散速度
                dust.scale = Main.rand.NextFloat(2.0f, 2.6f);
                dust.noGravity = true;
            }

            // **爆炸时的三角形光束扩散**
            for (int i = 0; i < 3; i++) // 三角形顶点爆炸光束
            {
                float angle = MathHelper.TwoPi / 3 * i + Main.rand.NextFloat(-0.1f, 0.1f);
                Vector2 direction = new Vector2(10f, 0).RotatedBy(angle);
                Dust burstDust = Dust.NewDustDirect(Projectile.Center + direction * 2f, 1, 1, DustID.Vortex);
                burstDust.velocity = direction * 0.5f;
                burstDust.scale = 2.2f;
                burstDust.noGravity = true;
            }
        }


        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            hitCount++; // 记录命中次数

            // 计算伤害倍率
            float damageMultiplier = hitCount switch
            {
                1 => 2.0f,  // 第 1 个敌人：2 倍伤害
                2 => 0.5f,  // 第 2 个敌人：0.5 倍
                3 => 0.4f,  // 第 3 个敌人：0.4 倍
                4 => 0.3f,  // 第 4 个敌人：0.3 倍
                5 => 0.2f,  // 第 5 个敌人：0.2 倍
                _ => 0.1f   // 之后一直 0.1 倍
            };

            modifiers.FinalDamage *= damageMultiplier;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // **触发爆炸粒子**
            CreateExplosionEffect();

            // **播放音效**
            SoundEngine.PlaySound(SoundID.Item94, Projectile.position);
        }


    }
}