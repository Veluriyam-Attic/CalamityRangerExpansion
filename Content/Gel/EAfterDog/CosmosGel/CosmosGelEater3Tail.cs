namespace CalamityRangerExpansion.Content.Gel.EAfterDog.CosmosGel
{
    internal class CosmosGelEater3Tail : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectile.EAfterDog";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.NeedsUUID[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.alpha = 0;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 1;
        }

        public override void AI()
        {
            //int previousSegmentID = Projectile.GetByUUID(Projectile.owner, (int)Projectile.ai[0]);
            //if (previousSegmentID < 0 || !Main.projectile[previousSegmentID].active)
            //{
            //    Projectile.Kill();
            //    return;
            //}

            //Projectile segmentAhead = Main.projectile[previousSegmentID];

            //// 计算跟随位置
            //Vector2 offsetToDestination = segmentAhead.Center - Projectile.Center;

            //// **平滑旋转**
            //if (segmentAhead.rotation != Projectile.rotation)
            //{
            //    float offsetAngle = MathHelper.WrapAngle(segmentAhead.rotation - Projectile.rotation);
            //    offsetToDestination = offsetToDestination.RotatedBy(offsetAngle * 0.08f);
            //}
            //Projectile.rotation = offsetToDestination.ToRotation() + MathHelper.PiOver2;

            //// 确保段长一致
            //if (offsetToDestination != Vector2.Zero)
            //    Projectile.Center = segmentAhead.Center - offsetToDestination.SafeNormalize(Vector2.Zero) * 16f;

            int previousSegmentID = Projectile.GetByUUID(Projectile.owner, (int)Projectile.ai[0]);
            if (previousSegmentID < 0 || !Main.projectile[previousSegmentID].active)
            {
                Projectile.Kill();
                return;
            }

            Projectile segmentAhead = Main.projectile[previousSegmentID];

            // **段间平滑跟随**
            Vector2 offsetToDestination = segmentAhead.Center - Projectile.Center;

            // **旋转平滑调整**
            if (segmentAhead.rotation != Projectile.rotation)
            {
                float offsetAngle = MathHelper.WrapAngle(segmentAhead.rotation - Projectile.rotation);
                offsetToDestination = offsetToDestination.RotatedBy(offsetAngle * 0.08f);
            }
            Projectile.rotation = offsetToDestination.ToRotation() + MathHelper.PiOver2;

            // **保持固定间距**
            float segmentSpacing = 16f;
            if (offsetToDestination != Vector2.Zero)
                Projectile.Center = segmentAhead.Center - offsetToDestination.SafeNormalize(Vector2.Zero) * segmentSpacing;


            // **粒子特效**
            //if (Main.rand.NextBool(6))
            //{
            //    Color particleColor = Main.rand.NextBool() ? Color.Purple : Color.Violet;
            //    Dust dust = Dust.NewDustPerfect(Projectile.position, DustID.FireworkFountain_Pink);
            //    dust.color = particleColor;
            //    dust.noGravity = true;
            //    dust.scale = 0.9f;
            //    dust.velocity *= 0.5f;
            //}
        }
    }
}
