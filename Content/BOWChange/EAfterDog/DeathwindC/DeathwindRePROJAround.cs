using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod;
using System;
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.EAfterDog.DeathwindC
{
    public class DeathwindRePROJAround : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.EAfterDog";

        private const float LaserLength = 80f;
        private const float LaserLengthChangeRate = 2f;
        private const float WaveTheta = 0.09f;
        private const int WaveTwistFrames = 9;

        private ref float WaveFrameState => ref Projectile.localAI[1];
        private bool IsPurple => Projectile.ai[2] >= 0.5f; // ai[2] = 0 蓝色，1 紫色

        public override string Texture => "CalamityMod/Projectiles/LaserProj"; // 贴图可换
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 30;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 1200;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = 5;
            Projectile.timeLeft = 240;
        }

        public override void AI()
        {
            // 设置颜色（仅第一次执行）
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                Projectile.ai[2] = Projectile.ai[2] < 0.5f ? 0f : 1f;
            }

            float waveSign = WaveFrameState < 0f ? -1f : 1f;

            // 初始化波动方向
            if (Math.Abs(WaveFrameState) < 1f)
            {
                float dirToUse = WaveFrameState == 0f ? (Main.rand.NextBool() ? -1f : 1f) : waveSign;
                waveSign = -dirToUse;
                WaveFrameState = dirToUse * WaveTwistFrames * 0.5f;

                float baseRotation = Projectile.velocity.ToRotation();
                for (int i = 0; i < Projectile.oldRot.Length; ++i)
                {
                    Projectile.oldRot[i] = baseRotation;
                    baseRotation += waveSign * WaveTheta;
                }
            }
            else if (Math.Abs(WaveFrameState) > WaveTwistFrames)
                WaveFrameState = -waveSign;
            else
                WaveFrameState += waveSign;

            // 曲线移动
            Projectile.velocity = Projectile.velocity.RotatedBy(waveSign * WaveTheta);
            Projectile.rotation = Projectile.velocity.ToRotation();
            BowChangeVFX.SpawnTrail(Projectile, BowChangeTheme.Deathwind, 0.35f);

            // 激光增长/缩短控制
            Projectile.localAI[0] += 10f;
            if (Projectile.localAI[0] > LaserLength)
                Projectile.localAI[0] = LaserLength;
            else
            {
                Projectile.localAI[0] -= LaserLengthChangeRate;
                if (Projectile.localAI[0] <= 0f)
                    Projectile.Kill();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 75);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return IsPurple
                ? new Color(200, 100, 255, Projectile.alpha) // 紫色
                : new Color(100, 255, 255, Projectile.alpha); // 蓝色
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return Projectile.DrawBeam(LaserLength, 2f, lightColor, curve: true);
        }

        public override void OnKill(int timeLeft)
        {
            int dustID = IsPurple ? DustID.PurpleTorch : DustID.BlueTorch;
            Vector2 pos = Projectile.Center - Projectile.velocity / 2f;
            Vector2 vel = Projectile.velocity / 4f;
            for (int i = 0; i < 4; ++i)
            {
                Dust d = Dust.NewDustDirect(pos, 0, 0, dustID, 0f, 0f, Scale: 1.5f);
                d.velocity += vel;
                d.velocity *= Main.rand.NextFloat(0.4f, 1f);
                d.noGravity = true;
            }
        }
    }
}
