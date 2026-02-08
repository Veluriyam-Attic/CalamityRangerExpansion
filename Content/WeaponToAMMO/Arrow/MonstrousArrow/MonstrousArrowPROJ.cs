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
using Terraria.DataStructures;
using CalamityRangerExpansion.CREConfigs;
using CalamityMod.Balancing;
using CalamityMod.Projectiles;

namespace CalamityRangerExpansion.Content.WeaponToAMMO.Arrow.MonstrousArrow
{
    internal class MonstrousArrowPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "WeaponToAMMO.Arrow.MonstrousArrow";
        public override string Texture => "CalamityRangerExpansion/Content/WeaponToAMMO/Arrow/MonstrousArrow/MonstrousArrow";

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
            Projectile.penetrate = 1; // 根据模式设置穿透次数
            Projectile.timeLeft = 200;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;
        }
        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
        }
        public override void AI()
        {
            // 保持弹幕旋转
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;

            // Lighting - 添加深橙色光源，光照强度为 0.55
            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3() * 0.55f);


            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 添加粒子特效
                for (int i = 0; i < 2; i++) // 每帧生成 2 个粒子
                {
                    Dust dust = Dust.NewDustPerfect(
                        Projectile.Center,
                        Main.rand.NextBool() ? DustID.Blood : 105, // 随机选择两种特效
                        new Vector2(Main.rand.NextFloat(-1.5f, 1.5f), Main.rand.NextFloat(-1.5f, 1.5f)), // 初始速度
                        150, // 粒子透明度
                        default,
                        Main.rand.NextFloat(1.5f, 1.75f) // 粒子缩放大小
                    );
                    dust.noGravity = true; // 无重力效果
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 吸血逻辑
            int heal = 2;

            //if (Main.player[Main.myPlayer].lifeSteal <= 0f || heal <= 0 || target.lifeMax <= 5)
            //    return;

            // 移动到了player的扩展
            Main.player[Projectile.owner].SpawnLifeStealProjectile(target, Projectile, ProjectileID.VampireHeal, heal, 1, false, 3000f);
            //CalamityGlobalProjectile.SpawnLifeStealProjectile(Projectile, Main.player[Projectile.owner], heal, ProjectileID.VampireHeal, 3000f);
        }

        public override void OnKill(int timeLeft)
        {
            // 生成红色粒子效果，以三角形速度扩散
            for (int i = 0; i < 55; i++) // 生成 x 个粒子
            {
                float angle = MathHelper.TwoPi / 20 * i; // 计算粒子角度
                Vector2 velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * Main.rand.NextFloat(2f, 14f); // 三角形扩散速度
                Dust dust = Dust.NewDustPerfect(
                    Projectile.Center,
                    DustID.Blood, // 红色粒子特效
                    velocity
                );
                dust.noGravity = true; // 粒子无重力
                dust.scale = Main.rand.NextFloat(1f, 2.1f); // 粒子缩放大小
            }
        }


    }
}