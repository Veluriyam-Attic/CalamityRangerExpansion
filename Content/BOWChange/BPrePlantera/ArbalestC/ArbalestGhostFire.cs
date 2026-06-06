using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using CalamityMod.Particles;
using CalamityMod.Graphics.Primitives;
using CalamityMod;

namespace CalamityRangerExpansion.Content.BOWChange.BPrePlantera.ArbalestC
{
    public class ArbalestGhostFire : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.BPrePlantera";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 215;
            Projectile.extraUpdates = 3;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;
            Projectile.alpha = 255;
        }

        private float curveStrength = 0.05f;
        private float homingLerp = 0.05f;
        private const int LerpStartTime = 35;

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            // 初期螺旋扰动
            if (Projectile.timeLeft > 180)
            {
                float spin = (float)Math.Sin(Main.GameUpdateCount * 0.1f + Projectile.whoAmI) * curveStrength;
                Projectile.velocity = Projectile.velocity.RotatedBy(spin);
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX) * 6f;
            }
            else
            {
                NPC target = FindTarget(Projectile.Center, 460f);
                if (target != null)
                {
                    Vector2 toTarget = target.Center - Projectile.Center;
                    Vector2 desired = Vector2.Lerp(Projectile.velocity, toTarget.SafeNormalize(Vector2.Zero) * 6f, homingLerp);
                    Projectile.velocity = desired;
                }
            }

            //// Green Dust
            //if (Main.rand.NextBool(2))
            //{
            //    for (int i = 0; i < 2; i++)
            //    {
            //        Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GreenTorch,
            //            -Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.2f, 0.4f),
            //            100, Color.LawnGreen, 1.4f);
            //        d.noGravity = true;
            //    }
            //}

            // 十字星粒子
            if (Main.rand.NextBool(3))
            {
                Vector2 dir = Projectile.velocity.SafeNormalize(Vector2.UnitX);
                Vector2 sparkVelocity = dir.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4)) * 2f;
                CritSpark spark = new CritSpark(
                    Projectile.Center,
                    sparkVelocity + Main.player[Projectile.owner].velocity,
                    Color.White,
                    Color.LightGreen,
                    0.5f,
                    16
                );
                GeneralParticleHandler.SpawnParticle(spark);
            }
        }

        private NPC FindTarget(Vector2 pos, float range)
        {
            NPC best = null;
            float minDist = range;

            foreach (var npc in Main.npc)
            {
                if (!npc.CanBeChasedBy()) continue;
                float dist = Vector2.Distance(npc.Center, pos);
                if (dist < minDist)
                {
                    minDist = dist;
                    best = npc;
                }
            }

            return best;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.CursedInferno, 300);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 14; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GreenTorch, Main.rand.NextVector2Circular(3f, 3f), 100, Color.GreenYellow, 1.3f);
                d.noGravity = true;
            }
        }

        public Color TrailColor(float completionRatio, Vector2 vertexPosition)
        {
            float opacity = Utils.GetLerpValue(0f, 0.1f, completionRatio, true) * Utils.GetLerpValue(0.75f, 0.55f, completionRatio, true);
            Color start = Color.Lerp(Color.White, Color.LightGreen, 0.45f);
            Color mid = Color.LightGreen;
            Color end = Color.Transparent;
            return CalamityUtils.MulticolorLerp(completionRatio, start, mid, end) * opacity;
        }

        public float TrailWidth(float completionRatio, Vector2 vertexPosition)
        {
            return MathHelper.SmoothStep(Projectile.width * 1.2f, 4.5f, completionRatio);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            GameShaders.Misc["CalamityMod:ImpFlameTrail"]
                .SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/EternityStreak"));
            PrimitiveRenderer.RenderTrail(Projectile.oldPos,
                new(TrailWidth, TrailColor, (_, _) => Projectile.Size * 0.5f, shader: GameShaders.Misc["CalamityMod:ImpFlameTrail"]), 74);
            return false;
        }
    }
}
