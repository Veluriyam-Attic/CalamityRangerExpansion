namespace CalamityRangerExpansion.Content.Arrows.DPreDog.PolterplasmArrow
{
    public class PolterplasmArrowEDeBuff : ModBuff, ILocalizedModType
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public new string LocalizationCategory => "Buffs";
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true; // 减益
        }
        // 保存敌人原始防御力
        private Dictionary<int, int> originalDefense = new Dictionary<int, int>();


        public override void Update(NPC npc, ref int buffIndex)
        {
            // 如果敌人是有效目标
            if (!npc.friendly && npc.lifeMax > 5 && !npc.boss)
            {
                // 检查并记录原始防御力
                if (!originalDefense.ContainsKey(npc.whoAmI))
                {
                    originalDefense[npc.whoAmI] = npc.defense; // 保存敌人原防御力
                }

                // 应用效果：防止移动、提高防御力
                npc.velocity = Vector2.Zero; // 禁止移动
                npc.aiStyle = -1; // 禁用 AI

                // 限制 Buff 时间为 1 秒
                npc.buffTime[buffIndex] = Math.Min(npc.buffTime[buffIndex], 60); // 1 秒 = 60 帧
            }

            // 检查 Buff 是否结束
            if (npc.buffTime[buffIndex] <= 0)
            {
                if (originalDefense.ContainsKey(npc.whoAmI))
                {
                    // 恢复敌人原始防御力和行为
                    npc.defense = originalDefense[npc.whoAmI]; // 恢复防御力
                    originalDefense.Remove(npc.whoAmI); // 移除记录
                }

                npc.aiStyle = npc.ModNPC?.AIType ?? npc.aiStyle; // 恢复 AI 行为
            }
        }

        public override void Unload()
        {
            // 确保卸载时清除防御力记录，避免内存泄漏
            originalDefense.Clear();
        }
    }
}

