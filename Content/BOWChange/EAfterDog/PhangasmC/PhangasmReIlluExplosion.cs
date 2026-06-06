using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework;

namespace CalamityRangerExpansion.Content.BOWChange.EAfterDog.PhangasmC
{
    internal class PhangasmReIlluExplosion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.EAfterDog";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj"; // 使用完全透明贴图

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 400; // 范围型爆炸判定大小
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 15; // 存活时间短
            Projectile.extraUpdates = 1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 600; // 防止重复命中
            Projectile.alpha = 255; // 完全透明
        }

        public override void AI()
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile other = Main.projectile[i];

                // 检测：其他玩家投射物 + 活动中 + 不等于自身
                if (other.active && other.owner == Projectile.owner && other.whoAmI != Projectile.whoAmI)
                {
                    if (other.friendly && other.DamageType == DamageClass.Ranged && other.Hitbox.Intersects(Projectile.Hitbox))
                    {
                        // 增加飞行速度
                        Projectile.velocity *= 1.25f;

                        // 播放音效（加速感）
                        SoundEngine.PlaySound(SoundID.Item20 with { Volume = 0.75f }, Projectile.Center);

                        // 爆发 Dust 特效（青绿/白）
                        for (int d = 0; d < 6; d++)
                        {
                            Dust dust = Dust.NewDustPerfect(
                                Projectile.Center + Main.rand.NextVector2Circular(12f, 12f),
                                Main.rand.NextBool() ? 226 : 272,
                                Main.rand.NextVector2Circular(2f, 2f),
                                100,
                                Color.Lerp(Color.Cyan, Color.LightGreen, Main.rand.NextFloat()),
                                Main.rand.NextFloat(0.6f, 1f)
                            );
                            dust.noGravity = true;
                        }

                        break; // 防止多次叠加，仅触发一次
                    }
                }
            }
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 命中 NPC 时触发，可添加爆炸特效、附加 Buff 等
        }

        public override void OnKill(int timeLeft)
        {
            // 弹幕消失时触发，可用于播放音效、生成爆炸粒子等
        }
    }
}
