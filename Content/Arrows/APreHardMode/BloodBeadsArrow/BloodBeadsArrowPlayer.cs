using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityRangerExpansion.Content.Arrows.APreHardMode.BloodBeadsArrow
{
    public class BloodBeadsArrowPlayer : ModPlayer
    {
        public bool downedBoss2; // 第一个邪恶boss被打败的开关
        public bool hardMode; // 困难模式开启的开关
        public bool downedGolemBoss; // 石巨人被打败的开关

        private int cooldownTimer = 0; // 冷却时间管理，单位是帧（1秒=60帧）

        public override void ResetEffects()
        {
            // 每帧递减冷却时间
            if (cooldownTimer > 0)
            {
                cooldownTimer--;
            }
        }

        // 检查冷却时间是否结束
        public bool CanApplyDebuff()
        {
            return cooldownTimer <= 0;
        }

        // 定义 ApplyDebuffs 方法，应用debuffs到目标，并设置冷却时间
        public void ApplyDebuffs(NPC target, int duration)
        {
            // 设置冷却时间为1.25秒（75帧）
            cooldownTimer = 75;

            List<int> debuffs = new List<int>()
            {
                BuffID.OnFire,           // 着火
                BuffID.Poisoned,         // 中毒
                BuffID.Confused,         // 困惑
                BuffID.Frostburn,        // 冻伤
                BuffID.Ichor,            // 灵液
                BuffID.CursedInferno,    // 诅咒地狱火
                BuffID.ShadowFlame,      // 暗影火
                BuffID.Daybreak,         // 黎明碎片
                BuffID.BetsysCurse,      // 贝希的诅咒
                BuffID.Midas,            // 迈达斯
                BuffID.Wet,              // 湿润
                BuffID.Suffocation,      // 窒息
                BuffID.Burning,          // 燃烧
                BuffID.Bleeding,         // 流血
                BuffID.Weak,             // 虚弱
                BuffID.WitheredArmor,    // 枯萎的盔甲
                BuffID.WitheredWeapon,   // 枯萎的武器
                BuffID.Silenced,         // 沉默
                BuffID.BrokenArmor,      // 盔甲破损
                BuffID.Slimed,           // 粘液
                BuffID.Slow,             // 减速
                BuffID.Stoned,           // 石化
                BuffID.Venom,            // 剧毒
                BuffID.Dazed,            // 眩晕
                BuffID.ObsidianSkin,     // 黑曜石皮肤
                BuffID.OgreSpit          // 食人魔的唾液
            };

            // 根据击败boss情况和游戏进度添加高级debuff
            Mod calamityMod;
            if (ModLoader.TryGetMod("CalamityMod", out calamityMod))
            {
                if (Main.getGoodWorld || downedGolemBoss)
                {
                    debuffs.Add(calamityMod.Find<ModBuff>("MiracleBlight").Type);
                    debuffs.Add(calamityMod.Find<ModBuff>("VulnerabilityHex").Type);
                }

                if (hardMode)
                {
                    debuffs.Add(calamityMod.Find<ModBuff>("MarkedforDeath").Type);
                    debuffs.Add(calamityMod.Find<ModBuff>("KamiFlu").Type);
                    debuffs.Add(calamityMod.Find<ModBuff>("GlacialState").Type);
                }

                if (downedBoss2)
                {
                    debuffs.Add(calamityMod.Find<ModBuff>("HolyFlames").Type);
                    debuffs.Add(calamityMod.Find<ModBuff>("GodSlayerInferno").Type);
                }

                // 添加一般性的 debuff
                debuffs.Add(calamityMod.Find<ModBuff>("BrimstoneFlames").Type);
                debuffs.Add(calamityMod.Find<ModBuff>("CrushDepth").Type);
                debuffs.Add(calamityMod.Find<ModBuff>("RancorBurn").Type);
            }


            // 随机选择一个debuff并应用
            int randomDebuff = debuffs[Main.rand.Next(debuffs.Count)];
            target.AddBuff(randomDebuff, duration);
        }
    }
}
