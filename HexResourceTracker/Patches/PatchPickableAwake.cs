using HarmonyLib;
using HexResourceTracker.Core;
using HexResourceTracker.Core.Tracking;
using HexResourceTracker.Core.Tracking.Monoscripts;
using HexResourceTracker.Models;

namespace HexResourceTracker.Patches
{
    [HarmonyPatch(typeof(Pickable), nameof(Pickable.Awake))]
    internal static class PatchPickableAwake
    {
        private static void Postfix(Pickable __instance)
        {
            if (!PluginConfig.IsModEnabled.Value)
            {
                return;
            }

            if (!PickableResourcePinService.TryGetResourceDefinition(__instance, out TrackedResourceDefinitionModel definition))
            {
                return;
            }

            TrackedMapObject.TryAdd(__instance.gameObject, definition.ResourcePrefabName);

            if (PluginConfig.TrackingMode.Value == TrackingModeEnum.RangeScanner)
            {
                return;
            }

            PickableResourcePinService.TryAddResourcePinFromPickable(__instance);
        }
    }
}