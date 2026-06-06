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
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Projectiles.Typeless;
using Terraria.DataStructures;
using CalamityRangerExpansion.Content.BOWChange.CPreMoodLord.VernalBolterC;

namespace CalamityRangerExpansion.Content.BOWChange.APreHardMode.LunarianBowC.废弃
{
    internal class LBGasoline : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.APreHardMode";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
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
            // 根据方案设置 aiStyle
            if (Projectile.ai[0] == 1) // 方案 a：Venom
            {
                Projectile.aiStyle = ProjAIStyleID.Arrow;
            }
            else if (Projectile.ai[0] == 2) // 方案 b：Poisoned
            {
                Projectile.aiStyle = ProjAIStyleID.Arrow;
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            Projectile.velocity *= 1.4f;

            // 检测玩家手持的武器并设置方案
            if (player.HeldItem.type == ModContent.ItemType<LunarianBowRe>()) // 检测是否是 LunarianBowRe
            {
                Projectile.ai[0] = 1; // 设置为方案 A
            }
            else if (player.HeldItem.type == ModContent.ItemType<VernalBolterRe>()) // 检测是否是 VernalBolterRe
            {
                Projectile.ai[0] = 2; // 设置为方案 B
            }
            else
            {
                Projectile.ai[0] = 1; // 默认方案 A
            }
        }
        public override void AI()
        {
            // 调整弹幕的旋转，使其在飞行时保持水平
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;

            // Lighting - 添加天蓝色光源，光照强度为 0.49
            Lighting.AddLight(Projectile.Center, Color.LightSkyBlue.ToVector3() * 0.49f);

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 根据方案选择不同的 Debuff
            if (Projectile.ai[0] == 1) // 方案 a
            {
                target.AddBuff(BuffID.OnFire, 300);
            }
            else if (Projectile.ai[0] == 2) // 方案 b
            {
                target.AddBuff(BuffID.CursedInferno, 300);
                target.AddBuff(BuffID.OnFire3, 300);
            }
        }
        public override void OnKill(int timeLeft)
        {
            // 生成火焰粉尘特效
            {
                for (int j = 0; j < 5; j++)
                {
                    int fire = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 2f);
                    Main.dust[fire].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[fire].scale = 0.5f;
                        Main.dust[fire].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int k = 0; k < 10; k++)
                {
                    int fire = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 3f);
                    Main.dust[fire].noGravity = true;
                    Main.dust[fire].velocity *= 5f;
                    fire = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 6, 0f, 0f, 100, default, 2f);
                    Main.dust[fire].velocity *= 2f;
                }
            }


            if (Projectile.ai[0] == 1) // 方案 a
            {
                // 生成 FuckYou 弹幕
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<FuckYou>(),
                    (int)(Projectile.damage * 0.75f), // 伤害倍率为 0.75
                    0f,
                    Projectile.owner,
                    ai0: 0.5f // 大小为 0.5
                );
            }
            else if (Projectile.ai[0] == 2) // 方案 b
            {
                // 生成 FuckYou 弹幕
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<FuckYou>(),
                    (int)(Projectile.damage * 1.0f), // 伤害倍率为 1.0
                    0f,
                    Projectile.owner,
                    ai0: 1.0f // 大小为 1.0
                );

                // 与此同时，生成 TotalityFire 特效弹幕
                for (int i = 0; i < 6; i++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f, 0.1f);
                    int flames = Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        Projectile.Center,
                        velocity,
                        ModContent.ProjectileType<TotalityFire>(),
                        (int)(Projectile.damage * 0.3f), // 伤害倍率为 0.3
                        0f,
                        Projectile.owner
                    );
                    if (flames.WithinBounds(Main.maxProjectiles))
                    {
                        Main.projectile[flames].DamageType = DamageClass.Ranged;
                        Main.projectile[flames].penetrate = 3;
                        Main.projectile[flames].usesLocalNPCImmunity = false;
                        Main.projectile[flames].usesIDStaticNPCImmunity = true;
                        Main.projectile[flames].idStaticNPCHitCooldown = 10;
                    }
                }
            }
        }







    }
}