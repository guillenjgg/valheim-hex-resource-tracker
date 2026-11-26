using HarmonyLib;
using HexResourceTracker.Core;

namespace HexResourceTracker.Patches
{
    [HarmonyPatch(typeof(Destructible), nameof(Destructible.Damage))]
    internal static class PatchDestructibleDamageLog
    {
        private static void Prefix(Destructible __instance)
        {
            DepositChangeLog.LogFirstHit(__instance, "Destructible", string.Empty);
        }
    }

    [HarmonyPatch(typeof(MineRock5), nameof(MineRock5.Damage))]
    internal static class PatchMineRock5DamageLog
    {
        private static void Prefix(MineRock5 __instance)
        {
            DepositChangeLog.LogFirstHit(__instance, "MineRock5", __instance != null ? __instance.m_name : string.Empty);
        }
    }

    [HarmonyPatch(typeof(MineRock), nameof(MineRock.Damage))]
    internal static class PatchMineRockDamageLog
    {
        private static void Prefix(MineRock __instance)
        {
            DepositChangeLog.LogFirstHit(__instance, "MineRock", __instance != null ? __instance.m_name : string.Empty);
        }
    }
}
