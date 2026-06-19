using CalamityRangerExpansion.Content.DeveloperItems.Gel.Pyrogeist;

namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.PhotovisceratorRE
{
    internal class PhotovisceratorREGelPlayer : ModPlayer
    {
        public int CurrentGelAmmoType;

        public override void ResetEffects()
        {
            if (Player.HeldItem.type == ModContent.ItemType<Photoviscerator>())
                CurrentGelAmmoType = PhotovisceratorREGelCompat.FindSelectedExpansionGel(Player);
            else
                CurrentGelAmmoType = 0;
        }
    }

    internal class PhotovisceratorREGelItem : GlobalItem
    {
        public override void OnConsumeAmmo(Item weapon, Item ammo, Player player)
        {
            if (weapon.type == ModContent.ItemType<Photoviscerator>() && PhotovisceratorREGelCompat.IsExpansionGel(ammo.type))
                player.GetModPlayer<PhotovisceratorREGelPlayer>().CurrentGelAmmoType = ammo.type;
        }
    }

    internal class PhotovisceratorREGelProjectile : GlobalProjectile
    {
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (!PhotovisceratorREGelCompat.IsPhotovisceratorDamageProjectile(projectile.type))
                return;

            if (source is not EntitySource_Parent { Entity: Projectile parent } ||
                parent.type != ModContent.ProjectileType<PhotovisceratorHoldout>())
            {
                return;
            }

            Player owner = Main.player[projectile.owner];
            int gelType = owner.GetModPlayer<PhotovisceratorREGelPlayer>().CurrentGelAmmoType;
            if (!PhotovisceratorREGelCompat.IsExpansionGel(gelType))
                return;

            EntitySource_ItemUse_WithAmmo ammoSource = new(owner, owner.HeldItem, gelType);
            PhotovisceratorREGelCompat.ApplyGelOnSpawn(projectile, ammoSource, gelType);
        }
    }

    internal static class PhotovisceratorREGelCompat
    {
        public static bool IsPhotovisceratorDamageProjectile(int projectileType)
        {
            return projectileType == ModContent.ProjectileType<ExoFire>() ||
                projectileType == ModContent.ProjectileType<ExoLight>() ||
                projectileType == ModContent.ProjectileType<ExoFlareCluster>();
        }

        public static int FindSelectedExpansionGel(Player player)
        {
            for (int i = 54; i < 58; i++)
            {
                Item ammo = player.inventory[i];
                if (ammo.stack > 0 && ammo.ammo == AmmoID.Gel)
                    return IsExpansionGel(ammo.type) ? ammo.type : 0;
            }

            for (int i = 0; i < 54; i++)
            {
                Item ammo = player.inventory[i];
                if (ammo.stack > 0 && ammo.ammo == AmmoID.Gel)
                    return IsExpansionGel(ammo.type) ? ammo.type : 0;
            }

            return 0;
        }

        public static bool IsExpansionGel(int itemType)
        {
            return itemType == ModContent.ItemType<AerialiteGel>() ||
                itemType == ModContent.ItemType<GeliticGel>() ||
                itemType == ModContent.ItemType<HurricaneGel>() ||
                itemType == ModContent.ItemType<WulfrimGel>() ||
                itemType == ModContent.ItemType<CryonicGel>() ||
                itemType == ModContent.ItemType<StarblightSootGel>() ||
                itemType == ModContent.ItemType<AstralGel>() ||
                itemType == ModContent.ItemType<LifeAlloyGel>() ||
                itemType == ModContent.ItemType<LivingShardGel>() ||
                itemType == ModContent.ItemType<PerennialGel>() ||
                itemType == ModContent.ItemType<PlagueGel>() ||
                itemType == ModContent.ItemType<ScoriaGel>() ||
                itemType == ModContent.ItemType<BloodstoneCoreGel>() ||
                itemType == ModContent.ItemType<DivineGeodeGel>() ||
                itemType == ModContent.ItemType<EffulgentFeatherGel>() ||
                itemType == ModContent.ItemType<PolterplasmGel>() ||
                itemType == ModContent.ItemType<ToothGel>() ||
                itemType == ModContent.ItemType<UelibloomGel>() ||
                itemType == ModContent.ItemType<UnholyEssenceGel>() ||
                itemType == ModContent.ItemType<AuricGel>() ||
                itemType == ModContent.ItemType<CosmosGel>() ||
                itemType == ModContent.ItemType<EndothermicEnergyGel>() ||
                itemType == ModContent.ItemType<MiracleMatterGel>() ||
                itemType == ModContent.ItemType<Pyrogeist>();
        }

        public static void ApplyGelOnSpawn(Projectile projectile, EntitySource_ItemUse_WithAmmo ammoSource, int gelType)
        {
            if (gelType == ModContent.ItemType<AerialiteGel>())
                projectile.GetGlobalProjectile<AerialiteGelGP>().OnSpawn(projectile, ammoSource);
            else if (gelType == ModContent.ItemType<GeliticGel>())
                projectile.GetGlobalProjectile<GeliticGelGP>().OnSpawn(projectile, ammoSource);
            else if (gelType == ModContent.ItemType<HurricaneGel>())
                projectile.GetGlobalProjectile<HurricaneGelGP>().OnSpawn(projectile, ammoSource);
            else if (gelType == ModContent.ItemType<WulfrimGel>())
                projectile.GetGlobalProjectile<WulfrimGelGP>().OnSpawn(projectile, ammoSource);
            else if (gelType == ModContent.ItemType<CryonicGel>())
                projectile.GetGlobalProjectile<CryonicGelGP>().OnSpawn(projectile, ammoSource);
            else if (gelType == ModContent.ItemType<StarblightSootGel>())
                projectile.GetGlobalProjectile<StarblightSootGelGP>().OnSpawn(projectile, ammoSource);
            else if (gelType == ModContent.ItemType<AstralGel>())
                projectile.GetGlobalProjectile<AstralGelGP>().OnSpawn(projectile, ammoSource);
            else if (gelType == ModContent.ItemType<LifeAlloyGel>())
                projectile.GetGlobalProjectile<LifeAlloyGelGP>().OnSpawn(projectile, ammoSource);
            else if (gelType == ModContent.ItemType<LivingShardGel>())
                projectile.GetGlobalProjectile<LivingShardGelGP>().OnSpawn(projectile, ammoSource);
            else if (gelType == ModContent.ItemType<PerennialGel>())
                projectile.GetGlobalProjectile<PerennialGelGP>().OnSpawn(projectile, ammoSource);
            else if (gelType == ModContent.ItemType<PlagueGel>())
                projectile.GetGlobalProjectile<PlagueGelGP>().OnSpawn(projectile, ammoSource);
            else if (gelType == ModContent.ItemType<ScoriaGel>())
                projectile.GetGlobalProjectile<ScoriaGelGP>().OnSpawn(projectile, ammoSource);
            else if (gelType == ModContent.ItemType<BloodstoneCoreGel>())
                projectile.GetGlobalProjectile<BloodstoneCoreGelGP>().OnSpawn(projectile, ammoSource);
            else if (gelType == ModContent.ItemType<DivineGeodeGel>())
                projectile.GetGlobalProjectile<DivineGeodeGelGP>().OnSpawn(projectile, ammoSource);
            else if (gelType == ModContent.ItemType<EffulgentFeatherGel>())
                projectile.GetGlobalProjectile<EffulgentFeatherGelGP>().OnSpawn(projectile, ammoSource);
            else if (gelType == ModContent.ItemType<PolterplasmGel>())
                projectile.GetGlobalProjectile<PolterplasmGelGP>().OnSpawn(projectile, ammoSource);
            else if (gelType == ModContent.ItemType<ToothGel>())
                projectile.GetGlobalProjectile<ToothGelGP>().OnSpawn(projectile, ammoSource);
            else if (gelType == ModContent.ItemType<UelibloomGel>())
                projectile.GetGlobalProjectile<UelibloomGelGP>().OnSpawn(projectile, ammoSource);
            else if (gelType == ModContent.ItemType<UnholyEssenceGel>())
                projectile.GetGlobalProjectile<UnholyEssenceGelGP>().OnSpawn(projectile, ammoSource);
            else if (gelType == ModContent.ItemType<AuricGel>())
                projectile.GetGlobalProjectile<AuricGelGP>().OnSpawn(projectile, ammoSource);
            else if (gelType == ModContent.ItemType<CosmosGel>())
                projectile.GetGlobalProjectile<CosmosGelGP>().OnSpawn(projectile, ammoSource);
            else if (gelType == ModContent.ItemType<EndothermicEnergyGel>())
                projectile.GetGlobalProjectile<EndothermicEnergyGelGP>().OnSpawn(projectile, ammoSource);
            else if (gelType == ModContent.ItemType<MiracleMatterGel>())
                projectile.GetGlobalProjectile<MiracleMatterGelGP>().OnSpawn(projectile, ammoSource);
            else if (gelType == ModContent.ItemType<Pyrogeist>())
                projectile.GetGlobalProjectile<PyrogeistGP>().OnSpawn(projectile, ammoSource);
        }
    }
}
