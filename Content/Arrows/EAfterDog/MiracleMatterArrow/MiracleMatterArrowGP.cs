//using CalamityMod.Items.Weapons.Ranged;
//using CalamityMod;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Terraria;
//using Terraria.ModLoader;

//namespace CalamityRangerExpansion.Content.Arrows.EAfterDog.MiracleMatterArrow
//{
//    public class MiracleMatterArrowGP : GlobalProjectile
//    {
//        public override bool InstancePerEntity => true;

//        // 标志此弹幕是否被附魔
//        public bool IsMiracleMatterInfused = false;

//        /// <summary>
//        /// 修改弹幕击中 NPC 时的伤害，仅当特定条件满足时降低伤害。
//        /// </summary>
//        /// <param name="projectile">当前弹幕</param>
//        /// <param name="target">被击中的 NPC</param>
//        /// <param name="modifiers">修改伤害的参数</param>
//        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
//        {
//            Player player = Main.player[projectile.owner];

//            // 条件判断：
//            // 1. 弹幕被附魔（通过 OnConsumedAsAmmo 标记）
//            // 2. 玩家正在使用 HeavenlyGale 武器
//            // 3. Boss Calamitas 尚未被击败
//            if (IsMiracleMatterInfused &&
//                player.HeldItem.type == ModContent.ItemType<HeavenlyGale>() &&
//                !DownedBossSystem.downedCalamitas)
//            {
//                // 将伤害降低到 25%
//                modifiers.FinalDamage *= 0.25f;
//            }
//        }
//    }
//}
