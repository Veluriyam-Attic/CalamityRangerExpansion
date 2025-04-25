using CalamityMod;
using CalamityRangerExpansion.CREConfigs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using CalamityMod.Graphics.Primitives;
using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Shaders;
using CalamityMod.Buffs.StatDebuffs;

namespace CalamityRangerExpansion.Content.WeaponToAMMO.Arrow.Terratoarrow
{
    internal class TerratoarrowSPIT : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "WeaponToAMMO.Arrow.Terratoarrow";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 50;
        }
        public float SlashWidthFunction(float _) => Projectile.width * Projectile.scale * Utils.GetLerpValue(0f, 0.1f, _, true);

        public Color SlashColorFunction(float _) => Color.Turquoise;

        public override bool PreDraw(ref Color lightColor)
        {
            GameShaders.Misc["CalamityMod:ExobladePierce"].SetShaderTexture(ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/BlobbyNoise"));
            GameShaders.Misc["CalamityMod:ExobladePierce"].UseImage2("Images/Extra_189");
            GameShaders.Misc["CalamityMod:ExobladePierce"].UseColor(Terratomere.TerraColor1);
            GameShaders.Misc["CalamityMod:ExobladePierce"].UseSecondaryColor(Terratomere.TerraColor2);

            // 17MAY2024: Ozzatron: remove Terratomere rendering its trails multiple times
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, new(SlashWidthFunction, SlashColorFunction, (_) => Projectile.Size * 0.5f, shader: GameShaders.Misc["CalamityMod:ExobladePierce"]), 30);

            return false;
        }

        public override void SetDefaults()
        {
            // 设置弹幕的基础属性
            Projectile.width = 11; // 弹幕宽度
            Projectile.height = 24; // 弹幕高度
            Projectile.friendly = true; // 对敌人有效
            Projectile.DamageType = DamageClass.Ranged; // 远程伤害类型
            Projectile.penetrate = 1; // 穿透力为1，击中一个敌人就消失
            Projectile.timeLeft = 300; // 弹幕存在时间为300帧
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
            Projectile.ignoreWater = true; // 弹幕不受水影响
            Projectile.arrow = true;
            Projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.Green.ToVector3() * 0.49f);

            // 前x帧不追踪，之后开始追踪敌人
            if (Projectile.ai[1] > 21)
            {
                NPC target = Projectile.Center.ClosestNPCAt(3800); // 查找范围内最近的敌人
                if (target != null)
                {
                    Vector2 direction = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * 11f, 0.08f); // 追踪速度为11f
                }
            }
            else
            {
                Projectile.ai[1]++;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Frostburn, 240); // 原版的霜火效果

            for (int i = 0; i < 1; i++) // 生成x个弹幕
            {
                // 随机方向
                Vector2 randomDirection = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi);

                // 生成新的 TerratoarrowSlash 弹幕
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center,                // 起始位置为当前弹幕的中心
                    randomDirection * 0f,             // 初始速度为 0
                    ModContent.ProjectileType<TerratoarrowSlash>(), // 弹幕类型
                    (int)(Projectile.damage * 0.5f), // 伤害倍率为 0.33
                    Projectile.knockBack,             // 使用原弹幕的击退值
                    Projectile.owner                  // 弹幕归属
                );
            }
        }
        public override void OnKill(int timeLeft)
        {
            // 发射大量的绿色粒子效果，角度偏移为左右各 1 度，速度较快
            for (int i = 0; i < 10; i++) // 调整粒子数量
            {
                float angleOffset = MathHelper.ToRadians(Main.rand.NextFloat(-3f, 3f)); // 左右各 X 度偏移
                Vector2 particleVelocity = Projectile.velocity.RotatedBy(angleOffset) * Main.rand.NextFloat(1f, 1.35f); // 粒子速度较快
                Dust greenDust = Dust.NewDustPerfect(Projectile.Center, DustID.GreenTorch, particleVelocity, 0, Color.LimeGreen, 1.5f); // 绿色粒子
                greenDust.noGravity = true; // 无重力
                greenDust.scale = Main.rand.NextFloat(1f, 1.5f); // 调整粒子大小
            }
        }
    }
}