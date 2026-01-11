namespace CalamityRangerExpansion.Content.Arrows.DPreDog.PolterplasmArrow
{
    public class PolterplasmArrowINV : ModProjectile
    {
        // 使用透明贴图
        public new string LocalizationCategory => "Projectile.DPreDog";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            Projectile.width = 10; // 弹幕宽度
            Projectile.height = 10; // 弹幕高度
            Projectile.friendly = true; // 友善弹幕（不会伤害玩家或队友）
            Projectile.hostile = false; // 非敌对弹幕
            Projectile.DamageType = DamageClass.Ranged; // 弹幕类型
            Projectile.penetrate = 1; // 击中一次后消失
            Projectile.timeLeft = 50000; // 弹幕的存活时间（300 帧 = 5 秒）
            Projectile.ignoreWater = true; // 忽略水的减速
            Projectile.tileCollide = false; // 不与地形碰撞
            Projectile.extraUpdates = 1; // 提升弹幕更新频率（更平滑）
        }

        public override void AI()
        {
            // 强力追踪逻辑
            NPC target = Projectile.Center.ClosestNPCAt(8000); // 查找范围内最近的敌人
            if (target != null)
            {
                Vector2 direction = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * 22f, 0.08f); // 追踪速度为22f
            }

            // 旋转弹幕朝向飞行方向
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Time++;
        }
        public ref float Time => ref Projectile.ai[1];

        public override bool? CanDamage() => Time >= 45f; // 初始的时候不会造成伤害，直到x为止

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 击中敌人后添加减益效果
            target.AddBuff(ModContent.BuffType<PolterplasmArrowEDeBuff>(), 60); // 添加1秒的Buff
        }
    }
}
