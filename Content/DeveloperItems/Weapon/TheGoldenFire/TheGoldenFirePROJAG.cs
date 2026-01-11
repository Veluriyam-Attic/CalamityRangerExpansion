using FlameParticle = CalamityMod.Particles.FlameParticle;

namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.TheGoldenFire
{
    internal class TheGoldenFirePROJAG : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.TheGoldenFire";
        public override string Texture => "CalamityMod/Projectiles/FireProj";

        public static int Lifetime => 96;
        public static int Fadetime => 80;
        public ref float Time => ref Projectile.ai[0];
        public int MistType = -1;

        // 添加接收的颜色信息
        public Color FireColor { get; set; } = Color.White;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.MaxUpdates = 4;
            Projectile.timeLeft = Lifetime; // 24 effectively
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = -1;// 命中后消失
        }
        private bool isStatic = false; // 默认情况下，弹幕不是静止状态

        public override void AI()
        {
            Time++;

            if (MistType == -1)
                MistType = Main.rand.Next(3);

            if (Time > Fadetime)
                Projectile.velocity *= 0.95f;

            // 在 60 帧后停止
            if (Time == 60)
            {
                Projectile.velocity = Vector2.Zero; // 速度归零
                isStatic = true; // 进入时间暂停状态
                Projectile.timeLeft = 180; // 让它仍然存在 180 帧
                // 不要判>=，要不然每帧都会重新变为180剩余时间
            }
            


            // 在时间暂停状态，降低伤害
            if (isStatic)
                Projectile.damage = (int)(Projectile.damage * 0.5f); // 伤害降低 50%

            // 仍然执行粒子特效
            if (Time > 6f && Time < Fadetime)
            {
                if (Main.rand.NextBool(16))
                {
                    Dust dust = Dust.NewDustDirect(Projectile.Center + Main.rand.NextVector2Circular(60f, 60f) * Utils.Remap(Time, 0f, Fadetime, 0.5f, 1f), 4, 4, DustID.CorruptTorch, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100);
                    if (Main.rand.NextBool(5))
                    {
                        dust.noGravity = true;
                        dust.scale *= 2f;
                        dust.velocity *= 0.8f;
                    }
                    dust.velocity *= 1.1f;
                    dust.velocity += Projectile.velocity * Utils.Remap(Time, 0f, Fadetime * 0.75f, 1f, 0.1f) * Utils.Remap(Time, 0f, Fadetime * 0.1f, 0.1f, 1f);
                }

                if (Main.rand.NextBool(17))
                {
                    bool LowVel = Main.rand.NextBool() ? false : true;
                    FlameParticle fire = new FlameParticle(Projectile.Center, 20, MathHelper.Clamp(Time * 0.05f, 0.15f, 1.75f), 0.05f, Color.Gold * (LowVel ? 1.2f : 0.5f), Color.DarkGoldenrod * (LowVel ? 1.2f : 0.5f));
                    fire.Velocity = new Vector2(Projectile.velocity.X * 0.8f, -10).RotatedByRandom(0.005f) * (LowVel ? Main.rand.NextFloat(0.4f, 0.65f) : Main.rand.NextFloat(0.8f, 1f));
                    GeneralParticleHandler.SpawnParticle(fire);
                }
            }
            else if (Time == 5f)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool(3) ? 295 : 181, Projectile.velocity.RotatedByRandom(MathHelper.ToRadians(30f)) * Main.rand.NextFloat(0.5f, 1f));
                dust.scale = Main.rand.NextFloat(0.8f, 1.8f);
                dust.noGravity = true;
                dust.fadeIn = 0.5f;
            }
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D fire = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Texture2D mist = ModContent.Request<Texture2D>("CalamityMod/Particles/MediumMist").Value;

            float length = ((Time > Fadetime - 10f) ? 0.1f : 0.15f);
            float vOffset = Math.Min(Time, 20f);
            float timeRatio = Utils.GetLerpValue(0f, Lifetime, Time);
            float fireSize = Utils.Remap(timeRatio, 0.2f, 0.5f, 0.25f, 1f);

            // 只有在弹幕完全过期时才隐藏
            if (timeRatio >= 1f && !isStatic)
                return false;

            for (float j = 1f; j >= 0f; j -= length)
            {
                // 强制颜色为 AliceBlue
                Color fireColor = Color.AliceBlue * (1f - j) * Utils.GetLerpValue(0f, 0.2f, timeRatio, true);

                Vector2 firePos = Projectile.Center - Main.screenPosition - Projectile.velocity * vOffset * j;
                float mainRot = (-j * MathHelper.PiOver2 - Main.GlobalTimeWrappedHourly * (j + 1f) * 2f / length) * Math.Sign(Projectile.velocity.X);
                float trailRot = MathHelper.PiOver4 - mainRot;

                // 在时间暂停状态下，确保主火焰不会消失
                if (isStatic)
                {
                    fireColor *= 0.85f; // 保持可见但稍微降低透明度
                    timeRatio = 0f; // 防止火焰透明度受 `timeRatio` 影响
                }

                // 绘制拖尾（不受时间暂停影响）
                Vector2 trailOffset = Projectile.velocity * vOffset * length * 0.5f;
                Main.EntitySpriteDraw(fire, firePos - trailOffset, null, fireColor * 0.25f, trailRot, fire.Size() * 0.5f, fireSize, SpriteEffects.None);

                // 绘制主火焰（时间暂停时仍然可见）
                Main.EntitySpriteDraw(fire, firePos, null, fireColor, mainRot, fire.Size() * 0.5f, fireSize, SpriteEffects.None);
            }

            // 绘制雾气
            if (MistType > 2 || MistType < 0)
                return false;
            Main.spriteBatch.SetBlendState(BlendState.Additive);
            Rectangle frame = mist.Frame(1, 3, 0, MistType);
            Main.EntitySpriteDraw(mist, Projectile.Center - Main.screenPosition, frame, Color.Lerp(Color.AliceBlue, Color.White, 0.3f), 0, frame.Size() * 0.5f, fireSize, SpriteEffects.None);
            Main.spriteBatch.SetBlendState(BlendState.AlphaBlend);

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.Kill();
        }
    }
}
