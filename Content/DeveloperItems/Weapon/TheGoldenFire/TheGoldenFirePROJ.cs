using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.World;
using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.Particles;

namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.TheGoldenFire
{
    public class TheGoldenFirePROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.TheGoldenFire";
        public override string Texture => "CalamityMod/Projectiles/FireProj";

        public static int Lifetime => 96;
        public static int Fadetime => 80;
        public ref float Time => ref Projectile.ai[0];
        public int MistType = -1;

        // 添加接收的颜色信息
        public Color FireColor { get; set; } = Color.White;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = 4;
            Projectile.timeLeft = Lifetime; // 24 effectively

            if (true)
            {
                Projectile.usesIDStaticNPCImmunity = true;
                Projectile.idStaticNPCHitCooldown = 8;
            }
            else
            {
                Projectile.usesLocalNPCImmunity = true;
                Projectile.localNPCHitCooldown = 8;
            }

        }

        public override void AI()
        {
            Time++;

            if (MistType == -1)
                MistType = Main.rand.Next(3);

            if (Time > Fadetime)
                Projectile.velocity *= 0.95f;

            if (Time > 6f && Time < Fadetime)
            {
                if (Main.rand.NextBool(16))
                {
                    Dust dust = Dust.NewDustDirect(Projectile.Center + Main.rand.NextVector2Circular(60f, 60f) * Utils.Remap(Time, 0f, Fadetime, 0.5f, 1f), 4, 4, DustID.CorruptTorch, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100);
                    if (Main.rand.NextBool(5))
                    {
                        dust.noGravity = true;
                        dust.scale *= 2f;
                        dust.velocity *= 0.8f;
                    }
                    dust.velocity *= 1.1f;
                    dust.velocity += Projectile.velocity * Utils.Remap(Time, 0f, Fadetime * 0.75f, 1f, 0.1f) * Utils.Remap(Time, 0f, Fadetime * 0.1f, 0.1f, 1f);
                }

                if (Main.rand.NextBool(17))
                {
                    bool LowVel = Main.rand.NextBool() ? false : true;
                    FlameParticle fire = new FlameParticle(Projectile.Center, 20, MathHelper.Clamp(Time * 0.05f, 0.15f, 1.75f), 0.05f, Color.Gold * (LowVel ? 1.2f : 0.5f), Color.DarkGoldenrod * (LowVel ? 1.2f : 0.5f));
                    fire.Velocity = new Vector2(Projectile.velocity.X * 0.8f, -10).RotatedByRandom(0.005f) * (LowVel ? Main.rand.NextFloat(0.4f, 0.65f) : Main.rand.NextFloat(0.8f, 1f));
                    GeneralParticleHandler.SpawnParticle(fire);
                }
            }
            else if (Time == 5f)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(3) ? 295 : 181, Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(30f)) * Main.rand.NextFloat(0.5f, 1f));
                dust.scale = Main.rand.NextFloat(0.8f, 1.8f);
                dust.noGravity = true;
                dust.fadeIn = 0.5f;
            }

            
        }
        #region 旧代码
        // Expanding hitbox
        //public override void ModifyDamageHitbox(ref Rectangle hitbox)
        //{
        //    int size = (int)Utils.Remap(Time, 0f, Fadetime, 10f, 40f);

        //    // Shrinks again after fading
        //    if (Time > Fadetime)
        //        size = (int)Utils.Remap(Time, Fadetime, Lifetime, 40f, 0f);
        //    hitbox.Inflate(size, size);
        //}

        //public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 360);
        //public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        //{
        //    if (Projectile.numHits > 0)
        //        Projectile.damage = (int)(Projectile.damage * 0.75f);
        //    if (Projectile.damage < 1)
        //        Projectile.damage = 1;
        //}
        #endregion

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D fire = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Texture2D mist = ModContent.Request<Texture2D>("CalamityMod/Particles/MediumMist").Value;

            float length = ((Time > Fadetime - 10f) ? 0.1f : 0.15f);
            float vOffset = Math.Min(Time, 20f);
            float timeRatio = Utils.GetLerpValue(0f, Lifetime, Time);
            float fireSize = Utils.Remap(timeRatio, 0.2f, 0.5f, 0.25f, 1f);

            if (timeRatio >= 1f)
                return false;

            for (float j = 1f; j >= 0f; j -= length)
            {
                // 使用传递的 FireColor 替代动态颜色
                Color fireColor = FireColor * (1f - j) * Utils.GetLerpValue(0f, 0.2f, timeRatio, true);

                // Positions and rotations
                Vector2 firePos = Projectile.Center - Main.screenPosition - Projectile.velocity * vOffset * j;
                float mainRot = (-j * MathHelper.PiOver2 - Main.GlobalTimeWrappedHourly * (j + 1f) * 2f / length) * Math.Sign(Projectile.velocity.X);
                float trailRot = MathHelper.PiOver4 - mainRot;

                // Draw one backtrail
                Vector2 trailOffset = Projectile.velocity * vOffset * length * 0.5f;
                Main.EntitySpriteDraw(fire, firePos - trailOffset, null, fireColor * 0.25f, trailRot, fire.Size() * 0.5f, fireSize, SpriteEffects.None);

                // Draw the main fire
                Main.EntitySpriteDraw(fire, firePos, null, fireColor, mainRot, fire.Size() * 0.5f, fireSize, SpriteEffects.None);

                // Draw the masking smoke
                if (MistType > 2 || MistType < 0)
                    return false;
                Main.spriteBatch.SetBlendState(BlendState.Additive);
                Rectangle frame = mist.Frame(1, 3, 0, MistType);
                Main.EntitySpriteDraw(mist, firePos, frame, Color.Lerp(fireColor, Color.White, 0.3f), mainRot, frame.Size() * 0.5f, fireSize, SpriteEffects.None);
                Main.EntitySpriteDraw(mist, firePos, frame, fireColor, mainRot, frame.Size() * 0.5f, fireSize * 3f, SpriteEffects.None);
                Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
            }
            return false;
        }
        #region 旧代码
        //public override bool PreDraw(ref Color lightColor)
        //{
        //    Texture2D fire = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
        //    Texture2D mist = ModContent.Request<Texture2D>("CalamityMod/Particles/MediumMist").Value;

        //    // 从 localAI 和 ai 读取颜色值
        //    Color gelColor = new Color((int)Projectile.localAI[0], (int)Projectile.localAI[1], (int)Projectile.ai[0]);

        //    float length = ((Time > Fadetime - 10f) ? 0.1f : 0.15f);
        //    float vOffset = Math.Min(Time, 20f);
        //    float timeRatio = Utils.GetLerpValue(0f, Lifetime, Time);
        //    float fireSize = Utils.Remap(timeRatio, 0.2f, 0.5f, 0.25f, 1f);

        //    if (timeRatio >= 1f)
        //        return false;

        //    for (float j = 1f; j >= 0f; j -= length)
        //    {
        //        // 动态颜色
        //        Color fireColor = gelColor * (1f - j) * Utils.GetLerpValue(0f, 0.2f, timeRatio, true);

        //        // 位置与旋转
        //        Vector2 firePos = Projectile.Center - Main.screenPosition - Projectile.velocity * vOffset * j;
        //        float mainRot = (-j * MathHelper.PiOver2 - Main.GlobalTimeWrappedHourly * (j + 1f) * 2f / length) * Math.Sign(Projectile.velocity.X);
        //        float trailRot = MathHelper.PiOver4 - mainRot;

        //        // 绘制拖尾
        //        Vector2 trailOffset = Projectile.velocity * vOffset * length * 0.5f;
        //        Main.EntitySpriteDraw(fire, firePos - trailOffset, null, fireColor * 0.25f, trailRot, fire.Size() * 0.5f, fireSize, SpriteEffects.None);

        //        // 绘制主火焰
        //        Main.EntitySpriteDraw(fire, firePos, null, fireColor, mainRot, fire.Size() * 0.5f, fireSize, SpriteEffects.None);

        //        // 绘制烟雾
        //        if (MistType > 2 || MistType < 0)
        //            return false;

        //        Main.spriteBatch.SetBlendState(BlendState.Additive);
        //        Rectangle frame = mist.Frame(1, 3, 0, MistType);
        //        Main.EntitySpriteDraw(mist, firePos, frame, Color.Lerp(fireColor, Color.White, 0.3f), mainRot, frame.Size() * 0.5f, fireSize, SpriteEffects.None);
        //        Main.EntitySpriteDraw(mist, firePos, frame, fireColor, mainRot, frame.Size() * 0.5f, fireSize * 3f, SpriteEffects.None);
        //        Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);
        //    }

        //    return false;
        //}
        #endregion

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.damage /= 5;
            Projectile.damage *= 3;
            // 0.6倍即最多2.5倍原伤害
        }
    }
}
