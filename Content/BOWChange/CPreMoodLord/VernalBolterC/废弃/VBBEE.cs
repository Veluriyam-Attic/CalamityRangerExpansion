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

//namespace CalamityRangerExpansion.Content.BOWChange.CPreMoodLord.VernalBolterC.废弃
//{
//    internal class VBBEE : ModProjectile, ILocalizedModType
//    {
//        public new string LocalizationCategory => "Weapons.CPreMoodLord";
//        public override void SetStaticDefaults()
//        {
//            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 1;
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
//            Projectile.timeLeft = 300; // 弹幕存在时间为x帧
//            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
//            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
//            Projectile.ignoreWater = true; // 弹幕不受水影响
//            Projectile.arrow = true;
//            Projectile.extraUpdates = 1;
//        }

//        public override void AI()
//        {
//            // 调整弹幕的旋转，使其在飞行时保持水平
//            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;

//            // Lighting - 添加天蓝色光源，光照强度为 0.49
//            Lighting.AddLight(Projectile.Center, Color.LightSkyBlue.ToVector3() * 0.49f);


//            // 在弹幕路径上生成黄黑双色粒子特效
//            for (int i = 0; i < 2; i++)
//            {
//                Vector2 offset = new Vector2(Projectile.width / 2, 0).RotatedBy(Projectile.rotation + MathHelper.PiOver2 * (i == 0 ? -1 : 1));
//                Vector2 particlePosition = Projectile.Center + offset + Projectile.velocity * 0.5f;

//                Dust particle = Dust.NewDustPerfect(particlePosition, DustID.Smoke, new Vector2(i == 0 ? -1 : 1, 0) * 1.5f, 100, i == 0 ? Color.Yellow : Color.Black, 1.2f);
//                particle.noGravity = true;
//                particle.velocity *= 0.75f;
//            }

//            // 前30帧不追踪，之后开始追踪敌人
//            if (Projectile.ai[1] > 45)
//            {
//                NPC target = Projectile.Center.ClosestNPCAt(2400); // 查找范围内最近的敌人
//                if (target != null)
//                {
//                    Vector2 direction = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
//                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * 18f, 0.08f); // 追踪速度为12f
//                }
//            }
//            else
//            {
//                Projectile.ai[1]++;
//            }
//        }

//        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
//        {
//            target.AddBuff(BuffID.Venom, 300); // 5 秒钟的 Venom
//            target.AddBuff(BuffID.Poisoned, 300); // 5 秒钟的 Poisoned            
//        }
//        public override void OnKill(int timeLeft)
//        {
//            // 在原地生成黄黑 Dust 特效
//            for (int i = 0; i < 20; i++)
//            {
//                Vector2 randomVelocity = Main.rand.NextVector2Circular(1.5f, 1.5f);
//                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Smoke, randomVelocity, 100, i % 2 == 0 ? Color.Yellow : Color.Black, 1.5f);
//                dust.noGravity = true;
//            }

//            // 往前方扩散 4 发弹幕
//            float spreadAngle = MathHelper.ToRadians(20f); // 总扩散角度 20 度
//            for (int i = -1; i <= 2; i++) // -1 到 2，总共 4 发
//            {
//                Vector2 velocity = Projectile.velocity.RotatedBy(spreadAngle / 3 * i);
//                int waspProjectile = Projectile.NewProjectile(
//                    Projectile.GetSource_FromThis(),
//                    Projectile.Center,
//                    velocity,
//                    ProjectileID.Wasp, // 原版 189 弹幕
//                    (int)(Projectile.damage * 1.2f), // 伤害倍率 1.2
//                    Projectile.knockBack,
//                    Projectile.owner
//                );

//                // 修改生成的弹幕属性
//                if (waspProjectile.WithinBounds(Main.maxProjectiles))
//                {
//                    Projectile proj = Main.projectile[waspProjectile];
//                    proj.DamageType = DamageClass.Ranged; // 改为远程伤害类型
//                    proj.penetrate = 5; // 设置穿透次数
//                    proj.localNPCHitCooldown = 15; // 本地 NPC 命中冷却时间
//                    proj.usesLocalNPCImmunity = true; // 启用本地无敌帧
//                }
//            }
//        }

//    }
//}