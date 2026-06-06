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
using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;
using CalamityMod.Particles;
using Terraria.DataStructures;
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.DPreDog.ArterialAssaultC
{
    internal class ArterialAssaultRePROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.DPreDog";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        internal Color ColorFunction(float completionRatio, Vector2 vertexPosition)
        {
            float fadeOpacity = Utils.GetLerpValue(0.94f, 0.54f, completionRatio, true) * Projectile.Opacity;
            return Color.Lerp(Color.Cyan, Color.White, 0.4f) * fadeOpacity;
        }

        internal float WidthFunction(float completionRatio, Vector2 vertexPosition)
        {
            float expansionCompletion = 1f - (float)Math.Pow(1f - Utils.GetLerpValue(0f, 0.3f, completionRatio, true), 2D);
            return MathHelper.Lerp(0f, 12f * Projectile.Opacity, expansionCompletion);
        }
        public override bool PreDraw(ref Color lightColor)
        {

            //Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            //Rectangle frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            //Vector2 origin = frame.Size() * 0.5f;
            //Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            //SpriteEffects direction = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Rectangle frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;
            Vector2 drawPos = Projectile.Center - Main.screenPosition;


            GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/ScarletDevilStreak"));
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(WidthFunction, ColorFunction, (_, _) => Projectile.Size * 0.5f, shader: GameShaders.Misc["CalamityMod:TrailStreak"]), 60);


            // 外描边
            //for (int i = 0; i < 8; i++)
            //{
            //    Vector2 offset = new Vector2(2f, 0).RotatedBy(MathHelper.TwoPi * i / 8f);
            //    Main.spriteBatch.Draw(texture, drawPos + offset, frame, Color.DarkRed * 0.6f, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
            //}

            // 本体

            // 主体贴图（翻转绘制）
            Main.spriteBatch.Draw(texture, drawPos, frame, Color.White, Projectile.rotation + MathHelper.Pi, origin, Projectile.scale, SpriteEffects.None, 0f);

            return false;
        }
        public override void SetDefaults()
        {
            // 设置弹幕的基础属性
            Projectile.width = 11; // 弹幕宽度
            Projectile.height = 24; // 弹幕高度
            Projectile.friendly = true; // 对敌人有效
            Projectile.DamageType = DamageClass.Ranged; // 远程伤害类型
            Projectile.penetrate = 1; // 穿透力为1，击中一个敌人就消失
            Projectile.timeLeft = 300; // 弹幕存在时间为x帧
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
            Projectile.ignoreWater = true; // 弹幕不受水影响
            Projectile.arrow = true;
            Projectile.extraUpdates = 1;
        }
        public override void OnSpawn(IEntitySource source)
        {
            float level = MathHelper.Clamp(Projectile.ai[0], 0f, 1f); // 蓄力等级
            Projectile.extraUpdates = (int)MathHelper.Lerp(1f, 3f, level); // 提高飞行精度
            Projectile.velocity *= MathHelper.Lerp(1f, 1.35f, level); // 增加速度
            Projectile.localNPCHitCooldown = (int)MathHelper.Lerp(14f, 5f, level); // 减少无敌帧
            Projectile.penetrate = 1 + (int)(level * 3f); // 多穿透（视觉强化）
        }
        // ??用于判断是否已经触发过一次追踪爆发特效
        private bool hasTracked = false;
        private int delayTimer = 0;
        private const int ChaseDelay = 30; // 延迟时间，单位帧
        public override void AI()
        {
            float level = MathHelper.Clamp(Projectile.ai[0], 0f, 1f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            BowChangeVFX.SpawnTrail(Projectile, BowChangeTheme.Arterial, 0.65f + level * 0.45f);
            {

                // === 血箭飞行特效 ===

                // ?? 基础火花拖尾（保持原有结构）
                for (int i = 0; i < 2; i++)
                {
                    SparkParticle spark = new SparkParticle(
                        Projectile.Center + Main.rand.NextVector2Circular(3f, 3f),
                        Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * 0.2f,
                        false,
                        50,
                        1.0f,
                        Color.Lerp(Color.Red, Color.OrangeRed, Main.rand.NextFloat(0.3f)) * 0.8f
                    );
                    GeneralParticleHandler.SpawnParticle(spark);
                }

                // ? 高亮血能量球（间断闪烁）
                if (Main.rand.NextBool(5))
                {
                    GlowOrbParticle orb = new GlowOrbParticle(
                        Projectile.Center,
                        Vector2.Zero,
                        false,
                        6,
                        1.1f,
                        Color.Red * 0.9f,
                        true,
                        false,
                        true
                    );
                    GeneralParticleHandler.SpawnParticle(orb);
                }

                // ?? 細長閃紅線（拖尾）
                AltSparkParticle alt = new AltSparkParticle(
                    Projectile.Center - Projectile.velocity * 1.5f,
                    Projectile.velocity * 0.01f,
                    false,
                    10,
                    1.2f,
                    Color.DarkRed * 0.18f
                );
                GeneralParticleHandler.SpawnParticle(alt);

                // ?? 飘血尘雾（营造血气）
                for (int i = 0; i < 2; i++)
                {
                    Dust d = Dust.NewDustPerfect(
                        Projectile.Center + Main.rand.NextVector2Circular(4f, 4f),
                        DustID.Blood,
                        Projectile.velocity * Main.rand.NextFloat(0.2f, 0.6f),
                        0,
                        Color.DarkRed,
                        Main.rand.NextFloat(1f, 1.4f)
                    );
                    d.noGravity = true;
                }

            }

            if (!hasTracked && Main.GameUpdateCount - Projectile.ai[1] > 20)
            {
                hasTracked = true;

                // === 爆发追踪视觉冲击特效 ===
                //for (int i = 0; i < 40; i++)
                //{
                //    Vector2 vel = Main.rand.NextVector2CircularEdge(10f, 10f) * Main.rand.NextFloat(1f, 4f);
                //    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Blood, vel, 0, Color.DarkRed, Main.rand.NextFloat(1.3f, 2.2f));
                //    d.noGravity = true;
                //}

                //Particle ring = new DirectionalPulseRing(
                //    Projectile.Center,
                //    Vector2.Zero,
                //    Color.Red,
                //    new Vector2(1.5f, 1.5f),
                //    0f,
                //    0.5f,
                //    6f,
                //    50
                //);
                //GeneralParticleHandler.SpawnParticle(ring);
            }

            delayTimer++;

            level = MathHelper.Clamp(Projectile.ai[0], 0f, 1f); // ? 修正：使用已存在的变量

            // === 1?? 非追踪阶段：主动远离同类 ===
            if (delayTimer < ChaseDelay)
            {
                float separationRange = 160f;
                float pushStrength = 0.25f;

                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile other = Main.projectile[i];
                    if (other.active && other.type == Projectile.type && other.whoAmI != Projectile.whoAmI)
                    {
                        float dist = Vector2.Distance(Projectile.Center, other.Center);
                        if (dist < separationRange && dist > 4f)
                        {
                            Vector2 push = (Projectile.Center - other.Center).SafeNormalize(Vector2.Zero) * pushStrength;
                            Projectile.velocity += push;
                        }
                    }
                }

                // 飞行方向随机微扰（让它们彼此方向也不同）
                Projectile.velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.04f, 0.04f));
                return;
            }

            // === 2?? 正常追踪阶段 ===
            NPC target = Projectile.Center.ClosestNPCAt(900f);
            if (target != null && target.CanBeChasedBy())
            {
                Vector2 desired = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitX) * 14f;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desired, 0.06f + 0.1f * level);
            }
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            BowChangeVFX.SpawnImpact(Projectile, BowChangeTheme.Arterial, 0.85f + Projectile.ai[0] * 0.45f);
            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 vel = Projectile.velocity.SafeNormalize(Vector2.UnitY)
                        .RotatedBy(Main.rand.NextFloat(-0.9f, 0.9f))
                        * Main.rand.NextFloat(5f, 8f);
                    Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        Projectile.Center,
                        vel,
                        ModContent.ProjectileType<ArterialAssaultReBloodSiphon>(),
                        Math.Max(1, (int)(Projectile.damage * 0.28f)),
                        Projectile.knockBack * 0.25f,
                        Projectile.owner);
                }
            }

            if (Main.rand.NextFloat() < 0.35f + Projectile.ai[0] * 0.5f)
            {
                int heal = (int)(damageDone * 0.12f);
                player.statLife += heal;
                player.HealEffect(heal);
            }


        }
        public override void OnKill(int timeLeft)
        {


            {
                Vector2 center = Projectile.Center;

                // ??中心爆炸粒子（细节爆发感）
                Particle explosion = new DetailedExplosion(
                    center,
                    Vector2.Zero,
                    Color.DarkRed * 0.9f,
                    Vector2.One * 1.1f,
                    Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi),
                    0f,
                    0.32f,
                    12
                );
                GeneralParticleHandler.SpawnParticle(explosion);

                // ??冲击波环（瞬间扩张）
                // ??改进版：随机椭圆冲击波（释放位置和形状都带随机）
                for (int i = 0; i < 2; i++)
                {
                    // 轻微偏移位置
                    Vector2 offsetPos = center + Main.rand.NextVector2Circular(8f, 8f);

                    // 椭圆扁平度随机（X、Y轴缩放范围：0.8~1.5）
                    float scaleX = Main.rand.NextFloat(0.8f, 1.5f);
                    float scaleY = Main.rand.NextFloat(0.8f, 1.5f);

                    Particle pulse = new DirectionalPulseRing(
                        offsetPos,
                        Vector2.Zero,
                        Color.Red,
                        new Vector2(scaleX, scaleY),
                        0f,
                        0.2f,
                        4f * 0.3f, // ??最终大小削减为原来的 30%
                        20
                    );
                    GeneralParticleHandler.SpawnParticle(pulse);
                }


                // ??极亮红光光晕
                StrongBloom strong = new StrongBloom(center, Vector2.Zero, Color.Red, 1.8f, 20);
                GeneralParticleHandler.SpawnParticle(strong);

                // ???重型血雾烟雾（上升）
                for (int i = 0; i < 4; i++)
                {
                    Vector2 vel = new Vector2(0, -1f).RotatedByRandom(0.4f) * Main.rand.NextFloat(1.5f, 4f);
                    Particle smoke = new HeavySmokeParticle(
                        center,
                        vel,
                        Color.DarkRed,
                        28,
                        Main.rand.NextFloat(0.9f, 1.4f),
                        1f,
                        MathHelper.ToRadians(Main.rand.NextFloat(-2f, 2f)),
                        true
                    );
                    GeneralParticleHandler.SpawnParticle(smoke);
                }

                // ?血裂感（裂开的血痕）
                for (int i = 0; i < 3; i++)
                {
                    CrackParticle crack = new CrackParticle(
                        center,
                        Main.rand.NextVector2Circular(2f, 2f),
                        Color.Red,
                        new Vector2(1f, 1f),
                        Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi),
                        0.1f,
                        0.3f,
                        28
                    );
                    GeneralParticleHandler.SpawnParticle(crack);
                }

                // ??点状血滴飞溅
                for (int i = 0; i < 16; i++)
                {
                    PointParticle drop = new PointParticle(
                        center,
                        Main.rand.NextVector2Circular(3f, 3f),
                        false,
                        20,
                        1.0f,
                        Color.DarkRed
                    );
                    GeneralParticleHandler.SpawnParticle(drop);
                }

                // ??保留一部分血Dust喷发（混乱感）
                for (int i = 0; i < 20; i++)
                {
                    Vector2 vel = Main.rand.NextVector2Circular(9f, 9f);
                    Dust d = Dust.NewDustPerfect(center, DustID.Blood, vel, 0, Color.Red, Main.rand.NextFloat(1.3f, 1.9f));
                    d.noGravity = Main.rand.NextBool();
                }

            }
        }






    }
}
