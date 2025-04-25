using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace CalamityThrowingSpear.Weapons.NewWeapons.BPrePlantera.TheLastLance
{
    public class PolterplasmArrowEDeBuff2 : ModBuff, ILocalizedModType
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public new string LocalizationCategory => "Buffs";
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true; // 减益
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            // 让敌人的防御力线性降低 15 点
            npc.defense -= 15;

            // 确保防御力不会降到负值以下（根据你的需求可以调整是否允许负值）
            if (npc.defense < 0)
            {
                npc.defense = 0;
            }
        }
    }
}

