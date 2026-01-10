namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.TenderElegy
{
    internal class TenderElegyBeilDamage : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.TenderElegy";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 25;            
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
         
        }

        public override void AI()
        {

        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
