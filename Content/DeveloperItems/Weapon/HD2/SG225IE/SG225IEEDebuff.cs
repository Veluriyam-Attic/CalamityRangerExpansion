namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.HD2.SG225IE
{
    // 专属灼烧 Buff：只负责“标记存在”
    public class SG225IEEDebuff : ModBuff, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.SG225IE";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj"; // 使用完全透明贴图

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.pvpBuff[Type] = true;
        }
    }

    // 叠层与每帧掉血逻辑：放在 GlobalNPC 上，确保“实时改变”
    public class SG225IEEGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        private int _stacks;        // 叠层次数（命中次数累计）
        private int _fxCounter;     // 特效节拍计数器
        private int _lastBuffTime;  // 用于维持刷新感

        // 外部调用：命中一次就叠 1 层，并刷新持续时间
        public void ApplyStack(int refreshTime)
        {
            _stacks = Math.Min(_stacks + 1, 10); // X=2*stacks，X<=20 => stacks<=10
            _lastBuffTime = Math.Max(_lastBuffTime, refreshTime);
        }

        public override void ResetEffects(NPC npc)
        {
            if (!npc.HasBuff(ModContent.BuffType<SG225IEEDebuff>()))
            {
                _stacks = 0;
                _fxCounter = 0;
                _lastBuffTime = 0;
            }
        }

        public override void AI(NPC npc)
        {
            if (!npc.HasBuff(ModContent.BuffType<SG225IEEDebuff>()))
                return;

            _fxCounter++;

            // 维持一个“有秩序的扭曲喷射”：用确定性正弦做左右摆动，而不是纯随机
            float sway = (float)Math.Sin(_fxCounter * 0.22f) * 0.55f;
            Vector2 up = new Vector2(0f, -1f).RotatedBy(sway);

            // 火花：线性火焰 + 十字星（科技火焰混合）
            if ((_fxCounter & 1) == 0)
            {
                // 1️⃣ 原有线性火焰拉丝
                Particle spark = new SparkParticle(
                    npc.Center + new Vector2(0f, -8f) + Main.rand.NextVector2Circular(6f, 6f),
                    up * Main.rand.NextFloat(1.0f, 2.6f) + Main.rand.NextVector2Circular(0.25f, 0.25f),
                    false,
                    18,
                    Main.rand.NextFloat(0.7f, 1.1f),
                    Color.OrangeRed
                );
                GeneralParticleHandler.SpawnParticle(spark);

                // 2️⃣ 小型十字星：更“科技感”的燃烧爆点
                Vector2 direction = up.SafeNormalize(Vector2.UnitY);
                Vector2 sparkVelocity =
                    direction.RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4)) * 6f;

                CritSpark critSpark = new CritSpark(
                    npc.Center + new Vector2(0f, -10f),
                    sparkVelocity,
                    Color.White,
                    Color.LightBlue,
                    1f,
                    16
                );
                GeneralParticleHandler.SpawnParticle(critSpark);
            }
        }

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            if (!npc.HasBuff(ModContent.BuffType<SG225IEEDebuff>()))
                return;

            int x = Math.Min(2 * _stacks, 20); // 每帧掉血 X，X=2*施加次数，X<=20

            // lifeRegen 规则：每秒伤害 = -lifeRegen / 2
            // 目标：每帧 X => 每秒 60X，所以需要 lifeRegen = -120X
            npc.lifeRegen -= 120 * x;

            if (damage < x)
                damage = x;
        }
    }
}
