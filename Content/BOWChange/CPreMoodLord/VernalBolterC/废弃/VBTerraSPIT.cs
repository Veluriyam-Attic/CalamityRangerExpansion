//using CalamityMod;
//using Microsoft.Xna.Framework;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Terraria.ID;
//using Terraria.ModLoader;
//using Terraria;
//using Terraria.Audio;

//namespace CalamityRangerExpansion.Content.BOWChange.CPreMoodLord.VernalBolterC.废弃
//{
//    internal class VBTerraSPIT : ModProjectile, ILocalizedModType
//    {
//        public new string LocalizationCategory => "Weapons.CPreMoodLord";
//        public override string Texture => "CalamityRangerExpansion/Content/BOWChange/CPreMoodLord/VernalBolterC/VBTerra";

//        public override void SetStaticDefaults()
//        {
//            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
//            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
//        }
//        public override bool PreDraw(ref Color lightColor)
//        {
//            // 画残影效果
//            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
//            return false;
//        }
//        public override void SetDefaults()
//        {
//            // 设置弹幕的基础属性
//            Projectile.width = 11; // 弹幕宽度
//            Projectile.height = 24; // 弹幕高度
//            Projectile.friendly = true; // 对敌人有效
//            Projectile.DamageType = DamageClass.Ranged; // 远程伤害类型
//            Projectile.penetrate = 1; // 穿透力为1，击中一个敌人就消失
//            Projectile.timeLeft = 150; // 弹幕存在时间为x帧
//            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
//            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
//            Projectile.ignoreWater = true; // 弹幕不受水影响
//            Projectile.arrow = true;
//            Projectile.extraUpdates = 1;
//        }

//        public override bool? CanHitNPC(NPC target) => Projectile.timeLeft < 90 && target.CanBeChasedBy(Projectile);

//        public override void AI()
//        {
//            Projectile.alpha -= 5;
//            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

//            if (Projectile.timeLeft < 90)
//            {
//                CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 450f, 12f, 20f);
//            }
//        }

//        public override void OnKill(int timeLeft)
//        {
//            SoundEngine.PlaySound(SoundID.Item60, Projectile.position);
//            Projectile.position.X = Projectile.position.X + Projectile.width / 2;
//            Projectile.position.Y = Projectile.position.Y + Projectile.height / 2;
//            Projectile.width = 30;
//            Projectile.height = 30;
//            Projectile.position.X = Projectile.position.X - Projectile.width / 2;
//            Projectile.position.Y = Projectile.position.Y - Projectile.height / 2;
//            for (int i = 0; i < 2; i++)
//            {
//                int terraDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 107, 0f, 0f, 100, default, 2f);
//                if (Main.rand.NextBool(2))
//                {
//                    Main.dust[terraDust].scale = 0.5f;
//                    Main.dust[terraDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
//                }
//            }
//        }

//    }
//}