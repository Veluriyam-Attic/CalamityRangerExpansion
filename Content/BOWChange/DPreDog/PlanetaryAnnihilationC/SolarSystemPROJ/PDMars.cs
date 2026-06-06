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
using Terraria.Audio;
using CalamityMod.Particles;

namespace CalamityRangerExpansion.Content.BOWChange.DPreDog.PlanetaryAnnihilationC.SolarSystemPROJ
{
    internal class PDMars : ModProjectile, ILocalizedModType
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


            // 每隔 x 帧生成一次
            if (Projectile.localAI[0] % 25 == 0)
            {
                 // 岩浆粒子特效
                for (int i = 0; i < 1; i++) // 每帧生成x个粒子
                {
                    Dust magmaDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Lava);
                    magmaDust.velocity *= 0.9f;
                    magmaDust.scale = Main.rand.NextFloat(1.2f, 1.5f);
                    magmaDust.noGravity = true;
                }

                // 火炬粒子特效
                for (int i = 0; i < 1; i++) // 每帧生成x个粒子
                {
                    Dust torchDust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch);
                    torchDust.velocity *= 0.99f;
                    torchDust.scale = Main.rand.NextFloat(0.8f, 1.2f);
                    torchDust.noGravity = true;
                }
            }
             
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            // 生成随机角度的弹幕
            int projectileCount = Main.rand.Next(1, 4); // 1~3 发弹幕
            for (int i = 0; i < projectileCount; i++)
            {
                // 随机生成角度
                float angle = Main.rand.NextFloat(0, MathHelper.TwoPi);
                Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

                // 随机伤害倍率
                float damageMultiplier = Main.rand.NextFloat(0.25f, 0.5f);
                int damage = (int)(Projectile.damage * damageMultiplier);

                // 发射 PDMarsSPIT 弹幕
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    direction * 10f, // 固定速度
                    ModContent.ProjectileType<PDMarsSPIT>(),
                    damage,
                    Projectile.knockBack,
                    Projectile.owner
                );

                // 生成红色线性粒子特效
                Color redColor = Color.Red;
                Particle redParticle = new SparkParticle(
                    Projectile.Center,
                    direction * 6f, // 线性粒子速度
                    false,
                    60, // 粒子寿命
                    Main.rand.NextFloat(0.8f, 1.2f), // 缩放
                    redColor
                );
                GeneralParticleHandler.SpawnParticle(redParticle);
            }
        }







    }
}