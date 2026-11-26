using HarmonyLib;
using HexResourceTracker.Core;

namespace HexResourceTracker.Patches
{
    [HarmonyPatch(typeof(MineRock5), nameof(MineRock5.AllDestroyed))]
    internal static class PatchMineRock5AllDestroyed
    {
        private static void Postfix(MineRock5 __instance, bool __result)
        {
            if (!PluginConfig.IsModEnabled.Value || __instance == null || !__result)
            {
                return;
            }

            DepositChangeLog.LogMinedOut(__instance, __instance.m_name);
        }
    }
}
