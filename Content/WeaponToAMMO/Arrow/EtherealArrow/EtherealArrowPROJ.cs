using CalamityMod.Projectiles;
using CalamityMod;
using CalamityRangerExpansion.CREConfigs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework.Graphics;
using CalamityRangerExpansion.LightingBolts;

namespace CalamityRangerExpansion.Content.WeaponToAMMO.Arrow.EtherealArrow
{
    internal class EtherealArrowPROJ : ModProjectile, ILocalizedModType
    {
        public override string Texture => "CalamityRangerExpansion/Content/WeaponToAMMO/Arrow/EtherealArrow/EtherealArrow";
        public new string LocalizationCategory => "WeaponToAMMO.Arrow.MonstrousArrow";
        //public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private int aiPhase = 0; // 当前阶段
        private float playerMaxMinions; // 玩家召唤栏位数
        private int playerMaxTurrets; // 玩家炮台栏位数

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }


        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 2;
            Projectile.timeLeft = 500;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;
        }
        public override void OnSpawn(IEntitySource source)
        {
            // 调整弹幕方向，随机偏移 -X 到 X 度
            float randomAngle = Main.rand.NextFloat(-MathHelper.Pi / 12, MathHelper.Pi / 12);
            Projectile.velocity = Projectile.velocity.RotatedBy(randomAngle);
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            playerMaxMinions = player.maxMinions;

            switch (aiPhase)
            {
                case 0: // 阶段1：逐渐减速
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;
                    Lighting.AddLight(Projectile.Center, Color.Blue.ToVector3() * 0.55f);
                    Projectile.velocity *= 0.895f;
                    if (++Projectile.ai[0] >= 55) // 持续的时间
                    {
                        aiPhase = 1;
                        Projectile.ai[0] = 0;
                    }
                    break;

                case 1: // 阶段2：寻找目标，旋转朝向
                    Projectile.rotation += 0.25f;
                    Lighting.AddLight(Projectile.Center, Color.Blue.ToVector3() * 0.55f);
                    NPC target = Projectile.Center.ClosestNPCAt(2800);
                    if (target != null)
                    {
                        float targetRotation = (target.Center - Projectile.Center).ToRotation();
                        float difference = MathHelper.WrapAngle(targetRotation - Projectile.rotation);
                        Projectile.rotation += MathHelper.Clamp(difference, -MathHelper.ToRadians(1), MathHelper.ToRadians(1)) + MathHelper.PiOver2 + MathHelper.Pi;
                    }
                    if (++Projectile.ai[0] >= 15)
                    {
                        aiPhase = 2;
                        Projectile.ai[0] = 0;
                    }
                    break;

                case 2: // 阶段3：追踪目标
                    playerMaxMinions = player.maxMinions;
                    playerMaxTurrets = player.maxTurrets;
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;
                    float X = 650 + playerMaxMinions * 150;
                    float Y = 14 + playerMaxMinions * 3;

                    target = Projectile.Center.ClosestNPCAt(X);
                    if (target != null)
                    {
                        Vector2 direction = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * Y, 0.08f);
                    }
                    break;
            }
        }
        public override void OnKill(int timeLeft)
        {
            LightingBoltsSystem.Spawn_SpectralWhispers(Projectile.Center);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Player player = Main.player[Projectile.owner];
            playerMaxTurrets = player.maxTurrets;
            float damageMultiplier = 1.0f + playerMaxTurrets * 0.5f;
            modifiers.FinalDamage *= damageMultiplier;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            playerMaxTurrets = player.maxTurrets;
            int particleCount = 30 + playerMaxTurrets * 10;

            // 生成6条线，每条线的角度均匀分布
            for (int i = 0; i < 6; i++)
            {
                float angle = MathHelper.TwoPi / 6 * i; // 每条线的起始角度
                Vector2 lineDirection = angle.ToRotationVector2(); // 线的方向

                // 在该方向上生成 `particleCount` 个粒子，每个粒子间距为1像素
                for (int j = 0; j < particleCount; j++)
                {
                    Dust.NewDustPerfect(
                        Projectile.Center + lineDirection * j, // 每个粒子的位置
                        Main.rand.Next(new[] { 206, 180, 172 }), // 粒子类型
                        lineDirection * Main.rand.NextFloat(1f, 3f), // 粒子速度
                        150,
                        default,
                        Main.rand.NextFloat(0.8f, 1.5f) // 粒子大小
                    ).noGravity = true; // 无重力效果
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (aiPhase < 2)
            {
                // 背光效果绘制
                Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
                Vector2 origin = texture.Size() * 0.5f;
                Vector2 drawPosition = Projectile.Center - Main.screenPosition;

                float chargeOffset = 3f;
                Color chargeColor = Color.White * 0.6f;
                chargeColor.A = 0;

                for (int i = 0; i < 8; i++)
                {
                    Vector2 drawOffset = (MathHelper.TwoPi * i / 8f).ToRotationVector2() * chargeOffset;
                    Main.spriteBatch.Draw(texture, drawPosition + drawOffset, null, chargeColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
                }

                Main.spriteBatch.Draw(texture, drawPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
                return false;
            }
            else
            {
                Texture2D lightTexture = ModContent.Request<Texture2D>("CalamityRangerExpansion/Content/WeaponToAMMO/Arrow/EtherealArrow/EtherealArrow").Value;
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    float colorInterpolation = (float)Math.Cos(Projectile.timeLeft / 32f + Main.GlobalTimeWrappedHourly / 20f + i / (float)Projectile.oldPos.Length * MathHelper.Pi) * 0.5f + 0.5f;
                    Color color = Color.Lerp(Color.Cyan, Color.LightBlue, colorInterpolation) * 0.4f;
                    color.A = 0;

                    Vector2 drawPosition = Projectile.oldPos[i] + lightTexture.Size() * 0.5f - Main.screenPosition;
                    float intensity = 0.9f + 0.15f * (float)Math.Cos(Main.GlobalTimeWrappedHourly % 60f * MathHelper.TwoPi);
                    intensity *= MathHelper.Lerp(0.15f, 1f, 1f - i / (float)Projectile.oldPos.Length);
                    Vector2 scale = new Vector2(1f) * intensity * 0.6f;

                    Main.EntitySpriteDraw(lightTexture, drawPosition, null, color, Projectile.rotation, lightTexture.Size() * 0.5f, scale, SpriteEffects.None, 0);
                }
                return false;
            }
        }

        public override bool? CanDamage()
        {
            return aiPhase == 2;
        }
    }
}