namespace CalamityRangerExpansion.Content.WeaponToAMMO.Bullet.DestructionBullet
{
    public class DestructionBulletEXP : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "WeaponToAMMO.Bullet.DestructionBullet";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            if (Main.getGoodWorld)
            {
                Projectile.width = 3500;
                Projectile.height = 3500;
            }
            else
            {
                Projectile.width = 175;
                Projectile.height = 175;
            }
            Projectile.friendly = true;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            // 所有可能的 Buff 列表
            int[] possibleBuffs = new int[]
            {
        BuffID.Daybreak, // 破晓
        BuffID.Electrified, // 带电
        BuffID.Ichor, // 灵液
        BuffID.CursedInferno, // 诅咒狱火
        BuffID.Midas, // 迈达斯
        BuffID.BetsysCurse, // 双足翼龙之怒火
        BuffID.Venom, // 酸性毒液
        ModContent.BuffType<ElementalMix>(), // 元素紊乱
        ModContent.BuffType<MarkedforDeath>(), // 死亡标记
        ModContent.BuffType<DestructionBulletPoisonIvy>(), // 常春藤毒素
        ModContent.BuffType<DestructionBulletDestruction>() // 灭世
            };

            // 检查目标身上已有的 Buff，排除已有的 Buff
            List<int> availableBuffs = new List<int>();
            foreach (int buff in possibleBuffs)
            {
                if (!target.HasBuff(buff)) // 如果目标没有该 Buff
                {
                    availableBuffs.Add(buff); // 添加到可用 Buff 列表
                }
            }

            // 如果还有可用 Buff，则随机选择一个施加
            if (availableBuffs.Count > 0)
            {
                int selectedBuff = availableBuffs[Main.rand.Next(availableBuffs.Count)];
                target.AddBuff(selectedBuff, 300); // 施加选中的 Buff，持续300帧
            }
        }

        public override void AI()
        {

        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
    }
}
