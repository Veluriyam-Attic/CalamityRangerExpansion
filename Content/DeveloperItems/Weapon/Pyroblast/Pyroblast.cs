using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using CalamityMod;
using CalamityMod.Items.Ammo;
using CalamityMod.Items.Materials;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.NPCs.Astral;
using CalamityMod.Items;
using CalamityMod.Rarities;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Particles;
using CalamityMod.NPCs.VanillaNPCAIOverrides.Bosses;

namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.Pyroblast
{
    public class Pyroblast : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.Pyroblast";
        public static readonly int OriginalUseTime = 30;

        public override void SetDefaults()
        {
            Item.damage = 277;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = OriginalUseTime;
            Item.shoot = ModContent.ProjectileType<PyroblastHoldOut>();
            Item.shootSpeed = 15f;
            Item.knockBack = 6.5f;

            Item.width = 96;
            Item.height = 42;
            Item.noMelee = true;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.useAmmo = AmmoID.Bullet;

            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.Calamity().devItem = true;
            Item.useStyle = ItemUseStyleID.Shoot;
        }
      
        public override bool CanConsumeAmmo(Item ammo, Player player) => player.ownedProjectileCounts[Item.shoot] != 0;

        public override void HoldItem(Player player) => player.Calamity().mouseRotationListener = true;

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override bool AltFunctionUse(Player player) => true;
        public override Vector2? HoldoutOffset() => new Vector2(-25, 0);

        public override bool CanUseItem(Player player)
        {
            // 平衡准则：
            // 右键追踪略高于死神濯身20%
            // 左见群体约等于纯原怒野110%的水平
            if (player.altFunctionUse == 2) // 右键逻辑
            {
                Item.noUseGraphic = false; // 显示武器
                Item.noMelee = false; // 启用近战
                Item.damage = 700; // 设置右键伤害
                Item.useTime = Item.useAnimation = 60; // 使用时间
                Item.shoot = ModContent.ProjectileType<AuricArrowBALL>(); // 右键发射的弹幕
                Item.shootSpeed = 10f; // 设置弹幕速度
                Item.UseSound = SoundID.Item61; // 播放右键音效
            }
            else // 左键逻辑
            {
                Item.noUseGraphic = true;
                Item.noMelee = true;
                Item.damage = 350;
                Item.useTime = Item.useAnimation = OriginalUseTime;
                Item.shoot = ModContent.ProjectileType<PyroblastHoldOut>();
                Item.shootSpeed = 15f;
                Item.UseSound = null; // 左键不播放音效
            }
            return true;
        }

        public override bool Shoot(Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2) // 右键逻辑
            {
                //// 释放8个弹幕，围绕玩家均匀分布
                //for (int i = 0; i < 8; i++)
                //{
                //    float angle = MathHelper.TwoPi / 8 * i; // 计算每个弹幕的角度
                //    Vector2 spawnVelocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 7f; // 计算速度
                //    Projectile projectile = Projectile.NewProjectileDirect(
                //        source,
                //        player.Center,
                //        spawnVelocity,
                //        ModContent.ProjectileType<AuricArrowBALL>(),
                //        damage,
                //        knockback,
                //        player.whoAmI
                //    );
                //    projectile.scale = 1.7f; // 设置弹幕的缩放比例
                //}

                //// 释放粒子特效
                //for (int i = 0; i < 50; i++)
                //{
                //    // 随机生成的环形粒子
                //    float angle = Main.rand.NextFloat(MathHelper.TwoPi);
                //    float distance = Main.rand.NextFloat(20f, 80f);
                //    Vector2 dustPosition = player.Center + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * distance;
                //    Dust dust = Dust.NewDustPerfect(dustPosition, DustID.GoldCoin, Vector2.Zero, 150, Color.LightYellow, 3.5f);
                //    dust.velocity = Vector2.UnitY.RotatedBy(angle) * Main.rand.NextFloat(2f, 4f);
                //    dust.noGravity = true;
                //}

                //// 释放规则粒子链
                //for (int i = 0; i < 8; i++)
                //{
                //    float angle = MathHelper.TwoPi / 8 * i;
                //    for (int j = 1; j <= 5; j++) // 每条链包含多个粒子
                //    {
                //        Vector2 chainPosition = player.Center + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * j * 15f;
                //        Dust chainDust = Dust.NewDustPerfect(chainPosition, DustID.DesertTorch, Vector2.Zero, 100, Color.Orange, 2.5f + j * 0.1f);
                //        chainDust.velocity = Vector2.Zero; // 静止粒子
                //        chainDust.noGravity = true;
                //    }
                //}

                //// 释放鼠标方向粒子
                //for (int i = 0; i < 12; i++)
                //{
                //    float randomAngle = Main.rand.NextFloat(-MathHelper.ToRadians(15), MathHelper.ToRadians(15)); // 随机角度范围 -15 至 15 度
                //    Vector2 direction = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX).RotatedBy(randomAngle);
                //    Vector2 positionOffset = player.Center + direction * Main.rand.NextFloat(10f, 40f); // 随机位置偏移
                //    Particle smoke = new HeavySmokeParticle(
                //        positionOffset,
                //        direction * Main.rand.NextFloat(12f, 42f), // 随机速度
                //        Color.Lerp(Color.Orange, Color.Yellow, Main.rand.NextFloat()), // 随机混合颜色
                //        Main.rand.Next(30, 60), // 粒子存活时间
                //        Main.rand.NextFloat(0.5f, 1.25f), // 粒子大小
                //        1.0f,
                //        MathHelper.ToRadians(Main.rand.NextFloat(-5f, 5f)), // 随机旋转速度
                //        true // 强视觉效果
                //    );
                //    GeneralParticleHandler.SpawnParticle(smoke);
                //}

                // **平行发射 5 发 PyroblastRocketScorpio 火箭弹**
                for (int i = 0; i < 5; i++)
                {
                    // **计算水平偏移**
                    Vector2 offset = Vector2.Normalize(velocity.RotatedBy(MathHelper.PiOver2));
                    Vector2 spawnPosition = position + offset * Main.rand.NextFloat(-19f, 19f);

                    // **发射火箭弹**
                    Projectile.NewProjectileDirect(
                        source,
                        spawnPosition,
                        velocity, // 速度与原始方向一致
                        ModContent.ProjectileType<PyroblastRocketScorpio>(), // 替换成火箭弹
                        damage,
                        knockback,
                        player.whoAmI
                    );
                }
            }
            else // 左键逻辑
            {
                Projectile.NewProjectileDirect(
                    source,
                    player.MountedCenter,
                    Vector2.Zero,
                    ModContent.ProjectileType<PyroblastHoldOut>(),
                    Item.damage,
                    Item.knockBack,
                    player.whoAmI
                ).velocity = (player.Calamity().mouseWorld - player.MountedCenter).SafeNormalize(Vector2.Zero);
            }
            return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe(1);
            recipe.AddIngredient<PlagueTaintedSMG>(1);
            recipe.AddIngredient<Lazhar>(1);
            recipe.AddIngredient<Scorpio>(1);
            recipe.AddIngredient(ItemID.IllegalGunParts, 2);
            recipe.AddIngredient(ItemID.LunarBar, 10);
            recipe.AddIngredient<DivineGeode>(10);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            //if (Main.zenithWorld)
            //醉酒世界drunkWorld
            //困难世界getGoodWorld
            //10周年世界tenthAnniversaryWorld
            //饥荒世界dontStarveWorld
            //蜜蜂世界notTheBeesWorld
            //地下世界remixWorld
            //陷阱世界noTrapsWorld
            //天顶世界zenithWorld

            // 第1阶段
            bool kingSlime = NPC.downedSlimeKing; // 史莱姆王
            bool eyeOfCthulhu = NPC.downedBoss1; // 眼睛
            bool desertScourge = DownedBossSystem.downedDesertScourge; // 荒漠灾虫
            bool crabulon = DownedBossSystem.downedCrabulon; // 螃蟹
            bool eaterOfWorldsOrBrain = NPC.downedBoss2; // EoW BoC
            bool hiveMind = DownedBossSystem.downedHiveMind; // 意志
            bool perforator = DownedBossSystem.downedPerforator; // 宿主
            bool queenBee = NPC.downedQueenBee; // 蜂后
            bool skeletron = NPC.downedBoss3; // 骷髅王
            bool deerclops = NPC.downedDeerclops; // 独眼巨鹿
            bool slimeGod = DownedBossSystem.downedSlimeGod; // 史莱姆之神
            
            // 第2阶段
            bool wallOfFlesh = Main.hardMode; // 肉山
            bool QueenSlime = NPC.downedQueenSlime; // 史莱姆女王
            bool mechanicalBosses1 = NPC.downedMechBoss1; // 毁灭者
            bool mechanicalBosses2 = NPC.downedMechBoss2; // 双子魔眼
            bool mechanicalBosses3 = NPC.downedMechBoss3; // 机械骷髅王
            bool brimstoneElemental = DownedBossSystem.downedBrimstoneElemental; // 硫磺火元素
            bool cryogen = DownedBossSystem.downedCryogen; // 极地冰灵
            bool aquaticScourge = DownedBossSystem.downedAquaticScourge; // 渊海灾虫

            // 第3阶段
            bool plantera = NPC.downedPlantBoss; // 花
            bool calamitas = DownedBossSystem.downedCalamitasClone; // 灾厄之影
            bool Leviathan = DownedBossSystem.downedLeviathan; // 利维坦
            bool astrumAureus = DownedBossSystem.downedAstrumAureus; // 白金
            bool golem = NPC.downedGolemBoss; // 石
            bool empress = NPC.downedEmpressOfLight; // 女皇
            bool fishron = NPC.downedFishron; // 公爵
            bool plaguebringer = DownedBossSystem.downedPlaguebringer; // 瘟疫使者歌莉娅
            bool ravager = DownedBossSystem.downedRavager; // 毁灭魔像
            bool cultist = NPC.downedAncientCultist; // 教徒
            bool astrumDeus = DownedBossSystem.downedAstrumDeus; // 星神游龙

            // 第4阶段
            bool moonLord = NPC.downedMoonlord; // 月总 
            bool dragonfolly = DownedBossSystem.downedDragonfolly; // 金龙
            bool guardians = DownedBossSystem.downedGuardians; // 亵渎守卫
            bool providence = DownedBossSystem.downedProvidence; // 亵渎天神
            bool ceaselessVoid = DownedBossSystem.downedCeaselessVoid; // 无尽虚空
            bool signus = DownedBossSystem.downedSignus; // 西格纳斯
            bool stormWeaver = DownedBossSystem.downedStormWeaver; // 风暴编织者
            bool polterghast = DownedBossSystem.downedPolterghast; // 幽花
            bool boomerDuke = DownedBossSystem.downedBoomerDuke; // 老核弹

            // 第5阶段
            bool devourerOfGods = DownedBossSystem.downedDoG; // 神吞
            bool yharon = DownedBossSystem.downedYharon; // 龙
            bool exoMechs = DownedBossSystem.downedExoMechs; // 巨械
            bool calamitasClone = DownedBossSystem.downedCalamitas; // 至尊灾厄
            bool BR = DownedBossSystem.downedBossRush; // BR

            // 这byd忍不了一点，单独列几个阴间的出来：
            // downedGSS：大沙狂鲨
            // downedCLAM：巨像蛤
            // downedCLAMHardMode：肉后巨像蛤
            // downedPlaguebringer：瘟疫使者（是boss）
            // CragmawMire：伽玛史莱姆
            // downedDreadnautilus：恐惧鹦鹉螺
            // Mauler：渊海狂鲨
            // CragmawMire：伽玛史莱姆
            // downedPrimordialWyrm：妖龙
        }

    }
}






