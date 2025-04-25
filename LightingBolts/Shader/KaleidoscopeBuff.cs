using Terraria;
using Terraria.ModLoader;

namespace CalamityRangerExpansion.LightingBolts.Shader
{
    public class KaleidoscopeBuff : ModBuff
    {
        //public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = false;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            // ✅ 正确触发 ModSystem 里的 Shader
            ModContent.GetInstance<KaleidoscopeSystem>().EnableEffect(true);
        }
    }
}
