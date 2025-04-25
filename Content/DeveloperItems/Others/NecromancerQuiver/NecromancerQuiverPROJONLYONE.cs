//using System.Collections.Generic;
//using System.IO;
//using CalamityMod.Buffs.DamageOverTime;
//using CalamityMod.CalPlayer;
//using CalamityMod.Dusts;
//using CalamityMod.Events;
//using CalamityMod.NPCs;
//using CalamityMod.NPCs.SupremeCalamitas;
//using CalamityMod.Particles;
//using CalamityMod.World;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Graphics;
//using ReLogic.Content;
//using ReLogic.Utilities;
//using Terraria;
//using Terraria.Audio;
//using Terraria.DataStructures;
//using Terraria.Graphics.Effects;
//using Terraria.ID;
//using Terraria.ModLoader;

//namespace CalamityMod.Projectiles.Boss
//{
//    public class NecromancerQuiverPROJONLYONE : ModProjectile, ILocalizedModType
//    {
//        public new string LocalizationCategory => "Projectiles.Boss";
//        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

//        public static readonly SoundStyle SpawnSound = new("CalamityMod/Sounds/Custom/SCalSounds/BrimstoneMonsterSpawn");
//        public static readonly SoundStyle DroneSound = new("CalamityMod/Sounds/Custom/SCalSounds/BrimstoneMonsterDrone");
//        public SlotId RumbleSlot;
//        public static Asset<Texture2D> screamTex;

//        internal static readonly float CircularHitboxRadius = 170f;
//        public static int MinimumDamagePerFrame = 4;
//        public static int MaximumDamagePerFrame = 16;
//        public static float AdrenalineLossPerFrame = 0.04f;
//        public static float SpeedToForceMaxDamage = 25f;

//        private float speedAdd = 0f;
//        private float speedLimit = 0f;
//        private int time = 0;
//        private int sitStill = 90;

//        public override void SetStaticDefaults()
//        {
//            if (!Main.dedServ)
//            {
//                screamTex = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/ScreamyFace", AssetRequestMode.AsyncLoad);
//            }
//        }

//        public override void SetDefaults()
//        {
//            // 设置弹幕为我方阵营
//            Projectile.friendly = true;
//            Projectile.hostile = false;

//            Projectile.width = 320;
//            Projectile.height = 320;
//            Projectile.ignoreWater = true;
//            Projectile.tileCollide = false;
//            Projectile.penetrate = -1;
//            Projectile.timeLeft = 36000;
//            Projectile.Opacity = 0f;
//            CooldownSlot = ImmunityCooldownID.Bosses;
//        }
//        public override void SendExtraAI(BinaryWriter writer)
//        {
//            writer.Write(speedAdd);
//            writer.Write(Projectile.localAI[0]);
//            writer.Write(speedLimit);
//        }

//        public override void ReceiveExtraAI(BinaryReader reader)
//        {
//            speedAdd = reader.ReadSingle();
//            Projectile.localAI[0] = reader.ReadSingle();
//            speedLimit = reader.ReadSingle();
//        }

//        public override void AI()
//        {
//            Player player = Main.player[Projectile.owner];
//            if (!player.active || player.dead)
//            {
//                Projectile.Kill();
//                return;
//            }

//            // 公转逻辑
//            const float initialSpeed = 10f; // 初始旋转速度
//            const float finalSpeed = 1f; // 最终旋转速度
//            const float speedDecay = 0.98f; // 每帧速度衰减
//            const int orbitRadius = 350; // 公转半径
//            const float rotationAngle = 720f; // 每秒旋转的角度（度数）

//            if (Projectile.localAI[0] == 0f)
//            {
//                Projectile.localAI[0] = initialSpeed;
//            }
//            else if (Projectile.localAI[0] > finalSpeed)
//            {
//                Projectile.localAI[0] *= speedDecay;
//            }

//            // 根据旋转速度计算角度
//            double angle = Projectile.ai[0] * MathHelper.ToRadians(rotationAngle) / 60.0;
//            Vector2 offset = new Vector2(orbitRadius, 0).RotatedBy(angle);
//            Projectile.Center = player.Center + offset;

//            Projectile.ai[0] += Projectile.localAI[0]; // 更新旋转位置

//            // 检测与远程弹幕的碰撞
//            for (int i = 0; i < Main.maxProjectiles; i++)
//            {
//                Projectile otherProj = Main.projectile[i];
//                if (otherProj.active && otherProj.owner == player.whoAmI && otherProj.friendly && !otherProj.hostile)
//                {
//                    if (otherProj.DamageType == DamageClass.Ranged && otherProj.Hitbox.Intersects(Projectile.Hitbox) && otherProj.ai[0] == 0)
//                    {
//                        // 增强弹幕属性
//                        otherProj.damage = (int)(otherProj.damage * 2.0); // 增加 100% 伤害
//                        otherProj.CritChance += 150; // 增加 150% 暴击率
//                        otherProj.ai[0] = 1; // 标记为已增强
//                        SoundEngine.PlaySound(SoundID.Item84, Projectile.position); // 播放特效声音
//                    }
//                }
//            }

