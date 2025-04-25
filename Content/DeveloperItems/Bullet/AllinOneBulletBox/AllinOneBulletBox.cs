//using CalamityMod.Items.Materials;
//using CalamityMod.Items;
//using CalamityMod.Rarities;
//using CalamityRangerExpansion.Content.DeveloperItems.Bullet.Che;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Terraria.ID;
//using Terraria.ModLoader;
//using Terraria;
//using CalamityMod;
//using CalamityRangerExpansion.Content.Ammunition.CPreMoodLord.ScoriaBullet;
//using CalamityRangerExpansion.Content.Ammunition.CPreMoodLord.PlagueBullet;
//using CalamityRangerExpansion.Content.Ammunition.CPreMoodLord.AstralBullet;
//using CalamityMod.Projectiles.Ranged;
//using Terraria.Audio;
//using CalamityMod.Items.Placeables;
//using CalamityRangerExpansion.Content.Ammunition.APreHardMode.AerialiteBullet;
//using Microsoft.Xna.Framework;

//namespace CalamityRangerExpansion.Content.DeveloperItems.Bullet.AllinOneBulletBox
//{
//    internal class AllinOneBulletBox : ModItem, ILocalizedModType
//    {
//        public new string LocalizationCategory => "DeveloperItems.AllinOneBulletBox";
//        // **电量参数**
//        public float MaxCharge = 50f;
//        public float CurrentCharge;
//        public float ChargePerUse = 0.05f;
//        public override void SetDefaults()
//        {
//            Item.damage = 26;
//            Item.DamageType = DamageClass.Ranged;
//            Item.width = 14;
//            Item.height = 32;
//            Item.maxStack = 1;
//            Item.consumable = false; // 弹药是消耗品
//            Item.knockBack = 3.5f;

//            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
//            Item.rare = ModContent.RarityType<HotPink>();
//            Item.Calamity().devItem = true;
//            Item.shoot = AmmoTypes[currentAmmoIndex]; // 初始弹药
//            Item.shootSpeed = 6f;
//            Item.ammo = AmmoID.Bullet; // 这是子弹类型的弹药
//            //Item.accessory = true;

//            CalamityGlobalItem modItem = Item.Calamity();
//            // **初始化电量**
//            Item.Calamity().UsesCharge = true;
//            Item.Calamity().MaxCharge = MaxCharge;
//            Item.Calamity().ChargePerUse = ChargePerUse;
//            CurrentCharge = MaxCharge;
//        }

//        //public override void UpdateAccessory(Player player, bool hideVisual)
//        //{
//        //    player.GetModPlayer<AllinOneBulletBoxPlayer>().BulletBoxActive = true;
//        //}


//        public override bool CanUseItem(Player player)
//        {
//            // **如果电量不足，不允许使用**
//            if (CurrentCharge <= 0)
//            {
//                Main.NewText("电量不足，无法使用！", 255, 0, 0);
//                return false;
//            }
//            return true;
//        }

//        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
//        {
//            // **检查是否有足够的电量**
//            if (CurrentCharge > 0)
//            {
//                CurrentCharge -= ChargePerUse; // 消耗电量
//                type = AmmoTypes[currentAmmoIndex]; // 选择当前弹药
//            }
//            else
//            {
//                Main.NewText("弹药箱电量耗尽！", 255, 0, 0);
//                type = 0; // 不发射子弹
//            }
//        }

//        // 五种不同的弹药
//        private static readonly int[] AmmoTypes = new int[]
//        {
//            ModContent.ProjectileType<AOBB1_PROJ>(),   // 1 号弹药（默认解锁）
//            ModContent.ProjectileType<AOBB2_PROJ>(),   // 2 号弹药（机械骷髅王后解锁）
//            ModContent.ProjectileType<AOBB3_PROJ>(),   // 3 号弹药（瘟疫使者 Goliath 后解锁）
//            ModContent.ProjectileType<AOBB4_PROJ>(), // 4 号弹药（圣洁之神 Providence 后解锁）
//            ModContent.ProjectileType<AOBB5_PROJ>()   // 5 号弹药（DoG 后解锁）
//        };

//        // 记录当前选中的弹药索引
//        private int currentAmmoIndex = 0;

//        public override bool CanRightClick()
//        {
//            return true;
//        }
//        public override bool ConsumeItem(Player player)
//        {
//            return false;
//        }
//        public override void RightClick(Player player)
//        {
//            int maxIndex = GetUnlockedAmmoIndex();
//            if (maxIndex == 0)
//            {
//                Main.NewText("你还没有解锁额外弹药！", 255, 50, 50);
//                return;
//            }

//            // 在已解锁的范围内循环切换
//            currentAmmoIndex = (currentAmmoIndex + 1) % (maxIndex + 1);
//            Item.shoot = AmmoTypes[currentAmmoIndex];

//            // 播放切换音效
//            SoundEngine.PlaySound(SoundID.Item149, player.position);

//            // 提示玩家当前的弹药类型
//            Main.NewText($"当前弹药: {currentAmmoIndex + 1} 号", 255, 255, 0);
//        }


//        // 计算当前解锁的最大弹药索引
//        private int GetUnlockedAmmoIndex()
//        {
//            if (DownedBossSystem.downedDoG) return 4;
//            if (DownedBossSystem.downedProvidence) return 3;
//            if (DownedBossSystem.downedPlaguebringer) return 2;
//            if (NPC.downedMechBoss3) return 1;
//            return 0;
//        }


//        public override void AddRecipes()
//        {
//            CreateRecipe().
//                AddIngredient<MysteriousCircuitry>(5).
//                AddIngredient<DubiousPlating>(7).
//                AddIngredient<AerialiteBar>(4).
//                AddIngredient<SeaPrism>(7).
//                AddIngredient<AerialiteBullet>(333).
//                AddTile(TileID.Anvils).
//                Register();
//        }
//    }
//}









////实验弹药箱：作为弹药被消耗，放在背包里右键切换形态，分别有5个阶段跟图纸一个时期，随流程解锁5种形态
////思考一下他们武器的特色是什么？
////嘉登造物系列的大多数都是纯用粒子特效，少数使用特殊特效，因此这一系列除了最后一阶段以外，剩下的全部使用纯粒子特效
////1 ：子弹击中敌人一次之后会穿过去，逐渐减速，随后回旋
////2 ：飞行一段距离之后传到玩家后方造成二次伤害，但传送之前仅一次伤害，传送之后依旧仅一次伤害，只是传送之后伤害更高而已
////3 ：击中一个敌人之后停留在原地，高频造成6次伤害会消失，消失时产生伤害很低的小范围爆炸，距离玩家更近时伤害更高
////4 ：击中后从它下方很远的地方散射喷发出2~3条激光能量束，能够穿透6个敌人
////5 ：模仿卡拉萨瓦、特斯拉炮的那种超高更新速度的纯粒子效果，但是会飞行期间左右移动，具有微弱的追踪性能，持续攻击一个敌人能够提高伤害，但是有上限

////1    旋回幽刃弹    Phantom Recurve Bullet    穿透后减速，随后回旋返回
////2    量子折跃弹    Quantum Warp Bullet    飞行后传送到玩家身后，二次高伤伤害
////3    亥姆霍兹陷阱弹    Helmholtz Trap Bullet    停留原地高频伤害，最后爆炸，近距离更痛
////4    旭日崩解弹    Solar Disruption Bullet    触发远程散射激光，能量束贯穿敌人
////5    震荡超粒子弹    Resonance Hyper-Particle    高速粒子武器，微弱追踪，持续攻击增伤