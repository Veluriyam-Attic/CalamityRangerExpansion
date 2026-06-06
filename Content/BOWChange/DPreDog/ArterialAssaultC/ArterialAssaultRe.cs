 
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod;
using Microsoft.Xna.Framework;
using CalamityRangerExpansion.Content.BOWChange.DPreDog.NettlevineGreatbowC;

namespace CalamityRangerExpansion.Content.BOWChange.DPreDog.ArterialAssaultC
{
    internal class ArterialAssaultRe : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.DPreDog";
        public override void SetDefaults()
        {
            // 设置武器基本属性
            Item.width = 40; // 弓的宽度
            Item.height = 80; // 弓的高度
            Item.damage = 94; // 武器伤害
            Item.DamageType = DamageClass.Ranged; // 伤害类型：远程
            Item.useTime = 5; // 使用时间（5帧）
            Item.useAnimation = 5; // 动画时间（5帧）
            Item.useStyle = ItemUseStyleID.Shoot; // 使用风格：射击
            Item.knockBack = 4; // 击退力
            // Item.UseSound = SoundID.Item5; // 一般情况下他没有使用音效
            Item.shoot = ModContent.ProjectileType<ArterialAssaultReHold>(); // 手持弹幕
            Item.shootSpeed = 0f; // 手持弹幕的初始速度为0
            Item.noMelee = true; // 不进行近战攻击
            Item.noUseGraphic = true; // 使用时隐藏物品模型
            Item.channel = true; // 支持长按
            Item.autoReuse = true; // 自动连点
            Item.useAmmo = AmmoID.Arrow;

            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
            // Item.Calamity().canFirePointBlankShots = true;
        }
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }
        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            // 左键（channel 模式）必须保证只有一个手持弹幕存在
            if (player.altFunctionUse == 2)
            {
                // 右键 → 无条件可用
                Item.channel = false;
                Item.shootSpeed = 22f;
                Item.useAmmo = 0; // 不消耗弹药
                Item.useTime = 30; // 使用时间
                Item.useAnimation = 30; // 动画时间
                Item.useStyle = ItemUseStyleID.Swing; // 使用风格：投掷
            }
            else
            {
                // 左键 → 长按手持，不重复召唤
                Item.channel = true;
                Item.shootSpeed = 0f;
                Item.useAmmo = AmmoID.Arrow;
                Item.useTime = 5; // 使用时间（5帧）
                Item.useAnimation = 5; // 动画时间（5帧）
                Item.useStyle = ItemUseStyleID.Shoot; // 使用风格：射击
            }

            return player.altFunctionUse == 2 || player.ownedProjectileCounts[Item.shoot] <= 0;
        }

        // 右键不消耗弹药
        public override bool CanConsumeAmmo(Item ammo, Player player)
        {
            return player.altFunctionUse != 2 && player.ownedProjectileCounts[Item.shoot] > 0;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 shootDirection = velocity.SafeNormalize(Vector2.UnitX * player.direction);

            if (player.altFunctionUse == 2)
            {
                // 右键：丢出右键弹幕（不消耗弹药）
                Projectile.NewProjectile(
                    source,
                    position,
                    shootDirection * Item.shootSpeed,
                    ModContent.ProjectileType<ArterialAssaultReRightPROJ>(),
                    damage,
                    knockback,
                    player.whoAmI
                );
            }
            else
            {
                // 左键：生成手持弹幕（无速度）
                Projectile.NewProjectile(
                    source,
                    position,
                    shootDirection,
                    ModContent.ProjectileType<ArterialAssaultReHold>(),
                    damage,
                    knockback,
                    player.whoAmI
                );
            }

            return false; // 手动生成弹幕，阻止默认行为
        }


        //public override Vector2? HoldoutOffset() => new Vector2(0, 10);
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            //recipe.AddIngredient(ItemID.Spear, 1);
            recipe.AddIngredient<ArterialAssault>();
            //recipe.AddCondition(Condition.BirthdayParty);
            //recipe.AddTile(TileType<DraedonsForge>());
            //recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}
