using HarmonyLib;

namespace HexResourceTracker.Patches
{
    [HarmonyPatch(typeof(Minimap), nameof(Minimap.UpdatePins))]
    internal static class PatchMinimapUpdatePins
    {
        private static void Postfix(Minimap __instance)
        {
            ResourcePinManager.UpdateResourcePinVisuals(__instance);
        }
    }
}