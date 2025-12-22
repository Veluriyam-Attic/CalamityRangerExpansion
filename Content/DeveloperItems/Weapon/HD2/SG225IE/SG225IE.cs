using CalamityMod;
using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.HD2.SG225IE
{
    public class SG225IE : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.SG225IE";
        public override void SetDefaults()
        {
            Item.damage = 26;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 36;
            Item.shoot = ModContent.ProjectileType<SG225IEHoldOut>();
            Item.shootSpeed = 15f;

            Item.width = 80;
            Item.height = 28;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.useAmmo = AmmoID.Bullet;
            Item.value = Item.buyPrice(0, 10, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = new SoundStyle("CalamityMod/Sounds/Item/DudFire") with { Volume = 0.5f, Pitch = -0.6f };
            Item.Calamity().devItem = true;
            Item.Calamity().canFirePointBlankShots = true;

        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] == 0;

        public override bool CanConsumeAmmo(Item ammo, Player player) => player.ownedProjectileCounts[Item.shoot] != 0;

        public override void HoldItem(Player player) => player.Calamity().mouseWorldListener = true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile holdout = Projectile.NewProjectileDirect(source, player.MountedCenter, Vector2.Zero, Item.shoot, 0, 0f, player.whoAmI);
            holdout.velocity = (player.Calamity().mouseWorld - player.MountedCenter).SafeNormalize(Vector2.UnitX);
            return false;
        }



    }
}
