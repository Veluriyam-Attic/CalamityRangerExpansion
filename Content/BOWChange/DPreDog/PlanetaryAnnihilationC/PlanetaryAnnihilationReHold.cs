 
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityRangerExpansion.Content.BOWChange.APreHardMode.ToxibowC;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityRangerExpansion.Content.BOWChange.APreHardMode.LunarianBowC;
using CalamityMod.Projectiles.Ranged;
using CalamityRangerExpansion.Content.BOWChange.DPreDog.PlanetaryAnnihilationC.SolarSystemPROJ;
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.DPreDog.PlanetaryAnnihilationC
{
    internal class PlanetaryAnnihilationReHold : BaseGunHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.DPreDog";

        public override string Texture => "CalamityRangerExpansion/Content/BOWChange/DPreDog/PlanetaryAnnihilationC/PlanetaryAnnihilationRe";

        public override int AssociatedItemID => ModContent.ItemType<PlanetaryAnnihilationRe>();

        public override bool? CanDamage() => Main.player[Projectile.owner].GetModPlayer<OpenBowDamagePlayer>().OpenBowDamage;

        public override Vector2 GunTipPosition => Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * (Projectile.width * 0.5f + 10f);

        public override float MaxOffsetLengthFromArm => 15f; // 后坐力偏移


        private int frameCounter = 0; // 帧计数器
        private int attackCycle = 0;  // 当前发射类型编号
        private int attackRound = 0; // 当前攻击轮数

        private const int FireInterval = 15; // 每次攻击的间隔为x帧
        private const int PauseInterval = 45; // 每8轮攻击后的停顿时间
        private const int MaxRounds = 8; // 最大攻击轮数

        private readonly int[] projectileTypes = {
            ModContent.ProjectileType<PAMercury>(),  // 水星
            ModContent.ProjectileType<PBVenus>(),   // 金星
            ModContent.ProjectileType<PCEarth>(),   // 地球
            ModContent.ProjectileType<PDMars>(),    // 火星
            ModContent.ProjectileType<PEJupiter>(), // 木星
            ModContent.ProjectileType<PFSaturn>(),  // 土星
            ModContent.ProjectileType<PGUranus>(),  // 天王星
            ModContent.ProjectileType<PHNeptune>()  // 海王星
        };

        public override void HoldoutAI()
        {
            frameCounter++;
            float cycleProgress = (attackCycle + attackRound * projectileTypes.Length) / (float)(MaxRounds * projectileTypes.Length);
            BowChangeVFX.SpawnCharge(Projectile, GunTipPosition, BowChangeTheme.Planetary, cycleProgress, 0.45f);

            if (frameCounter >= FireInterval)
            {
                frameCounter = 0;

                if (attackCycle < projectileTypes.Length)
                {
                    FireProjectile(attackCycle);
                    attackCycle++;

                    // 如果一轮结束，重置并增加轮数
                    if (attackCycle == projectileTypes.Length)
                    {
                        attackCycle = 0;
                        attackRound++;
                    }
                }

                // 如果达到最大轮数，进入停顿阶段
                if (attackRound >= MaxRounds)
                {
                    attackRound = 0;
                    frameCounter = -PauseInterval; // 设置停顿时间
                }
            }
        }

        private void FireProjectile(int cycle)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 shootDirection = Projectile.velocity.SafeNormalize(Vector2.UnitX * player.direction);
            Vector2 side = shootDirection.RotatedBy(MathHelper.PiOver2);

            // 两个位置的偏移量
            Vector2 leftOffset = -side * 10f;
            Vector2 rightOffset = side * 10f;

            // 两个偏移向量
            Vector2[] offsets = { leftOffset, rightOffset };

            foreach (Vector2 offset in offsets)
            {
                Vector2 spawnPosition = GunTipPosition + offset;

                // 发射弹幕
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    spawnPosition,
                    shootDirection * 30f,
                    projectileTypes[cycle], // 当前弹幕类型
                    Projectile.damage,
                    Projectile.knockBack,
                    player.whoAmI
                );
            }

            // 固定音效
            SoundEngine.PlaySound(SoundID.Item75, player.Center);
            BowChangeVFX.SpawnMuzzle(Projectile, GunTipPosition, shootDirection * 30f, BowChangeTheme.Planetary, 0.75f);

            if (cycle == projectileTypes.Length - 1)
            {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    GunTipPosition + shootDirection * 360f,
                    shootDirection * 2f,
                    ModContent.ProjectileType<PlanetaryAnnihilationReGravityWell>(),
                    (int)(Projectile.damage * 0.55f),
                    Projectile.knockBack,
                    player.whoAmI);
            }
        }

    }
}
