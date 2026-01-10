
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.Glock17
{
    internal class Glock17EDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // 不显示图标、不在Buff栏出现，仅用于后台处理
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.NurseCannotRemoveDebuff[Type] = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            // 防御减半（保证不低于0）
            npc.defense = Math.Max(0, npc.defense / 2);

            // 持续散射银白粒子特效
            if (Main.rand.NextBool(2)) // 每帧约50%概率生成
            {
                Vector2 spawnPos = npc.Hitbox.Top() + new Vector2(Main.rand.NextFloat(npc.width), 0f);
                Vector2 vel = Main.rand.NextVector2Circular(1f, 1f);

                // 可选 spark 粒子类型也可混合
                var p = new CritSpark(spawnPos, vel, Color.White, Color.LightGray, 0.4f, 10);
                GeneralParticleHandler.SpawnParticle(p);
            }
        }
    }
}