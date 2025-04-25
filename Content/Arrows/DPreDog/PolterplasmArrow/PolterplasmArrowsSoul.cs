using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityRangerExpansion.CREConfigs;

namespace CalamityRangerExpansion.Content.Arrows.DPreDog.PolterplasmArrow
{
    public class PolterplasmArrowsSoul : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectile.DPreDog";

        private const int TimeLeft = 660;
        private float HomingBuff = 1;
        public ref float Time => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.alpha = 100;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = TimeLeft;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
        }

        public override void AI()
        {
            if (HomingBuff > 0)
                HomingBuff -= 0.01f;

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
                Projectile.frame = 0;

            Lighting.AddLight(Projectile.Center, 0.5f, 0.2f, 0.9f);

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {

                if (Projectile.timeLeft % 2 == 0 && Projectile.timeLeft < TimeLeft - 10)
                {
                    SparkParticle spark = new SparkParticle(Projectile.Center + Main.rand.NextVector2Circular(15, 15) - Projectile.velocity.SafeNormalize(Vector2.UnitX) * 10, -Projectile.velocity * Main.rand.NextFloat(0.5f, 1.5f), false, Main.rand.Next(9, 12 + 1), 0.4f, Color.Plum);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }


            // 前30帧不追踪，之后开始追踪敌人
            if (Projectile.ai[1] > 30)
            {
                NPC target = Projectile.Center.ClosestNPCAt(1800); // 查找范围内最近的敌人
                if (target != null)
                {
                    Vector2 direction = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * 18f, 0.08f); // 追踪速度为18f
                }
            }
            else
            {
                Projectile.ai[1]++;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            Time++;
        }

        public override bool? CanDamage() => Time >= 15f; // 初始的时候不会造成伤害，直到15为止

        public override bool PreDraw(ref Color lightColor)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                if (Projectile.timeLeft > TimeLeft - 5)
                    return false;

                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
                return false;
            }
            return true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.timeLeft < 85)
            {
                byte b2 = (byte)(Projectile.timeLeft * 3);
                byte a2 = (byte)(100f * ((float)b2 / 255f));
                return new Color((int)b2, (int)b2, (int)b2, (int)a2);
            }
            return new Color(255, 255, 255, 100);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.numHits > 0)
                Projectile.damage = (int)(Projectile.damage * 0.88f);
            if (Projectile.damage < 1)
                Projectile.damage = 1;
        }

        public override void OnKill(int timeLeft)
        {
        
            SoundEngine.PlaySound(VoidEdge.ProjectileDeathSound with { Pitch = -0.3f }, Projectile.Center);

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                int points = 25;
                float radians = MathHelper.TwoPi / points;
                Vector2 spinningPoint = Vector2.Normalize(new Vector2(-1f, -1f));
                float rotRando = Main.rand.NextFloat(0.1f, 2.5f);
                for (int k = 0; k < points; k++)
                {
                    Vector2 velocity = spinningPoint.RotatedBy(radians * k).RotatedBy(-0.45f * rotRando);
                    LineParticle subTrail = new LineParticle(Projectile.Center + velocity * 20.5f, velocity * 15, false, 30, 0.75f, Color.Plum);
                    GeneralParticleHandler.SpawnParticle(subTrail);
                }
                for (int k = 0; k < 17; k++)
                {
                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center, 66, new Vector2(14, 14).RotatedByRandom(100) * Main.rand.NextFloat(0.3f, 1.8f));
                    dust2.scale = Main.rand.NextFloat(1.15f, 1.45f);
                    dust2.noGravity = true;
                    dust2.color = Color.Plum;
                }
            }

            #region 旧代码

            //// 添加生成 PolterplasmArrowFlower 的逻辑
            //int extraProjectileCount = 5; // 生成的数量
            //float speedMultiplier = 1.1f; // 初始速度倍率
            //for (int i = 0; i < extraProjectileCount; i++)
            //{
            //    // 随机生成圆周上的位置
            //    Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2Circular(50, 50); // 50 为生成的半径
            //                                                                                  // 指向主弹幕中心的方向
            //    Vector2 direction = (Projectile.Center - spawnPos).SafeNormalize(Vector2.Zero);
            //    // 生成新弹幕
            //    Projectile.NewProjectile(
            //        Projectile.GetSource_FromThis(),
            //        spawnPos,
            //        direction * Projectile.velocity.Length() * speedMultiplier, // 速度为当前弹幕速度的 1.1 倍
            //        ModContent.ProjectileType<PolterplasmArrowFlower>(), // 替换为实际弹幕类型
            //        (int)(Projectile.damage * 5.0f), // 伤害倍率为 5 倍
            //        Projectile.knockBack, // 使用当前弹幕的击退值
            //        Main.myPlayer
            //    );
            //}

            #endregion
        }
    }
}
