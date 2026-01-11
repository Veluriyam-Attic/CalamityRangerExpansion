namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.Glock17
{
    public class Glock17 : ModItem, ILocalizedModType
    {
        public static Texture2D TextureA;
        public static Texture2D TextureB;
        public static Texture2D TextureC;

        public override void Load()
        {
            if (!Main.dedServ)
            {
                TextureA = ModContent.Request<Texture2D>("CalamityRangerExpansion/Content/DeveloperItems/Weapon/Glock17/Glock17a").Value;
                TextureB = ModContent.Request<Texture2D>("CalamityRangerExpansion/Content/DeveloperItems/Weapon/Glock17/Glock17b").Value;
                TextureC = ModContent.Request<Texture2D>("CalamityRangerExpansion/Content/DeveloperItems/Weapon/Glock17/Glock17c").Value;
            }
        }

        private Texture2D ChooseTextureByGameProgress()
        {
            if (NPC.downedMoonlord)
                return TextureC;
            else if (Main.hardMode)
                return TextureB;
            else
                return TextureA;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D tex = GetTextureByStage();
            spriteBatch.Draw(tex, position, null, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            Texture2D tex = GetTextureByStage();
            Vector2 position = Item.position - Main.screenPosition + new Vector2(Item.width / 2f, Item.height / 2f);
            spriteBatch.Draw(tex, position, null, lightColor, rotation, tex.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            return false;
        }
        private Texture2D GetTextureByStage()
        {
            if (NPC.downedMoonlord)
                return ModContent.Request<Texture2D>("CalamityRangerExpansion/Content/DeveloperItems/Weapon/Glock17/Glock17c").Value;
            else if (Main.hardMode)
                return ModContent.Request<Texture2D>("CalamityRangerExpansion/Content/DeveloperItems/Weapon/Glock17/Glock17b").Value;
            else
                return ModContent.Request<Texture2D>("CalamityRangerExpansion/Content/DeveloperItems/Weapon/Glock17/Glock17a").Value;
        }

        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            if (NPC.downedMoonlord)
            {
                Item.damage = 302;
                Item.width = 74;
                Item.height = 38;
            }
            else if (Main.hardMode)
            {
                Item.damage = 133;
                Item.width = 68;
                Item.height = 32;
            }
        }
        public new string LocalizationCategory => "DeveloperItems.Glock17";
        public override void SetDefaults()
        {
            Item.width = 60;
            Item.height = 24;
            Item.damage = 8;
            Item.DamageType = DamageClass.Ranged;
            Item.useTime = Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = Item.buyPrice(silver: 80);
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.Item41;
            Item.autoReuse = true;
            Item.shoot = ProjectileID.BulletHighVelocity;
            Item.shootSpeed = 13f;
            Item.useAmmo = AmmoID.Bullet;
            Item.Calamity().canFirePointBlankShots = true;
            Item.Calamity().donorItem = true;
            
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // ✅ 计算持枪角度
            float itemRotation = player.compositeFrontArm.rotation + MathHelper.PiOver2 * player.gravDir;

            // ✅ 计算基础枪口位置（基于抬手）
            Vector2 baseGunPos = player.MountedCenter + itemRotation.ToRotationVector2() * 7f;

            // ✅ 进一步偏移：模拟枪口前伸 + 稍微偏转，确保贴图枪口位置准确
            Vector2 gunPosition = baseGunPos + velocity.RotatedBy(-0.6f * player.direction) + velocity * 1.35f;

            // ✅ 计算阶段（传入弹幕）
            float stage = NPC.downedMoonlord ? 3f : Main.hardMode ? 2f : 1f;

            // ✅ 发射弹幕，传入阶段
            Projectile.NewProjectile(source, gunPosition, velocity, ModContent.ProjectileType<Glock17Proj>(), damage, knockback, player.whoAmI, stage);

            // ✅ 发射时银光粒子特效（优雅混合）
            Vector2 baseVel = velocity.SafeNormalize(Vector2.UnitX) * 2.5f;
            Vector2 origin = gunPosition;

            for (int i = 0; i < 13; i++)
            {
                Dust d = Dust.NewDustPerfect(origin, DustID.Silver, baseVel.RotatedByRandom(0.2f) * Main.rand.NextFloat(0.5f, 1.2f));
                d.scale = Main.rand.NextFloat(0.5f, 0.8f);
                d.noGravity = true;
            }

            for (int i = 0; i < 2; i++)
            {
                Color c = Color.Lerp(Color.White, Color.Silver, Main.rand.NextFloat());
                Vector2 vel = baseVel.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.4f, 1f);
                var spark = new CalamityMod.Particles.SparkParticle(origin, vel, false, 12, 0.5f, c);
                CalamityMod.Particles.GeneralParticleHandler.SpawnParticle(spark);
            }

            for (int i = 0; i < 10; i++)
            {
                Vector2 vel = baseVel.RotatedByRandom(0.4f) * Main.rand.NextFloat(0.6f, 1.5f);
                var critSpark = new CalamityMod.Particles.CritSpark(origin, vel, Color.White, Color.LightGray, 0.6f, 12);
                CalamityMod.Particles.GeneralParticleHandler.SpawnParticle(critSpark);
            }

            return false;
        }




        public override void UseStyle(Player player, Rectangle heldItemFrame)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));
            float itemRotation = player.compositeFrontArm.rotation + MathHelper.PiOver2 * player.gravDir;

            Vector2 gunPosition = player.MountedCenter + itemRotation.ToRotationVector2() * 7f;
            Vector2 gunSize = new Vector2(60, 24);
            Vector2 originOffset = new Vector2(-16, 4);

            CalamityMod.CalamityUtils.CleanHoldStyle(player, itemRotation, gunPosition, gunSize, originOffset);

            base.UseStyle(player, heldItemFrame);
        }

        public override void UseItemFrame(Player player)
        {
            player.ChangeDir(Math.Sign((player.Calamity().mouseWorld - player.Center).X));

            float animProgress = 0.5f - player.itemTime / (float)player.itemTimeMax;
            float rotation = (player.Center - player.Calamity().mouseWorld).ToRotation() * player.gravDir + MathHelper.PiOver2;

            if (animProgress < 0.4f)
                rotation += -0.03f * (float)Math.Pow((0.6f - animProgress) / 0.6f, 2) * player.direction;

            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation);
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.IronBar, 10)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}
