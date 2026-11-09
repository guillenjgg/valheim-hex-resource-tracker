using HarmonyLib;
using HexResourceTracker.Core.PinManagers;
using System.Collections.Generic;

namespace HexResourceTracker.Patches
{
    [HarmonyPatch(typeof(Minimap), nameof(Minimap.SetMapData))]
    internal static class PatchMinimapSetMapDataLabels
    {
        private static void Postfix(Minimap __instance)
        {
            SavedPinLabelService.Reconcile(__instance);
        }
    }

    [HarmonyPatch(typeof(Minimap), nameof(Minimap.AddSharedMapData))]
    internal static class PatchMinimapSharedMapLabels
    {
        private static void Postfix(Minimap __instance)
        {
            SavedPinLabelService.Reconcile(__instance);
        }
    }

    [HarmonyPatch(typeof(Minimap), nameof(Minimap.GetMapData))]
    internal static class PatchMinimapSaveLabels
    {
        private static void Prefix(Minimap __instance, out Dictionary<Minimap.PinData, string> __state)
        {
            __state = SavedPinLabelService.PrepareForSave(__instance);
        }

        // Restore live labels even when another save patch or serialization throws.
        private static void Finalizer(Dictionary<Minimap.PinData, string> __state)
        {
            SavedPinLabelService.RestoreAfterSave(__state);
        }
    }
}
