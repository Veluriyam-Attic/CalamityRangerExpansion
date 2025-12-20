using CalamityMod;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.HD2.LAS17
{
    internal class LAS17Proj : ModProjectile, ILocalizedModType
    {
        public override string Texture => "CalamityMod/Projectiles/LaserProj";
        public new string LocalizationCategory => "DeveloperItems.LAS17";

        // 从 Holdout 传入的热量阶段
        public int WeaponStage = 0;

        private bool penetratedSet;
        private int fxCounter;

        // SHPL 同款：偏黄激光
        public override Color? GetAlpha(Color lightColor)
            => new Color(255, 235, 120, 0);

        // 纯 Beam 绘制
        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawBeam(200f, 3f, lightColor);
            return false;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 500;
            Projectile.penetrate = 2;
            Projectile.extraUpdates = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;

            // 🔑 SHPL 核心：出生即不可见
            Projectile.alpha = 255;
        }

        public override void OnSpawn(IEntitySource source)
        {
            // localAI[0] = 0 表示“尚未完成初始化”
            Projectile.localAI[0] = 0f;
        }

        public override void AI()
        {
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 25;
            }
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }
            Lighting.AddLight((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16, 0.5f, 0.2f, 0.5f);
            float timerIncr = 3f;
            if (Projectile.ai[1] == 0f)
            {
                Projectile.localAI[0] += timerIncr;
                if (Projectile.localAI[0] > 100f)
                {
                    Projectile.localAI[0] = 100f;
                }
            }
            else
            {
                Projectile.localAI[0] -= timerIncr;
                if (Projectile.localAI[0] <= 0f)
                {
                    Projectile.Kill();
                    return;
                }
            }

            // =========================
            // 穿透只设置一次
            // =========================
            if (!penetratedSet)
            {
                Projectile.penetrate = WeaponStage switch
                {
                    >= 5 => -1,
                    >= 4 => 7,
                    >= 2 => 3,
                    _ => 1
                };
                penetratedSet = true;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            // 只在主更新帧推进特效节奏
            if (Projectile.numUpdates == 0)
                fxCounter++;

            Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            Vector2 back = -dir;

 
            Lighting.AddLight(Projectile.Center, new Color(255, 220, 120).ToVector3() * 0.6f);






            {


                //// =========================
                //// 特效：直线逻辑线（每帧在直线上释放一个辉光点）
                //// =========================
                //{
                //    // 在弹幕后方沿直线均匀铺点
                //    float backDist = 14f; // 点与弹幕中心的固定距离
                //    Vector2 pos = Projectile.Center + back * backDist;

                //    GlowOrbParticle orb = new GlowOrbParticle(
                //        pos,                 // 固定在直线上的位置
                //        Vector2.Zero,        // 不移动
                //        false,               // 不受重力
                //        5,                   // 生命周期短，形成连续线感
                //        0.9f,                // 尺寸
                //        Color.Gold,          // 偏黄
                //        true,                // 加法混合，亮
                //        false,
                //        true
                //    );
                //    GeneralParticleHandler.SpawnParticle(orb);
                //}


            }




        }
    }
}
