using CalamityRangerExpansion.Content.Gel.CPreMoodLord.ScoriaGel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;

namespace CalamityRangerExpansion.Content.Gel.CPreMoodLord.PlagueGel
{
    internal class PlagueGelGP : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool IsPlagueGelInfused = false;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_ItemUse_WithAmmo ammoSource && ammoSource.AmmoItemIdUsed == ModContent.ItemType<PlagueGel>())
            {
                IsPlagueGelInfused = true;
                projectile.netUpdate = true;
                projectile.damage = (int)(projectile.damage * 0.95f); // 减少 5% 伤害
                #region 旧代码
                //// 在弹幕生成时生成前方、左侧、右侧的绿色烟雾
                //for (int i = -1; i <= 1; i++) // -1：左，0：正前方，1：右
                //{
                //    float angle = MathHelper.ToRadians(15 * i); // 偏移角度：-15°, 0°, +15°
                //    Vector2 dustVelocity = projectile.velocity.RotatedBy(angle).SafeNormalize(Vector2.Zero);
                //    Particle smoke = new HeavySmokeParticle(
                //        projectile.Center,
                //        dustVelocity * Main.rand.NextFloat(1f, 2.6f),
                //        Color.Green,
                //        18,
                //        Main.rand.NextFloat(0.9f, 1.6f),
                //        0.35f,
                //        Main.rand.NextFloat(-1, 1),
                //        true
                //    );
                //    GeneralParticleHandler.SpawnParticle(smoke);
                //}
                #endregion
            }
            base.OnSpawn(projectile, source);
        }

        public override void AI(Projectile projectile)
        {
            if (IsPlagueGelInfused)
            {
                // 随机生成瘟疫圆圈特效
                if (Main.rand.NextBool(6)) // 大约每6帧生成一次
                {
                    float pulseScale = Main.rand.NextFloat(0.3f, 0.4f); // 随机生成粒子的缩放范围
                    DirectionalPulseRing pulse = new DirectionalPulseRing(
                        projectile.Center + Main.rand.NextVector2Circular(12, 12), // 初始位置在弹幕中心附近随机偏移
                        new Vector2(2, 2).RotatedByRandom(100) * Main.rand.NextFloat(0.2f, 1.1f), // 初速度为随机方向和强度
                        (Main.rand.NextBool(3) ? Color.LimeGreen : Color.Green) * 0.8f, // 随机颜色为浅绿或深绿
                        new Vector2(1, 1), // 粒子默认没有形状变形（等比例）
                        pulseScale - 0.25f, // 初始旋转角度
                        pulseScale, // 初始大小
                        0f, // 最终大小
                        15 // 粒子的生命周期为15帧
                    );
                    GeneralParticleHandler.SpawnParticle(pulse); // 生成粒子
                }
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (IsPlagueGelInfused && target.active && !target.friendly)
            {
                // 检查弹幕的所有者是否为本地玩家，避免多人游戏同步问题
                if (projectile.owner == Main.myPlayer)
                {
                    // 获取弹幕所有者玩家
                    Player owner = Main.player[projectile.owner];

                    // 检查玩家是否在丛林区域
                    if (owner != null && owner.ZoneJungle)
                    {
                        // 为目标施加 10 秒的 PlagueGelEDebuff 和 Plague
                        target.AddBuff(ModContent.BuffType<PlagueGelEDebuff>(), 600); // 600 帧等于 10 秒
                        target.AddBuff(ModContent.BuffType<Plague>(), 600);
                    }
                }
            }
        }
    }
}

