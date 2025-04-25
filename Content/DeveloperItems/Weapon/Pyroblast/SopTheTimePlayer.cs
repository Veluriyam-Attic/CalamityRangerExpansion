//using Microsoft.Xna.Framework;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Terraria;
//using Terraria.ModLoader;

//namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.Pyroblast
//{
//    public class SopTheTimePlayer : ModPlayer
//    {
//        public override void UpdateAutopause()
//        {
//            base.UpdateAutopause();
//        }


//        public bool isTimePaused = false; // 全局时间暂停控制变量

//        public override void PostUpdate()
//        {
//            if (isTimePaused)
//            {
//                // 冻结所有 NPC
//                foreach (var npc in Main.npc)
//                {
//                    if (npc.active)
//                    {
//                        npc.velocity = Vector2.Zero; // 停止移动
//                        npc.aiStyle = -1; // 暂停 AI
//                    }
//                }

//                // 冻结所有弹幕
//                foreach (var proj in Main.projectile)
//                {
//                    if (proj.active && proj.owner != Main.myPlayer) // 忽略玩家自身的弹幕
//                    {
//                        proj.velocity = Vector2.Zero;
//                        proj.aiStyle = -1; // 暂停 AI
//                    }
//                }

//                // 玩家例外，可以继续正常更新
//            }
//        }



//    }
//}
