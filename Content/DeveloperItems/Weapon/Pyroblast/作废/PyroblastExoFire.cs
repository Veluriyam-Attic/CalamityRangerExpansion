//using System;
//using CalamityMod;
//using CalamityMod.Buffs.DamageOverTime;
//using CalamityMod.Graphics.Metaballs;
//using CalamityMod.Items.Weapons.Ranged;
//using CalamityMod.Particles;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using Terraria;
//using Terraria.Audio;
//using Terraria.ModLoader;

//namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.Pyroblast.作废
//{
//    // Photoviscerator left click main projectile (the flamethrower itself)
//    public class PyroblastExoFire : ModProjectile, ILocalizedModType
//    {
//        public new string LocalizationCategory => "DeveloperItems.Pyroblast";
//        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
//        public ref float LightPower => ref Projectile.ai[2];

//        public Color sparkColor;
//        public int Time = 0;
//        public ref int audioCooldown => ref Main.player[Projectile.owner].Calamity().PhotoAudioCooldown;
//        public ref int PhotoTimer => ref Main.player[Projectile.owner].Calamity().PhotoTimer;

//        public override void SetDefaults()
//        {
//            Projectile.width = Projectile.height = 30;
//            Projectile.friendly = true;
//            Projectile.ignoreWater = true;
//            Projectile.tileCollide = false;
//            Projectile.DamageType = DamageClass.Ranged;
//            Projectile.penetrate = -1;
//            Projectile.MaxUpdates = 30;
//            Projectile.timeLeft = 240;
//            Projectile.usesIDStaticNPCImmunity = true;
//            Projectile.idStaticNPCHitCooldown = 6;
//        }

//        public override void AI()
//        {
//            // 设置固定的粒子颜色为橙红色
//            sparkColor = Color.OrangeRed;

//            Time++;
//            // 添加白色光效
//            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.2f);

//            Player Owner = Main.player[Projectile.owner];
//            float targetDist = Vector2.Distance(Owner.Center, Projectile.Center); // 用于屏幕外判定
//            if (targetDist < 1400f)
//            {
//                if (PhotoTimer == 0)
//                    PhotoMetaball3.SpawnParticle(Projectile.Center + Owner.velocity, 42 - Time * 0.165f);
//                if (PhotoTimer == 1)
//                    PhotoMetaball3.SpawnParticle(Projectile.Center + Owner.velocity, 37 - Time * (PhotoTimer == 0 ? 0.165f : 0.088f) - PhotoTimer * 0.2f + (PhotoTimer == 1 ? 20 : 0)); ;

//                PhotoMetaball4.SpawnParticle(Projectile.Center + Owner.velocity, 37 - Time * (PhotoTimer == 0 ? 0.165f : 0.088f) - PhotoTimer * 0.2f + (PhotoTimer == 1 ? 20 : 0));
//            }
//            if (Main.rand.NextBool(35) && targetDist < 1400f && Time > 5)
//            {
//                Dust dust = Dust.NewDustPerfect(Projectile.Center, 263, new Vector2(0, -5).RotatedByRandom(0.05f) * Main.rand.NextFloat(0.3f, 1.6f));
//                dust.noGravity = true;
//                dust.scale = Main.rand.NextFloat(0.3f, 1f);
//                dust.color = sparkColor; // 使用固定的橙红色
//            }
//        }


//        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
//        {
//            //target.AddBuff(ModContent.BuffType<MiracleBlight>(), 300);
//            if (audioCooldown == 0)
//            {
//                //SoundEngine.PlaySound(Photoviscerator.HitSound, target.Center);
//                audioCooldown = 16;
//            }
//        }

//        //public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<MiracleBlight>(), 300);

//    }
//}
