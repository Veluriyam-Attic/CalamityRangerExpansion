using System;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;

namespace CalamityRangerExpansion.Content;

/// <summary>
/// 用于复刻灾厄删掉的类型
/// <br/> CalamityMod.Projectiles.CommonProjectileAI
/// </summary>
public static class CommonProjectileAI
{
    private static StickyProjAIDelegate stickyProjAIAction;
    private static ModifyHitNPCStickyDelegate modifyHitNPCStickyAction;

    //public static void StickyProjAI(this Projectile projectile, int timeLeft, bool findNewNPC = false)
    private delegate void StickyProjAIDelegate(Projectile projectile, int timeLeft, bool findNewNPC = false);
    //public static void ModifyHitNPCSticky(this Projectile projectile, int maxStick)
    private delegate void ModifyHitNPCStickyDelegate(Projectile projectile, int maxStick);
    public class CommonProjectileAISystem : ModSystem
    {
        public override void Load()
        {
            if(!ModLoader.TryGetMod("CalamityMod", out var mod)) return;
            var commprojectileType = AssemblyManager.GetLoadableTypes(mod.Code).FirstOrDefault(f => f.FullName == "CalamityMod.Projectiles.CommonProjectileAI");
            stickyProjAIAction = GetMethodToDelegate<StickyProjAIDelegate>(commprojectileType, "StickyProjAI");
            modifyHitNPCStickyAction = GetMethodToDelegate<ModifyHitNPCStickyDelegate>(commprojectileType, "ModifyHitNPCSticky");
            base.Load();
        }

        public static TDelegate GetMethodToDelegate<TDelegate>(Type type, string methodName, Type[] types = null) where TDelegate : Delegate
        {
            var BFAll = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            System.Reflection.MethodInfo methodInfo;
            if (types != null) {
                methodInfo = type.GetMethod(methodName, BFAll, types);
            } else {
                methodInfo = type.GetMethod(methodName, BFAll);
            }
            if(methodInfo == null) throw new Exception($"在类型: {type.FullName} 中未找到方法 - {methodName}");
            if(methodInfo.IsStatic == false) {
                return methodInfo.CreateDelegate<TDelegate>(type);
            } else {
                return methodInfo.CreateDelegate<TDelegate>();
            }
        }
    }

    /// <summary>
    /// Call this function in the ai of your projectile so it can stick to enemies, also requires ModifyHitNPCSticky to be called in ModifyHitNPC
    /// </summary>
    /// <param name="projectile">The projectile you're adding sticky behaviour to</param>
    /// <param name="timeLeft">Number of seconds you want a projectile to cling to an NPC</param>
    public static void StickyProjAI(this Projectile projectile, int timeLeft, bool findNewNPC = false)
    {
        stickyProjAIAction(projectile, timeLeft, findNewNPC);
    }

    /// <summary>
    /// Call this function in ModifyHitNPC to make your projectiles stick to enemies, needs StickyProjAI to be called in the AI of the projectile
    /// </summary>
    /// <param name="projectile">The projectile you're giving sticky behaviour to</param>
    /// <param name="maxStick">How many projectiles of this type can stick to one enemy</param>
    public static void ModifyHitNPCSticky(this Projectile projectile, int maxStick)
    {
        modifyHitNPCStickyAction(projectile, maxStick);
    }
}
