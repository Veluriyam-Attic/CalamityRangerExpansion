using System;
using System.Collections.Generic;
using CalamityMod.Items.Weapons.Ranged;
using CalamityRangerExpansion.Content.BOWChange.APreHardMode.LunarianBowC;
using CalamityRangerExpansion.Content.BOWChange.APreHardMode.ToxibowC;
using CalamityRangerExpansion.Content.BOWChange.BPrePlantera.ArbalestC;
using CalamityRangerExpansion.Content.BOWChange.BPrePlantera.BarinauticalC;
using CalamityRangerExpansion.Content.BOWChange.BPrePlantera.BrimstoneFuryC;
using CalamityRangerExpansion.Content.BOWChange.BPrePlantera.CorrodedCaustibowC;
using CalamityRangerExpansion.Content.BOWChange.BPrePlantera.DarkechoGreatbowC;
using CalamityRangerExpansion.Content.BOWChange.BPrePlantera.PearlwoodBowC;
using CalamityRangerExpansion.Content.BOWChange.CPreMoodLord.BlossomFluxC;
using CalamityRangerExpansion.Content.BOWChange.CPreMoodLord.MalevolenceC;
using CalamityRangerExpansion.Content.BOWChange.CPreMoodLord.TheBallistaC;
using CalamityRangerExpansion.Content.BOWChange.CPreMoodLord.VernalBolterC;
using CalamityRangerExpansion.Content.BOWChange.DPreDog.ArterialAssaultC;
using CalamityRangerExpansion.Content.BOWChange.DPreDog.DaemonsFlameC;
using CalamityRangerExpansion.Content.BOWChange.DPreDog.NettlevineGreatbowC;
using CalamityRangerExpansion.Content.BOWChange.DPreDog.PlanetaryAnnihilationC;
using CalamityRangerExpansion.Content.BOWChange.DPreDog.TheStormC;
using CalamityRangerExpansion.Content.BOWChange.EAfterDog.CondemnationC;
using CalamityRangerExpansion.Content.BOWChange.EAfterDog.DeathwindC;
using CalamityRangerExpansion.Content.BOWChange.EAfterDog.PhangasmC;
using CalamityRangerExpansion.Content.BOWChange.EAfterDog.UltimaC;
using CalamityRangerExpansion.Content.BOWChange.Global;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityRangerExpansion.Content.BOWChange
{
    internal class SwitchWeapons : ModPlayer
    {
        private static readonly List<(Type ChargedWeapon, int BaseWeaponID)> WeaponPairs = new()
        {
            (typeof(LunarianBowRe), ModContent.ItemType<LunarianBow>()),
            (typeof(ToxibowRe), ModContent.ItemType<Toxibow>()),
            (typeof(ArbalestRe), ModContent.ItemType<Arbalest>()),
            (typeof(BarinauticalRe), ModContent.ItemType<Barinautical>()),
            (typeof(BrimstoneFuryRe), ModContent.ItemType<BrimstoneFury>()),
            (typeof(CorrodedCaustibowRe), ModContent.ItemType<CorrodedCaustibow>()),
            (typeof(DarkechoGreatbowRe), ModContent.ItemType<DarkechoGreatbow>()),
            (typeof(PearlwoodBowRe), ItemID.PearlwoodBow),
            (typeof(BlossomFluxRe), ModContent.ItemType<BlossomFlux>()),
            (typeof(MalevolenceRe), ModContent.ItemType<Malevolence>()),
            (typeof(TheBallistaRe), ModContent.ItemType<TheBallista>()),
            (typeof(VernalBolterRe), ModContent.ItemType<VernalBolter>()),
            (typeof(ArterialAssaultRe), ModContent.ItemType<ArterialAssault>()),
            (typeof(DaemonsFlameRe), ModContent.ItemType<DaemonsFlame>()),
            (typeof(NettlevineGreatbowRe), ModContent.ItemType<NettlevineGreatbow>()),
            (typeof(PlanetaryAnnihilationRe), ModContent.ItemType<PlanetaryAnnihilation>()),
            (typeof(TheStormRe), ModContent.ItemType<TheStorm>()),
            (typeof(CondemnationRe), ModContent.ItemType<Condemnation>()),
            (typeof(DeathwindRe), ModContent.ItemType<ThreadOfEradication>()),
            (typeof(PhangasmRe), ModContent.ItemType<Riftburst>()),
            (typeof(UltimaRe), ModContent.ItemType<Ultima>()),
        };

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (!CBRKeys.ChangeBow.JustPressed || Main.playerInventory)
                return;

            foreach ((Type chargedWeapon, int baseWeaponID) in WeaponPairs)
            {
                if (Player.HeldItem.ModItem?.GetType() == chargedWeapon)
                {
                    ReplaceWeapon(baseWeaponID);
                    return;
                }

                if (Player.HeldItem.type == baseWeaponID)
                {
                    ReplaceWeapon(GetModItemID(chargedWeapon));
                    return;
                }
            }
        }

        private void ReplaceWeapon(int targetWeaponID)
        {
            int currentPrefix = Player.HeldItem.prefix;
            Item newWeapon = new Item();

            newWeapon.SetDefaults(targetWeaponID);
            newWeapon.Prefix(currentPrefix);
            Player.inventory[Player.selectedItem] = newWeapon;

            SoundEngine.PlaySound(SoundID.Item68);
        }

        private static int GetModItemID(Type targetWeaponType)
        {
            return (int)typeof(ModContent).GetMethod("ItemType", Type.EmptyTypes)
                .MakeGenericMethod(targetWeaponType)
                .Invoke(null, null);
        }








    }
}
