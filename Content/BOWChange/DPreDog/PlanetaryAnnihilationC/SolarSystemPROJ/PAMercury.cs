using CalamityMod;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace CalamityRangerExpansion.Content.BOWChange.DPreDog.PlanetaryAnnihilationC.SolarSystemPROJ
{
    internal class PAMercury : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.DPreDog";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // 画残影效果
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
        public override void SetDefaults()
        {
            // 设置弹幕的基础属性
            Projectile.width = 11; // 弹幕宽度
            Projectile.height = 24; // 弹幕高度
            Projectile.friendly = true; // 对敌人有效
            Projectile.DamageType = DamageClass.Ranged; // 远程伤害类型
            Projectile.penetrate = 1; // 穿透力为1，击中一个敌人就消失
            Projectile.timeLeft = 300; // 弹幕存在时间为x帧
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
            Projectile.ignoreWater = true; // 弹幕不受水影响
            Projectile.arrow = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            // 调整弹幕的旋转，使其在飞行时保持水平
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;

            // Lighting - 添加天蓝色光源，光照强度为 0.49
            Lighting.AddLight(Projectile.Center, Color.LightSkyBlue.ToVector3() * 0.49f);


            // 每隔 x 帧生成一次
            if (Projectile.localAI[0] % 15 == 0)
            {
                // 生成双螺旋气泡特效
                float offset = (float)Math.Sin(Projectile.localAI[0] * 0.1f) * 1.5f; // 双螺旋的偏移量
                Vector2 bubblePos1 = Projectile.Center + Projectile.velocity.RotatedBy(MathHelper.PiOver2) * offset;
                Vector2 bubblePos2 = Projectile.Center + Projectile.velocity.RotatedBy(-MathHelper.PiOver2) * offset;
                Gore bubble1 = Gore.NewGorePerfect(Projectile.GetSource_FromAI(), bubblePos1, Projectile.velocity * 0.2f + Main.rand.NextVector2Circular(1f, 1f), 411);
                Gore bubble2 = Gore.NewGorePerfect(Projectile.GetSource_FromAI(), bubblePos2, Projectile.velocity * 0.2f + Main.rand.NextVector2Circular(1f, 1f), 411);
                bubble1.timeLeft = 8 + Main.rand.Next(6);
                bubble2.timeLeft = 8 + Main.rand.Next(6);
                bubble1.scale = Main.rand.NextFloat(0.6f, 1f);
                bubble2.scale = Main.rand.NextFloat(0.6f, 1f);
                bubble1.type = Main.rand.NextBool(3) ? 412 : 411;
                bubble2.type = Main.rand.NextBool(3) ? 412 : 411;
            }
             

            // 更新本地 AI
            Projectile.localAI[0]++;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 计算治疗量为本次伤害的 10%
            int healAmount = (int)(damageDone * 0.10f);

            // 遍历所有存活玩家并治疗
            foreach (Player player in Main.player)
            {
                if (player.active && !player.dead)
                {
                    player.statLife += healAmount;
                    player.HealEffect(healAmount); // 显示治疗效果
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            // 在死亡时生成一组气泡
            for (int i = 0; i < 2; i++) // 生成 x 个气泡
            {
                Vector2 bubblePos = Projectile.Center + Main.rand.NextVector2Circular(16f, 16f); // 随机生成位置
                Gore bubble = Gore.NewGorePerfect(Projectile.GetSource_FromThis(), bubblePos, Main.rand.NextVector2Circular(1f, 1f), 411);
                bubble.timeLeft = 10 + Main.rand.Next(6);
                bubble.scale = Main.rand.NextFloat(0.8f, 1.2f);
                bubble.type = Main.rand.NextBool(3) ? 412 : 411;
            }
        }







    }
}