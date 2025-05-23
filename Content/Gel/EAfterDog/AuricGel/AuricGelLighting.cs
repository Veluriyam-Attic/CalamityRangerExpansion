﻿using CalamityMod.Graphics.Primitives;
using CalamityMod;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria;

namespace CalamityRangerExpansion.Content.Gel.EAfterDog.AuricGel
{
    internal class AuricGelLighting : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectile.EAfterDog";

        public const int MaximumBranchingIterations = 3;
        public const float LightningTurnRandomnessFactor = 1.7f;
        public ref float InitialVelocityAngle => ref Projectile.ai[0];
        // Technically not a ratio, and more of a seed, but its used in a 0-2pi squash
        // later in the code to get an arbitrary unit vector (which is then checked).
        public ref float BaseTurnAngleRatio => ref Projectile.ai[1];
        public ref float AccumulatedXMovementSpeeds => ref Projectile.localAI[0];
        public ref float BranchingIteration => ref Projectile.localAI[1];
        /*public float Time
        {
            get => Projectile.localAI[0];
            set => Projectile.localAI[0] = value;
        }*/
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 50;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10000;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 2;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 10;
            Projectile.timeLeft = 60 * Projectile.extraUpdates; //  20代表把它的长度缩短到1/3 (默认60)？
            Projectile.DamageType = DamageClass.Ranged; // 远程伤害类型

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;

            // Readjust the velocity magnitude the moment this projectile is created
            // to make velocity setting outside the scope of this projectile less irritating
            // to consider alongside extraUpdate multipliers.
            // Also set the initial angle.
            if (Projectile.velocity != Vector2.Zero)
            {
                Projectile.velocity /= (Projectile.extraUpdates * 8f);
                //  * 4代表把它的长度缩短到1/4？
            }
        }
        public override void AI()
        {
            Projectile.frameCounter++;
            float adjustedTimeLife = Projectile.timeLeft / Projectile.MaxUpdates;
            Projectile.scale = (float)Math.Sin(MathHelper.Pi * Projectile.timeLeft / (45f * (Projectile.MaxUpdates - 1))) * 4f;
            if (Projectile.scale > 1f)
                Projectile.scale = 1f;

            Lighting.AddLight(Projectile.Center, Color.Blue.ToVector3());
            if (Projectile.frameCounter >= Projectile.extraUpdates * 2)
            {
                Projectile.frameCounter = 0;

                float originalSpeed = MathHelper.Min(6f, Projectile.velocity.Length());
                UnifiedRandom unifiedRandom = new UnifiedRandom((int)BaseTurnAngleRatio);
                int turnTries = 0;
                Vector2 newBaseDirection = -Vector2.UnitY;
                Vector2 potentialBaseDirection;

                do
                {
                    BaseTurnAngleRatio = unifiedRandom.Next() % 100;
                    potentialBaseDirection = (BaseTurnAngleRatio / 100f * MathHelper.TwoPi).ToRotationVector2();

                    // Ensure that the new potential direction base is always moving upwards (this is supposed to be somewhat similar to a -UnitY + RotatedBy).
                    potentialBaseDirection.Y = -Math.Abs(potentialBaseDirection.Y);

                    bool canChangeLightningDirection = true;

                    // Potential directions with very little Y speed should not be considered, because this
                    // consequentially means that the X speed would be quite large.
                    if (potentialBaseDirection.Y > -0.02f)
                        canChangeLightningDirection = false;

                    // This mess of math basically encourages movement at the ends of an extraUpdate cycle,
                    // discourages super frequenent randomness as the accumulated X speed changes get larger,
                    // or if the original speed is quite large.
                    if (Math.Abs(potentialBaseDirection.X * (Projectile.extraUpdates + 1) * 2f * originalSpeed + AccumulatedXMovementSpeeds) > Projectile.MaxUpdates * LightningTurnRandomnessFactor)
                    {
                        canChangeLightningDirection = false;
                    }

                    // If the above checks were all passed, redefine the base direction of the lightning.
                    if (canChangeLightningDirection)
                        newBaseDirection = potentialBaseDirection;

                    turnTries++;
                }
                while (turnTries < 20);

                if (Projectile.velocity != Vector2.Zero)
                {
                    AccumulatedXMovementSpeeds += newBaseDirection.X * (Projectile.extraUpdates + 1) * 2f * originalSpeed;
                    Projectile.velocity = newBaseDirection.RotatedBy(InitialVelocityAngle + MathHelper.PiOver2) * originalSpeed;
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                }
            }
        }

        internal float WidthFunction(float completionRatio)
        {
            float baseWidth = MathHelper.Lerp(4f, 7f, (float)Math.Sin(MathHelper.Pi * 4f * completionRatio) * 0.5f + 0.5f) * Projectile.scale;
            return baseWidth * (float)Math.Sin(MathHelper.Pi * completionRatio);
        }
        internal Color ColorFunction(float completionRatio)
        {
            Color baseColor = Color.Lerp(Color.Gold, Color.LightYellow, (float)Math.Sin(MathHelper.TwoPi * completionRatio + Main.GlobalTimeWrappedHourly * 4f) * 0.5f + 0.5f);
            return Color.Lerp(baseColor, Color.Yellow, ((float)Math.Sin(MathHelper.Pi * completionRatio + Main.GlobalTimeWrappedHourly * 4f) * 0.5f + 0.5f) * 0.8f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(WidthFunction, ColorFunction, (_) => Projectile.Size * 0.5f, false), 90);
            return false;
        }
    }
}
