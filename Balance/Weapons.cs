global using B = CalamityRangerExpansion.Balance;
using CalamityMod;
using Terraria;
namespace CalamityRangerExpansion.Balance
{
    internal static class Weapons
    {
        /// <summary>
        /// 关于虞金引焱的一些数值
        /// </summary>
        internal class TheGoldenFire
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

            internal static readonly bool[] DownNum =
            {
                NPC.downedBoss1,//克眼
                NPC.downedBoss2,//世吞/克脑
                    DownedBossSystem.downedHiveMind || //腐巢
                    DownedBossSystem.downedPerforator,//宿主
                NPC.downedBoss3,//骷髅王
                DownedBossSystem.downedSlimeGod,//史神
                Main.hardMode,//肉山
                    NPC.downedMechBoss1 && //机械三王
                    NPC.downedMechBoss2 &&
                    NPC.downedMechBoss3,
                DownedBossSystem.downedCalamitasClone,//灾影
                NPC.downedPlantBoss,//世花
                NPC.downedGolemBoss,//石巨人
                NPC.downedAncientCultist,//拜月教
                NPC.downedMoonlord,//月总
                DownedBossSystem.downedProvidence,//亵渎天神
                    DownedBossSystem.downedSignus && //西格纳斯
                    DownedBossSystem.downedStormWeaver && //风编
                    DownedBossSystem.downedCeaselessVoid,//无尽虚空
                DownedBossSystem.downedPolterghast,//幽花
                DownedBossSystem.downedDoG,//神吞
                DownedBossSystem.downedYharon,//犽戎
                    DownedBossSystem.downedExoMechs && //星流
                    DownedBossSystem.downedCalamitas,//女巫
                DownedBossSystem.downedPrimordialWyrm,//始源妖龙
                false
            };
        }
    }
}
