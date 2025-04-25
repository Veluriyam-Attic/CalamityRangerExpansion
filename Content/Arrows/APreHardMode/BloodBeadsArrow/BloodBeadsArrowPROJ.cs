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
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Ranged;
using Terraria.DataStructures;
using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Particles;
using CalamityRangerExpansion.CREConfigs;

namespace CalamityRangerExpansion.Content.Arrows.APreHardMode.BloodBeadsArrow
{
    public class BloodBeadsArrowPROJ : ModProjectile, ILocalizedModType
    {
        public override string Texture => "CalamityRangerExpansion/Content/Arrows/APreHardMode/BloodBeadsArrow/BloodBeadsArrow";

        public new string LocalizationCategory => "Projectile.APreHardMode";
        //public bool splitShot = false;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            #region 旧代码

            //if (splitShot)
            //{
            //    // 使用与 CinderArrowProj 类似的击中后状态的绘制逻辑
            //    Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Particles/DrainLine").Value;
            //    CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], Color.Red * 0.3f, 1, texture);
            //    return false;
            //}
            //else

            #endregion

            {
                // 普通的残影效果
                Texture2D texture = ModContent.Request<Texture2D>("CalamityRangerExpansion/Content/Arrows/APreHardMode/BloodBeadsArrow/BloodBeadsArrow").Value;
                if (Projectile.timeLeft > 300)
                    CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], Color.White * 0.3f, 1, texture);
                return true;
            }
        }

        public override void SetDefaults()
        {
            Projectile.width = 11;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 4;
            Projectile.timeLeft = 600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;
            Projectile.ignoreWater = true;
            Projectile.arrow = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            // 弹幕的旋转
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 添加红色的类似 ShadeFire 的粒子效果
                if (Main.rand.NextBool(7))
                {
                    Color smokeColor = Color.Lerp(Color.Red, Color.Black, 0.7f);
                    Particle smoke = new HeavySmokeParticle(Projectile.Center, Projectile.velocity * 0.5f, smokeColor, 20, Projectile.scale * Main.rand.NextFloat(0.6f, 1.2f), 0.3f, MathHelper.ToRadians(3f), required: true);
                    GeneralParticleHandler.SpawnParticle(smoke);
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 在消失时生成更深红色的烟雾效果
                int Dusts = 15;
                float radians = MathHelper.TwoPi / Dusts;
                Vector2 spinningPoint = Vector2.Normalize(new Vector2(-1f, -1f));
                for (int i = 0; i < Dusts; i++)
                {
                    Vector2 dustVelocity = spinningPoint.RotatedBy(radians * i).RotatedBy(0.5f) * 6.5f;
                    Color deepRed = new Color(139, 0, 0); // 深红色
                    Particle smoke = new HeavySmokeParticle(Projectile.Center, dustVelocity * Main.rand.NextFloat(1f, 2.6f), deepRed, 18, Main.rand.NextFloat(0.9f, 1.6f), 0.35f, Main.rand.NextFloat(-1, 1), true);
                    GeneralParticleHandler.SpawnParticle(smoke);
                }
            }

            #region 旧注释

            //// 模仿 CinderArrowProj 生成三个受重力影响的弹幕
            //if (Main.myPlayer == Projectile.owner)
            //{
            //    for (int b = 0; b < 3; b++)
            //    {
            //        Vector2 velocity = Vector2.UnitY.RotatedByRandom(0.8f) * Main.rand.NextFloat(-3.5f, -3f);
            //        Projectile shrapnel = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<BloodBeadsArrowPROJ>(), (int)(Projectile.damage * 0.06f), 0f, Projectile.owner, ai2: 1f);
            //        shrapnel.timeLeft = 300;
            //        shrapnel.arrow = false;
            //        shrapnel.MaxUpdates = 4;
            //        shrapnel.friendly = true;
            //        shrapnel.ignoreWater = true;
            //        shrapnel.scale = 0.8f; // 小弹幕的缩放
            //    }
            //}

            #endregion
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // splitShot = true;  // 进入splitShot状态，启用特殊的PreDraw效果

            // 通过BloodBeadsArrowPROJ触发BloodBeadsArrowPlayer的ApplyDebuffs方法
            BloodBeadsArrowPlayer bloodBeadsPlayer = Main.player[Projectile.owner].GetModPlayer<BloodBeadsArrowPlayer>();
            if (bloodBeadsPlayer != null)
            {
                bloodBeadsPlayer.ApplyDebuffs(target, 120); // 每个debuff持续2秒（120帧）
            }
        }
    }
}