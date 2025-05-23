﻿using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.NPCs.OldDuke;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using CalamityRangerExpansion.Content.Arrows.DPreDog.EffulgentFeatherArrow;
using CalamityRangerExpansion.Content.Arrows.DPreDog.DivineGeodeArrow;
using Terraria.GameContent;
using CalamityMod;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.CalPlayer.DrawLayers;
using Terraria.DataStructures;

namespace CalamityRangerExpansion
{
    public class ChangeWeaponDamage : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {

            // 检查是否为 神明吞噬者
            if ((npc.type == ModContent.NPCType<DevourerofGodsHead>() || npc.type == ModContent.NPCType<DevourerofGodsBody>() || npc.type == ModContent.NPCType<DevourerofGodsTail>()))
            {
                // 检查弹幕类型是否为 闪耀金羽箭 以及它的电场
                if (projectile.type == ModContent.ProjectileType<EffulgentFeatherArrowAura>() ||
                    projectile.type == ModContent.ProjectileType<EffulgentFeatherArrowPROJ>())
                {
                    modifiers.SourceDamage *= 0.85f;
                }
            }

            // 检查是否为 神明吞噬者 （仅身体）
            if ((npc.type == ModContent.NPCType<DevourerofGodsBody>()))
            {
                // 检查弹幕类型是否为 神圣晶石箭 以及它的爆炸
                if (projectile.type == ModContent.ProjectileType<DivineGeodeArrowPROJ>() ||
                projectile.type == ModContent.ProjectileType<DivineGeodeArrowEXP>())
                {
                    modifiers.SourceDamage *= 5f;
                }
            }

            base.ModifyHitByProjectile(npc, projectile, ref modifiers);
        }

    }

    public class ArterialAssaultFix : GlobalProjectile
    {
        //public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        //{
        //    Player player = Main.LocalPlayer;
        //    if (ModContent.ItemType<ArterialAssault>() == player.HeldItem.type)
        //    {
        //        projectile.damage = 0;
        //    }
        //}

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            Player player = Main.LocalPlayer;
            if (ModContent.ItemType<ArterialAssault>() == player.HeldItem.type && source is EntitySource_ItemUse_WithAmmo)
            {
                projectile.damage = 0;
            }
        }
    }
}
