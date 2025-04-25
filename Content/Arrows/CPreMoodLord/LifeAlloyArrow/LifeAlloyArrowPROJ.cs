using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Ranged;
using Terraria.DataStructures;
using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using CalamityMod.Particles;
using CalamityRangerExpansion.CREConfigs;
using CalamityRangerExpansion.Content.Arrows.CPreMoodLord.PerennialArrow;
using CalamityRangerExpansion.Content.Arrows.CPreMoodLord.ScoriaArrow;

namespace CalamityRangerExpansion.Content.Arrows.CPreMoodLord.LifeAlloyArrow
{
    public class LifeAlloyArrowPROJ : ModProjectile, ILocalizedModType
    {
        public override string Texture => "CalamityRangerExpansion/Content/Arrows/CPreMoodLord/LifeAlloyArrow/LifeAlloyArrow";

        public new string LocalizationCategory => "Projectile.CPreMoodLord";
        private Color currentColor = Color.Black; // 初始化为黑色
        private float bendAngle = 0f;
        private bool growing = false;
        private float variance;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 画残影效果
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
                return false;
            }
            return true;
        }
        public override void SetDefaults()
        {
            // 设置弹幕的基础属性
            Projectile.width = 11; // 弹幕宽度
            Projectile.height = 24; // 弹幕高度
            Projectile.friendly = true; // 对敌人有效
            Projectile.DamageType = DamageClass.Ranged; // 远程伤害类型
            Projectile.penetrate = 1; // 穿透力为1，击中一个敌人就消失
            Projectile.timeLeft = 300; // 弹幕存在时间为600帧
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
            Projectile.ignoreWater = true; // 弹幕不受水影响
            Projectile.arrow = true;
            Projectile.extraUpdates = 3;
        }

        private Color currentColorLeft = Color.Black;
        private Color currentColorRight = Color.Black;
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.velocity *= 0.4f;
        }
        public override void AI()
        {
            // 调整弹幕的旋转，使其在飞行时保持水平
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 初始化弯折控制变量
                if (variance == 0)
                {
                    variance = Main.rand.NextFloat(0.3f, 1.7f); // 随机控制弯折的幅度
                }

                // 控制弯折角度的变化
                if (bendAngle <= -0.5f)
                {
                    growing = true;
                }
                if (bendAngle >= 0.5f)
                {
                    growing = false;
                }
                bendAngle += (growing ? 0.07f * variance : -0.07f * variance);

                // 计算粒子生成的偏移位置（保持箭矢直线飞行）
                Vector2 offset = Projectile.velocity.RotatedBy(bendAngle) * 5f; // 控制弯折幅度
                Vector2 particlePosition = Projectile.Center + offset;

                // 检查是否启用了特效
                if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
                {
                    // 初始化随机颜色（仅设置一次）
                    if (currentColorLeft == Color.Black && currentColorRight == Color.Black)
                    {
                        currentColorLeft = Main.rand.NextBool() ? Color.Red : Color.Green;
                        currentColorRight = Main.rand.NextBool() ? Color.Blue : Color.Yellow;
                    }

                    // 添加天蓝色光源
                    Lighting.AddLight(Projectile.Center, Color.LightSkyBlue.ToVector3() * 0.49f);

                    // 获取偏移位置的左右两侧位置
                    Vector2 leftPosition = particlePosition - Projectile.velocity.SafeNormalize(Vector2.Zero) * Projectile.width / 2;
                    Vector2 rightPosition = particlePosition + Projectile.velocity.SafeNormalize(Vector2.Zero) * Projectile.width / 2;

                    // 在左端生成带有随机颜色的粒子
                    GlowOrbParticle leftOrb = new GlowOrbParticle(
                        leftPosition, Vector2.Zero, false, 5, 0.55f, currentColorLeft, true, true
                    );
                    GeneralParticleHandler.SpawnParticle(leftOrb);

                    // 在右端生成带有随机颜色的粒子
                    GlowOrbParticle rightOrb = new GlowOrbParticle(
                        rightPosition, Vector2.Zero, false, 5, 0.55f, currentColorRight, true, true
                    );
                    GeneralParticleHandler.SpawnParticle(rightOrb);

                    // 生成视觉上的光点效果
                    PointParticle leftSpark = new PointParticle(
                        leftPosition, Projectile.velocity, false, 2, 0.6f, currentColorLeft
                    );
                    GeneralParticleHandler.SpawnParticle(leftSpark);

                    PointParticle rightSpark = new PointParticle(
                        rightPosition, Projectile.velocity, false, 2, 0.6f, currentColorRight
                    );
                    GeneralParticleHandler.SpawnParticle(rightSpark);
                }

            }

                
        }

        public override void OnKill(int timeLeft)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 随机生成 1~3 个额外弹幕
                int extraProjectiles = Main.rand.Next(1, 3); // 随机生成数量（1 至 2）
                for (int b = 0; b < extraProjectiles; b++)
                {
                    Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        Projectile.Center,
                        Projectile.velocity.RotatedByRandom(0.5f) * 0.2f,
                        ModContent.ProjectileType<LifeAlloyArrowPROJSPLIT>(),
                        (int)((Projectile.damage) * 0.1), // 伤害
                        0f,
                        Projectile.owner,
                        ai0: 0f,
                        ai1: 0f,
                        ai2: Main.rand.Next(0, 5) // 随机生成 0 到 4 的数值，决定颜色
                    );
                }
            }
        }

        #region 旧代码

        //public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        //{
        //    Player player = Main.player[Projectile.owner];
        //    Vector2 circleCenter = player.Center; // 圆心为玩家位置
        //    float circleRadius = 5 * 16f; // 半径为5个方块，转换为像素

        //    // 随机选择一个角度
        //    float randomAngle = Main.rand.NextFloat(0, MathHelper.TwoPi);
        //    Vector2 spawnPosition = circleCenter + circleRadius * new Vector2((float)Math.Cos(randomAngle), (float)Math.Sin(randomAngle));

        //    // 瞄准主弹幕击中敌人的位置
        //    Vector2 directionToTarget = Vector2.Normalize(target.Center - spawnPosition) * Projectile.velocity.Length(); // 方向向量，长度等于主弹幕的速度

        //    // 随机选择一种弹幕类型
        //    int selectedProjectileType;
        //    Color lineColor;
        //    Color smokeColor;

        //    switch (Main.rand.Next(3))
        //    {
        //        case 0: // PerennialArrowPROJ
        //            selectedProjectileType = ModContent.ProjectileType<PerennialArrowPROJ>();
        //            lineColor = Color.Green;
        //            smokeColor = Color.Green;
        //            break;
        //        case 1: // ScoriaArrowPROJ
        //            selectedProjectileType = ModContent.ProjectileType<ScoriaArrowPROJ>();
        //            lineColor = Color.OrangeRed;
        //            smokeColor = Color.OrangeRed;
        //            break;
        //        default: // VeriumBoltProj
        //            selectedProjectileType = ModContent.ProjectileType<VeriumBoltProj>();
        //            lineColor = Color.LightBlue;
        //            smokeColor = Color.LightBlue;
        //            break;
        //    }

        //    // 发射弹幕，让新生成的弹幕速度是本体速度的x倍
        //    Vector2 adjustedVelocity = directionToTarget * 50f;
        //    Projectile.NewProjectile(Projectile.GetSource_FromThis(), spawnPosition, adjustedVelocity, selectedProjectileType, (int)((Projectile.damage) * 0.75), Projectile.knockBack, Projectile.owner);

        //    // 检查是否启用了特效
        //    if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
        //    {
        //        // 画粒子特效连接线
        //        for (float i = 0; i <= 1; i += 0.05f) // 线条由粒子连接组成
        //        {
        //            Vector2 positionOnLine = Vector2.Lerp(Projectile.Center, spawnPosition, i);
        //            Particle lineParticle = new GlowOrbParticle(positionOnLine, Vector2.Zero, false, 3, 0.4f, lineColor, true, true);
        //            GeneralParticleHandler.SpawnParticle(lineParticle);
        //        }

        //        // 在弹幕发射位置释放轻型烟雾
        //        for (int i = 0; i < 25; i++)
        //        {
        //            Vector2 dustVelocity = Main.rand.NextVector2Circular(2f, 2f); // 随机速度
        //            Particle smoke = new HeavySmokeParticle(spawnPosition, dustVelocity * Main.rand.NextFloat(1f, 2.6f), smokeColor, 18, Main.rand.NextFloat(0.9f, 1.6f), 0.35f, Main.rand.NextFloat(-1, 1), true);
        //            GeneralParticleHandler.SpawnParticle(smoke);
        //        }
        //    }

        //}

        #endregion
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 60% 概率触发后续逻辑
            if (Main.rand.NextFloat() > 0.6f)
                return;

            Player player = Main.player[Projectile.owner];

            // 定义偏移量
            float offsetX = 5 * 16f; // 水平偏移 5 格
            float offsetY = 3 * 16f; // 垂直偏移 3 格

            // 计算从玩家到目标的方向向量
            Vector2 relativeDirection = Vector2.Normalize(target.Center - player.Center);

            // 计算垂直偏移方向
            Vector2 perpendicularOffset = new Vector2(-relativeDirection.Y, relativeDirection.X) * offsetX;

            // 计算两个额外弹幕生成点
            Vector2 firstSpawnPosition = player.Center + relativeDirection * offsetY + perpendicularOffset;
            Vector2 secondSpawnPosition = player.Center + relativeDirection * offsetY - perpendicularOffset;

            // 瞄准目标
            Vector2 directionToTargetFirst = Vector2.Normalize(target.Center - firstSpawnPosition) * Projectile.velocity.Length();
            Vector2 directionToTargetSecond = Vector2.Normalize(target.Center - secondSpawnPosition) * Projectile.velocity.Length();

            // 随机选择弹幕类型
            int selectedProjectileType;
            Color lineColor;
            Color smokeColor;

            switch (Main.rand.Next(3))
            {
                case 0:
                    selectedProjectileType = ModContent.ProjectileType<PerennialArrowPROJ>();
                    lineColor = Color.Green;
                    smokeColor = Color.Green;
                    break;
                case 1:
                    selectedProjectileType = ModContent.ProjectileType<ScoriaArrowPROJ>();
                    lineColor = Color.OrangeRed;
                    smokeColor = Color.OrangeRed;
                    break;
                default:
                    selectedProjectileType = ModContent.ProjectileType<VeriumBoltProj>();
                    lineColor = Color.LightBlue;
                    smokeColor = Color.LightBlue;
                    break;
            }

            // 在两个固定点生成弹幕
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), firstSpawnPosition, directionToTargetFirst * 50f, selectedProjectileType, (int)((Projectile.damage) * 0.175), Projectile.knockBack, Projectile.owner);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), secondSpawnPosition, directionToTargetSecond * 50f, selectedProjectileType, (int)((Projectile.damage) * 0.175), Projectile.knockBack, Projectile.owner);

            // 特效：生成粒子连接线和轻型烟雾
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 从命中点到第一个生成点绘制粒子线
                DrawParticleLine(Projectile.Center, firstSpawnPosition, lineColor);
                // 从命中点到第二个生成点绘制粒子线
                DrawParticleLine(Projectile.Center, secondSpawnPosition, lineColor);

                // 释放轻型烟雾
                GenerateSmoke(firstSpawnPosition, smokeColor);
                GenerateSmoke(secondSpawnPosition, smokeColor);
            }
        }

        // 工具方法：绘制粒子连接线
        private void DrawParticleLine(Vector2 start, Vector2 end, Color color)
        {
            for (float i = 0; i <= 1; i += 0.05f)
            {
                Vector2 positionOnLine = Vector2.Lerp(start, end, i);
                Particle lineParticle = new GlowOrbParticle(positionOnLine, Vector2.Zero, false, 3, 0.4f, color, true, true);
                GeneralParticleHandler.SpawnParticle(lineParticle);
            }
        }

        // 工具方法：释放轻型烟雾
        private void GenerateSmoke(Vector2 position, Color color)
        {
            for (int i = 0; i < 15; i++)
            {
                Vector2 dustVelocity = Main.rand.NextVector2Circular(2f, 2f);
                Particle smoke = new HeavySmokeParticle(position, dustVelocity * Main.rand.NextFloat(1f, 2.6f), color, 18, Main.rand.NextFloat(0.9f, 1.6f), 0.35f, Main.rand.NextFloat(-1, 1), true);
                GeneralParticleHandler.SpawnParticle(smoke);
            }
        }
    }
}