using CalamityMod;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;

namespace CalamityRangerExpansion.Content.BOWChange.APreHardMode.ToxibowC
{
    internal class ToxibowReEXP : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.APreHardMode";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            if (Main.getGoodWorld)
            {
                Projectile.width = 350;
                Projectile.height = 350;
            }
            else
            {
                Projectile.width = 50;
                Projectile.height = 50;
            }
            Projectile.friendly = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            target.AddBuff(BuffID.Venom, 300);   
        }

    }
}