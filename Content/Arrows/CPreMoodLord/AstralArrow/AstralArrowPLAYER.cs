//using Microsoft.Xna.Framework;
//using Terraria;
//using Terraria.ModLoader;

//namespace CalamityRangerExpansion.Content.Arrows.CPreMoodLord.AstralArrow
//{
//    public class AstralArrowPLAYER : ModPlayer
//    {
//        private bool hasSizeIncreased = false; // 标记是否已经放大过

//        public override void PostUpdate()
//        {
//            // 检查玩家是否有 AstralArrowPBuff
//            if (Player.HasBuff(ModContent.BuffType<AstralArrowPBuff>()) && !hasSizeIncreased)
//            {
//                // 放大倍数
//                float scaleFactor = 1.5f;

//                // 使用 Size 属性调整玩家的碰撞箱大小
//                Player.Size *= scaleFactor;

//                // 可选：调整玩家位置以避免视觉偏移
//                Player.position -= new Vector2(Player.width / 2f, Player.height / 2f) * (scaleFactor - 1f);

//                // 标记为已放大
//                hasSizeIncreased = true;
//            }
//            // 还原碰撞箱大小（如果需要在Buff移除时恢复）
//            else if (!Player.HasBuff(ModContent.BuffType<AstralArrowPBuff>()) && hasSizeIncreased)
//            {
//                // 还原倍数
//                float restoreFactor = 1 / 1.5f;

//                // 使用 Size 属性恢复玩家的碰撞箱大小
//                Player.Size *= restoreFactor;

//                // 可选：调整玩家位置以避免视觉偏移
//                Player.position += new Vector2(Player.width / 2f, Player.height / 2f) * (1.5f - 1f);

//                // 重置标记
//                hasSizeIncreased = false;
//            }
//        }

//    }
//}
