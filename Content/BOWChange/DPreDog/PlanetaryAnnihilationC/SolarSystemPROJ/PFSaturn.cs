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
using CalamityMod.Particles;

namespace CalamityRangerExpansion.Content.BOWChange.DPreDog.PlanetaryAnnihilationC.SolarSystemPROJ
{
    internal class PFSaturn : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.DPreDog";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
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
            Projectile.width = 11; // 弹幕宽度
            Projectile.height = 24; // 弹幕高度
            Projectile.friendly = true; // 对敌人有效
            Projectile.DamageType = DamageClass.Ranged; // 远程伤害类型
            Projectile.penetrate = 3; // 穿透力
            Projectile.timeLeft = 300; // 弹幕存在时间为x帧
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
            Projectile.ignoreWater = true; // 弹幕不受水影响
            Projectile.arrow = true;
            Projectile.extraUpdates = 1;
        }
        public override void OnSpawn(IEntitySource source)
        {
            //Projectile.velocity *= 0.4f;
        }
        public override void AI()
        {
            // 调整弹幕的旋转，使其在飞行时保持水平
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;

            // Lighting - 添加天蓝色光源，光照强度为 0.49
            Lighting.AddLight(Projectile.Center, Color.LightSkyBlue.ToVector3() * 0.49f);


            // 维持 EffulgentFeatherArrowAura 的存在
            if (Projectile.localAI[0] == 0f)
            {
                // 在第一次执行时生成 Aura 弹幕，并记录其 ID
                int auraProjectileID = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PFSaturnRing>(), (int)(Projectile.damage * 0.15f), 0, Projectile.owner, Projectile.whoAmI);
                Projectile.localAI[0] = auraProjectileID + 1; // 存储 Aura 的 ID，+1 以防止冲突
            }
            else
            {
                // 确保 Aura 与箭头保持同步
                int auraProjectileID = (int)(Projectile.localAI[0] - 1);
                if (Main.projectile.IndexInRange(auraProjectileID))
                {
                    Projectile auraProjectile = Main.projectile[auraProjectileID];
                    if (auraProjectile.active && auraProjectile.type == ModContent.ProjectileType<PFSaturnRing>() && auraProjectile.ai[0] == Projectile.whoAmI)
                    {
                        // 将 Aura 位置设置为与箭头匹配
                        auraProjectile.Center = Projectile.Center;
                        auraProjectile.damage = (int)((Projectile.damage) * 0.15);
                        auraProjectile.velocity = Vector2.Zero; // 确保 Aura 不移动
                    }
                    else
                    {
                        // 如果 Aura 不存在或失效，则重新生成
                        Projectile.localAI[0] = 0f;
                    }
                }
            }



            // 在弹幕后方释放尖字型粒子特效
            for (int i = 0; i < Main.rand.Next(1, 3); i++) // 每帧生成 1~2 个粒子
            {
                // 随机化角度和速度
                float angleOffset = Main.rand.NextFloat(-0.2f, 0.2f); // 随机角度偏移
                float speedMultiplier = Main.rand.NextFloat(0.4f, 0.7f); // 随机速度倍率

                Vector2 sparkVelocity = Projectile.velocity.RotatedBy(angleOffset) * speedMultiplier;

                // 设置尖字型粒子的颜色为土星代表色
                Color saturnColor = Main.rand.Next(3) switch
                {
                    0 => new Color(216, 178, 135), // 浅棕色，土星表面
                    1 => new Color(241, 226, 205), // 浅黄色，土星环带
                    2 => new Color(202, 163, 118)  // 土黄色，土星带状云
                };

                // 生成尖字型粒子
                PointParticle spark = new PointParticle(
                    Projectile.Center - Projectile.velocity + sparkVelocity,
                    sparkVelocity,
                    false,
                    15, // 粒子寿命
                    Main.rand.NextFloat(1.0f, 1.3f), // 粒子大小
                    saturnColor
                );

                // 释放粒子
                GeneralParticleHandler.SpawnParticle(spark);
            }


        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int particleCount = 12; // 固定生成 12 个粒子，形成环形阵列
            float angleIncrement = MathHelper.TwoPi / particleCount; // 每两个粒子之间的角度

            for (int i = 0; i < particleCount; i++)
            {
                float angle = angleIncrement * i; // 计算每个粒子的角度
                Vector2 velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 4f; // 固定速度向外扩散

                // 设置尖字型粒子的颜色为土星代表色
                Color saturnColor = new Color(216, 178, 135); // 使用浅棕色代表土星

                // 生成尖字型粒子
                PointParticle spark = new PointParticle(
                    Projectile.Center,
                    velocity,
                    false,
                    20, // 粒子寿命
                    1.2f, // 粒子大小
                    saturnColor
                );

                // 释放粒子
                GeneralParticleHandler.SpawnParticle(spark);
            }
        }

        public override void OnKill(int timeLeft)
        {

        }







    }
}