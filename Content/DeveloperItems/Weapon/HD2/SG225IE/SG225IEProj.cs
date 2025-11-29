//using CalamityMod;
//using Microsoft.Xna.Framework;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Terraria.DataStructures;
//using Terraria.ID;
//using Terraria.ModLoader;
//using Terraria;
//using CalamityMod.Particles;
//using CalamityMod.Projectiles.Typeless;
//using Terraria.Audio;
//using Terraria.Graphics.Renderers;

//namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.SG225IE
//{
//    public class SG225IEProj : ModProjectile, ILocalizedModType
//    {
//        public new string LocalizationCategory => "DeveloperItems.SG225IE";

//        //public override string Texture => "CalamityMod/Projectiles/InvisibleProj"; // 使用完全透明贴图
//        public override void SetStaticDefaults()
//        {
//            // 设置弹幕拖尾长度和模式
//            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
//            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
//        }

//        public override bool PreDraw(ref Color lightColor)
//        {
//            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
//            return false;
//        }
//        public override Color? GetAlpha(Color lightColor)
//        {
//            return new Color(50, 255, 50, Projectile.alpha);
//        }
//        public override void SetDefaults()
//        {
//            Projectile.width = Projectile.height = 15;
//            Projectile.friendly = true;
//            Projectile.hostile = false;
//            Projectile.DamageType = DamageClass.Ranged;
//            Projectile.penetrate = 6; // 可击中次数
//            Projectile.timeLeft = 300;
//            Projectile.ignoreWater = true;
//            Projectile.tileCollide = true;
//            Projectile.extraUpdates = 4; // 可调节飞行平滑度
//            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
//            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
//        }


//        public override void OnSpawn(IEntitySource source)
//        {
         
//        }


//        public override void AI()
//        {
//            // 调整旋转方向
//            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

//            // 🔥火焰尘埃
//            if (Main.rand.NextBool(3))
//            {
//                Dust flame = Dust.NewDustPerfect(
//                    Projectile.Center + Main.rand.NextVector2Circular(4f, 4f),
//                    DustID.Torch,
//                    -Projectile.velocity * 0.2f + Main.rand.NextVector2Circular(0.5f, 0.5f),
//                    100,
//                    Color.OrangeRed,
//                    Main.rand.NextFloat(1f, 1.6f)
//                );
//                flame.noGravity = true;
//            }

//            // ✨火星粒子（Calamity spark）
//            if (Main.rand.NextBool(5))
//            {
//                var spark = new SparkParticle(
//                    Projectile.Center,
//                    Main.rand.NextVector2Circular(0.5f, 0.5f),
//                    false, 12, 0.5f, Color.OrangeRed
//                );
//                GeneralParticleHandler.SpawnParticle(spark);
//            }
//        }
//        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
//        {
//            // ✂️ 每次命中目标后永久降低当前弹幕的伤害乘数
//            Projectile.damage = (int)(Projectile.damage * 0.7f);
//        }
//        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
//        {
//            // 🔥添加多个燃烧debuff
//            target.AddBuff(BuffID.OnFire, 240);
//            target.AddBuff(BuffID.CursedInferno, 180);
//            target.AddBuff(BuffID.Frostburn2, 180);

//            // 💥击中特效：小爆炸
//            for (int i = 0; i < 6; i++)
//            {
//                Dust.NewDust(Projectile.Center, 10, 10, DustID.Torch, Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
//            }

//            // 🎧命中音效
//            SoundEngine.PlaySound(SoundID.Item74, Projectile.Center);
//        }

//        public override void OnKill(int timeLeft)
//        {
//            // 🔥火焰尘埃四散
//            for (int i = 0; i < 12; i++)
//            {
//                Dust.NewDust(
//                    Projectile.Center,
//                    10, 10,
//                    DustID.Torch,
//                    Main.rand.NextFloat(-2.4f, 2.4f),
//                    Main.rand.NextFloat(-2.4f, 2.4f));
//            }

//            // 🌫️外圈大烟雾（大而稀疏）
//            for (int i = 0; i < 5; i++)
//            {
//                Vector2 offset = Main.rand.NextVector2CircularEdge(12f, 12f);
//                Particle outerSmoke = new HeavySmokeParticle(
//                    Projectile.Center + offset,
//                    offset * 0.08f,
//                    Color.WhiteSmoke,
//                    20,
//                    Main.rand.NextFloat(1.2f, 1.8f),
//                    0.3f,
//                    Main.rand.NextFloat(-1f, 1f),
//                    false
//                );
//                GeneralParticleHandler.SpawnParticle(outerSmoke);
//            }

//            // 🌫️内圈小烟雾（紧贴弹幕中心）
//            for (int i = 0; i < 4; i++)
//            {
//                Vector2 offset = Main.rand.NextVector2Circular(5f, 5f);
//                Particle innerSmoke = new HeavySmokeParticle(
//                    Projectile.Center + offset,
//                    offset * 0.05f,
//                    Color.WhiteSmoke,
//                    18,
//                    Main.rand.NextFloat(0.9f, 1.3f),
//                    0.35f,
//                    Main.rand.NextFloat(-1f, 1f),
//                    false
//                );
//                GeneralParticleHandler.SpawnParticle(innerSmoke);
//            }

//        }





//    }
//}
