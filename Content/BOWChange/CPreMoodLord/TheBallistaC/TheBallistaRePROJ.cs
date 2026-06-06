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
using Terraria.DataStructures;
using CalamityMod.Buffs.StatDebuffs;
using Terraria.Audio;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Particles;
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.CPreMoodLord.TheBallistaC
{
    internal class TheBallistaRePROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.CPreMoodLord";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
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
        }
        public override void OnSpawn(IEntitySource source)
        {
            float level = Projectile.ai[0]; // 来自蓄力进度（0~1）
            Projectile.localAI[0] = MathHelper.Clamp(level, 0f, 1f);

            // 动态设定命中无敌帧，最小为 4（越高越快）
            Projectile.localNPCHitCooldown = (int)MathHelper.Clamp(15 - level * 11f, 4f, 15f);

            // 动态设定 extraUpdates，提高速度平滑感（最多3）
            Projectile.extraUpdates = (int)MathHelper.Clamp(1 + level * 2f, 1f, 3f);

            Projectile.penetrate = (int)MathHelper.Clamp(1 + level * 8f, 1f, 9f); // 穿透次数：最低1，最高9
            Projectile.usesLocalNPCImmunity = true; // 启用本地无敌帧
        }

        public override void AI()
        {
            float level = Projectile.localAI[0];

            // 调整弹幕的旋转，使其在飞行时保持水平
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;

            // Lighting - 添加天蓝色光源，光照强度为 0.49
            Lighting.AddLight(Projectile.Center, Color.LightSkyBlue.ToVector3() * 0.49f);
            BowChangeVFX.SpawnTrail(Projectile, BowChangeTheme.Ballista, 0.55f + level * 0.55f);

            // 飞行时的烟雾
            Projectile.localAI[1] += 1f;
            if (Projectile.localAI[1] > 6f)
            {
                for (int d = 0; d < 5; d++)
                {
                    Dust dust = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Dirt, Projectile.velocity.X, Projectile.velocity.Y, 100, default, 1f)];
                    dust.velocity = Vector2.Zero;
                    dust.position -= Projectile.velocity / 5f * d;
                    dust.noGravity = true;
                    dust.scale = 0.65f;
                    dust.noLight = true;
                }
            }

            // 尾部雨刮式粒子
            if (Main.rand.NextBool(2))
            {
                Vector2 offset = Vector2.UnitY.RotatedBy(MathHelper.PiOver2 * MathF.Sign(MathF.Sin(Projectile.timeLeft / 5f))) * 6f;
                Vector2 pos = Projectile.Center + offset;

                Particle spark = new SparkParticle(
                    pos,
                    -Projectile.velocity.SafeNormalize(Vector2.UnitY) * 1.5f,
                    false,
                    20,
                    1.2f,
                    Color.Yellow
                );
                GeneralParticleHandler.SpawnParticle(spark);
            }

        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float level = MathHelper.Clamp(Projectile.localAI[0], 0f, 1f);
            float multiplier = MathHelper.Lerp(0.5f, 2.5f, level);
            modifiers.SourceDamage *= multiplier;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Crumbling>(), 300);
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    target.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<TheBallistaReSiegeCrater>(),
                    Math.Max(1, (int)(Projectile.damage * (0.35f + Projectile.localAI[0] * 0.25f))),
                    Projectile.knockBack * 0.35f,
                    Projectile.owner);
            }

            for (int i = 0; i < 20; i++)
            {
                Vector2 dustPosition = target.Center + Main.rand.NextVector2Circular(16f, 16f);
                Vector2 dustVelocity = new Vector2(0, -Main.rand.NextFloat(1f, 3f));
                Dust.NewDustPerfect(dustPosition, DustID.Dirt, dustVelocity, 0, default, Main.rand.NextFloat(1f, 1.5f));
            }

            BowChangeVFX.SpawnImpact(Projectile, BowChangeTheme.Ballista, 0.9f + Projectile.localAI[0] * 0.5f);
        }
        public override void OnKill(int timeLeft)
        {
            // 屏幕震动（保留）
            float shakePower = 5f;
            float distanceFactor = Utils.GetLerpValue(1000f, 0f, Projectile.Distance(Main.LocalPlayer.Center), true);
            Main.LocalPlayer.Calamity().GeneralScreenShakePower = Math.Max(Main.LocalPlayer.Calamity().GeneralScreenShakePower, shakePower * distanceFactor);

            if (Projectile.owner == Main.myPlayer)
            {
                int shardCount = (int)MathHelper.Lerp(4f, 20f, MathHelper.Clamp(Projectile.localAI[0], 0f, 1f));

                for (int i = 0; i < shardCount; i++)
                {
                    float angle = MathHelper.TwoPi * i / shardCount + Main.rand.NextFloat(-0.1f, 0.1f); // 无序微扰
                    Vector2 vel = angle.ToRotationVector2() * Main.rand.NextFloat(6f, 10f);

                    Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        Projectile.Center,
                        vel,
                        ModContent.ProjectileType<FossilShard>(),
                        Projectile.damage / 4, // 固定伤害比例
                        Projectile.knockBack * 0.5f,
                        Projectile.owner
                    );
                }
            }

            {
                // === 天降破城弩箭 ===
                if (Projectile.owner == Main.myPlayer)
                {
                    Vector2 playerCenter = Main.player[Projectile.owner].Center;
                    Vector2 projCenter = Projectile.Center;

                    // 计算当前水平坐标中心
                    float screenMidX = (playerCenter.X + projCenter.X) * 0.5f;
                    float shiftX = (playerCenter.X - screenMidX) * 0.25f;
                    float finalX = screenMidX + shiftX;

                    // 设定初始位置（从空中落下）
                    Vector2 spawnPos = new Vector2(finalX, projCenter.Y - 70f * 16f);
                    Vector2 shootDir = (projCenter - spawnPos).SafeNormalize(Vector2.UnitY);

                    int super = Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        spawnPos,
                        shootDir * 15f,
                        ModContent.ProjectileType<TheBallistaReSuperPROJ>(),
                        (int)(Projectile.damage * 3f),
                        Projectile.knockBack * 2f,
                        Projectile.owner
                    );

                    if (Main.projectile.IndexInRange(super))
                    {
                        Projectile p = Main.projectile[super];
                        p.scale = 3f;
                        p.rotation = shootDir.ToRotation() + MathHelper.PiOver2;
                    }
                }

            }

            // 宏伟爆破粒子特效（光环 + 电花）
            for (int i = 0; i < 44; i++)
            {
                float angle = MathHelper.TwoPi * i / 24f;
                Vector2 vel = angle.ToRotationVector2() * Main.rand.NextFloat(4f, 8f);
                CritSpark spark = new CritSpark(Projectile.Center, vel, Color.OrangeRed, Color.Yellow, 1.5f, 25);
                GeneralParticleHandler.SpawnParticle(spark);
            }

            for (int i = 0; i < 160; i++)
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Dirt, Main.rand.NextVector2Circular(8f, 8f));
                d.scale = Main.rand.NextFloat(1.2f, 1.8f);
                d.noGravity = true;
            }




        }





    }
}
