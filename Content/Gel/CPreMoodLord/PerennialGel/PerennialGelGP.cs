namespace CalamityRangerExpansion.Content.Gel.CPreMoodLord.PerennialGel
{
    public class PerennialGelGP : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool IsPerennialGelInfused = false;

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_ItemUse_WithAmmo ammoSource && ammoSource.AmmoItemIdUsed == ModContent.ItemType<PerennialGel>())
            {
                IsPerennialGelInfused = true;
                projectile.netUpdate = true;
                projectile.damage = (int)(projectile.damage * 0.75f); // 减少 25% 伤害
            }
            base.OnSpawn(projectile, source);
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (IsPerennialGelInfused && target.active && !target.friendly)
            {
                // 给所有玩家添加 1200 帧的 PerennialGelPBuff
                foreach (Player player in Main.player)
                {
                    if (player.active)
                    {
                        player.AddBuff(ModContent.BuffType<PerennialGelPBuff>(), 1200);
                    }
                }

                //// 修改伤害为原来的 75%
                //target.SimpleStrikeNPC((int)(damageDone * 0.75f), (int)projectile.knockBack);
            }
        }

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(projectile, target, ref modifiers);
        }

        public override void ModifyDamageHitbox(Projectile projectile, ref Rectangle hitbox)
        {
            base.ModifyDamageHitbox(projectile, ref hitbox);
        }

        public override void OnKill(Projectile projectile, int timeLeft)
        {
            base.OnKill(projectile, timeLeft);
        }
    }
}

