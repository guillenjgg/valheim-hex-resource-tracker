using HarmonyLib;
using HexResourceTracker.UI;

namespace HexResourceTracker.Patches
{
    [HarmonyPatch(typeof(Minimap), nameof(Minimap.Start))]
    internal static class PatchMinimapStart
    {
        private static void Postfix()
        {
            ResourceTrackerMapOverlay.Create();
        }
    }
}