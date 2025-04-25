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
    internal class AOBB1_PROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.AllinOneBulletBox";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj"; // 透明贴图

        private bool hasHitOnce = false; // 记录是否已经击中过敌人
        private bool isReturning = false; // 记录是否进入回旋状态
        private float baseSpeed; // 记录初始速度
        private int direction = 0; // -1 左回旋, 1 右回旋

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1; // 无限穿透
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 2; // 提高更新频率
            baseSpeed = Projectile.velocity.Length(); // 记录初始速度
        }

        public override void AI()
        {
            // 释放蓝色粒子特效（DustID.BlueTorch）
            if (Main.rand.NextBool(3)) // 1/3 概率生成
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.BlueTorch);
                dust.velocity *= 0.2f;
                dust.scale = 1.2f;
                dust.noGravity = true;
            }

            // 如果已经开始回旋
            if (isReturning)
            {
                // 速度缓慢增加
                Projectile.velocity *= 1.02f;

                // 继续回旋
                Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(2.5f * direction));

                return;
            }

            // 如果已经击中过敌人，则开始减速并逐渐拐弯
            if (hasHitOnce)
            {
                // 速度逐渐降低，但不会低于初始速度的20%
                float minSpeed = baseSpeed * 0.2f;
                if (Projectile.velocity.Length() > minSpeed)
                {
                    Projectile.velocity *= 0.98f;
                }
                else
                {
                    // 进入回旋状态
                    isReturning = true;
                }

                // 让弹幕缓慢朝着左或右回旋（初次命中时决定方向）
                Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(1.5f * direction));
            }
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (!hasHitOnce) // 第一次命中，伤害减半
            {
                modifiers.FinalDamage *= 0.5f;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!hasHitOnce) // 第一次命中
            {
                hasHitOnce = true;

                // 播放音效（建议使用 SoundID.Item103）
                SoundEngine.PlaySound(SoundID.Item103, Projectile.position);

                // 生成大量蓝色粒子
                for (int i = 0; i < 10; i++)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.BlueTorch);
                    dust.velocity = Main.rand.NextVector2Circular(2f, 2f);
                    dust.scale = 1.5f;
                    dust.noGravity = true;
                }

                // 随机决定回旋方向
                direction = Main.rand.NextBool() ? -1 : 1;
            }
            else if (isReturning) // 第二次命中
            {
                // 第二次命中伤害正常
                Projectile.Kill();
            }
        }
    }
}