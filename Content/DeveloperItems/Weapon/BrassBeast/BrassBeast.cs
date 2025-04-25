using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using CalamityMod;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items;
using CalamityMod.Rarities;

namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.BrassBeast
{
    public class BrassBeast : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.BrassBeast";
        public override void SetDefaults()
        {
            Item.damage = 120; // 攻击力随意设定
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 30; // 使用时间
            Item.shoot = ModContent.ProjectileType<BrassBeastHoldOut>(); // 绑定 BrassBeastHoldOut
            Item.shootSpeed = 15f;
            Item.knockBack = 8f;

            Item.width = 120;
            Item.height = 40;
            Item.noMelee = true;
            Item.channel = true; // 支持持续攻击
            Item.noUseGraphic = true; // 不显示武器
            Item.useAmmo = AmmoID.Bullet;

            Item.value = Item.buyPrice(0, 50, 0, 0); // 设定物品价值
            Item.rare = ItemRarityID.Red; // 设定稀有度
            Item.useStyle = ItemUseStyleID.Shoot;

            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.Calamity().devItem = true;
        }
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }
        public override bool AltFunctionUse(Player player) => true;
        public override bool CanConsumeAmmo(Item ammo, Player player) => player.ownedProjectileCounts[Item.shoot] != 0;
        public override void HoldItem(Player player) => player.Calamity().mouseRotationListener = true;
        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2) // 右键逻辑
            {
                Item.damage = 215; // 右键攻击倍率
                Item.useTime = Item.useAnimation = 90; // 使用时间
                Item.shoot = ModContent.ProjectileType<BrassBeastHeavySmoke>();
                Item.shootSpeed = 10f; // 弹幕速度
                Item.UseSound = SoundID.Item38; // 播放右键音效
                Item.noUseGraphic = false; // 显示武器
            }
            else // 左键逻辑
            {
                Item.damage = 120; // 原始伤害
                Item.useTime = Item.useAnimation = 30; // 左键时间
                Item.shoot = ModContent.ProjectileType<BrassBeastHoldOut>();
                Item.shootSpeed = 15f;
                Item.UseSound = null; // 左键不播放音效
                Item.noUseGraphic = true; // 不显示武器
            }
            return player.ownedProjectileCounts[Item.shoot] == 0;
        }
        public override Vector2? HoldoutOffset() => new Vector2(-35, 0);

        public override bool Shoot(Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 mouseDirection = (Main.MouseWorld - player.MountedCenter).SafeNormalize(Vector2.UnitX);

            if (player.altFunctionUse == 2) // 右键逻辑
            {
                // 发射x发 BrassBeastHeavySmoke
                for (int i = 0; i < 1; i++)
                {
                    // 在鼠标方向上随机扩散30度以内
                    //Vector2 randomSpread = mouseDirection.RotatedByRandom(MathHelper.ToRadians(30)) * 10f;
                    //Projectile.NewProjectile(
                    //    source,
                    //    player.MountedCenter,
                    //    randomSpread,
                    //    ModContent.ProjectileType<BrassBeastHeavySmoke>(),
                    //    damage,
                    //    knockback,
                    //    player.whoAmI
                    //);

                    Projectile.NewProjectile(
                        source,
                        player.MountedCenter,
                        mouseDirection * 10f, // 取消扩散，直接朝向鼠标方向
                        ModContent.ProjectileType<BrassBeastHeavySmoke>(),
                        damage,
                        knockback,
                        player.whoAmI
                    );
                }

                //// 发射15发玩家当前的子弹
                //Item heldItem = player.HeldItem;
                //if (player.HasAmmo(heldItem))
                //{
                //    for (int i = 0; i < 15; i++)
                //    {
                //        if (player.PickAmmo(heldItem, out int ammoProjectile, out float shootSpeed, out int ammoDamage, out float ammoKnockback, out int ammoType))
                //        {
                //            Vector2 randomSpread = mouseDirection.RotatedByRandom(MathHelper.ToRadians(30)) * shootSpeed;
                //            Projectile.NewProjectile(
                //                source,
                //                player.MountedCenter,
                //                randomSpread,
                //                ammoProjectile,
                //                ammoDamage,
                //                ammoKnockback,
                //                player.whoAmI
                //            );
                //        }
                //    }
                //}
            }
            else // 左键逻辑
            {
                // 发射 BrassBeastHoldOut，方向实时指向鼠标
                Projectile.NewProjectileDirect(
                    source,
                    player.MountedCenter,
                    mouseDirection * 15f, // 根据鼠标方向确定速度
                    ModContent.ProjectileType<BrassBeastHoldOut>(),
                    damage,
                    knockback,
                    player.whoAmI
                );
            }

            return false;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (NPC.downedMoonlord) // 击败月亮领主后
            {
                damage *= 4.5f; // 提升伤害倍率至 X
            }
        }


        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(1);
            recipe.AddIngredient(ItemID.CopperBar, 10);
            recipe.AddIngredient(ItemID.SoulofNight, 5);
            recipe.AddIngredient(ItemID.IllegalGunParts, 2);
            recipe.AddIngredient(ItemID.Lens, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
