using CalamityMod;
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

namespace CalamityRangerExpansion.Content.BOWChange.EAfterDog.PhangasmC
{
    internal class PhangasmReUFO : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.EAfterDog";
        public override string Texture => "CalamityMod/Projectiles/Melee/StreamGougePortal";

        public override void SetStaticDefaults()
        {
            // 设置弹幕拖尾长度和模式
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 origin = texture.Size() * 0.5f;
            SpriteEffects flip = SpriteEffects.None;

            float time = Main.GlobalTimeWrappedHourly;
            float scale = 1.0f * Projectile.Opacity;

            // 绘制三层 portal，不同颜色 + 旋转速度
            DrawPortalLayer(texture, drawPos, origin, scale * 1.2f, time * 1.4f, Color.DarkGreen * 0.6f);
            DrawPortalLayer(texture, drawPos, origin, scale * 1.1f, -time * 1.1f, Color.LightGreen * 0.5f);
            DrawPortalLayer(texture, drawPos, origin, scale * 1.0f, time * 0.6f, Color.LightBlue * 0.6f);

            // ?叠加新版发光贴图层=====================================

            // ?? cellnoise_005：稳定叠加两层（减速+抖动减弱）
            Texture2D cellnoise = ModContent.Request<Texture2D>("CalamityRangerExpansion/texture/SuperTexturePack/cellnoise_005").Value;
            Vector2 originCell = cellnoise.Size() * 0.5f;
            for (int i = 0; i < 2; i++)
            {
                float baseSpin = (i == 0 ? 0.5f : -0.35f); // 原本是1.3和-0.9，明显降低
                float rot = time * baseSpin + Main.rand.NextFloat(-0.05f, 0.05f); // 抖动幅度大幅减小
                Color col = Color.Lerp(Color.Cyan, Color.LightGreen, Main.rand.NextFloat(0.4f, 0.8f)) * 0.6f;
                Main.EntitySpriteDraw(cellnoise, drawPos, null, col, rot, originCell, scale * 0.55f, SpriteEffects.None, 0);
            }

            // ?? fx_Sparks1 和 fx_Sparks2：大号缓旋贴图（放大2.5倍 + 平滑旋转）
            string[] sparkPaths = {
    "CalamityRangerExpansion/texture/SuperTexturePack/fx_Sparks1",
    "CalamityRangerExpansion/texture/SuperTexturePack/fx_Sparks2"
};
            for (int i = 0; i < 2; i++)
            {
                string path = sparkPaths[Main.rand.Next(sparkPaths.Length)];
                Texture2D spark = ModContent.Request<Texture2D>(path).Value;
                Vector2 originSpark = spark.Size() * 0.5f;

                float baseSpeed = (i == 0 ? 0.6f : -0.45f); // 旋转方向+速度
                float rot = time * baseSpeed; // 无抖动、平滑旋转

                Color col = Color.White * 0.14f;
                float enlargedScale = scale * 1.05f;

                Main.EntitySpriteDraw(spark, drawPos, null, col, rot, originSpark, enlargedScale, SpriteEffects.None, 0);
            }


            return false;
        }

        private void DrawPortalLayer(Texture2D tex, Vector2 pos, Vector2 origin, float scale, float rot, Color color)
        {
            Main.EntitySpriteDraw(tex, pos, null, color with { A = 0 }, rot, origin, scale, SpriteEffects.None, 0);
        }

        private void DrawTwirl(Texture2D tex, Vector2 pos, Vector2 origin, float scale, float rot, Color color)
        {
            Main.EntitySpriteDraw(tex, pos, null, color with { A = 0 }, rot, origin, scale, SpriteEffects.None, 0);
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 3; // 可击中次数
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 0; // 可调节飞行平滑度
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
        }

        public override void OnSpawn(IEntitySource source)
        {
            // 弹幕生成时执行，用于初始化粒子或播放生成音效
        }

        public override void AI()
        {
            Projectile.timeLeft = 300;

            Player player = Main.player[Projectile.owner];
            if (!player.active || player.dead || player.HeldItem.type != ModContent.ItemType<PhangasmRe>())
            {
                Projectile.Kill();
                return;
            }

            // 强制跟随到头顶 35格处
            Vector2 targetPos = player.Center - new Vector2(0f, 15f * 16f);
            Projectile.Center = Vector2.Lerp(Projectile.Center, targetPos, 0.25f); // 平滑贴近

            // 可视角度旋转（保留）
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 弹幕命中 NPC 时执行，可用于生成击中特效、播放音效、回复血量等
        }

        public override void OnKill(int timeLeft)
        {
            // 弹幕死亡（时间到或碰撞）时执行，可用于生成碎裂粒子、播放破碎音效
        }


    }
}
