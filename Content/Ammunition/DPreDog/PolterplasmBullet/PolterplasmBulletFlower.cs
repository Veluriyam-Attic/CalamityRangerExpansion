using CalamityMod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Particles;
using CalamityRangerExpansion.CREConfigs;
using Terraria.GameContent;

namespace CalamityRangerExpansion.Content.Ammunition.DPreDog.PolterplasmBullet
{
    public class PolterplasmBulletFlower : ModProjectile, ILocalizedModType
    {
        public override string Texture => "CalamityRangerExpansion/Content/Ammunition/DPreDog/PolterplasmBullet/PolterplasmBulletFlower";
        public new string LocalizationCategory => "Projectile.DPreDog";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                Texture2D lightTexture = TextureAssets.Projectile[Projectile.type].Value;
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    float colorInterpolation = (float)Math.Cos(Projectile.timeLeft / 32f + Main.GlobalTimeWrappedHourly / 20f + i / (float)Projectile.oldPos.Length * MathHelper.Pi) * 0.5f + 0.5f;
                    Color color = Color.Lerp(Color.Blue, Color.WhiteSmoke, colorInterpolation) * 0.4f;
                    color.A = 0;
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
                    Main.EntitySpriteDraw(lightTexture, drawPosition, null, outerColor, Projectile.rotation, lightTexture.Size() * 0.5f, outerScale * 0.6f, SpriteEffects.None, 0);
                    Main.EntitySpriteDraw(lightTexture, drawPosition, null, innerColor, Projectile.rotation, lightTexture.Size() * 0.5f, innerScale * 0.6f, SpriteEffects.None, 0);
                }
                return false;
            }        
            return false;
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 50;
            Projectile.penetrate = 200;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            // 由于我们是水平贴图，因此什么也不需要转动
            Projectile.rotation = Projectile.velocity.ToRotation();

            // 添加光效
            Lighting.AddLight(Projectile.Center, Color.Lerp(Color.Blue, Color.AliceBlue, 0.5f).ToVector3() * 0.49f);

            // 子弹在出现之后很短一段时间会变得可见


            // 刚出现时不追踪，超过60帧后开始追踪敌人
            if (Projectile.ai[0] > 60)
            {
                NPC target = Projectile.Center.ClosestNPCAt(8800); // 查找范围内最近的敌人
                if (target != null)
                {
                    Vector2 direction = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * 4f, 0.08f); // 追踪速度为12f，调整跟随效果
                }
            }
            else
            {
                // 每一帧右拐 x 度
                Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(3));
                Projectile.ai[1]++;
            }

            if (Projectile.penetrate < 200) // 如果弹幕已经击中敌人，停止追踪能力
            {
                if (Projectile.timeLeft > 60)
                {
                    Projectile.timeLeft = 60; // 弹幕开始缩小并减速
                }
                Projectile.velocity *= 0.88f;
            }
            Time++;
        }
        public ref float Time => ref Projectile.ai[1];

        public override bool? CanDamage() => Time >= 60f; // 初始的时候不会造成伤害，直到x为止

        public override void OnSpawn(IEntitySource source)
        {

        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            player.AddBuff(ModContent.BuffType<PolterplasmBulletFlowerPBuff>(), 300); // 5 秒（300 帧）Buff
        }

        public override void OnKill(int timeLeft)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 在弹幕死亡位置释放轻型烟雾
                for (int i = 0; i < 3; i++)
                {
                    // 生成360度随机方向的速度
                    Vector2 dustVelocity = Main.rand.NextVector2Circular(4f, 4f);
                    // 设置颜色为x色
                    Color smokeColor = Color.AliceBlue;

                    // 创建并生成粒子
                    Particle smoke = new HeavySmokeParticle(Projectile.Center, dustVelocity * Main.rand.NextFloat(1f, 2.6f), smokeColor, 18, Main.rand.NextFloat(0.9f, 1.6f), 0.35f, Main.rand.NextFloat(-1, 1), true);
                    GeneralParticleHandler.SpawnParticle(smoke);
                }
            }
        }
    }
}
