using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;

namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.HD2.R36
{
    internal class R36ProjChange : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool IsR36Shard = false;
        public int DelayTimer = 0;

        public override void AI(Projectile projectile)
        {
            if (!IsR36Shard)
                return;

            DelayTimer++;
        }

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (!IsR36Shard)
                return;

            //if (DelayTimer < 5)
            //    modifiers.FinalDamage *= 0f; // 前5帧禁止伤害
        }
        public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers)
        {
            if (!IsR36Shard)
                return;

            // 🔥击中玩家只造成15%伤害
            modifiers.FinalDamage *= 0.03f;
        }



    }
}