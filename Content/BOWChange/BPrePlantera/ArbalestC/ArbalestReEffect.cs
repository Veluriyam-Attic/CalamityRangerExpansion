using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod;

namespace CalamityRangerExpansion.Content.BOWChange.BPrePlantera.ArbalestC
{
    internal class ArbalestReEffect : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public int ArbalestType = -1; // -1表示非本武器弹幕，0为A弹，1为B弹

        public override void AI(Projectile projectile)
        {
            int type = projectile.GetGlobalProjectile<ArbalestReEffect>().ArbalestType;
            if (type == -1)
                return;

            if (type == 0)
            {
                // A：绿色火焰拖尾（增强版）
                if (Main.rand.NextBool(1))
                {
                    // A：绿色机械激光拖尾 + 冲击波
                    projectile.aiStyle = -1;
                    projectile.extraUpdates += 2;

                    if (Main.rand.NextBool(2))
                    {
                        Color sparkColor = new Color(100, 255, 160);
                        float sparkScale = 1.8f + Main.rand.NextFloat(-0.3f, 0.5f);
                        SparkParticle spark = new SparkParticle(
                            projectile.Center,
                            projectile.velocity * 0.4f,
                            false,
                            9,
                            sparkScale,
                            sparkColor
                        );
                        GeneralParticleHandler.SpawnParticle(spark);
                    }

                    if (Main.rand.NextBool(16))
                    {
                        Particle pulse = new DirectionalPulseRing(
                            projectile.Center,
                            projectile.velocity.SafeNormalize(Vector2.UnitY) * 0.75f,
                            Color.LimeGreen,
                            new Vector2(1.1f, 2.6f),
                            projectile.rotation - MathHelper.PiOver4 - MathHelper.PiOver4,
                            0.28f,
                            0.045f,
                            20
                        );
                        GeneralParticleHandler.SpawnParticle(pulse);
                    }
                }




            }
            else if (type == 1)
            {
                // B：红色机械激光拖尾 + 冲击波
                projectile.aiStyle = -1;
                projectile.extraUpdates += 2;

                if (Main.rand.NextBool(2))
                {
                    Color sparkColor = new Color(255, 100, 100);
                    float sparkScale = 1.8f + Main.rand.NextFloat(-0.3f, 0.5f);
                    SparkParticle spark = new SparkParticle(
                        projectile.Center,
                        projectile.velocity * 0.4f,
                        false,
                        9,
                        sparkScale,
                        sparkColor
                    );
                    GeneralParticleHandler.SpawnParticle(spark);
                }

                if (Main.rand.NextBool(16))
                {
                    Particle pulse = new DirectionalPulseRing(
                        projectile.Center,
                        projectile.velocity.SafeNormalize(Vector2.UnitY) * 0.75f,
                        Color.Red,
                        new Vector2(1.1f, 2.6f),
                        projectile.rotation - MathHelper.PiOver4 - MathHelper.PiOver4,
                        0.28f,
                        0.045f,
                        20
                    );
                    GeneralParticleHandler.SpawnParticle(pulse);
                }
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            int type = projectile.GetGlobalProjectile<ArbalestReEffect>().ArbalestType;
            if (type == -1)
                return;

            if (type == 0)
            {
                target.AddBuff(BuffID.CursedInferno, 300); // A弹：绿色鬼火附带诅咒之火

                // A命中：生成绿色Dust与鬼火弹幕
                for (int i = 0; i < 32; i++)
                {
                    Vector2 offset = Main.rand.NextVector2Unit() * Main.rand.NextFloat(4f, 8f);
                    Dust d = Dust.NewDustPerfect(projectile.Center + offset, DustID.GreenTorch, offset * 0.2f, 100, Color.LimeGreen, 1.1f);
                    d.noGravity = true;
                }

                for (int i = 0; i < 3; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(3f, 3f);
                    Projectile.NewProjectile(
                        projectile.GetSource_FromThis(),
                        projectile.Center,
                        velocity,
                        ModContent.ProjectileType<ArbalestGhostFire>(),
                        projectile.damage / 3,
                        0f,
                        projectile.owner
                    );
                }
            }
            else if (type == 1)
            {
                target.AddBuff(BuffID.OnFire, 240); // B弹：红色激光附带点燃

                // B命中：冲击波特效
                Particle pulse = new DirectionalPulseRing(
                    projectile.Center,
                    projectile.velocity.SafeNormalize(Vector2.UnitY) * 0.75f,
                    Color.Red,
                    new Vector2(1f, 2.5f),
                    projectile.rotation - MathHelper.PiOver4,
                    0.25f,
                    0.035f,
                    20
                );
                GeneralParticleHandler.SpawnParticle(pulse);
            }
        }
        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(projectile, target, ref modifiers);

            int type = projectile.GetGlobalProjectile<ArbalestReEffect>().ArbalestType;
            if (type == -1)
                return;

            if (type == 1) // B弹：红色激光
                modifiers.SourceDamage *= 2f;
        }


   

    }
}