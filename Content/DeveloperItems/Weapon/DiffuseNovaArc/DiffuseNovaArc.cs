//using CalamityMod.Items.Materials;
//using CalamityMod.Items.Weapons.Ranged;
//using CalamityMod.Items;
//using CalamityMod.Projectiles.Ranged;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Terraria.Audio;
//using Terraria.DataStructures;
//using Terraria.ID;
//using Terraria.ModLoader;
//using Terraria;
//using CalamityMod;
//using Microsoft.Xna.Framework;
//using CalamityMod.Rarities;

//namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.DiffuseNovaArc
//{
//    public class DiffuseNovaArc : ModItem, ILocalizedModType
//    {
//        public override void SetStaticDefaults()
//        {
//            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<ArcNovaDiffuser>();
//            ItemID.Sets.IsRangedSpecialistWeapon[Type] = true;
//        }
//        public static readonly SoundStyle ChargeLV1 = new("CalamityMod/Sounds/Item/ArcNovaDiffuserChargeLV1") { Volume = 0.6f };
//        public static readonly SoundStyle ChargeLV2 = new("CalamityMod/Sounds/Item/ArcNovaDiffuserChargeLV2") { Volume = 0.6f };
//        public static readonly SoundStyle ChargeStart = new("CalamityMod/Sounds/Item/ArcNovaDiffuserChargeStart") { Volume = 0.6f };
//        public static readonly SoundStyle ChargeLoop = new("CalamityMod/Sounds/Item/ArcNovaDiffuserChargeLoop") { Volume = 0.6f };
//        public static readonly int ChargeLoopSoundFrames = 151;
//        public static readonly SoundStyle SmallShot = new("CalamityMod/Sounds/Item/ArcNovaDiffuserSmallShot") { PitchVariance = 0.3f, Volume = 0.5f };
//        public static readonly SoundStyle BigShot = new("CalamityMod/Sounds/Item/ArcNovaDiffuserBigShot") { PitchVariance = 0.3f, Volume = 0.8f };

//        public static int AftershotCooldownFrames = 9;
//        public static int Charge1Frames = 156;
//        public static int Charge2Frames = 308;

//        public new string LocalizationCategory => "DeveloperItems.DiffuseNovaArc";


//        public override void SetDefaults()
//        {
//            Item.width = 58;
//            Item.height = 28;
//            Item.damage = 172;
//            Item.DamageType = DamageClass.Ranged;
//            Item.useTime = Item.useAnimation = AftershotCooldownFrames;
//            Item.noMelee = true;
//            Item.noUseGraphic = true;
//            Item.channel = true;
//            Item.useStyle = ItemUseStyleID.Shoot;
//            Item.knockBack = 4f;
//            Item.UseSound = null;
//            Item.autoReuse = false;
//            Item.shoot = ModContent.ProjectileType<DiffuseNovaArcHoldout>();
//            Item.shootSpeed = 12f;
//            Item.Calamity().canFirePointBlankShots = true;

//            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
//            Item.rare = ModContent.RarityType<HotPink>();
//            Item.Calamity().donorItem = true;
//        }

//        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

//        public override void HoldItem(Player player) => player.Calamity().mouseWorldListener = true;

//        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
//        {
//            Projectile holdout = Projectile.NewProjectileDirect(source, player.MountedCenter, Vector2.Zero, ModContent.ProjectileType<DiffuseNovaArcHoldout>(), damage, knockback, player.whoAmI, 0, 1);
//            holdout.velocity = player.Calamity().mouseWorld - player.RotatedRelativePoint(player.MountedCenter);
//            return false;
//        }

//        //public override void AddRecipes()
//        //{
//        //    CreateRecipe().
//        //        AddIngredient<OpalStriker>().
//        //        AddIngredient<MagnaCannon>().
//        //        AddIngredient<LifeAlloy>(3).
//        //        AddIngredient(ItemID.MartianConduitPlating, 15).
//        //        AddTile(TileID.MythrilAnvil).
//        //        Register();
//        //}
//    }
//}
