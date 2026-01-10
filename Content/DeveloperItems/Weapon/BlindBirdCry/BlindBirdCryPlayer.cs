namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.BlindBirdCry
{
    public class BlindBirdCryPlayer : ModPlayer
    {
        private float whipTagMultiplier = 1f; // 初始为 1，无额外加成

        // 设置鞭子 Tag 值
        public void SetWhipTagMultiplier(float value)
        {
            whipTagMultiplier = 1f + (value / 100f); // 将百分比转换为倍率
        }

        public override void PostUpdate()
        {
            // 检查玩家是否手持 BlindBirdCry 武器
            if (Player.HeldItem.type == ModContent.ItemType<BlindBirdCry>())
            {
                // 首先设置远程暴击率伤害为0
                Player.GetDamage(DamageClass.Ranged) = new Terraria.ModLoader.StatModifier(0f, 1f, 0f, 1f);
                Player.GetCritChance(DamageClass.Ranged) = 0;

                // 新将 Rogue 伤害重置为 0
                Player.GetDamage(ModContent.GetInstance<RogueDamageClass>()) = new StatModifier(0f, 1f, 0f, 0f);

                // 保持 Stealth 为最满状态
                Player.Calamity().stealthAcceleration = 100f; // 设置一个非常高的加速倍率

                // 将鞭子的 Tag 值作为远程伤害的乘算加成
                Player.GetDamage(DamageClass.Ranged) *= whipTagMultiplier;

                var meleeDamage = Player.GetDamage(DamageClass.Melee);
                var rangerDamage = Player.GetDamage(DamageClass.Ranged);

                float additive = rangerDamage.Additive + meleeDamage.Additive;
                float multiplicative = rangerDamage.Multiplicative * meleeDamage.Multiplicative;
                float baseValue = rangerDamage.Base + meleeDamage.Base;

                Player.GetDamage(DamageClass.Ranged) = new Terraria.ModLoader.StatModifier(additive, multiplicative, baseValue, rangerDamage.Flat);

                int MACrit = (int)Player.GetCritChance(DamageClass.Magic);
                Player.GetCritChance(DamageClass.Ranged) += MACrit;
            }
            else
            {
                // 如果玩家未手持 BlindBirdCry，重置倍率
                whipTagMultiplier = 1f;
            }
        }
    }
}