using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace CalamityRangerExpansion.Content.BOWChange
{
    public static partial class CBR_UT
    {
        public static void UpdateProjectileHeldVariables(Projectile projectile, Player player, Vector2 armPosition, Vector2 shootDirection)
        {
            if (Main.myPlayer == projectile.owner)
            {
                // 插值平滑调整方向
                float interpolant = Utils.GetLerpValue(5f, 25f, player.Distance(Main.MouseWorld), true);
                Vector2 oldVelocity = projectile.velocity;
                projectile.velocity = Vector2.Lerp(projectile.velocity, shootDirection, interpolant);

                // 如果方向变化，触发网络更新
                if (projectile.velocity != oldVelocity)
                {
                    projectile.netSpam = 0;
                    projectile.netUpdate = true;
                }
            }

            // 更新位置和旋转
            projectile.position = armPosition - projectile.Size * 0.5f + projectile.velocity.SafeNormalize(Vector2.Zero) * 35f;

            projectile.rotation = projectile.velocity.ToRotation();
            int oldDirection = projectile.spriteDirection;
            if (oldDirection == -1)
                projectile.rotation += MathHelper.Pi;
            projectile.spriteDirection = (projectile.velocity.X > 0).ToDirectionInt();

            // 方向改变时修正旋转
            if (projectile.spriteDirection != oldDirection)
                projectile.rotation -= MathHelper.Pi;

            // 防止投射物消失
            projectile.timeLeft = 3;
        }

        public static void ManipulatePlayerVariables(Projectile projectile, Player player, Vector2 shootDirection)
        {
            player.ChangeDir(projectile.direction); // 确保玩家方向和投射物一致
            player.heldProj = projectile.whoAmI; // 绑定手持投射物
            player.itemTime = player.itemAnimation = 2; // 防止动画中断
            player.itemRotation = (projectile.velocity * projectile.direction).ToRotation(); // 设置物品旋转角度

            projectile.position = player.RotatedRelativePoint(player.MountedCenter, true) - projectile.Size / 2f; // 让弹幕的旋转中心完美对着玩家
            projectile.rotation = projectile.velocity.ToRotation();
        }

        public static void AdjustProjectileDirectionAndFlip(Projectile projectile, ref SpriteEffects spriteEffects, ref float rotation)
        {
            // 动态检测方向并翻转
            if (Math.Cos(projectile.rotation) < 0f) // 如果弹幕指向左侧
            {
                spriteEffects = SpriteEffects.FlipHorizontally; // 水平翻转
                rotation += MathHelper.Pi; // 旋转180度
            }
        }

    }
}
