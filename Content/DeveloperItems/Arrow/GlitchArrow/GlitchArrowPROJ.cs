//using CalamityMod;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using Terraria;
//using Terraria.ID;
//using Terraria.ModLoader;



//namespace CalamityRangerExpansion.Content.DeveloperItems.Arrow.GlitchArrow
//{
//    internal class GlitchArrowPROJ : ModProjectile, ILocalizedModType
//    {
//        public new string LocalizationCategory => "DeveloperItems.GlitchArrow";
//        public override string Texture => "CalamityRangerExpansion/Content/DeveloperItems/Arrow/GlitchArrow/GlitchArrow";

//        private int randomEffectCooldown;
//        private static readonly List<Action<Projectile>> GlitchEffects = new(); // 故障效果列表
//        private int aiSwitchTimer = 60; // 用于控制 AI 切换的计时器
//        private int textureIndex = 1; // 存储当前的随机贴图索引
//        private Color randomColor = Color.White; // 存储随机颜色
//        private int movementTimer = 3; // 控制抖动的计时器


//        public override void SetDefaults()
//        {
//            // 基础属性设置
//            Projectile.width = 11;
//            Projectile.height = 24;
//            Projectile.friendly = true;
//            Projectile.DamageType = DamageClass.Ranged;
//            Projectile.penetrate = Main.rand.Next(1, 10); // 随机穿透次数
//            Projectile.timeLeft = Main.rand.Next(300, 3000); // 存活时间随机化
//            Projectile.usesLocalNPCImmunity = true;
//            Projectile.localNPCHitCooldown = Main.rand.Next(1, 15); // 随机无敌帧冷却
//            Projectile.ignoreWater = true;
//            Projectile.arrow = true;
//            Projectile.extraUpdates = Main.rand.Next(0, 5); // 随机更新速率
//            Projectile.scale = Main.rand.NextFloat(0.05f, 1.15f); // 随机缩放
//        }

//        public override void AI()
//        {
//            // 基础旋转逻辑
//            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + MathHelper.Pi;

//            {

//                // 最大的 AI 样式编号
//                int maxAIStyle = 194; // 确认最大编号：https://docs.tmodloader.net/docs/stable/class_proj_a_i_style_i_d.html

//                // 每 60 帧切换一次 AI
//                if (--aiSwitchTimer <= 0)
//                {
//                    aiSwitchTimer = 60; // 重置计时器
//                    int newAIStyle;

//                    // 随机生成一个与当前不同的 AI 样式
//                    do
//                    {
//                        newAIStyle = Main.rand.Next(0, maxAIStyle + 1);
//                    } while (newAIStyle == Projectile.aiStyle); // 避免切换回相同的 AI

//                    Projectile.aiStyle = newAIStyle;
//                }

//                // 每帧随机变色
//                randomColor = new Color(Main.rand.Next(256), Main.rand.Next(256), Main.rand.Next(256));

//                // 每帧切换随机贴图
//                textureIndex = Main.rand.Next(1, 1022);

//                // 每 3 帧进行一次小范围抖动
//                if (--movementTimer <= 0)
//                {
//                    movementTimer = 3; // 重置计时器
//                    Vector2 randomOffset = new Vector2(Main.rand.NextFloat(-Projectile.width * 1.5f, Projectile.width * 1.5f),
//                                                       Main.rand.NextFloat(-Projectile.height * 1.5f, Projectile.height * 1.5f));
//                    Projectile.position += randomOffset;
//                }

//                // 添加光效
//                Lighting.AddLight(Projectile.Center, randomColor.ToVector3() * 0.49f);
//            }

          

//            // 每 X~Y 帧随机触发一次故障效果
//            if (randomEffectCooldown-- <= 0)
//            {
//                randomEffectCooldown = Main.rand.Next(30, 120);
//                ActivateRandomGlitchEffect();
//            }

//            // 添加光效
//            Lighting.AddLight(Projectile.Center, Color.LightSkyBlue.ToVector3() * 0.49f);
//        }

//        public override bool PreDraw(ref Color lightColor)
//        {
//            //SpriteBatch spriteBatch = Main.spriteBatch;

