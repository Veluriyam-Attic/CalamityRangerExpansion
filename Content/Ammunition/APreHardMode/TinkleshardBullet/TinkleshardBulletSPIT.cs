using CalamityMod;
using CalamityRangerExpansion.CREConfigs;
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
using Terraria.GameContent;

namespace CalamityRangerExpansion.Content.Ammunition.APreHardMode.TinkleshardBullet
{
    internal class TinkleshardBulletSPIT : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectile.APreHardMode";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 1;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        #region 旧代码
        //    public override bool PreDraw(ref Color lightColor)
        //    {
        //        SpriteBatch spriteBatch = Main.spriteBatch;

        //        // 动态随机选择贴图
        //        string[] textures = new[]
        //        {
        //    "CalamityRangerExpansion/Content/Ammunition/APreHardMode/TinkleshardBullet/TinkleshardBullet_Piece1",
        //    "CalamityRangerExpansion/Content/Ammunition/APreHardMode/TinkleshardBullet/TinkleshardBullet_Piece2",
        //    "CalamityRangerExpansion/Content/Ammunition/APreHardMode/TinkleshardBullet/TinkleshardBullet_Piece3",
        //    "CalamityRangerExpansion/Content/Ammunition/APreHardMode/TinkleshardBullet/TinkleshardBullet_Piece4"
        //};
        //        string selectedTexture = textures[Main.rand.Next(textures.Length)];
        //        Texture2D texture = ModContent.Request<Texture2D>(selectedTexture).Value;

        //        // 计算绘制的原点和位置
        //        Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);
        //        Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

        //        // 绘制贴图
        //        spriteBatch.Draw(texture, drawPosition, null, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);

        //        return false; // 禁用默认绘制
        //    }
        #endregion

        private string selectedTexture; // 存储随机选择的贴图路径

        public override void OnSpawn(IEntitySource source)
        {
            // 在弹幕生成时随机选择一次贴图
            string[] textures = new[]
            {
                "CalamityRangerExpansion/Content/Ammunition/APreHardMode/TinkleshardBullet/TinkleshardBullet_Piece1",
                "CalamityRangerExpansion/Content/Ammunition/APreHardMode/TinkleshardBullet/TinkleshardBullet_Piece2",
                "CalamityRangerExpansion/Content/Ammunition/APreHardMode/TinkleshardBullet/TinkleshardBullet_Piece3",
                "CalamityRangerExpansion/Content/Ammunition/APreHardMode/TinkleshardBullet/TinkleshardBullet_Piece4"
            };

            selectedTexture = textures[Main.rand.Next(textures.Length)];

            Time = 0f; // 初始化计时器

        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            // 加载固定的贴图
            Texture2D texture = ModContent.Request<Texture2D>(selectedTexture).Value;

            // 计算绘制的原点和位置
            Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

            // 绘制贴图
            spriteBatch.Draw(texture, drawPosition, null, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);

            return false; // 禁用默认绘制
        }


        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 0;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;
            Projectile.aiStyle = ProjAIStyleID.Arrow; // 受重力影响
            Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi); // 随机初始旋转
        }

        #region 旧代码
        //private static readonly string[] Textures = new[]
        //{
        //    "CalamityRangerExpansion/Content/Ammunition/APreHardMode/TinkleshardBullet/TinkleshardBullet_Piece1",
        //    "CalamityRangerExpansion/Content/Ammunition/APreHardMode/TinkleshardBullet/TinkleshardBullet_Piece2",
        //    "CalamityRangerExpansion/Content/Ammunition/APreHardMode/TinkleshardBullet/TinkleshardBullet_Piece3",
        //    "CalamityRangerExpansion/Content/Ammunition/APreHardMode/TinkleshardBullet/TinkleshardBullet_Piece4"
        //};
        //public override string Texture => Textures[Main.rand.Next(Textures.Length)];
        #endregion

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            Lighting.AddLight(Projectile.Center, Color.Lerp(Color.Blue, Color.AliceBlue, 0.5f).ToVector3() * 0.49f);

            // Projectile becomes visible after a few frames
            if (Projectile.timeLeft == 298)
                Projectile.alpha = 0;

            // 飞行过程中生成冰蓝色特效
            if (Main.rand.NextBool(3)) // 每帧有 1/3 概率生成粒子
            {
                Dust dust = Dust.NewDustPerfect(
                    Projectile.Center,
                    DustID.IceTorch, // 冰蓝色粒子类型
                    -Projectile.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.2f, 0.5f), // 添加随机偏移的速度
                    150,
                    Color.LightBlue, // 颜色为冰蓝色
                    Main.rand.NextFloat(0.3f, 0.6f) // 随机大小
                );
                dust.noGravity = true; // 粒子无重力
                dust.fadeIn = 0.5f; // 逐渐淡入
            }

            // 模拟旋转效果
            Projectile.rotation += MathHelper.ToRadians(2);

            // 生成水蓝色粒子特效
            if (Main.rand.NextBool(3))
            {
                Dust.NewDustPerfect(
                    Projectile.Center,
                    DustID.BlueTorch,
                    Vector2.Zero,
                    100,
                    Color.CornflowerBlue,
                    Main.rand.NextFloat(0.8f, 1.2f)
                ).noGravity = true;
            }
            Time++;
        }
        public ref float Time => ref Projectile.ai[1];

        public override bool? CanDamage() => Time >= 5f; // 初始时不造成伤害，直到帧数达到 X

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }

        public override void OnKill(int timeLeft)
        {

        }

    }
}
