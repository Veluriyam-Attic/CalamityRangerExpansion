namespace CalamityRangerExpansion.CREConfigs
{
    public class CREsConfigs : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;


        //[Label("开启全部特效")]
        //[Tooltip("用于开关弹药的所有特效")]
        [DefaultValue(true)]
        public bool EnableSpecialEffects { get; set; }


        //[Label("开启弹药显示")]
        //[Tooltip("在左下角显示现在正在使用的弹药")]
        [DefaultValue(true)]
        public bool EnableAmmoChecking { get; set; }
    }
}