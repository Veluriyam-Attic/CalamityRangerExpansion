using Terraria;
using Terraria.ModLoader;

namespace CalamityRangerExpansion.Content.Ammunition.CPreMoodLord.ScoriaBullet
{
    public class ScoriaBulletPBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            int stackCount = player.GetModPlayer<ScoriaBulletPlayer>().StackCount;
            float speedBonus = stackCount * 0.03f; // 每层 +3% 速度
            float damageBonus = stackCount * 0.02f; // 每层 +2% 伤害

            player.runAcceleration += speedBonus;
            player.accRunSpeed += speedBonus;
            player.GetDamage(DamageClass.Ranged) += damageBonus;
        }
    }
}
