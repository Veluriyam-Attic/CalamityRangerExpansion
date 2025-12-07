//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Terraria.ID;
//using Terraria.ModLoader;
//using CalamityMod;

//namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.DiffuseNovaArc
//{
//    public class DNAShimmerTransforms : GlobalItem
//    {
//        public override void SetStaticDefaults()
//        {
//            // 检查 ArcNovaDiffuser 是否存在于游戏中
//            if (ModLoader.HasMod("CalamityMod"))
//            {
//                // 获取 ArcNovaDiffuser 的 ItemType
//                int arcNovaDiffuserType = ModContent.ItemType<CalamityMod.Items.Weapons.Ranged.ArcNovaDiffuser>();

//                // 动态修改 ArcNovaDiffuser 的 Shimmer 转化目标为 DiffuseNovaArc
//                ItemID.Sets.ShimmerTransformToItem[arcNovaDiffuserType] = ModContent.ItemType<DiffuseNovaArc>();
//            }
//        }
//    }
//}