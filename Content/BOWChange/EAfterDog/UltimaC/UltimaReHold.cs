using CalamityMod;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.EAfterDog.UltimaC
{
    internal class UltimaReHold : BaseGunHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.EAfterDog";
        public override string Texture => "CalamityRangerExpansion/Content/BOWChange/EAfterDog/UltimaC/UltimaRe";
        public override int AssociatedItemID => ModContent.ItemType<UltimaRe>();
        public override bool? CanDamage() => false;

        private int shotCount = 0; // 记录射击次数
        private int phase = 1; // 阶段
        private const int ShotsPerPhase = 25; // 每阶段射击次数

        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
            Player player = Main.player[Projectile.owner];

            int damage = Projectile.damage; // 获取 UltimaReHold 的伤害值

            // 生成两个僚机，并传递伤害值
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center + new Vector2(-50, 0), Vector2.Zero, ModContent.ProjectileType<UltimaWingMAN>(), damage, 0, player.whoAmI, -1);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center + new Vector2(50, 0), Vector2.Zero, ModContent.ProjectileType<UltimaWingMAN>(), damage, 0, player.whoAmI, 1);
        }
        private int fireCooldown = 7; // 定义冷却时间计数器
        public override void HoldoutAI()
        {
            Player player = Main.player[Projectile.owner];
            BowChangeVFX.SpawnCharge(Projectile, GunTipPosition, BowChangeTheme.Ultima, MathHelper.Clamp(shotCount / (float)(ShotsPerPhase * 3), 0f, 1f), 0.55f);

            if (shotCount >= ShotsPerPhase * 2)
            {
                phase = 3; // 进入过渡阶段
            }
            else if (shotCount >= ShotsPerPhase)
            {
                phase = 2; // 进入第二阶段
            }

            // 冷却时间计数器减少
            if (fireCooldown > 0)
            {
                fireCooldown--;
            }
            else
            {
                FireProjectile(player); // 调用发射逻辑
                fireCooldown = 7; // 重置冷却时间
            }
        }

        private void FireProjectile(Player player)
        {
            // 发射位置略有偏移
            Vector2 shotDirection = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitX);
            Vector2 shotPosition = Projectile.Center + Main.rand.NextVector2Circular(30, 30);

            int damage = player.GetWeaponDamage(player.HeldItem);
            float knockBack = player.HeldItem.knockBack;

            if (phase == 1)
            {
                // 第一阶段：仅发射 UltimaBolt
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), shotPosition, shotDirection * 35f, ModContent.ProjectileType<UltimaBolt>(), damage, knockBack, player.whoAmI);
            }
            else if (phase == 2)
            {
                // 第二阶段：发射 UltimaBolt，并有 10% 概率发射 UltimaSpark
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), shotPosition, shotDirection * 35f, ModContent.ProjectileType<UltimaBolt>(), damage, knockBack, player.whoAmI);
                if (Main.rand.NextFloat() <= 0.25f)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), shotPosition, shotDirection * 35f, ModContent.ProjectileType<UltimaSpark>(), damage, knockBack, player.whoAmI);
                }
            }
            else if (phase == 3)
            {
                // 第三阶段：每发 UltimaBolt 有概率替换为 UltimaRay，概率逐步递增
                float rayChance = Math.Min(0.02f * (shotCount - ShotsPerPhase * 2), 1f); // 概率最大为 100%
                int projectileType = Main.rand.NextFloat() <= rayChance ? ModContent.ProjectileType<UltimaRay>() : ModContent.ProjectileType<UltimaBolt>();
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), shotPosition, shotDirection * 12f, projectileType, damage, knockBack, player.whoAmI);
            }
            //SoundEngine.PlaySound(player.ActiveItem().UseSound.GetValueOrDefault(), Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item88, Projectile.Center);
            BowChangeVFX.SpawnMuzzle(Projectile, shotPosition, shotDirection * 18f, BowChangeTheme.Ultima, phase == 3 ? 0.9f : 0.65f);
            shotCount++;

            if (shotCount % 12 == 0 || shotCount == ShotsPerPhase || shotCount == ShotsPerPhase * 2)
                SpawnConstellationNode(player, shotPosition, shotDirection);
        }

        private void SpawnConstellationNode(Player player, Vector2 shotPosition, Vector2 shotDirection)
        {
            Vector2 side = shotDirection.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(-90f, 90f);
            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                shotPosition + side,
                shotDirection.RotatedByRandom(0.35f) * 4f,
                ModContent.ProjectileType<UltimaReConstellationNode>(),
                (int)(Projectile.damage * (phase == 3 ? 0.72f : 0.52f)),
                Projectile.knockBack,
                player.whoAmI,
                phase);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.active && proj.owner == Projectile.owner && proj.type == ModContent.ProjectileType<UltimaWingMAN>())
                {
                    proj.Kill();
                }
            }
        }
    }
}
