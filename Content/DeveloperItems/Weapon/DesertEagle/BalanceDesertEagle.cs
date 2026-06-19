namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.DesertEagle
{
    public sealed class BalanceDesertEagle
    {
        public static readonly string[] StageNames =
        {
            "Pre-Hardmode",
            "Pre-Plantera",
            "Pre-Moon Lord",
            "Post-Moon Lord"
        };

        private static readonly int[] SilverVolleyDamage =
        {
            48,
            92,
            155,
            240
        };

        private static readonly int[] LifeRoundDamage =
        {
            145,
            310,
            620,
            1180
        };

        private static readonly int[] SilverVolleyProjectileCount =
        {
            2,
            3,
            4,
            5
        };

        private static readonly float[] HoldoutSpinContactDamageMultiplier =
        {
            0.22f,
            0.27f,
            0.31f,
            0.36f
        };

        private static readonly float[] HoldoutFullChargeRoundDamageMultiplier =
        {
            10f,
            14f,
            18f,
            22f
        };

        public int GetProgressState()
        {
            if (NPC.downedMoonlord)
                return 3;

            if (NPC.downedPlantBoss)
                return 2;

            if (Main.hardMode)
                return 1;

            return 0;
        }

        public int GetSilverVolleyDamage() => GetIntValue(SilverVolleyDamage);

        public int GetLifeRoundDamage() => GetIntValue(LifeRoundDamage);

        public int GetSilverVolleyProjectileCount() => GetIntValue(SilverVolleyProjectileCount);

        public float GetHoldoutSpinContactDamageMultiplier() => GetFloatValue(HoldoutSpinContactDamageMultiplier);

        public float GetHoldoutFullChargeRoundDamageMultiplier() => GetFloatValue(HoldoutFullChargeRoundDamageMultiplier);

        private int GetIntValue(int[] values)
        {
            int index = Utils.Clamp(GetProgressState(), 0, values.Length - 1);
            return Math.Max(1, values[index]);
        }

        private float GetFloatValue(float[] values)
        {
            int index = Utils.Clamp(GetProgressState(), 0, values.Length - 1);
            return Math.Max(0.01f, values[index]);
        }
    }
}
