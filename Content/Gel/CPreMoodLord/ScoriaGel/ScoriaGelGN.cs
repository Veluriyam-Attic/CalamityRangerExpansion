using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System;

namespace CalamityRangerExpansion.Content.Gel.CPreMoodLord.ScoriaGel
{
    public class ScoriaGelGN : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool IsMarkedByScoriaGel = false;
        public int MarkDuration = 0;

        public override void AI(NPC npc)
        {
            if (IsMarkedByScoriaGel)
            {
                MarkDuration--;
                if (MarkDuration <= 0)
                {
                    IsMarkedByScoriaGel = false;
                }
            }
        }

        public override void OnKill(NPC npc)
        {
            if (IsMarkedByScoriaGel)
            {
                // 平均释放 5 个 ScoriaGelFireBall
                for (int i = 0; i < 5; i++)
                {
                    float angle = MathHelper.ToRadians(360f / 5 * i);
                    Vector2 spawnVelocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 10f;

                    Projectile.NewProjectile(
                        npc.GetSource_FromThis(),
                        npc.Center,
                        spawnVelocity,
                        ModContent.ProjectileType<ScoriaGelFireBall>(),
                        npc.damage * 4, // 伤害为敌人伤害的 4 倍
                        0f,
                        Main.myPlayer
                    );
                }
            }
        }
    }
}
