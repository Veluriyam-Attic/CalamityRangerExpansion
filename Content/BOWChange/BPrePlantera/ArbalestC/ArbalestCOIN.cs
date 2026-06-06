//using CalamityMod;
//using Microsoft.Xna.Framework;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Terraria.Audio;
//using Terraria.ID;
//using Terraria.ModLoader;
//using Terraria;

//namespace CalamityRangerExpansion.Content.BOWChange.BPrePlantera.ArbalestC
//{
//    internal class ArbalestCOIN : ModProjectile, ILocalizedModType
//    {
//        public new string LocalizationCategory => "Weapons.BPrePlantera";
//        public override void SetStaticDefaults()
//        {
//            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
//            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
//            Main.projFrames[Projectile.type] = 8;

//        }
//        public override bool PreDraw(ref Color lightColor)
//        {
//            // 画残影效果
//            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
//            return false;
//        }
//        public override void SetDefaults()
//        {
//            // 设置弹幕的基础属性
//            Projectile.width = 11; // 弹幕宽度
//            Projectile.height = 24; // 弹幕高度
//            Projectile.friendly = true; // 对敌人有效
//            Projectile.DamageType = DamageClass.Ranged; // 远程伤害类型
//            Projectile.penetrate = 4; // 穿透力为x
//            Projectile.timeLeft = 300; // 弹幕存在时间为600帧
//            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
//            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
//            Projectile.ignoreWater = true; // 弹幕不受水影响
//            Projectile.arrow = true;
//            Projectile.extraUpdates = 1;
//            Projectile.aiStyle = ProjAIStyleID.Arrow; // 让弹幕受到重力影响
//        }

//        public override void AI()
//        {
//            // 调整弹幕的旋转，使其在飞行时保持水平
//            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;

//            // Lighting - 添加天蓝色光源，光照强度为 0.49
//            Lighting.AddLight(Projectile.Center, Color.LightSkyBlue.ToVector3() * 0.49f);

//            // 原有帧切换逻辑
//            Projectile.frameCounter++;
//            if (Projectile.frameCounter > 4)
//            {
//                Projectile.frame++;
//                Projectile.frameCounter = 0;
//            }
//            if (Projectile.frame > 7)
//            {
//                Projectile.frame = 0;
//            }

//            // 每3帧在弹幕当前位置生成1~2个滞留的GoldCoin Dust
//            if (Projectile.timeLeft % 3 == 0)
//            {
//                int dustCount = Main.rand.Next(1, 3); // 随机生成1到2个Dust
//                for (int i = 0; i < dustCount; i++)
//                {
//                    Dust dust = Dust.NewDustPerfect(Projectile.Center, 246, Vector2.Zero, 0, default, 1f);
//                    dust.noGravity = true; // 无重力
//                }
//            }
//            Time++;
//        }
//        public ref float Time => ref Projectile.ai[1];
//        public override bool? CanDamage() => Time >= 30f;

//        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
//        {
//            target.AddBuff(BuffID.Midas, 300);
//            SoundEngine.PlaySound(SoundID.CoinPickup, Projectile.position);

//            // 计算反弹的方向，遵循入射角等于出射角的原则
//            Vector2 reflectDirection = Vector2.Reflect(Projectile.velocity, Vector2.Normalize(target.Center - Projectile.Center));
//            Projectile.velocity = reflectDirection;
//        }
//        public override bool OnTileCollide(Vector2 oldVelocity)
//        {
//            // 碰撞物块时反弹
//            if (Projectile.velocity.X != oldVelocity.X) Projectile.velocity.X = -oldVelocity.X;
//            if (Projectile.velocity.Y != oldVelocity.Y) Projectile.velocity.Y = -oldVelocity.Y;

//            // 每次反弹减少一次穿透次数
//            Projectile.penetrate--;

//            return false;
//        }
//        public override void OnKill(int timeLeft)
//        {

//        }






//    }
//}