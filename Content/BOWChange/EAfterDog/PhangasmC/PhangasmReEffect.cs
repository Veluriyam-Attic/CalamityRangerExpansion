using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.Particles;
using CalamityRangerExpansion.Content.BOWChange.EAfterDog.PhangasmC;
using Terraria.DataStructures;
using CalamityMod.Sounds;
using Terraria.Audio;
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.EAfterDog.PhangasmC
{
    public class PhangasmReEffect : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        public bool HasHitNPC = false;

        // 用于区分真箭 / 幻箭，0 = 真箭，1 = 幻箭，-1 = 非本武器弹幕
        public int PhangasmArrowType = -1;
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            int type = projectile.GetGlobalProjectile<PhangasmReEffect>().PhangasmArrowType;
            if (type == -1)
                return;

            // 全部弹幕共通设置
            projectile.extraUpdates = 9;
            projectile.arrow = true;
            projectile.tileCollide = false;

            if (projectile.timeLeft < 1400)
                projectile.timeLeft = 1400;
            else
                projectile.timeLeft += 500;

            if (type == 0)
            {
                // 真箭：增加穿透次数
                projectile.penetrate += 2;
                projectile.velocity *= 1.7f;
            }
            else if (type == 1)
            {
                // 幻箭：初始速度减缓
                projectile.velocity *= 1.1f;
            }
        }

        public override void AI(Projectile projectile)
        {
            int type = projectile.GetGlobalProjectile<PhangasmReEffect>().PhangasmArrowType;
            if (type == -1)
                return;
            BowChangeVFX.SpawnTrail(projectile, BowChangeTheme.Phangasm, type == 0 ? 0.55f : 0.38f);

            if (type == 0)
            {
                // 真箭：本体发光、描边、圆润特效
                if (Main.rand.NextBool(2))
                {
                    // 有序：Spark 拖尾（偏青绿）
                    GeneralParticleHandler.SpawnParticle(new SparkParticle(
                        projectile.Center,
                        projectile.velocity * 0.15f,
                        false,
                        40,
                        1.0f,
                        Color.Lerp(Color.Cyan, Color.Teal, Main.rand.NextFloat())
                    ));
                }

                if (Main.rand.NextBool(8))
                {
                    // 有序：AltSpark 粒子线（切割感）
                    GeneralParticleHandler.SpawnParticle(new AltSparkParticle(
                        projectile.Center,
                        projectile.velocity * 0.01f,
                        false,
                        12,
                        1.1f,
                        Color.LightCyan * 0.2f
                    ));
                }

                // 检查命中点附近是否有幻影标记弹幕，并判断其寿命是否小于50%
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile mark = Main.projectile[i];
                    if (mark.active && mark.type == ModContent.ProjectileType<PhangasmReIlluMark>() && mark.owner == projectile.owner)
                    {
                        if (mark.Hitbox.Intersects(projectile.Hitbox))
                        {
                            if (mark.timeLeft <= 150) // 使用设定的300/2
                            {
                                mark.Kill(); // 条件满足，直接摧毁标记
                            }
                        }
                    }
                }



            }
            else if (type == 1)
            {
                // 幻箭：虚幻染色、尖锐特效
                if (Main.rand.NextBool(3))
                {
                    // 有序：CritSpark（可爱虚影）
                    Vector2 direction = projectile.velocity.SafeNormalize(Vector2.UnitX);
                    Vector2 sparkVel = direction.RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)) * 2.5f;
                    CritSpark spark = new CritSpark(
                        projectile.Center,
                        sparkVel,
                        Color.LightBlue,
                        Color.White,
                        1f,
                        20
                    );
                    GeneralParticleHandler.SpawnParticle(spark);
                }


                if (Main.rand.NextBool(16))
                {
                    // 无序：虚幻爆点十字星
                    GenericSparkle sparker = new GenericSparkle(
                        projectile.Center,
                        Vector2.Zero,
                        Color.GreenYellow,
                        Color.Cyan,
                        Main.rand.NextFloat(1.5f, 2.2f),
                        5,
                        Main.rand.NextFloat(-0.02f, 0.02f),
                        1.6f
                    );
                    GeneralParticleHandler.SpawnParticle(sparker);
                }
            }

            // 通用灵动 Dust 特效（真幻共用）
            //if (projectile.timeLeft < 1395 && Main.rand.NextBool(2)) // 稍微延迟几帧再开始
            if (Main.rand.NextBool(2))
            {
                    Vector2 dustOffset = Main.rand.NextVector2CircularEdge(12f, 12f);
                Vector2 velocity = -projectile.velocity.RotatedByRandom(0.6f) * Main.rand.NextFloat(0.4f, 1.2f);

                Dust dust = Dust.NewDustPerfect(projectile.Center + dustOffset, Main.rand.NextBool(3) ? 226 : 272, velocity);
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.25f, 0.55f);
                dust.fadeIn = 0.8f;
            }




        }


        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            int type = projectile.GetGlobalProjectile<PhangasmReEffect>().PhangasmArrowType;
            if (type == -1)
                return;
            BowChangeVFX.SpawnImpact(projectile, BowChangeTheme.Phangasm, type == 0 ? 0.95f : 0.7f);

            var modProj = projectile.GetGlobalProjectile<PhangasmReEffect>();

            // 开关：命中一次后设置标志，防止重复触发
            if (!modProj.HasHitNPC)
            {
                modProj.HasHitNPC = true;

                if (type == 0)
                {

                    // 真箭：基于当前飞行方向发散
                    Vector2 shootDir = projectile.velocity.SafeNormalize(Vector2.UnitY); // 保底安全方向
                    CCBLightingBoltsSystem.Phangasm_TrueArrowHitEffect(projectile.Center, shootDir);

                    // 真箭：斩杀攻击（释放2个）
                    for (int i = 0; i < 2; i++)
                    {
                        Projectile.NewProjectile(
                            projectile.GetSource_FromThis(),
                            target.Center,
                            Vector2.Zero,
                            ModContent.ProjectileType<PhangasmReTrueSlash>(),
                            (int)(projectile.damage * 0.5f),
                            projectile.knockBack,
                            projectile.owner
                        );
                    }

                    // 关键音效
                    SoundEngine.PlaySound(CommonCalamitySounds.SwiftSliceSound with { Volume = 0.5f }, projectile.Center);

                    {
                        // 有序粒子：主线 Spark 拖尾
                        for (int i = 0; i < 5; i++)
                        {
                            Vector2 vel = shootDir.RotatedByRandom(MathHelper.ToRadians(15f)) * Main.rand.NextFloat(1f, 2f);
                            GeneralParticleHandler.SpawnParticle(new SparkParticle(projectile.Center, vel, false, 40, 1.2f, Color.Cyan));
                        }

                        // 线状割裂：AltSpark
                        for (int i = 0; i < 3; i++)
                        {
                            GeneralParticleHandler.SpawnParticle(new AltSparkParticle(
                                projectile.Center,
                                shootDir.RotatedByRandom(0.3f) * 0.5f,
                                false,
                                16,
                                1.3f,
                                Color.Teal * 0.2f
                            ));
                        }

                        // 无序：爆点烟雾
                        for (int i = 0; i < 2; i++)
                        {
                            GeneralParticleHandler.SpawnParticle(new HeavySmokeParticle(
                                projectile.Center + Main.rand.NextVector2Circular(4f, 4f),
                                shootDir * 0.4f,
                                Color.WhiteSmoke,
                                20,
                                1f,
                                0.6f,
                                0.05f,
                                false
                            ));
                        }                  
                    }
                }
                else if (type == 1)
                {
                    // 幻箭追踪触发逻辑：剩余时间低于 75% 时开始追踪
                    int maxTime = 1900; // 1400+500：OnSpawn 中设定的时间总值
                    if (projectile.timeLeft <= maxTime * 0.75f)
                    {
                        CalamityUtils.HomeInOnNPC(projectile, true, 2000f, 12, 200f);
                    }

                    // 幻箭：在原地有序展开
                    CCBLightingBoltsSystem.Phangasm_IllusionArrowHitEffect(projectile.Center);

                    {
                        // 判断范围内是否已有标记弹幕存在
                        bool markExists = false;
                        float checkRadius = 10 * 16f;

                        for (int i = 0; i < Main.maxProjectiles; i++)
                        {
                            Projectile other = Main.projectile[i];
                            if (other.active && other.type == ModContent.ProjectileType<PhangasmReIlluMark>() && other.owner == projectile.owner)
                            {
                                if (Vector2.Distance(other.Center, projectile.Center) < checkRadius)
                                {
                                    markExists = true;
                                    break;
                                }
                            }
                        }

                        // 如果不存在才生成
                        if (!markExists)
                        {
                            Projectile.NewProjectile(
                                projectile.GetSource_FromThis(),
                                projectile.Center,
                                Vector2.Zero,
                                ModContent.ProjectileType<PhangasmReIlluMark>(),
                                (int)(projectile.damage * 3f), // 翻三倍伤害
                                0f,
                                projectile.owner
                            );
                        }

                    }

                    {
                        // 十字星：GenericSparkle
                        for (int i = 0; i < 2; i++)
                        {
                            GeneralParticleHandler.SpawnParticle(new GenericSparkle(
                                projectile.Center,
                                Vector2.Zero,
                                Color.LightBlue,
                                Color.LightGreen,
                                Main.rand.NextFloat(1.5f, 2.1f),
                                6,
                                Main.rand.NextFloat(-0.02f, 0.02f),
                                1.5f
                            ));
                        }

                        // 上升光粒：SquishyLightParticle
                        for (int i = 0; i < 3; i++)
                        {
                            GeneralParticleHandler.SpawnParticle(new SquishyLightParticle(
                                projectile.Center + Main.rand.NextVector2Circular(8f, 8f),
                                -Vector2.UnitY.RotatedByRandom(0.6f) * Main.rand.NextFloat(0.2f, 1f),
                                0.25f,
                                Color.Green * 0.9f,
                                30
                            ));
                        }

                        // 辅助烟雾
                        for (int i = 0; i < 2; i++)
                        {
                            GeneralParticleHandler.SpawnParticle(new HeavySmokeParticle(
                                projectile.Center + Main.rand.NextVector2Circular(4f, 4f),
                                Vector2.Zero,
                                Color.LightSeaGreen * 0.6f,
                                24,
                                0.8f,
                                0.4f,
                                0.01f,
                                false
                            ));
                        }
                    }

                    // ? 所有弹幕共通 Dust 效果
                    for (int i = 0; i < 8; i++)
                    {
                        Dust d = Dust.NewDustPerfect(
                            projectile.Center + Main.rand.NextVector2Circular(6f, 6f),
                            DustID.TintableDustLighted,
                            Main.rand.NextVector2Circular(1f, 1f),
                            100,
                            Color.Lerp(Color.Cyan, Color.Teal, Main.rand.NextFloat()),
                            Main.rand.NextFloat(0.9f, 1.2f)
                        );
                        d.noGravity = true;
                    }
                }

                if (projectile.owner == Main.myPlayer)
                {
                    Projectile.NewProjectile(
                        projectile.GetSource_FromThis(),
                        target.Center,
                        projectile.velocity.SafeNormalize(Vector2.UnitY) * 0.8f,
                        ModContent.ProjectileType<PhangasmReMirageRift>(),
                        (int)(projectile.damage * (type == 0 ? 0.42f : 0.34f)),
                        projectile.knockBack,
                        projectile.owner,
                        type,
                        target.whoAmI);
                }
            }


            if (projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile ufo = Main.projectile[i];
                    if (ufo.active && ufo.owner == projectile.owner && ufo.type == ModContent.ProjectileType<PhangasmReUFO>())
                    {
                        int fireCount = Main.rand.Next(3, 5); // 3~4 发
                        for (int j = 0; j < fireCount; j++)
                        {
                            Vector2 spawnPos = ufo.Center + Main.rand.NextVector2Circular(48f, 48f);

                            // 加入±20°的偏转
                            Vector2 shootDir = Vector2.Normalize(target.Center - spawnPos).RotatedByRandom(MathHelper.ToRadians(20f));

                            float speed = Main.rand.NextFloat(11f, 16f); // 速度有点浮动
                            int damage = (int)(projectile.damage * 0.2f);

                            Projectile.NewProjectile(
                                projectile.GetSource_FromThis(),
                                spawnPos,
                                shootDir * speed,
                                ModContent.ProjectileType<PhangasmReGOEST>(),
                                damage,
                                0f,
                                projectile.owner
                            );
                        }
                        break;
                    }
                }
            }


        }


        public override void OnKill(Projectile projectile, int timeLeft)
        {
            int type = projectile.GetGlobalProjectile<PhangasmReEffect>().PhangasmArrowType;
            if (type == -1)
                return;

            if (type == 0)
            {
                BowChangeVFX.SpawnImpact(projectile, BowChangeTheme.Phangasm, 0.65f);
                // 真箭死亡特效（极少情况死亡前未命中）
                // [TODO] 补充视觉反馈
            }
            else if (type == 1)
            {
                BowChangeVFX.SpawnImpact(projectile, BowChangeTheme.Phangasm, 0.45f);
                // 幻箭死亡：不爆炸，仅视觉淡出
                // [TODO] 留下残影烟雾或虚影
            }
        }






    }
}
