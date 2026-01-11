namespace CalamityRangerExpansion.Content.WeaponToAMMO.Bullet.DestructionBullet
{
    public class DestructionBulletDestruction : ModBuff, ILocalizedModType
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public new string LocalizationCategory => "Buffs";

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true; // This is a debuff
            Main.buffNoSave[Type] = true; // Does not persist on logout
        }
        // 添加一个计时器字段
        private int smokeTimer = 0;
        public override void Update(NPC npc, ref int buffIndex)
        {
            // 减少敌人30%的伤害
            npc.damage = (int)(npc.damage * 0.7f);

            // 增加计时器
            smokeTimer++;

            // 每2帧触发一次粒子效果
            if (smokeTimer >= 2)
            {
                smokeTimer = 0; // 重置计时器

                // 随机方向的红色烟雾粒子
                Vector2 dustVelocity = Vector2.One.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(1f, 2.6f);
                Particle smoke = new HeavySmokeParticle(
                    npc.Center,
                    dustVelocity,
                    Color.Red,
                    18,
                    Main.rand.NextFloat(0.9f, 1.6f), // 粒子随机缩放
                    0.35f,
                    Main.rand.NextFloat(-1f, 1f),
                    true
                );
                GeneralParticleHandler.SpawnParticle(smoke);
            }
        }
    }
}
