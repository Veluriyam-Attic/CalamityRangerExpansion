using CalamityMod;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.HD2.R36
{
    public class R36 : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.R36";

        public override void SetDefaults()
        {
            Item.width = 80;
            Item.height = 30;
            Item.damage = 105;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 75;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 8f;
            Item.value = Item.buyPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.Yellow;
            //Item.UseSound = SoundID.Item61;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<R36Proj>();
            Item.shootSpeed = 25f;
            Item.useAmmo = AmmoID.Bullet;
            Item.scale = 0.9f;
            Item.Calamity().devItem = true;


            Item.UseSound = new SoundStyle("CalamityRangerExpansion/Content/DeveloperItems/Weapon/R36/爆裂铳开火")
            {
                Volume = 1.0f,
                Pitch = 0f
            };
        }

        public override Vector2? HoldoutOffset() => new Vector2(-40, -7);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // 获取玩家使用的真实弹药类型
            //if (player.PickAmmo(Item, out _, out _, out _, out _, out int ammoType))  
            //{
            //    // 发射固定弹幕 R36Proj，并传入 ammoType 作为 ai[1]
            //    int projID = Projectile.NewProjectile(
            //        source,
            //        position,
            //        velocity,
            //        ModContent.ProjectileType<R36Proj>(),
            //        damage,
            //        knockback,
            //        player.whoAmI
            //    );

            //    // ⚠️ 安全检查：是否创建成功
            //    if (Main.projectile.IndexInRange(projID))
            //    {
            //        Main.projectile[projID].ai[1] = ammoType; // 手动写入 ai[1]
            //    }
            //} 这个是错误的，因为他直接把id和编号混淆了

            if (player.PickAmmo(Item, out int projType, out _, out _, out _, out _))
            {
                position += (player.Calamity().mouseWorld - player.MountedCenter).SafeNormalize(Vector2.UnitX) * 85f; // 往前面偏移，实现枪口射出

                int projID = Projectile.NewProjectile(
                    source,
                    position,
                    velocity,
                    ModContent.ProjectileType<R36Proj>(),
                    damage,
                    knockback,
                    player.whoAmI
                );

                if (Main.projectile.IndexInRange(projID))
                {
                    Main.projectile[projID].ai[1] = projType; // ✅传入的是弹药的真实弹幕类型，而不是弹药 item ID
                }
            }

            //这里解释一下是为什么:
            //player.PickAmmo(Item, out _, out _, out _, out _, out int ammoType)
            //这里的 ammoType，是指：
            //玩家所使用的“弹药物品”的 ID，比如 ItemID.MusketBall，或者 ModContent.ItemType<ExplodingAmmo>()
            //这代表的是物品，而 不是发射出来的那个“弹幕”。
            //Projectile.NewProjectile(..., ammoType)
            //⚠️也就是说你把“子弹物品 ID”当成了“弹幕 ID”去发射！
            //这时候系统会在 ProjectileID 表里查找 ammoType 所对应的弹幕，但：
            //原版游戏中有些子弹其实根本没有对应弹幕（比如火弹、银弹等只是给普通子弹添加特效）
            //安装多个 mod 后，弹幕 ID 顺序发生改变

            //而正确写法则是这一段:
            //player.PickAmmo(Item, out int projType, out _, out _, out _, out _)
            //这个 projType 是：
            //PickAmmo 内部根据当前弹药，动态决定的弹幕类型 ID
            //✅ 它是拿来发射弹幕的！专用于 Projectile.NewProjectile



            return false; // 阻止默认发射行为
        }

        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.ChangeDir(Math.Sign((Main.MouseWorld - player.Center).X));
            float itemRotation = player.compositeFrontArm.rotation + MathHelper.PiOver2 * player.gravDir;

            Vector2 itemPosition = player.MountedCenter + itemRotation.ToRotationVector2() * 35f;
            Vector2 itemSize = new Vector2(Item.width, Item.height);
            Vector2 itemOrigin = new Vector2(-5, 6);

            CalamityUtils.CleanHoldStyle(player, itemRotation, itemPosition, itemSize, itemOrigin);
            base.UseStyle(player, heldItemFrame);
        }

        public override void UseItemFrame(Player player)
        {
            player.ChangeDir(Math.Sign((Main.MouseWorld - player.Center).X));

            float animProgress = 1 - player.itemTime / (float)player.itemTimeMax;
            float rotation = (player.Center - Main.MouseWorld).ToRotation() * player.gravDir + MathHelper.PiOver2;

            if (animProgress < 0.5f)
            {
                rotation += -0.45f * (float)Math.Pow((0.5f - animProgress) / 0.5f, 2) * player.direction;
            }

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);

            // 后臂动作（模拟拉动枪机）
            if (animProgress > 0.5f)
            {
                float backArmRotation = rotation + 0.52f * player.direction;
                Player.CompositeArmStretchAmount stretch = ((float)Math.Sin(MathHelper.Pi * (animProgress - 0.5f) / 0.36f)).ToStretchAmount();
                player.SetCompositeArmBack(true, stretch, backArmRotation);
            }
        }








    }
}
