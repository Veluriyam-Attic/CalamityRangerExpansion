using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.DPreDog.DaemonsFlameC
{
    internal class DaemonsFlameProjWhite : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.DPreDog";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public bool ableToHit = true;
        public NPC target;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 200;
            Projectile.extraUpdates = 4;
            Projectile.timeLeft = 750;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 50;
            Projectile.DamageType = DamageClass.Ranged;
        }
        public ref float Time => ref Projectile.ai[1];
        public override bool? CanDamage() => Time >= 20f; // 初始的时候不会造成伤害，直到x为止


        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.localAI[0] += 1f / (Projectile.extraUpdates + 1);
            Time++;
            BowChangeVFX.SpawnTrail(Projectile, BowChangeTheme.DaemonsFlame, 0.45f);


            // 获取目标敌人
            if (target == null || !target.active || target.whoAmI != (int)Projectile.ai[0])
            {
                int id = (int)Projectile.ai[0];
                if (id >= 0 && id < Main.maxNPCs && Main.npc[id].active)
                    target = Main.npc[id];
            }

            // 若成功追踪敌人
            if (target != null && target.active && !target.friendly)
            {
                Vector2 toTarget = target.Center - Projectile.Center;
                float speed = MathHelper.Lerp(4f, 14f, Utils.GetLerpValue(600f, 64f, toTarget.Length(), true));
                Vector2 desiredVelocity = toTarget.SafeNormalize(Vector2.UnitY) * speed;

                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 0.08f);
            }
            else
            {
                // ??没有目标，逐渐停止
                Projectile.velocity *= 0.92f;

                if (Projectile.timeLeft > 60)
                    Projectile.timeLeft = 60; // 提前结束寿命
            }

            // ?幽灵轨迹尘埃特效
            if (Main.rand.NextBool(3))
            {
                Dust d = Dust.NewDustPerfect(
                    Projectile.Center + Main.rand.NextVector2Circular(2f, 2f),
                    DustID.WhiteTorch,
                    Projectile.velocity * 0.1f,
                    150,
                    Color.White,
                    Main.rand.NextFloat(1.0f, 1.4f)
                );
                d.noGravity = true;
            }

            if (Projectile.penetrate < 200) // 如果弹幕已经击中敌人，停止追踪能力
            {
                if (Projectile.timeLeft > 60) { Projectile.timeLeft = 60; } // 弹幕开始缩小并减速
                Projectile.velocity *= 0.88f;
            }

            if (Projectile.timeLeft <= 20) // 弹幕即将消失时停止造成伤害
            {
                ableToHit = false;
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D lightTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/SmallGreyscaleCircle").Value;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float colorInterpolation = (float)Math.Cos(Projectile.timeLeft / 32f + Main.GlobalTimeWrappedHourly / 20f + i / (float)Projectile.oldPos.Length * MathHelper.Pi) * 0.5f + 0.5f;
                Color color = Color.Lerp(Color.White, Color.WhiteSmoke, colorInterpolation) * 0.8f;  // **调整颜色渐变**
                color.A = 255;  // **确保透明度不会丢失**

                Vector2 drawPosition = Projectile.oldPos[i] + lightTexture.Size() * 0.5f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY) + new Vector2(-28f, -28f);

                Color outerColor = color;
                Color innerColor = color * 0.5f;

                float intensity = 0.9f + 0.15f * (float)Math.Cos(Main.GlobalTimeWrappedHourly % 60f * MathHelper.TwoPi);
                intensity *= MathHelper.Lerp(0.15f, 1f, 1f - i / (float)Projectile.oldPos.Length);

                if (Projectile.timeLeft <= 60) // 弹幕即将消失时缩小
                {
                    intensity *= Projectile.timeLeft / 60f;
                }

                Vector2 outerScale = new Vector2(1f) * intensity;
                Vector2 innerScale = new Vector2(1f) * intensity * 0.7f;

                outerColor *= intensity;
                innerColor *= intensity;

                Main.EntitySpriteDraw(lightTexture, drawPosition, null, outerColor, 0f, lightTexture.Size() * 0.5f, outerScale * 0.6f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(lightTexture, drawPosition, null, innerColor, 0f, lightTexture.Size() * 0.5f, innerScale * 0.6f, SpriteEffects.None, 0);
            }
            return false;
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            BowChangeVFX.SpawnImpact(Projectile, BowChangeTheme.DaemonsFlame, 0.75f);
            if (Projectile.owner == Main.myPlayer && Main.rand.NextBool(2))
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<DaemonsFlameReHexSeal>(), Math.Max(1, Projectile.damage / 3), 0f, Projectile.owner);
            }
        }
    }
}
