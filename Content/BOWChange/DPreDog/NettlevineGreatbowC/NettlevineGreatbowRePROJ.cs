using CalamityMod;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Particles;
using Terraria.DataStructures;
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.DPreDog.NettlevineGreatbowC
{
    internal class NettlevineGreatbowRePROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.DPreDog";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // 获取弹幕中心位置与旋转方向
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            float rotation = Projectile.rotation;

            // ??1. 画本体贴图（透明度可以考虑更改）
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = texture.Size() * 0.5f;
            Color baseColor = Projectile.GetAlpha(lightColor) * 1.0f; // 透明度

            Main.EntitySpriteDraw(texture, drawPos, null, baseColor, rotation, origin, Projectile.scale, SpriteEffects.None, 0);
        
            return false; // 告诉游戏我们已经完成绘制
        }

        public override void SetDefaults()
        {
            // 设置弹幕的基础属性
            Projectile.width = 11; // 弹幕宽度
            Projectile.height = 24; // 弹幕高度
            Projectile.friendly = true; // 对敌人有效
            Projectile.DamageType = DamageClass.Ranged; // 远程伤害类型
            Projectile.penetrate = 2; // 穿透力为1，击中一个敌人就消失
            Projectile.timeLeft = 300; // 弹幕存在时间为x帧
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
            Projectile.ignoreWater = true; // 弹幕不受水影响
            Projectile.arrow = true;
            Projectile.extraUpdates = 1;
        }
        private NPC target;
        private bool latched;
        public override void OnSpawn(IEntitySource source)
        {
            // 每颗弹幕独立相位种子
            Projectile.localAI[0] = Main.rand.NextFloat(0f, MathHelper.TwoPi);
            Projectile.localAI[1] = Main.rand.NextFloat(0.8f, 1.2f); // 随机行为幅度
        }

        public override void AI()
        {
            // 保持旋转与飞行方向一致（弹头向前）
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;

            // 添加绿色光源
            Lighting.AddLight(Projectile.Center, Color.ForestGreen.ToVector3() * 0.42f);
            BowChangeVFX.SpawnTrail(Projectile, BowChangeTheme.Nettle, latched ? 0.3f : 0.7f);

            {
                // 粒子每帧更新一次
                if (Main.rand.NextBool(2))
                {
                    // 1??绿色 AltSpark（类似尾迹特效）
                    Vector2 sparkVelocity = -Projectile.velocity.SafeNormalize(Vector2.UnitY) * Main.rand.NextFloat(0.5f, 2.2f);
                    Particle altSpark = new AltSparkParticle(
                        Projectile.Center + Main.rand.NextVector2Circular(4f, 4f), // 位置带点抖动
                        -Projectile.velocity.SafeNormalize(Vector2.UnitY) * Main.rand.NextFloat(0.5f, 2.2f), // 向尾部拖出
                        false, // 不受重力
                        24,    // 生命周期
                        0.6f,  // 缩放大小
                        Color.LimeGreen // 绿色
                    );
                    GeneralParticleHandler.SpawnParticle(altSpark);
                }

                // 累加时间，用于控制 dust
                Projectile.ai[0] += 1f;
                if (Projectile.ai[0] > 6f)
                {
                    // 2??普通 Dust（经典 TerraBlade 风格）
                    for (int d = 0; d < 3; d++)
                    {
                        Dust dust = Main.dust[
                            Dust.NewDust(
                                Projectile.position,
                                Projectile.width,
                                Projectile.height,
                                DustID.TerraBlade,
                                Projectile.velocity.X,
                                Projectile.velocity.Y,
                                100,
                                default,
                                1f)
                        ];
                        dust.velocity = Vector2.Zero;
                        dust.position -= Projectile.velocity / 5f * d;
                        dust.noGravity = true;
                        dust.scale = 0.65f;
                        dust.noLight = true;
                    }

                    // 3??参考 TerraLance 死亡时的 Dust 爆裂（有序+无序组合）
                    if (Main.rand.NextBool(3))
                    {
                        Vector2 offset = Main.rand.NextVector2Circular(6f, 6f);
                        Dust burstDust = Dust.NewDustPerfect(
                            Projectile.Center + offset,
                            DustID.TerraBlade,
                            offset.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(0.5f, 2f),
                            150,
                            Color.GreenYellow,
                            Main.rand.NextFloat(1.1f, 1.6f)
                        );
                        burstDust.noGravity = true;
                    }
                }
            }

            if (latched)
            {
                if (target != null && target.active)
                {
                    Projectile.Center = target.Center;
                    Projectile.velocity = Vector2.Zero;
                }
                return;
            }

            // 飞行前期（前X帧） - 随机扩散式扰动
            if (Projectile.timeLeft > 250)
            {
                float waveStrength = 0.15f * Projectile.localAI[1];
                float waveOffset = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f + Projectile.localAI[0]);

                Vector2 curve = Projectile.velocity.RotatedBy(MathHelper.ToRadians(15f) * waveStrength * waveOffset);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, curve, 0.1f);
            }

            // 飞行后期（锁定目标）
            else if (target == null || !target.active)
            {
                NPC potentialTarget = Main.npc.Where(n =>
                    n.CanBeChasedBy(this) &&
                    Projectile.Distance(n.Center) < 900f).OrderBy(n =>
                    Projectile.Distance(n.Center)).FirstOrDefault();

                if (potentialTarget != null)
                {
                    target = potentialTarget;
                }
            }

            if (target != null && target.active)
            {
                Vector2 toTarget = (target.Center - Projectile.Center);
                float distance = toTarget.Length();
                Vector2 baseDir = toTarget.SafeNormalize(Vector2.UnitY);

                // ??加入旋转扰动（模拟藤蔓螺旋绕行）
                float swirlStrength = MathHelper.Lerp(0.25f, 0.05f, 1f - distance / 900f); // 距离越近扰动越弱
                float swirlAngle = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f + Projectile.identity) * swirlStrength;
                Vector2 swirlDir = baseDir.RotatedBy(swirlAngle);

                // ??速度插值控制（接近时更柔和）
                float approachSpeed = MathHelper.Lerp(6f, 10f, 1f - distance / 900f);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, swirlDir * approachSpeed, 0.08f);
            }


        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 触发扎入
            this.target = target;
            latched = true;
            Projectile.velocity = Vector2.Zero;
            Projectile.timeLeft = 180;
            BowChangeVFX.SpawnImpact(Projectile, BowChangeTheme.Nettle, 0.9f);
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    target.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<NettlevineGreatbowReRootSnare>(),
                    Math.Max(1, (int)(Projectile.damage * 0.45f)),
                    0f,
                    Projectile.owner);
            }

            SoundEngine.PlaySound(SoundID.Item108.WithVolumeScale(0.33f), Projectile.Center);
        }



        public override void OnKill(int timeLeft)
        {
            BowChangeVFX.SpawnImpact(Projectile, BowChangeTheme.Nettle, 0.75f);

            // 1?? 藤蔓弹幕释放（3~6）
            int vineCount = Main.rand.Next(3, 7);
            for (int i = 0; i < vineCount; i++)
            {
                Vector2 direction = Main.rand.NextVector2Unit() * Main.rand.NextFloat(6f, 10f);
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    direction,
                    ModContent.ProjectileType<NettlevineGreatbowReVine>(),
                    (int)(Projectile.damage * 0.85f),
                    0f,
                    Projectile.owner
                );
            }

            // 2?? 深绿 Dust（爆发感）
            for (int i = 0; i < 25; i++)
            {
                Vector2 burst = Main.rand.NextVector2Circular(4f, 4f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Grass, burst, 100, Color.ForestGreen, Main.rand.NextFloat(1.2f, 1.8f));
                d.noGravity = true;
            }

            // 3?? Spark 粒子
            for (int i = 0; i < 8; i++)
            {
                Vector2 sparkVel = Main.rand.NextVector2Circular(3f, 3f);
                Particle spark = new SparkParticle(
                    Projectile.Center,
                    sparkVel,
                    false,
                    20,
                    1.2f,
                    Color.GreenYellow
                );
                GeneralParticleHandler.SpawnParticle(spark);
            }

            // 4?? 轻型绿色烟雾
            for (int i = 0; i < 6; i++)
            {
                Particle smoke = new HeavySmokeParticle(
                    Projectile.Center + Main.rand.NextVector2Circular(6f, 6f),
                    Main.rand.NextVector2Circular(0.5f, 0.5f),
                    new Color(60, 255, 80),
                    24,
                    Main.rand.NextFloat(1.0f, 1.6f),
                    0.35f,
                    Main.rand.NextFloat(-0.1f, 0.1f),
                    false
                );
                GeneralParticleHandler.SpawnParticle(smoke);
            }
        }



    }
}
