namespace CalamityRangerExpansion.Content.DonorItem.StarDustBullet
{
    public class STARProj : ModProjectile
    {
        public override string Texture => "CalamityRangerExpansion/Content/DonorItem/StarDustBullet/STARS";

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.damage = 10;
            Projectile.friendly = true;
            Projectile.width = 28;
            Projectile.height = 10;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.timeLeft = 600;
            Projectile.penetrate = 1;
        }

        public static bool IsSouth = true;

        public override void OnSpawn(IEntitySource source)
        {
            if (!IsSouth)
            {
                Projectile.ai[0] = 1f;
            }
            else
            {
                Projectile.ai[0] = 2f;
            }

            Projectile.ai[1] = 1f;

            IsSouth = !IsSouth;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D STARNorth = ModContent.Request<Texture2D>("CalamityRangerExpansion/Content/DonorItem/StarDustBullet/STARN", AssetRequestMode.ImmediateLoad).Value;
            if (Projectile.ai[0] == 1f && Projectile.ai[0] != 2f)
            {
                Main.spriteBatch.Draw(STARNorth, Projectile.Center - Main.screenPosition,null,Color.White,Projectile.velocity.ToRotation(), STARNorth.Size()/2,1,SpriteEffects.None,0);
            }
            return Projectile.ai[0] != 1f;
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {

            foreach (Projectile iron in Main.projectile)
            {
                if (iron.type == ModContent.ProjectileType<IronDust>() && iron.velocity == Vector2.Zero)
                {
                    iron.velocity = (target.position - iron.position) / 10;
                }
            }

            StarBoom(target);

            Projectile.Kill();
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Vector2 acceleration = Projectile.velocity / 30;
            if (Projectile.ai[1] != 2f)
            {
                Projectile.velocity -= acceleration;
            }
        }

        public void StarBoom(NPC npc)
        {
            for (int i = 0; i < 3; i++)
            {
                float randomvalue = Main.rand.NextFloat(0, 2 * float.Pi);
                float r = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
                r += randomvalue;
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.position, r.ToRotationVector2() * 10f, ModContent.ProjectileType<IronDust>(), Projectile.damage, Projectile.knockBack);
            }
        }
    }
}
