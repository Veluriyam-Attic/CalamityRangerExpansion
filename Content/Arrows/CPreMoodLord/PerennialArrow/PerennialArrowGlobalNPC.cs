using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CalamityRangerExpansion.Content.Arrows.CPreMoodLord.PerennialArrow
{
    public class PerennialArrowGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public float damageMultiplier = 1f;

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (npc.HasBuff(ModContent.BuffType<PerennialArrowEBuff>()))
            {
                modifiers.FinalDamage *= damageMultiplier;
            }
        }
    }
}
