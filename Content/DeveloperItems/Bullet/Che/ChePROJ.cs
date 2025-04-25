using CalamityMod.Particles;
using CalamityMod.Projectiles.Typeless;
using CalamityMod;
using CalamityRangerExpansion.CREConfigs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace CalamityRangerExpansion.Content.DeveloperItems.Bullet.Che
{
    internal class ChePROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.ChineseChess.Che";
        public override string Texture => "CalamityRangerExpansion/Content/DeveloperItems/Bullet/Che/Che";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 画残影效果
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
                return false;
            }
            return true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 44;
            Projectile.height = 44;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
            Projectile.MaxUpdates = 4;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            // 子弹旋转逻辑
            Projectile.rotation = Projectile.velocity.ToRotation();

            // 子弹在短时间后变得可见
            if (Projectile.timeLeft == 119)
                Projectile.alpha = 0;


            // 添加飞行粒子效果
            //if (Main.rand.NextBool(2)) // 修改为 1/2 概率
            //{
            //    int[] dustTypes = { DustID.GemTopaz, DustID.Flare, DustID.OrangeTorch };
            //    int selectedDust = Main.rand.Next(dustTypes); // 随机选择粒子类型
            //    Dust dust = Dust.NewDustPerfect(
            //        Projectile.Center,
            //        selectedDust, // 随机粒子
            //        -Projectile.velocity.RotatedByRandom(0.1f) * Main.rand.NextFloat(0.01f, 0.3f)
            //    );
            //    dust.noGravity = true;
            //    dust.scale = Main.rand.NextFloat(0.6f, 1.0f); // 保持原粒子大小
            //}

            // 飞行过程中留下粒子特效
            if (Main.rand.NextBool(2)) // 1/2 概率
            {
                Vector2 trailPos = Projectile.Center; // 粒子生成位置
                float trailScale = Main.rand.NextFloat(0.6f, 1.0f); // 随机缩放
                Color trailColor = Color.OrangeRed; // 橙红色粒子颜色

                // 创建并生成粒子
                Particle trail = new SparkParticle(trailPos, Projectile.velocity * 0.2f, false, 60, trailScale, trailColor);
                GeneralParticleHandler.SpawnParticle(trail);
            }

        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 设置烟雾生成范围
            Vector2 center = Projectile.Center;
            float spreadAngle = MathHelper.ToRadians(30); // 左右各 30 度的范围
            int smokeCount = 6; // 烟雾数量

            for (int i = 0; i < smokeCount; i++)
            {
                // 随机角度
                float randomAngle = Main.rand.NextFloat(-spreadAngle, spreadAngle);
                Vector2 smokeVelocity = Projectile.velocity.RotatedBy(randomAngle) * Main.rand.NextFloat(0.5f, 1.5f);

                // 创建烟雾特效
                int smokeType = 244; // 烟雾类型
                Dust dust = Dust.NewDustPerfect(center, smokeType, smokeVelocity, 100, default, Main.rand.NextFloat(2f, 3f));
                dust.noGravity = true;
                dust.velocity *= 1.5f;
            }
            SoundEngine.PlaySound(SoundID.DD2_BallistaTowerShot, Projectile.Center);

        }

        public override void OnKill(int timeLeft)
        {

        }
    }
}
