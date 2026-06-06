using CalamityMod;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using CalamityMod.Buffs.DamageOverTime;
using CalamityRangerExpansion.Content.BOWChange.CPreMoodLord.VernalBolterC;

namespace CalamityRangerExpansion.Content.BOWChange.APreHardMode.LunarianBowC.废弃
{
    internal class LBCorruption : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.APreHardMode";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override bool PreDraw(ref Color lightColor)
        {

            // 根据方案选择不同的效果
            if (Projectile.ai[0] == 1) // 方案 a
            {
                // 画残影效果
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
                return false;
            }
            else if (Projectile.ai[0] == 2) // 方案 b
            {
                // 获取 SpriteBatch 和投射物纹理
                SpriteBatch spriteBatch = Main.spriteBatch;
                Texture2D lightTexture = ModContent.Request<Texture2D>("CalamityRangerExpansion/Content/BOWChange/APreHardMode/LunarianBowC/LBCorruption").Value;

                // 遍历投射物的旧位置数组，绘制光学拖尾效果
                for (int i = 0; i < Projectile.oldPos.Length; i++)
                {
                    // 计算颜色插值值，使颜色在旧位置之间平滑过渡
                    float colorInterpolation = (float)Math.Cos(Projectile.timeLeft / 32f + Main.GlobalTimeWrappedHourly / 20f + i / (float)Projectile.oldPos.Length * MathHelper.Pi) * 0.5f + 0.5f;

                    // 使用x色渐变
                    Color color = Color.Lerp(Color.Purple, Color.MediumPurple, colorInterpolation) * 0.4f;
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
                //Main.EntitySpriteDraw(lightTexture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, lightColor, Projectile.rotation, lightTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
                return false;
            }
            return false;
        }

        public override void SetDefaults()
        {
            // 设置弹幕的基础属性
            Projectile.width = 11; // 弹幕宽度
            Projectile.height = 24; // 弹幕高度
            Projectile.friendly = true; // 对敌人有效
            Projectile.DamageType = DamageClass.Ranged; // 远程伤害类型
            Projectile.penetrate = 1; // 穿透力为1，击中一个敌人就消失
            Projectile.timeLeft = 300; // 弹幕存在时间为x帧
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
            Projectile.ignoreWater = true; // 弹幕不受水影响
            Projectile.arrow = true;
            Projectile.extraUpdates = 1;
            // 根据方案设置 aiStyle
            if (Projectile.ai[0] == 1) // 方案 a：Venom
            {
                Projectile.aiStyle = ProjAIStyleID.Arrow;
            }
            else if (Projectile.ai[0] == 2) // 方案 b：Poisoned
            {
                Projectile.aiStyle = -1; // 自定义 AI
            }
        }
        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];

            // 检测玩家手持的武器并设置方案
            if (player.HeldItem.type == ModContent.ItemType<LunarianBowRe>()) // 检测是否是 LunarianBowRe
            {
                Projectile.ai[0] = 1; // 设置为方案 A
            }
            else if (player.HeldItem.type == ModContent.ItemType<VernalBolterRe>()) // 检测是否是 VernalBolterRe
            {
                Projectile.ai[0] = 2; // 设置为方案 B
            }
            else
            {
                Projectile.ai[0] = 1; // 默认方案 A
            }
        }

        public override void AI()
        {
            // 调整弹幕的旋转，使其在飞行时保持水平
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;

            // Lighting - 添加天蓝色光源，光照强度为 0.49
            Lighting.AddLight(Projectile.Center, Color.LightSkyBlue.ToVector3() * 0.49f);

            // 根据方案选择不同的特效
            if (Projectile.ai[0] == 1) // 方案 a
            {
                // 添加紫色 Corruption 粒子特效
                for (int i = 0; i < 2; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Corruption, Projectile.velocity * 0.2f, 100, default, 1.2f);
                    dust.noGravity = true;
                    dust.velocity *= 0.5f;
                }
            }
            else if (Projectile.ai[0] == 2) // 方案 b
            {

            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<BrainRot>(), 300);
        }

        public override void OnKill(int timeLeft)
        {

        }

    }
}