namespace CalamityRangerExpansion.Content.Gel.DPreDog.UnholyEssenceGel
{
    internal class UnholyEssenceGelGP : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool IsUnholyEssenceGelInfused = false;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_ItemUse_WithAmmo ammoSource && ammoSource.AmmoItemIdUsed == ModContent.ItemType<UnholyEssenceGel>())
            {
                IsUnholyEssenceGelInfused = true;
                projectile.damage = (int)(projectile.damage * 1f); // 增加 25% 伤害
                projectile.netUpdate = true;
            }
            base.OnSpawn(projectile, source);
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (IsUnholyEssenceGelInfused && target.active && !target.friendly)
            {
                // 施加 HolyFlames Buff，持续 300 帧（5 秒）
                target.AddBuff(ModContent.BuffType<HolyFlames>(), 300);
            }
        }
    }
}
