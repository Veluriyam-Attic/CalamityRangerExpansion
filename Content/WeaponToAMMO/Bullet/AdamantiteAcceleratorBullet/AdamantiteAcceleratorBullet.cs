using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items;
using CalamityMod.Rarities;
using CalamityRangerExpansion.Content.WeaponToAMMO.Bullet.ApoctosisMagicBullet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod;

namespace CalamityRangerExpansion.Content.WeaponToAMMO.Bullet.AdamantiteAcceleratorBullet
{
    internal class AdamantiteAcceleratorBullet : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "WeaponToAMMO.Bullet.AdamantiteAcceleratorBullet";
        public override void SetDefaults()
        {
            Item.width = 8;
            Item.height = 18;
            Item.damage = 22;
            Item.DamageType = DamageClass.Ranged;
            Item.maxStack = 1;
            Item.consumable = false;
            Item.knockBack = 3f;
            //Item.mana = 8;

            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.shoot = ModContent.ProjectileType<AdamantiteAcceleratorBulletPROJ>();
            Item.shootSpeed = 6f;
            Item.ammo = AmmoID.Bullet;
        }
        public override void ModifyTooltips(List<TooltipLine> list)
        {
            // 检查是否加载了模组 AccessoryForPlanetaryEmpire
            Mod plaMod;
            bool isPLALoaded = ModLoader.TryGetMod("AccessoryForPlanetaryEmpire", out plaMod);

            // 根据是否加载模组替换占位符 [HavePLA]
            string tooltipKey = isPLALoaded ? "TooltipHavePLA" : "TooltipNormal";
            string localizedTooltip = this.GetLocalizedValue(tooltipKey);

            // 替换提示内容
            list.FindAndReplace("[HavePLA]", localizedTooltip);
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(1);
            recipe.AddIngredient<AdamantiteParticleAccelerator>(1);
            recipe.AddCondition(Condition.NearShimmer);
            //recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }

    }
}
