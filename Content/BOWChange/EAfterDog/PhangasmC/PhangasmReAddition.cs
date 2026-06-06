//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Terraria.ID;
//using Terraria.ModLoader;
//using Terraria;
//using Microsoft.Xna.Framework;
//using CalamityRangerExpansion.Content.BOWChange.APreHardMode.GaleforceC;

//namespace CalamityRangerExpansion.Content.BOWChange.EAfterDog.PhangasmC
//{
//    internal class PhangasmReAddition : GlobalProjectile
//    {
//        public override bool InstancePerEntity => true;

//        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
//        {
//            // 检查是否是我方弹幕且为射手类伤害类型且伤害大于0
//            if (projectile.owner == Main.myPlayer
//                && projectile.DamageType == DamageClass.Ranged
//                && projectile.type != ModContent.ProjectileType<PhangasmReGOEST>()) // 如果弹幕是 PhangasmReGOEST，不触发生成逻辑
//            {
//                Player player = Main.player[projectile.owner];

//                // 检查玩家是否手持 PhangasmRe 武器
//                if (player.HeldItem.ModItem is ModItem modItem && modItem.Name == "PhangasmRe")
//                {
//                    // 计算伤害为原始伤害的 25%
//                    int newDamage = (int)(damageDone * 0.25f);

//                    for (int i = 0; i < 3; i++) // 每次随机发射三发弹幕
//                    {
//                        // 计算新的弹幕位置
//                        Vector2 spawnPosition = player.Center + new Vector2(Main.rand.Next(-20, 21), Main.rand.Next(-20, 21));

//                        // 计算新的弹幕方向，指向目标NPC
//                        Vector2 shootDirection = Vector2.Normalize(target.Center - spawnPosition) + new Vector2(Main.rand.NextFloat(-0.1f, 0.1f), Main.rand.NextFloat(-0.1f, 0.1f));

//                        // 发射自定义弹幕 PhangasmReGOEST
//                        Projectile.NewProjectile(
//                            projectile.GetSource_FromThis(),
//                            spawnPosition,
//                            shootDirection * 12f, // 设置弹幕速度
//                            ModContent.ProjectileType<PhangasmReGOEST>(),
//                            newDamage,
//                            0f, // 无击退
//                            player.whoAmI
//                        );
//                    }
//                }
//            }
//        }
//    }
//}
