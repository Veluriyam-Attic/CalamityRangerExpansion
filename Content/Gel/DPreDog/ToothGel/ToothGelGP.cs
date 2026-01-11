namespace CalamityRangerExpansion.Content.Gel.DPreDog.ToothGel
{
    internal class ToothGelGP : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool IsToothGelInfused = false;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_ItemUse_WithAmmo ammoSource && ammoSource.AmmoItemIdUsed == ModContent.ItemType<ToothGel>())
            {
                IsToothGelInfused = true;
                projectile.netUpdate = true;
                projectile.damage = (int)(projectile.damage * 1.1f); // 增加 10% 伤害


            }
            base.OnSpawn(projectile, source);
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (IsToothGelInfused && target.active && !target.friendly)
            {
                target.AddBuff(ModContent.BuffType<CrushDepth>(), 300); // 深渊水压

                // 用的是反器材大狙弹幕（AMRShot）那里的代码
                target.Calamity().miscDefenseLoss = 30;
            }
        }
    }
}

