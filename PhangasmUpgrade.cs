//using CalamityMod.Items.Weapons.Ranged;
//using CalamityMod;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Terraria;
//using Terraria.ModLoader;

//namespace CalamityRangerExpansion
//{
//    public class PhangasmUpgrade : ModPlayer
//    {
//        public override void PostUpdate()
//        {
//            // 检查玩家当前持有的武器是否是 Phangasm
//            if (Player.HeldItem.type == ModContent.ItemType<Phangasm>())
//            {
//                // 初始化远程伤害百分比加成
//                float percentBonus = 0f;

//                // 如果击败了 Yharon，增加 30% 的远程伤害
//                if (DownedBossSystem.downedYharon)
//                {
//                    percentBonus += 0.3f; // 30% 增伤
//                }

//                // 如果击败了 Calamitas 或 ExoMechs，增加 40% 的远程伤害
//                if (DownedBossSystem.downedCalamitas || DownedBossSystem.downedExoMechs)
//                {
//                    percentBonus += 0.4f; // 40% 增伤
//                }

//                // 如果 Calamitas 和 ExoMechs 都被击败，增加额外的 130% 远程伤害
//                if (DownedBossSystem.downedCalamitas && DownedBossSystem.downedExoMechs)
//                {
//                    percentBonus += 1.3f; // 130% 增伤
//                }

//                // 应用伤害加成
//                Player.GetDamage(DamageClass.Ranged) *= (1 + percentBonus);
//            }
//        }
//    }
//}
