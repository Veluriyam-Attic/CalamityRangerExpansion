namespace CalamityRangerExpansion.Content.DeveloperItems.Weapon.TenderElegy
{
    public class TenderElegyBeil : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "DeveloperItems.TenderElegy";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 9; // **设置 9 帧动画**
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int framing = texture.Height / Main.projFrames[Projectile.type];
            int y6 = framing * Projectile.frame;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Rectangle(0, y6, texture.Width, framing), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture.Width / 2f, framing / 2f), Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1; // **不会消失**
            Projectile.timeLeft = 9999; // **无限存活，直到目标死亡**
            Projectile.light = 0.5f;
            Projectile.aiStyle = 0; // **自定义 AI**
            Projectile.scale = 1.5f;
        }

        private int attackTimer = 0; // **新增计时器**
        private const int animationCycle = 60; // **完整动画循环时长**
        private const int frameDuration = animationCycle / 9; // **每帧持续时间（60 帧 / 9 帧 = 6.67，向上取整为 7）**

        public override void AI()
        {
            NPC target = Main.npc[(int)Projectile.ai[0]];

            // **如果目标死亡，则销毁自己**
            if (!target.active)
            {
                Projectile.Kill();
                return;
            }
            Projectile.timeLeft = 9999;
            // **让铃铛弹幕悬浮在目标敌人头顶**
            Vector2 destination = target.Center + new Vector2(0, -4 * 16f);
            Projectile.velocity = (destination - Projectile.Center) * 0.1f;

            // **自定义计时器，每 60 帧触发一次伤害**
            attackTimer++;
            if (attackTimer >= animationCycle) // **保证动画完整播放后再发射**
            {
                attackTimer = 0; // **重置计时器**

                Projectile.NewProjectile(
                    Projectile.GetSource_FromThis(),
                    target.Center, Vector2.Zero, // **在目标位置生成**
                    ModContent.ProjectileType<TenderElegyBeilDamage>(), // **伤害弹幕**
                    (int)(Projectile.damage * 30.0f), 0, Projectile.owner
                );
                SoundEngine.PlaySound(SoundID.MaxMana, Projectile.Center);
            }

            // **让动画每 7 帧切换一张图**
            if (attackTimer % frameDuration == 0)
            {
                Projectile.frame++;
                if (Projectile.frame >= Main.projFrames[Projectile.type])
                    Projectile.frame = 0;
            }
        }
        public override bool? CanDamage()
        {
            return false; // **永久禁用铃铛弹幕自身的伤害**
        }
        public override void OnKill(int timeLeft)
        {
            NPC target = Main.npc[(int)Projectile.ai[0]];
            if (target.active)
            {
                // **解除目标 NPC 的 `hasBell` 标记**
                target.GetGlobalNPC<TenderElegyBeilGNPC>().hasBell = false;
            }
        }
    }
}
