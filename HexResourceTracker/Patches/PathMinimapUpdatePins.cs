using HarmonyLib;
using HexResourceTracker.Core;
using HexResourceTracker.Core.PinManagers;
using HexResourceTracker.UI;

namespace HexResourceTracker.Patches
{
    [HarmonyPatch(typeof(Minimap), nameof(Minimap.UpdatePins))]
    internal static class PatchMinimapUpdatePins
    {
        private static void Postfix(Minimap __instance)
        {
            if (!PluginConfig.IsModEnabled.Value)
            {
                return;
            }

            ResourcePinManager.UpdateResourcePinVisuals(__instance);
            ResourceTrackerMapOverlay.UpdateBiome();
        }
    }
}