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
using Terraria.DataStructures;
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.DPreDog.DaemonsFlameC
{
    internal class DaemonsFlameProjRed : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.DPreDog";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public bool ableToHit = true;
        public NPC target;

        // 让每个弹幕都为一个独立的个体，各自之间都会相互随机
        protected override bool CloneNewInstances => true;

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
            Projectile.timeLeft = 350;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 50;
            Projectile.DamageType = DamageClass.Ranged;
        }
        public ref float Time => ref Projectile.ai[1];
        public override bool? CanDamage() => Time >= 20f; // 初始的时候不会造成伤害，直到x为止
        public override void OnSpawn(IEntitySource source)
        {
            // 初始化旋转角度（用于螺旋偏移）
            Projectile.localAI[1] = Main.rand.NextFloat(MathHelper.TwoPi);

            // 搜索最近敌人并记录 ID 到 ai[0]
            NPC closest = null;
            float minDist = 1200f;

            foreach (NPC npc in Main.npc)
            {
                if (npc.CanBeChasedBy() && !npc.friendly && !npc.dontTakeDamage)
                {
                    float dist = Vector2.Distance(npc.Center, Projectile.Center);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        closest = npc;
                    }
                }
            }

            if (closest != null)
            {
                Projectile.ai[0] = closest.whoAmI; // ? 记录目标 NPC ID
                target = closest;
            }
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.localAI[0] += 1f / (Projectile.extraUpdates + 1);
            Time++;
            BowChangeVFX.SpawnTrail(Projectile, BowChangeTheme.DaemonsFlame, 0.5f);

            // 目标锁定并存在
            if (target == null || !target.active || target.whoAmI != (int)Projectile.ai[0])
            {
                int id = (int)Projectile.ai[0];
                if (id >= 0 && id < Main.npc.Length && Main.npc[id].active)
                    target = Main.npc[id];
            }

            // 如果成功追踪
            if (target != null && target.active)
            {
                // 核心旋转追踪逻辑 ?
                Vector2 toTarget = target.Center - Projectile.Center;
                float baseSpeed = 18f;
                float spiralRadius = 16f * 5f;

                // 更新旋转角度（每帧 +θ）
                Projectile.localAI[1] += 0.12f;

                // 增加扰动方向
                Vector2 spiralOffset = Projectile.localAI[1].ToRotationVector2() * spiralRadius;

                Vector2 desiredVelocity = (toTarget + spiralOffset).SafeNormalize(Vector2.UnitX) * baseSpeed;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 0.12f);
            }

            {

                // 命中后触发衰减行为
                if (Projectile.penetrate < 200)
                {
                    if (Projectile.timeLeft > 60)
                        Projectile.timeLeft = 60;

                    Projectile.velocity *= 0.88f;
                }

                if (Projectile.timeLeft <= 20)
                    ableToHit = false;
            }

        }


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D lightTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/SmallGreyscaleCircle").Value;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float colorInterpolation = (float)Math.Cos(Projectile.timeLeft / 32f + Main.GlobalTimeWrappedHourly / 20f + i / (float)Projectile.oldPos.Length * MathHelper.Pi) * 0.5f + 0.5f;
                Color color = Color.Lerp(Color.Red, Color.DarkRed, colorInterpolation) * 0.8f;  // **调整颜色渐变**
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
