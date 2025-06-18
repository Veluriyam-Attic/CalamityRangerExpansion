using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Typeless;
using CalamityMod;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Items.Fishing.BrimstoneCragCatches;
using CalamityMod.Particles;
using CalamityRangerExpansion.LightingBolts.Metaballs;
using Terraria.GameContent;

namespace CalamityRangerExpansion.Content.DeveloperItems.Bullet.ShadowsBullet
{
    public class ShadowsBulletPROJ : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.ShadowsBullet";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj"; // 使用透明贴图

        private bool hasSplit = false; // 确保弹幕只分裂一次
        private int hitCounter = 0;
        private int frameCounter = 0;


        public static readonly int[] VanillaProjectiles = new int[]
        {
            14, 36, 89, 104, 207, 242, 279, 283, 284, 285, 286, 287, 638, 981
        };
        public static readonly int[] CalamityProjectiles = new int[]
        {
            ModContent.ProjectileType<MarksmanShot>(),
            ModContent.ProjectileType<FungiOrb>(),
            ModContent.ProjectileType<BloodClotFriendly>(),
            ModContent.ProjectileType<Aquashard>(),
            ModContent.ProjectileType<BouncingShotgunPellet>(),
            ModContent.ProjectileType<ArcherfishShot>(),

            ModContent.ProjectileType<P90Round>(),
            ModContent.ProjectileType<NitroShot>(),
            ModContent.ProjectileType<SlagRound>(),
            ModContent.ProjectileType<ClamorRifleProj>(),
            ModContent.ProjectileType<NeedlerProj>(),
            ModContent.ProjectileType<HydrasBlood>(),
            ModContent.ProjectileType<AquaBlast>(),
            ModContent.ProjectileType<ArcherfishShot>(),
            ModContent.ProjectileType<SicknessRound>(),
            ModContent.ProjectileType<PlagueTaintedProjectile>(),
            ModContent.ProjectileType<RealmRavagerBullet>(),
            ModContent.ProjectileType<Shroom>(),
            ModContent.ProjectileType<AstralRound>(),

            ModContent.ProjectileType<PlanarRipperBolt>(),
            ModContent.ProjectileType<ChargedBlast>(),
            ModContent.ProjectileType<AuralisBullet>(),
            ModContent.ProjectileType<SpykerProj>(),
            ModContent.ProjectileType<AngelicBeam>(),
            ModContent.ProjectileType<ClaretCannonProj>(),
            ModContent.ProjectileType<ArcherfishShot>(),
            ModContent.ProjectileType<CorinthPrimeAirburstGrenade>(),
            //ModContent.ProjectileType<HighExplosivePeanutShell>(),
            ModContent.ProjectileType<ShockblastRound>(),
            ModContent.ProjectileType<EmesisGore>(),
            ModContent.ProjectileType<AMRShot>(),
            ModContent.ProjectileType<KarasawaShot>(),
            ModContent.ProjectileType<FishronRPG>(),
            ModContent.ProjectileType<ImpactRound>(),
            ModContent.ProjectileType<UniversalGenesisStarcaller>(),
            ModContent.ProjectileType<UniversalGenesisStar>(),
            ModContent.ProjectileType<AuricBullet>(),
            ModContent.ProjectileType<ImpactRound>(),
            ModContent.ProjectileType<CardHeart>(),
            ModContent.ProjectileType<CardSpade>(),
            ModContent.ProjectileType<CardDiamond>(),
            ModContent.ProjectileType<CardClub>(),
            ModContent.ProjectileType<PiercingBullet>(),
            //ModContent.ProjectileType<PrismMine>(),
            ModContent.ProjectileType<PrismEnergyBullet>(),
            ModContent.ProjectileType<PrismaticEnergyBlast>(),
            ModContent.ProjectileType<PrismEnergyBullet>(),

            ModContent.ProjectileType<FlashRoundProj>(),
            ModContent.ProjectileType<MarksmanShot>(),
            ModContent.ProjectileType<HallowPointRoundProj>(),
            ModContent.ProjectileType<DryadsTearMain>(),
            ModContent.ProjectileType<HailstormBulletProj>(),
            ModContent.ProjectileType<BubonicRoundProj>(),
            ModContent.ProjectileType<HyperiusBulletProj>(),
            ModContent.ProjectileType<HolyFireBulletProj>(),
            ModContent.ProjectileType<BloodfireBulletProj>(),
            ModContent.ProjectileType<GodSlayerSlugProj>()
        };
        public static readonly string[] CustomModProjectiles = new string[]
        {
            "AerialiteBulletPROJ","TinkleshardBulletPROJ","WulfrimBulletPROJ",
            "CryonicBulletPROJ","StarblightSootBulletPROJ",
            "AstralBulletPROJ", "PerennialBulletPROJ", "PlagueBulletPROJ", "ScoriaBulletPROJ",
            "DivineGeodeBulletPROJ","EffulgentFeatherBulletPROJ","PolterplasmBulletPROJ","ToothBulletPROJ","UelibloomBulletPROJ",
            "AuricBuletPROJ","EndothermicEnergyBulletPROJ", "MiracleMatterBulletPROJ"
        };

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 6; // 无限穿透
            Projectile.timeLeft = 250;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 4; // 增加更新次数
            Projectile.arrow = true;
            Projectile.alpha = 255;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 1;
        }

        public override void AI()
        {
            // 调整弹幕的旋转，使其在飞行时保持水平
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.velocity *= 1.006f; // 逐渐加速

            // 每 5 帧生成一个 ShadowAmmoMetaball
            if (frameCounter % 3 == 0)
            {
                Vector2 spawnPosition = Projectile.Center + Main.rand.NextVector2Circular(5f, 5f);
                Vector2 velocity = Main.rand.NextVector2Circular(1f, 1f);
                float size = Main.rand.NextFloat(15f, 30f);
                ShadowAmmoMetaball.SpawnParticle(spawnPosition, velocity, size);
            }

        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Main.rand.NextBool(100)) // 1/100 概率
            {
                // 造成 75 倍伤害
                modifiers.FinalDamage *= 75;

                // 释放黑色冲击波特效
                Particle pulse = new DirectionalPulseRing(
                    Projectile.Center,
                    Vector2.Zero, // 冲击波无速度
                    Color.Black, // 使用深黑色
                    new Vector2(1.5f, 3.5f), // 扩散尺寸
                    Projectile.rotation,
                    0.3f, // 效果渐隐速度
                    0.05f,
                    30
                );
                GeneralParticleHandler.SpawnParticle(pulse);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // 计算发射位置（命中点的左前方 & 右前方 X）
            Vector2 leftPos = target.Center + Projectile.velocity.RotatedBy(MathHelper.PiOver4) * 3f;
            Vector2 rightPos = target.Center + Projectile.velocity.RotatedBy(-MathHelper.PiOver4) * 3f;

            // 获取随机弹幕类型
            int randomType;
            int containerType = Main.rand.Next(3);
            if (containerType == 0)
                randomType = VanillaProjectiles[Main.rand.Next(VanillaProjectiles.Length)];
            else if (containerType == 1)
                randomType = CalamityProjectiles[Main.rand.Next(CalamityProjectiles.Length)];
            else
                randomType = Mod.Find<ModProjectile>(CustomModProjectiles[Main.rand.Next(CustomModProjectiles.Length)]).Type;

            // 发射左侧弹幕
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), leftPos, Projectile.velocity, randomType, (int)(Projectile.damage * 0.75f), Projectile.knockBack, Projectile.owner);

            // 发射右侧弹幕
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), rightPos, Projectile.velocity, randomType, (int)(Projectile.damage * 0.75f), Projectile.knockBack, Projectile.owner);

            // 生成黑色粒子特效
            //int particleCount = Main.rand.Next(3, 6);
            //for (int i = 0; i < particleCount; i++)
            //{
            //    Vector2 direction = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(15)) * Main.rand.NextFloat(0.5f, 1.5f);
            //    Dust dust = Dust.NewDustPerfect(
            //        target.Center,
            //        DustID.ShadowbeamStaff,
            //        direction,
            //        0,
            //        Color.Black,
            //        Main.rand.NextFloat(1f, 1.5f)
            //    );
            //    dust.noGravity = true;
            //}

            // 计数器，每 5 次命中爆发大量 ShadowAmmoMetaball
            hitCounter++;
            if (hitCounter % 5 == 0)
            {
                int burstParticleCount = Main.rand.Next(15, 25);
                for (int i = 0; i < burstParticleCount; i++)
                {
                    Vector2 spawnPosition = target.Center + Main.rand.NextVector2Circular(50f, 50f);
                    Vector2 velocity = Main.rand.NextVector2Circular(3f, 3f);
                    float size = Main.rand.NextFloat(30f, 60f);
                    ShadowAmmoMetaball.SpawnParticle(spawnPosition, velocity, size);
                }

                // 额外爆发一堆黑色粒子特效
                int explosionParticleCount = Main.rand.Next(20, 30); // 让它比普通飞行粒子更多
                for (int j = 0; j < explosionParticleCount; j++)
                {
                    Vector2 particleVelocity = Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(45)) * Main.rand.NextFloat(2f, 5f); // 扩大散射范围 & 速度
                    Dust dust = Dust.NewDustPerfect(target.Center, DustID.Smoke, particleVelocity, 0, Color.Black, Main.rand.NextFloat(1.2f, 2.0f)); // 更大更亮
                    dust.noGravity = true;
                }
            }
        }








        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // 获取 SpriteBatch 和投射物纹理
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D lightTexture = TextureAssets.Projectile[Projectile.type].Value;


            // 遍历投射物的旧位置数组，绘制光学拖尾效果
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                // 计算颜色插值值，使颜色在旧位置之间平滑过渡
                float colorInterpolation = (float)Math.Cos(Projectile.timeLeft / 32f + Main.GlobalTimeWrappedHourly / 20f + i / (float)Projectile.oldPos.Length * MathHelper.Pi) * 0.5f + 0.5f;

                // 使用天蓝色渐变
                Color color = Color.Lerp(Color.Black, Color.DarkGray, colorInterpolation) * 0.4f;
                color.A = 0;

                // 计算绘制位置，将位置调整到碰撞箱的中心
                Vector2 drawPosition = Projectile.oldPos[i] + Projectile.Size * 0.5f - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

                // 计算外部和内部的颜色
                Color outerColor = color;
                Color innerColor = color * 0.5f;

                // 计算强度，使拖尾逐渐变弱
                float intensity = 0.9f + 0.15f * (float)Math.Cos(Main.GlobalTimeWrappedHourly % 60f * MathHelper.TwoPi);
                intensity *= MathHelper.Lerp(0.15f, 1f, 1f - i / (float)Projectile.oldPos.Length);
                if (Projectile.timeLeft <= 60)
                {
                    intensity *= Projectile.timeLeft / 60f; // 如果弹幕即将消失，则拖尾也逐渐消失
                }

                // 计算外部和内部的缩放比例，使拖尾具有渐变效果
                Vector2 outerScale = new Vector2(2f) * intensity;
                Vector2 innerScale = new Vector2(2f) * intensity * 0.7f;
                outerColor *= intensity;
                innerColor *= intensity;

                // 绘制外部的拖尾效果，并应用旋转
                Main.EntitySpriteDraw(lightTexture, drawPosition, null, outerColor, Projectile.rotation, lightTexture.Size() * 0.5f, outerScale * 0.6f, SpriteEffects.None, 0);

                // 绘制内部的拖尾效果，并应用旋转
                Main.EntitySpriteDraw(lightTexture, drawPosition, null, innerColor, Projectile.rotation, lightTexture.Size() * 0.5f, innerScale * 0.6f, SpriteEffects.None, 0);
            }

            // 绘制默认的弹幕，并应用旋转
            Main.EntitySpriteDraw(lightTexture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, lightColor, Projectile.rotation, lightTexture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }



    }
}