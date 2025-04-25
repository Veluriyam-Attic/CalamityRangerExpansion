using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace CalamityRangerExpansion.Content.DeveloperItems.Bullet.AllinOneBulletBox
{
    internal class AOBB2_PROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.AllinOneBulletBox";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj"; // 透明贴图

        private bool hasWarped = false; // 记录是否已传送
        private Vector2 storedVelocity; // 存储原始方向
        private int warpTimer = 12; // 12 帧后传送

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1; // 初始穿透无限
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 2;
            storedVelocity = Projectile.velocity; // 记录原始方向
        }

        public override void AI()
        {
            if (!hasWarped)
            {
                // **正常飞行阶段**
                if (warpTimer > 0)
                {
                    warpTimer--;
                    CreateFlyingDust(); // 绿色粒子
                    return;
                }

                // **触发传送**
                Warp();
            }
            else
            {
                // **传送后飞行**
                CreatePostWarpDust(); // 传送后的不同粒子效果
            }
        }

        private void Warp()
        {
            hasWarped = true;

            // **传送前特效**
            CreateWarpEffect(Projectile.Center);

            // **传送位置**
            Vector2 warpOffset = new Vector2(-Main.rand.NextFloat(100f, 160f), 0).RotatedBy(Main.player[Projectile.owner].velocity.ToRotation());
            Projectile.Center = Main.player[Projectile.owner].Center + warpOffset;

            // **保持方向**
            Projectile.velocity = storedVelocity;
            Projectile.alpha = 0; // 重新变可见
            Projectile.penetrate = 1; // 只穿透 1 次

            // **释放二次传送特效**
            CreateWarpEffect(Projectile.Center);
        }

        private void CreateFlyingDust()
        {
            if (Main.rand.NextBool(3)) // 1/3 概率生成粒子
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GreenTorch);
                dust.velocity *= 0.1f;
                dust.scale = 1.1f;
                dust.noGravity = true;
            }
        }

        private void CreatePostWarpDust()
        {
            if (Main.rand.NextBool(2)) // 1/2 概率
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GreenFairy);
                dust.velocity *= 0.3f;
                dust.scale = 1.3f;
                dust.noGravity = true;
            }
        }

        private void CreateWarpEffect(Vector2 position)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust dust = Dust.NewDustDirect(position, 10, 10, DustID.GreenTorch);
                dust.velocity = Main.rand.NextVector2Circular(3f, 3f);
                dust.scale = 1.5f;
                dust.noGravity = true;
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (!hasWarped) // 传送前
            {
                modifiers.FinalDamage *= 0.5f; // 伤害减半
            }
        }
    }
}