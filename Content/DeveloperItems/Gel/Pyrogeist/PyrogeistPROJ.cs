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
using CalamityRangerExpansion.Content.Gel.EAfterDog.AuricGel;

namespace CalamityRangerExpansion.Content.DeveloperItems.Gel.Pyrogeist
{
    internal class PyrogeistPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.Pyrogeist";
        private float rotationAngle = 0f; // 用于粒子旋转的角度
        private const float rotationSpeed = 0.05f; // 粒子旋转速度
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;
        }

        public override void AI()
        {
            // 保持弹幕旋转（对于倾斜走向的弹幕而言）
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;

            // Lighting - 添加深橙色光源，光照强度为 0.55
            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3() * 0.55f);

            // 生成淡蓝色粒子环绕效果
            rotationAngle += rotationSpeed;
            if (rotationAngle > MathHelper.TwoPi)
            {
                rotationAngle -= MathHelper.TwoPi;
            }


            // 这个特效不可关闭，因为子弹的贴图是透明的，关闭了之后子弹就看不到了
            {
                // 粒子位置计算，形成等边三角形环绕
                float[] angles = { 0f, MathHelper.TwoPi / 3f, MathHelper.TwoPi * 2f / 3f };
                float radius = 1 * 16f; // 半径为1格
                foreach (float initialAngle in angles)
                {
                    float angle = initialAngle + rotationAngle;
                    Vector2 position = Projectile.Center + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius;
                    int dust = Dust.NewDust(position, 0, 0, DustID.RedTorch, 0f, 0f, 100, default, Main.rand.NextFloat(1.5f, 2.5f));
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity = Vector2.Zero;
                }
            }

            // 追踪逻辑
            if (Main.getGoodWorld || Projectile.ai[1] > 15)
            {
                // 根据模式决定追踪范围和速度
                float trackingRange = Main.getGoodWorld ? 3800f : 1800f;
                float trackingSpeed = Main.getGoodWorld ? 15f : 12f;

                NPC target = Projectile.Center.ClosestNPCAt(trackingRange);
                if (target != null)
                {
                    Vector2 direction = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * trackingSpeed, 0.08f);
                }
            }
            else
            {
                Projectile.ai[1]++;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }

        public override void OnKill(int timeLeft)
        {
            // 检查场上是否已经存在 2 个或更多 InfernoFriendlyBlast 弹幕
            int existingBlasts = 0;
            foreach (Projectile proj1 in Main.projectile)
            {
                if (proj1.active && proj1.type == ProjectileID.InfernoFriendlyBlast)
                {
                    existingBlasts++;
                    if (existingBlasts >= 2)
                        return; // 如果已经存在 2 个或更多，则不释放新的弹幕
                }
            }

            // 在原地释放 296 号原版弹幕 InfernoFriendlyBlast
            int explosionProjectile = Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                Projectile.Center,
                Vector2.Zero,
                ProjectileID.InfernoFriendlyBlast,
                Projectile.damage, // 使用当前弹幕的伤害值
                0f,
                Projectile.owner
            );

            // 设置 InfernoFriendlyBlast 的属性
            Projectile proj = Main.projectile[explosionProjectile];
            proj.friendly = true;
            proj.hostile = false;
            proj.penetrate = -1;
            proj.localNPCHitCooldown = 60;
            proj.usesLocalNPCImmunity = true;
            proj.DamageType = DamageClass.Ranged; // 设置为远程伤害类型
        }
    }
}