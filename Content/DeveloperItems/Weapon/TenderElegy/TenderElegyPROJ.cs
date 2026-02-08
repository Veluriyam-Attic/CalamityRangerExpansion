using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Terraria.GameContent;
using CalamityMod.Graphics.Primitives;
using System;

namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.TenderElegy
{
    internal class TenderElegyPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.TenderElegy";

        private const int TrackingDelay = 35; // 追踪延迟帧数
        private bool isHoming = false; // 是否启用追踪

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 90;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            // 设置弹幕的基础属性
            Projectile.width = Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.arrow = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            // 旋转方向，使弹幕朝向飞行方向
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            // 添加光照
            Lighting.AddLight(Projectile.Center, Color.LightSkyBlue.ToVector3() * 0.49f);

            // 追踪逻辑
            if (Projectile.ai[0] >= TrackingDelay)
            {
                if (!isHoming)
                {
                    isHoming = true;
                }

                // 追踪最近的敌人
                CalamityUtils.HomeInOnNPC(Projectile, true, 8848f, 40f, 1f);
            }

            // 增加 AI 计时器
            Projectile.ai[0]++;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityRangerExpansion/Content/DeveloperItems/Weapon/TenderElegy/TenderElegyPROJ").Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            // **应用 HeavenlyGaleTrail Shader**
            GameShaders.Misc["CalamityMod:HeavenlyGaleTrail"].SetShaderTexture(
                ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/EternityStreak")
            );
            GameShaders.Misc["CalamityMod:HeavenlyGaleTrail"].UseImage2("Images/Extra_189");

            // **计算动态颜色**
            float localIdentityOffset = Projectile.identity * 0.1372f;
            Color mainColor = CalamityUtils.MulticolorLerp(
                (Main.GlobalTimeWrappedHourly * 2f + localIdentityOffset) % 1f,
                Color.Cyan, Color.Lime, Color.GreenYellow, Color.Goldenrod, Color.Orange
            );
            Color secondaryColor = CalamityUtils.MulticolorLerp(
                (Main.GlobalTimeWrappedHourly * 2f + localIdentityOffset + 0.2f) % 1f,
                Color.Cyan, Color.Lime, Color.GreenYellow, Color.Goldenrod, Color.Orange
            );

            GameShaders.Misc["CalamityMod:HeavenlyGaleTrail"].UseColor(mainColor);
            GameShaders.Misc["CalamityMod:HeavenlyGaleTrail"].UseSecondaryColor(secondaryColor);
            GameShaders.Misc["CalamityMod:HeavenlyGaleTrail"].Apply();

            // **提高拖尾采样点，保证平滑度**
            int numPoints = 73;
            PrimitiveRenderer.RenderTrail(
                Projectile.oldPos,
                new(
                    ImpFlameWidthFunction,
                    ImpFlameColorFunction,
                    (_, _) => Projectile.Size * 0.5f,
                    shader: GameShaders.Misc["CalamityMod:HeavenlyGaleTrail"]
                ),
                numPoints
            );

            // 绘制弹幕本体
            Main.EntitySpriteDraw(
                texture,
                drawPosition,
                null,
                lightColor,
                Projectile.rotation,
                texture.Size() / 2,
                Projectile.scale,
                SpriteEffects.None,
                0
            );

            return false; // 禁用原始绘制
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            var npcGlobal = target.GetGlobalNPC<TenderElegyBeilGNPC>();

            // 如果目标敌人没有铃铛，则生成铃铛弹幕
            if (!npcGlobal.hasBell)
            {
                Vector2 spawnPosition = target.Center + new Vector2(0, -30);
                int proj = Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    spawnPosition, Vector2.Zero,
                    ModContent.ProjectileType<TenderElegyBeil>(),
                    (int)(Projectile.damage * 0.5f),
                    Projectile.knockBack,
                    Projectile.owner,
                    target.whoAmI // 绑定目标敌人 ID
                );

                Main.projectile[proj].ai[0] = target.whoAmI;
                npcGlobal.hasBell = true;
            }

            // 生成类似 MagnomalyRocket 命中时的粒子特效
            int dustType = Main.rand.NextBool() ? 107 : 234;
            if (Main.rand.NextBool(4))
            {
                dustType = 269;
            }

            for (int i = 0; i < 20; i++) // 生成 X 个粒子
            {
                Vector2 spawnPos = target.Center + Main.rand.NextVector2Circular(32f, 32f); // 以目标中心为圆心，半径 32 像素内随机生成
                Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(1f, 3f); // 随机方向，随机速度

                int dust = Dust.NewDust(spawnPos, 0, 0, dustType, velocity.X, velocity.Y, 100, default, Main.rand.NextFloat(1.7f, 2.2f));
                Main.dust[dust].noGravity = true;
                Main.dust[dust].noLight = true;
            }
        }

        private float ImpFlameWidthFunction(float completionRatio, Vector2 vexpos)
        {
            float baseWidth = 30f;
            float taper = MathHelper.Lerp(1f, 0.2f, completionRatio);
            return baseWidth * taper;
        }

        private Color ImpFlameColorFunction(float completionRatio, Vector2 vexpos)
        {
            float localIdentityOffset = Main.rand.NextFloat(0.1f, 0.2f);

            // 颜色渐变
            Color baseColor = CalamityUtils.MulticolorLerp(
                (Main.GlobalTimeWrappedHourly * 2f + localIdentityOffset) % 1f,
                Color.Cyan, Color.Lime, Color.GreenYellow, Color.Goldenrod, Color.Orange
            );

            return Color.Lerp(baseColor, Color.Transparent, completionRatio);
        }
    }
}
