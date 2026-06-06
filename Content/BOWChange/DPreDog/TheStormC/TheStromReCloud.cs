using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.DPreDog.TheStormC
{
    internal class TheStromReCloud : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.DPreDog";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
        }

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 54;
            Projectile.height = 28;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 120;
            Projectile.Opacity = 0f;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.975f;
            if (Main.GameUpdateCount % 5 == 0)
                BowChangeVFX.SpawnCharge(Projectile, Projectile.Center, BowChangeTheme.Storm, Projectile.Opacity, 0.35f);

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                int maxFrame = Projectile.timeLeft < 60 ? 6 : 3;
                if (Projectile.frame >= maxFrame)
                    Projectile.frame = 0;

                //// 在最后 60 帧时且尚未生成闪电时劈下一道闪电
                //if (Projectile.frame == 5 && Main.myPlayer == Projectile.owner && Projectile.timeLeft <= 60 && !hasStruckLightning)
                //{
                //    SoundEngine.PlaySound(CommonCalamitySounds.LightningSound, Projectile.Center);
                //    float ai = Main.rand.Next(100);
                //    Vector2 velocity = Vector2.UnitY * 7f;

                //    // 生成我方激光弹幕，并设置穿透次数
                //    int lightningProjectile = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Bottom, velocity, ProjectileID.CultistBossLightningOrbArc, Projectile.damage, 0f, Projectile.owner, MathHelper.PiOver2, ai);

                //    // 设置为友方并增加穿透次数
                //    Projectile proj = Main.projectile[lightningProjectile];
                //    proj.friendly = true;
                //    proj.hostile = false;
                //    proj.penetrate = 10; // 设置穿透次数
                //    proj.localNPCHitCooldown = 50; // 无敌帧冷却时间
                //    proj.usesLocalNPCImmunity = true;

                //    // 标记已生成闪电
                //    hasStruckLightning = true;
                //}
            }

            if (Projectile.timeLeft < 30)
                Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 0f, 0.14f);
            else
                Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 1f, 0.33f);

            // 粒子特效和弹幕生成逻辑
            if (Projectile.timeLeft % 15 == 0) // 每 x 帧触发一次
            {
                GenerateEnergyProjectiles();
            }

        }

        //public override void OnKill(int timeLeft) // 死亡时释放随机角度的3个小龙卷风
        //{
        //    float speed = 15f; // 初始速度
        //    float baseAngle = Main.rand.NextFloat(0, MathHelper.TwoPi); // 随机生成基础角度
        //    float angleIncrement = MathHelper.TwoPi / 3; // 每个弹幕的夹角为120度

        //    for (int i = 0; i < 3; i++) // 平均释放3个弹幕
        //    {
        //        float angle = baseAngle + angleIncrement * i; // 当前弹幕的角度
        //        Vector2 velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * speed;

        //        Projectile.NewProjectile(
        //            Projectile.GetSource_FromThis(),
        //            Projectile.Center,
        //            velocity,
        //            ModContent.ProjectileType<TheStromReWind>(),
        //            (int)(Projectile.damage * 0.33f), // 伤害倍率为0.33
        //            0f,
        //            Projectile.owner
        //        );
        //    }
        //}


        private void GenerateEnergyProjectiles()
        {
            // 找到最近的敌人
            NPC target = FindClosestNPC();
            if (target == null) return; // 没有敌人则不生成弹幕

            Player player = Main.player[Projectile.owner];

            for (int i = 0; i < Main.rand.Next(1, 3); i++) // 随机生成 1~2 个弹幕
            {
                if (player.HasAmmo(player.HeldItem)) // 检查玩家是否有弹药
                {
                    if (player.PickAmmo(player.HeldItem, out int ammoProjectile, out _, out int damage, out float knockback, out _))
                    {
                        // 检查弹药类型
                        int finalProjectile = ammoProjectile == ProjectileID.WoodenArrowFriendly
                            ? ModContent.ProjectileType<TheStormReEnergy>() // 转化为特殊弹幕
                            : ammoProjectile;

                        // 计算发射位置
                        float offsetX = Main.rand.NextFloat(-19f, 19f) * (i % 2 == 0 ? 1 : -1);
                        Vector2 spawnPosition = Projectile.Center + new Vector2(offsetX, 0);

                        // 计算弹幕方向
                        Vector2 velocity = (target.Center - spawnPosition).SafeNormalize(Vector2.Zero) * 40f;

                        // 生成弹幕
                        Projectile.NewProjectile(
                            Projectile.GetSource_FromThis(),
                            spawnPosition,
                            velocity,
                            finalProjectile, // 使用转化后的弹幕类型
                            damage,
                            knockback,
                            Projectile.owner
                        );
                        BowChangeVFX.SpawnMuzzle(Projectile, spawnPosition, velocity, BowChangeTheme.Storm, 0.45f);

                        // 生成粒子特效
                        GenerateEnergyParticles(spawnPosition);
                    }
                }
            }
        }


        private void GenerateEnergyParticles(Vector2 position)
        {
            for (int j = 0; j < Main.rand.Next(3, 5); j++) // 随机生成 3~5 个粒子
            {
                Vector2 particleVelocity = new Vector2(
                    Main.rand.NextFloat(-2f, 2f),
                    Main.rand.NextFloat(-1f, 1f)
                );
                Dust.NewDustPerfect(position, DustID.Electric, particleVelocity, 100, Color.Cyan, 1.2f).noGravity = true;
            }
        }

        private NPC FindClosestNPC()
        {
            NPC closestNPC = null;
            float minDistance = float.MaxValue;

            foreach (NPC npc in Main.npc)
            {
                if (npc.active && !npc.friendly && npc.life > 0 && npc.CanBeChasedBy())
                {
                    float distance = Vector2.Distance(Projectile.Center, npc.Center);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestNPC = npc;
                    }
                }
            }

            return closestNPC;
        }
    }
}
