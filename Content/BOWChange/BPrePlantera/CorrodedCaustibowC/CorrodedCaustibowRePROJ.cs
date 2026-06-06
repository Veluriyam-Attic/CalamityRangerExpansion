using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;
using Terraria.DataStructures;
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.BPrePlantera.CorrodedCaustibowC
{
    public class CorrodedCaustibowRePROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.BPrePlantera";

        private int stage = 0;
        private int frameCounter = 0;

        public override void SetDefaults()
        {
            Projectile.arrow = true; // 他一定是箭，所以这句话一定要加
            Projectile.width = Projectile.height = 10; // 弹幕宽高
            Projectile.friendly = true; // 当然他是我方弹幕
            Projectile.DamageType = DamageClass.Ranged; // 远程伤害
            Projectile.penetrate = 4; // 可造成多少次伤害然后消失
            Projectile.extraUpdates = 1; // 每次额外更新多少次数，值越大弹幕就越快
            Projectile.timeLeft = 120; // 剩余时间
            Projectile.ignoreWater = true; // 是否无视水体的影响
            Projectile.tileCollide = true; // 是否与方块发生碰撞后消除自己
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
            Projectile.aiStyle = ProjAIStyleID.Arrow; // 让弹幕受到重力影响
        }

        public override void OnSpawn(IEntitySource source)
        {
            stage = (int)Projectile.ai[0];

            // 出生特效：绿色/紫色能量爆闪
            for (int i = 0; i < 16; i++)
            {
                Vector2 offset = Vector2.UnitY.RotatedBy(MathHelper.TwoPi * i / 16f) * 12f;
                Dust d = Dust.NewDustPerfect(Projectile.Center + offset, DustID.Demonite,
                    offset.SafeNormalize(Vector2.Zero) * 1.2f,
                    120,
                    Main.rand.NextBool() ? Color.LimeGreen : Color.Purple,
                    1.2f);
                d.noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
        }

        public override void AI()
        {
            frameCounter++;

            // 飞行特效：毒雾轨迹
            if (Main.rand.NextBool(2))
            {
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Poisoned,
                    Projectile.velocity.RotatedByRandom(0.3f) * -0.2f,
                    120,
                    Color.Lerp(Color.LimeGreen, Color.Purple, Main.rand.NextFloat(0.3f, 0.7f)),
                    Main.rand.NextFloat(0.9f, 1.3f));
                d.noGravity = true;
            }
            BowChangeVFX.SpawnTrail(Projectile, BowChangeTheme.Caustic, 0.35f + stage * 0.15f);

            // 第二阶段：每 5 帧生成一枚 ASpark
            if (stage >= 2 && frameCounter % 5 == 0)
            {
                Vector2 spawnPos = Projectile.Center + new Vector2(0f, -12f);
                Vector2 vel = Main.rand.NextVector2Circular(2f, 2f) + new Vector2(0f, -4f);
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    spawnPos,
                    vel,
                    ModContent.ProjectileType<CorrodedCaustibowReASpark>(),
                    (int)(Projectile.damage * 0.6f),
                    Projectile.knockBack * 0.5f,
                    Projectile.owner
                );
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            BowChangeVFX.SpawnImpact(Projectile, BowChangeTheme.Caustic, 0.8f + stage * 0.2f);
            // 第一阶段：生成腐蚀火墙
            if (stage >= 1)
            {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    target.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<CorrodedCaustFireWall>(),
                    (int)(Projectile.damage * 0.8f),
                    0f,
                    Projectile.owner
                );

                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    target.Center,
                    Vector2.Zero,
                    ModContent.ProjectileType<CorrodedCaustibowReAcidBloom>(),
                    Math.Max(1, (int)(Projectile.damage * (0.35f + stage * 0.08f))),
                    0f,
                    Projectile.owner);
            }

            // 播放击中音效
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
        }

        public override void OnKill(int timeLeft)
        {
            // 第三阶段：死亡时爆炸出 5 发 C 弹幕
            if (stage >= 3)
            {
                for (int i = 0; i < 5; i++)
                {
                    Vector2 vel = (MathHelper.TwoPi * i / 5f).ToRotationVector2() * 6f;
                    Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        Projectile.Center,
                        vel,
                        ModContent.ProjectileType<CorrodedCaustibowReC>(),
                        (int)(Projectile.damage * 0.7f),
                        Projectile.knockBack,
                        Projectile.owner
                    );
                }
            }

            // 死亡爆散特效
            for (int i = 0; i < 12; i++)
            {
                Dust boom = Dust.NewDustPerfect(
                    Projectile.Center + Main.rand.NextVector2Circular(6f, 6f),
                    DustID.GreenTorch,
                    Main.rand.NextVector2Circular(3f, 3f),
                    120,
                    Main.rand.NextBool() ? Color.LimeGreen : Color.Purple,
                    1.3f
                );
                boom.noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
        }
    }
}
