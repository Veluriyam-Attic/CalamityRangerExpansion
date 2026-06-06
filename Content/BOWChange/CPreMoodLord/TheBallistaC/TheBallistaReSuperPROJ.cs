using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Projectiles.Ranged;
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
using CalamityMod.Particles;

namespace CalamityRangerExpansion.Content.BOWChange.CPreMoodLord.TheBallistaC
{
    internal class TheBallistaReSuperPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.CPreMoodLord";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // 画残影效果
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
        public override void SetDefaults()
        {
            // 设置弹幕的基础属性
            Projectile.width = 11; // 弹幕宽度
            Projectile.height = 24; // 弹幕高度
            Projectile.friendly = true; // 对敌人有效
            Projectile.DamageType = DamageClass.Ranged; // 远程伤害类型
            Projectile.penetrate = -1; // 穿透力为1，击中一个敌人就消失
            Projectile.timeLeft = 300; // 弹幕存在时间为x帧
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
            Projectile.ignoreWater = true; // 弹幕不受水影响
            Projectile.arrow = true;
            Projectile.extraUpdates = 6;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;
            Lighting.AddLight(Projectile.Center, Color.LightSkyBlue.ToVector3() * 0.49f);

            // ??夸张强化尾迹（土色版）：土壤+石粉+灰金火花
            if (Projectile.ai[0]++ > 6f)
            {
                Vector2 basePos = Projectile.Center;
                Vector2 backDir = -Projectile.velocity.SafeNormalize(Vector2.UnitY);

                int segmentCount = 3;
                int particlesPerRing = 4;   

                for (int seg = 0; seg < segmentCount; seg++)
                {
                    float radius = 6f + seg * 8f;
                    for (int i = 0; i < particlesPerRing; i++)
                    {
                        float angle = MathHelper.TwoPi * i / particlesPerRing + Main.rand.NextFloat(-0.05f, 0.05f);
                        Vector2 offset = angle.ToRotationVector2() * radius;

                        Vector2 spawnPos = basePos + offset;
                        Vector2 velocity = backDir * (1.2f + seg * 0.5f) + Main.rand.NextVector2Circular(0.4f, 0.4f);

                        // ? 主体：暗土尘雾（橄榄褐色）
                        Dust d = Dust.NewDustPerfect(spawnPos, DustID.Dirt, velocity, 0, new Color(120, 100, 60), Main.rand.NextFloat(1.2f, 1.8f));
                        d.noGravity = true;
                        d.alpha = 100;

                        // ? 第二层：灰褐闪尾
                        if (seg == 1 && i % 4 == 0)
                        {
                            Particle trail = new SparkParticle(
                                spawnPos,
                                velocity * 0.8f,
                                false,
                                30,
                                1.15f,
                                Color.Lerp(new Color(140, 120, 70), Color.DarkOliveGreen, 0.5f) // 土金色混合
                            );
                            GeneralParticleHandler.SpawnParticle(trail);
                        }
                    }
                }
            }




            // 额外：超速高亮火花与闪电
            if (Main.rand.NextBool(2))
            {
                Vector2 v = Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedByRandom(MathHelper.PiOver4) * Main.rand.NextFloat(4f, 8f);
                Particle spark = new SparkParticle(
                    Projectile.Center,
                    v,
                    false,
                    30,
                    1.2f,
                    Color.LightYellow
                );
                GeneralParticleHandler.SpawnParticle(spark);
            }

            // 烟气感重型拖尾
            if (Main.rand.NextBool(3))
            {
                Vector2 v = -Projectile.velocity * 0.3f + Main.rand.NextVector2Circular(1.5f, 1.5f);
                Particle smoke = new HeavySmokeParticle(
                    Projectile.Center,
                    v,
                    Color.LightBlue * 0.7f,
                    40,
                    Main.rand.NextFloat(0.9f, 1.3f),
                    0.35f,
                    Main.rand.NextFloat(-0.1f, 0.1f),
                    true
                );
                GeneralParticleHandler.SpawnParticle(smoke);
            }
        }

        public override void OnSpawn(IEntitySource source)
        {



        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 添加破碎debuff
            target.AddBuff(ModContent.BuffType<Crumbling>(), 300);

            // 多重爆炸音效，堆叠气势
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item62 with { Pitch = -0.3f }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballShot, Projectile.Center);

            // 爆炸特效（主要火花 + 烟雾 + 电闪 + 扩散）
            Vector2 center = Projectile.Center;

            // —— 深橙能量粒子
            for (int i = 0; i < 50; i++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(16f, 16f);
                Dust d = Dust.NewDustPerfect(center, DustID.LavaMoss, vel, 0, Color.Orange * 0.9f, Main.rand.NextFloat(1.6f, 2.2f));
                d.noGravity = true;
            }

            // —— 熔金电流火点刺
            for (int i = 0; i < 30; i++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(10f, 10f);
                Particle point = new PointParticle(center, vel, false, 32, 1.3f, Color.OrangeRed);
                GeneralParticleHandler.SpawnParticle(point);
            }

            // —— 核熔重烟雾
            for (int i = 0; i < 32; i++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(4f, 4f);
                Particle smoke = new HeavySmokeParticle(center, vel, Color.Black, 60, 1.7f, 0.45f, 0f, true);
                GeneralParticleHandler.SpawnParticle(smoke);
            }

            // —— 闪光脉冲能量线
            for (int i = 0; i < 18; i++)
            {
                Vector2 vel = Main.rand.NextVector2CircularEdge(20f, 20f) * Main.rand.NextFloat(1.8f, 3.5f);
                AltSparkParticle spark = new AltSparkParticle(center, vel, false, 30, 1.1f, Color.Lerp(Color.Yellow, Color.Orange, 0.6f));
                GeneralParticleHandler.SpawnParticle(spark);
            }

        }


        public override void OnKill(int timeLeft)
        {
            float shakePower = 5f;
            float distanceFactor = Utils.GetLerpValue(1000f, 0f, Projectile.Distance(Main.LocalPlayer.Center), true);
            Main.LocalPlayer.Calamity().GeneralScreenShakePower = Math.Max(Main.LocalPlayer.Calamity().GeneralScreenShakePower, shakePower * distanceFactor);

     


        }





    }
}