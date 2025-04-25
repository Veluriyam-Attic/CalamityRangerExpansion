using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using CalamityRangerExpansion.CREConfigs;

namespace CalamityRangerExpansion.Content.DeveloperItems.Bullet.GrapeShot
{
    public class GrapeShotPlayer : ModPlayer
    {
        private int grapeShotCounter = 0; // 计数器
        public int grapeShotX = 0; // 当前的 x 值
        private int lastAttackTime = 0; // 上次收到 GrapeShotPROJ 消息的时间计数

        public void IncrementGrapeShotCounter()
        {
            grapeShotCounter++;
            lastAttackTime = (int)(Main.GameUpdateCount); // 更新最后一次攻击时间
            if (grapeShotCounter >= 50) // 每 50 次增加 x 的值
            {
                grapeShotX++;
                grapeShotCounter = 0; // 重置计数器

                // 检查是否启用了特效
                if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
                {
                    // 当 x 增加时，在玩家头顶生成紫色粒子特效
                    for (int i = 0; i < 35; i++)
                    {
                        Vector2 dustPosition = Player.Top + new Vector2(Main.rand.NextFloat(-50, 50), -20);
                        Dust dust = Dust.NewDustPerfect(dustPosition, DustID.PurpleTorch);
                        dust.noGravity = true;
                        dust.scale = 3.5f;
                        dust.velocity = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-3f, -1f));
                    }
                }
            }
        }

        public int GetGrapeShotX()
        {
            return grapeShotX; // 返回当前 x 的值
        }

        public override void PostUpdate()
        {
            // 如果已经有1秒（60帧）没有收到攻击信息，并且x大于0，则x减1
            if (Main.GameUpdateCount - lastAttackTime > 60 && grapeShotX > 0)
            {
                grapeShotX--;
                lastAttackTime = (int)(Main.GameUpdateCount); // 重置最后一次攻击时间

                // 检查是否启用了特效
                if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
                {
                    // 当 x 减少时，在玩家下方生成紫色粒子特效
                    for (int i = 0; i < 35; i++)
                    {
                        Vector2 dustPosition = Player.Bottom + new Vector2(Main.rand.NextFloat(-50, 50), 20);
                        Dust dust = Dust.NewDustPerfect(dustPosition, DustID.PurpleTorch);
                        dust.noGravity = true;
                        dust.scale = 3.5f;
                        dust.velocity = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(1f, 3f));
                    }
                }
            }
        }
    }
}