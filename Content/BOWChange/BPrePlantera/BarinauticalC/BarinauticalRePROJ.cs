using CalamityMod;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.Particles;
using Terraria.Audio;
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.BPrePlantera.BarinauticalC
{
    internal class BarinauticalRePROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.BPrePlantera";

        // 这里设定多帧图和拖尾类型和长度
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }
        // 这里套用拖尾模板，可调整是否发光
        // 有的时候会调用更加复杂的情况，比如发光描边等等
        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
            return false;
        }

        private ref float Mode => ref Projectile.localAI[0]; // 0 = A, 1 = B
        private ref float Timer => ref Projectile.ai[0];
        private int diveState = 0; // B 模式用：0=正常，1=减速，2=飞天
        public override void SetDefaults()
        {
            Projectile.arrow = true; // 他一定是箭，所以这句话一定要加
            Projectile.width = Projectile.height = 10; // 弹幕宽高
            Projectile.friendly = true; // 当然他是我方弹幕
            Projectile.DamageType = DamageClass.Ranged; // 远程伤害
            Projectile.penetrate = 1; // 可造成多少次伤害然后消失
            Projectile.extraUpdates = 1; // 每次额外更新多少次数，值越大弹幕就越快
            Projectile.timeLeft = 600; // 剩余时间
            Projectile.ignoreWater = true; // 是否无视水体的影响
            Projectile.tileCollide = true; // 是否与方块发生碰撞后消除自己
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
            Projectile.aiStyle = ProjAIStyleID.Arrow; // 让弹幕受到重力影响（这个可选）
        }

        public override void OnSpawn(Terraria.DataStructures.IEntitySource source)
        {
            if (Mode == 0)
            {
                Projectile.extraUpdates = 2; // A型-直线火花
                Projectile.timeLeft = 600; // 剩余时间
            }
            else
            {
                Projectile.extraUpdates = 1; // B型-空中闪电
                Projectile.timeLeft = 300; // 剩余时间
            }
 

        }

        public override void AI()
        {
            Timer++;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            BowChangeVFX.SpawnTrail(Projectile, BowChangeTheme.Ocean, Mode == 0 ? 0.65f : 0.85f);

            // 通用粒子特效
            if (Main.rand.NextBool(3))
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Electric, Main.rand.NextVector2Circular(1.5f, 1.5f), 100, Color.Cyan, 1.1f);
                d.noGravity = true;
            }

            if (Main.rand.NextBool(5))
            {
                SquareParticle p = new SquareParticle(
                    Projectile.Center + Main.rand.NextVector2Circular(2f, 2f), // 位置
                    -Projectile.velocity * 0.1f,                                // 速度
                    false,                                                     // 不受重力
                    24,                                                        // 存活时间
                    1f + Main.rand.NextFloat(-0.3f, 0.4f),                     // 缩放
                    Main.rand.NextBool() ? Color.SkyBlue : Color.LightBlue     // 颜色
                );
                GeneralParticleHandler.SpawnParticle(p);
            }

            // 分支逻辑
            if (Mode == 0)
            {
                float spiralRadius = 20f;
                float time = Main.GlobalTimeWrappedHourly * 5f + Projectile.whoAmI;

                for (int i = 0; i < 2; i++)
                {
                    float angleOffset = i * MathHelper.Pi; // 两条对称螺旋
                    float angle = time + angleOffset;
                    Vector2 offset = angle.ToRotationVector2() * spiralRadius;

                    Vector2 spawnPos = Projectile.Center + offset;

                    // 发光 dust（蓝电）
                    Dust d = Dust.NewDustPerfect(
                        spawnPos,
                        DustID.Electric,
                        Vector2.Zero,
                        150,
                        Color.Cyan,
                        0.9f
                    );
                    d.noGravity = true;

                    // spark 辅助
                    if (Main.rand.NextBool(5))
                    {
                        SparkParticle spark = new SparkParticle(
                            spawnPos,
                            -Projectile.velocity * 0.15f,
                            false,
                            30,
                            0.8f,
                            Color.LightBlue
                        );
                        GeneralParticleHandler.SpawnParticle(spark);
                    }
                }
            }
            else // B型
            {
                if (diveState == 0)
                {
                    // 检查下方是否存在敌人
                    Rectangle detectBox = new Rectangle(
                        (int)Projectile.Center.X - 32,
                        (int)Projectile.Center.Y,
                        64,
                        800 // 50格的纵深
                    );

                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC npc = Main.npc[i];
                        if (npc.active && !npc.friendly && !npc.dontTakeDamage && npc.CanBeChasedBy(Projectile))
                        {
                            if (npc.Hitbox.Intersects(detectBox))
                            {
                                diveState = 1;
                                break;
                            }
                        }
                    }
                }

                if (diveState == 1)
                {
                    // 死亡下落：清除水平速度，只往下冲
                    float fallSpeed = 20f;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.UnitY * fallSpeed, 0.3f);

                    // 可加轻微电特效表示“触发”
                    if (Main.rand.NextBool(3))
                    {
                        Dust d = Dust.NewDustPerfect(
                            Projectile.Center + Main.rand.NextVector2Circular(6f, 4f),
                            DustID.Electric,
                            new Vector2(0, 1.5f),
                            100,
                            Color.Cyan,
                            1.2f
                        );
                        d.noGravity = true;
                    }
                }

            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            BowChangeVFX.SpawnImpact(Projectile, BowChangeTheme.Ocean, Mode == 0 ? 0.8f : 1f);

            if (Mode == 0) // A型：电火花
            {

            }

            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    target.Center,
                    -Projectile.velocity.SafeNormalize(Vector2.UnitY) * 0.01f,
                    ModContent.ProjectileType<BarinauticalReDepthCharge>(),
                    Math.Max(1, (int)(Projectile.damage * (Mode == 0 ? 0.35f : 0.55f))),
                    Projectile.knockBack * 0.25f,
                    Projectile.owner);
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (Mode == 0)
            {
                // A型死亡特效：电符冲击（使用 spark + crack）

                for (int i = 0; i < 6; i++)
                {
                    Vector2 dir = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(15f)).SafeNormalize(Vector2.UnitY);
                    float speed = Main.rand.NextFloat(3f, 6f);

                    // 尖锐电火星（线性粒子）
                    SparkParticle spark = new SparkParticle(
                        Projectile.Center,
                        dir * speed * 0.4f,
                        false,
                        40,
                        1.1f + Main.rand.NextFloat(-0.2f, 0.2f),
                        Color.Cyan
                    );
                    GeneralParticleHandler.SpawnParticle(spark);

                    // 闪电裂纹粒子
                    CrackParticle crack = new CrackParticle(
                        Projectile.Center,
                        dir * Main.rand.NextFloat(1.5f, 2.2f), // 更随机的速度
                        Main.rand.NextBool() ? Color.Cyan : Color.LightBlue, // 混色
                        new Vector2(Main.rand.NextFloat(0.7f, 1.3f), Main.rand.NextFloat(0.6f, 1.4f)), // 随机拉伸
                        Main.rand.NextFloat(-MathHelper.TwoPi, MathHelper.TwoPi), // 完整旋转随机
                        Main.rand.NextFloat(0.05f, 0.2f), // 初始缩放
                        Main.rand.NextFloat(0.5f, 0.75f),  // 最终放大
                        Main.rand.Next(28, 40)            // 存活时间随机
                    );
                    GeneralParticleHandler.SpawnParticle(crack);
                }

                for (int i = 0; i < 4; i++)
                {
                    Vector2 vel = Main.rand.NextVector2Circular(13f, 13f);
                    Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        Projectile.Center,
                        vel,
                        ModContent.ProjectileType<BarinauticalReSpark>(),
                        Projectile.damage / 3,
                        0f,
                        Projectile.owner
                    );
                }

                SoundEngine.PlaySound(SoundID.Item94 with { Volume = 1.0f }, Projectile.Center);
            }
            else
            {
                Vector2 spawnPosition = Projectile.Center + new Vector2(Main.rand.NextFloat(-50, 50), -800);
                Vector2 velocity = Vector2.UnitY * 20f;
                int lightningProjectile = Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPosition, velocity, ProjectileID.CultistBossLightningOrbArc, (int)(Projectile.damage * 2.1f), 0f, Projectile.owner, MathHelper.PiOver2, Main.rand.Next(100));
                Projectile proj = Main.projectile[lightningProjectile];
                proj.friendly = true;
                proj.hostile = false;
                proj.penetrate = -1;
                proj.localNPCHitCooldown = 60;
                proj.usesLocalNPCImmunity = true;
                proj.extraUpdates = 4;
            }

            SoundEngine.PlaySound(SoundID.Item122 with { Volume = 1.0f }, Projectile.Center);
        }



    }
}
