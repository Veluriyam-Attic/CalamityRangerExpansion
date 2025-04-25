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
using CalamityMod.Particles;
using CalamityRangerExpansion.CREConfigs;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Accessories;
using CalamityMod.Buffs.StatDebuffs;
using CalamityRangerExpansion.LightingBolts;

namespace CalamityRangerExpansion.Content.Ammunition.CPreMoodLord.PerennialBullet
{
    public class PerennialBulletPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectile.CPreMoodLord";
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
            Projectile.MaxUpdates = 5;
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

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 添加飞行期间的粒子特效
                if (Main.rand.NextBool(3)) // 随机1/3概率生成
                {
                    Dust dust = Dust.NewDustPerfect(
                        Projectile.Center,
                        Main.rand.NextBool() ? 107 : 74, // 随机选择两种粒子类型
                        -Projectile.velocity.RotatedByRandom(0.1f) * Main.rand.NextFloat(0.01f, 0.3f) // 轻微随机速度
                    );
                    dust.noGravity = true; // 粒子无重力
                    dust.scale = Main.rand.NextFloat(0.5f, 0.9f); // 随机大小
                    if (dust.type == 107)
                        dust.scale = Main.rand.NextFloat(0.35f, 0.55f); // 特殊类型的粒子缩小
                }
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

            // 给予玩家可以堆叠的 PerennialBulletPBuff
            //var player = Main.player[Projectile.owner].GetModPlayer<PerennialBulletPlayer>();
            //player.IncreaseStackCount(); // 每次击中敌人时增加堆叠

            // 永远施加 Buff，维持效果
            Player player = Main.player[Projectile.owner];
            player.AddBuff(ModContent.BuffType<PerennialBulletPBuff>(), 300); // 每次刷新 Buff 持续时间，确保不停覆盖
            player.GetModPlayer<PerennialBulletPlayer>().IncreaseStackCount(); // 增加堆叠

            target.AddBuff(ModContent.BuffType<AbsorberAffliction> (), 300); // 西晋之痛【阴阳吸金石专属】
            target.AddBuff(BuffID.Poisoned, 300);
            target.AddBuff(BuffID.Venom, 300);
            target.AddBuff(ModContent.BuffType<SagePoison>(), 300); // 鼠尾草中毒【剧毒芽孢专属】

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 在正45度和负45度之间随机选择一个角度
                float randomAngle = Main.rand.NextFloat(-45f, 45f); // 随机角度
                float angleOffset = MathHelper.ToRadians(randomAngle); // 转为弧度
                Vector2 direction = Projectile.velocity.RotatedBy(angleOffset).SafeNormalize(Vector2.UnitY); // 根据随机角度旋转方向
                Vector2 velocity = direction * Main.rand.NextFloat(2f, 4f); // 设置粒子速度（随机）

                // 创建粒子特效
                Particle trail = new SparkParticle(
                    Projectile.Center,               // 粒子生成位置
                    velocity,                        // 粒子速度
                    false,                           // 不受重力
                    60,                              // 粒子生命周期
                    Main.rand.NextFloat(0.8f, 1.2f), // 粒子缩放
                    Color.Green                      // 粒子颜色
                );
                GeneralParticleHandler.SpawnParticle(trail); // 生成粒子
            }
        }


        public override void OnKill(int timeLeft)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 生成绿色烟雾特效，类似CinderArrowProj的效果
                int Dusts = 3;
                float radians = MathHelper.TwoPi / Dusts;
                Vector2 spinningPoint = Vector2.Normalize(new Vector2(-1f, -1f));
                for (int i = 0; i < Dusts; i++)
                {
                    Vector2 dustVelocity = spinningPoint.RotatedBy(radians * i).RotatedBy(0.5f) * 6.5f;
                    Particle smoke = new HeavySmokeParticle(Projectile.Center, dustVelocity * Main.rand.NextFloat(1f, 2.6f), Color.Green, 18, Main.rand.NextFloat(0.9f, 1.6f), 0.35f, Main.rand.NextFloat(-1, 1), true);
                    GeneralParticleHandler.SpawnParticle(smoke);
                }
            }
        }
    }
}
