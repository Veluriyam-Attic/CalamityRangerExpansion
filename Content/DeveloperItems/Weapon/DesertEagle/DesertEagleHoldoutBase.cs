namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.DesertEagle
{
    internal abstract class DesertEagleHoldoutBase : ModProjectile
    {
        public abstract int AssociatedItemID { get; }

        public virtual Vector2 GunTipPosition =>
            Projectile.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * Projectile.width * 0.5f;

        public virtual float WeaponTurnSpeed => 0.2f;
        public virtual float RecoilResolveSpeed => 0.3f;
        public virtual float MaxOffsetLengthFromArm { get; }

        public virtual float OffsetXUpwards { get; }
        public virtual float OffsetXDownwards { get; }
        public virtual float BaseOffsetY { get; }
        public virtual float OffsetYUpwards { get; }
        public virtual float OffsetYDownwards { get; }

        public Player Owner { get; private set; }
        public Item HeldItem { get; private set; }

        public float OffsetLengthFromArm { get; set; }

        public Player.CompositeArmStretchAmount FrontArmStretch { get; set; } = Player.CompositeArmStretchAmount.Full;
        public Player.CompositeArmStretchAmount BackArmStretch { get; set; } = Player.CompositeArmStretchAmount.Full;

        private Asset<Texture2D> ItemTexture => TextureAssets.Item[AssociatedItemID];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = ItemTexture is null ? 1 : ItemTexture.Width();
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            ResolveOwner();
            OffsetLengthFromArm = MaxOffsetLengthFromArm;
        }

        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => false;

        public override void AI()
        {
            if (!ResolveOwner())
            {
                Projectile.Kill();
                return;
            }

            KillHoldoutLogic();
            if (!Projectile.active)
                return;

            ManageHoldout();
            HoldoutAI();
        }

        public virtual void KillHoldoutLogic()
        {
            if (Owner is null || !Owner.active || Owner.dead || Owner.HeldItem.type != AssociatedItemID)
                Projectile.Kill();
        }

        public virtual void ManageHoldout()
        {
            Vector2 armPosition = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            float holdoutDirection = Projectile.velocity.ToRotation();

            if (Projectile.owner == Main.myPlayer)
            {
                Vector2 ownerToMouse = Main.MouseWorld - armPosition;
                float proximityLookingUpwards = Vector2.Dot(ownerToMouse.SafeNormalize(Vector2.Zero), -Vector2.UnitY * Owner.gravDir);
                int direction = MathF.Sign(ownerToMouse.X);
                if (direction == 0)
                    direction = Owner.direction;

                Vector2 lengthOffset = Projectile.rotation.ToRotationVector2() * OffsetLengthFromArm;
                Vector2 armOffset = new(
                    Utils.Remap(MathF.Abs(proximityLookingUpwards), 0f, 1f, 0f, proximityLookingUpwards > 0f ? OffsetXUpwards : OffsetXDownwards) * direction,
                    BaseOffsetY * Owner.gravDir +
                    Utils.Remap(MathF.Abs(proximityLookingUpwards), 0f, 1f, 0f, proximityLookingUpwards > 0f ? OffsetYUpwards : OffsetYDownwards) * Owner.gravDir);

                Projectile.Center = armPosition + lengthOffset + armOffset;
                Projectile.velocity = holdoutDirection.AngleTowards(ownerToMouse.ToRotation(), WeaponTurnSpeed).ToRotationVector2();
                Projectile.rotation = holdoutDirection;
                Projectile.spriteDirection = direction;
                Owner.ChangeDir(direction);
            }
            else
            {
                Vector2 lengthOffset = Projectile.rotation.ToRotationVector2() * OffsetLengthFromArm;
                float proximityLookingUpwards = Vector2.Dot(Projectile.velocity.SafeNormalize(Vector2.Zero), -Vector2.UnitY * Owner.gravDir);
                int direction = Projectile.spriteDirection == 0 ? Owner.direction : Projectile.spriteDirection;

                Vector2 armOffset = new(
                    Utils.Remap(MathF.Abs(proximityLookingUpwards), 0f, 1f, 0f, proximityLookingUpwards > 0f ? OffsetXUpwards : OffsetXDownwards) * direction,
                    BaseOffsetY * Owner.gravDir +
                    Utils.Remap(MathF.Abs(proximityLookingUpwards), 0f, 1f, 0f, proximityLookingUpwards > 0f ? OffsetYUpwards : OffsetYDownwards) * Owner.gravDir);

                Projectile.Center = armPosition + lengthOffset + armOffset;
                Projectile.velocity = Projectile.rotation.ToRotationVector2();
                Owner.ChangeDir(direction);
            }

            int currentDirection = Projectile.spriteDirection;
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = Owner.itemAnimation = 2;
            Owner.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();

            float armRotation = (Projectile.rotation - MathHelper.PiOver2) * Owner.gravDir +
                (Owner.gravDir == -1 ? MathHelper.Pi : 0f);

            Owner.SetCompositeArmFront(true, FrontArmStretch, armRotation);
            Owner.SetCompositeArmBack(true, BackArmStretch, armRotation);
            Projectile.timeLeft = 2;

            if (OffsetLengthFromArm != MaxOffsetLengthFromArm)
                OffsetLengthFromArm = MathHelper.Lerp(OffsetLengthFromArm, MaxOffsetLengthFromArm, RecoilResolveSpeed);

            if (Projectile.owner == Main.myPlayer)
                Projectile.netUpdate = true;
        }

        private bool ResolveOwner()
        {
            if (Projectile.owner < 0 || Projectile.owner >= Main.maxPlayers)
                return false;

            Owner ??= Main.player[Projectile.owner];
            HeldItem = Owner.HeldItem;
            return Owner is not null;
        }

        public abstract void HoldoutAI();
    }
}
