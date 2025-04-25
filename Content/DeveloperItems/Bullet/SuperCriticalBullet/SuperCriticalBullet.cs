//using CalamityMod.Items.Materials;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Terraria.ID;
//using Terraria.ModLoader;
//using Terraria;
//using CalamityMod.Items;
//using CalamityMod.Rarities;
//using CalamityMod;

//namespace CalamityRangerExpansion.Content.DeveloperItems.Bullet.SuperCriticalBullet
//{
//    internal class SuperCriticalBullet : ModItem, ILocalizedModType
//    {
//        public new string LocalizationCategory => "DeveloperItems.SuperCriticalBullet";
//        public override void SetDefaults()
//        {
//            Item.damage = 30;
//            Item.DamageType = DamageClass.Ranged;
//            Item.width = 14;
//            Item.height = 32;
//            Item.maxStack = 1;
//            Item.consumable = false; // 弹药是消耗品
//            Item.knockBack = 3.5f;

//            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
//            Item.rare = ModContent.RarityType<HotPink>();
//            Item.Calamity().devItem = true;
//            Item.shoot = ModContent.ProjectileType<SuperCriticalBulletPROJ>();
//            Item.shootSpeed = 6f;
//            Item.ammo = AmmoID.Bullet; // 这是子弹类型的弹药
//            Item.crit = 300;
//        }

//        public override void AddRecipes()
//        {
//            Recipe recipe1 = CreateRecipe(1);
//            recipe1.AddIngredient(ItemID.MoonlordBullet, 333); // 夜明弹
//            recipe1.AddIngredient<ShadowspecBar>(5); // 魔影弹
//            recipe1.AddTile(TileID.Anvils);
//            recipe1.Register();
//        }

//    }
//}
