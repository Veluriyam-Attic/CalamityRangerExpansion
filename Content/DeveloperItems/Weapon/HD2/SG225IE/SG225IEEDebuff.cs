namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.HD2.SG225IE
{
    // 专属灼烧 Buff：只负责“标记存在”
    public class SG225IEEDebuff : ModBuff, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.SG225IE";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private static Dictionary<int, int> stackMap = new();

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public static void AddStack(NPC npc)
        {
            for (int i = 0; i < npc.buffType.Length; i++)
            {
                if (npc.buffType[i] == ModContent.BuffType<SG225IEEDebuff>())
                {
                    npc.buffTime[i] += 60;
                    return;
                }
            }
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            int totalTime = npc.buffTime[buffIndex];

            int stage = totalTime / 60; // 每 60 帧算一层

            if (stage > 5)
                stage = 5;

            int frameDamage = stage;

            if (frameDamage <= 0)
                frameDamage = 1;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.lifeRegen -= 120 * frameDamage;
            }
        
     

            // =========================
            // 特效逻辑
            // =========================
            Vector2 up = -Vector2.UnitY;

            const float goldenAngle = 2.399963f;
            float theta = npc.buffTime[buffIndex] * goldenAngle;
            float radius = 9f;

            Vector2 orderedOffset = new Vector2(
                (float)Math.Cos(theta),
                (float)Math.Sin(theta)
            ) * radius;

            Vector2 spawnPos = npc.Center + orderedOffset;

            Vector2 verticalDir = Main.rand.NextBool() ? -Vector2.UnitY : Vector2.UnitY;

            Particle spark = new SparkParticle(
                spawnPos,
                verticalDir * 2f,
                false,
                10,
                1.05f,
                Color.Orange * 1.25f
            );

            GeneralParticleHandler.SpawnParticle(spark);

            CritSpark critSpark = new CritSpark(
                npc.Center + Main.rand.NextVector2Circular(10f, 12f),
                up * 4f,
                Color.OrangeRed * 1.4f,
                Color.Gold * 1.3f,
                1.1f,
                18
            );

            GeneralParticleHandler.SpawnParticle(critSpark);
        }
    }

    public class SG225IEEDOTGlobal : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public override void UpdateLifeRegen(NPC npc, ref int damage)
        {
            int buffType = ModContent.BuffType<SG225IEEDebuff>();
            int index = npc.FindBuffIndex(buffType);

            if (index < 0)
                return;

            int totalTime = npc.buffTime[index];
            int stage = totalTime / 60;

            if (stage > 5)
                stage = 5;

            if (stage <= 0)
                stage = 1;

            int dps;

            switch (stage)
            {
                case 1: dps = 30; break;
                case 2: dps = 60; break;
                case 3: dps = 90; break;
                case 4: dps = 120; break;
                default: dps = 150; break;
            }

            npc.lifeRegen -= 2 * dps;

            if (damage < dps / 60)
                damage = dps / 60;
            if (damage < stage)
                damage = stage;
        }
    }



    //// 叠层与每帧掉血逻辑：放在 GlobalNPC 上，确保“实时改变”
    //public class SG225IEEGlobalNPC : GlobalNPC
    //{
    //    public override bool InstancePerEntity => true;

    //    private int _stacks;        // 叠层次数（命中次数累计）
    //    private int _fxCounter;     // 特效节拍计数器
    //    private int _lastBuffTime;  // 用于维持刷新感

    //    // 外部调用：命中一次就叠 1 层，并刷新持续时间
    //    public void ApplyStack(int refreshTime)
    //    {
    //        _stacks = Math.Min(_stacks + 1, 10); // X=2*stacks，X<=20 => stacks<=10
    //        _lastBuffTime = Math.Max(_lastBuffTime, refreshTime);
    //    }

    //    public override void ResetEffects(NPC npc)
    //    {
    //        if (!npc.HasBuff(ModContent.BuffType<SG225IEEDebuff>()))
    //        {
    //            _stacks = 0;
    //            _fxCounter = 0;
    //            _lastBuffTime = 0;
    //        }
    //    }

    //    public override void AI(NPC npc)
    //    {
    //        if (!npc.HasBuff(ModContent.BuffType<SG225IEEDebuff>()))
    //            return;

    //        _fxCounter++;

    //        // 只在主帧执行一次密集特效
    //        if ((_fxCounter & 1) != 0)
    //            return;

    //        // ===== 方向：严格正上方 =====
    //        Vector2 up = -Vector2.UnitY;

    //        // ===== 有序随机生成位置（黄金角分布）=====
    //        const float goldenAngle = 2.399963f; // 137.5° 弧度值
    //        float radius = 6f + (_fxCounter % 5) * 1.2f;
    //        float theta = _fxCounter * goldenAngle;

    //        Vector2 orderedOffset = new Vector2(
    //            (float)Math.Cos(theta),
    //            (float)Math.Sin(theta)
    //        ) * radius;

    //        Vector2 spawnPos = npc.Center + orderedOffset;

    //        // =========================
    //        // 1️⃣ 线性火焰粒子（上下 50%）
    //        // =========================

    //        // 生成范围扩大 50%
    //        Vector2 expandedOffset = orderedOffset * 1.5f;
    //        Vector2 finalSpawnPos = npc.Center + expandedOffset;

    //        // 上 / 下 各 50%
    //        Vector2 verticalDir = Main.rand.NextBool() ? -Vector2.UnitY : Vector2.UnitY;

    //        Particle spark = new SparkParticle(
    //            finalSpawnPos,
    //            verticalDir * (1.8f + (_fxCounter % 3) * 0.3f),
    //            false,
    //            10, // 寿命改为原来的 50%
    //            1.05f,
    //            Color.Orange * 1.25f
    //        );

    //        GeneralParticleHandler.SpawnParticle(spark);

    //        // =========================
    //        // 2️⃣ CritSpark —— 更燃橙色
    //        // =========================
    //        float spinOffset = (_fxCounter % 6) * (MathHelper.Pi / 12f);

    //        Vector2 critVelocity = up.RotatedBy(spinOffset) * 4f;

    //        CritSpark critSpark = new CritSpark(
    //            npc.Center + Main.rand.NextVector2Circular(10f, 12f),
    //            critVelocity,
    //            Color.OrangeRed * 1.4f,
    //            Color.Gold * 1.3f,
    //            1.1f,
    //            18
    //        );

    //        GeneralParticleHandler.SpawnParticle(critSpark);
    //    }

    //    public override void UpdateLifeRegen(NPC npc, ref int damage)
    //    {
    //        if (!npc.HasBuff(ModContent.BuffType<SG225IEEDebuff>()))
    //            return;

    //        // ⭐ 只在服务器执行扣血
    //        //if (Main.netMode == NetmodeID.MultiplayerClient)
    //        //    return;

    //        int x = Math.Min(2 * _stacks, 20);

    //        npc.lifeRegen -= 120 * x;

    //        if (damage < x)
    //            damage = x;
    //    }
    //}



}
