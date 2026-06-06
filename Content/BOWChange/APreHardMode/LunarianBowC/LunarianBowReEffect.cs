using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework;
using CalamityMod.Projectiles.Typeless;

namespace CalamityRangerExpansion.Content.BOWChange.APreHardMode.LunarianBowC
{
    public class LunarianBowReEffect : GlobalProjectile
    {
        // 每个弹幕独立持有该变量（确保不会冲突）
        public override bool InstancePerEntity => true;

        // 通用标记变量，-1 代表无效，0/1/2 可自由定义含义
        public int EffectType = -1;

        // 1. 每帧执行的逻辑
        public override void AI(Projectile projectile)
        {
            int type = projectile.GetGlobalProjectile<LunarianBowReEffect>().EffectType;
            if (type == -1)
                return;

            // TODO：写入按类型区分的粒子拖尾等
        }

        // 2. 与NPC碰撞时调用（用于播放击中特效、生成新弹幕等）
        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            int type = projectile.GetGlobalProjectile<LunarianBowReEffect>().EffectType;
            if (type == -1)
                return;

            // TODO：命中特效、附加状态、召唤额外弹幕等
        }

        // 3. 命中前可修改伤害倍率（比如不同类型提升倍率）
        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            int type = projectile.GetGlobalProjectile<LunarianBowReEffect>().EffectType;
            if (type == -1)
                return;

            // TODO：根据 type 修改伤害倍率
        }

        // 4. 弹幕死亡时调用（用于爆炸、光圈、清屏等）
        public override void OnKill(Projectile projectile, int timeLeft)
        {
            int type = projectile.GetGlobalProjectile<LunarianBowReEffect>().EffectType;
            if (type == -1)
                return;

            // 示例：在原位置生成一个名为 "FuckYou" 的弹幕，3倍大小
            int newProj = Projectile.NewProjectile(
                projectile.GetSource_FromThis(),
                projectile.Center,
                Vector2.Zero, // 静止不动
                ModContent.ProjectileType<FuckYou>(),
                projectile.damage,
                projectile.knockBack,
                projectile.owner
            );

            if (newProj >= 0 && newProj < Main.maxProjectiles)
            {
                Main.projectile[newProj].scale = 3f;
            }
        }


        // 5. 弹幕命中玩家时（PvP 模式下用）
        public override void OnHitPlayer(Projectile projectile, Player target, Player.HurtInfo info)
        {
            int type = projectile.GetGlobalProjectile<LunarianBowReEffect>().EffectType;
            if (type == -1)
                return;

            // TODO：附加debuff、特殊击退等
        }

        // 6. 弹幕的绘制前处理（修改颜色、绘制额外特效）
        public override bool PreDraw(Projectile projectile, ref Color lightColor)
        {
            int type = projectile.GetGlobalProjectile<LunarianBowReEffect>().EffectType;
            if (type == -1)
                return true;

            // TODO：自定义绘制或改变颜色
            return true;
        }

        // 7. 弹幕的绘制后处理（添加外轮廓、拖尾等）
        public override void PostDraw(Projectile projectile, Color lightColor)
        {
            int type = projectile.GetGlobalProjectile<LunarianBowReEffect>().EffectType;
            if (type == -1)
                return;

            // TODO：绘制附加图层、粒子环等
        }





    }
}
