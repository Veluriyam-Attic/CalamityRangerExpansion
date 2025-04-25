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

namespace CalamityRangerExpansion.Content.Ammunition.DPreDog.UelibloomBullet
{
    public class UelibloomBulletPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectile.DPreDog";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
                return false;
            }
            return true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.MaxUpdates = 6;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;
        }

        public override void AI()
        {
            // 由于我们是水平贴图，因此什么也不需要转动
            Projectile.rotation = Projectile.velocity.ToRotation();

            // 添加光效
            Lighting.AddLight(Projectile.Center, Color.Lerp(Color.Blue, Color.AliceBlue, 0.5f).ToVector3() * 0.49f);

            // 子弹在出现之后很短一段时间会变得可见
            if (Projectile.timeLeft == 296)
                Projectile.alpha = 0;

            // 添加飞行期间的橙色粒子特效
            if (Main.rand.NextBool(3)) // 随机1/3概率生成
            {
                Dust dust = Dust.NewDustPerfect(
                    Projectile.Center,
                    Main.rand.NextBool() ? 2 : 3, // 随机选择两种粒子类型
                    -Projectile.velocity.RotatedByRandom(0.1f) * Main.rand.NextFloat(0.01f, 0.3f) // 轻微随机速度
                );
                dust.noGravity = true; // 粒子无重力
                dust.scale = Main.rand.NextFloat(0.5f, 0.9f); // 随机大小
                if (dust.type == 2)
                    dust.scale = Main.rand.NextFloat(0.35f, 0.55f); // 特殊类型的粒子缩小
            }

        }

        public override void OnSpawn(IEntitySource source)
        {

        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {

        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            // 生成 3 个随机方向的 UelibloomBulletLEAF 弹幕
            for (int i = 0; i < 3; i++)
            {
                // 随机生成一个角度（0 到 360 度）
                float angle = MathHelper.ToRadians(Main.rand.Next(0, 360));

                // 设置固定初始速度（长度为 20f）
                Vector2 velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 1.4f;

                // 创建 UelibloomBulletLEAF 弹幕
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),  // 弹幕来源
                    Projectile.Center,               // 生成位置
                    velocity,                        // 固定初始速度
                    ModContent.ProjectileType<UelibloomBulletLEAF>(), // UelibloomBulletLEAF 弹幕类型
                    (int)(Projectile.damage * 0.07f), // 伤害倍率
                    Projectile.knockBack,            // 使用当前弹幕的击退力
                    Projectile.owner                 // 拥有者
                );
            }
        }


        public override void OnKill(int timeLeft)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                LightingBoltsSystem.Spawn_GreenShimmerPath(Projectile.Center, Main.player[Projectile.owner]);

                // 创建叶子形状的粒子特效
                int leafCount = 12; // 每个叶子的粒子数量
                float leafLength = 8 * 16f; // 叶子的长度
                float leafWidth = 4 * 16f; // 叶子的宽度（控制曲线范围）

                for (int i = -1; i <= 1; i += 2) // 两片叶子，正方向和反方向
                {
                    for (int j = 0; j < leafCount; j++) // 每片叶子的粒子
                    {
                        // 计算每个粒子的角度和位置
                        float progress = j / (float)(leafCount - 1); // 从 0 到 1 的进度
                        float angleOffset = (progress - 0.5f) * MathHelper.Pi * i; // 叶子弧度偏移
                        float x = progress * leafLength; // 沿叶子长度方向的分布
                        float y = (float)Math.Sin(angleOffset) * leafWidth * (0.5f - Math.Abs(progress - 0.5f)); // 曲线形状

                        Vector2 position = Projectile.Center + new Vector2(x, y);
                        Vector2 velocity = (position - Projectile.Center).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(2f, 4f);

                        Dust dust = Dust.NewDustPerfect(
                            position,
                            DustID.GreenTorch, // 绿色叶子特效
                            velocity,
                            100,
                            Color.LimeGreen, // 粒子颜色
                            Main.rand.NextFloat(1.2f, 1.8f) // 粒子大小
                        );
                        dust.noGravity = true; // 粒子无重力
                        dust.fadeIn = 0.1f; // 快速淡入效果
                    }
                }
            }
        }
    }
}
