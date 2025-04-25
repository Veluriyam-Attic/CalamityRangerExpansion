using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using CalamityMod.Particles;

namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.BrassBeast
{
    public class BrassBeastPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.BrassBeast";
        // 使用透明贴图
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            // 基本参数设定
            Projectile.width = Projectile.height = 30; // 碰撞箱大小
            Projectile.friendly = true; // 友方投射物
            Projectile.ignoreWater = true; // 无视水
            Projectile.tileCollide = false; // 不与地形碰撞
            Projectile.DamageType = DamageClass.Ranged; // 远程伤害
            Projectile.penetrate = 1; // 穿
            Projectile.MaxUpdates = 30; // 最大更新帧数
            Projectile.timeLeft = 150; // 存活时间
            Projectile.usesIDStaticNPCImmunity = true; // 开启静态NPC击中冷却
            Projectile.idStaticNPCHitCooldown = 6; // 每6帧可击中一次相同NPC
        }

        public override void AI()
        {
            // 添加光照
            Lighting.AddLight(Projectile.Center, Color.OrangeRed.ToVector3() * 0.2f);

            // 飞行期间释放粒子特效
            ReleaseParticles();
            ReleaseHeavySmoke();
            //// 每三次更新释放一次重型烟雾粒子
            //if (Main.GameUpdateCount % 3 == 0)
            //{
            //    ReleaseHeavySmoke();
            //}
        }

        //private void ReleaseParticles()
        //{
        //    // 双螺旋粒子特效
        //    float offsetMagnitude = 10f;
        //    float rotationSpeed = MathHelper.PiOver4 * 2.5f; // 旋转速度增加至原来的2.5倍
        //    int dustType = DustID.Smoke;

        //    // 两个螺旋点
        //    for (int i = 0; i < 2; i++)
        //    {
        //        float rotation = (Main.GameUpdateCount * rotationSpeed + MathHelper.Pi * i) % MathHelper.TwoPi;
        //        Vector2 offset = offsetMagnitude * new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
        //        Dust.NewDustPerfect(Projectile.Center + offset, dustType, Vector2.Zero, 100, Color.Gray, 1.2f).noGravity = true;
        //    }

        //    // 粒子往两侧排开
        //    for (int i = 0; i < 3; i++)
        //    {
        //        int sideDustType = Main.rand.Next(new int[] { 195, 191, 240 });
        //        Vector2 sideOffset = new Vector2(Projectile.velocity.Y, -Projectile.velocity.X).SafeNormalize(Vector2.Zero) * (5f * (i - 1));
        //        Dust.NewDustPerfect(Projectile.Center + sideOffset, sideDustType, Vector2.Zero, 150, Color.White, 1.5f).noGravity = true;
        //    }
        //}

        private void ReleaseParticles()
        {
            // 双螺旋粒子特效
            float offsetMagnitude = 10f;
            float rotationSpeed = MathHelper.PiOver4 * 2.5f; // 旋转速度增加至原来的2.5倍
            int dustType = DustID.Smoke;

            // 两个螺旋点
            for (int i = 0; i < 16; i++)
            {
                float rotation = (Main.GameUpdateCount * rotationSpeed + MathHelper.PiOver2 * i) % MathHelper.TwoPi;
                Vector2 offset = offsetMagnitude * new Vector2((float)Math.Cos(rotation), (float)Math.Sin(rotation));
                Dust smokeDust = Dust.NewDustPerfect(Projectile.Center + offset, dustType, Main.rand.NextVector2Circular(2f, 2f), 100, Color.Gray, Main.rand.NextFloat(0.8f, 1.5f));
                smokeDust.noGravity = true;
            }

            // 粒子随机喷射
            for (int i = 0; i < 24; i++)
            {
                int sideDustType = Main.rand.Next(new int[] { 195, 191, 240 });
                Vector2 randomDirection = Main.rand.NextVector2Circular(1.5f, 1.5f); // 随机方向
                Dust.NewDustPerfect(Projectile.Center, sideDustType, randomDirection * Main.rand.NextFloat(1f, 3f), 150, Color.White, Main.rand.NextFloat(0.8f, 1.5f)).noGravity = true;
            }
        }


        private void ReleaseHeavySmoke()
        {
            Vector2 smokePosition = Projectile.Center + Main.rand.NextVector2Circular(20f, 20f);
            Particle heavySmoke = new HeavySmokeParticle(
                smokePosition,
                Main.rand.NextVector2Circular(2f, 2f),
                Color.Black, // 烟雾颜色
                Main.rand.Next(20, 30), // 存活时间减半
                Main.rand.NextFloat(0.75f, 0.85f), // 缩放减半
                0.5f, // 透明度减半
                MathHelper.ToRadians(Main.rand.NextFloat(0f, 2f)), // 随机旋转
                true
            );
            GeneralParticleHandler.SpawnParticle(heavySmoke);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 修改后的重型烟雾粒子
            for (int i = 0; i < 15; i++)
            {
                Vector2 smokePosition = Projectile.Center + Main.rand.NextVector2Circular(20f, 20f);
                Particle heavySmoke = new HeavySmokeParticle(
                    smokePosition,
                    Main.rand.NextVector2Circular(3f, 3f),
                    Color.Black, // 烟雾颜色
                    Main.rand.Next(20, 30), // 存活时间减半
                    Main.rand.NextFloat(0.75f, 1f), // 缩放减半
                    0.5f, // 透明度减半
                    MathHelper.ToRadians(Main.rand.NextFloat(0f, 2f)), // 随机旋转
                    true
                );
                GeneralParticleHandler.SpawnParticle(heavySmoke);
            }

            //// 尖字型粒子特效
            //for (int i = 0; i < 6; i++)
            //{
            //    Vector2 offset = Main.rand.NextVector2Circular(15f, 15f);
            //    PointParticle spikeParticle = new PointParticle(
            //        Projectile.Center + offset,
            //        Main.rand.NextVector2Circular(2f, 2f), // 粒子速度
            //        false,
            //        25, // 粒子存活时间
            //        1.5f, // 粒子大小倍率
            //        Color.OrangeRed // 颜色为橙红色
            //    );
            //    GeneralParticleHandler.SpawnParticle(spikeParticle);
            //}
        }


    }
}

