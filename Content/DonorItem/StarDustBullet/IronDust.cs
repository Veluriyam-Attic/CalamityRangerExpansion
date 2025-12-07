using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityRangerExpansion.Content.DonorItem.StarDustBullet
{
    public class IronDust : ModProjectile
    {
        public Texture2D tex;

        public override string Texture => "CalamityRangerExpansion/Content/DonorItem/StarDustBullet/STATS";

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.damage = 10;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Vector2 acceleration = Projectile.velocity / 30;
            if (Projectile.ai[1] != 2f)
            {
                Projectile.velocity -= acceleration;
            }
            if(Projectile.velocity.X <= 1f && Projectile.velocity.Y <= 1f)
            {
                Projectile.velocity = Vector2.Zero;
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
            tex = Main.rand.Next([
                ModContent.Request<Texture2D>("CalamityRangerExpansion/Content/DonorItem/StarDustBullet/IronFilings-1").Value,
                ModContent.Request<Texture2D>("CalamityRangerExpansion/Content/DonorItem/StarDustBullet/IronFilings-2").Value,
                ModContent.Request<Texture2D>("CalamityRangerExpansion/Content/DonorItem/StarDustBullet/IronFilings-3").Value,
                ModContent.Request<Texture2D>("CalamityRangerExpansion/Content/DonorItem/StarDustBullet/IronFilings-4").Value,
                ModContent.Request<Texture2D>("CalamityRangerExpansion/Content/DonorItem/StarDustBullet/IronFilings-5").Value,
                ModContent.Request<Texture2D>("CalamityRangerExpansion/Content/DonorItem/StarDustBullet/IronFilings-6").Value
            ]);

            float r = Main.rand.NextFloat(0, float.Pi * 2);

        }

        public override bool PreDraw(ref Color lightColor)
        {

            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.velocity.ToRotation(), tex.Size() / 2, 1.5f, SpriteEffects.None, 0);

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.Kill();
        }
    }
}
