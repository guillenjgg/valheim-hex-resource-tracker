using HarmonyLib;
using HexResourceTracker.Core;

namespace HexResourceTracker.Patches
{
    [HarmonyPatch(typeof(MineRock), nameof(MineRock.Start))]
    internal static class PatchMineRockStart
    {
        private static void Postfix(MineRock __instance)
        {
            if (!PluginConfig.IsModEnabled.Value || __instance == null)
            {
                return;
            }

            OreResourcePinService.TryAddResourcePinFromMineRock(__instance);
        }
    }
}