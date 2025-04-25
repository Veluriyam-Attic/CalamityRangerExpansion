using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace CalamityRangerExpansion.Content.Gel.EAfterDog.CosmosGel
{
    internal class CosmosGelEater1Head : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectile.EAfterDog";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.NeedsUUID[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 600;
            Projectile.alpha = 0;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 1;
        }
        private int attackStateTimer = 0; // 计时器控制攻击状态
        public override void AI()
        {
            //Player player = Main.player[Projectile.owner];
            //Vector2 targetPos = Main.MouseWorld; // 目标是鼠标位置
            //Vector2 direction = targetPos - Projectile.Center;
            //float distance = direction.Length();

            //// **惰性追踪**：缓慢调整方向，避免突然急转
            //float maxTurnAngle = MathHelper.ToRadians(6); // 每帧最大转向角度
            //direction.Normalize();
            //Vector2 newVelocity = Vector2.Lerp(Projectile.velocity, direction * 12f, 0.08f);
            //float angleDifference = MathHelper.WrapAngle(newVelocity.ToRotation() - Projectile.velocity.ToRotation());

            //if (Math.Abs(angleDifference) > maxTurnAngle)
            //{
            //    angleDifference = Math.Sign(angleDifference) * maxTurnAngle;
            //    newVelocity = Projectile.velocity.RotatedBy(angleDifference);
            //}
            //Projectile.velocity = newVelocity;

            //// 旋转朝向飞行方向
            //Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            attackStateTimer++;

            // **找到最近的敌人（锁定范围 3000）**
            NPC target = FindClosestEnemy(3000f);

            if (target != null)
            {
                float distance = Vector2.Distance(Projectile.Center, target.Center);

                // **进入强力追踪模式（模仿 Mechworm 激光冲刺）**
                //if (attackStateTimer % 90 < 40) // 40 帧内调整方向
                {
                    float angularTurnSpeed = MathHelper.ToRadians(12f);
                    float moveSpeed = MathHelper.Lerp(Projectile.velocity.Length(), 24f, 0.3f);

                    if (distance > 1000f)
                        moveSpeed = MathHelper.Lerp(Projectile.velocity.Length(), 35f, 0.3f);

                    Projectile.velocity = Projectile.velocity.ToRotation()
                        .AngleTowards((target.Center - Projectile.Center).ToRotation(), angularTurnSpeed)
                        .ToRotationVector2() * moveSpeed;
                }
                //else // **50 帧内直接冲刺**
                //{
                //    float moveSpeed = 40f;
                //    Projectile.velocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * moveSpeed;
                //}
            }
            else
            {
                // **如果没有敌人，则使用惰性追踪鼠标**
                Player player = Main.player[Projectile.owner];
                Vector2 targetPos = Main.MouseWorld;
                Vector2 direction = targetPos - Projectile.Center;

                float moveSpeed = 15f;
                direction.Normalize();
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * moveSpeed, 0.08f);
            }

            // **维持速度**
            float moveSpeedFinal = 15f;
            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * moveSpeedFinal;

            // **旋转朝向**
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            // **生成蠕虫身体**
            if (Projectile.ai[0] == 0)
            {
                int prev = Projectile.whoAmI;
                for (int i = 0; i < 8; i++)
                {
                    prev = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                        ModContent.ProjectileType<CosmosGelEater2Body>(), Projectile.damage, Projectile.knockBack, Projectile.owner, prev);
                }

                // 生成尾巴
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<CosmosGelEater3Tail>(), Projectile.damage, Projectile.knockBack, Projectile.owner, prev);

                Projectile.ai[0] = 1;
            }





        }






        // **寻找最近的敌人**
        private NPC FindClosestEnemy(float maxRange)
        {
            NPC closestNPC = null;
            float closestDistance = maxRange;

            foreach (NPC npc in Main.npc)
            {
                if (npc.active && !npc.friendly && npc.lifeMax > 5 && !npc.dontTakeDamage)
                {
                    float distance = Vector2.Distance(Projectile.Center, npc.Center);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestNPC = npc;
                    }
                }
            }
            return closestNPC;
        }
    }
}
