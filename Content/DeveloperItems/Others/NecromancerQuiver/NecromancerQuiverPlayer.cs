//using CalamityMod.Projectiles.Boss;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Terraria.ModLoader;
//using Terraria;
//using Microsoft.Xna.Framework;

//namespace CalamityRangerExpansion.Content.DeveloperItems.Others.NecromancerQuiver
//{
//    internal class NecromancerQuiverPlayer : ModPlayer
//    {
//        public bool hasNecromancerQuiver = false;

//        public override void ResetEffects()
//        {
//            hasNecromancerQuiver = false;
//        }

//        public override void PostUpdate()
//        {
//            if (hasNecromancerQuiver)
//            {
//                // Check if the unique NecromancerQuiverPROJONLYONE projectile exists
//                bool projectileExists = false;
//                for (int i = 0; i < Main.maxProjectiles; i++)
//                {
//                    Projectile proj = Main.projectile[i];
//                    if (proj.active && proj.owner == Player.whoAmI &&
//                        proj.type == ModContent.ProjectileType<NecromancerQuiverPROJONLYONE>())
//                    {
//                        projectileExists = true;
//                        break;
//                    }
//                }

//                // If the projectile doesn't exist, summon it
//                if (!projectileExists)
//                {
//                    Vector2 spawnPosition = Player.Center + Player.direction * new Vector2(30 * 16, 0); // 30 tiles in player's facing direction
//                    Projectile.NewProjectile(Player.GetSource_Accessory(Player.HeldItem), spawnPosition, Vector2.Zero,
//                        ModContent.ProjectileType<NecromancerQuiverPROJONLYONE>(), 50, 2f, Player.whoAmI);
//                }
//            }
//            else
//            {
//                // Remove the projectile if the accessory is no longer active
//                for (int i = 0; i < Main.maxProjectiles; i++)
//                {
//                    Projectile proj = Main.projectile[i];
//                    if (proj.active && proj.owner == Player.whoAmI &&
//                        proj.type == ModContent.ProjectileType<NecromancerQuiverPROJONLYONE>())
//                    {
//                        proj.Kill();
//                    }
//                }
//            }
//        }
//    }
//}
