using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using CalamityMod.Particles;
using CalamityRangerExpansion.CREConfigs;

namespace CalamityRangerExpansion.Content.Ammunition.DPreDog.EffulgentFeatherBullet
{
    internal class EffulgentFeatherBulletPBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true; // Buff 不会保存
        }

        public override void Update(Player player, ref int buffIndex)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                if (player.buffTime[buffIndex] % 10 == 0) // 每 10 帧触发一次
                {
                    int particleCount = Main.rand.Next(2, 5); // 随机生成 2~4 个粒子
                    for (int i = 0; i < particleCount; i++)
                    {
                        Vector2 particleVelocity = Main.rand.NextVector2Circular(3f, 3f); // 随机方向
                        float randomScale = Main.rand.NextFloat(1.1f, 1.5f); // 随机大小
                        Particle bolt = new CrackParticle(
                            player.Center,
                            particleVelocity,
                            Color.Aqua * 0.65f,
                            Vector2.One * randomScale,
                            0,
                            0,
                            randomScale,
                            11
                        );
                        GeneralParticleHandler.SpawnParticle(bolt);
                    }
                }
            }
        }
    }
}
