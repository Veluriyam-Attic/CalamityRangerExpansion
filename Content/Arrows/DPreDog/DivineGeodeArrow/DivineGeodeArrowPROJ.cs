namespace CalamityRangerExpansion.Content.Arrows.DPreDog.DivineGeodeArrow
{
    public class DivineGeodeArrowPROJ : ModProjectile, ILocalizedModType
    {
        //public override string Texture => "CalamityRangerExpansion/Content/Arrows/DPreDog/DivineGeodeArrow/DivineGeodeArrow";

        public new string LocalizationCategory => "Projectile.DPreDog";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 画残影效果
                CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
                return false;
            }
            return true;
        }
        public override void SetDefaults()
        {
            // 设置弹幕的基础属性
            Projectile.width = 11; // 弹幕宽度
            Projectile.height = 24; // 弹幕高度
            Projectile.friendly = true; // 对敌人有效
            Projectile.DamageType = DamageClass.Ranged; // 远程伤害类型
            Projectile.penetrate = 1; // 穿透力为1，击中一个敌人就消失
            Projectile.timeLeft = 600; // 弹幕存在时间为600帧
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
            Projectile.ignoreWater = true; // 弹幕不受水影响
            Projectile.arrow = true;
            Projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            // 调整弹幕的旋转，使其在飞行时保持水平
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;

            // Lighting - 添加土黄色/卡其色光源，光照强度为 0.55
            Lighting.AddLight(Projectile.Center, new Color(189, 183, 107).ToVector3() * 0.55f);

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                // 刚生成时释放几个卡其色的小圆圈往外扩散
                if (Projectile.ai[0] == 0f)
                {
                    #region 旧代码

                    //for (int i = 0; i < 3; i++)
                    //{
                    //    Particle pulse = new DirectionalPulseRing(Projectile.Center, Projectile.velocity * 0.75f, Color.Khaki, new Vector2(1f, 2.5f), Projectile.rotation, 0.2f, 0.03f, 20);
                    //    GeneralParticleHandler.SpawnParticle(pulse);
                    //}

                    #endregion

                    for (int i = 0; i <= 5; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.GoldFlame, Projectile.velocity);
                        dust.scale = Main.rand.NextFloat(1.6f, 2.5f);
                        dust.velocity = Projectile.velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.3f, 1.6f);
                        dust.noGravity = true;
                    }
                }

                // 为箭矢本体后面添加卡其色光束特效
                if (Projectile.numUpdates % 3 == 0)
                {
                    Color outerSparkColor = new Color(189, 183, 107); // 卡其色
                    float scaleBoost = MathHelper.Clamp(Projectile.ai[0] * 0.005f, 0f, 2f);
                    float outerSparkScale = 1.2f + scaleBoost;
                    SparkParticle spark = new SparkParticle(Projectile.Center, Projectile.velocity, false, 7, outerSparkScale, outerSparkColor);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
            }


            Projectile.ai[0]++;
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 180);

            // 遍历所有玩家，给予每个玩家 5 秒的 DivineGeodeBulletPBuff
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && !player.dead) // 确保玩家有效且未死亡
                {
                    player.AddBuff(ModContent.BuffType<DivineGeodeArrowPBuff>(), 300); // 给予 5 秒的 Buff
                }
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return true;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            // 消亡时释放额外弹幕
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    Projectile.Center, Vector2.Zero,
                    ModContent.ProjectileType<DivineGeodeArrowEXP>(),
                    (int)(Projectile.damage * 0.4f), Projectile.knockBack,
                    Projectile.owner
                );
            }

            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                #region 旧代码

                //// 在结束时释放几个卡其色的小圆圈特效
                //for (int i = 0; i < 3; i++)
                //{
                //    Particle pulse = new DirectionalPulseRing(Projectile.Center, Projectile.velocity * 0.75f, Color.Khaki, new Vector2(1f, 2.5f), Projectile.rotation, 0.2f, 0.03f, 20);
                //    GeneralParticleHandler.SpawnParticle(pulse);
                //}

                #endregion

                // 在结束时释放浅黄色的小型特效粒子
                for (int i = 0; i <= 5; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.GoldFlame, Projectile.velocity);
                    dust.scale = Main.rand.NextFloat(1.35f, 2.1f);
                    dust.velocity = Projectile.velocity.RotatedByRandom(0.06f) * Main.rand.NextFloat(0.8f, 3.1f);
                    dust.color = Color.LightYellow;
                    dust.noGravity = true;
                }

                // 消亡时释放爆炸特效
                Particle blastRing = new CustomPulse(
                    Projectile.Center, Vector2.Zero, Color.Khaki,
                    "CalamityRangerExpansion/texture/IonizingRadiation",
                    Vector2.One * 0.33f, Main.rand.NextFloat(-10f, 10f),
                    0.07f, 0.33f, 30
                );
                GeneralParticleHandler.SpawnParticle(blastRing);



            }


            // 检查是否启用了特效
            if (ModContent.GetInstance<CREsConfigs>().EnableSpecialEffects)
            {
                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = 32;
                Projectile.position.X = Projectile.position.X - Projectile.width / 2;
                Projectile.position.Y = Projectile.position.Y - Projectile.height / 2;
                for (int num621 = 0; num621 < 2; num621++)
                {
                    int num622 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 6; num623++)
                {
                    int num624 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 244, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }

                #region 旧代码

                //if (Main.netMode != NetmodeID.Server)
                //{
                //    Vector2 goreSource = Projectile.Center;
                //    int goreAmt = 3;
                //    Vector2 source = new Vector2(goreSource.X - 24f, goreSource.Y - 24f);
                //    for (int goreIndex = 0; goreIndex < goreAmt; goreIndex++)
                //    {
                //        float velocityMult = 0.33f;
                //        if (goreIndex < goreAmt / 3)
                //        {
                //            velocityMult = 0.66f;
                //        }
                //        if (goreIndex >= 2 * goreAmt / 3)
                //        {
                //            velocityMult = 1f;
                //        }
                //        //Mod mod = ModContent.GetInstance<CalamityMod>();
                //        int type = Main.rand.Next(61, 64);
                //        int smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                //        Gore gore = Main.gore[smoke];
                //        gore.velocity *= velocityMult;
                //        gore.velocity.X += 1f;
                //        gore.velocity.Y += 1f;
                //        type = Main.rand.Next(61, 64);
                //        smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                //        gore = Main.gore[smoke];
                //        gore.velocity *= velocityMult;
                //        gore.velocity.X -= 1f;
                //        gore.velocity.Y += 1f;
                //        type = Main.rand.Next(61, 64);
                //        smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                //        gore = Main.gore[smoke];
                //        gore.velocity *= velocityMult;
                //        gore.velocity.X += 1f;
                //        gore.velocity.Y -= 1f;
                //        type = Main.rand.Next(61, 64);
                //        smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                //        gore = Main.gore[smoke];
                //        gore.velocity *= velocityMult;
                //        gore.velocity.X -= 1f;
                //        gore.velocity.Y -= 1f;
                //    }
                //}

                //float x = Projectile.position.X + Main.rand.Next(-400, 400);
                //float y = Projectile.position.Y - Main.rand.Next(500, 800);
                //Vector2 vector = new Vector2(x, y);
                //float num15 = Projectile.position.X + Projectile.width / 2 - vector.X;
                //float num16 = Projectile.position.Y + Projectile.height / 2 - vector.Y;
                //num15 += Main.rand.Next(-100, 101);
                //int num17 = 25;
                //float num18 = (float)Math.Sqrt(num15 * num15 + num16 * num16);
                //num18 = num17 / num18;
                //num15 *= num18;
                //num16 *= num18;
                //if (Projectile.owner == Main.myPlayer)
                //{
                //    //int num19 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), x, y, num15, num16, ModContent.ProjectileType<SkyFlareFriendly>(), Projectile.damage / 2, 5f, Projectile.owner, 0f, 0f);
                //    //Main.projectile[num19].ai[1] = Projectile.position.Y;
                //}

                #endregion
            }
        }
    }
}