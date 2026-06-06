using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Typeless;
using CalamityMod;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.EAfterDog.UltimaC
{
    internal class UltimaWingMAN : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.EAfterDog";
        public override bool PreDraw(ref Color lightColor)
        {
            // 获取纹理资源和位置
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = texture.Size() * 0.5f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            // 背光效果部分 - 亮白色光晕
            float chargeOffset = 3f; // 控制充能效果扩散的偏移量
            Color chargeColor = Color.White * 0.6f; // 设置为亮白色
            chargeColor.A = 0; // 设置透明度

            // 修复旋转逻辑，确保与速度方向同步
            float rotation = (Main.MouseWorld - Projectile.Center).ToRotation();
            //float rotation = Projectile.Center.ToRotation();
            //SpriteEffects direction = SpriteEffects.None;
            //if (Projectile.spriteDirection == -1)
            //{
            //    direction = SpriteEffects.FlipHorizontally;
            //}

            // 设置贴图方向
            SpriteEffects direction = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;


            // 绘制充能效果 - 圆周上绘制多个充能光效
            for (int i = 0; i < 8; i++)
            {
                Vector2 drawOffset = (MathHelper.TwoPi * i / 8f).ToRotationVector2() * chargeOffset;
                Main.spriteBatch.Draw(texture, drawPosition + drawOffset, null, chargeColor, rotation, origin, Projectile.scale, direction, 0f);
            }

            // 渲染实际的投射物本体
            Main.spriteBatch.Draw(texture, drawPosition, null, Projectile.GetAlpha(lightColor), rotation, origin, Projectile.scale, direction, 0f);

            return false;            
        }

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.friendly = false;
            Projectile.timeLeft = 3600;
        }
        private int astralStarCooldown = 15; // AstralStar 发射频率
        private int plasmaBlastCooldown = 60; // PlasmaBlast 发射频率
        private int astralStarTimer = 0;
        private int plasmaBlastTimer = 0;

        public override void AI()
        {
            //Projectile.rotation = (Main.MouseWorld - Projectile.Center).ToRotation() + MathHelper.PiOver4;

            //Vector2 offset = new Vector2(100 * Projectile.ai[0], 0).RotatedBy((Main.MouseWorld - player.Center).ToRotation());
            //Projectile.Center = player.Center + offset;


            Player player = Main.player[Projectile.owner];
            Vector2 weaponToMouse = Main.MouseWorld - player.Center;
            Vector2 perpendicularOffset = weaponToMouse.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2) * (100 * Projectile.ai[0]);
            Projectile.Center = player.Center + perpendicularOffset;
            Projectile.rotation = (Main.MouseWorld - Projectile.Center).ToRotation();



            //Player player = Main.player[Projectile.owner];

            //// 在固定半径上顺时针旋转
            //float rotationSpeed = MathHelper.ToRadians(4); // 每帧旋转角度（4像素）
            //Projectile.ai[1] += rotationSpeed; // 更新旋转角度
            //float radius = 100f; // 半径
            //Vector2 center = player.Center; // 以玩家为圆心
            //Projectile.Center = center + new Vector2((float)Math.Cos(Projectile.ai[1]), (float)Math.Sin(Projectile.ai[1])) * radius;


            {
                //// 计算目标位置（武器中心点的一侧）
                //Vector2 weaponToMouse = Main.MouseWorld - player.Center;
                //Vector2 perpendicularOffset = weaponToMouse.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2) * (100 * Projectile.ai[0]);
                //Vector2 targetPosition = player.Center + perpendicularOffset;

                //// 限制移动速度
                //Vector2 movement = targetPosition - Projectile.Center;
                //float maxSpeed = 5f; // 每帧最大移动速度
                //if (movement.Length() > maxSpeed)
                //{
                //    movement.Normalize();
                //    movement *= maxSpeed;
                //}
                //Projectile.Center += movement;

                //// 僚机始终指向鼠标
                //Projectile.rotation = (Main.MouseWorld - Projectile.Center).ToRotation();
            }

            // AstralStar 发射逻辑
            astralStarTimer++;
            if (astralStarTimer >= astralStarCooldown)
            {
                FireProjectile(ModContent.ProjectileType<AstralStar>(), 35f);
                astralStarTimer = 0;
            }

            // PlasmaBlast 发射逻辑（仅在右键按下时触发）
            var modPlayer = player.GetModPlayer<UltimaRePlayer>();
            if (modPlayer.IsRightClicking)
            {
                plasmaBlastTimer++;
                if (plasmaBlastTimer >= plasmaBlastCooldown)
                {
                    FireProjectile(ModContent.ProjectileType<StarfleetStar>(), 75f);
                    plasmaBlastTimer = 0;
                }
                SoundEngine.PlaySound(SoundID.Item89, Projectile.Center);
            }
        }

        private void FireProjectile(int projType, float speedMultiplier)
        {
            Player player = Main.player[Projectile.owner];
            Vector2 direction = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitX);
            Vector2 velocity = direction * speedMultiplier;

            // 发射弹幕
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, projType, Projectile.damage, Projectile.knockBack, player.whoAmI);
            BowChangeVFX.SpawnMuzzle(Projectile, Projectile.Center, velocity, BowChangeTheme.Ultima, speedMultiplier > 40f ? 0.85f : 0.55f);

            // 自身往相反方向移动
            Vector2 recoilOffset = -direction * 7f;
            //Projectile.position += recoilOffset;

            // 使用弹幕特效的逻辑模拟快速返回
            for (int i = 0; i < 5; i++)
            {
                Vector2 returnVelocity = recoilOffset * (1f - i * 0.2f);
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Electric, returnVelocity, 100, default, 0.7f + 0.06f * i);
                dust.noGravity = true;
            }

            // 生成散射粒子特效
            GenerateElectricParticles(direction);
        }

        private void GenerateElectricParticles(Vector2 direction)
        {
            for (int i = -1; i <= 1; i++) // 左、中、右三个方向
            {
                Vector2 particleDirection = direction.RotatedBy(MathHelper.ToRadians(15 * i)); // 每个粒子有不同的偏移角度
                for (int j = 0; j < 5; j++) // 每条链包含 5 个粒子
                {
                    float speed = 2f + j; // 速度递增
                    float scale = 0.75f + j * 0.05f; // 大小递增
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Electric, particleDirection * speed, 100, default, scale);
                    dust.noGravity = true; // 禁用重力效果
                }
            }
        }
    }
}
