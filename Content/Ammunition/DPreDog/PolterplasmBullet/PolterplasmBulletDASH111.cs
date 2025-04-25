//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using CalamityMod.CalPlayer.Dashes;
//using CalamityMod.Enums;
//using CalamityMod.Particles;
//using Microsoft.Xna.Framework;
//using Terraria;
//using Terraria.DataStructures;
//using CalamityMod.CalPlayer.Dashes;
//using CalamityMod.Enums;
//using CalamityMod.Particles;
//using Microsoft.Xna.Framework;
//using Terraria;
//using Terraria.DataStructures;
//using CalamityMod.Buffs.DamageOverTime;
//using CalamityMod.Items.Accessories;
//using CalamityMod.Projectiles.Typeless;
//using CalamityMod;
//using Terraria.Graphics.Shaders;
//using Terraria.ModLoader;

//namespace CalamityRangerExpansion.Content.Ammunition.DPreDog.PolterplasmBullet
//{
//    public class PolterplasmBulletDASH : PlayerDashEffect
//    {
//        public static new string ID => "Polterplasm Dash"; // 唯一冲刺标识符

//        public override DashCollisionType CollisionType => DashCollisionType.ShieldSlam;

//        public override bool IsOmnidirectional => false;

//        public override float CalculateDashSpeed(Player player) => 21.5f;
//        public bool PostHit = false;

//        public override void OnDashEffects(Player player)
//        {
//            PostHit = false;

//            // 粉色粒子特效
//            for (int d = 0; d < 4; d++)
//            {
//                Dust pinkDust = Dust.NewDustPerfect(
//                    player.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-15f, 15f)),
//                    173, // Pink Dust
//                    -player.velocity.RotatedByRandom(MathHelper.ToRadians(10f)) * Main.rand.NextFloat(0.1f, 0.8f),
//                    0,
//                    default,
//                    Main.rand.NextFloat(1.8f, 2.8f)
//                );
//                pinkDust.noGravity = true;
//            }
//        }

//        public override void MidDashEffects(Player player, ref float dashSpeed, ref float dashSpeedDecelerationFactor, ref float runSpeedDecelerationFactor)
//        {
//            for (int d = 0; d < 4; d++)
//            {
//                Dust hFlameDust = Dust.NewDustPerfect(player.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), Main.rand.NextFloat(-15f, 15f)) - (player.velocity * 1.2f), Main.rand.NextBool(8) ? 222 : 162, -player.velocity.RotatedByRandom(MathHelper.ToRadians(10f)) * Main.rand.NextFloat(0.1f, 0.8f), 0, default, Main.rand.NextFloat(1.8f, 2.8f));
//                hFlameDust.shader = GameShaders.Armor.GetSecondaryShader(player.cShield, player);
//                hFlameDust.noGravity = hFlameDust.type == 222 ? false : true;
//                hFlameDust.fadeIn = 0.5f;
//                if (hFlameDust.type == 222)
//                {
//                    hFlameDust.scale = Main.rand.NextFloat(0.8f, 1.2f);
//                    hFlameDust.velocity += new Vector2(0, -2.5f) * Main.rand.NextFloat(0.8f, 1.2f);
//                }
//                if (hFlameDust.type == 180)
//                    hFlameDust.scale = Main.rand.NextFloat(1.6f, 2.2f);

//                Dust dust = Dust.NewDustPerfect(player.Center + Main.rand.NextVector2Circular(6, 6) - player.velocity * 2, 228);
//                dust.velocity = -player.velocity * Main.rand.NextFloat(0.6f, 1.4f);
//                dust.scale = Main.rand.NextFloat(0.9f, 1.4f);
//                dust.noGravity = true;
//            }
//            // Dash at a faster speed than the default value.
//            dashSpeed = 14f;
//        }

//        public override void OnHitEffects(Player player, NPC npc, IEntitySource source, ref DashHitContext hitContext)
//        {
//            if (!PostHit)
//            {
//                player.Calamity().GeneralScreenShakePower = 3.5f;
//                PostHit = true;
//            }
//            float particleScale = Main.rand.NextFloat(0.45f, 0.55f);
//            Particle explosion = new DetailedExplosion(npc.Center, Vector2.Zero, Color.Gray * 0.6f, Vector2.One, Main.rand.NextFloat(-5, 5), 0f, particleScale + 0.07f, 20, false);
//            GeneralParticleHandler.SpawnParticle(explosion);
//            Particle explosion2 = new DetailedExplosion(npc.Center, Vector2.Zero, Color.Orange, Vector2.One, Main.rand.NextFloat(-5, 5), 0f, particleScale, 20);
//            GeneralParticleHandler.SpawnParticle(explosion2);

//            // Define hit context variables.
//            int hitDirection = player.direction;
//            if (player.velocity.X != 0f)
//                hitDirection = Math.Sign(player.velocity.X);
//            hitContext.HitDirection = hitDirection;
//            hitContext.PlayerImmunityFrames = ElysianAegis.ShieldSlamIFrames;

//            // Define damage parameters.
//            int dashDamage = ElysianAegis.ShieldSlamDamage;
//            hitContext.damageClass = DamageClass.Melee;
//            hitContext.BaseDamage = player.ApplyArmorAccDamageBonusesTo(dashDamage);
//            hitContext.BaseKnockback = ElysianAegis.ShieldSlamKnockback;

//            // On-hit Supreme Holy Explosion
//            int supremeExplosionDamage = (int)player.GetBestClassDamage().ApplyTo(ElysianAegis.RamExplosionDamage);
//            supremeExplosionDamage = player.ApplyArmorAccDamageBonusesTo(supremeExplosionDamage);
//            Projectile.NewProjectile(source, player.Center, Vector2.Zero, ModContent.ProjectileType<HolyExplosionSupreme>(), supremeExplosionDamage, ElysianAegis.RamExplosionKnockback, Main.myPlayer, 1f, 0f);
//            npc.AddBuff(ModContent.BuffType<HolyFlames>(), 300);
//        }
//    }

//}
