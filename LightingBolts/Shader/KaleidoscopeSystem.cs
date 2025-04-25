using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using System.Collections.Generic;
using Terraria.UI;

namespace CalamityRangerExpansion.LightingBolts.Shader
{
    public class KaleidoscopeSystem : ModSystem
    {
        private static Effect kaleidoscopeEffect;
        private RenderTarget2D screenTarget;
        public bool IsEffectEnabled { get; private set; } = false; // Buff 控制是否启用


        public override void Load()
        {
            if (!Main.dedServ)
            {
                // ✅ 修正 Shader 路径
                kaleidoscopeEffect = ModContent.Request<Effect>("CalamityRangerExpansion/LightingBolts/Shader/XNBcoder/Effects/KaleidoscopeScreenShader", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            }
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int index = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
            if (index != -1)
            {
                layers.Insert(index, new LegacyGameInterfaceLayer(
                    "CalamityRangerExpansion: KaleidoscopeEffect",
                    delegate
                    {
                        if (IsEffectEnabled)
                        {
                            ApplyKaleidoscopeEffect();
                        }
                        return true;
                    },
                    InterfaceScaleType.UI)
                );
            }
        }

        public void EnableEffect(bool enable)
        {
            IsEffectEnabled = enable;
        }

        private void ApplyKaleidoscopeEffect()
        {
            GraphicsDevice graphics = Main.graphics.GraphicsDevice;
            SpriteBatch spriteBatch = Main.spriteBatch;

            // **初始化 RenderTarget**
            if (screenTarget == null || screenTarget.Width != Main.screenWidth || screenTarget.Height != Main.screenHeight)
            {
                screenTarget?.Dispose();
                screenTarget = new RenderTarget2D(graphics, Main.screenWidth, Main.screenHeight);
            }

            // **捕获游戏世界（不包括 UI）**
            graphics.SetRenderTarget(screenTarget);
            graphics.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White); // 只捕获游戏画面
            spriteBatch.End();

            graphics.SetRenderTarget(null); // ✅ 释放 RenderTarget，防止 UI 被影响

            // **设置 Shader 变量**
            kaleidoscopeEffect.Parameters["uCenter"].SetValue((Main.LocalPlayer.Center - Main.screenPosition) / new Vector2(Main.screenWidth, Main.screenHeight));
            kaleidoscopeEffect.Parameters["uSegments"].SetValue(6f);
            kaleidoscopeEffect.Parameters["uRadius"].SetValue(70f * 16f);
            kaleidoscopeEffect.Parameters["uOpacity"].SetValue(1.0f); // ✅ 只影响游戏世界

            // **绘制万花筒效果**
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.Default, RasterizerState.CullNone, kaleidoscopeEffect, Main.GameViewMatrix.TransformationMatrix);
            spriteBatch.Draw(screenTarget, Vector2.Zero, Color.White);
            spriteBatch.End();

            // ✅ **不恢复 SpriteBatch**，让 UI 继续正常绘制
        }


    }
}
