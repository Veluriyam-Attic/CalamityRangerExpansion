


namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.Glock17
{
    public class Glock17 : ModItem, ILocalizedModType
    {
        private static Texture2D TextureA;
        private static Texture2D TextureB;
        private static Texture2D TextureC;

        private int CoolDown = 0;

        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs([B.Weapons.Glock17.CoolDownSecond, B.Weapons.Glock17.DamageMultiplier, B.Weapons.Glock17.EffectTimeSecond, B.Weapons.Glock17.StageThirdExtraDamage]);

        private void ApplyEffect(ref Dictionary<string, int> effects, NPC npc, ref NPC.HitModifiers modifiers)
        {
            if (VeluriyamGlobalNPCMethod.SafeCanEnableEffect(ref effects, "YCRE:Glock17"))
                modifiers.FinalDamage *= 1.25f;
        }


        public override void Load()
        {
            if (!Main.dedServ)
            {
                TextureA = ModContent.Request<Texture2D>("CalamityRangerExpansion/Content/DeveloperItems/Weapon/Glock17/Glock17a").Value;
                TextureB = ModContent.Request<Texture2D>("CalamityRangerExpansion/Content/DeveloperItems/Weapon/Glock17/Glock17b").Value;
                TextureC = ModContent.Request<Texture2D>("CalamityRangerExpansion/Content/DeveloperItems/Weapon/Glock17/Glock17c").Value;
            }
            VeluriyamGlobalNPCMethod.SafeSignEffectKey(ref VeluriyamGlobalNPC.Signed, "YCRE:Glock17");
            VeluriyamGlobalNPC.ModifyIncomingHitEvent += ApplyEffect;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            Texture2D tex = GetTextureByStage();
            spriteBatch.Draw(tex, position, null, drawColor, 0f, origin, scale, SpriteEffects.None, 0f);
            CoolDown -= CoolDown <= 0 ? 0 : 1;
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
            else
            {
                Item.width = 60;
                Item.height = 24;
                Item.damage = 8;
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
            Item.shootSpeed = 13f;
            Item.useAmmo = AmmoID.Bullet;
            Item.shoot = ProjectileID.Bullet;
            Item.Calamity().donorItem = true;

        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int t = ModContent.ProjectileType<Glock17Proj>();
            if (Main.hardMode && CoolDown <= 0)
            { 
                type = t;
                CoolDown = B.Weapons.Glock17.CoolDownSecond * 60;
            }
            if(type == t)
            {
                // ✅ 计算持枪角度
                float itemRotation = player.compositeFrontArm.rotation + MathHelper.PiOver2 * player.gravDir;

                // ✅ 计算基础枪口位置（基于抬手）
                Vector2 baseGunPos = player.MountedCenter + itemRotation.ToRotationVector2() * 7f;

                // ✅ 进一步偏移：模拟枪口前伸 + 稍微偏转，确保贴图枪口位置准确
                Vector2 gunPosition = baseGunPos + velocity.RotatedBy(-0.6f * player.direction) + velocity * 1.35f;
                Projectile.NewProjectile(source, gunPosition, velocity, type, damage, knockback);
            }
            return type != t;
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