//            //// 检查 textureIndex 是否有效
//            //if (textureIndex < 1 || textureIndex > 1021) // 合法范围为 1 到 1021
//            //{
//            //    textureIndex = 1; // 设置为默认值，避免错误
//            //}

//            //Texture2D texture = ModContent.Request<Texture2D>($"Terraria/Images/Projectile_{textureIndex}").Value;

//            //Vector2 drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);
//            //Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);

//            //// 使用新的随机颜色绘制
//            //spriteBatch.Draw(texture, drawPosition, null, randomColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);

//            //return false;


//            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
//            return false;
//        }
//        public override void SetStaticDefaults()
//        {
//            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 1;
//            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
//        }
//        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
//        {
//            // 随机添加 3~7 种随机 Buff
//            List<int> debuffs = GetDebuffList();
//            int numDebuffs = Main.rand.Next(3, 8);

//            ActivateRandomGlitchEffect();

//            for (int i = 0; i < numDebuffs; i++)
//            {
//                int debuff = debuffs[Main.rand.Next(debuffs.Count)];
//                int duration = Main.rand.Next(300, 3001); // 持续时间随机化
//                target.AddBuff(debuff, duration);
//            }

//            // 在敌人头顶弹出随机颜色的随机数字
//            int randomNumber = Main.rand.Next(-114514, 114515); // 随机数字范围 -114514 ~ 114514
//            Color randomColor = new Color(Main.rand.Next(256), Main.rand.Next(256), Main.rand.Next(256)); // 随机颜色
//            CombatText.NewText(target.getRect(), randomColor, randomNumber.ToString(), true);
//        }

//        public override void OnKill(int timeLeft)
//        {
//            // 随机生成粒子
//            CreateRandomParticles();

//            // 随机生成图案
//            GenerateRandomPattern();
//        }

//        private void ActivateRandomGlitchEffect()
//        {
//            // 定义故障效果
//            if (GlitchEffects.Count == 0)
//            {
//                GlitchEffects.Add(p => p.velocity *= Main.rand.NextFloat(0.5f, 2f)); // 突然调整速度（范围0.5~2）
//                GlitchEffects.Add(p => p.scale *= Main.rand.NextFloat(0.5f, 1.5f)); // 突然调整大小（范围0.5~1.5）
//                GlitchEffects.Add(p => p.position += new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101))); // 突然传送
//                GlitchEffects.Add(p => p.rotation = Main.rand.NextFloat(MathHelper.TwoPi)); // 突然旋转
//                GlitchEffects.Add(p => Projectile.NewProjectile(p.GetSource_FromThis(), p.position, p.velocity, p.type, p.damage, p.knockBack, p.owner)); // 自我复制
//                GlitchEffects.Add(p => p.velocity = Main.player[p.owner].Center - p.Center); // 突然追踪玩家
//                GlitchEffects.Add(p => p.Kill()); // 突然消失
//                //GlitchEffects.Add(p => Main.time = Main.rand.Next(0, 54000)); // 随机调整时间【太晃眼睛了，不要了】
//                GlitchEffects.Add(p => Main.player[p.owner].statLife += Main.rand.Next(1, 101)); // 回复随机血量
//                GlitchEffects.Add(p => p.velocity = -p.velocity); // 突然反向飞行
//                GlitchEffects.Add(p => p.timeLeft = Main.rand.Next(30, 300)); // 突然调整剩余寿命
//                GlitchEffects.Add(p => Projectile.NewProjectile(p.GetSource_FromThis(), p.position, Vector2.Zero, ProjectileID.Grenade, p.damage, 0, p.owner, ai0: 1)); // 生成静止的手雷
//                GlitchEffects.Add(p => Dust.NewDust(p.position, p.width, p.height, DustID.Shadowflame, Main.rand.Next(-3, 4), Main.rand.Next(-3, 4))); // 爆发出暗影火粒子
//                GlitchEffects.Add(p => Main.npc.Where(n => n.active && !n.friendly).ToList().ForEach(n =>
//                {
//                    n.StrikeNPC(new NPC.HitInfo
//                    {
//                        Damage = Main.rand.Next((int)(p.damage * 0.5), Math.Min((int)(p.damage * 5), 500)),
//                        Knockback = 0f,
//                        HitDirection = Main.rand.Next(0, 2) == 0 ? -1 : 1 // 随机方向
//                    });
//                })); // 让所有敌人随机掉血
//                GlitchEffects.Add(p => Projectile.NewProjectile(p.GetSource_FromThis(), Main.player[Main.rand.Next(Main.player.Length)].Center, new Vector2(0, -5), ProjectileID.StarWrath, 50, 1, p.owner)); // 在随机玩家头顶生成星星
//                GlitchEffects.Add(p => { p.velocity += Main.rand.NextVector2CircularEdge(5, 5); }); // 添加随机方向冲量







