using HarmonyLib;
using HexResourceTracker.Core;

namespace HexResourceTracker.Patches
{
    [HarmonyPatch(typeof(Minimap), nameof(Minimap.Start))]
    internal static class PatchMinimapStart
    {
        private static void Postfix()
        {
            if (!PluginConfig.IsModEnabled.Value)
            {
                return;
            }

            ResourceTrackerMapOverlay.Create();
        }
    }
}