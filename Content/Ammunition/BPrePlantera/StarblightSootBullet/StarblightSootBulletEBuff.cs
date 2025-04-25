//using Microsoft.Xna.Framework;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Terraria;
//using Terraria.ModLoader;

//namespace CalamityRangerExpansion.Content.Ammunition.BPrePlantera.StarblightSootBullet
//{
//    public class StarblightSootBulletEBuff : ModBuff
//    {
//        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

//        public override void SetStaticDefaults()
//        {
//            Main.debuff[Type] = true; // 是一个负面状态
//            Main.buffNoTimeDisplay[Type] = false;
//        }

//        public override void Update(NPC npc, ref int buffIndex)
//        {
//            npc.damage = (int)(npc.damage * 0.7);
//        }

//    }
//}
