using CalamityRangerExpansion.Content.Gel.DPreDog.UelibloomGel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using CalamityMod.Buffs.DamageOverTime;
using CalamityRangerExpansion.Content.DeveloperItems.Weapon.TheGoldenFire;

namespace CalamityRangerExpansion.Content.Gel.CPreMoodLord.AstralGel
{
    public class AstralGelGP : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool IsAstralGelInfused = false;

        private int staticTimer = 0; // 计时器，用于控制静止状态
        private bool isStatic = false; // 是否处于静止状态

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (projectile.type == ModContent.ProjectileType<TheGoldenFirePROJAG>())
                return; // 直接返回，不对这个弹幕进行修改

            if (source is EntitySource_ItemUse_WithAmmo ammoSource && ammoSource.AmmoItemIdUsed == ModContent.ItemType<AstralGel>())
            {
                IsAstralGelInfused = true;
                projectile.netUpdate = true;
                projectile.damage = (int)(projectile.damage * 0.85f); // 减少 15% 伤害
            }
            base.OnSpawn(projectile, source);
        }

        public override void AI(Projectile projectile)
        {
            if (IsAstralGelInfused)
            {
                // 记录弹幕的飞行距离
                if (projectile.localAI[0] == 0)
                {
                    // 在第一次调用AI时，初始化弹幕的初始位置
                    projectile.localAI[0] = projectile.Center.X; // 记录初始X坐标
                    projectile.localAI[1] = projectile.Center.Y; // 记录初始Y坐标
                }

                // 计算弹幕从初始位置到当前的位置的总飞行距离
                float distanceX = projectile.Center.X - projectile.localAI[0];
                float distanceY = projectile.Center.Y - projectile.localAI[1];
                float totalDistance = (float)Math.Sqrt(distanceX * distanceX + distanceY * distanceY);

                // 如果弹幕飞行距离达到10个tile（x * 16像素）
                if (totalDistance >= 18 * 16f)
                {
                    projectile.velocity = Vector2.Zero; // 强制静止
                    isStatic = true; // 设置静止状态
                }

                // 计时器逻辑（在静止状态开始计时）
                if (isStatic)
                {
                    staticTimer++;
                    projectile.timeLeft = 180 * projectile.MaxUpdates;
                    projectile.alpha = 0;    
                    if (staticTimer >= 180 * projectile.MaxUpdates) // 静止X秒后
                    {
                        projectile.Kill(); // 强制销毁弹幕
                    }
                }
            }
        }


        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (IsAstralGelInfused && target.active && !target.friendly)
            {
                // 伤害调整
                if (isStatic)
                {
                    damageDone = (int)(damageDone * 1f * 0.25f); // 静止状态期间伤害 *85% *25%
                }
                else
                {
                    damageDone = (int)(damageDone * 1f); // 普通伤害 *85%
                }

                // 添加 AstralInfectionDebuff，持续 300 帧（5 秒）
                target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 300);
            }
        }
    }
}

