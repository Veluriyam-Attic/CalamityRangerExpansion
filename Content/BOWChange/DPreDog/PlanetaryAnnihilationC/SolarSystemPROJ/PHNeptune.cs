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
using CalamityMod.Projectiles.Melee;

namespace CalamityRangerExpansion.Content.BOWChange.DPreDog.PlanetaryAnnihilationC.SolarSystemPROJ
{
    internal class PHNeptune : ModProjectile, ILocalizedModType
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
            Projectile.penetrate = 1; // 穿透力为1，击中一个敌人就消失
            Projectile.timeLeft = 300; // 弹幕存在时间为x帧
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
            Projectile.ignoreWater = true; // 弹幕不受水影响
            Projectile.arrow = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            // 调整弹幕的旋转，使其在飞行时保持水平
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;

            // Lighting - 添加天蓝色光源，光照强度为 0.49
            Lighting.AddLight(Projectile.Center, Color.LightSkyBlue.ToVector3() * 0.49f);


            // 飞行时的烟雾
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] > 6f)
            {
                for (int d = 0; d < 5; d++)
                {
                    Dust dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Shadowflame, Projectile.velocity.X, Projectile.velocity.Y, 100, default, 1f)];
                    dust.velocity = Vector2.Zero;
                    dust.position -= Projectile.velocity / 5f * d;
                    dust.noGravity = true;
                    dust.scale = 0.65f;
                    dust.noLight = true;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }
        public override void OnKill(int timeLeft)
        {
            // 创建一个规则且带有规律色彩的粒子爆发效果
            int killDust = 36; // 固定生成36个粒子，形成规则圆环
            for (int i = 0; i < killDust; ++i)
            {
                float angle = MathHelper.TwoPi / killDust * i; // 计算每个粒子的位置
                Vector2 dustOffset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 40f; // 圆环半径40
                Vector2 spawnPosition = Projectile.Center + dustOffset;

                int dustID = Main.rand.NextBool() ? 198 : 199;
                int idx = Dust.NewDust(spawnPosition, 0, 0, dustID, 0f, 0f);

                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity = dustOffset * 0.05f; // 粒子缓慢向外扩散
                Main.dust[idx].scale = Main.rand.NextFloat(1.0f, 1.5f);
            }

            // 生成螺旋效果的三个触手弹幕
            int tentacleCount = 3;
            float initialAngle = Main.rand.NextFloat(0, MathHelper.TwoPi); // 随机初始角度
            for (int i = 0; i < tentacleCount; ++i)
            {
                float angle = initialAngle + MathHelper.TwoPi / tentacleCount * i; // 计算每个触手的角度
                Vector2 projVel = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 4f;
                SpawnTentacle(projVel);
            }
        }

        private void SpawnTentacle(Vector2 tentacleVelocity)
        {
            int damage = Projectile.damage;
            float kb = Projectile.knockBack;

            // 随机化触手行为变量
            float ai0 = Main.rand.NextFloat(0.01f, 0.08f) * (Main.rand.NextBool() ? -1f : 1f);
            float ai1 = Main.rand.NextFloat(0.01f, 0.08f) * (Main.rand.NextBool() ? -1f : 1f);

            if (Projectile.owner == Main.myPlayer)
            {
                // 生成触手弹幕
                int tentacleProjectile = Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    tentacleVelocity,
                    ModContent.ProjectileType<VoidTentacle>(),
                    damage,
                    kb,
                    Projectile.owner,
                    ai0,
                    ai1
                );

                // 设置触手属性
                Projectile proj = Main.projectile[tentacleProjectile];
                proj.friendly = true;
                proj.hostile = false;
                proj.penetrate = -1;
                proj.localNPCHitCooldown = 1;
                proj.usesLocalNPCImmunity = true;

                // 设置触手为射手类型伤害
                proj.DamageType = DamageClass.Ranged;
            }
        }






    }
}