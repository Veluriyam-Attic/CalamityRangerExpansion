using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace CalamityRangerExpansion.Systems
{
    internal interface IScreenOverlayProjectile
    {
    }

    internal sealed class ScreenOverlayProjectileSystem : ModSystem
    {
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int cursorLayerIndex = layers.FindIndex(layer => layer.Name == "Vanilla: Cursor");
            if (cursorLayerIndex < 0)
                cursorLayerIndex = 0;

            layers.Insert(cursorLayerIndex, new LegacyGameInterfaceLayer(
                "CalamityRangerExpansion: Screen Overlay Projectiles",
                DrawScreenOverlayProjectiles,
                InterfaceScaleType.Game));
        }

        private static bool DrawScreenOverlayProjectiles()
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile projectile = Main.projectile[i];
                if (!projectile.active || projectile.ModProjectile is not IScreenOverlayProjectile)
                    continue;

                Color lightColor = Color.White;
                projectile.ModProjectile.PreDraw(ref lightColor);
            }

            return true;
        }
    }
}
