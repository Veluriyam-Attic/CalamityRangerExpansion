using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Projectiles.DraedonsArsenal;
using Microsoft.Xna.Framework.Graphics;
using CalamityRangerExpansion.CREConfigs;
using Terraria.Audio;

namespace CalamityRangerExpansion.Content.Arrows.EAfterDog.AuricArrow
{
    public class AuricArrowPROJ : ModProjectile, ILocalizedModType
    {
        public override string Texture => "CalamityRangerExpansion/Content/Arrows/EAfterDog/AuricArrow/AuricArrow";

        public new string LocalizationCategory => "Projectile.EAfterDog";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 2;  // 穿透次数为2
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;  // 存活时间
            Projectile.light = 0.5f;
            Projectile.extraUpdates = 2;  // 更多帧更新
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
            Projectile.arrow = true;
        }

        public override void AI()
        {
            // 控制旋转方向
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 产生DNA形状的粒子特效
                float frequency = 30f;  // 30帧一个回合
                float amplitude = 20f;  // 振动幅度

                Vector2 leftOffset = new Vector2(-amplitude * (float)Math.Sin(Projectile.ai[0] * MathHelper.TwoPi / frequency), 0);
                Vector2 rightOffset = new Vector2(amplitude * (float)Math.Sin(Projectile.ai[0] * MathHelper.TwoPi / frequency), 0);

                if (Projectile.ai[0] % 2 == 0)
                {
                    Dust.NewDustPerfect(Projectile.Center + leftOffset, DustID.GoldFlame, Vector2.Zero, 0, Color.Gold, 1.2f).noGravity = true;
                    Dust.NewDustPerfect(Projectile.Center + rightOffset, DustID.GoldFlame, Vector2.Zero, 0, Color.Gold, 1.2f).noGravity = true;
                }

            }
             

