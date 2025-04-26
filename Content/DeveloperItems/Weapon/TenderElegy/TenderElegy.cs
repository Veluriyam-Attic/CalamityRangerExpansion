using CalamityMod.Items;
using CalamityMod.Rarities;
using CalamityMod;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Weapons.Ranged;

namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.TenderElegy
{
    public class TenderElegy : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.TenderElegy";
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<TenderElegyi>();
        }
        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 60;
            Item.damage = 666;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 3;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 2f;
            Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
            Item.shootSpeed = 19f;
            Item.shoot = ProjectileID.WoodenArrowFriendly; // 仅作为默认值，实际箭矢类型取决于使用的弹药
            Item.useAmmo = AmmoID.Arrow; // 确保消耗箭矢

            Item.value = CalamityGlobalItem.RarityHotPinkBuyPrice;
            Item.rare = ModContent.RarityType<HotPink>();
            Item.Calamity().devItem = true;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-17, 0);

        //public override bool Shoot(Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        //{
        //    Vector2 sourcePos = player.RotatedRelativePoint(player.MountedCenter, true); // 获取旋转后的玩家中心点
        //    float spreadAngle = MathHelper.Pi * 0.0f; // ? π 角度偏移
        //    int arrowCount = 1; // 并排箭矢数量
        //    float spacing = 1f; // 箭矢间距，单位：像素

        //    velocity.Normalize();
        //    velocity *= Item.shootSpeed; // 归一化后重新赋予速度

        //    // 计算箭矢发射是否可穿过障碍物
        //    bool canHit = Collision.CanHit(sourcePos, 0, 0, sourcePos + velocity, 0, 0);

        //    for (int i = 0; i < arrowCount; i++)
        //    {
        //        float offsetIndex = i - (arrowCount - 1) / 2f; // 计算箭矢相对中心的偏移量

        //        // 计算发射起点，使箭矢从不同位置发射
        //        Vector2 perpOffset = velocity.RotatedBy(MathHelper.PiOver2) * offsetIndex * spacing; // 获取垂直方向偏移
        //        Vector2 firePosition = sourcePos + perpOffset;

        //        // 计算箭矢方向，使其保持一致
        //        Vector2 arrowVelocity = velocity.RotatedBy(spreadAngle * offsetIndex);
        //        if (!canHit)
        //            firePosition -= velocity; // 如果有障碍物，则将起点稍微向后调整，避免直接撞墙

        //        // 生成箭矢
        //        Projectile arrow = Projectile.NewProjectileDirect(source, firePosition, arrowVelocity, type, damage, knockback, player.whoAmI);

        //        // 赋予特效
        //        TenderElegyGPROJ cgp = arrow.GetGlobalProjectile<TenderElegyGPROJ>();
        //        cgp.isSpecialArrow = true;
        //    }
        //    return false; // 阻止默认的单箭矢发射
        //}
        public override bool Shoot(Player player, Terraria.DataStructures.EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 sourcePos = player.RotatedRelativePoint(player.MountedCenter, true); // 获取旋转后的玩家中心点
            float spreadAngle = MathHelper.Pi * 0.0f; // 角度偏移
            int arrowCount = 1; // 发射的箭矢数量
            float spacing = 1f; // 箭矢之间的间隔

            velocity.Normalize();
            velocity *= Item.shootSpeed; // 归一化后重新赋予速度

            // 确保新的弹幕类型始终为 TenderElegyPROJ
            int projType = ModContent.ProjectileType<TenderElegyPROJ>();

            // 计算箭矢发射是否可穿过障碍物
            bool canHit = Collision.CanHit(sourcePos, 0, 0, sourcePos + velocity, 0, 0);

            for (int i = 0; i < arrowCount; i++)
            {
                float offsetIndex = i - (arrowCount - 1) / 2f; // 计算箭矢相对中心的偏移量

                // 计算发射起点，使箭矢从不同位置发射
                Vector2 perpOffset = velocity.RotatedBy(MathHelper.PiOver2) * offsetIndex * spacing;
                Vector2 firePosition = sourcePos + perpOffset;

                // 计算箭矢方向，使其保持一致
                Vector2 projVelocity = velocity.RotatedBy(spreadAngle * offsetIndex);
                if (!canHit)
                    firePosition -= velocity; // 如果有障碍物，则将起点稍微向后调整，避免直接撞墙

                // 生成新的 TenderElegyPROJ 弹幕
                Projectile.NewProjectileDirect(source, firePosition, projVelocity, projType, damage, knockback, player.whoAmI);
            }

            return false; // 阻止默认的单箭矢发射
        }


        public override bool CanRightClick()
        {
            return base.CanRightClick();
        }
    }
}
