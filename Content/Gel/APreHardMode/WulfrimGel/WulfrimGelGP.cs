namespace CalamityRangerExpansion.Content.Gel.APreHardMode.WulfrimGel
{
    internal class WulfrimGelGP : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        public bool IsWulfrimGelInfused = false;
        private Dictionary<int, int> cooldownTimers = new Dictionary<int, int>(); // 存储每个 NPC 的冷却计时

        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_ItemUse_WithAmmo ammoSource && ammoSource.AmmoItemIdUsed == ModContent.ItemType<WulfrimGel>())
            {
                IsWulfrimGelInfused = true;
                projectile.netUpdate = true;
            }
            base.OnSpawn(projectile, source);
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (IsWulfrimGelInfused && target.active && !target.friendly)
            {
                int npcId = target.whoAmI;

                // 检查冷却计时器是否存在或冷却是否完成
                if (!cooldownTimers.ContainsKey(npcId) || cooldownTimers[npcId] <= 0)
                {
                    // 造成额外的固定 5 点伤害
                    var damageInfo = target.CalculateHitInfo(5, hit.HitDirection, false, 0);
                    // 参数解释：
                    // 1. `5` - 额外伤害值
                    // 2. `hit.HitDirection` - 击退方向，与原弹幕保持一致
                    // 3. `false` - 不暴击
                    // 4. `0` - 击退强度为 0（不额外击退）

                    target.StrikeNPC(damageInfo);

                    // 在目标上方弹出橙色的 5 点伤害数值
                    CombatText.NewText(target.getRect(), Color.Orange, 5, true);

                    // 开启冷却计时（60 帧 = 1 秒）
                    cooldownTimers[npcId] = 60;
                }
            }

            base.OnHitNPC(projectile, target, hit, damageDone);
        }

        public override void AI(Projectile projectile)
        {
            // 更新冷却计时器
            foreach (var key in cooldownTimers.Keys.ToList())
            {
                if (cooldownTimers[key] > 0)
                {
                    cooldownTimers[key]--;
                }
            }

            base.AI(projectile);
        }
    }
}

