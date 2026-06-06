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
//using Terraria.Audio;
//using CalamityMod.Items.Weapons.Ranged;

//namespace CalamityRangerExpansion.Content.BOWChange.BPrePlantera.ArbalestC
//{
//    internal class ArbalestPROJ : ModProjectile, ILocalizedModType
//    {
//        public new string LocalizationCategory => "Weapons.BPrePlantera";
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
//            Projectile.timeLeft = 600; // 弹幕存在时间为600帧
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

//            // 每2帧生成两个GoldCoin Dust
//            if (Projectile.timeLeft % 2 == 0)
//            {
//                Vector2 forwardPoint = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 20f;
//                for (int i = -1; i <= 1; i += 2) // -1 和 1，生成两个 Dust
//                {
//                    Dust dust = Dust.NewDustPerfect(forwardPoint, 246, new Vector2(i * 1.5f, 0f), 0, default, 1f);
//                    dust.noGravity = true;
//                    dust.velocity *= 0.8f; // 控制Dust速度
//                }
//            }
//        }

//        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
//        {
          
//        }
//        public override void OnKill(int timeLeft)
//        {
//            // 正中头彩！
//            SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/SevensStrikerCoinShot"));

//            // 散射弹幕逻辑
//            int scatterCount = Main.rand.Next(1, 4); // 随机生成1到3个弹幕
//            for (int i = 0; i < scatterCount; i++)
//            {
//                // 基于正上方方向随机扩散
//                float angle = MathHelper.ToRadians(20) * (Main.rand.NextFloat() - 0.5f) * 2; // 随机范围内的角度
//                Vector2 scatterVelocity = -Vector2.UnitY.RotatedBy(angle) * 7.5f; // 正上方扩散

//                Projectile.NewProjectile(
//                    Projectile.GetSource_FromThis(),
//                    Projectile.Center,
//                    scatterVelocity,
//                    ModContent.ProjectileType<ArbalestCOIN>(), // 替换为散射的弹幕类型
//                    (int)(Projectile.damage * 0.33f),
//                    Projectile.knockBack,
//                    Projectile.owner
//                );
//            }
//        }
//    }
//}