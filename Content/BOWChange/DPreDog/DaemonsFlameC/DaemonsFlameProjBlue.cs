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
using CalamityMod.Projectiles.Ranged;
using Terraria.DataStructures;
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.DPreDog.DaemonsFlameC
{
    internal class DaemonsFlameProjBlue : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.DPreDog";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public bool ableToHit = true;
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
            Projectile.timeLeft = 750;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 50;
            Projectile.DamageType = DamageClass.Ranged;
        }
        public ref float Time => ref Projectile.ai[1];
        public override bool? CanDamage() => Time >= 20f; // 初始的时候不会造成伤害，直到x为止

        private Projectile targetProjectile = null; // 当前追踪的鬼火弹幕
        private int wanderTime = 0; // 游荡时间计数器
        private const int searchCooldown = 30; // 游荡时间时长
        private bool recentlyHit = false; // 是否正在游荡
        public override void OnSpawn(IEntitySource source)
        {
            // 初次生成时锁定一个玩家标记的弹幕作为唯一目标
            float minDist = 900f;
            Projectile found = null;

            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (!proj.active || proj.owner != Projectile.owner)
                    continue;

                var effect = proj.GetGlobalProjectile<DaemonsFlameReEffect>();
                if (effect != null && effect.IsDaemonsFlameReArrow)
                {
                    float dist = Vector2.Distance(proj.Center, Projectile.Center);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        found = proj;
                    }
                }
            }

            targetProjectile = found;
        }


        private bool targetPermanentlyLost = false; // ? 一旦目标丢失后标记为永久丢失

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Time++;
            BowChangeVFX.SpawnTrail(Projectile, BowChangeTheme.DaemonsFlame, 0.45f);

            // ?? 命中后会游荡一段时间
            if (recentlyHit)
            {
                wanderTime++;
                if (wanderTime > searchCooldown)
                {
                    recentlyHit = false;
                    wanderTime = 0;
                }

                // 随机漂移效果
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Main.rand.NextVector2Circular(1.2f, 1.2f), 0.1f);
                return;
            }

            // ?? 若没有目标弹幕，尝试锁定（前提是还没丢失过目标）
            if (!targetPermanentlyLost)
            {
                if (targetProjectile == null || !targetProjectile.active ||
                    !targetProjectile.GetGlobalProjectile<DaemonsFlameReEffect>().IsDaemonsFlameReArrow)
                {
                    Projectile found = null;
                    float minDist = 900f;

                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile proj = Main.projectile[i];
                        if (!proj.active || proj.owner != Projectile.owner)
                            continue;

                        var effect = proj.GetGlobalProjectile<DaemonsFlameReEffect>();
                        if (effect != null && effect.IsDaemonsFlameReArrow)
                        {
                            float dist = Vector2.Distance(proj.Center, Projectile.Center);
                            if (dist < minDist)
                            {
                                minDist = dist;
                                found = proj;
                            }
                        }
                    }

                    if (found != null)
                    {
                        targetProjectile = found;
                    }
                    else
                    {
                        targetPermanentlyLost = true; // ? 没找到目标，永远丢失
                        targetProjectile = null;
                    }
                }
            }

            // ?? 执行追踪逻辑
            if (targetProjectile != null && targetProjectile.active)
            {
                Vector2 toTarget = targetProjectile.Center - Projectile.Center;

                float targetSpeed = targetProjectile.velocity.Length();
                float desiredSpeed = Math.Max(targetSpeed * 1.2f, 12f);
                Vector2 desiredVelocity = toTarget.SafeNormalize(Vector2.UnitX) * desiredSpeed;

                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 0.1f);

                // 手动碰撞检测
                if (!recentlyHit && Projectile.Hitbox.Intersects(targetProjectile.Hitbox))
                {
                    recentlyHit = true;
                    wanderTime = 0;
                    Projectile.velocity *= 0.6f;
                }
            }
            else
            {
                // ?目标永久失效，执行直线飞行逻辑
                Projectile.velocity *= 1.02f; // 缓慢加速直线飞行
            }

            {
                // ?生命周期收尾
                if (Projectile.penetrate < 200 && Projectile.timeLeft > 60)
                    Projectile.timeLeft = 60;

                if (Projectile.timeLeft <= 20)
                    ableToHit = false;

                // ?? 粒子特效
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
            }
        }


        public override bool? CanHitNPC(NPC target) => Time >= 20f;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D lightTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/SmallGreyscaleCircle").Value;
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                float colorInterpolation = (float)Math.Cos(Projectile.timeLeft / 32f + Main.GlobalTimeWrappedHourly / 20f + i / (float)Projectile.oldPos.Length * MathHelper.Pi) * 0.5f + 0.5f;
                Color color = Color.Lerp(Color.Blue, Color.Cyan, colorInterpolation) * 0.8f;  // **调整颜色渐变**
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
