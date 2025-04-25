using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityRangerExpansion.Content.DeveloperItems.Weapon.DiffuseNovaArc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class DiffuseNovaArcHoldout : BaseGunHoldoutProjectile
    {
        public override int AssociatedItemID => ModContent.ItemType<DiffuseNovaArc>();
        public override float MaxOffsetLengthFromArm => 24f;
        public override float OffsetXUpwards => -5f;
        public override float BaseOffsetY => -5f;
        public override float OffsetYDownwards => 5f;

        public static int FramesPerLoad = 13;
        public static int MaxLoadableShots = 15;
        public static float BulletSpeed = 12f;
        public SlotId NovaChargeSlot;

        public ref float CurrentChargingFrames => ref Projectile.ai[0];
        public ref float ShotsLoaded => ref Projectile.ai[1];
        public ref float ShootRecoilTimer => ref Projectile.ai[2]; // Dual functions for rapid fire shooting cooldown and recoil
        public bool ChargeLV1 => CurrentChargingFrames >= ArcNovaDiffuser.Charge1Frames;
        public bool ChargeLV2 => CurrentChargingFrames >= ArcNovaDiffuser.Charge2Frames;

        public override void KillHoldoutLogic()
        {
            if (Owner.CantUseHoldout(false) || HeldItem.type != Owner.ActiveItem().type)
                Projectile.Kill();
        }
        // 确保大光球只发射一次
        private bool hasFiredBigShot = false;
        // 阶段二的发射次数计数器
        private int stageTwoShotsFired = 0;

        public override void HoldoutAI()
        {
            if (SoundEngine.TryGetActiveSound(NovaChargeSlot, out var ChargeSound) && ChargeSound.IsPlaying)
                ChargeSound.Position = Projectile.Center;

            // Fire if the owner stops channeling or otherwise cannot use the weapon.
            if (Owner.CantUseHoldout())
            {
                KeepRefreshingLifetime = false;

                // 第二阶段后，扫射固定 25 发
                if (ChargeLV2)
                {
                    // 每帧递减 ShootRecoilTimer
                    if (ShootRecoilTimer > 0)
                    {
                        ShootRecoilTimer -= 1f; // 每帧减少 1
                        return; // 如果计时器未到零，直接返回
                    }

                    if (ShotsLoaded > 0 && stageTwoShotsFired < 25) // 限制发射次数为25
                    {
                        Projectile.timeLeft = ArcNovaDiffuser.AftershotCooldownFrames * 2;

                        ChargeSound?.Stop();
                        SoundEngine.PlaySound(ArcNovaDiffuser.SmallShot, Projectile.position);

                        Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * BulletSpeed;
                        Vector2 fireVec = shootVelocity.RotatedByRandom(MathHelper.ToRadians(2f));
                        Projectile.NewProjectile(
                            Projectile.GetSource_FromThis(),
                            GunTipPosition,
                            fireVec,
                            ModContent.ProjectileType<NovaShot>(), // 扫射弹幕类型
                            (int)((Projectile.damage) * 2.5), // 当前伤害
                            Projectile.knockBack,
                            Projectile.owner
                        );

                        // 特效
                        for (int i = 0; i <= 4; i++)
                        {
                            Dust dust = Dust.NewDustPerfect(
                                GunTipPosition - Projectile.velocity * 15,
                                107,
                                shootVelocity.RotatedByRandom(MathHelper.ToRadians(15f)) * Main.rand.NextFloat(0.9f, 1.2f),
                                0,
                                default,
                                Main.rand.NextFloat(0.8f, 1.8f)
                            );
                            dust.noGravity = true;
                        }

                        ShotsLoaded--; // 固定减少
                        stageTwoShotsFired++; // 增加发射计数器

                        // 调整最终蓄力模式的发射间隔，与正常扫射一致
                        ShootRecoilTimer = 5f; // 重置发射间隔

                        OffsetLengthFromArm -= 5f;

                        // 当到达 25 发时，重置状态
                        if (stageTwoShotsFired >= 25)
                        {
                            ShotsLoaded = 0; // 清空弹药
                            stageTwoShotsFired = 0; // 重置发射计数器
                        }
                    }
                }

                // 第一阶段蓄力到第二阶段，发射1个大光球
                else if (ChargeLV1 && !hasFiredBigShot)
                {
                    Projectile.timeLeft = ArcNovaDiffuser.AftershotCooldownFrames * 2;

                    ChargeSound?.Stop();
                    SoundEngine.PlaySound(ArcNovaDiffuser.BigShot, Projectile.position);

                    Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * BulletSpeed;
                    int charge2Damage = Projectile.damage * 20; // 使用大光球伤害
                    float charge2KB = Projectile.knockBack * 3f;
                    Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        GunTipPosition,
                        shootVelocity,
                        ModContent.ProjectileType<NovaChargedShot>(), // 大光球类型
                        charge2Damage,
                        charge2KB,
                        Projectile.owner
                    );

                    // 屏幕震动和特效
                    Owner.Calamity().GeneralScreenShakePower = 6.5f;
                    for (int i = 0; i <= 25; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(
                            GunTipPosition - Projectile.velocity * 15,
                            107,
                            shootVelocity.RotatedByRandom(MathHelper.ToRadians(15f)) * Main.rand.NextFloat(0.2f, 1.2f),
                            0,
                            default,
                            Main.rand.NextFloat(1f, 2.3f)
                        );
                        dust.noGravity = true;
                    }

                    ShotsLoaded--; // 减少弹药
                    if (ShotsLoaded <= 0) ShotsLoaded = 15; // 第一阶段子弹填满
                    ShootRecoilTimer = 24f;
                    OffsetLengthFromArm -= 25f;

                    // 设置标志，确保大光球只发射一次
                    hasFiredBigShot = true;
                }
                // 第一阶段蓄力前，正常扫射
                else if (ShotsLoaded > 0 && !ChargeLV1 && !ChargeLV2) // 确保仅在第一阶段蓄力前触发
                {
                    Projectile.timeLeft = ArcNovaDiffuser.AftershotCooldownFrames;

                    ShootRecoilTimer -= 2f;

                    if (ShootRecoilTimer <= 0f)
                    {
                        ChargeSound?.Stop();
                        SoundEngine.PlaySound(ArcNovaDiffuser.SmallShot, Projectile.position);

                        Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * BulletSpeed;
                        Vector2 fireVec = shootVelocity.RotatedByRandom(MathHelper.ToRadians(2f));
                        Projectile.NewProjectile(
                            Projectile.GetSource_FromThis(),
                            GunTipPosition,
                            fireVec,
                            ModContent.ProjectileType<NovaShot>(),
                            Projectile.damage,
                            Projectile.knockBack,
                            Projectile.owner
                        );

                        ShotsLoaded--;
                        ShootRecoilTimer = 16f;
                        OffsetLengthFromArm -= 5f;
                    }
                }


                // 当切换阶段或重置时，清除标志
                if (!ChargeLV1)
                {
                    hasFiredBigShot = false; // 重置标志
                }
            }
            else
            {
                // Loads shots until maxed out
                if (ShotsLoaded < MaxLoadableShots && CurrentChargingFrames % FramesPerLoad == 0)
                    ShotsLoaded++;

                // 增加蓄力帧数
                if (ChargeLV1)
                    CurrentChargingFrames += 2; // 第一阶段蓄力之后加速
                else
                    CurrentChargingFrames++; // 正常增加蓄力帧数

                // 声音逻辑
                if (ChargeLV1)
                {
                    // 第二阶段蓄力声音
                    if (CurrentChargingFrames == ArcNovaDiffuser.Charge2Frames)
                        SoundEngine.PlaySound(ArcNovaDiffuser.ChargeLV2, Projectile.Center);
                    // 第一阶段蓄力声音
                    else if (CurrentChargingFrames == ArcNovaDiffuser.Charge1Frames)
                    {
                        SoundEngine.PlaySound(ArcNovaDiffuser.ChargeLV1, Projectile.Center);
                        ShotsLoaded = MaxLoadableShots; // 第一阶段蓄力完成后弹药装满
                    }

                    // 循环蓄力声音
                    if ((CurrentChargingFrames - ArcNovaDiffuser.Charge1Frames) % (DiffuseNovaArc.ChargeLoopSoundFrames * 2) == 0)
                        NovaChargeSlot = SoundEngine.PlaySound(ArcNovaDiffuser.ChargeLoop, Projectile.Center);
                }
                else if (CurrentChargingFrames == 10)
                    NovaChargeSlot = SoundEngine.PlaySound(ArcNovaDiffuser.ChargeStart, Projectile.Center);

                // 蓄力视觉效果
                if (CurrentChargingFrames >= 10)
                {
                    float particleScale = MathHelper.Clamp(CurrentChargingFrames, 0f, ArcNovaDiffuser.Charge2Frames);
                    for (int i = 0; i < (ChargeLV2 ? 4 : ChargeLV1 ? 3 : 2); i++)
                    {
                        SparkParticle spark2 = new SparkParticle(
                            (GunTipPosition - Projectile.velocity * 4) + Main.rand.NextVector2Circular(12, 12),
                            -Projectile.velocity * Main.rand.NextFloat(16.1f, 30.8f),
                            false,
                            Main.rand.Next(2, 7),
                            Main.rand.NextFloat(particleScale / 350f, particleScale / 270f),
                            Main.rand.NextBool(4) ? Color.Chartreuse : Color.Lime
                        );
                        GeneralParticleHandler.SpawnParticle(spark2);
                    }
                    Particle orb = new GenericBloom(GunTipPosition, Projectile.velocity, Color.Lime, particleScale / 270f, 2, false);
                    GeneralParticleHandler.SpawnParticle(orb);
                    Particle orb2 = new GenericBloom(GunTipPosition, Projectile.velocity, Color.White, particleScale / 400f, 2, false);
                    GeneralParticleHandler.SpawnParticle(orb2);

                    float strength = particleScale / 45f;
                    Vector3 DustLight = new Vector3(0.000f, 0.255f, 0.000f);
                    Lighting.AddLight(GunTipPosition, DustLight * strength);
                }

                // 第一阶段满蓄力特效
                if (CurrentChargingFrames == ArcNovaDiffuser.Charge1Frames)
                {
                    for (int i = 0; i < 36; i++)
                    {
                        Dust chargefull = Dust.NewDustPerfect(GunTipPosition, 107);
                        chargefull.velocity = (MathHelper.TwoPi * i / 36f).ToRotationVector2() * 8f + Owner.velocity;
                        chargefull.scale = Main.rand.NextFloat(1f, 1.3f);
                        chargefull.noGravity = true;
                    }
                }

                // 第二阶段满蓄力特效
                if (CurrentChargingFrames == ArcNovaDiffuser.Charge2Frames)
                {
                    for (int i = 0; i < 45; i++)
                    {
                        Dust chargefull = Dust.NewDustPerfect(GunTipPosition, 107);
                        chargefull.velocity = (MathHelper.TwoPi * i / 36f).ToRotationVector2() * 12f + Owner.velocity;
                        chargefull.scale = Main.rand.NextFloat(1.2f, 1.4f);
                        chargefull.noGravity = true;
                    }
                }
            }




        }

        public override void OnKill(int timeLeft)
        {
            if (SoundEngine.TryGetActiveSound(NovaChargeSlot, out var ChargeSound))
                ChargeSound?.Stop();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f);
            Vector2 rotationPoint = texture.Size() * 0.5f;
            SpriteEffects flipSprite = (Projectile.spriteDirection * Owner.gravDir == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            if (!Owner.CantUseHoldout())
            {
                float rumble = MathHelper.Clamp(CurrentChargingFrames, 0f, ArcNovaDiffuser.Charge2Frames);
                drawPosition += Main.rand.NextVector2Circular(rumble / 70f, rumble / 70f);
            }

            Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(lightColor), drawRotation, rotationPoint, Projectile.scale * Owner.gravDir, flipSprite);

            return false;
        }
    }
}
