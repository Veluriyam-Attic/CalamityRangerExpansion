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
using CalamityMod.Particles;

namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.BrassBeast
{
    public class BrassBeastHoldOut : BaseGunHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.BrassBeast";
        public override string Texture => "CalamityRangerExpansion/Content/DeveloperItems/Weapon/BrassBeast/BrassBeast";

        public override int AssociatedItemID => ModContent.ItemType<BrassBeast>();
        public override float MaxOffsetLengthFromArm => 20f; // 武器持有距离
        public override float OffsetXUpwards => -10f;
        public override float BaseOffsetY => -15f;
        public override float OffsetYDownwards => 10f;

        private int frameCounter = 0; // 帧计数器

        //public override Vector2 GunTipPosition => Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * (Projectile.width * 0.5f + 10f);
        public override Vector2 GunTipPosition => Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * (Projectile.width * 0.5f + 0f);

        public override void HoldoutAI()
        {
            Player player = Main.player[Projectile.owner];

            frameCounter++;
            if (frameCounter % 15 == 0) // 每15帧发射一次子弹
            {
                ShootProjectile(player);
                CreateShell();
                CreateParticles();
                OffsetLengthFromArm -= 15f; // 模拟后坐力
            }

            // 逐渐恢复后坐力位置
            if (OffsetLengthFromArm < MaxOffsetLengthFromArm)
            {
                OffsetLengthFromArm = MathHelper.Lerp(OffsetLengthFromArm, MaxOffsetLengthFromArm, 0.1f);
            }
        }

        private void ShootProjectile(Player player)
        {
            //Vector2 direction = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitX);
            Vector2 direction = Vector2.UnitX.RotatedBy(Projectile.rotation);

            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                GunTipPosition,
                direction * 20f, // 子弹速度
                ModContent.ProjectileType<BrassBeastPROJ>(),
                Projectile.damage,
                Projectile.knockBack,
                player.whoAmI
            );
            //SoundEngine.PlaySound(SoundID.Item40, Projectile.Center); // 发射音效
            SoundEngine.PlaySound(new SoundStyle("CalamityRangerExpansion/Sound/新突击兵"), Projectile.Center);

        }

        private void CreateShell()
        {
            Vector2 shellPosition = GunTipPosition;
            Vector2 shellDirection = -Vector2.UnitX.RotatedBy(Projectile.rotation) + new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-0.5f, 0.5f));
            Point anchorPos = new Point((int)shellPosition.X / 16, (int)shellPosition.Y / 16);

            Color burnColor = Main.rand.NextBool(4) ? Color.PaleGreen : Main.rand.NextBool(4) ? Color.PaleTurquoise : Color.OrangeRed;
            Particle shell = new TitaniumRailgunShell(shellPosition, anchorPos, Projectile.rotation + MathHelper.PiOver2, burnColor);
            //GeneralParticleHandler.SpawnParticle(shell);
        }

        private void CreateParticles()
        {
            // 枪口粒子特效
            for (int i = 0; i < 15; i++)
            {
                Vector2 randomDirection = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f)).SafeNormalize(Vector2.UnitX);
                float speed = Main.rand.NextFloat(2f, 4f);
                int dustType = Main.rand.Next(new int[] { DustID.Smoke, DustID.RainbowTorch, DustID.GemDiamond });

                // 上后方粒子
                Dust.NewDustPerfect(GunTipPosition, dustType, -randomDirection * speed, 0, default, 1.2f).noGravity = true;

                // 下后方粒子
                Dust.NewDustPerfect(GunTipPosition, dustType, randomDirection * speed, 0, default, 1.2f).noGravity = true;
            }

            // 尖字型粒子特效
            for (int i = 0; i < 5; i++)
            {
                float angle = MathHelper.ToRadians(120) * i;
                Vector2 particleDirection = Vector2.UnitX.RotatedBy(Projectile.rotation + angle);
                PointParticle spark = new PointParticle(
                    GunTipPosition,
                    particleDirection * 5f, // 固定方向与速度
                    false,
                    20,
                    1.3f,
                    Color.Orange
                );
                GeneralParticleHandler.SpawnParticle(spark);
            }
        }
    }
}
