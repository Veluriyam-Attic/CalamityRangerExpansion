using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System;

namespace CalamityRangerExpansion.Content.BOWChange.BPrePlantera.CorrodedCaustibowC
{
    internal class CorrodedCaustibowReEffect : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public int AttackType = -1;

        private bool bTriggerUsed = false;
        private Vector2 BOrigin = Vector2.Zero;

        public override void AI(Projectile projectile)
        {
            if (AttackType == -1)
                return;

            float angle = projectile.timeLeft * 0.4f;
            Vector2 offset = new Vector2(0f, 8f).RotatedBy(angle);
            Dust dust = Dust.NewDustDirect(projectile.Center + offset, 0, 0, DustID.Demonite);
            dust.noGravity = true;
            dust.velocity = offset.RotatedBy(MathHelper.PiOver2) * 0.2f;

            if (AttackType == 1 && !bTriggerUsed)
            {
                if (BOrigin == Vector2.Zero)
                    BOrigin = projectile.Center;

                float distance = Vector2.Distance(BOrigin, projectile.Center);
                if (distance >= 16f * 16f)
                {
                    bTriggerUsed = true;

                    Projectile.NewProjectile(
                        projectile.GetSource_FromThis(),
                        projectile.Center,
                        Vector2.Zero,
                        ModContent.ProjectileType<CorrodedCaustFireWall>(),
                        (int)(projectile.damage * 0.6f),
                        0f,
                        projectile.owner
                    );
                }
            }
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (AttackType == 0)
            {
                Vector2 spawnOffset = new Vector2(Main.rand.NextFloat(-320, 320), Main.rand.NextFloat(-320, 320));
                Vector2 spawnPos = target.Center + spawnOffset;

                Vector2 toTarget = (target.Center - spawnPos).SafeNormalize(Vector2.UnitY) * 12f;

                Projectile.NewProjectile(
                    projectile.GetSource_FromThis(),
                    spawnPos,
                    toTarget,
                    ModContent.ProjectileType<CorrodedCaustibowReASpark>(),
                    (int)(projectile.damage * 0.75f),
                    0f,
                    projectile.owner
                );
            }
        }






    }
}
