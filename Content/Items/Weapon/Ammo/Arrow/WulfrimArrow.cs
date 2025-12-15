

namespace CalamityRangerExpansion.Content.Items.Weapon.Ammo.Arrow
{
    public class WulfrimArrow : VeluriyamItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 54;
            Item.damage = 8;
            Item.DamageType = DamageClass.Ranged;
            Item.ammo = AmmoID.Arrow;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.shootSpeed = 16f;
            Item.knockBack = 3f;
            Item.value = Item.sellPrice(copper: 12);
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<WulfrimArrowProjectile>();
        }
    }

    public class WulfrimArrowProjectile : VeluriyamProjectile
    {
        public override string Texture => "CalamityRangerExpansion/Content/Items/Weapon/Ammo/Arrow/WulfrimArrow";

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 54;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
            Projectile.scale = 0.7f;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;
            
        }
    }
}