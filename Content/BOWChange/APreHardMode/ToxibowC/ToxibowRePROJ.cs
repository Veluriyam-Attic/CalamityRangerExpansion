using CalamityMod;
using Microsoft.Xna.Framework;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.Particles;
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.APreHardMode.ToxibowC
{
    internal class ToxibowRePROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.APreHardMode";

        private int stage = 1; // 记录阶段

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, (int)1.2f);
            return false;
        }

        public override void SetDefaults()
        {
            Projectile.arrow = true;
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1; // 初始值，OnSpawn 根据阶段调整
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 150;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
        }

        public override void OnSpawn(IEntitySource source)
        {
            stage = (int)Projectile.ai[0];
            if (stage <= 0) stage = 1;

            // 设置穿透次数
            if (stage == 1)
                Projectile.penetrate = 1;
            else if (stage == 2)
                Projectile.penetrate = 4;
            else if (stage == 3)
                Projectile.penetrate = 6;

            // 出生特效
            for (int i = 0; i < 20; i++)
            {
                Vector2 offset = Main.rand.NextVector2Circular(12f, 12f);
                Dust d = Dust.NewDustPerfect(
                    Projectile.Center + offset,
                    DustID.PoisonStaff,
                    offset.SafeNormalize(Vector2.UnitY) * Main.rand.NextFloat(1f, 3f),
                    120,
                    Color.Lerp(Color.LimeGreen, Color.YellowGreen, Main.rand.NextFloat()),
                    Main.rand.NextFloat(1f, 1.8f)
                );
                d.noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
        }

        public override void AI()
        {
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi) + MathHelper.ToRadians(90) * Projectile.direction;

            // 毒雾光效
            Lighting.AddLight(Projectile.Center, Color.GreenYellow.ToVector3() * 0.5f);

            // 飞行轨迹尘埃
            if (Main.rand.NextBool(2))
            {
                Dust trail = Dust.NewDustPerfect(Projectile.Center, DustID.Poisoned,
                    Projectile.velocity.RotatedByRandom(0.3f) * -0.2f,
                    120,
                    Color.Lerp(Color.LimeGreen, Color.YellowGreen, Main.rand.NextFloat(0.3f, 0.8f)),
                    Main.rand.NextFloat(0.8f, 1.3f));
                trail.noGravity = true;
            }
            BowChangeVFX.SpawnTrail(Projectile, BowChangeTheme.Toxic, 0.35f + stage * 0.15f);
        }

        public override bool? CanDamage() => Projectile.timeLeft < 140; // 出生10帧无敌

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            BowChangeVFX.SpawnImpact(Projectile, BowChangeTheme.Toxic, 0.7f + stage * 0.2f);
            // 命中特效
            for (int i = 0; i < 12; i++)
            {
                Dust d = Dust.NewDustPerfect(
                    Projectile.Center,
                    DustID.Poisoned,
                    Main.rand.NextVector2Circular(3f, 3f),
                    120,
                    Color.Green,
                    Main.rand.NextFloat(1f, 2f)
                );
                d.noGravity = true;
            }

            // 阶段效果
            if (stage == 2 || stage == 3)
            {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    target.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<ToxibowReEXP>(),
                    (int)(Projectile.damage * 0.8f),
                    Projectile.knockBack,
                    Projectile.owner
                );
            }

            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    target.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<ToxibowReToxinPod>(),
                    Math.Max(1, (int)(Projectile.damage * (0.28f + stage * 0.08f))),
                    0f,
                    Projectile.owner);
            }
        }

        public override void OnKill(int timeLeft)
        {
            // 烟雾特效
            for (int i = 0; i < 40; i++)
            {
                Vector2 dustVel = Main.rand.NextVector2Circular(2f, 2f);
                Color smokeColor = Main.rand.NextBool() ? Color.GreenYellow : Color.DarkOliveGreen;
                Particle smoke = new HeavySmokeParticle(
                    Projectile.Center,
                    dustVel * Main.rand.NextFloat(1f, 2.5f),
                    smokeColor,
                    20,
                    Main.rand.NextFloat(1f, 1.8f),
                    0.4f,
                    Main.rand.NextFloat(-1f, 1f),
                    true
                );
                GeneralParticleHandler.SpawnParticle(smoke);
            }

            // 阶段效果：第 3 阶段死亡时也释放 EXP
            if (stage == 3)
            {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<ToxibowReEXP>(),
                    (int)(Projectile.damage * 1.0f),
                    Projectile.knockBack,
                    Projectile.owner
                );

                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<ToxibowReToxinPod>(),
                    Math.Max(1, (int)(Projectile.damage * 0.45f)),
                    0f,
                    Projectile.owner);
            }

            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
        }
    }
}
