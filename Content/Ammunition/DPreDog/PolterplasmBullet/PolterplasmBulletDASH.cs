using System;
using CalamityMod;
using CalamityMod.Enums;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityRangerExpansion.Content.Ammunition.DPreDog.PolterplasmBullet
{
    public class PolterplasmBulletDASH : ModPlayer
    {
        public static string ID => "Polterplasm Bullet Dash";

        public bool canDash = false; // 冲刺开关
        private bool isDashing = false;
        private int dashCooldown = 0;
        private const int dashCooldownMax = 20; // 冲刺冷却时间
        private const float dashSpeed = 40f; // 冲刺速度
        private int dnaParticleTimer = 0; // 控制粒子生成的计时器
        private int spikeParticleTimer = 0; // 控制尖刺特效生成的计时器
        private enum DashDirection
        {
            Directionless,
            Left,
            Right
        }

        public override void ResetEffects()
        {
            canDash = false; // 每帧重置开关，由 Buff 控制是否打开

            // 如果可以使用 PolterplasmBulletDASH，则设置为当前冲刺
            if (canDash)
            {
                Player.Calamity().DashID = "Polterplasm Bullet Dash";
            }
        }
        public override void ProcessTriggers(Terraria.GameInput.TriggersSet triggersSet)
        {
            DashDirection dashDirection;

            // 调用 HandleHorizontalDash 确定冲刺方向
            if (canDash && dashCooldown == 0 && HandleHorizontalDash(out dashDirection))
            {
                Player.direction = dashDirection == DashDirection.Right ? 1 : -1; // 根据冲刺方向调整玩家面向
                StartDash();
            }
        }

        private bool HandleHorizontalDash(out DashDirection direction)
        {
            direction = DashDirection.Directionless;
            bool dashWasExecuted = false;

            // 检测手动冲刺键是否绑定
            var manualDashHotkeys = CalamityMod.CalamityKeybinds.DashHotkey.GetAssignedKeys();
            bool manualHotkeyBound = (manualDashHotkeys?.Count ?? 0) > 0;
            bool pressedManualHotkey = manualHotkeyBound && CalamityMod.CalamityKeybinds.DashHotkey.JustPressed;

            int dashDirectionToUse = 0;

            // 如果手动冲刺键被按下，禁用双击方向键逻辑
            if (pressedManualHotkey)
            {
                if (Player.controlRight && !Player.controlLeft)
                    dashDirectionToUse = 1; // 向右冲刺
                else if (Player.controlLeft && !Player.controlRight)
                    dashDirectionToUse = -1; // 向左冲刺
                else
                    dashDirectionToUse = MathF.Abs(Player.velocity.X) <= 0.01f ? Player.direction : (Player.velocity.X > 0f ? 1 : -1);
            }
            else if (!manualHotkeyBound) // 如果未绑定手动冲刺键，使用双击方向键逻辑
            {
                bool vanillaLeftDashInput = Player.controlLeft && Player.releaseLeft;
                bool vanillaRightDashInput = Player.controlRight && Player.releaseRight;
                dashDirectionToUse = vanillaRightDashInput ? 1 : vanillaLeftDashInput ? -1 : 0;
            }

            // 确定冲刺方向并执行
            if (dashDirectionToUse == 1)
            {
                direction = DashDirection.Right;
                dashWasExecuted = true;
            }
            else if (dashDirectionToUse == -1)
            {
                direction = DashDirection.Left;
                dashWasExecuted = true;
            }

            return dashWasExecuted;
        }


        private void StartDash()
        {
            isDashing = true;
            dashCooldown = dashCooldownMax;
            dnaParticleTimer = 0;
            Player.immuneTime = 60; // 设置无敌时间

            // 冲刺方向基于玩家面向方向
            Player.velocity = new Vector2(Player.direction, 0).SafeNormalize(Vector2.Zero) * dashSpeed;
        }


        public override void PreUpdateMovement()
        {
            if (isDashing)
            {
                PerformDash();
                CheckDashCollision();
            }

            if (dashCooldown > 0)
            {
                dashCooldown--;
            }
        }

        private void PerformDash()
        {
            Player.velocity *= 0.975f; // 冲刺期间速度逐渐衰减

            // 生成改进后的粒子特效
            CreateDNAParticles();
            CreateSpikeParticles();

            // 如果速度低于一定阈值，停止冲刺
            if (Player.velocity.Length() < 5f)
            {
                isDashing = false;
            }
        }

        private void CheckDashCollision()
        {
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && !npc.friendly && npc.lifeMax > 5 && Player.getRect().Intersects(npc.getRect()))
                {
                    // 造成伤害
                    npc.StrikeNPC(new NPC.HitInfo
                    {
                        Damage = 300,
                        Knockback = 5f,
                        HitDirection = Player.direction
                    });

                    // 播放音效
                    SoundStyle sound = new("CalamityMod/Sounds/Item/PhantomSpirit");
                    SoundEngine.PlaySound(sound, npc.Center);

                    // 屏幕震动效果
                    float shakePower = 1.7f; // 设置震动强度
                    float distanceFactor = Utils.GetLerpValue(1000f, 0f, Player.Distance(npc.Center), true); // 距离衰减
                    Main.LocalPlayer.Calamity().GeneralScreenShakePower = Math.Max(Main.LocalPlayer.Calamity().GeneralScreenShakePower, shakePower * distanceFactor);

                    // 播放冲击波特效（大小减少30%）
                    //float particleScale = 0.7f; // 调整为70%大小
                    //Particle explosion = new DetailedExplosion(npc.Center, Vector2.Zero, Color.Gray * 0.6f, Vector2.One, Main.rand.NextFloat(-5, 5), 0f, particleScale + 0.07f, 20, false);
                    //GeneralParticleHandler.SpawnParticle(explosion);

                    //Particle explosion2 = new DetailedExplosion(npc.Center, Vector2.Zero, Color.Orange, Vector2.One, Main.rand.NextFloat(-5, 5), 0f, particleScale, 20);
                    //GeneralParticleHandler.SpawnParticle(explosion2);

                    // 播放幽灵蓝色或浅粉红色光环特效
                    // 确保冲刺特效每次生成两个粒子
                    for (int i = 0; i < 2; i++)
                    {
                        // 定义颜色：一个浅蓝，一个浅粉红
                        Color particleColor = (i == 0) ? Color.LightBlue : Color.LightPink;

                        // 随机方向
                        float randomRotation = Main.rand.NextFloat(0, MathHelper.TwoPi);
                        Vector2 randomDirection = randomRotation.ToRotationVector2();

                        // 创建粒子
                        Particle blastRing = new CustomPulse(
                            Player.Center,                        // 粒子生成位置
                            randomDirection * 5f,                // 粒子移动方向（加速度大小可调）
                            particleColor,                       // 粒子颜色
                            "CalamityRangerExpansion/texture/PolterplasmBulletDASH", // 粒子纹理路径
                            Vector2.One * 0.33f,                 // 粒子缩放大小
                            Main.rand.NextFloat(-10f, 10f),      // 粒子旋转速度
                            0.035f,                              // 粒子衰减率
                            0.45f,                               // 粒子透明度
                            30                                   // 粒子生命周期
                        );

                        // 生成粒子
                        GeneralParticleHandler.SpawnParticle(blastRing);
                    }

                    // 播放粒子特效（随机喷射）
                    for (int i = 0; i < Main.rand.Next(35, 76); i++)
                    {
                        Vector2 velocity = Main.rand.NextVector2Circular(7.5f, 7.5f) * Main.rand.NextFloat(3.5f, 7.5f);
                        Dust dust = Dust.NewDustPerfect(npc.Center, Main.rand.Next(new[] { 135, 180, 187 }), velocity, 150, default, Main.rand.NextFloat(1.93f, 2.75f));
                        dust.noGravity = true;
                    }
                }
            }
        }

        private void CreateDNAParticles()
        {
            if (++dnaParticleTimer % 5 != 0) return; // 控制粒子生成频率

            for (int i = 0; i < Main.rand.Next(5, 15); i++)
            {
                Dust dust = Dust.NewDustPerfect(
                    Player.Center + Main.rand.NextVector2Circular(10f, 10f),
                    Main.rand.Next(new[] { DustID.PinkTorch, 177 }),
                    null,
                    150,
                    default,
                    Main.rand.NextFloat(2.0f, 2.75f)
                );
                dust.noGravity = true;
            }
        }

        private void CreateSpikeParticles()
        {
            // 玩家冲刺方向的单位向量
            Vector2 direction = Player.velocity.SafeNormalize(Vector2.Zero);

            // 固定生成位置：玩家中心上方和下方
            Vector2 upperPosition = Player.Center + new Vector2(0, -2 * 16);
            Vector2 lowerPosition = Player.Center + new Vector2(0, 2 * 16);

            // 固定生成方向：玩家冲刺方向的正后方
            Vector2 reverseDirection = -direction;

            // 生成上方粒子
            PointParticle upperSpark = new PointParticle(
                upperPosition,
                reverseDirection * 5f, // 固定速度
                false,
                5, // 存在时间
                1.5f, // 固定大小
                Color.LightBlue // 固定颜色
            );
            GeneralParticleHandler.SpawnParticle(upperSpark);

            // 生成下方粒子
            PointParticle lowerSpark = new PointParticle(
                lowerPosition,
                reverseDirection * 5f, // 固定速度
                false,
                5, // 存在时间
                1.5f, // 固定大小
                Color.LightPink // 固定颜色
            );
            GeneralParticleHandler.SpawnParticle(lowerSpark);
        }
    }
}
