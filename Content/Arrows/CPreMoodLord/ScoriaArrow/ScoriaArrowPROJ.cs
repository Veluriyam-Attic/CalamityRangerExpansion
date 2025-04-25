using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalamityMod;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Projectiles.Typeless;
using CalamityRangerExpansion.CREConfigs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;


namespace CalamityRangerExpansion.Content.Arrows.CPreMoodLord.ScoriaArrow
{
    public class ScoriaArrowPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectile.CPreMoodLord";
        private bool hasTriggeredUpwardMovement = false; // 添加变量来标记是否触发过向上飞行效果

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 7;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 如果禁用特效计时器大于 0，则直接返回 false（禁用特效）
                if (disableEffectTimer > 0)
                {
                    return false;
                }

                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Type], lightColor, 1);
                return false;
            }
            return true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 14; // 假设箭的宽度
            Projectile.height = 28; // 假设箭的高度
            Projectile.friendly = true; // 是否对玩家友好
            Projectile.DamageType = DamageClass.Ranged; // 远程伤害类型
            Projectile.arrow = true; // 标记为箭类型
            Projectile.extraUpdates = 0; // 提升更新速率
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
            Projectile.timeLeft = 240; // 持续时间为4秒
            Projectile.aiStyle = ProjAIStyleID.Arrow; // 受重力影响
            Projectile.ignoreWater = false;
            Projectile.penetrate = 2; // 2穿
        }
        public override void OnSpawn(IEntitySource source)
        {
            Projectile.velocity *= 0.6f;
        }
        private int disableEffectTimer = 0; // 计时器，控制禁用特效的时间
        public override void AI()
        {
            if (disableEffectTimer > 0)
                disableEffectTimer--; // 每帧减少计时器

            // 调整弹幕的旋转，使其在飞行时保持水平
            // 垂直朝下贴图
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;

            // 倾斜朝右下角贴图
            // Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi - MathHelper.ToRadians(45);


            // 加速效果：每帧增加2%的X轴速度，Y轴增加5个速度单位
            Projectile.velocity.X *= 1.02f;
            Projectile.velocity.Y *= 1.03f;

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 可以加入一些基本的粒子特效
                if (Main.rand.NextBool(3))
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, null, 0, Color.Orange, 1.5f);
                    dust.noGravity = true;
                    dust.velocity *= 0.5f;
                }

                // 添加烟雾效果，每隔一段时间随机释放烟雾
                if (Main.rand.NextBool(5) && Main.netMode != NetmodeID.Server) // 20%的几率生成烟雾
                {
                    int smoke = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position, Vector2.Zero, Main.rand.Next(375, 378), 0.75f);
                    Main.gore[smoke].velocity = Projectile.velocity * 0.1f;
                    Main.gore[smoke].behindTiles = true;
                }
            }
             

            // 自定义AI逻辑
            if (Projectile.timeLeft <= 60 && !hasTriggeredUpwardMovement) // 飞行了4秒，没有命中任何东西且没有触发过上升效果
            {
                TriggerDownwardMovement(); // 触发垂直向上飞行效果
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!hasTriggeredUpwardMovement) // 确保只触发一次
            {
                TriggerDownwardMovement(); // 触发向上飞行
            }
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (!hasTriggeredUpwardMovement) // 确保只触发一次
            {
                TriggerDownwardMovement(); // 触发向上飞行
            }
            // 添加5秒的狱炎减益效果（狱炎持续时间为5秒，单位为60帧/秒）
            target.AddBuff(BuffID.OnFire, 300); // 300帧 = 5秒
        }

        // 定义垂直飞行和粒子效果的触发
        private void TriggerDownwardMovement()
        {

            disableEffectTimer = 6; // 禁用特效 6 帧

            // x%的概率触发爆炸效果
            if (Main.rand.NextFloat() < 0.2f)
            {
                // 生成爆炸弹幕
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<FuckYou>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            }
            // 标记已经触发
            hasTriggeredUpwardMovement = true;

            // 改变方向为向下飞行
            Projectile.velocity = new Vector2(0, 6.5f).RotatedByRandom(MathHelper.ToRadians(15)); // 随机偏移向下的方向

            // 允许穿墙
            Projectile.tileCollide = false;

            // 无限穿透
            Projectile.penetrate = -1; // 无限穿透

            // 调整粒子颜色，使用橘黄色或深灰色
            SparkParticle Visual = new SparkParticle(Projectile.Center, Projectile.velocity * 0.1f, false, 2, 1.2f, Color.Orange);
            GeneralParticleHandler.SpawnParticle(Visual);

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 类似的轨迹粒子效果
                if (Projectile.localAI[0] % 3 == 0)
                {
                    LineParticle subTrail = new LineParticle(Projectile.Center, Projectile.velocity * 0.01f, false, 4, 1.1f, Color.Orange);
                    GeneralParticleHandler.SpawnParticle(subTrail);
                }
            }


            // 在固定距离飞行后消失，并产生粒子
            if (Projectile.localAI[0] >= 100) // 假设飞行100帧的距离
            {
                Projectile.Kill();
                for (int i = 0; i < 5; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, -Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(30)), 0, Color.Orange, Main.rand.NextFloat(1.5f, 2.5f));
                    dust.noGravity = true;
                }
            }

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 在飞行的过程中释放波浪线粒子效果
                if (Main.rand.NextBool(2))
                {
                    Vector2 offset = new Vector2(0, 5).RotatedByRandom(MathHelper.ToRadians(180));
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, DustID.Torch, null, 0, Color.Orange, 1.5f);
                    dust.noGravity = true;
                }
            }


            // 定期释放 ScoriaArrowFireball 弹幕
            if (Projectile.localAI[0] % 3 == 0)
            {
                // 生成一发朝向主弹幕反方向的弹幕
                Vector2 reverseDirection = -Projectile.velocity.SafeNormalize(Vector2.UnitX) * 5f; // 获取反向速度并设置为5
                //Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, reverseDirection, ModContent.ProjectileType<ScoriaArrowFireball>(), (int)(Projectile.damage * 0.33f), Projectile.knockBack, Projectile.owner);
            }

        }


        public override void OnKill(int timeLeft)
        {
            // 弹幕结束时生成橘黄色粒子特效
            for (int i = 0; i < 15; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Torch, -Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(30)), 0, Color.Orange, Main.rand.NextFloat(1.5f, 2.5f));
                dust.noGravity = true;
            }
            for (int i = 0; i < 3; i++)
            {
                //Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Circular(5f, 5f), ModContent.ProjectileType<ScoriaArrowFireball>(), (int)(Projectile.damage * 0.33f), Projectile.knockBack, Projectile.owner);
            }

        }
    }
}