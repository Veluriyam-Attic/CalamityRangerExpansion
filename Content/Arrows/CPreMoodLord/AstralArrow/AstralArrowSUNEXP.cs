//using CalamityMod.Buffs.DamageOverTime;
//using CalamityMod.Buffs.StatDebuffs;
//using CalamityRangerExpansion.Content.DeveloperItems.Bullet.DestructionBullet;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Terraria.ID;
//using Terraria.ModLoader;
//using Terraria;
//using Microsoft.Xna.Framework;

//namespace CalamityRangerExpansion.Content.Arrows.CPreMoodLord.AstralArrow
//{
//    internal class AstralArrowSUNEXP : ModProjectile, ILocalizedModType
//    {
//        public new string LocalizationCategory => "DeveloperItems.DestructionBullet";
//        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

//        public override void SetDefaults()
//        {
//            //if (Main.getGoodWorld)
//            //{
//            //    Projectile.width = 3500;
//            //    Projectile.height = 3500;
//            //}
//            //else
//            {
//                Projectile.width = 160;
//                Projectile.height = 160;
//            }
//            Projectile.friendly = true;
//            Projectile.ignoreWater = false;
//            Projectile.tileCollide = false;
//            Projectile.penetrate = -1;
//            Projectile.timeLeft = 2;
//            Projectile.DamageType = DamageClass.Ranged;
//            Projectile.usesLocalNPCImmunity = true;
//            Projectile.localNPCHitCooldown = -1;
//        }
//        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
//        {
     
//        }

//        public override void AI()
//        {

//        }

//        public override bool PreDraw(ref Color lightColor)
//        {
//            return false;
//        }
//    }
//}
