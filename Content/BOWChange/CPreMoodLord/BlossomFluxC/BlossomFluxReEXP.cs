using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityRangerExpansion.Content.BOWChange.CPreMoodLord.BlossomFluxC
{
    internal class BlossomFluxReEXP : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.CPreMoodLord";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 10 * 16;
            Projectile.height = 10 * 16;

            Projectile.friendly = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;

            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //target.AddBuff(ModContent.BuffType<RiptideDebuff>(), 240);
        }

        public override void AI()
        {
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.dedServ)
                return;

            Vector2 c = Projectile.Center;

            // 声音：绿爆的“绽放感”
            SoundEngine.PlaySound(SoundID.Item14 with { Volume = 1.25f, Pitch = 0.15f }, c);

            // 让每次爆炸的“有序结构”不完全重叠（但仍然确定性偏多）
            int seed = Projectile.identity * 97 + Projectile.owner * 131;
            float baseRot = (seed % 1000) / 1000f * MathHelper.TwoPi;

            // 颜色基调：嫩绿 / 黄绿 / 冷绿高光
            Color cA = new Color(90, 255, 150);
            Color cB = new Color(160, 255, 200);
            Color cC = new Color(110, 235, 255);
            Color cD = new Color(180, 255, 120);

            // =========================
            // ① 外环：两层“绽放刻度环”（有序）
            // =========================
            for (int ring = 0; ring < 2; ring++)
            {
                int pts = ring == 0 ? 24 : 36;
                float radius = ring == 0 ? 26f : 62f;
                float speed = ring == 0 ? 3.4f : 4.6f;
                float scale = ring == 0 ? 1.15f : 1.35f;

                for (int i = 0; i < pts; i++)
                {
                    float t = (float)i / pts;
                    float ang = baseRot + MathHelper.TwoPi * t;

                    // 轻微“花瓣扰动”，保持秩序又不死板
                    float wobble = 1f + 0.07f * (float)Math.Sin((i + ring * 3) * 0.85f);
                    Vector2 dir = ang.ToRotationVector2();
                    Vector2 pos = c + dir * radius * wobble;

                    Dust d = Dust.NewDustPerfect(pos, 107);
                    d.noGravity = true;
                    d.velocity = dir * speed;
                    d.scale = scale * (0.9f + 0.12f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 8f + i));
                    d.alpha = 60;
                    d.color = Color.Lerp(cA, cB, 0.35f + 0.55f * (float)Math.Sin(i * 0.32f + ring));
                }
            }

            // =========================
            // ② 螺旋：Fermat 螺旋（数学美感，有序→无序过渡）
            // =========================
            const int spiralCount = 72;
            float golden = MathHelper.ToRadians(137.5f);

            for (int i = 0; i < spiralCount; i++)
            {
                float u = (i + 1f) / spiralCount;
                float ang = baseRot + i * golden;

                // 半径随 sqrt 增长：中心密、外圈疏
                float r = 8f + (float)Math.Sqrt(u) * 92f;

                Vector2 dir = ang.ToRotationVector2();
                Vector2 pos = c + dir * r;

                // 速度外扩，外圈略更强
                float spd = MathHelper.Lerp(2.2f, 6.2f, u);
                Vector2 vel = dir * spd;

                Dust d = Dust.NewDustPerfect(pos, 107);
                d.noGravity = true;
                d.velocity = vel;
                d.scale = MathHelper.Lerp(0.95f, 1.45f, u);
                d.alpha = 70;
                d.color = Color.Lerp(cD, cC, 0.25f + 0.55f * (float)Math.Sin(i * 0.21f));
            }

            // =========================
            // ③ 冲击波“绿脉射线”（粒子：AltSpark / PointParticle）
            // =========================
            int rays = 12;
            for (int i = 0; i < rays; i++)
            {
                float ang = baseRot + MathHelper.TwoPi * i / rays;
                Vector2 dir = ang.ToRotationVector2();

                // 绿脉线（有方向、有爆点感）
                AltSparkParticle vein = new AltSparkParticle(
                    c + dir * 10f,
                    dir * 9.5f,
                    false,
                    18,
                    1.55f,
                    Color.Lerp(cA, cC, 0.35f) * 0.55f
                );
                GeneralParticleHandler.SpawnParticle(vein);

                // 点刺（像“孢子针刺”）
                PointParticle stab = new PointParticle(
                    c + dir * 6f,
                    dir * 4.2f,
                    false,
                    16,
                    1.15f,
                    Color.Lerp(cD, cB, 0.5f)
                );
                GeneralParticleHandler.SpawnParticle(stab);
            }

            // =========================
            // ④ 核心花粉云（无序，但密度受控）
            // =========================
            int core = 30;
            for (int i = 0; i < core; i++)
            {
                Vector2 dir = Main.rand.NextVector2Unit();
                float dist = Main.rand.NextFloat(0f, 18f);
                float spd = Main.rand.NextFloat(2.2f, 7.2f);

                Dust d = Dust.NewDustPerfect(c + dir * dist, 107);
                d.noGravity = true;
                d.velocity = dir * spd;
                d.scale = Main.rand.NextFloat(1.1f, 1.8f);
                d.alpha = 80;
                d.color = Color.Lerp(cA, cD, Main.rand.NextFloat());
            }
        }

        public override bool PreDraw(ref Color lightColor) => false;
    }
}
