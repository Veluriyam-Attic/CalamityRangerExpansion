using CalamityMod.Particles;
using Microsoft.Xna.Framework.Graphics;
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

namespace CalamityRangerExpansion.Content.BOWChange.BPrePlantera.CorrodedCaustibowC
{
    internal class CorrodedCaustibowReASpark : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.BPrePlantera";
        private bool stuck;
        private NPC stuckTarget;
        private float rotationOffset;

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 50; // 存活
            Projectile.extraUpdates = 12; 
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            {
                // ??细螺旋轨迹 Dust（原样保留）
                float spiralAngle1 = Main.GameUpdateCount * 0.25f;
                Vector2 spiralOffset2 = new Vector2(0f, 5f).RotatedBy(spiralAngle1);
                Dust spiralDust = Dust.NewDustPerfect(Projectile.Center + spiralOffset2, DustID.Demonite);
                spiralDust.noGravity = true;
                spiralDust.velocity = spiralOffset2.RotatedBy(MathHelper.PiOver2) * 0.15f;

                // ??推进轨道型 Dust（角度偏摆）
                for (int i = -1; i <= 1; i++)
                {
                    if (i == 0) continue;
                    Vector2 side = Projectile.velocity.RotatedBy(MathHelper.ToRadians(i * 12)).SafeNormalize(Vector2.UnitY);
                    Vector2 dustPos = Projectile.Center - side * 4f;

                    Dust stream = Dust.NewDustPerfect(dustPos, DustID.GreenTorch);
                    stream.velocity = -side * Main.rand.NextFloat(1.5f, 3f);
                    stream.scale = Main.rand.NextFloat(1f, 1.3f);
                    stream.noGravity = true;
                    stream.fadeIn = 1.1f;
                }

            }


            // ??细螺旋轨迹 Dust（单环绕）
            float spiralAngle = Main.GameUpdateCount * 0.25f;
            Vector2 spiralOffset = new Vector2(0f, 5f).RotatedBy(spiralAngle);
            Dust dust = Dust.NewDustPerfect(Projectile.Center + spiralOffset, DustID.Demonite);
            dust.noGravity = true;
            dust.velocity = spiralOffset.RotatedBy(MathHelper.PiOver2) * 0.15f;

            // ?尖刺Spark：前方两侧固定角度生成
            Vector2 front = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitY) * 8f;
            Vector2 sideL = Projectile.velocity.RotatedBy(-MathHelper.PiOver2).SafeNormalize(Vector2.UnitY) * 2.5f;
            Vector2 sideR = Projectile.velocity.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.UnitY) * 2.5f;

            if (Main.rand.NextBool(2))
            {
                SparkParticle spikeL = new SparkParticle(
                    front,
                    sideL,
                    false,
                    16,
                    0.6f,
                    Color.Lerp(Color.GreenYellow, Color.DeepSkyBlue, 0.5f)
                );
                SparkParticle spikeR = new SparkParticle(
                    front,
                    sideR,
                    false,
                    16,
                    0.6f,
                    Color.Lerp(Color.LimeGreen, Color.Purple, 0.5f)
                );
                GeneralParticleHandler.SpawnParticle(spikeL);
                GeneralParticleHandler.SpawnParticle(spikeR);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 播放爆炸音效
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);

            // ??额外爆炸粒子（立即释放）
            for (int i = 0; i < 12; i++)
            {
                Dust boom = Dust.NewDustPerfect(
                    Projectile.Center + Main.rand.NextVector2Circular(5f, 5f),
                    DustID.Demonite,
                    Main.rand.NextVector2Circular(4f, 4f),
                    150,
                    Color.Purple,
                    1.2f
                );
                boom.noGravity = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            // ??额外爆炸粒子（死时）
            for (int i = 0; i < 10; i++)
            {
                Dust boom = Dust.NewDustPerfect(
                    Projectile.Center + Main.rand.NextVector2Circular(6f, 6f),
                    DustID.PurpleTorch,
                    Main.rand.NextVector2Circular(3f, 3f),
                    150,
                    Color.LimeGreen,
                    1.3f
                );
                boom.noGravity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // 与地形碰撞直接消失
            return true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Rectangle frame = tex.Frame();
            Vector2 origin = frame.Size() / 2f;

            Main.EntitySpriteDraw(
                tex,
                Projectile.Center - Main.screenPosition,
                frame,
                Projectile.GetAlpha(lightColor),
                Projectile.rotation,
                origin,
                Projectile.scale,
                SpriteEffects.None,
                0
            );
            return false;
        }
    }
}