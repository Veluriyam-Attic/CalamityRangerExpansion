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
//using CalamityMod.Buffs.DamageOverTime;
//using CalamityMod.Items.Weapons.Ranged;
//using Terraria.Audio;

//namespace CalamityRangerExpansion.Content.BOWChange.DPreDog.DaemonsFlameC
//{
//    internal class DaemonsFlameRePROJLazer : ModProjectile, ILocalizedModType
//    {
//        public new string LocalizationCategory => "Weapons.DPreDog";
//        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

//        public override void SetStaticDefaults()
//        {
//            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 35;
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
//            Projectile.timeLeft = 100; // 弹幕存在时间为x帧
//            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
//            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
//            Projectile.ignoreWater = true; // 弹幕不受水影响
//            Projectile.arrow = true;
//            Projectile.extraUpdates = 5;
//        }

//        public override void AI()
//        {
//            // 调整弹幕的旋转，使其在飞行时保持水平
//            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;

//            // Lighting - 添加天蓝色光源，光照强度为 0.49
//            Lighting.AddLight(Projectile.Center, Color.LightSkyBlue.ToVector3() * 0.49f);

//            // 刚生成时释放几个天蓝色的小圆圈往外扩散
//            if (Projectile.ai[0] == 0f)
//            {
//                for (int i = 0; i < 3; i++)
//                {
//                    Particle pulse = new DirectionalPulseRing(Projectile.Center, Projectile.velocity * 0.75f, Color.LightSkyBlue, new Vector2(1f, 2.5f), Projectile.rotation, 0.2f, 0.03f, 20);
//                    GeneralParticleHandler.SpawnParticle(pulse);
//                }

//                for (int i = 0; i <= 5; i++)
//                {
//                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.BlueTorch, Projectile.velocity);
//                    dust.color = Color.LightSkyBlue;
//                    dust.scale = Main.rand.NextFloat(1.6f, 2.5f);
//                    dust.velocity = Projectile.velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.3f, 1.6f);
//                    dust.noGravity = true;
//                }
//            }

//            // 为箭矢本体后面添加天蓝色光束特效
//            if (Projectile.numUpdates % 3 == 0)
//            {
//                Color outerSparkColor = Color.LightSkyBlue; // 天蓝色
//                float scaleBoost = MathHelper.Clamp(Projectile.ai[0] * 0.005f, 0f, 2f);
//                float outerSparkScale = 1.2f + scaleBoost;
//                SparkParticle spark = new SparkParticle(Projectile.Center, Projectile.velocity, false, 7, outerSparkScale, outerSparkColor);
//                GeneralParticleHandler.SpawnParticle(spark);
//            }
//        }

//        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
//        {
//            // 添加两条随机方向的蓝色线性粒子
//            Color electricColor = Color.LightSkyBlue;
//            for (int i = 0; i < 2; i++)
//            {
//                Vector2 randomDirection = Main.rand.NextVector2Circular(1f, 1f);
//                Particle electricParticle = new SparkParticle(target.Center, randomDirection * 2f, false, 60, Main.rand.NextFloat(0.8f, 1.2f), electricColor);
//                GeneralParticleHandler.SpawnParticle(electricParticle);
//            }

//            target.AddBuff(ModContent.BuffType<Nightwither>(), 450);
//            SoundEngine.PlaySound(HalleysInferno.Hit, Projectile.Center);
//        }

//        public override void OnKill(int timeLeft)
//        {
//            // 在结束时释放几个天蓝色的小圆圈特效
//            for (int i = 0; i < 3; i++)
//            {
//                Particle pulse = new DirectionalPulseRing(Projectile.Center, Projectile.velocity * 0.75f, Color.LightSkyBlue, new Vector2(1f, 2.5f), Projectile.rotation, 0.2f, 0.03f, 20);
//                GeneralParticleHandler.SpawnParticle(pulse);
//            }

//            // 在结束时释放天蓝色的小型特效粒子
//            for (int i = 0; i <= 5; i++)
//            {
//                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.GoldFlame, Projectile.velocity);
//                dust.color = Color.LightSkyBlue;
//                dust.scale = Main.rand.NextFloat(1.35f, 2.1f);
//                dust.velocity = Projectile.velocity.RotatedByRandom(0.06f) * Main.rand.NextFloat(0.8f, 3.1f);
//                dust.noGravity = true;
//            }

//            // 消亡时释放天蓝色爆炸特效
//            Particle blastRing = new CustomPulse(
//                Projectile.Center, Vector2.Zero, Color.LightSkyBlue,
//                "CalamityMod/Particles/FlameExplosion",
//                Vector2.One * 0.33f, Main.rand.NextFloat(-10f, 10f),
//                0.07f, 0.33f, 30
//            );
//            GeneralParticleHandler.SpawnParticle(blastRing);
//        }
//    }
//}