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
//using Terraria.DataStructures;

//namespace CalamityRangerExpansion.Content.BOWChange.CPreMoodLord.VernalBolterC.废弃
//{
//    internal class VBTerra : ModProjectile, ILocalizedModType
//    {
//        public new string LocalizationCategory => "Weapons.CPreMoodLord";
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
//            Projectile.timeLeft = 300; // 弹幕存在时间为x帧
//            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
//            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
//            Projectile.ignoreWater = true; // 弹幕不受水影响
//            Projectile.arrow = true;
//            Projectile.extraUpdates = 7;
//        }
//        private bool hasBounced = false; // 记录是否已经反弹过一次
//        private float dustAngle = 0f; // 控制粒子生成的弯曲角度
//        private bool growing = true;  // 控制 dustAngle 的增减方向
//        private float variance = 1f;  // 控制角度变化的随机幅度
//        public override void OnSpawn(IEntitySource source)
//        {
//            Projectile.velocity *= 0.4f;
//        }
//        public override void AI()
//        {
//            // 调整弹幕的旋转，使其在飞行时保持水平
//            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;

//            // Lighting - 添加天蓝色光源，光照强度为 0.49
//            Lighting.AddLight(Projectile.Center, Color.LightSkyBlue.ToVector3() * 0.49f);

//            // 控制 dustAngle 的增减
//            if (dustAngle <= -0.5f)
//            {
//                growing = true;
//            }
//            if (dustAngle >= 0.5f)
//            {
//                growing = false;
//            }
//            dustAngle += growing ? 0.07f * variance : -0.07f * variance;

//            // 粒子生成逻辑
//            if (Projectile.localAI[0] > 12f && Projectile.Distance(Main.player[Projectile.owner].Center) < 1200)
//            {
//                // 创建红色的 GlowOrbParticle 粒子
//                GlowOrbParticle orb = new GlowOrbParticle(
//                    Projectile.Center + Projectile.velocity.RotatedBy(dustAngle) * 4.5f - Projectile.velocity * 5,
//                    Vector2.Zero, false, 5, 0.55f + MathF.Abs(dustAngle * 0.5f),
//                    Color.Red, true, true
//                );
//                GeneralParticleHandler.SpawnParticle(orb);

//                // 创建翠绿色的 PointParticle 粒子（火花）
//                PointParticle spark = new PointParticle(
//                    Projectile.Center + Projectile.velocity * 3.5f,
//                    Projectile.velocity, false, 2, 0.6f, Color.LimeGreen
//                );
//                GeneralParticleHandler.SpawnParticle(spark);
//            }

//            Projectile.localAI[0]++;
//        }

//        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
//        {

//        }
//        public override void OnKill(int timeLeft)
//        {
//            // 定义四个方向的单位向量
//            Vector2[] directions = new Vector2[]
//            {
//        new Vector2(0, -1), // 正上
//        new Vector2(0, 1),  // 正下
//        new Vector2(-1, 0), // 正左
//        new Vector2(1, 0)   // 正右
//            };

//            foreach (var direction in directions)
//            {
//                // 创建 VBTerraSPIT 弹幕
//                int spitProjectile = Projectile.NewProjectile(
//                    Projectile.GetSource_FromThis(),
//                    Projectile.Center,
//                    direction * (Projectile.velocity.Length() * 2.5f), // 初始速度是自身速度的 2.5 倍
//                    ModContent.ProjectileType<VBTerraSPIT>(),
//                    (int)(Projectile.damage * 0.33f), // 伤害倍率为 0.33
//                    Projectile.knockBack,
//                    Projectile.owner
//                );

//                // 进一步设置弹幕属性（如有需要）
//                if (spitProjectile.WithinBounds(Main.maxProjectiles))
//                {
//                    Projectile proj = Main.projectile[spitProjectile];
//                    proj.friendly = true;
//                    proj.hostile = false;
//                }
//            }
//        }
//    }
//}