namespace CalamityRangerExpansion.Content.DonorItem.StarDustBullet
{
    public class STAROnSpawn : ModProjectile
    {
        public int p1;
        public int p2;

        public static float circleangle1 = 0;
        public static float circleangle2 = 0;

        public override string Texture => "CalamityRangerExpansion/Content/DonorItem/StarDustBullet/empty";

        public override void SetDefaults()
        {
            Projectile.timeLeft = 600;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Player player = Main.LocalPlayer;

            Vector2 direction = Main.MouseWorld - player.Center;
            int name = ModContent.ProjectileType<STARProj>();

            float r = (float)Math.Atan2(direction.Y, direction.X);

            float angle = Main.rand.NextFloat(float.Pi / 5, float.Pi / 3);

            float r2 = r + angle;
            float r3 = r - angle;

            if (circleangle1 >= (float.Pi * 2))
            {
                circleangle1 -= (float.Pi *2);
            }
            else
            {
                circleangle1 += (float.Pi / 7);
            }

            float circle1 = r + circleangle1;

            if(circleangle2 <= -(float.Pi * 2))
            {
                circleangle2 += (2 * float.Pi);
            }
            else{
                circleangle2 -= (float.Pi / 7);
            }

            float circle2 = r + circleangle2;

            p1 = Projectile.NewProjectile(source, Projectile.position,
                //r2.ToRotationVector2() * Main.rand.NextFloat(10f,20f)
                circle1.ToRotationVector2() * Main.rand.NextFloat(10f, 20f)
                , name, Projectile.damage, Projectile.knockBack);
            p2 = Projectile.NewProjectile(source, Projectile.position, 
                //r3.ToRotationVector2() * Main.rand.NextFloat(10f, 20f)
                circle2.ToRotationVector2() * Main.rand.NextFloat(10f, 20f)
                , name, Projectile.damage, Projectile.knockBack);
        }

        public override void AI()
        {
            float randomfloat = Main.rand.NextFloat(0, 1);
            Vector2 dustposition = new Vector2((Main.projectile[p1].Center.X * randomfloat) + (Main.projectile[p2].Center.X * (1 - randomfloat)), (Main.projectile[p1].Center.Y * randomfloat) + (Main.projectile[p2].Center.Y * (1 - randomfloat)));
            for (int i = 0; i < 2; i++)
            {
                //Dust.NewDust(dustposition, 3, 3,31 ,0,0,0,Color.White,0.5f);
                //Dust.QuickDustSmall(dustposition, Color.White);
                Dust.QuickDust(dustposition, Color.White);
            }


            foreach (var npc in Main.npc)
            {
                if (Collision.CheckAABBvLineCollision(npc.position, npc.Size, Main.projectile[p1].Center, Main.projectile[p2].Center))
                {
                    Main.projectile[p1].DirectionTo(npc.position);
                    Main.projectile[p1].velocity = ((npc.position - Main.projectile[p1].position) / 10);
                    Main.projectile[p2].DirectionTo(npc.position);
                    Main.projectile[p2].velocity = ((npc.position - Main.projectile[p2].position) / 10);
                    Projectile.Kill();
                }
            }
        }
    }
}
