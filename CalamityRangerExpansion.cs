namespace CalamityRangerExpansion
{
    public class AnyRecipes : ModSystem
    {
        public override void AddRecipeGroups()
        {
            #region 箭矢组
            RecipeGroup AnyArrow = new RecipeGroup(() => Language.GetTextValue("Mods.CalamityRangerExpansion.RecipeGroup.Arrow"), new int[]
            {
                #region 本Mod箭矢
                ModContent.ItemType<LifeAlloyArrow>(),
                ModContent.ItemType<AerialiteArrow>(),
                ModContent.ItemType<PrismArrow>(),
                ModContent.ItemType<WulfrimArrow>(),
                ModContent.ItemType<StarblightSootArrow>(),
                ModContent.ItemType<AstralArrow>(),
                ModContent.ItemType<PerennialArrow>(),
                ModContent.ItemType<Content.Arrows.CPreMoodLord.PlagueArrow.PlagueArrow>(),
                ModContent.ItemType<ScoriaArrow>(),
                ModContent.ItemType<DivineGeodeArrow>(),
                ModContent.ItemType<EffulgentFeatherArrow>(),
                ModContent.ItemType<ToothArrow>(),
                ModContent.ItemType<UelibloomArrow>(),
                ModContent.ItemType<AuricArrow>(),
                ModContent.ItemType<EndothermicEnergyArrow>(),
                ModContent.ItemType<MiracleMatterArrow>(),
                #endregion
                #region 原版箭矢
                ItemID.WoodenArrow,
                ItemID.FlamingArrow,
                ItemID.UnholyArrow,
                ItemID.JestersArrow,
                ItemID.HellfireArrow,
                ItemID.HolyArrow,
                ItemID.CursedArrow,
                ItemID.FrostburnArrow,
                ItemID.ChlorophyteArrow,
                ItemID.IchorArrow,
                ItemID.VenomArrow,
                ItemID.BoneArrow,
                ItemID.MoonlordArrow,
                ItemID.ShimmerArrow,
                #endregion
                #region Calamity箭矢
                ModContent.ItemType<CinderArrow>(),
                ModContent.ItemType<VeriumBolt>(),
                ModContent.ItemType<SproutingArrow>(),
                ModContent.ItemType<IcicleArrow>(),
                ModContent.ItemType<ElysianArrow>(),
                ModContent.ItemType<BloodfireArrow>(),
                ModContent.ItemType<VanquisherArrow>()
                #endregion
            });
            AnyArrow.IconicItemId = ItemID.WoodenArrow;
            RecipeGroup.RegisterGroup("CalamityRangerExpansion:RecipeGroupArrow", AnyArrow);
            #endregion

            #region 子弹组
            RecipeGroup AnyBullet = new RecipeGroup(() => Language.GetTextValue("Mods.CalamityRangerExpansion.RecipeGroup.Bullet"), new int[]
            {
                #region 本Mod子弹
                ModContent.ItemType<AerialiteBullet>(),
                ModContent.ItemType<TinkleshardBullet>(),
                ModContent.ItemType<WulfrimBullet>(),
                ModContent.ItemType<CryonicBullet>(),
                ModContent.ItemType<StarblightSootBullet>(),
                ModContent.ItemType<AstralBullet>(),
                ModContent.ItemType<PerennialBullet>(),
                ModContent.ItemType<PlagueBullet>(),
                ModContent.ItemType<ScoriaBullet>(),
                ModContent.ItemType<DivineGeodeBullet>(),
                ModContent.ItemType<EffulgentFeatherBullet>(),
                ModContent.ItemType<PolterplasmBullet>(),
                ModContent.ItemType<ToothBullet>(),
                ModContent.ItemType<UelibloomBullet>(),
                ModContent.ItemType<AuricBulet>(),
                ModContent.ItemType<EndothermicEnergyBullet>(),
                ModContent.ItemType<MiracleMatterBullet>(),
                #endregion
                #region 原版子弹
                ItemID.MusketBall,
                ItemID.MeteorShot,
                ItemID.SilverBullet,
                ItemID.CrystalBullet,
                ItemID.CursedBullet,
                ItemID.ChlorophyteBullet,
                ItemID.HighVelocityBullet,
                ItemID.IchorBullet,
                ItemID.VenomBullet,
                ItemID.PartyBullet,
                ItemID.NanoBullet,
                ItemID.ExplodingBullet,
                ItemID.GoldenBullet,
                ItemID.MoonlordBullet,
                ItemID.TungstenBullet,
                #endregion
                #region Calamity子弹
                ModContent.ItemType<FlashRound>(),
                ModContent.ItemType<MarksmanRound>(),
                ModContent.ItemType<HallowPointRound>(),
                ModContent.ItemType<DryadsTear>(),
                ModContent.ItemType<HailstormBullet>(),
                ModContent.ItemType<BubonicRound>(),
                ModContent.ItemType<MortarRound>(),
                ModContent.ItemType<RubberMortarRound>(),
                ModContent.ItemType<HyperiusBullet>(),
                ModContent.ItemType<HolyFireBullet>(),
                ModContent.ItemType<BloodfireBullet>(),
                ModContent.ItemType<GodSlayerSlug>()
                #endregion

            });
            AnyBullet.IconicItemId = ItemID.MusketBall;
            RecipeGroup.RegisterGroup("CalamityRangerExpansion:RecipeGroupBullet", AnyBullet);
            #endregion

            #region 凝胶组
            RecipeGroup AnyGel = new RecipeGroup(() => Language.GetTextValue("Mods.CalamityRangerExpansion.RecipeGroup.Gel"), new int[]
            {
                #region 本Mod凝胶
                ModContent.ItemType<AerialiteGel>(),
                ModContent.ItemType<GeliticGel>(),
                ModContent.ItemType<HurricaneGel>(),
                ModContent.ItemType<WulfrimGel>(),
                ModContent.ItemType<CryonicGel>(),
                ModContent.ItemType<StarblightSootGel>(),
                ModContent.ItemType<AstralGel>(),
                ModContent.ItemType<LifeAlloyGel>(),
                ModContent.ItemType<LivingShardGel>(),
                ModContent.ItemType<PerennialGel>(),
                ModContent.ItemType<PlagueGel>(),
                ModContent.ItemType<ScoriaGel>(),
                ModContent.ItemType<BloodstoneCoreGel>(),
                ModContent.ItemType<DivineGeodeGel>(),
                ModContent.ItemType<EffulgentFeatherGel>(),
                ModContent.ItemType<PolterplasmGel>(),
                ModContent.ItemType<ToothGel>(),
                ModContent.ItemType<UelibloomGel>(),
                ModContent.ItemType<UnholyEssenceGel>(),
                ModContent.ItemType<AuricGel>(),
                ModContent.ItemType<CosmosGel>(),
                ModContent.ItemType<EndothermicEnergyGel>(),
                ModContent.ItemType<MiracleMatterGel>(),
                #endregion
                #region 原版凝胶
                ItemID.Gel,
                ItemID.PinkGel,
                #endregion
                #region Calamity凝胶
                ModContent.ItemType<BlightedGel>(),
                ModContent.ItemType<PurifiedGel>()
                #endregion
            });
            AnyGel.IconicItemId = ItemID.Gel;
            RecipeGroup.RegisterGroup("CalamityRangerExpansion:RecipeGroupGel", AnyGel);
            #endregion
        }

    }
}
