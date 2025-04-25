using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CalamityRangerExpansion.Content.Ammunition.CPreMoodLord.PerennialBullet 
{ 
    public class PerennialBulletPBuff : ModBuff, ILocalizedModType
    {
        public new string LocalizationCategory => "Buffs";
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true; // 不保存 Buff
            Main.debuff[Type] = false;   // Buff 是增益效果
        }

        public override void Update(Player player, ref int buffIndex)
        {
            //int stackCount = player.GetModPlayer<PerennialBulletPlayer>().StackCount; // 获取堆叠层数
            //int maxDefenseBoost = Main.getGoodWorld ? 5000 : 20; // 根据条件动态调整防御力加成上限
            //int defenseBoost = Math.Min(stackCount, maxDefenseBoost); // 防御力加成不超过设定的上限
            //player.statDefense += defenseBoost; // 根据堆叠层数动态增加防御力

            // 获取当前堆叠层数
            int stackCount = player.GetModPlayer<PerennialBulletPlayer>().StackCount;

            // 计算最大提升值
            int maxHealthBoostPerLevel = Main.getGoodWorld ? 100 : 10; // 每级增加的最大生命值
            int maxBoostLevel = 10; // 最大等级为 10
            int totalHealthBoost = Math.Min(stackCount, maxBoostLevel) * maxHealthBoostPerLevel; // 计算总生命值提升

            // 动态增加玩家的最大生命值
            player.statLifeMax2 += totalHealthBoost;
        }
    }
}
