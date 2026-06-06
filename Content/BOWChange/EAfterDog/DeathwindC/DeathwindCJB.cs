using CalamityMod;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;
using CalamityMod.Particles;

namespace CalamityRangerExpansion.Content.BOWChange.EAfterDog.DeathwindC
{
    internal class DeathwindCJB : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.EAfterDog";
        private static readonly Color ShaderColorOne = new Color(130, 80, 255);
        private static readonly Color ShaderColorTwo = new Color(80, 220, 255);
        private static readonly Color ShaderEndColor = new Color(255, 255, 255);

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 120;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 200;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.alpha = 120;
            Projectile.timeLeft = 800;
            Projectile.tileCollide = false; 
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 6;
            Projectile.extraUpdates = 4;
        }

        public override void AI()
        {
            Projectile.localAI[1]++; // 作为飞行状态的计时器

            if (Projectile.localAI[1] < 36f)
            {
                // 前 36 帧，每帧向左拐 1°
                Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(-1f));
            }
            else if (Projectile.localAI[1] >= 60f && Projectile.localAI[1] < 96f)
            {
                // 接下来的 36 帧，每帧向右拐 1°
                Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(1f));
            }
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item92, Projectile.position);
                Projectile.localAI[0] += 1f;
            }
            Lighting.AddLight(Projectile.Center, 0.2f, 0.15f, 0.4f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.velocity.Y += Projectile.ai[0];
        }

        private float PrimitiveWidthFunction(float completionRatio, Vector2 vertexPosition)
        {
            return MathHelper.Lerp(16f * 5f, 46f * 5f, completionRatio);
        }

        private Color PrimitiveColorFunction(float completionRatio, Vector2 vertexPosition)
        {
            float colorLerpFactor = 0.6f;
            float cosArgument = completionRatio * 2.7f - Main.GlobalTimeWrappedHourly * 5.3f;
            float startingInterpolant = (float)Math.Cos(cosArgument) * 0.5f + 0.5f;
            Color startingColor = Color.Lerp(ShaderColorOne, ShaderColorTwo, startingInterpolant * colorLerpFactor);
            return Color.Lerp(startingColor, ShaderEndColor, MathHelper.SmoothStep(0f, 1f, completionRatio));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            GameShaders.Misc["CalamityMod:TrailStreak"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/Trails/SylvestaffStreak"));

            Vector2 offset = Projectile.Size * 0.5f + Projectile.velocity * 1.4f;
            PrimitiveRenderer.RenderTrail(
                Projectile.oldPos,
                new PrimitiveSettings(PrimitiveWidthFunction, PrimitiveColorFunction, (_, _) => offset, shader: GameShaders.Misc["CalamityMod:TrailStreak"]),
                76
            );

            // 默认贴图
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 origin = texture.Size() * 0.5f;

            float pulse = 0.95f + 0.05f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 6f); // 呼吸脉动
            float shadowOffset = 4f;
            Color baseColor = Color.White * 0.9f;
            Color shadowColor = Color.Black * 0.4f;

            // ? 阴影层绘制（下层稍偏移）
            for (int i = 0; i < 4; i++)
            {
                float offsetRot = MathHelper.PiOver2 * i;
                Vector2 offset1 = new Vector2(shadowOffset, 0).RotatedBy(offsetRot);
                Main.spriteBatch.Draw(texture, drawPosition + offset1, null, shadowColor, Projectile.rotation, origin, Projectile.scale * pulse, SpriteEffects.None, 0f);
            }

            // 主体绘制
            Main.spriteBatch.Draw(texture, drawPosition, null, baseColor, Projectile.rotation, origin, Projectile.scale * pulse, SpriteEffects.None, 0f);



            // 本体叠加核心光圈纹理
            //Texture2D core1 = ModContent.Request<Texture2D>("CalamityRangerExpansion/texture/KsTexture/light_01").Value;
            //Texture2D core2 = ModContent.Request<Texture2D>("CalamityRangerExpansion/texture/KsTexture/light_02").Value;
            //Texture2D core3 = ModContent.Request<Texture2D>("CalamityRangerExpansion/texture/KsTexture/light_03").Value;
            //Texture2D mask7 = ModContent.Request<Texture2D>("CalamityRangerExpansion/texture/SuperTexturePack/fx_SmokeyHalo1").Value;
            //Texture2D mask18 = ModContent.Request<Texture2D>("CalamityRangerExpansion/texture/SuperTexturePack/fx_Smoke15").Value;

            //Vector2 drawPos = Projectile.Center - Main.screenPosition;
            //float pulse = 0.9f + 0.1f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 5f);
            //float rotation = Main.GlobalTimeWrappedHourly * 1.6f;
            //Color color = Color.LightPink * 0.8f;
            //Color blue = Color.LightBlue * 0.6f;

            //void DrawCore(Texture2D tex, float scale, Color col)
            //{
            //    Main.spriteBatch.Draw(tex, drawPos, null, col, rotation, tex.Size() * 0.5f, scale * pulse, SpriteEffects.None, 0f);
            //}

            //DrawCore(core1, 0.5f, color);
            //DrawCore(core2, 0.5f, color);
            //DrawCore(core3, 0.5f, color);
            //DrawCore(mask7, 0.6f, blue);
            //DrawCore(mask18, 0.6f, blue);



            return false;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, Projectile.alpha);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Vector2 center = Projectile.Center;

            // ?? 生成多个环绕旋转爆点粒子
            for (int i = 0; i < 32; i++)
            {
                float angle = MathHelper.TwoPi * i / 32f;
                Vector2 vel = angle.ToRotationVector2() * Main.rand.NextFloat(4f, 10f);

                Dust d = Dust.NewDustPerfect(center, DustID.PurpleTorch, vel, 100, Color.MediumPurple, Main.rand.NextFloat(1.2f, 2f));
                d.noGravity = true;
            }

            // ?? 爆点核心多重 Star 特效
            for (int i = 0; i < 3; i++)
            {
                GenericSparkle sparkle = new GenericSparkle(
                    center,
                    Vector2.Zero,
                    Color.Cyan,
                    Color.White,
                    Main.rand.NextFloat(2f, 3.5f),
                    5,
                    Main.rand.NextFloat(-0.03f, 0.03f),
                    2.4f
                );
                GeneralParticleHandler.SpawnParticle(sparkle);
            }

            // ? 十字方向 Spark 延迟发射
            for (int i = 0; i < 4; i++)
            {
                Vector2 dir = (MathHelper.PiOver2 * i).ToRotationVector2();
                PointParticle p = new PointParticle(
                    center,
                    dir * 10f,
                    false,
                    25,
                    1.5f,
                    Color.LightBlue
                );
                GeneralParticleHandler.SpawnParticle(p);
            }

            // ?? 闪电炸光感 Dust
            for (int i = 0; i < 20; i++)
            {
                Vector2 vel = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(4f, 8f);
                Dust d = Dust.NewDustPerfect(center, DustID.Electric, vel, 100, Color.Cyan * 0.7f, 1.4f);
                d.noGravity = true;
            }

            // ?? 声音效果
            SoundEngine.PlaySound(SoundID.Item125, center);
        }
    
        public override void OnKill(int timeLeft)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.PurificationPowder, Projectile.oldVelocity.X * 0.5f, Projectile.oldVelocity.Y * 0.5f);
            }
        }

    }
}
