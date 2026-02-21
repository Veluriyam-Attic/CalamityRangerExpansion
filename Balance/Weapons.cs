namespace CalamityRangerExpansion.Balance
{
    internal static class Weapons
    {
        /// <summary>
        /// 关于虞金引焱的一些数值
        /// </summary>
        internal static class TheGoldenFire
        {
            internal static readonly int[] Damage =
            {
                8,
                13,
                17,
                18,
                19,
                23,
                43,
                58,
                65,
                77,
                112,
                248,
                252,
                261,
                270,
                666,
                721,
                8848,
                11451
            };

            internal static readonly float[] ShootSpeed =
            {
                3.5f,
                5.5f,
                6f,
                6.5f,
                7f,
                8f,
                8f,
                8f,
                8f,
                8f,
                10f,
                14f,
                14f,
                14f,
                14f,
                16f,
                16f,
                20f,
                20f
            };

            internal static readonly float[] KnockBack =
            {
                0.5f,
                0.6f,
                0.6f,
                0.6f,
                0.9f,
                1f,
                1f,
                1f,
                1f,
                1f,
                1f,
                1.5f,
                1.6f,
                1.7f,
                1.8f,
                1.9f,
                2f,
                2.1f,
                2.2f,
            };

            internal static readonly int[] UseTime =
            {
                5,
                5,
                5,
                5,
                4,
                2,
                2,
                2,
                2,
                2,
                1,
                1,
                1,
                1,
                1,
                1,
                1,
                1,
                1,
            };
            // 不能标记为readonly

        }

        internal static class Glock17
        {
            internal const int CoolDownSecond = 20;
            internal const int EffectTimeSecond = 10;
            internal const int StageThirdExtraDamage = 30000;
            internal const float DamageMultiplier = 1.25f;
        }
    }
}
