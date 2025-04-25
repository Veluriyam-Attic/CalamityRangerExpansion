using CalamityRangerExpansion.CREConfigs;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using CalamityMod;
using Terraria.DataStructures;

namespace CalamityRangerExpansion.Content.Ammunition.EAfterDog.AuricBulet
{
    public class AuricBuletBALL : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectile.EAfterDog";
        private const int NoDamageTime = 2;  // 0.15秒不造成伤害（60帧/秒）

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;  // 穿透次数
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;  // 存活时间
            Projectile.alpha = 80;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
        }
        private float X = 3 * 16f; // 公转半径，初始值为x

        public override void AI()
        {
            // 初始化旋转方向和中心点
            if (Projectile.localAI[0] == 0)
            {
                Projectile.localAI[0] = 1;  // 标记为已初始化
                Projectile.ai[0] = Projectile.Center.X;  // 保存初始X坐标
                Projectile.ai[1] = Projectile.Center.Y;  // 保存初始Y坐标
                Projectile.ai[2] = Main.rand.NextFloat(0f, MathHelper.TwoPi);  // 随机生成初始角度偏移

                // 随机选择旋转方向，1为顺时针，-1为逆时针
                Projectile.localAI[2] = Main.rand.NextBool() ? 1f : -1f;
            }

            // **让公转圆心逐渐上升**
            Projectile.ai[1] -= 1.0f; // 每帧让圆心的Y坐标减少（向上移动）

            // 获取公转中心点
            Vector2 centerPoint = new Vector2(Projectile.ai[0], Projectile.ai[1]);

            // 根据旋转方向更新角度
            Projectile.localAI[1] += Projectile.localAI[2] * (1f / 60f); // 控制旋转速度
            float angle = Projectile.localAI[1] * MathHelper.TwoPi + Projectile.ai[2];
            Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * X;

            // 更新弹幕位置，使其围绕中心点旋转
            Projectile.Center = centerPoint + offset;

            // 在飞行过程中逐渐变透明和加速
            Projectile.alpha += 5;

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 产生金色粒子效果
                int dustType = Main.rand.NextBool(3) ? 244 : 246;
                float scale = 0.8f + Main.rand.NextFloat(0.6f);
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity = Projectile.velocity / 3f;
                Main.dust[idx].scale = scale;
            }
            Time++;
        }
        public ref float Time => ref Projectile.ai[1]; 
             
        public override bool? CanDamage() => Time >= 30f; // 初始的时候不会造成伤害，直到x为止
        private bool ProjectileWithinScreen()
        {
            Vector2 screenPosition = Main.screenPosition;
            return Projectile.position.X > screenPosition.X && Projectile.position.X < screenPosition.X + Main.screenWidth
                   && Projectile.position.Y > screenPosition.Y && Projectile.position.Y < screenPosition.Y + Main.screenHeight;
        }
        public override void OnSpawn(IEntitySource source)
        {

        }
        public override bool PreDraw(ref Color lightColor)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                Main.spriteBatch.SetBlendState(BlendState.Additive);

                Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
                Vector2 drawPosition = Projectile.Center - Main.screenPosition;
                Vector2 origin = texture.Size() * 0.5f;
                Color color = new Color(255, 215, 0); // 金黄色
                Main.EntitySpriteDraw(texture, drawPosition, null, color, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
                Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
                return false;
            }
            return true;
        }
    }
}