namespace CalamityRangerExpansion.Content.DonorItem.StarDustBullet 
{ 
    public class STAR : ModItem , ILocalizedModType
    {
        public new string LocalizationCategory => "DonorItem";

        public override void SetDefaults()
        {
            Item.ammo = AmmoID.Bullet;
            Item.DamageType = DamageClass.Ranged;
            Item.rare = ItemRarityID.Master;
            Item.value = Item.buyPrice(0, 0, 1, 0);
            Item.value = Item.sellPrice(0, 0, 0, 75);
            Item.maxStack = 9999;
            Item.damage = 10;
            Item.height = 36;
            Item.width = 26;
            Item.shoot = ModContent.ProjectileType<STAROnSpawn>();
            Item.shootSpeed = 1.1f;
            Item.Calamity().donorItem = true;
        }
    }
}
