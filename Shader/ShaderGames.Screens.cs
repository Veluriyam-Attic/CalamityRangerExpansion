using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace CalamityRangerExpansion.Shader
{
    public sealed partial class ShaderGames
    {
        private const string DarkPlasmaKamuiVortexRegistrationName = "DarkPlasmaKamuiVortex";
        private const string DarksunFragmentGravitationalLensingRegistrationName = "DarksunFragmentGravitationalLensing";

        public static Effect BlackHoleDistortionShader => GetEffect("BlackHoleDistortion");
        public static Effect ScreenSimplyDistortedShader => GetEffect("ScreenSimplyDistorted");
        public static Effect DarkPlasmaKamuiVortexShader => GetEffect("DarkPlasmaKamuiVortex");
        public static Effect DarksunFragmentGravitationalLensingShader => GetEffect("DarksunFragmentGravitationalLensing");

        private static readonly ShaderDefinition[] ScreenShaders =
        [
            // Screen shaders are activated through Filters.Scene.
            new("BlackHoleDistortion", ShaderCategory.Screen, "Pass1", "BlackHoleDistortion"),
            new("DarkPlasmaKamuiVortex", ShaderCategory.Screen, "Pass1", DarkPlasmaKamuiVortexRegistrationName),
            new("DarksunFragmentGravitationalLensing", ShaderCategory.Screen, "Pass1", DarksunFragmentGravitationalLensingRegistrationName),
            new("ScreenSimplyDistorted", ShaderCategory.Screen, "Pass1", "ScreenSimplyDistorted")
        ];

        private static void RegisterScreenShaders()
        {
            foreach (ShaderDefinition shader in ScreenShaders)
                RegisterSceneFilter(shader.Name, shader.PassName, shader.RegistrationName, EffectPriority.Medium);
        }

        private static void UpdateScreenShaderParameters()
        {
            foreach (ShaderDefinition shader in ScreenShaders)
            {
                string key = SceneFilterKey(shader.RegistrationName);
                Filter filter = Filters.Scene[key];
                if (filter is null || !filter.IsActive())
                    continue;

                Effect effect = filter.GetShader().Shader;
                switch (shader.Name)
                {
                    case "BlackHoleDistortion":
                        effect.Parameters["uCenter"]?.SetValue(new Vector2(0.5f, 0.5f));
                        effect.Parameters["uRadius"]?.SetValue(0.42f);
                        effect.Parameters["uStrength"]?.SetValue(0.16f + 0.04f * MathF.Sin(Main.GlobalTimeWrappedHourly * 2.4f));
                        effect.Parameters["uTime"]?.SetValue(Main.GlobalTimeWrappedHourly);
                        break;

                    case "DarkPlasmaKamuiVortex":
                        ConfigureCenteredScreenShader(filter, effect);
                        effect.Parameters["uRadius"]?.SetValue(Math.Min(Main.screenWidth, Main.screenHeight) * 0.28f);
                        effect.Parameters["uStrength"]?.SetValue(0.48f + 0.08f * MathF.Sin(Main.GlobalTimeWrappedHourly * 2.2f));
                        effect.Parameters["uOpacity"]?.SetValue(0.85f);
                        effect.Parameters["uTime"]?.SetValue(Main.GlobalTimeWrappedHourly);
                        break;

                    case "DarksunFragmentGravitationalLensing":
                        ConfigureCenteredScreenShader(filter, effect);
                        float radius = Math.Min(Main.screenWidth, Main.screenHeight) * 0.34f;
                        effect.Parameters["uRadius"]?.SetValue(radius);
                        effect.Parameters["uHorizonRadius"]?.SetValue(radius * 0.28f);
                        effect.Parameters["uStrength"]?.SetValue(0.56f);
                        break;

                    case "ScreenSimplyDistorted":
                        effect.Parameters["uScreenResolution"]?.SetValue(GetScreenSize());
                        break;
                }
            }
        }

        private static void ConfigureCenteredScreenShader(Filter filter, Effect effect)
        {
            Vector2 screenSize = GetScreenSize();
            Vector2 targetWorldPosition = Main.screenPosition + screenSize * 0.5f;

            filter.GetShader().UseTargetPosition(targetWorldPosition);
            effect.Parameters["uScreenResolution"]?.SetValue(screenSize);
            effect.Parameters["uScreenPosition"]?.SetValue(Main.screenPosition);
            effect.Parameters["uTargetPosition"]?.SetValue(targetWorldPosition);
        }

        private static Vector2 GetScreenSize()
        {
            return new Vector2(Math.Max(Main.screenWidth, 1), Math.Max(Main.screenHeight, 1));
        }
    }
}
