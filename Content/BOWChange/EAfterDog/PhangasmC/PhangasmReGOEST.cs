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

namespace CalamityRangerExpansion.Content.BOWChange.EAfterDog.PhangasmC
{
    internal class PhangasmReGOEST : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.EAfterDog";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 1;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // 画残影效果
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
        public override void SetDefaults()
        {
            // 设置弹幕的基础属性
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true; // 对敌人有效
            Projectile.DamageType = DamageClass.Ranged; // 远程伤害类型
            Projectile.penetrate = 1; // 穿透力为1，击中一个敌人就消失
            Projectile.timeLeft = 1200; // 弹幕存在时间为1200帧
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
            Projectile.ignoreWater = true; // 弹幕不受水影响
            Projectile.arrow = true;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
            Projectile.alpha = 0;
        }

        public override void AI()
        {
            // 调整弹幕的旋转，使其在飞行时保持水平
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            // 添加光源，光照颜色为天蓝色，强度为 0.49
            Lighting.AddLight(Projectile.Center, Color.LightSkyBlue.ToVector3() * 0.49f);

            // 检查玩家是否仍然手持 PhangasmRe
            Player player = Main.player[Projectile.owner];
            if (!(player.HeldItem.ModItem is ModItem modItem && modItem.Name == "PhangasmRe"))
            {
                Projectile.Kill(); // 如果玩家不再手持 PhangasmRe，则删除弹幕
                return;
            }

            NPC target = Projectile.Center.ClosestNPCAt(5800); // 查找范围内最近的敌人
            if (target != null)
            {
                Vector2 direction = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * 12f, 0.08f); // 追踪速度为12f
            }

        }

        public override void OnKill(int timeLeft)
        {
            // 弹幕死亡特效
            int numParticles = Main.rand.Next(5, 10);
            for (int i = 0; i < numParticles; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.Center, 0, 0, 229, 0f, 0f, 100);
                Dust dust = Main.dust[dustIndex];
                dust.velocity *= 1.6f;
                dust.velocity.Y -= 1f;
                dust.position -= Vector2.One * 4f;
                dust.position = Vector2.Lerp(dust.position, Projectile.Center, 0.5f);
                dust.noGravity = true;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            // 返回染色，颜色为透明度动态调整的白色
            return new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 0);
        }
    }
}