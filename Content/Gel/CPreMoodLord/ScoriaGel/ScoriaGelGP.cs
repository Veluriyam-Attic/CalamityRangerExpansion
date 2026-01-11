namespace CalamityRangerExpansion.Content.Gel.CPreMoodLord.ScoriaGel
{
    public class ScoriaGelGP : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool IsScoriaGelInfused = false;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_ItemUse_WithAmmo ammoSource && ammoSource.AmmoItemIdUsed == ModContent.ItemType<ScoriaGel>())
            {
                IsScoriaGelInfused = true;
                projectile.netUpdate = true;
                projectile.damage = (int)(projectile.damage * 0.95f); // 减少 5% 伤害
            }
            base.OnSpawn(projectile, source);
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (IsScoriaGelInfused && target.active && !target.friendly)
            {
                // 启用 ScoriaGelGN 的标记和计时器
                target.GetGlobalNPC<ScoriaGelGN>().IsMarkedByScoriaGel = true;
                target.GetGlobalNPC<ScoriaGelGN>().MarkDuration = 300; // 持续 5 秒
            }
        }
    }
}

