using VeluriyamLib.Content.BaseClass;

namespace CalamityRangerExpansion.Content.Accessories
{
    public class CthulhusPupil : VeluriyamItem
    {
        public override void SetDefaults()
        {
            Item.width = Item.height = 50;
            Item.accessory = true;
            Item.rare = ItemRarityID.Master;
            Item.value = Item.sellPrice(0, 0, 50, 0);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.ammoCost75 = true;
            player.GetDamage<RangedDamageClass>() += 0.04f;
        }
    }

    public class AddLoot : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ItemID.EyeOfCthulhuBossBag && lateInstantiation;

        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {itemLoot.Add(ItemDropRule.NotScalingWithLuck(ModContent.ItemType<CthulhusPupil>(), 5,1,1));}
    }
}