//            // 可选的粒子特效逻辑（保留）
//            // 在这里添加与视觉效果相关的代码
//        }

//        public override bool? CanDamage()
//        {
//            return false; // 弹幕本身不造成伤害
//        }

//        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) => CalamityUtils.CircularHitboxCollision(Projectile.Center, CircularHitboxRadius * Projectile.scale * Projectile.Opacity, targetHitbox);

     

//        public override void OnKill(int timeLeft)
//        {
//            if (SoundEngine.TryGetActiveSound(RumbleSlot, out var RumblePlaying) && RumblePlaying.IsPlaying)
//            {
//                RumblePlaying?.Stop();
//            }
//        }

//        public override bool PreDraw(ref Color lightColor)
//        {
//            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
//            lightColor.R = (byte)(255 * Projectile.Opacity);
//            Main.spriteBatch.End();
//            Effect shieldEffect = Filters.Scene["CalamityMod:HellBall"].GetShader().Shader;
//            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, shieldEffect, Main.GameViewMatrix.TransformationMatrix);

//            float noiseScale = 0.6f;

//            // Define shader parameters

//            shieldEffect.Parameters["time"].SetValue(Projectile.timeLeft / 60f * 0.24f);
//            shieldEffect.Parameters["blowUpPower"].SetValue(3.2f);
//            shieldEffect.Parameters["blowUpSize"].SetValue(0.4f);
//            shieldEffect.Parameters["noiseScale"].SetValue(noiseScale);

//            float opacity = Projectile.Opacity;
//            shieldEffect.Parameters["shieldOpacity"].SetValue(opacity);
//            shieldEffect.Parameters["shieldEdgeBlendStrenght"].SetValue(4f);

//            Color edgeColor = Color.Black * opacity;
//            Color shieldColor = Color.Lerp(Color.Red, Color.Magenta, 0.5f) * opacity;

//            // Define shader parameters for ball color
//            shieldEffect.Parameters["shieldColor"].SetValue(shieldColor.ToVector3());
//            shieldEffect.Parameters["shieldEdgeColor"].SetValue(edgeColor.ToVector3());

//            Vector2 pos = Projectile.Center - Main.screenPosition;

//            float scale = 0.715f;
//            Main.spriteBatch.Draw(screamTex.Value, pos, null, Color.White, 0, screamTex.Size() * 0.5f, scale * Projectile.scale * Projectile.Opacity, 0, 0);

//            //Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, tex.Size() / 2f, Projectile.scale * 0.3f, SpriteEffects.None, 0);
//            Main.spriteBatch.End();
//            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

//            bool isCirrus = CalamityGlobalNPC.SCal != -1 && Main.npc[CalamityGlobalNPC.SCal].active && Main.npc[CalamityGlobalNPC.SCal].ModNPC<SupremeCalamitas>().cirrus;
//            if (isCirrus)
//            {
//                Texture2D hageTex = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/BrimstoneMonsterII").Value;
//                lightColor.B = (byte)(255 * Projectile.Opacity);
//                Main.EntitySpriteDraw(hageTex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, hageTex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
//            }
//            else
//            {
//                Texture2D vortexTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/SoulVortex").Value;
//                Texture2D centerTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/LargeBloom").Value;
//                for (int i = 0; i < 10; i++)
//                {
//                    float angle = MathHelper.TwoPi * i / 3f + Main.GlobalTimeWrappedHourly * MathHelper.TwoPi;
//                    Color outerColor = Color.Lerp(Color.Red, Color.Magenta, i * 0.15f);
//                    Color drawColor = Color.Lerp(outerColor, Color.Black, i * 0.2f) * 0.5f;
//                    drawColor.A = 0;
//                    Vector2 drawPosition = Projectile.Center - Main.screenPosition;

//                    drawPosition += (angle + Main.GlobalTimeWrappedHourly * i / 16f).ToRotationVector2() * 6f;
//                    Main.EntitySpriteDraw(vortexTexture, drawPosition, null, drawColor * Projectile.Opacity, -angle + MathHelper.PiOver2, vortexTexture.Size() * 0.5f, (Projectile.scale * (1 - i * 0.05f)) * Projectile.Opacity, SpriteEffects.None, 0);
//                }
//                Main.EntitySpriteDraw(centerTexture, Projectile.Center - Main.screenPosition, null, Color.Black * Projectile.Opacity, Projectile.rotation, centerTexture.Size() * 0.5f, (Projectile.scale * 0.9f) * Projectile.Opacity, SpriteEffects.None, 0);
//            }
//            return false;
//        }

//        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
//        {
//            behindNPCs.Add(index);
//        }
//    }
//}