            Projectile.ai[0] += 2f;  // 更新ai，两倍的绘制速度
        }

        //public override bool PreDraw(ref Color lightColor)
        //{
        //    CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
        //    return false;
        //}


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            // 播放音效
            SoundEngine.PlaySound(SoundID.Item91, Projectile.position);

            // 1% 概率生成 AuricArrowPROJ 弹幕
            if (Main.rand.NextFloat() < 0.01f)
            {
                Vector2 spawnPosition = target.Center;
                Vector2 spawnVelocity = new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));

                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    spawnPosition,
                    spawnVelocity,
                    ModContent.ProjectileType<AuricArrowPROJ>(), // 生成的仍为 AuricArrowPROJ
                    Projectile.damage, // 继承当前弹幕的伤害
                    Projectile.knockBack,
                    Projectile.owner
                );
            }

            // 特效：法阵核心
            for (int i = 0; i < 10; i++) // 扩散波纹粒子
            {
                float angle = MathHelper.TwoPi / 10 * i; // 每个粒子的角度
                Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * Main.rand.NextFloat(60f, 80f); // 半径扩大为原来的3~4倍
                Dust waveParticle = Dust.NewDustPerfect(target.Center + offset, DustID.GoldFlame, Vector2.Zero, 0, Color.Goldenrod, 1.5f);
                waveParticle.noGravity = true; // 无重力
                waveParticle.fadeIn = 1f; // 逐渐消失
            }

            // 特效：符文环绕
            for (int i = 0; i < 5; i++) // 生成5个符文粒子
            {
                Vector2 runeOffset = new Vector2(Main.rand.NextFloat(90f, 120f), 0).RotatedByRandom(MathHelper.TwoPi); // 半径扩大为原来的3~4倍
                Dust runeParticle = Dust.NewDustPerfect(target.Center + runeOffset, DustID.IceTorch, runeOffset * -0.3f, 0, Color.LightYellow, 2f); // 调整速度
                runeParticle.noGravity = true; // 无重力
                runeParticle.fadeIn = 1f;
            }

            // 特效：电磁火花
            for (int i = 0; i < 15; i++) // 随机生成火花
            {
                Vector2 sparkVelocity = Main.rand.NextVector2Circular(6f, 8f); // 扩散速度提高为原来的3~4倍
                Dust sparkParticle = Dust.NewDustPerfect(target.Center, DustID.Electric, sparkVelocity, 100, Color.White, 1.2f);
                sparkParticle.noGravity = true; // 无重力
                sparkParticle.fadeIn = 1f;
            }

            #region 旧代码

            //// 生成爆炸弹幕
            //Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FuckYou>(), (int)(Projectile.damage * 0.25f), Projectile.knockBack, Projectile.owner);

            //// 往四周生成6个AuricArrowBALL
            //int numBalls = 6;
            //float angleStep = MathHelper.TwoPi / numBalls;
            //for (int i = 0; i < numBalls; i++)
            //{
            //    Vector2 velocity = new Vector2(2f, 0f).RotatedBy(angleStep * i);
            //    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<AuricArrowBALL>(), (int)(Projectile.damage * 0.75f), Projectile.knockBack, Projectile.owner);
            //}

            #endregion

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 检查是否已有 PlasmaGrenadeSmallExplosion 存在
                bool exists = false;
                foreach (Projectile proj in Main.projectile)
                {
                    if (proj.active && proj.type == ModContent.ProjectileType<PlasmaGrenadeSmallExplosion>())
                    {
                        exists = true;
                        break; // 如果已找到一个，则直接退出检查
                    }
                }

                // 如果不存在，则释放新的 PlasmaGrenadeSmallExplosion
                if (!exists)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PlasmaGrenadeSmallExplosion>(), 0, 0, Projectile.owner);
                }
            }
        }
        public override void OnKill(int timeLeft)
        {
         
        }


        public override bool PreDraw(ref Color lightColor)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 获取 SpriteBatch 和投射物纹理
                SpriteBatch spriteBatch = Main.spriteBatch;
                Texture2D lightTexture = ModContent.Request<Texture2D>("CalamityRangerExpansion/Content/Arrows/EAfterDog/AuricArrow/AuricArrow").Value;

                // 遍历投射物的旧位置数组，绘制光学拖尾效果
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    // 计算颜色插值值，使颜色在旧位置之间平滑过渡
                    float colorInterpolation = (float)Math.Cos(Projectile.timeLeft / 32f + Main.GlobalTimeWrappedHourly / 20f + i / (float)Projectile.oldPos.Length * MathHelper.Pi) * 0.5f + 0.5f;

                    // 使用金黄色渐变
                    Color color = Color.Lerp(Color.Gold, Color.Yellow, colorInterpolation) * 0.4f;
                    color.A = 0;

                    // 计算绘制位置，将位置调整到碰撞箱的中心
                    Vector2 drawPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

                    // 计算外部和内部的颜色
                    Color outerColor = color;
                    Color innerColor = color * 0.5f;

                    // 计算强度，使拖尾逐渐变弱
                    float intensity = 0.9f + 0.15f * (float)Math.Cos(Main.GlobalTimeWrappedHourly % 60f * MathHelper.TwoPi);
                    intensity *= MathHelper.Lerp(0.15f, 1f, 1f - i / (float)Projectile.oldPos.Length);
                    if (Projectile.timeLeft <= 60)
                    {
                        intensity *= Projectile.timeLeft / 60f; // 如果弹幕即将消失，则拖尾也逐渐消失
                    }

                    // 计算外部和内部的缩放比例，使拖尾具有渐变效果
                    Vector2 outerScale = new Vector2(2f) * intensity;
                    Vector2 innerScale = new Vector2(2f) * intensity * 0.7f;
                    outerColor *= intensity;
                    innerColor *= intensity;

                    // 绘制外部的拖尾效果，并应用旋转
                    Main.EntitySpriteDraw(lightTexture, drawPosition, null, outerColor, Projectile.rotation, lightTexture.Size() * 0.5f, outerScale * 0.6f, SpriteEffects.None, 0);

                    // 绘制内部的拖尾效果，并应用旋转
                    Main.EntitySpriteDraw(lightTexture, drawPosition, null, innerColor, Projectile.rotation, lightTexture.Size() * 0.5f, innerScale * 0.6f, SpriteEffects.None, 0);
                }

                // 绘制默认的弹幕，并应用旋转
                Main.EntitySpriteDraw(lightTexture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, lightColor, Projectile.rotation, lightTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

                return false;
            }
            return true;
        }
    }
}