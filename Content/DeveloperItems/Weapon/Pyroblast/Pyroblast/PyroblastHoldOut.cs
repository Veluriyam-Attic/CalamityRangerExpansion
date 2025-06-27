using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.Audio;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Particles;

namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.Pyroblast
{
    public class PyroblastHoldOut : BaseGunHoldoutProjectile, ILocalizedModType
    {
        public override string Texture => "CalamityRangerExpansion/Content/DeveloperItems/Weapon/Pyroblast/Pyroblast";
        public new string LocalizationCategory => "DeveloperItems.Pyroblast";
        public override int AssociatedItemID => ModContent.ItemType<Pyroblast>(); // 绑定武器
        public override Vector2 GunTipPosition => Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * (Projectile.width * 0.5f + 10f);
        public override float MaxOffsetLengthFromArm => 15f; // 设置与手臂的距离
        public override float OffsetXUpwards => -12f; // 武器向上的偏移
        public override float BaseOffsetY => -10f; // 基础的Y轴偏移
        public override float OffsetYDownwards => 10f; // 武器向下的偏移

        public int upgradeLevel = 1; // 当前等级
        public int frameCounter = 0; // 帧计数器
        public int upgradeTimer = 0; // 用于升级的计时器

        public override void HoldoutAI()
        {
            Player player = Main.player[Projectile.owner];

            // 检查玩家是否未手持 Pyroblast
            if (player.HeldItem.type != ModContent.ItemType<Pyroblast>())
            {
                DisableEnhancedLogic(); // 取消所有强化逻辑
                return; // 退出逻辑，避免其他操作
            }

            // 每6帧生成一个子弹
            frameCounter++;
            if (frameCounter % 6 == 0)
            {
                ShootPyroblast(player);
            }

            // 升级逻辑
            if (upgradeLevel < 6)
            {
                upgradeTimer++;
                if (upgradeTimer >= 300) // 每5秒升级一次
                {
                    UpgradeLevel();
                    upgradeTimer = 0;
                }
            }

            // 根据当前等级执行额外逻辑
            ExecuteUpgradeLogic(player);
        }

        private void ShootPyroblast(Player player)
        {
            for (int i = 0; i < 2; i++) // 循环生成两发子弹
            {
                // 生成 PyroblastPROJ 子弹，并设置初始速度与随机偏移
                Vector2 direction = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitX).RotatedByRandom(MathHelper.ToRadians(2));
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    GunTipPosition,
                    direction * 12f,
                    ModContent.ProjectileType<PyroblastPROJ>(),
                    Projectile.damage,
                    Projectile.knockBack,
                    player.whoAmI
                );
            }

            // 后坐力效果：武器后退2像素
            OffsetLengthFromArm -= 2f;

            // 播放开火音效
            SoundEngine.PlaySound(SoundID.Item41, Projectile.Center);
        }


        private void UpgradeLevel()
        {
            upgradeLevel++;
            if (upgradeLevel > 6) upgradeLevel = 6; // 限制等级最大为6

            if (upgradeLevel <= 6)
            {
                // 升级特效：玩家中心产生大量火焰与岩浆粒子
                for (int i = 0; i < 30; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(10f, 10f);
                    Dust.NewDustPerfect(Main.LocalPlayer.Center, DustID.Torch, velocity, 100, Color.OrangeRed, 2f).noGravity = true;
                }
                for (int i = 0; i < 30; i++) // 生成x个橙色粒子
                {
                    Vector2 velocity = Main.rand.NextVector2CircularEdge(10f, 10f).RotatedByRandom(MathHelper.ToRadians(20)) * Main.rand.NextFloat(2f, 5f);
                    LineParticle subTrail = new LineParticle(Projectile.Center, velocity * 35, false, 30, 0.75f, Color.Orange);
                    GeneralParticleHandler.SpawnParticle(subTrail);
                }
                SoundEngine.PlaySound(SoundID.Item25, Main.LocalPlayer.Center); // 播放升级音效
            }
        }

        private void ExecuteUpgradeLogic(Player player)
        {
            if (upgradeLevel >= 2)
            {
                ShootLazharSolarBeam(player); // 发射激光
            }

            if (upgradeLevel >= 3)
            {
                LaunchMissile(player); // 发射导弹
            }

            if (upgradeLevel >= 4)
            {
                PyroblastPROJ.EnableHoming = true; // 启用 PyroblastPROJ 强化
            }

            if (upgradeLevel >= 5)
            {
                PyroblastSolarBeam.IsEnhanced = true; // 启用 PyroblastSolarBeam 强化
            }

            if (upgradeLevel == 6)
            {
                PyroblastRocket.EnableSpecialAbility = true; // 启用 PyroblastRocket 强化
            }
        }

        private void DisableEnhancedLogic()
        {
            PyroblastPROJ.EnableHoming = false;
            PyroblastSolarBeam.IsEnhanced = false;
            PyroblastRocket.EnableSpecialAbility = false;
        }
        public override void OnKill(int timeLeft)
        {
            DisableEnhancedLogic();
        }
        // 射出有一定随机角度的激光
        private void ShootLazharSolarBeam(Player player)
        {
            if (frameCounter % 10 == 0) // 每隔一段时间射击一次
            {
                // 计算朝向鼠标的基础方向
                Vector2 baseDirection = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitX);

                // 在基础方向上随机偏移 ±3 度
                Vector2 randomizedDirection = baseDirection.RotatedByRandom(MathHelper.ToRadians(3));

                // 发射激光
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    GunTipPosition,
                    randomizedDirection * 10f, // 设置速度为方向的 10 倍
                    ModContent.ProjectileType<PyroblastSolarBeam>(),
                    Projectile.damage,
                    Projectile.knockBack,
                    player.whoAmI
                );
            }
        }

        // 第3阶段发射导弹
        private void LaunchMissile(Player player)
        {
            if (frameCounter % 15 == 0) // 每隔15帧发射导弹
            {
                Vector2 direction = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitX);
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    GunTipPosition,
                    direction * 12f, // 设置速度为方向的12倍
                    ModContent.ProjectileType<PyroblastRocket>(),
                    (int)(Projectile.damage * 2.0f), // 伤害倍率为2.0
                    Projectile.knockBack,
                    player.whoAmI
                );

                // 播放发射导弹音效
                SoundEngine.PlaySound(SoundID.Item61.WithVolumeScale(0.002f), Projectile.Center);
            }
        }
    }
}
