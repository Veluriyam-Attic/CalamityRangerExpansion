using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using CalamityRangerExpansion.Content.BOWChange;

namespace CalamityRangerExpansion.Content.BOWChange.BPrePlantera.CorrodedCaustibowC
{
    internal class CorrodedCaustFireWall : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Weapons.BPrePlantera";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj"; // 使用完全透明贴图

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 16 * 20; // 自由设定较高的高度
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 10;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 14;
        }

        public override void AI()
        {
            if (Projectile.timeLeft % 1 == 0)
            {
                for (int i = 0; i < 24; i++)
                {
                    // ??随机生成位置：以墙体中心为圆形范围
                    Vector2 offset = Main.rand.NextVector2Circular(Projectile.width * 0.5f, Projectile.height * 0.5f);
                    Vector2 spawnPos = Projectile.Center + offset;

                    // ??生成 Dust（毒性/腐蚀感）
                    int type = Main.rand.NextBool(2) ? DustID.TerraBlade : DustID.Demonite;

                    Dust d = Dust.NewDustPerfect(
                        spawnPos,
                        type,
                        Main.rand.NextVector2Circular(2f, 2f),
                        100,
                        Color.Lerp(Color.LimeGreen, Color.Purple, Main.rand.NextFloat(0.4f, 0.9f)),
                        Main.rand.NextFloat(1.2f, 2.2f)
                    );
                    d.noGravity = true;
                    d.fadeIn = Main.rand.NextFloat(1f, 1.5f);
                }
            }

            // ??增加“粘滞光效”感
            Lighting.AddLight(Projectile.Center, new Color(80, 255, 120).ToVector3() * 0.45f);
            BowChangeVFX.SpawnCharge(Projectile, Projectile.Center, BowChangeTheme.Caustic, 1f, 0.25f);
        }



        public override bool PreDraw(ref Color lightColor)
        {
            // 完全透明不绘制本体
            return false;
        }
    }
}
