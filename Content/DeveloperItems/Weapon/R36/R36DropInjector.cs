using CalamityMod.Items.Accessories;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.AcidRain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.NPCs.NormalNPCs;

namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.R36
{
    internal class R36DropInjector : GlobalNPC
    {

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (npc.type == ModContent.NPCType<ArmoredDiggerHead>())
            {
                // 添加掉落：武器每次必掉（可自行调整为概率掉落）
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<R36>(), 1));
            }
        }


    }
}
