using HarmonyLib;
using UnityEngine;

namespace HexResourceTracker.Patches
{
    [HarmonyPatch(typeof(Minimap), nameof(Minimap.RemovePin), typeof(Vector3), typeof(float))]
    internal static class PatchMinimapRemovePin
    {
        private static bool Prefix(Vector3 pos, float radius, ref bool __result)
        {
            if (!PluginConfig.IsModEnabled.Value)
            {
                return true;
            }

            if (ResourcePinManager.RemoveClosestResourcePin(pos, radius))
            {
                __result = true;
                return false;
            }

            return true;
        }
    }
}