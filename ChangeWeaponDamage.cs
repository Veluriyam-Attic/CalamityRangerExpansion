namespace CalamityRangerExpansion
{
    public class ChangeWeaponDamage : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => (entity.type == ModContent.NPCType<DevourerofGodsHead>() || entity.type == ModContent.NPCType<DevourerofGodsBody>() || entity.type == ModContent.NPCType<DevourerofGodsTail>()) && lateInstantiation;

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {                
            // 检查弹幕类型是否为 闪耀金羽箭 以及它的电场
            if (projectile.type == ModContent.ProjectileType<EffulgentFeatherArrowAura>() ||
                projectile.type == ModContent.ProjectileType<EffulgentFeatherArrowPROJ>())
            {
                modifiers.SourceDamage *= 0.85f;
            }
        }

        public class ChangeDamageDOGbody : GlobalNPC
        {
            public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.type == ModContent.NPCType<DevourerofGodsBody>() && lateInstantiation;

            public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
            {
                if (projectile.type == ModContent.ProjectileType<DivineGeodeArrowPROJ>() ||
                projectile.type == ModContent.ProjectileType<DivineGeodeArrowEXP>())
                {
                    modifiers.SourceDamage *= 5f;
                }
            }
        }
    }


    public class ArterialAssaultFix : GlobalItem
    {
        // 多使用AppliesToEntity来减少开销
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.type == ModContent.ItemType<ArterialAssault>() && lateInstantiation;

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            damage /= 100;
            return true;
        }
    }
}
