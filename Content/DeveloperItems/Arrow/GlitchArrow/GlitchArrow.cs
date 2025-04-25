//using CalamityMod.Items.Materials;
//using CalamityRangerExpansion.Content.Arrows.CPreMoodLord.CoreofCalamityArrow;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Terraria.ID;
//using Terraria.ModLoader;
//using Terraria;

//namespace CalamityRangerExpansion.Content.DeveloperItems.Arrow.GlitchArrow
//{
//    internal class GlitchArrow : ModItem, ILocalizedModType
//    {
//        public new string LocalizationCategory => "DeveloperItems.GlitchArrow";
//        private int textureIndex; // 当前贴图索引
//        //public override string Texture => $"Terraria/Images/Projectile_{textureIndex}"; // 动态贴图路径
//        private int frameCounter = 0;
//        public override void PostUpdate()
//        {
//            // 每帧随机更新贴图索引
//            textureIndex = Main.rand.Next(1, 1022);

//            frameCounter++;

//            if (frameCounter % 5 == 0) // 每 5 帧更新一次伤害
//            {
//                Item.damage = Main.rand.Next(1, 7);
//            }

//            if (frameCounter % 3 == 0) // 每 3 帧更新一次贴图
//            {
//                textureIndex = Main.rand.Next(1, 1022);
//            }
//        }

//        public override void SetDefaults()
//        {
//            Item.damage = Main.rand.Next(1, 7); // 1~6 之间随机伤害
//            Item.DamageType = DamageClass.Ranged;
//            Item.width = 14;
//            Item.height = 32;
//            Item.maxStack = 1;
//            Item.consumable = false; // 弹药是消耗品
//            Item.knockBack = 3.5f;
//            Item.value = 10;
//            Item.rare = ItemRarityID.Blue;
//            Item.shoot = ModContent.ProjectileType<GlitchArrowPROJ>();
//            Item.shootSpeed = 15f;
//            Item.ammo = AmmoID.Arrow; // 这是箭矢类型的弹药
//        }

//        public override void AddRecipes()
//        {
//            Recipe recipe = CreateRecipe(1);
//            //recipe.AddIngredient<EssenceofHavoc>(1); // 混乱精华
//            //recipe.AddIngredient(ItemID.Nanites, 1); // 纳米机器人
//            //recipe.AddIngredient(ItemID.Cog, 1); // 齿轮
//            //recipe.AddIngredient(ItemID.SpectrePaintbrush, 1); // 幽灵刷子
//            //recipe.AddIngredient(ItemID.MartianLamppost, 1); // 火星灯柱
//            //recipe.AddIngredient(ItemID.ShimmerCampfire, 1); // 以太篝火
//            //recipe.AddIngredient(ItemID.JimsDrone, 1); // 四轴竞速无人机
//            //recipe.AddIngredient(ItemID.SkywarePiano, 1); // 天域钢琴
//            //recipe.AddIngredient(ItemID.BorealWoodBow, 1); // 针叶木弓
//            //recipe.AddIngredient(ItemID.PalladiumColumn, 1); // 钯金柱
//            //recipe.AddIngredient(ItemID.IchorTorch, 1); // 灵液火把
//            //recipe.AddIngredient(ItemID.Cannonball, 1); // 炮弹
            
//            // 添加 X 个随机物品（范围 1~5450）
//            for (int i = 0; i < 10; i++)
//            {
//                int randomItemID = Main.rand.Next(1, 5451); // 1 ~ 5450 之间的随机物品 ID
//                recipe.AddIngredient(randomItemID, 1);
//            }

//            recipe.AddTile(TileID.HeavyWorkBench);
//            recipe.Register();
//        }
//    }
//}
