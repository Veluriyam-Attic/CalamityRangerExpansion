namespace CalamityRangerExpansion.Content.WeaponToAMMO.Arrow.TheDrill
{
    public class TheDrillPlayer : ModPlayer
    {
        public override void ProcessTriggers(Terraria.GameInput.TriggersSet triggersSet)
        {
            // 检查鼠标右键点击，清除所有 TheDrillPROJ
            if (Main.mouseRight)
            {
                ClearAllTheDrillProj();
            }

            // 如果玩家携带 NoBuilding debuff，直接摧毁所有 TheDrillPROJ
            if (Player.HasBuff(BuffID.NoBuilding))
            {
                ClearAllTheDrillProj();
            }


            // 如果场上有任何 Boss 存活，直接摧毁所有 TheDrillPROJ
            if (Main.npc.Any(npc => npc.boss && npc.active))
            {
                ClearAllTheDrillProj();
            }
        }

        private void ClearAllTheDrillProj()
        {
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (proj.active && proj.type == ModContent.ProjectileType<TheDrillPROJ>())
                {
                    proj.Kill(); // 清除 TheDrillPROJ
                }
            }
        }

    }
}
