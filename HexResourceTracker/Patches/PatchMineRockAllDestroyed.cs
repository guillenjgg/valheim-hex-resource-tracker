using HarmonyLib;
using HexResourceTracker.Core;

namespace HexResourceTracker.Patches
{
    [HarmonyPatch(typeof(MineRock), nameof(MineRock.AllDestroyed))]
    internal static class PatchMineRockAllDestroyed
    {
        private static void Postfix(MineRock __instance, bool __result)
        {
            if (!PluginConfig.IsModEnabled.Value || __instance == null || !__result)
            {
                return;
            }

            DepositChangeLog.LogMinedOut(__instance, __instance.m_name);

            OreResourcePinService.TryRemoveResourcePinFromMineRock(__instance);
        }
    }
}