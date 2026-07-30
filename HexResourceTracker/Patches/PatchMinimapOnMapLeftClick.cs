using HarmonyLib;

namespace HexResourceTracker.Patches
{
    [HarmonyPatch(typeof(Minimap), nameof(Minimap.OnMapLeftClick))]
    internal static class PatchMinimapOnMapLeftClick
    {
        [HarmonyPostfix]
        private static void Postfix()
        {
            DungeonPinManager.UpdateCheckedStates();
        }
    }
}