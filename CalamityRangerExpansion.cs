#region using太多了，折叠了
using CalamityMod.Items.Ammo;
using CalamityMod.Items.Materials;
using CalamityRangerExpansion.Content.Ammunition.APreHardMode.AerialiteBullet;
using CalamityRangerExpansion.Content.Ammunition.APreHardMode.TinkleshardBullet;
using CalamityRangerExpansion.Content.Ammunition.APreHardMode.WulfrimBullet;
using CalamityRangerExpansion.Content.Ammunition.BPrePlantera.CryonicBullet;
using CalamityRangerExpansion.Content.Ammunition.BPrePlantera.StarblightSootBullet;
using CalamityRangerExpansion.Content.Ammunition.CPreMoodLord.AstralBullet;
using CalamityRangerExpansion.Content.Ammunition.CPreMoodLord.PerennialBullet;
using CalamityRangerExpansion.Content.Ammunition.CPreMoodLord.PlagueBullet;
using CalamityRangerExpansion.Content.Ammunition.CPreMoodLord.ScoriaBullet;
using CalamityRangerExpansion.Content.Ammunition.DPreDog.DivineGeodeBullet;
using CalamityRangerExpansion.Content.Ammunition.DPreDog.EffulgentFeatherBullet;
using CalamityRangerExpansion.Content.Ammunition.DPreDog.PolterplasmBullet;
using CalamityRangerExpansion.Content.Ammunition.DPreDog.ToothBullet;
using CalamityRangerExpansion.Content.Ammunition.DPreDog.UelibloomBullet;
using CalamityRangerExpansion.Content.Ammunition.EAfterDog.AuricBulet;
using CalamityRangerExpansion.Content.Ammunition.EAfterDog.EndothermicEnergyBullet;
using CalamityRangerExpansion.Content.Ammunition.EAfterDog.MiracleMatterBullet;
using CalamityRangerExpansion.Content.Arrows.APreHardMode.AerialiteArrow;
using CalamityRangerExpansion.Content.Arrows.APreHardMode.PrismArrow;
using CalamityRangerExpansion.Content.Arrows.APreHardMode.WulfrimArrow;
using CalamityRangerExpansion.Content.Arrows.BPrePlantera.StarblightSootArrow;
using CalamityRangerExpansion.Content.Arrows.CPreMoodLord.AstralArrow;
using CalamityRangerExpansion.Content.Arrows.CPreMoodLord.LifeAlloyArrow;
using CalamityRangerExpansion.Content.Arrows.CPreMoodLord.PerennialArrow;
using CalamityRangerExpansion.Content.Arrows.CPreMoodLord.PlagueArrow;
using CalamityRangerExpansion.Content.Arrows.CPreMoodLord.ScoriaArrow;
using CalamityRangerExpansion.Content.Arrows.DPreDog.DivineGeodeArrow;
using CalamityRangerExpansion.Content.Arrows.DPreDog.EffulgentFeatherArrow;
using CalamityRangerExpansion.Content.Arrows.DPreDog.ToothArrow;
using CalamityRangerExpansion.Content.Arrows.DPreDog.UelibloomArrow;
using CalamityRangerExpansion.Content.Arrows.EAfterDog.AuricArrow;
using CalamityRangerExpansion.Content.Arrows.EAfterDog.EndothermicEnergyArrow;
using CalamityRangerExpansion.Content.Arrows.EAfterDog.MiracleMatterArrow;
using CalamityRangerExpansion.Content.Gel.APreHardMode.AerialiteGel;
using CalamityRangerExpansion.Content.Gel.APreHardMode.GeliticGel;
using CalamityRangerExpansion.Content.Gel.APreHardMode.HurricaneGel;
using CalamityRangerExpansion.Content.Gel.APreHardMode.WulfrimGel;
using CalamityRangerExpansion.Content.Gel.BPrePlantera.CryonicGel;
using CalamityRangerExpansion.Content.Gel.BPrePlantera.StarblightSootGel;
using CalamityRangerExpansion.Content.Gel.CPreMoodLord.AstralGel;
using CalamityRangerExpansion.Content.Gel.CPreMoodLord.LifeAlloyGel;
using CalamityRangerExpansion.Content.Gel.CPreMoodLord.LivingShardGel;
using CalamityRangerExpansion.Content.Gel.CPreMoodLord.PerennialGel;
using CalamityRangerExpansion.Content.Gel.CPreMoodLord.PlagueGel;
using CalamityRangerExpansion.Content.Gel.CPreMoodLord.ScoriaGel;
using CalamityRangerExpansion.Content.Gel.DPreDog.BloodstoneCoreGel;
using CalamityRangerExpansion.Content.Gel.DPreDog.DivineGeodeGel;
using CalamityRangerExpansion.Content.Gel.DPreDog.EffulgentFeatherGel;
using CalamityRangerExpansion.Content.Gel.DPreDog.PolterplasmGel;
using CalamityRangerExpansion.Content.Gel.DPreDog.ToothGel;
using CalamityRangerExpansion.Content.Gel.DPreDog.UelibloomGel;
using CalamityRangerExpansion.Content.Gel.DPreDog.UnholyEssenceGel;
using CalamityRangerExpansion.Content.Gel.EAfterDog.AuricGel;
using CalamityRangerExpansion.Content.Gel.EAfterDog.CosmosGel;
using CalamityRangerExpansion.Content.Gel.EAfterDog.EndothermicEnergyGel;
using CalamityRangerExpansion.Content.Gel.EAfterDog.MiracleMatterGel;
using CalamityRangerExpansion.Content.Gel.ZBag;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Default;
using Terraria.Testing;
using Terraria.UI;

#endregion

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
                ModContent.ItemType<PlagueArrow>(),
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
