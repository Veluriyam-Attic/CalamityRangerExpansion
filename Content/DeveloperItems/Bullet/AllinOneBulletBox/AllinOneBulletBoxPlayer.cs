using CalamityRangerExpansion.Content.DeveloperItems.Bullet.ShadowsBullet;
using CalamityRangerExpansion.LightingBolts.Metaballs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;

namespace CalamityRangerExpansion.Content.DeveloperItems.Bullet.AllinOneBulletBox
{
    public class AllinOneBulletBoxPlayer : ModPlayer
    {
        public bool BulletBoxActive = false;
        private int timer = 0;

        public override void ResetEffects()
        {
            BulletBoxActive = false; // 确保未佩戴饰品时不会残留效果
        }

        public override void PostUpdate()
        {
            if (!BulletBoxActive)
                return;

            // **持续生成 metaball 特效**
            Vector2 spawnPosition = Main.MouseWorld + Main.rand.NextVector2Circular(5f, 5f);
            Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f);
            float size = Main.rand.NextFloat(15f, 30f);
            ShadowAmmoMetaball.SpawnParticle(spawnPosition, velocity, size);

            // **计时器**
            timer++;
            if (timer >= 300) // 5秒触发一次 (60帧 * 5)
            {
                GenerateBulletCircle(Main.MouseWorld);
                timer = 0;
            }
        }

        private void GenerateBulletCircle(Vector2 center)
        {
            int numProjectiles = Main.rand.Next(5, 10); // 每次生成 5~10 个弹幕
            float minRadius = 5 * 16;
            float maxRadius = 15 * 16;

            for (int i = 0; i < numProjectiles; i++)
            {
                float angle = MathHelper.TwoPi * Main.rand.NextFloat();
                float radius = MathHelper.Lerp(minRadius, maxRadius, Main.rand.NextFloat());
                Vector2 spawnPos = center + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * radius;
                Vector2 shootVelocity = Vector2.Normalize(center - spawnPos) * 10f;

                // 获取 ShadowsBulletPROJ.cs 里的子弹容器
                int bulletType;
                int containerType = Main.rand.Next(3);
                if (containerType == 0)
                    bulletType = ShadowsBulletPROJ.VanillaProjectiles[Main.rand.Next(ShadowsBulletPROJ.VanillaProjectiles.Length)];
                else if (containerType == 1)
                    bulletType = ShadowsBulletPROJ.CalamityProjectiles[Main.rand.Next(ShadowsBulletPROJ.CalamityProjectiles.Length)];
                else
                    bulletType = Mod.Find<ModProjectile>(ShadowsBulletPROJ.CustomModProjectiles[Main.rand.Next(ShadowsBulletPROJ.CustomModProjectiles.Length)]).Type;

                Projectile.NewProjectile(
                    Player.GetSource_FromThis(),
                    spawnPos,
                    shootVelocity,
                    bulletType,
                    30,  // 伤害
                    2f,  // 击退
                    Player.whoAmI
                );
            }
        }
    }
}
