namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.BlindBirdCry
{
    public class BlindBirdCryINVPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.BlindBirdCry";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj"; // 使用透明贴图

        private NPC attachedNPC; // 绑定的目标敌人
        private int attackTimer = 0; // 用于计时召唤弹幕
        private const int MaxFlightTime = 120; // 飞行寻找目标的最大时间（2秒）

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true; // 友方投射物
            Projectile.ignoreWater = true; // 不受水影响
            Projectile.tileCollide = false; // 不与地形碰撞
            Projectile.penetrate = -1; // 无限穿透
            Projectile.timeLeft = MaxFlightTime; // 初始飞行时间
            Projectile.DamageType = DamageClass.Magic; // 魔法伤害
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Projectile.timeLeft = 300; // 延长存活时间

            if (attachedNPC == null || !attachedNPC.active)
            {
                SearchForTarget(); // 搜索最近的敌人
                FlyTowardsTarget(); // 朝向目标飞行
            }
            else
            {
                StickToTarget(); // 黏在敌人身上
                SummonProjectiles(); // 从四面召唤弹幕
            }
        }

        private void SearchForTarget()
        {
            NPC closestNPC = null;
            float minDistance = 1600f; // 搜索范围
            foreach (NPC npc in Main.npc)
            {
                if (npc.CanBeChasedBy(this) && !npc.friendly)
                {
                    float distance = Vector2.Distance(npc.Center, Projectile.Center);
                    if (distance < minDistance)
                    {
                        closestNPC = npc;
                        minDistance = distance;
                    }
                }
            }

            if (closestNPC != null)
            {
                attachedNPC = closestNPC; // 绑定目标
                Projectile.timeLeft = 300; // 延长存活时间
            }
        }

        private void FlyTowardsTarget()
        {
            if (attachedNPC != null)
            {
                Vector2 direction = (attachedNPC.Center - Projectile.Center).SafeNormalize(Vector2.UnitX);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * 10f, 0.1f); // 平滑飞向目标
            }
        }

        private void StickToTarget()
        {
            if (attachedNPC != null && attachedNPC.active)
            {
                Projectile.Center = attachedNPC.Center; // 固定在敌人身上
                Projectile.velocity = Vector2.Zero; // 停止移动
            }
            else
            {
                attachedNPC = null; // 目标失效，重新寻找
                Projectile.timeLeft = MaxFlightTime;
            }
        }

        private void SummonProjectiles()
        {
            attackTimer++;
            if (attackTimer >= 20) // 每30帧召唤一次弹幕
            {
                attackTimer = 0;

                // 从四个随机方向召唤弹幕
                for (int i = 0; i < 4; i++)
                {
                    Vector2 spawnOffset = Main.rand.NextVector2Circular(3700f, 3700f); // 随机方向的初始偏移
                    Vector2 direction = (Projectile.Center - (Projectile.Center + spawnOffset)).SafeNormalize(Vector2.UnitY); // 朝向投射物中心
                    Projectile.NewProjectile(
                        Projectile.GetSource_FromThis(),
                        Projectile.Center + spawnOffset,
                        direction * 40f, // 速度
                        ModContent.ProjectileType<BlindBirdCryCrods>(), // 投射物类型
                        Projectile.damage,
                        Projectile.knockBack,
                        Projectile.owner
                    );
                }
            }
        }
    }
}