//            }

//            // 随机选择一个故障效果执行
//            GlitchEffects[Main.rand.Next(GlitchEffects.Count)](Projectile);
//        }

//        private void CreateRandomParticles()
//        {
//            for (int i = 0; i < 100; i++) // 随机生成粒子
//            {
//                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Main.rand.Next(DustID.Count));
//            }
//        }

//        private void GenerateRandomPattern()
//        {
//            Vector2 center = Projectile.Center;
//            int pattern = Main.rand.Next(3); // 0: 正方形, 1: 三角形, 2: 五角形

//            switch (pattern)
//            {
//                case 0:
//                    for (int i = 0; i < 4; i++)
//                    {
//                        float angle = MathHelper.PiOver2 * i;
//                        Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 50;
//                        Dust.NewDustPerfect(center + offset, DustID.Torch); // 正方形
//                    }
//                    break;
//                case 1:
//                    for (int i = 0; i < 3; i++)
//                    {
//                        float angle = MathHelper.TwoPi / 3 * i;
//                        Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 50;
//                        Dust.NewDustPerfect(center + offset, DustID.Frost); // 三角形
//                    }
//                    break;
//                case 2:
//                    for (int i = 0; i < 5; i++)
//                    {
//                        float angle = MathHelper.TwoPi / 5 * i;
//                        Vector2 offset = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 50;
//                        Dust.NewDustPerfect(center + offset, DustID.PurpleCrystalShard); // 五角形
//                    }
//                    break;
//            }
//        }

//        private List<int> GetDebuffList()
//        {
//            // 初始化 Buff 列表
//            List<int> debuffs = new List<int>
//            {
//                BuffID.OnFire, BuffID.Poisoned, BuffID.Confused, BuffID.Frostburn,
//                BuffID.Ichor, BuffID.CursedInferno, BuffID.ShadowFlame, BuffID.Daybreak,
//                BuffID.BetsysCurse, BuffID.Midas, BuffID.Wet, BuffID.Suffocation,
//                BuffID.Burning, BuffID.Bleeding, BuffID.Weak, BuffID.WitheredArmor,
//                BuffID.WitheredWeapon, BuffID.Silenced, BuffID.BrokenArmor, BuffID.Slimed,
//                BuffID.Slow, BuffID.Stoned, BuffID.Venom, BuffID.Dazed, BuffID.ObsidianSkin,
//                BuffID.OgreSpit
//            };

//            if (ModLoader.TryGetMod("CalamityMod", out var calamityMod))
//            {
//                debuffs.Add(calamityMod.Find<ModBuff>("MiracleBlight").Type);
//                debuffs.Add(calamityMod.Find<ModBuff>("VulnerabilityHex").Type);
//                debuffs.Add(calamityMod.Find<ModBuff>("MarkedforDeath").Type);
//                debuffs.Add(calamityMod.Find<ModBuff>("KamiFlu").Type);
//                debuffs.Add(calamityMod.Find<ModBuff>("GlacialState").Type);
//                debuffs.Add(calamityMod.Find<ModBuff>("HolyFlames").Type);
//                debuffs.Add(calamityMod.Find<ModBuff>("GodSlayerInferno").Type);
//                debuffs.Add(calamityMod.Find<ModBuff>("BrimstoneFlames").Type);
//                debuffs.Add(calamityMod.Find<ModBuff>("CrushDepth").Type);
//                debuffs.Add(calamityMod.Find<ModBuff>("RancorBurn").Type);
//            }

//            return debuffs;
//        }
//    }
//}
