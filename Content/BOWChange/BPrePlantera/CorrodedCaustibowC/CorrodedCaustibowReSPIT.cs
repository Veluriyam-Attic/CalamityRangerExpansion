//using Microsoft.Xna.Framework;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Terraria.ID;
//using Terraria.ModLoader;
//using Terraria;

//namespace CalamityRangerExpansion.Content.BOWChange.BPrePlantera.CorrodedCaustibowC
//{
//    public class CorrodedCaustibowReSPIT : ModProjectile, ILocalizedModType
//    {
//        public new string LocalizationCategory => "Weapons.BPrePlantera";
//        public override void SetDefaults()
//        {
//            Projectile.width = 10;
//            Projectile.height = 10;
//            Projectile.friendly = true;
//            Projectile.DamageType = DamageClass.Ranged;
//            Projectile.penetrate = 1;
//            Projectile.timeLeft = 300; // 弹幕存活时间
//            Projectile.ignoreWater = true;
//            Projectile.tileCollide = true;
//            Projectile.extraUpdates = 1; // 提高更新频率，保证旋转流畅
//            Projectile.aiStyle = ProjAIStyleID.Arrow; // 让弹幕受到重力影响（这个可选）
//        }

//        public override void AI()
//        {
//            // 持续旋转，每帧增加 4 度
//            Projectile.rotation += MathHelper.ToRadians(4f);

//            // 在固定方向发射粒子特效，随着旋转方向改变
//            //Vector2 particleDirection = Projectile.velocity.RotatedBy(Projectile.rotation);
//            //Vector2 particlePosition = Projectile.Center;
//            //Vector2 particleVelocity = particleDirection * 0.5f; // 粒子速度

//            //if (Main.rand.NextBool(2)) // 50% 几率释放粒子
//            //{
//            //    Dust dust = Dust.NewDustPerfect(
//            //        particlePosition,
//            //        DustID.PoisonStaff, // 绿色毒雾特效
//            //        particleVelocity,
//            //        150, // 透明度
//            //        Color.GreenYellow, // 绿色粒子
//            //        Main.rand.NextFloat(1f, 1.5f) // 缩放
//            //    );
//            //    dust.noGravity = true; // 悬浮粒子
//            //}
//            Time++;
//        }
//        public ref float Time => ref Projectile.ai[1];
//        public override bool? CanDamage() => Time >= 30f;

//        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
//        {
//            // 为敌人添加酸性毒液 Debuff
//            target.AddBuff(BuffID.Venom, 300); // 中毒时间为 5 秒
//        }

//        public override bool OnTileCollide(Vector2 oldVelocity)
//        {
//            // 碰撞后反弹，遵循入射角等于出射角的原则
//            if (Projectile.velocity.X != oldVelocity.X)
//            {
//                Projectile.velocity.X = -oldVelocity.X; // 水平反弹
//            }
//            if (Projectile.velocity.Y != oldVelocity.Y)
//            {
//                Projectile.velocity.Y = -oldVelocity.Y; // 垂直反弹
//            }

//            return false; // 不销毁弹幕
//        }
//    }
//}