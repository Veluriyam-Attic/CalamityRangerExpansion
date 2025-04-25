using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.Audio;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;

namespace CalamityRangerExpansion.Content.Arrows.EAfterDog.AuricArrow
{
    public class AuricArrowNPC : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.NewWeapons.EAfterDog";
        private bool hasDashed = false; // 是否已冲刺
        private int dashStartDelay = 35; // 开始冲刺的延迟
        private NPC targetNPC; // 追踪的目标敌人
        public ref float Time => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 66;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.scale = 1.5f;
            Projectile.ignoreWater = true;
            Projectile.alpha = 50;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 280;

            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 10; // 无敌帧冷却时间为30帧
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
                Projectile.frame = 0;

            Lighting.AddLight(Projectile.Center, 0.5f, 0.25f, 0f);

            // 缓慢飞行并逐渐减速
            if (!hasDashed)
            {
                Projectile.velocity *= 0.98f;
            }

            // 生成不受重力影响的火焰粒子，分布在弹幕体积内
            if (Main.rand.NextBool(3))
            {
                Vector2 randomOffset = new Vector2(Main.rand.NextFloat(-Projectile.width / 2, Projectile.width / 2), Main.rand.NextFloat(-Projectile.height / 2, Projectile.height / 2));
                Dust dust = Dust.NewDustPerfect(Projectile.Center + randomOffset, DustID.OrangeTorch, Projectile.velocity * 0.3f, 0, Color.OrangeRed, 1.2f);
                dust.noGravity = true;
            }


            // 计数飞行的帧数
            Projectile.ai[0]++;

            // 在飞行第60帧时寻找并锁定敌人
            if (Projectile.ai[0] >= dashStartDelay && !hasDashed)
            {
                targetNPC = FindTarget(3200f); // 寻找最近的敌人
                if (targetNPC != null)
                {
                    hasDashed = true;
                }
            }

            // 持续追踪目标
            if (hasDashed && targetNPC != null && targetNPC.active)
            {
                Vector2 direction = Vector2.Normalize(targetNPC.Center - Projectile.Center);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * 35f, 0.1f); // 持续追踪
            }

            Time++;
        }


        public override bool? CanDamage() => Time >= 12f; // 初始的时候不会造成伤害，直到12为止


        // 寻找最近的敌人
        private NPC FindTarget(float range)
        {
            NPC closestNPC = null;
            float closestDistance = range;

            foreach (NPC npc in Main.npc)
            {
                if (npc.CanBeChasedBy(Projectile) && Projectile.Distance(npc.Center) < closestDistance)
                {
                    closestDistance = Projectile.Distance(npc.Center);
                    closestNPC = npc;
                }
            }

            return closestNPC;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/Yharon/YharonInfernado"));
            float rotation = MathHelper.TwoPi / 3;
            for (int i = 0; i < 3; i++)
            {
                Vector2 direction = Vector2.UnitY.RotatedBy(rotation * i);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, direction * 10f, ModContent.ProjectileType<AuricArrowPROJ>(), Projectile.damage, 0, Main.myPlayer);
            }

            float rotationStep = MathHelper.TwoPi / 5; // 每两个弹幕之间的夹角
            for (int i = 0; i < 5; i++)
            {
                Vector2 direction = Vector2.UnitY.RotatedBy(rotationStep * i); // 计算方向
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    direction * 15f, // 设置速度为
                    ModContent.ProjectileType<DropThing>(),
                    1, // 设置伤害为1
                    0,
                    Main.myPlayer
                );
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Dragonfire>(), 300); // 龙焰
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, 16f * Projectile.scale, targetHitbox);

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int framing = texture.Height / Main.projFrames[Projectile.type];
            int y6 = framing * Projectile.frame;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle(0, y6, texture.Width, framing), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, framing / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

    }
}
