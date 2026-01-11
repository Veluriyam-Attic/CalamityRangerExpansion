namespace CalamityRangerExpansion.Content.WeaponToAMMO.Bullet.NorthStar
{
    public class PolarStarO : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "WeaponToAMMO.Bullet.NorthStar";
        //public override string Texture => "CalamityMod/Projectiles/LaserProj";

        private int dust1 = 86;
        private int dust2 = 91;

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesLocalNPCImmunity = true; // 弹幕使用本地无敌帧
            Projectile.localNPCHitCooldown = 14; // 无敌帧冷却时间为14帧
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            PPPlayer modPlayer = player.GetModPlayer<PPPlayer>();

            // 设置不同的强化阶段状态
            if (modPlayer.polarisBoostThree)
            {
                Projectile.ai[1] = 2f; // 第三强化形态
            }
            else if (modPlayer.polarisBoostTwo)
            {
                Projectile.ai[1] = 1f; // 第二强化形态
            }
            else
            {
                Projectile.ai[1] = 0f; // 未强化状态
            }

            // 处理不同形态的逻辑
            if (Projectile.ai[1] == 2f || Projectile.ai[1] == 1f) // 第二和第三强化形态的公共效果
            {
                // 添加光照效果
                Vector2 value7 = new Vector2(5f, 10f);
                Lighting.AddLight(Projectile.Center, 0.25f, 0f, 0.25f);

                // 控制局部计时器，用于生成粒子效果
                Projectile.localAI[0] += 1f;
                if (Projectile.localAI[0] >= 48f)
                {
                    Projectile.localAI[0] = 0f;
                }

                // 生成粒子效果，使投射物更加炫目
                for (int d = 0; d < 2; d++)
                {
                    int dustType = d == 0 ? 86 : 91; // 使用特定的尘埃类型
                    Vector2 value8 = -Vector2.UnitY.RotatedBy(Projectile.localAI[0] * 0.1308997f + d * MathHelper.Pi, default) * value7;
                    int num42 = Dust.NewDust(Projectile.Center, 0, 0, dustType, 0f, 0f, 160, default, 1f);
                    Main.dust[num42].scale = dustType == 86 ? 1.5f : 1f;
                    Main.dust[num42].noGravity = true;
                    Main.dust[num42].position = Projectile.Center + value8;
                    Main.dust[num42].velocity = Projectile.velocity;
                    int num458 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 0.8f);
                    Main.dust[num458].noGravity = true;
                    Main.dust[num458].velocity *= 0f;
                }
            }

            if (Projectile.ai[1] == 2f) // 第3级
            {
                // 增加计时器
                Projectile.localAI[1]++;

                if (Projectile.localAI[1] >= 40f) // 前 40 帧不追踪
                {
                    // 查找范围内最近的敌人
                    NPC target = Projectile.Center.ClosestNPCAt(1800);
                    if (target != null)
                    {
                        Vector2 direction = (target.Center - Projectile.Center).SafeNormalize(Vector2.Zero);
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, direction * 17f, 0.08f); // 追踪速度为 17f
                    }
                }
                else
                {
                    // 在前 40 帧保持直线飞行
                    Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 17f; // 固定直线飞行速度
                }
            }
            else if (Projectile.ai[1] == 1f) // 第二级
            {
                if (Projectile.alpha > 0)
                {
                    Projectile.alpha -= 25;
                }
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                }
                float num55 = 30f;
                float num56 = 1f;
                if (Projectile.ai[1] == 1f)
                {
                    Projectile.localAI[0] += num56;
                    if (Projectile.localAI[0] > num55)
                    {
                        Projectile.localAI[0] = num55;
                    }
                }
                else
                {
                    Projectile.localAI[0] -= num56;
                    if (Projectile.localAI[0] <= 0f)
                    {
                        Projectile.Kill();
                    }
                }
            }
            else // 1等级
            {
                if (Projectile.alpha > 0)
                {
                    Projectile.alpha -= 25;
                }
                if (Projectile.alpha < 0)
                {
                    Projectile.alpha = 0;
                }
                float num55 = 40f;
                float num56 = 1.5f;
                if (Projectile.ai[1] == 0f)
                {
                    Projectile.localAI[0] += num56;
                    if (Projectile.localAI[0] > num55)
                    {
                        Projectile.localAI[0] = num55;
                    }
                }
                else
                {
                    Projectile.localAI[0] -= num56;
                    if (Projectile.localAI[0] <= 0f)
                    {
                        Projectile.Kill();
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            PPPlayer modPlayer = player.GetModPlayer<PPPlayer>();

            // 每次击中敌人时，通知玩家增加计数器
            if (Projectile.owner == Main.myPlayer && !target.SpawnedFromStatue)
            {
                modPlayer.IncreaseBoostLevel();
            }
            player.AddBuff(ModContent.BuffType<PolarisBuff>(), 300);
        }


        public override Color? GetAlpha(Color lightColor) => new Color(100, 100, 255, 0);

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[1] != 2f)
            {
                return Projectile.DrawBeam(100f, 3f, lightColor);
            }
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return true;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item62 with { Volume = SoundID.Item62.Volume * 0.5f }, Projectile.position);
            if (Projectile.ai[1] == 1f) //Boost two
            {
                int projectiles = Main.rand.Next(2, 5);
                if (Projectile.owner == Main.myPlayer)
                {
                    for (int k = 0; k < projectiles; k++)
                    {
                        int split = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X, Projectile.position.Y, Main.rand.Next(-10, 11) * 2f, Main.rand.Next(-10, 11) * 2f, ModContent.ProjectileType<ChargedBlast2PS>(),
                        (int)(Projectile.damage * 1.85), (int)(Projectile.knockBack * 0.5), Main.myPlayer, 0f, 0f);
                    }
                }
            }
            if (Projectile.ai[1] == 2f) //Boost three
            {
                // 即便是第3阶段，也会有第二阶段的分裂弹幕，并且数量更多
                int projectiles = Main.rand.Next(3, 6);
                if (Projectile.owner == Main.myPlayer)
                {
                    for (int k = 0; k < projectiles; k++)
                    {
                        int split = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position.X, Projectile.position.Y, Main.rand.Next(-10, 11) * 2f, Main.rand.Next(-10, 11) * 2f, ModContent.ProjectileType<ChargedBlast2PS>(),
                        (int)(Projectile.damage * 0.5), (int)(Projectile.knockBack * 0.5), Main.myPlayer, 0f, 0f);
                    }
                }

                Projectile.ExpandHitboxBy(275);
                Projectile.maxPenetrate = -1;
                Projectile.penetrate = -1;
                Projectile.usesLocalNPCImmunity = true;
                Projectile.localNPCHitCooldown = 10;
                //Projectile.damage /= 2;
                Projectile.Damage();

                int dustAmt = 36;
                for (int d = 0; d < dustAmt; d++) // 108 dusts
                {
                    Vector2 source = Vector2.Normalize(Projectile.velocity) * new Vector2(Projectile.width / 2f, Projectile.height) * 0.3f;
                    source = source.RotatedBy((double)((d - (dustAmt / 2 - 1)) * MathHelper.TwoPi / dustAmt), default) + Projectile.Center;
                    Vector2 dustVel = source - Projectile.Center;

                    int i = Dust.NewDust(source + dustVel, 0, 0, Main.rand.NextBool(2) ? dust1 : dust2, dustVel.X * 0.3f, dustVel.Y * 0.3f, 100, default, 2f);
                    Main.dust[i].noGravity = true;

                    int j = Dust.NewDust(source + dustVel, 0, 0, Main.rand.NextBool(2) ? dust1 : dust2, dustVel.X * 0.2f, dustVel.Y * 0.2f, 100, default, 2f);
                    Main.dust[j].noGravity = true;

                    int k = Dust.NewDust(source + dustVel, 0, 0, Main.rand.NextBool(2) ? dust1 : dust2, dustVel.X * 0.1f, dustVel.Y * 0.1f, 100, default, 2f);
                    Main.dust[k].noGravity = true;
                }

                bool random = Main.rand.NextBool();
                float angleStart = Main.rand.NextFloat(0f, MathHelper.TwoPi);
                for (float angle = 0f; angle < MathHelper.TwoPi; angle += 0.05f) // 125 dusts
                {
                    random = !random;
                    Vector2 velocity = angle.ToRotationVector2() * (2f + (float)(Math.Sin(angleStart + angle * 3f) + 1) * 2.5f) * Main.rand.NextFloat(0.95f, 1.05f);
                    Dust d = Dust.NewDustPerfect(Projectile.Center, random ? dust1 : dust2, velocity);
                    d.noGravity = true;
                    d.customData = 0.025f;
                    d.scale = 2f;
                }
            }
        }
    }
}
