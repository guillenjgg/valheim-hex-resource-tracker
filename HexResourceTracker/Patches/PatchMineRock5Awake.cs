using HarmonyLib;
using HexResourceTracker.Core;
using HexResourceTracker.Models;

namespace HexResourceTracker.Patches
{
    [HarmonyPatch(typeof(MineRock5), nameof(MineRock5.Awake))]
    internal static class PatchMineRock5Awake
    {
        private static void Postfix(MineRock5 __instance)
        {
            if (!PluginConfig.IsModEnabled.Value || __instance == null)
            {
                return;
            }

            OreResourcePinService.TryAddOrRelinkResourcePinFromMineRock5Ore(__instance);
        }
    }
}