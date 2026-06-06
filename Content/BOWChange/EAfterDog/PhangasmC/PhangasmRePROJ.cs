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

//namespace CalamityRangerExpansion.Content.BOWChange.EAfterDog.PhangasmC
//{
//    internal class PhangasmRePROJ : ModProjectile, ILocalizedModType
//    {
//        public new string LocalizationCategory => "Weapons.EAfterDog";
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
//            Projectile.penetrate = 3; // 穿透力
//            Projectile.timeLeft = 350; // 弹幕存在时间为x帧
//            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
//            Projectile.localNPCHitCooldown = 20; // 无敌帧冷却时间
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

//            //if (Projectile.timeLeft % 2 == 0) // 每两帧执行一次
//            //{
//            //    Color smokeColor = Main.rand.NextBool() ? Color.LightBlue : Color.DeepSkyBlue;
//            //    Particle smoke = new HeavySmokeParticle(
//            //        Projectile.Center,
//            //        Projectile.velocity * 0.5f, // 缓慢移动的烟雾
//            //        smokeColor,
//            //        20, // 粒子存活时间
//            //        Projectile.scale * 0.5f * Main.rand.NextFloat(0.6f, 1.2f), // 缩放为原始大小的0.5倍
//            //        0.8f,
//            //        MathHelper.ToRadians(3f),
//            //        required: true
//            //    );
//            //    GeneralParticleHandler.SpawnParticle(smoke);
//            //}

//            Projectile.StickyProjAI(15);
//            if (Projectile.ai[0] == 2f)
//            {
//                Projectile.velocity *= 0f;
//            }
//        }


//        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
//        {
//            //for (int i = 0; i < Main.rand.Next(2, 5); i++) // 生成2~4个粒子
//            //{
//            //    float angle = MathHelper.ToRadians(Main.rand.NextFloat(0f, 360f)); // 随机角度
//            //    float angleOffset = MathHelper.ToRadians(Main.rand.NextFloat(-30f, 30f)); // 随机偏移
//            //    Vector2 spawnDirection = new Vector2((float)Math.Cos(angle + angleOffset), (float)Math.Sin(angle + angleOffset));
//            //    Vector2 velocity = spawnDirection * Main.rand.NextFloat(3f, 7f); // 随机速度范围

//            //    GeneralParticleHandler.SpawnParticle(new GenericBloom(
//            //        Projectile.Center, // 粒子生成位置
//            //        velocity * 0.85f, // 调整粒子速度
//            //        Color.LightBlue, // 粒子颜色
//            //        0.25f, // 粒子透明度
//            //        Main.rand.Next(20) + 10 // 粒子存活时间
//            //    ));
//            //}

//            // 生成六边形的 Dust 链特效
//            for (int i = 0; i < 6; i++) // 六条链
//            {
//                float angle = MathHelper.TwoPi / 6 * i; // 每条链的角度间隔
//                Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)); // 计算方向向量

//                // 每条链包含 4 个粒子
//                for (int j = 0; j < 4; j++)
//                {
//                    float speed = 1f + j; // 速度逐步增加
//                    float scale = 0.7f + j * 0.1f; // 大小逐步增加
//                    Vector2 velocity = direction * speed;

//                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.BlueFlare, velocity, 100, default, scale);
//                    dust.noGravity = true; // 禁用重力效果
//                }
//            }
//        }

//        public override void OnKill(int timeLeft)
//        {
//            Player player = Main.player[Projectile.owner];

//            // 计算发射方向
//            Vector2 fireDirection = Projectile.velocity.SafeNormalize(Vector2.UnitX);

//            // 生成 x 发随机方向的电能粒子特效
//            for (int i = 0; i < 6; i++)
//            {
//                Vector2 randomDirection = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f)).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(3f, 6f);
//                // 生成粒子特效（假设使用电能粒子类型）
//                Color electricColor = Color.Cyan; // 这里可以自定义颜色
//                Particle electricParticle = new SparkParticle(Projectile.Center, randomDirection, false, 60, Main.rand.NextFloat(0.8f, 1.2f), electricColor);
//                GeneralParticleHandler.SpawnParticle(electricParticle);
//            }
//        }


//        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
//        {
//            // 调用原始的ModifyHitNPCSticky方法，确保粘附逻辑正常
//            Projectile.ModifyHitNPCSticky(20);
//            Projectile.alpha = 255;
//            Projectile.damage = (int)(Projectile.damage * 0.8f);
//        }
//        //public override bool OnTileCollide(Vector2 oldVelocity)
//        //{
//        //    Projectile.ai[0] = 2f;
//        //    Projectile.timeLeft = 300;
//        //    Projectile.alpha = 255;
//        //    return false;
//        //}
//    }
//}