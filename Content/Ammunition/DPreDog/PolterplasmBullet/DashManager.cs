namespace CalamityRangerExpansion.Content.Ammunition.DPreDog.PolterplasmBullet
{
    public class DashManager : ModSystem
    {
        public override void Load()
        {
            PlayerDashManager.TryAddDash(new YuDash());
        }
    }
}
