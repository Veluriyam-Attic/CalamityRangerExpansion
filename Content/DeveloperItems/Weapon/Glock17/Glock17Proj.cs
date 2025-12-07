using CalamityMod.Particles;
using CalamityMod.Projectiles.Typeless;
using CalamityMod;
using CalamityRangerExpansion.Content.Ammunition.CPreMoodLord.ScoriaBullet;
using CalamityRangerExpansion.CREConfigs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.Glock17
{
    internal class Glock17Proj : ModProjectile, ILocalizedModType
    {

        public new string LocalizationCategory => "DeveloperItems.Glock17";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
                return false;
            }
            return true;
        }
        // 子弹行为阶段（默认1：普通模式，2：困难，3：月后）
        public int Stage => (int)(Projectile.ai[0] == 0 ? 1 : Projectile.ai[0]);

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 4;
            Projectile.timeLeft = 300;
            Projectile.MaxUpdates = 3;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
        }

        public override void AI()
        {
            // 由于我们是水平贴图，因此什么也不需要转动
            Projectile.rotation = Projectile.velocity.ToRotation();

            // 添加光效
            Lighting.AddLight(Projectile.Center, Color.Lerp(Color.Orange, Color.Red, 0.5f).ToVector3() * 0.55f);

            // 子弹在出现之后很短一段时间会变得可见
            if (Projectile.timeLeft == 296)
                Projectile.alpha = 0;

            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                float spread = 10f + Stage * 2f; // 🔼 螺旋偏移范围扩大
                float frequency = Stage * 1.0f;  // 保留：备用频率变量（可用于动态节奏）

                // 数学曲线模拟“螺旋发光”感
                if (Main.rand.NextFloat() < 0.9f) // 🔼 基本每帧都会触发
                {
                    double angle = Projectile.ai[1] + Main.GameUpdateCount * 0.3 * Stage;
                    Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * spread;

                    Vector2 particlePos = Projectile.Center + offset.RotatedBy(Projectile.velocity.ToRotation());

                    Dust d = Dust.NewDustPerfect(particlePos, DustID.SilverFlame, Vector2.Zero);
                    d.noGravity = true;
                    d.scale = 0.9f + 0.2f * Stage; // 🔼 粒子更大更明显
                }
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
         

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 混合银光喷射特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                int count = 8 + Stage * 3;
                for (int i = 0; i < count; i++)
                {
                    Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(1f, 3f + Stage);
                    Color c = Color.Lerp(Color.White, Color.Silver, Main.rand.NextFloat());
                    PointParticle spark = new PointParticle(Projectile.Center, velocity, false, 12, 0.9f, c);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }

            // 🔥 Stage 2+：附加 Glock17EDebuff
            if (Stage >= 2)
                target.AddBuff(ModContent.BuffType<Glock17EDebuff>(), 300); // 5秒

            // 🌕 Stage 3：命中后额外从目标头顶射下一发追击弹
            if (Stage >= 3 && Main.myPlayer == Projectile.owner && target.CanBeChasedBy())
            {
                Vector2 spawnPos = target.Center - new Vector2(0, 300f); // 头顶偏上
                Vector2 velocity = Vector2.UnitY * 20f;

                int proj = Projectile.NewProjectile(
                    Projectile.GetSource_OnHit(target),
                    spawnPos,
                    velocity,
                    2422, // 你需要创建这个追击弹幕
                    Projectile.damage,
                    Projectile.knockBack,
                    Projectile.owner
                );

                if (proj.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[proj].tileCollide = false; // 穿墙
                    Main.projectile[proj].usesLocalNPCImmunity = true;
                    Main.projectile[proj].localNPCHitCooldown = 4; // 本地判定冷却
                }
            }
        }




        public override void OnKill(int timeLeft)
        {

        }
    }
}
