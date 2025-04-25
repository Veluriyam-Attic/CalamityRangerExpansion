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

namespace CalamityRangerExpansion.Content.DeveloperItems.Bullet.AllinOneBulletBox
{
    internal class AOBB3_PROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.AllinOneBulletBox";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj"; // 透明贴图

        private float initialSpeed; // 记录初始速度
        private float maxSpeed; // 最高速度（2.5倍）

        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1; // 仅能命中 1 次
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 2;
        }

        public override void OnSpawn(IEntitySource source)
        {
            initialSpeed = Projectile.velocity.Length(); // 记录初始速度
            maxSpeed = initialSpeed * 2.5f; // 最高速度
        }

        public override void AI()
        {
            // **线性加速**
            if (Projectile.velocity.Length() < maxSpeed)
            {
                Projectile.velocity *= 1.02f; // 每帧增加速度
            }

            CreateFlyingDust();
        }

        private void CreateFlyingDust()
        {
            if (Main.rand.NextBool(3)) // 1/3 概率生成粒子
            {
                Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.GreenTorch);
                dust.velocity *= 0.1f;
                dust.scale = 1.2f;
                dust.noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // **生成持续伤害的 AOBB3_Stay 弹幕**
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<AOBB3_Stay>(), Projectile.damage, 0f, Projectile.owner);
            }

            // **释放粒子爆炸**
            CreateExplosionEffect();

            // **子弹消失**
            Projectile.Kill();
        }

        private void CreateExplosionEffect()
        {
            for (int i = 0; i < 12; i++)
            {
                Dust dust = Dust.NewDustDirect(Projectile.Center, 10, 10, DustID.GreenTorch);
                dust.velocity = Main.rand.NextVector2Circular(4f, 4f);
                dust.scale = 1.5f;
                dust.noGravity = true;
            }
        }
    }
}