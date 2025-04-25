//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Terraria.ModLoader;
//using Terraria;

//namespace CalamityRangerExpansion.Content.Gel.BPrePlantera.CryonicGel
//{
//    internal class CryonicGelEDebuff : ModBuff, ILocalizedModType
//    {
//        public new string LocalizationCategory => "Buffs";
//        public override void SetStaticDefaults()
//        {
//            Main.debuff[Type] = true; // 标记为减益效果
//        }

//        public override void Update(NPC npc, ref int buffIndex)
//        {
//            npc.defense -= 5; // 减少 5 防御力
//        }
//    }
//}
