#region using太多了
using CalamityMod.Items.Weapons.Ranged;
using CalamityRangerExpansion.Content.DeveloperItems.Weapon.Pyroblast;
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
#endregion

namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.TheGoldenFire
{
    public class TheGoldenFire : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.TheGoldenFire";

        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.DamageType = DamageClass.Ranged;
            Item.useAnimation = 5;
            Item.shoot = ModContent.ProjectileType<TheGoldenFirePROJ>();
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

        private int currentStage = 0; // 当前阶段

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            // 初始化面板基础值
            int baseDamage = 8;
            float baseShootSpeed = 3.5f;
            float baseKnockBack = 0.5f;
            int baseUseTime = 4;

            // 定义最终面板值
            int finalDamage = baseDamage;
            float finalShootSpeed = baseShootSpeed;
            float finalKnockBack = baseKnockBack;
            int finalUseTime = baseUseTime;

            // 从早到晚检测击败的最晚敌人

            // 击败了 克苏鲁之眼
            if (NPC.downedBoss1)
            {
                finalDamage = 13;
                finalShootSpeed = 3.5f;
                finalKnockBack = 0.5f;
                currentStage = 1;
                finalUseTime = 5;
            }

            // 击败了 世吞或克脑 中的一个
            if (NPC.downedBoss2)
            {
                finalDamage = 17;
                finalShootSpeed = 5.5f;
                finalKnockBack = 0.6f;
                currentStage = 2;
                finalUseTime = 5;
            }

            // 击败了 腐巢意志或血肉宿主 中的一个
            if (DownedBossSystem.downedHiveMind || DownedBossSystem.downedPerforator)
            {
                finalDamage = 18;
                finalShootSpeed = 6f;
                finalKnockBack = 0.6f;
                currentStage = 3;
                finalUseTime = 5;
            }

            // 击败了 骷髅王
            if (NPC.downedBoss3)
            {
                finalDamage = 19;
                finalShootSpeed = 6.5f;
                finalKnockBack = 0.6f;
                currentStage = 4;
                finalUseTime = 5;
            }

            // 击败了 史莱姆之神
            if (DownedBossSystem.downedSlimeGod)
            {
                finalDamage = 23;
                finalShootSpeed = 7f;
                finalKnockBack = 0.9f;
                currentStage = 5;
                finalUseTime = 4;

            }

            // 进入困难模式（击败肉山）
            if (Main.hardMode)
            {
                finalDamage = 43;
                finalShootSpeed = 8f;
                finalKnockBack = 1.0f;
                currentStage = 6;
                finalUseTime = 2;
            }

            // 击败了 双子魔眼+机械蠕虫+机械骷髅王 的全部3者
            if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3)
            {
                finalDamage = 58;
                finalShootSpeed = 8f;
                finalKnockBack = 1f;
                currentStage = 7;
                finalUseTime = 2;
            }

            // 击败了 灾厄之影
            if (DownedBossSystem.downedCalamitasClone)
            {
                finalDamage = 65;
                finalShootSpeed = 8f;
                finalKnockBack = 1f;
                currentStage = 8;
                finalUseTime = 2;
            }
            
            // 击败了 世纪之花
            if (NPC.downedPlantBoss)
            {
                finalDamage = 77;
                finalShootSpeed = 8f;
                finalKnockBack = 1f;
                currentStage = 9;
                finalUseTime = 2;
            }

            // 击败了 石巨人
            if (NPC.downedGolemBoss)
            {
                finalDamage = 112;
                finalShootSpeed = 8f;
                finalKnockBack = 1f;
                currentStage = 10;
                finalUseTime = 2;
            }

            // 击败了 拜月教邪教徒
            if (NPC.downedAncientCultist)
            {
                finalDamage = 122;
                finalShootSpeed = 10f;
                finalKnockBack = 1f;
                currentStage = 11;
                finalUseTime = 1;
            }

            // 击败了 月球领主
            if (NPC.downedMoonlord)
            {
                finalDamage = 248;
                finalShootSpeed = 14f;
                finalKnockBack = 1.5f;
                currentStage = 12;
                finalUseTime = 1;
            }

            // 击败了 亵渎天神
            if (DownedBossSystem.downedProvidence)
            {
                finalDamage = 252;
                finalShootSpeed = 14f;
                finalKnockBack = 1.6f;
                currentStage = 13;
                finalUseTime = 1;
            }

            // 击败了 西格纳斯+风暴编织者+无尽虚空 的全部三者
            if (DownedBossSystem.downedSignus && DownedBossSystem.downedStormWeaver && DownedBossSystem.downedCeaselessVoid)
            {
                finalDamage = 261;
                finalShootSpeed = 14f;
                finalKnockBack = 1.7f;
                currentStage = 14;
                finalUseTime = 1;
            }

            // 击败了 花灵
            if (DownedBossSystem.downedPolterghast)
            {
                finalDamage = 270;
                finalShootSpeed = 14f;
                finalKnockBack = 1.8f;
                currentStage = 15;
                finalUseTime = 1;
            }

            // 击败了 神明吞噬者
            if (DownedBossSystem.downedDoG)
            {
                finalDamage = 666;
                finalShootSpeed = 16f;
                finalKnockBack = 1.9f;
                currentStage = 16;
                finalUseTime = 1;
            }

            // 击败了 龙
            if (DownedBossSystem.downedYharon)
            {
                finalDamage = 721;
                finalShootSpeed = 16f;
                finalKnockBack = 2.0f;
                currentStage = 17;
                finalUseTime = 1;
            }

            // 击败了 巨械+终灾 的全部2者
            if (DownedBossSystem.downedExoMechs && DownedBossSystem.downedCalamitas)
            {
                finalDamage = 8848;
                finalShootSpeed = 20f;
                finalKnockBack = 2.1f;
                currentStage = 18;
                finalUseTime = 1;
            }

            // 击败了始源妖龙
            if (DownedBossSystem.downedPrimordialWyrm)
            {
                finalDamage = 11451;
                finalShootSpeed = 20f;
                finalKnockBack = 2.2f;
                currentStage = 19;
                finalUseTime = 1;
            }


            // 设置最终的伤害倍率
            damage.Base = finalDamage;

            // 修改 shootSpeed 和 knockBack
            Item.shootSpeed = finalShootSpeed;
            Item.knockBack = finalKnockBack;
            Item.useTime = finalUseTime;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            //// 根据当前阶段动态替换书签对应的 Tooltip
            //string stageKey = $"TooltipS{currentStage}"; // 生成对应阶段的书签键，例如 TooltipS0, TooltipS1

            // 根据当前阶段动态生成书签对应的 Tooltip 键
            string stageKey = currentStage switch
            {
                1 => WorldGen.crimson ? "TooltipS1S" : "TooltipS1F", // 阶段2：根据猩红或腐化选择
                2 => WorldGen.crimson ? "TooltipS2S" : "TooltipS2F", // 阶段3：根据猩红或腐化选择
                _ => $"TooltipS{currentStage}" // 其他阶段使用默认键
            };

            // 在提示信息中查找并替换书签 [Stage]
            list.FindAndReplace("[Stage]", this.GetLocalizedValue(stageKey));
        }

        //public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] == 0;

        //public override bool CanConsumeAmmo(Item ammo, Player player) => player.ownedProjectileCounts[Item.shoot] != 0;


        public override void HoldItem(Player player) => player.Calamity().mouseRotationListener = true;

        public static  Dictionary<int, Color> GelColors = new Dictionary<int, Color>
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

                    if (Main.projectile[projID].ModProjectile is TheGoldenFirePROJ fireProj)
                    {
                        Color ReadonlyFireColor = fireColor;
                        fireProj.FireColor = ReadonlyFireColor;
                    }
                }

                // 生成粒子特效
                //GenerateFireParticles(position, fireColor);
                // byd和尿尿一样，不删是人？
            }
            return false;
        }

        private void GenerateFireParticles(Vector2 position, Color fireColor)
        {
            Vector2 mousePosition = Main.MouseWorld; // 获取鼠标位置
            Vector2 directionToMouse = (mousePosition - position).SafeNormalize(Vector2.UnitX); // 计算朝向鼠标的方向

            for (int i = 0; i < 20; i++) // 每次射击生成20个粒子
            {
                float randomAngle = MathHelper.ToRadians(Main.rand.NextFloat(-15f, 15f)); // 随机偏移角度
                Vector2 adjustedDirection = directionToMouse.RotatedBy(randomAngle); // 方向带有随机偏移
                float speed = Main.rand.NextFloat(6f, 10f); // 随机速度
                Vector2 velocity = adjustedDirection * speed;
                int dustType = Main.rand.Next(new int[] { DustID.Torch, DustID.Lava, DustID.Smoke });

                // 应用 fireColor 进行粒子染色
                Dust.NewDustPerfect(position, dustType, velocity, 100, fireColor, 1.5f).noGravity = true;
            }
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

