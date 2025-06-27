//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using CalamityMod;
//using CalamityMod.Projectiles.BaseProjectiles;
//using Microsoft.Xna.Framework;
//using Terraria;
//using Terraria.ModLoader;

//namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.Pyroblast.作废
//{
//    public class CorinthPrimeAirburst : BaseMassiveExplosionProjectile, ILocalizedModType, IModType
//    {
//        public new string LocalizationCategory => "DeveloperItems.Pyroblast";

//        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

//        public override int Lifetime => 60;

//        // 调整屏幕震动触发条件
//        public override bool UsesScreenshake => Projectile.damage > 1;

//        // 调整屏幕震动强度
//        public override float GetScreenshakePower(float pulseCompletionRatio)
//        {
//            return CalamityUtils.Convert01To010(pulseCompletionRatio) * 8f; // 强度减半
//        }

//        public override Color GetCurrentExplosionColor(float pulseCompletionRatio)
//        {
//            return Color.Lerp(Color.Blue * 1.6f, Color.Cyan, MathHelper.Clamp(pulseCompletionRatio * 2.2f, 0f, 1f));
//        }

//        public override void SetDefaults()
//        {
//            Projectile.width = Projectile.height = 2;
//            Projectile.friendly = true;
//            Projectile.tileCollide = false;
//            Projectile.penetrate = -1;
//            Projectile.usesLocalNPCImmunity = true;
//            Projectile.localNPCHitCooldown = 10;
//            Projectile.timeLeft = Lifetime;
//            Projectile.DamageType = DamageClass.Ranged;
//        }

//        public override void PostAI()
//        {
//            Lighting.AddLight(Projectile.Center, 0f, 0f, 0.3f);
//        }
//    }
//}
