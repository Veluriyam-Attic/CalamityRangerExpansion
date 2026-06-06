//using CalamityMod;
//using Microsoft.Xna.Framework;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Terraria.ID;
//using Terraria.ModLoader;
//using Terraria;
//using CalamityMod.Particles;
//using Terraria.Audio;

//namespace CalamityRangerExpansion.Content.BOWChange.DPreDog.DaemonsFlameC
//{
//    internal class DaemonsFlameRePROJ : ModProjectile, ILocalizedModType
//    {
//        public new string LocalizationCategory => "Weapons.DPreDog";

//        public override void SetStaticDefaults()
//        {
//            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
//            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
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
//            Projectile.penetrate = 1; // 穿透力为1，击中一个敌人就消失
//            Projectile.timeLeft = 15; // 弹幕存在时间为x帧
//            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
//            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
//            Projectile.ignoreWater = true; // 弹幕不受水影响
//            Projectile.arrow = true;
//            Projectile.extraUpdates = 8;
//        }

//        public override void AI()
//        {
//            // 调整弹幕的旋转，使其在飞行时保持水平
//            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;

//            // Lighting - 添加天蓝色光源，光照强度为 0.49
//            Lighting.AddLight(Projectile.Center, Color.LightSkyBlue.ToVector3() * 0.49f);

//            // 搞个那个哈雷彗星炮类似的激光，注意是把它发射出去，自己站在原地等一等（释放大量的电能粒子特效往自己这吸引，然后逐渐减速，模仿翠芒，但是有瞄准的能力，并不是直接往前射，不穿），类似普罗特斯
//            // 弹幕保持直线运动并逐渐减速
//            Projectile.velocity *= 0.99f;


//            //// 添加能量光效
//            //LineParticle energy = new LineParticle(Projectile.Center + Projectile.velocity * 4, Projectile.velocity * 4.95f, false, 9, 2.4f, Color.BlueViolet);
//            //GeneralParticleHandler.SpawnParticle(energy);

//            //// 每帧生成电能粒子
//            //for (int i = 0; i < Main.rand.Next(5, 7); i++)
//            //{
//            //    // 在弹幕中心的附近生成粒子，方向平行于弹幕的飞行方向
//            //    Vector2 randomOffset = Main.rand.NextVector2Circular(4f, 4f);
//            //    Vector2 particleVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(1f, 2f); // 平行于弹幕方向

//            //    Particle electricParticle = new SparkParticle(
//            //        Projectile.Center + randomOffset,
//            //        particleVelocity, // 粒子速度平行于弹幕
//            //        true,
//            //        40,
//            //        Main.rand.NextFloat(0.8f, 0.9f),
//            //        Color.Cyan
//            //    );

//            //    GeneralParticleHandler.SpawnParticle(electricParticle);
//            //}

//            // 飞行特效 - 绘制三角形粒子效果
//            for (int i = 0; i < 3; i++) // 三角形顶点
//            {
//                Vector2 triangleVertex;
//                switch (i)
//                {
//                    case 0:
//                        triangleVertex = Projectile.Center + Projectile.velocity.RotatedBy(MathHelper.PiOver4) * 10f;
//                        break;
//                    case 1:
//                        triangleVertex = Projectile.Center + Projectile.velocity.RotatedBy(-MathHelper.PiOver4) * 10f;
//                        break;
//                    case 2:
//                        triangleVertex = Projectile.Center - Projectile.velocity.SafeNormalize(Vector2.Zero) * 15f;
//                        break;
//                    default:
//                        triangleVertex = Projectile.Center;
//                        break;
//                }

//                // 生成粒子特效 141 和 164，大小和速度随机化
//                Dust dust = Dust.NewDustPerfect(triangleVertex, Main.rand.NextBool() ? 141 : 164);
//                dust.scale = Main.rand.NextFloat(1.1f, 1.6f); // 粒子大小
//                dust.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(10f, 13f); // 粒子速度
//                dust.noGravity = true; // 禁用重力效果
//            }

//        }

//        public override void OnKill(int timeLeft)
//        {
//            // 寻找最近的敌人
//            NPC closestNPC = null;
//            float closestDistance = float.MaxValue;

//            foreach (NPC npc in Main.npc)
//            {
//                if (npc.active && !npc.friendly && npc.life > 0)
//                {
//                    float distance = Vector2.Distance(Projectile.Center, npc.Center);
//                    if (distance < closestDistance)
//                    {
//                        closestDistance = distance;
//                        closestNPC = npc;
//                    }
//                }
//            }

//            // 发射激光
//            if (closestNPC != null)
//            {
//                Vector2 directionToTarget = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
//                Projectile.NewProjectile(
//                    Projectile.GetSource_FromThis(),
//                    Projectile.Center,
//                    directionToTarget * 75f,
//                    ModContent.ProjectileType<DaemonsFlameRePROJLazer>(),
//                    (int)(Projectile.damage * 1.0f),
//                    Projectile.knockBack,
//                    Projectile.owner
//                );

//                // 播放音效
//                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/HalleysInfernoShoot"), Projectile.Center);
//            }
//        }

//        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
//        {

//        }
//    }
//}