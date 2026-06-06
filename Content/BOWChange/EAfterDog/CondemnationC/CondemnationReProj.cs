using CalamityMod.Graphics.Metaballs;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Typeless;
using CalamityMod;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.EAfterDog.CondemnationC
{
    internal class CondemnationReProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.EAfterDog";

        // 当前从 Holdout 中传入的阶段值（范围：0~5）
        public int WeaponStage = 0;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 500;
            Projectile.penetrate = 2;
            Projectile.extraUpdates = 2; // 可调节飞行平滑度
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
        }

        public override void OnSpawn(IEntitySource source)
        {
            WeaponStage = (int)MathHelper.Clamp(Projectile.ai[0], 0f, 5f);

            // 3级及以上在出生点释放一个超大爆炸弹幕 Fuckyou
            if (WeaponStage >= 3)
            {
                Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<FuckYou>(),
                    (int)(Projectile.damage * 2f), // 伤害倍率2.0
                    0f,
                    Projectile.owner,
                    2f // scale 参数，Fuckyou使用它控制爆炸范围
                );
            }
        }
        private bool penetratedSet = false;

        public override void AI()
        {
            // ?只在第一帧根据 WeaponStage 设置穿透力
            if (!penetratedSet)
            {
                Projectile.penetrate = WeaponStage switch
                {
                    >= 5 => -1,
                    >= 4 => 7,
                    >= 2 => 3,
                    _ => 1
                };
                penetratedSet = true;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            //  + MathHelper.PiOver4
            BowChangeVFX.SpawnTrail(Projectile, BowChangeTheme.Condemnation, 0.55f + WeaponStage * 0.12f);



            {
                // ?1?? 血红魔能 Dust（随阶段增强）
                int dustCount = 1 + WeaponStage;
                for (int i = 0; i < dustCount; i++)
                {
                    Dust dust = Dust.NewDustPerfect(
                        Projectile.Center + Main.rand.NextVector2Circular(4f, 4f),
                        267, // 魔能 Dust
                        Projectile.velocity * 0.2f + Main.rand.NextVector2Circular(1f, 1f),
                        100,
                        Color.Lerp(Color.Crimson, Color.Purple, Main.rand.NextFloat()),
                        1.1f + WeaponStage * 0.1f
                    );
                    dust.noGravity = true;
                }

                // ?2?? Spark 痛感尾迹（2级开启）
                if (WeaponStage >= 2 && Main.rand.NextBool(6 - WeaponStage)) // 越高越频繁
                {
                    Vector2 sparkVel = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(0.4f) * Main.rand.NextFloat(1.5f, 3f);
                    Particle trail = new SparkParticle(
                        Projectile.Center,
                        sparkVel,
                        false,
                        40 + WeaponStage * 5,
                        1.1f,
                        Color.Lerp(Color.DarkRed, Color.Purple, Main.rand.NextFloat(0.4f, 0.8f))
                    );
                    GeneralParticleHandler.SpawnParticle(trail);
                }

                //// ?3?? 恶意烟雾（4级以上开启）
                //if (WeaponStage >= 4 && Main.rand.NextBool(8))
                //{
                //    Particle smoke = new HeavySmokeParticle(
                //        Projectile.Center,
                //        Projectile.velocity.RotatedByRandom(0.5f) * 0.4f,
                //        new Color(100, 10, 10, 100), // 深血红色
                //        20,
                //        Main.rand.NextFloat(0.9f, 1.4f),
                //        0.25f,
                //        Main.rand.NextFloat(-1f, 1f),
                //        false
                //    );
                //    GeneralParticleHandler.SpawnParticle(smoke);
                //}

                // ?4?? 光照：持续紫红光晕
                Lighting.AddLight(Projectile.Center, Color.Violet.ToVector3() * 0.5f + Color.Red.ToVector3() * 0.2f);
            }


        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 击中时在原地生成爆炸弹幕
            float scale = 1f;
            float damageMultiplier = 1f;

            // 设置倍率与缩放
            if (WeaponStage >= 5)
            {
                scale = 1f;
                damageMultiplier = 3.5f;
            }
            else if (WeaponStage >= 4)
            {
                scale = 1f;
                damageMultiplier = 1.6f;
            }
            else if (WeaponStage >= 2)
            {
                scale = 1f;
                damageMultiplier = 0.5f;
            }

            if (WeaponStage >= 2)
            {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<FuckYou>(),
                    (int)(Projectile.damage * damageMultiplier),
                    0f,
                    Projectile.owner,
                    scale // 作为scale传递（Fuckyou内部使用 ai[0] 或其他字段接收）
                );

                // ?1?? 播放爆炸音效
                SoundEngine.PlaySound(SoundID.Item14.WithVolumeScale(1.0f).WithPitchOffset(0.1f), Projectile.Center);

           

            }

            BowChangeVFX.SpawnImpact(Projectile, BowChangeTheme.Condemnation, 0.85f + WeaponStage * 0.16f);

            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    target.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<CondemnationReJudgementBrand>(),
                    (int)(Projectile.damage * (0.38f + WeaponStage * 0.08f)),
                    Projectile.knockBack,
                    Projectile.owner,
                    WeaponStage,
                    target.whoAmI);
            }



            {
                // ?2?? 点刺型粒子（痛觉反馈）
                for (int i = 0; i < 8; i++)
                {
                    Vector2 offset = Main.rand.NextVector2Circular(2f, 2f);
                    Vector2 vel = -Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedByRandom(0.5f) * Main.rand.NextFloat(1f, 3f);
                    Color color = Color.Lerp(Color.DarkRed, Color.MediumVioletRed, Main.rand.NextFloat());

                    PointParticle p = new PointParticle(
                        Projectile.Center + offset,
                        vel,
                        false,
                        18,
                        1.3f,
                        color
                    );
                    GeneralParticleHandler.SpawnParticle(p);
                }

                // ?3?? 十字星散弹（锐利高亮）
                for (int i = 0; i < 4; i++)
                {
                    Vector2 sparkVel = Main.rand.NextVector2CircularEdge(1f, 1f) * 6f;
                    Color start = Color.White;
                    Color end = Color.Red;

                    CritSpark star = new CritSpark(
                        Projectile.Center,
                        sparkVel,
                        start,
                        end,
                        0.9f,
                        20
                    );
                    GeneralParticleHandler.SpawnParticle(star);
                }

                // ?4?? 魔咒血尘（中心散开）
                for (int i = 0; i < 20; i++)
                {
                    Vector2 dustVel = Main.rand.NextVector2Circular(4f, 4f);
                    Dust blood = Dust.NewDustPerfect(
                        Projectile.Center,
                        267, // 魔能 Dust
                        dustVel,
                        100,
                        Color.Lerp(Color.Crimson, Color.Purple, Main.rand.NextFloat()),
                        Main.rand.NextFloat(1.2f, 1.8f)
                    );
                    blood.noGravity = true;
                }
            }


        }
        public override void OnKill(int timeLeft)
        {
            BowChangeVFX.SpawnImpact(Projectile, BowChangeTheme.Condemnation, 0.75f + WeaponStage * 0.12f);

            for (int i = 0; i < 36; i++)
            {
                float angle = MathHelper.TwoPi * i / 36f;
                Vector2 offset = angle.ToRotationVector2() * 32f;
                Vector2 pos = Projectile.Center + offset;

                Dust dust = Dust.NewDustPerfect(
                    pos,
                    267,
                    offset.RotatedBy(MathHelper.PiOver2) * 0.5f,
                    100,
                    Color.Lerp(Color.Crimson, Color.Purple, Main.rand.NextFloat()),
                    1.6f
                );
                dust.noGravity = true;
            }




            for (int i = 0; i < 6; i++)
            {
                float angle = MathHelper.TwoPi * i / 6f;
                Vector2 vel = angle.ToRotationVector2() * 5f;

                CritSpark spark = new CritSpark(
                    Projectile.Center,
                    vel,
                    Color.Red,
                    Color.DarkRed,
                    1.2f,
                    20
                );
                GeneralParticleHandler.SpawnParticle(spark);
            }
            for (int i = 0; i < 8; i++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(3f, 3f);
                Particle smoke = new HeavySmokeParticle(
                    Projectile.Center,
                    vel,
                    new Color(100, 20, 20, 100),
                    24,
                    Main.rand.NextFloat(1.0f, 1.4f),
                    0.3f,
                    Main.rand.NextFloat(-0.2f, 0.2f),
                    false
                );
                GeneralParticleHandler.SpawnParticle(smoke);
            }

        }





    }
}



