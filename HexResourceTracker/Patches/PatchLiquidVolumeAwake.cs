using HarmonyLib;
using HexResourceTracker.Core;

namespace HexResourceTracker.Patches
{
    [HarmonyPatch(typeof(LiquidVolume), nameof(LiquidVolume.Awake))]
    internal static class PatchLiquidVolumeAwake
    {
        private static void Postfix(LiquidVolume __instance)
        {
            if (!PluginConfig.IsModEnabled.Value)
            {
                return;
            }

            TarPitResourcePinService.TrackTarPit(__instance);
        }
    }
}
