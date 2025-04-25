using CalamityMod;
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
using CalamityRangerExpansion.CREConfigs;
using CalamityRangerExpansion.LightingBolts;

namespace CalamityRangerExpansion.Content.WeaponToAMMO.Bullet.OrichalcumBullet
{
    internal class OrichalcumBulletPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "WeaponToAMMO.Bullet.OrichalcumBullet";
        //public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

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
            Projectile.timeLeft = 300;
            Projectile.light = 0.5f;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.extraUpdates = 2;
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
            Projectile.rotation = Projectile.velocity.ToRotation();

            // Lighting - 添加深橙色光源，光照强度为 0.55
            Lighting.AddLight(Projectile.Center, Color.Pink.ToVector3() * 0.55f);

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // Add flying particles
                if (Main.rand.NextBool(1)) // 随机 1/3 概率
                {
                    Dust dust = Dust.NewDustPerfect(
                        Projectile.Center,
                        Main.rand.NextBool() ? 145 : 69, // 粒子特效 ID 145 和 69 交替
                        -Projectile.velocity.RotatedByRandom(0.1f) * Main.rand.NextFloat(0.01f, 0.3f)
                    );
                    dust.noGravity = true; // 粒子无重力
                    dust.scale = Main.rand.NextFloat(0.5f, 0.9f); // 随机大小
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 触发额外的花瓣弹幕，每次击中生成 2 个
            int petalCount = 2;
            for (int i = 0; i < petalCount; i++)
            {
                Player player = Main.player[Projectile.owner];

                // 确定弹幕生成位置（屏幕左侧或右侧的随机高度）
                float spawnX = player.direction > 0 ? Main.screenPosition.X : Main.screenPosition.X + Main.screenWidth;
                float spawnY = Main.screenPosition.Y + Main.rand.Next(Main.screenHeight);
                Vector2 spawnPosition = new Vector2(spawnX, spawnY);

                // 计算花瓣弹幕的目标方向（目标敌人的中心）
                Vector2 trajectory = target.Center - spawnPosition;

                // 添加随机偏移量，让轨迹更自然
                trajectory.X += Main.rand.NextFloat(-5f, 5f);
                trajectory.Y += Main.rand.NextFloat(-5f, 5f);

                // 归一化速度，使弹幕总是以 24 像素/帧的速度前进
                float speedMultiplier = 24f / trajectory.Length();
                trajectory *= speedMultiplier;

                // 生成花瓣弹幕，伤害值是当前弹幕伤害的 75%
                int petalProjectileID = Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    spawnPosition, trajectory,
                    ProjectileID.FlowerPetal,
                    (int)(Projectile.damage * 0.75), 0f, Projectile.owner
                );

                // 确保新生成的弹幕计算为远程伤害
                if (petalProjectileID.WithinBounds(Main.maxProjectiles))
                    Main.projectile[petalProjectileID].DamageType = DamageClass.Ranged;
            }



            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                LightingBoltsSystem.Spawn_BlossomPathIndicator(Projectile.Center, Main.player[Projectile.owner]);

                // 粒子特效：正方形弹开
                for (int i = 0; i < 4; i++)
                {
                    Vector2 offset = i switch
                    {
                        0 => new Vector2(-1, -1), // 左上
                        1 => new Vector2(1, -1),  // 右上
                        2 => new Vector2(1, 1),   // 右下
                        _ => new Vector2(-1, 1),  // 左下
                    };
                    Dust dust = Dust.NewDustPerfect(
                        target.Center + offset * 20f, // 粒子起点偏移
                        i % 2 == 0 ? 145 : 69,       // 使用交替的粒子 ID
                        offset * Main.rand.NextFloat(1f, 2f) // 偏移方向和速度
                    );
                    dust.noGravity = true; // 粒子无重力
                    dust.scale = Main.rand.NextFloat(1f, 1.5f); // 粒子缩放大小
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
        }


    }
}