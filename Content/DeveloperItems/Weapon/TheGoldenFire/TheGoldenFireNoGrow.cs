#region using太多了
using CalamityMod.Items.Weapons.Ranged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod;
using Microsoft.Xna.Framework;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Items;
using CalamityMod.Rarities;
using Terraria.DataStructures;
using Terraria.ModLoader.IO;
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
using CalamityRangerExpansion.Content.Gel.CPreMoodLord.ScoriaGel;
using CalamityRangerExpansion.Content.Gel.DPreDog.BloodstoneCoreGel;
using CalamityRangerExpansion.Content.Gel.DPreDog.DivineGeodeGel;
using CalamityRangerExpansion.Content.Gel.DPreDog.EffulgentFeatherGel;
using CalamityRangerExpansion.Content.Gel.DPreDog.PolterplasmGel;
using CalamityRangerExpansion.Content.Gel.DPreDog.UelibloomGel;
using CalamityRangerExpansion.Content.Gel.DPreDog.UnholyEssenceGel;
using CalamityRangerExpansion.Content.Gel.EAfterDog.AuricGel;
using CalamityRangerExpansion.Content.Gel.EAfterDog.CosmosGel;
using CalamityRangerExpansion.Content.Gel.EAfterDog.MiracleMatterGel;
using CalamityRangerExpansion.Content.Gel.CPreMoodLord.PlagueGel;
using CalamityMod.Projectiles.Ranged;
using Terraria.Utilities.Terraria.Utilities;
#endregion

namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.TheGoldenFire
{
    public class TheGoldenFireNoGrow : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.TheGoldenFire";

        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.DamageType = DamageClass.Ranged;
            Item.useAnimation = 5;
            Item.shoot = ModContent.ProjectileType<TheGoldenFirePROJGrow>();
            Item.knockBack = 6.5f;
            Item.shootSpeed = 5f;
            Item.useTime = 5;

            Item.width = 96;
            Item.height = 42;
            Item.noMelee = true;
            Item.channel = true;
            Item.noUseGraphic = false;
            Item.useAmmo = AmmoID.Gel;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item34;
            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = -13;
            Item.Calamity().devItem = true;
        }

   


        public override void HoldItem(Player player) => player.Calamity().mouseRotationListener = true;

        public static Dictionary<int, Color> GelColors = new Dictionary<int, Color>
        {
            { ItemID.Gel, Color.Gold }, // 原版凝胶
            { ModContent.ItemType<AerialiteGel>(), Color.LightSkyBlue }, // 模组中的天蓝色凝胶
            { ModContent.ItemType<GeliticGel>(), Color.LightSkyBlue }, // 双色凝胶
            { ModContent.ItemType<HurricaneGel>(), Color.Blue }, // 棱镜凝胶
            { ModContent.ItemType<WulfrimGel>(), new Color(153, 255, 102) }, // 钨钢凝胶
            { ModContent.ItemType<CryonicGel>(), Color.LightSkyBlue }, // 寒元凝胶
            { ModContent.ItemType<StarblightSootGel>(), new Color(208,163,230) }, // 调星凝胶
            { ModContent.ItemType<AstralGel>(), Color.AliceBlue }, // 幻星凝胶
            { ModContent.ItemType<LifeAlloyGel>(), Color.SpringGreen }, // 生命合金凝胶
            { ModContent.ItemType<LivingShardGel>(), Color.ForestGreen }, // 生命碎片凝胶
            { ModContent.ItemType<PerennialGel>(), Color.GreenYellow }, // 永恒凝胶
            { ModContent.ItemType<ScoriaGel>(), Color.DarkOrange }, // 熔渣凝胶
            { ModContent.ItemType<BloodstoneCoreGel>(), Color.MediumVioletRed }, // 血石核心凝胶
            { ModContent.ItemType<DivineGeodeGel>(), Color.Gold }, // 神圣晶石凝胶
            { ModContent.ItemType<EffulgentFeatherGel>(), Color.LightGray }, // 金羽凝胶
            { ModContent.ItemType<PolterplasmGel>(), Color.LightPink }, // 灵质凝胶
            { ModContent.ItemType<UelibloomGel>(), Color.DarkGreen }, // 龙蒿凝胶
            { ModContent.ItemType<UnholyEssenceGel>(), Color.LightYellow }, // 烛火精华凝胶
            { ModContent.ItemType<AuricGel>(), Color.LightGoldenrodYellow }, // 金元凝胶
            { ModContent.ItemType<CosmosGel>(), Color.MediumPurple }, // 宇宙凝胶
            { ModContent.ItemType<MiracleMatterGel>(), Color.GhostWhite }, // 奇迹凝胶
            { ModContent.ItemType<PlagueGel>(), Color.Green }, // 奇迹凝胶
            //{ ModContent.ItemType<EndothermicEnergyGel>(), Color.Blue } // 霜华凝胶
        };

        public override bool Shoot(Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.PickAmmo(player.HeldItem, out _, out _, out _, out _, out int ammoType))
            {
                // 如果使用的是 AstralGel，则强制转换为 TheGoldenFirePROJAG【特殊情况特殊对待】
                if (ammoType == ModContent.ItemType<AstralGel>())
                {
                    type = ModContent.ProjectileType<TheGoldenFirePROJAG>();
                }

                // 获取对应的颜色
                if (!GelColors.TryGetValue(ammoType, out Color fireColor))
                {
                    Color readonlyFireColor = Color.Gold;
                    fireColor = readonlyFireColor; // 默认颜色
                }

                // 发射火焰弹幕并传递颜色
                for (int i = 0; i < 2; i++) // 发射两发弹幕
                {
                    Vector2 adjustedVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(5f));
                    int projID = Projectile.NewProjectile(source, position, adjustedVelocity, type, damage, knockback, player.whoAmI);

                    if (Main.projectile[projID].ModProjectile is TheGoldenFirePROJGrow fireProj)
                    {
                        Color ReadonlyFireColor = fireColor;
                        fireProj.FireColor = ReadonlyFireColor;
                    }
                }


            }
            return false;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-20, 0);

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(1);
            recipe.AddIngredient<SparkSpreader>(1);
            recipe.AddIngredient(ItemID.Gel, 10);
            recipe.AddRecipeGroup("AnySilverBar", 10);
            recipe.AddIngredient(ItemID.IllegalGunParts, 2);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}

