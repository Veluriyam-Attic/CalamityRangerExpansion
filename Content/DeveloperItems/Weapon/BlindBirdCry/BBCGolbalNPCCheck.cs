namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.BlindBirdCry
{
    internal class BBCGolbalNPCCheck : GlobalNPC
    {
        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            // 记录找到的第一个鞭子相关的 Debuff
            float whipTagValue = 0f;

            // 遍历敌人身上的 Buff
            for (int i = 0; i < NPC.maxBuffs; i++)
            {
                if (npc.buffTime[i] > 0)
                {
                    switch (npc.buffType[i])
                    {
                        case BuffID.BlandWhipEnemyDebuff: // Leather Whip
                            whipTagValue = 4f;
                            break;
                        case BuffID.ThornWhipNPCDebuff: // Snapthorn
                            whipTagValue = 6f;
                            break;
                        case BuffID.BoneWhipNPCDebuff: // Spinal Tap
                            whipTagValue = 7f;
                            break;
                        case BuffID.FlameWhipEnemyDebuff: // Firecracker
                            whipTagValue = 2.75f;
                            break;
                        case BuffID.CoolWhipNPCDebuff: // Cool Whip
                            whipTagValue = 6f;
                            break;
                        case BuffID.SwordWhipNPCDebuff: // Durendal
                            whipTagValue = 9f;
                            break;
                        case BuffID.ScytheWhipEnemyDebuff: // Dark Harvest
                            whipTagValue = 10f;
                            break;
                        case BuffID.MaceWhipNPCDebuff: // Morning Star
                            whipTagValue = 8f;
                            break;
                        case BuffID.RainbowWhipNPCDebuff: // Kaleidoscope
                            whipTagValue = 20f;
                            break;
                        default:
                            continue;
                    }

                    // 一旦找到第一个匹配的 Debuff，停止遍历
                    if (whipTagValue > 0f)
                    {
                        break;
                    }
                }
            }

            // 如果找到对应的鞭子 Tag 值，则传递给 BlindBirdCryPlayer
            if (whipTagValue > 0f)
            {
                if (Main.LocalPlayer.GetModPlayer<BlindBirdCryPlayer>() is BlindBirdCryPlayer player)
                {
                    player.SetWhipTagMultiplier(whipTagValue);
                }
            }
        }
    }
}