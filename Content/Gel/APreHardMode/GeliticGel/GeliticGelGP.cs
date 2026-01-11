namespace CalamityRangerExpansion.Content.Gel.APreHardMode.GeliticGel
{
    internal class GeliticGelGP : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool IsGeliticGelInfused = false;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_ItemUse_WithAmmo ammoSource && ammoSource.AmmoItemIdUsed == ModContent.ItemType<GeliticGel>())
            {
                IsGeliticGelInfused = true;
                projectile.netUpdate = true;
                projectile.damage = (int)(projectile.damage * 0.975f);
            }
            base.OnSpawn(projectile, source);
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (IsGeliticGelInfused && target.active && !target.friendly)
            {
                Player player = Main.player[projectile.owner];

                if (player.ZoneCorrupt || player.ZoneCrimson)
                {
                    // 在邪恶地形下，施加 BurningBlood 和 BrainRot
                    target.AddBuff(ModContent.BuffType<BurningBlood>(), 300); // 5 秒
                    target.AddBuff(ModContent.BuffType<BrainRot>(), 300);     // 5 秒
                }
                else if (player.ZoneHallow)
                {
                    // 在神圣地形下，施加 HolyFlames 和 Confused困惑（31号原版 Buff）
                    target.AddBuff(ModContent.BuffType<HolyFlames>(), 300); // 5 秒
                    target.AddBuff(31, 300);                                // 5 秒
                }
                target.AddBuff(BuffID.Slimed, 300); // 原版的史莱姆效果
            }
        }
    }
}
