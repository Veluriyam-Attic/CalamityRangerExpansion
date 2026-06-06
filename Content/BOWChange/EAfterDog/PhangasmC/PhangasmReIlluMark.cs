using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria.Audio;

namespace CalamityRangerExpansion.Content.BOWChange.EAfterDog.PhangasmC
{
    internal class PhangasmReIlluMark : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.EAfterDog";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj"; // 使用完全透明贴图

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 20; // 范围型爆炸判定大小
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300; // 存活时间短
            Projectile.extraUpdates = 3;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 600; // 防止重复命中
            Projectile.alpha = 255; // 完全透明
        }

        public override void AI()
        {
            // 每 5 帧生成一次小型视觉扩散
            if (Projectile.timeLeft % 5 == 0)
            {
                for (int i = 0; i < 15; i++)
                {
                    Vector2 vel = Main.rand.NextVector2Circular(1.8f, 1.8f);
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.BlueCrystalShard, vel, 100,
                        Color.Lerp(Color.Cyan, Color.Green, Main.rand.NextFloat()), Main.rand.NextFloat(1.1f, 1.6f));
                    d.noGravity = true;
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            // 生成爆炸判定弹幕
            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                Projectile.Center,
                Vector2.Zero,
                ModContent.ProjectileType<PhangasmReIlluExplosion>(),
                Projectile.damage,
                0f,
                Projectile.owner
            );

            SoundEngine.PlaySound(SoundID.Item14 with { Volume = 1.2f, Pitch = -0.0f }, Projectile.Center);

            {
                // 多层爆炸圆环特效（混合贴图 + 偏转 + 大小浮动）
                string texPath = "CalamityRangerExpansion/texture/SuperTexturePack/";
                string[] explosionTextures = {
                    "energy_001",
                    "explosion2_001",
                    "explosion2_002",
                    "sun_006",
                    "sparknoise_008"
                };

                foreach (string tex in explosionTextures)
                {
                    // 大小波动 1.15 ~ 1.55
                    float scale = 1.35f + Main.rand.NextFloat(-0.2f, 0.2f);
                    float rotation = Main.rand.NextFloat(-8f, 8f); // 偏转增加视觉层次

                    Particle p = new CustomPulse(
                        Projectile.Center,
                        Vector2.Zero,
                        Color.Lerp(Color.Aqua, Color.LightGreen, Main.rand.NextFloat()),
                        texPath + tex,
                        Vector2.One * scale,
                        rotation,
                        0.02f,
                        scale, // 最终大小和初始一致，形成“持续扩张感”
                        18
                    );
                    GeneralParticleHandler.SpawnParticle(p);
                }

            }

            // 无序：爆散 Spark
            for (int i = 0; i < 28; i++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(1f, 1f).SafeNormalize(Vector2.UnitY) * Main.rand.NextFloat(12f, 18f);
                GeneralParticleHandler.SpawnParticle(new SparkParticle(
                    Projectile.Center,
                    vel,
                    false,
                    30,
                    Main.rand.NextFloat(3.8f, 5.4f),
                    Color.Lerp(Color.LightGreen, Color.Cyan, Main.rand.NextFloat())
                ));
            }

            // 装饰烟雾
            for (int i = 0; i < 6; i++)
            {
                GeneralParticleHandler.SpawnParticle(new HeavySmokeParticle(
                    Projectile.Center + Main.rand.NextVector2Circular(26f, 26f),
                    Vector2.Zero,
                    Color.LightSeaGreen * 0.7f,
                    24,
                    Main.rand.NextFloat(1.9f, 2.5f),
                    0.3f,
                    0.02f,
                    false
                ));
            }

           
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 命中 NPC 时触发，可添加爆炸特效、附加 Buff 等
        }







    }
}
