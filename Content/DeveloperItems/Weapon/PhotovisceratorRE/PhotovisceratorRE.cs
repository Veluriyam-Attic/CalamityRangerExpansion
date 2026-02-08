using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using CalamityMod;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Particles;

namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.PhotovisceratorRE
{
    internal class PhotovisceratorRE : ModItem, ILocalizedModType
    {
        public static readonly SoundStyle UseSound = new("CalamityMod/Sounds/Item/PhotoUseSound") { Volume = 0.35f };
        public static readonly SoundStyle HitSound = new("CalamityMod/Sounds/Item/PhotoHitSound") { Volume = 0.4f };
        public new string LocalizationCategory => "Weapons.RE";

        // Left-click stats
        public static float AmmoNotConsumeChance = 0.95f;
        public static int LightBombCooldown = 10;

        // Right-click stats
        public static float RightClickVelocityMult = 2.5f;
        public static int RightClickCooldown = 30;

        public override void SetDefaults()
        {
            Item.width = 208;
            Item.height = 66;

            Item.damage = 495;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = LightBombCooldown;
            Item.shootSpeed = 6f;
            Item.knockBack = 2f;
            Item.shoot = ModContent.ProjectileType<ExoFire>();
            Item.useAmmo = AmmoID.Gel;
            Item.useStyle = ItemUseStyleID.Shoot;
            //Item.channel = true;
            Item.noMelee = true;
            Item.noUseGraphic = false;

            Item.rare = ModContent.RarityType<CalamityMod.Rarities.CosmicPurple>(); //Violet 已经没了，译名紫罗兰
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
        }

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true; // 开启右键全自动点击功能
        }

        public override bool AltFunctionUse(Player player) => true; // 启用右键功能
        public override bool CanUseItem(Player player)
        {
            // 设置左右键性能
            if (player.altFunctionUse == 2) // 右键
            {
                Item.useTime = Item.useAnimation = RightClickCooldown;
                Item.damage = (int)(495 * 0.7f); // 右键伤害倍率为 70%
                Item.shootSpeed = 6f * RightClickVelocityMult; // 右键速度倍率
                Item.shoot = ModContent.ProjectileType<ExoFlareCluster>(); // 右键弹幕类型
            }
            else // 左键
            {
                Item.useTime = Item.useAnimation = 1; // 左键非常快速
                Item.damage = 495;
                Item.shootSpeed = 6f;
                Item.shoot = ModContent.ProjectileType<ExoFire>(); // 左键主要弹幕
            }

            return base.CanUseItem(player);
        }


        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 shootPosition = player.MountedCenter;
            Vector2 shootVelocity = Vector2.Normalize(Main.MouseWorld - shootPosition) * velocity.Length();


            if (player.altFunctionUse == 2) // 右键攻击逻辑
            {

                for (int i = 0; i <= 15; i++) // 每次右键释放 15 个粒子
                {
                    // 随机选择特效颜色
                    Color sparkColor = Main.rand.Next(4) switch
                    {
                        0 => Color.Red,
                        1 => Color.MediumTurquoise,
                        2 => Color.Orange,
                        _ => Color.LawnGreen,
                    };

                    // 生成环形特效
                    Vector2 particleVelocity = velocity.RotatedByRandom(0.6f) * Main.rand.NextFloat(0.3f, 1.6f);
                    DirectionalPulseRing pulse = new(position, particleVelocity, sparkColor, new Vector2(1, 1), 0, Main.rand.NextFloat(0.2f, 0.35f), 0f, 40);
                    GeneralParticleHandler.SpawnParticle(pulse);

                    DirectionalPulseRing pulse2 = new(position, velocity.RotatedByRandom(0.1f) * Main.rand.NextFloat(0.8f, 3.1f), sparkColor, new Vector2(1, 1), 0, Main.rand.NextFloat(0.2f, 0.35f), 0f, 40);
                    GeneralParticleHandler.SpawnParticle(pulse2);

                    // 生成尘埃特效
                    Dust dust = Dust.NewDustPerfect(position, 263, velocity.RotatedByRandom(0.6f) * Main.rand.NextFloat(0.3f, 1.6f));
                    dust.noGravity = true;
                    dust.scale = Main.rand.NextFloat(1.3f, 1.8f);
                    dust.color = sparkColor;
                }

                // 生成 ExoFlareCluster 弹幕
                Projectile.NewProjectile(source, shootPosition, shootVelocity, ModContent.ProjectileType<ExoFlareCluster>(), Item.damage, Item.knockBack, player.whoAmI);
                SoundEngine.PlaySound(UseSound, player.Center);
            }
            else // 左键攻击逻辑
            {
                if (Main.rand.NextBool())
                {
                    MediumMistParticle smoke = new MediumMistParticle(
                        shootPosition + Main.rand.NextVector2Circular(5, 5),
                        (shootVelocity * 15).RotatedByRandom(0.15f) * Main.rand.NextFloat(0.25f, 1.1f),
                        Color.WhiteSmoke, // 可以替换为其他颜色
                        Color.GhostWhite,
                        Main.rand.NextFloat(1.8f, 2.9f),
                        160,
                        Main.rand.NextFloat(-3f, 3f)
                    );
                    GeneralParticleHandler.SpawnParticle(smoke);

                    MediumMistParticle smoke2 = new MediumMistParticle(
                        shootPosition + Main.rand.NextVector2Circular(5, 5),
                        (shootVelocity * 18).RotatedByRandom(0.05f) * Main.rand.NextFloat(0.9f, 1.5f),
                        Color.WhiteSmoke,
                        Color.White,
                        Main.rand.NextFloat(0.8f, 1.9f),
                        160,
                        Main.rand.NextFloat(-3f, 3f)
                    );
                    GeneralParticleHandler.SpawnParticle(smoke2);
                }


                if (Main.rand.NextBool(4)) // 每隔一定概率生成特效
                {
                    Color energyColor = Color.Orange; // 设定特效颜色
                    Vector2 flamePosition = position + velocity * 0.12f;
                    Vector2 verticalOffset = Vector2.UnitY.RotatedBy(velocity.ToRotation());

                    if (Math.Cos(velocity.ToRotation()) < 0f)
                        verticalOffset *= -1f;

                    Vector2 flameAngle = -Vector2.UnitY.RotatedBy(velocity.ToRotation() + MathHelper.ToRadians(Main.rand.NextFloat(270f, 300f)));
                    SquishyLightParticle exoEnergy = new(flamePosition - verticalOffset * 26f, flameAngle * Main.rand.NextFloat(0.8f, 3.6f), 0.25f, energyColor, 20);
                    GeneralParticleHandler.SpawnParticle(exoEnergy);

                    SquishyLightParticle exoEnergy2 = new(position - verticalOffset * 22f, flameAngle * Main.rand.NextFloat(0.7f, 3.2f), 0.2f, energyColor, 12);
                    GeneralParticleHandler.SpawnParticle(exoEnergy2);
                }

                // 生成 ExoFire 弹幕
                int projIndex = Projectile.NewProjectile(source, shootPosition, shootVelocity, ModContent.ProjectileType<ExoFire>(), Item.damage, Item.knockBack, player.whoAmI);

                // 如果生成的弹幕有效，则设置其初始和动态变粗逻辑
                if (projIndex >= 0 && projIndex < Main.projectile.Length)
                {
                    Projectile proj = Main.projectile[projIndex];

                    // 设置初始缩放
                    proj.scale = 0.5f;

                    // 在弹幕的 ai[0] 用作计时器逻辑
                    proj.localAI[0] = 0; // 计时器
                }


                SoundEngine.PlaySound(UseSound, player.Center);

                // 每60次左键生成光炸弹 ExoLight
                if (Main.rand.NextBool(60))
                {
                    for (int i = -1; i <= 1; i += 2) // 循环生成两个炸弹，分别交叉方向
                    {
                        Vector2 targetDirection = Vector2.Normalize(Main.MouseWorld - shootPosition); // 鼠标方向
                        Vector2 lightVelocity = targetDirection.RotatedBy(0.2f * i) * shootVelocity.Length(); // 基于鼠标方向旋转

                        Projectile.NewProjectile(source, shootPosition, lightVelocity, ModContent.ProjectileType<ExoLight>(), Item.damage, Item.knockBack, player.whoAmI);
                    }

                    // 播放声音
                    SoundEngine.PlaySound(HitSound, player.Center);
                }
            }

            return true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-35, 0);

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Ranged/PhotovisceratorGlow").Value);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ChromaticEruption>(). //ElementalEruption todo => namespace CalamityMod.Items.Weapons.Ranged [LegacyName("ElementalEruption")]
                AddIngredient<HalleysInferno>().
                AddIngredient<DeadSunsWind>().
                AddIngredient<MiracleMatter>().
                AddTile<DraedonsForge>().
                Register();
        }
    }
}
