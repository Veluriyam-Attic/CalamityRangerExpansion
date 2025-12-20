using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using System;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using CalamityMod;
using CalamityMod.Items.Weapons.Ranged;
using Terraria.ModLoader;

namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.HD2.SG225IE
{
    internal class SG225IEHoldOut : BaseGunHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.SG225IE";
        public override string Texture => "CalamityRangerExpansion/Content/DeveloperItems/Weapon/HD2/SG225IE/SG225IE";
        public override int AssociatedItemID => ModContent.ItemType<SG225IE>();

        public override float MaxOffsetLengthFromArm => 36f;
        public override float RecoilResolveSpeed => 0.15f;
        public override float OffsetXUpwards => -8f;
        public override float OffsetXDownwards => 2f;
        public override float BaseOffsetY => -10f;
        public override float OffsetYUpwards => 12f;
        public override float OffsetYDownwards => 6f;

        // ai[0]：主计时器
        // ai[1]：当前爆发中已射出的轮数
        public ref float timer => ref Projectile.ai[0];
        public ref float burstShots => ref Projectile.ai[1];

        private const int PreDelay = 40;     // 爆发前等待
        private const int PostDelay = 40;    // 爆发后等待
        private const int BurstCount = 6;    // 每次爆发射 6 轮
        private const int BurstInterval = 4; // 爆发内两轮之间的间隔

        public override void HoldoutAI()
        {
            Vector2 muzzleDirection = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            timer++;

            // =========================
            // 爆发阶段
            // =========================
            if (burstShots > 0)
            {
                // 每隔 BurstInterval 帧射一轮
                if (timer >= BurstInterval)
                {
                    timer = 0;
                    FireOnce(muzzleDirection);
                    burstShots++;

                    // 6 轮射完，进入冷却
                    if (burstShots > BurstCount)
                    {
                        burstShots = 0;
                        timer = -PostDelay; // 用负数表示冷却阶段
                    }
                }
                return;
            }

            // =========================
            // 冷却阶段
            // =========================
            if (timer < 0)
                return;

            // =========================
            // 等待完成，进入爆发
            // =========================
            if (timer >= PreDelay)
            {
                timer = 0;
                burstShots = 1;
                FireOnce(muzzleDirection);
            }
        }

        private void FireOnce(Vector2 muzzleDirection)
        {
            if (!Owner.PickAmmo(Owner.ActiveItem(), out int pickedProjType, out float shootSpeed, out int damage, out float knockback, out int ammoItemType))
                return;

            Vector2 muzzle = GunTipPosition;
            int projectileCount = Main.rand.Next(8, 13); // 8~12 发散射

            for (int i = 0; i < projectileCount; i++)
            {
                float spread = MathHelper.ToRadians(Main.rand.NextFloat(-10f, 10f));
                float speedMod = Main.rand.NextFloat(0.85f, 1.15f);
                float damageMod = Main.rand.NextFloat(0.75f, 1.1f);

                Vector2 perturbedVelocity =
                    muzzleDirection.RotatedBy(spread) * shootSpeed * speedMod;

                int projToShoot = pickedProjType;

                // 普通火枪子弹 → 自定义火焰弹
                if (ammoItemType == ItemID.MusketBall)
                    projToShoot = ModContent.ProjectileType<SG225IEProj>();

                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    muzzle,
                    perturbedVelocity,
                    projToShoot,
                    (int)(damage * damageMod),
                    knockback,
                    Projectile.owner
                );
            }

            // 💥 后坐力
            Owner.velocity += -muzzleDirection * 0.8f;

            // 📸 屏幕震动
            if (Main.myPlayer == Owner.whoAmI)
            {
                float shakePower = 2.5f;
                float distFactor = Utils.GetLerpValue(1000f, 0f, Projectile.Distance(Main.LocalPlayer.Center), true);
                Main.LocalPlayer.Calamity().GeneralScreenShakePower =
                    Math.Max(Main.LocalPlayer.Calamity().GeneralScreenShakePower, shakePower * distFactor);
            }

            // 🔊 音效
            SoundEngine.PlaySound(
                new SoundStyle("CalamityMod/Sounds/Item/FlakKrakenShoot")
                { Pitch = 0.5f, Volume = 0.6f },
                muzzle
            );
        }
    }
}
