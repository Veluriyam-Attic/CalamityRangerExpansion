using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ID;

namespace CalamityRangerExpansion.Content.BOWChange.EAfterDog.PhangasmC
{
    public class PhangasmRePlayer : ModPlayer
    {
        public override void PostUpdate()
        {
            Player player = Main.LocalPlayer;

            bool usingWeapon = player.HeldItem.type == ModContent.ItemType<PhangasmRe>();
            bool ufoExists = false;

            // 检查是否已有 UFO 弹幕
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.owner == player.whoAmI && proj.type == ModContent.ProjectileType<PhangasmReUFO>())
                {
                    ufoExists = true;

                    if (!usingWeapon)
                        proj.Kill(); // 没拿武器就删掉

                    break;
                }
            }

            // 没有 UFO 且正在使用武器，生成 UFO 弹幕
            if (!ufoExists && usingWeapon)
            {
                Vector2 spawnPos = player.Center - new Vector2(0f, 35f * 16f);
                Projectile.NewProjectile(
                    player.GetSource_Misc("PhangasmUFO"),
                    spawnPos,
                    Vector2.Zero,
                    ModContent.ProjectileType<PhangasmReUFO>(),
                    0,
                    0f,
                    player.whoAmI
                );
                SoundEngine.PlaySound(SoundID.Item117 with { Volume = 0.8f, Pitch = -0.1f }, spawnPos);

            }
        }





    }
}
