using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace CalamityRangerExpansion.Content.Gel.CPreMoodLord.PlagueGel
{
    internal class PlagueGelEDebuff : ModBuff, ILocalizedModType
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public new string LocalizationCategory => "Buffs";
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true; // This is a debuff
            Main.buffNoSave[Type] = true; // Does not persist on logout
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            // 每秒减少800点生命值
            if (npc.lifeRegen > 0)
            {
                npc.lifeRegen = 0; // 防止自然回血
            }
            npc.lifeRegen -= 500; // 每秒损失500生命

            #region 旧代码
            //// 每帧释放一个绿色线性粒子
            //Vector2 trailPos = npc.Center + new Vector2(Main.rand.NextFloat(-npc.width / 2f, npc.width / 2f), npc.height / 2f);
            //Particle trail = new SparkParticle(
            //    trailPos,
            //    new Vector2(0, 1f), // 线性向下的粒子
            //    false,
            //    60,
            //    Main.rand.NextFloat(0.5f, 1f), // 粒子随机缩放
            //    Color.Lime
            //);
            //GeneralParticleHandler.SpawnParticle(trail);
            #endregion

            // 每帧在敌人中心释放一个绿色相关的 Dust
            if (Main.rand.NextBool()) // 随机决定是否生成 Dust
            {
                Dust dust = Dust.NewDustDirect(
                    npc.Center,            // 从敌人中心生成
                    0,                     // Dust 的宽度（0 表示一个点）
                    0,                     // Dust 的高度（0 表示一个点）
                    DustID.Grass,          // 绿色相关的 Dust ID
                    Main.rand.NextFloat(-3f, 3f), // 随机 X 方向速度
                    Main.rand.NextFloat(-3f, 3f), // 随机 Y 方向速度
                    0,                     // 不添加额外透明度
                    default,               // 使用默认颜色
                    Main.rand.NextFloat(1f, 1.5f) // 随机 Dust 的缩放大小
                );

                dust.noGravity = true; // 无重力效果
            }

        }
    }
}
