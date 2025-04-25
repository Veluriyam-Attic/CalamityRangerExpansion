using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace CalamityRangerExpansion.Content.Ammunition.DPreDog.EffulgentFeatherBullet
{
    internal class EffulgentFeatherBulletAREA : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectile.DPreDog";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 200;
            Projectile.friendly = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 100;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
           
        }

        public override void AI()
        {
            // 中心始终与玩家位置对齐
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.Center;

            // 如果玩家没有 Buff，销毁自己
            if (!player.HasBuff(ModContent.BuffType<EffulgentFeatherBulletPBuff>()))
            {
                Projectile.Kill();
                return;
            }

            // 每帧重置存在时间
            Projectile.timeLeft = 2;

            // 穿透 -1 次
            Projectile.penetrate = -1;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Electrified, 180);
            target.AddBuff(ModContent.BuffType<GalvanicCorrosion>(), 6);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
