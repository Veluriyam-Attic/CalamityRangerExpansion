namespace CalamityRangerExpansion.Content.Accessories
{
    public class DragonFangs : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 54;
            Item.accessory = true;
            Item.rare = ItemRarityID.Master;
            Item.value = Item.sellPrice(1, 0, 0, 0);
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(1)
            .AddIngredient<AuricBar>(5)
            .AddIngredient<YharonSoulFragment>(3)
            .AddIngredient<ReaperToothNecklace>(1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetArmorPenetration<GenericDamageClass>() += 100f;
            VeluriyamPlayer.OnHitNPCEvent += AddBuff;
        }

        private void AddBuff(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<Dragonfire>(), 120, false);
        }
    }
}