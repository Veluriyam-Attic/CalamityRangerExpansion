using CalamityRangerExpansion.Content.BOWChange.BPrePlantera.CorrodedCaustibowC;
using CalamityRangerExpansion.Content.BOWChange;
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
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Graphics.Metaballs;
using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityRangerExpansion.Content.BOWChange.EAfterDog.DeathwindC
{
    public class DeathwindRePROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.EAfterDog";
        private const float LaserLength = 80f;
        private const float LaserLengthChangeRate = 2f;

        private const float WaveTheta = 0.09f; // 每帧旋转弧度
        private const int WaveTwistFrames = 9; // 波动周期长度

        private ref float WaveFrameState => ref Projectile.localAI[1];

        public override string Texture => "CalamityMod/Projectiles/LaserProj"; // 可替换为你自己的贴图
        public ref float LightPower => ref Projectile.ai[2];

        public Color sparkColor;
        public int Time = 0;
        public ref int audioCooldown => ref Main.player[Projectile.owner].Calamity().PhotoAudioCooldown;
        public ref int PhotoTimer => ref Main.player[Projectile.owner].Calamity().PhotoTimer;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 50;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            // 选择贴图
            bool isBlue = Projectile.ai[2] < 0.5f;
            Texture2D texNormal = ModContent.Request<Texture2D>(
                isBlue ? "CalamityRangerExpansion/Content/BOWChange/EAfterDog/DeathwindC/DeathwindRePROJBlue" :
                         "CalamityRangerExpansion/Content/BOWChange/EAfterDog/DeathwindC/DeathwindRePROJPurple"
            ).Value;

            // 模拟短暂故障（每 45 帧一次，切换两下）
            int flashTime = (int)(Main.GameUpdateCount % 45);
            if (flashTime < 5 || (flashTime >= 10 && flashTime < 15))
                isBlue = !isBlue;

            Color baseColor = isBlue ? Color.Cyan : Color.MediumPurple;
            baseColor *= 0.85f;
            baseColor.A = 0;

            Vector2 origin = texNormal.Size() * 0.5f;
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);

            // 绘制主体贴图
            spriteBatch.Draw(texNormal, drawPos, null, baseColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);

            // 星星贴图动画
            Texture2D starTex = ModContent.Request<Texture2D>("CalamityRangerExpansion/texture/KsTexture/star_07").Value;
            float pulse = 0.9f + 0.1f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f);
            float starRot = -Main.GlobalTimeWrappedHourly * 2.1f;
            Vector2 starDrawPos = Projectile.Center + new Vector2(0f, -20f).RotatedBy(Projectile.rotation) - Main.screenPosition;

            Color starColor = isBlue ? Color.Cyan : Color.MediumPurple;
            starColor *= 0.7f;
            starColor.A = 0;

            spriteBatch.Draw(starTex, starDrawPos, null, starColor, starRot, starTex.Size() * 0.5f, 0.6f * pulse, SpriteEffects.None, 0f);

            return false;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = 5;
            Projectile.timeLeft = 150;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 6;
            Projectile.scale = 2;
        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
            Projectile.ai[2] = Projectile.ai[2] < 0.5f ? 0f : 1f;

            Vector2 baseVel = Projectile.velocity.SafeNormalize(Vector2.UnitX) * Projectile.velocity.Length();
            Player player = Main.player[Projectile.owner];

            // 生成 ? 个附属曲线弹幕
            for (int i = -5; i <= 5; i++)
            {
                Vector2 offset = Vector2.UnitY.RotatedBy(Projectile.rotation + MathHelper.PiOver2) * i * 10f;
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center + offset,
                    baseVel,
                    ModContent.ProjectileType<DeathwindRePROJAround>(),
                    Projectile.damage / 2,
                    Projectile.knockBack,
                    player.whoAmI,
                    ai0: 0f,
                    ai1: 0f,
                    ai2: Projectile.ai[2]
                );
            }

            // 随机决定贴图颜色
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            BowChangeVFX.SpawnTrail(Projectile, BowChangeTheme.Deathwind, 0.6f);

            {
                // ??判断主题颜色（蓝 / 紫）
                bool isBlue = Projectile.ai[2] < 0.5f;
                Color dustColor = isBlue ? Color.Cyan : Color.MediumPurple;
                int dustID = isBlue ? DustID.Electric : DustID.PurpleTorch;

                // ??环绕型 Dust：围绕 rotation 每帧转动 3 个点
                float circleRadius = 12f;
                int ringCount = 3;
                float circleSpeed = 0.3f;

                for (int i = 0; i < ringCount; i++)
                {
                    float angle = Main.GameUpdateCount * circleSpeed + i * MathHelper.TwoPi / ringCount;
                    Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * circleRadius;
                    offset = offset.RotatedBy(Projectile.rotation); // ?? 绑定 rotation

                    Vector2 pos = Projectile.Center + offset;

                    Dust d = Dust.NewDustPerfect(pos, dustID, Vector2.Zero, 150, dustColor, 1.4f);
                    d.noGravity = true;
                    d.velocity = offset.SafeNormalize(Vector2.Zero) * 0.3f; // 有一点离心力
                }

                // ??中心脉冲 Dust（每 2 帧生成一撮，模拟能量脉冲）
                if (Main.GameUpdateCount % 2 == 0)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        Vector2 dir = (MathHelper.TwoPi * i / 6f).ToRotationVector2().RotatedBy(Projectile.rotation);
                        Vector2 velocity = dir * Main.rand.NextFloat(1.5f, 3.5f);

                        Dust core = Dust.NewDustPerfect(Projectile.Center, dustID, velocity, 100, dustColor, 1.1f);
                        core.noGravity = true;
                    }
                }

                // ?电子闪星（每 5 帧触发 2 个）带有光粒
                if (Main.GameUpdateCount % 5 == 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 randOffset = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(8f, 18f);
                        Vector2 pos = Projectile.Center + randOffset;
                        Vector2 vel = -randOffset.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(2f, 4f);

                        Dust flash = Dust.NewDustPerfect(pos, DustID.GemAmethyst, vel, 80, dustColor * 0.8f, 1.6f);
                        flash.noGravity = true;
                    }
                }


            }
        }



        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 120);
            BowChangeVFX.SpawnImpact(Projectile, BowChangeTheme.Deathwind, 0.95f);
            if (audioCooldown == 0)
            {
                //SoundEngine.PlaySound(Photoviscerator.HitSound, target.Center);
                audioCooldown = 16;
            }

            {
                Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.UnitX);
                // ?? 从身后远处生成冲击波 DeathwindCJB，打向自己
                Vector2 backOffset = -direction.RotatedByRandom(MathHelper.ToRadians(10f)) * 1200f; // 远处带轻微角度扰动
                Vector2 sourcePos = Projectile.Center + backOffset;
                Vector2 toSelf = (Projectile.Center - sourcePos).SafeNormalize(Vector2.UnitY) * 35f;

                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    sourcePos,
                    toSelf,
                    ModContent.ProjectileType<DeathwindCJB>(),
                    (int)(Projectile.damage * 0.75f),
                    Projectile.knockBack * 2f,
                    Projectile.owner
                );
            }

            {
                // ?? 计算正前方偏移位置
                Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.UnitX);
                Vector2 sparkOrigin = Projectile.Center + direction * 40f;

                if (Projectile.owner == Main.myPlayer && Main.rand.NextBool(2))
                {
                    Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        target.Center,
                        direction * 1.2f,
                        ModContent.ProjectileType<DeathwindReColorPrism>(),
                        (int)(Projectile.damage * 0.5f),
                        Projectile.knockBack,
                        Projectile.owner,
                        Projectile.ai[2],
                        target.whoAmI);
                }

                bool isBlue = Projectile.ai[2] < 0.5f;
                Color sparkColor = isBlue ? Color.Cyan : Color.MediumPurple;
                int dustType = isBlue ? DustID.Electric : DustID.PurpleTorch;

                // ??生成 Dust 爆裂效果
                for (int i = 0; i < 30; i++)
                {
                    Vector2 dustVel = direction.RotatedByRandom(MathHelper.ToRadians(25f)) * Main.rand.NextFloat(6f, 12f);
                    Dust d = Dust.NewDustPerfect(sparkOrigin, dustType, dustVel, 100, sparkColor, 1.5f);
                    d.noGravity = true;
                }

                // ??生成 Spark 粒子束（更亮）
                for (int i = 0; i < 6; i++)
                {
                    Vector2 sparkVel = direction.RotatedByRandom(MathHelper.ToRadians(20f)) * Main.rand.NextFloat(8f, 14f);
                    PointParticle p = new PointParticle(
                        sparkOrigin,
                        sparkVel,
                        false,
                        30,
                        1.2f,
                        sparkColor
                    );
                    GeneralParticleHandler.SpawnParticle(p);
                }

                // 音效播放控制
                if (audioCooldown == 0)
                {
                    //SoundEngine.PlaySound(Photoviscerator.HitSound, target.Center);
                    audioCooldown = 16;
                }
            }
        }
        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];

            // ?? 随机初始角度
            float baseAngle = Main.rand.NextFloat(MathHelper.TwoPi);

            for (int i = 0; i < 4; i++)
            {
                float angle = baseAngle + MathHelper.TwoPi * i / 4f;
                Vector2 velocity = angle.ToRotationVector2() * 12f;

                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    velocity,
                    ModContent.ProjectileType<DeathwindCJB>(),
                    Projectile.damage,
                    Projectile.knockBack,
                    player.whoAmI
                );
            }

            // ?? 爆炸特效颜色根据主题决定
            bool isBlue = Projectile.ai[2] < 0.5f;
            Color coreColor = isBlue ? Color.Cyan : Color.MediumPurple;
            Color glowColor = isBlue ? Color.White : Color.DeepPink;
            int dustID = isBlue ? DustID.Electric : DustID.PurpleTorch;

            Vector2 center = Projectile.Center;

            // ?? 生成大量 Dust
            for (int i = 0; i < 30; i++)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(4f, 10f);
                Dust d = Dust.NewDustPerfect(center, dustID, velocity, 100, coreColor, Main.rand.NextFloat(1.4f, 2.1f));
                d.noGravity = true;
            }

            // ?? 生成 PointParticle（爆点核心粒）
            for (int i = 0; i < 8; i++)
            {
                Vector2 dir = Main.rand.NextVector2Circular(1f, 1f).SafeNormalize(Vector2.Zero);
                PointParticle p = new PointParticle(
                    center + dir * 10f,
                    dir * Main.rand.NextFloat(5f, 8f),
                    false,
                    20,
                    1.2f,
                    coreColor
                );
                GeneralParticleHandler.SpawnParticle(p);
            }

            // ?? 生成 AltSpark 环绕粒子
            for (int i = 0; i < 6; i++)
            {
                Vector2 vel = Main.rand.NextVector2CircularEdge(1f, 1f) * 5f;

                AltSparkParticle alt = new AltSparkParticle(
                    center,
                    vel,
                    false, // 不受重力
                    30,    // 生命周期
                    0.5f,  // 缩放
                    coreColor // 单一颜色
                );

                GeneralParticleHandler.SpawnParticle(alt);
            }


            // ? 十字星闪爆
            for (int i = 0; i < 3; i++)
            {
                GenericSparkle sparkle = new GenericSparkle(
                    center,
                    Vector2.Zero,
                    coreColor,
                    glowColor,
                    Main.rand.NextFloat(1.8f, 2.5f),
                    5,
                    Main.rand.NextFloat(-0.02f, 0.02f),
                    1.68f
                );
                GeneralParticleHandler.SpawnParticle(sparkle);
            }

            // ??（可选）播放爆炸音效
            SoundEngine.PlaySound(SoundID.Item74, center);

            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    center,
                    Vector2.Zero,
                    ModContent.ProjectileType<DeathwindReColorPrism>(),
                    (int)(Projectile.damage * 0.45f),
                    Projectile.knockBack,
                    Projectile.owner,
                    Projectile.ai[2],
                    -1f);
            }
        }






    }
}
