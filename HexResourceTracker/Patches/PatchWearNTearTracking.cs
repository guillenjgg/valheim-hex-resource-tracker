using HarmonyLib;
using HexResourceTracker.Core;
using HexResourceTracker.Core.Tracking;
using HexResourceTracker.Core.Tracking.Monoscripts;
using HexResourceTracker.Models;

namespace HexResourceTracker.Patches
{
    [HarmonyPatch(typeof(WearNTear), nameof(WearNTear.Awake))]
    internal static class PatchWearNTearAwake
    {
        private static void Postfix(WearNTear __instance)
        {
            if (!PluginConfig.IsModEnabled.Value)
            {
                return;
            }

            if (__instance == null)
            {
                return;
            }

            string prefabName = __instance.gameObject.name.Replace("(Clone)", string.Empty).Trim();

            if (!TrackedResources.TryGetByPrefabName(prefabName, out TrackedResourceDefinitionModel definition))
            {
                return;
            }

            if (definition.ResourceType != TrackedResourceTypeEnum.Deposit)
            {
                return;
            }

            TrackedMapObject.TryAdd(__instance.gameObject, prefabName);

            if (PluginConfig.TrackingMode.Value == TrackingModeEnum.RangeScanner)
            {
                return;
            }

            OreResourcePinService.TryAddResourcePinFromDestructibleOre(__instance);
        }
    }

    [HarmonyPatch(typeof(WearNTear), nameof(WearNTear.Destroy))]
    internal static class PatchWearNTearDestroy
    {
        private static void Prefix(WearNTear __instance)
        {
            if (!PluginConfig.IsModEnabled.Value)
            {
                return;
            }

            if (__instance == null)
            {
                return;
            }

            OreResourcePinService.TryRemoveResourcePinFromDestructibleOre(__instance);
        }
    }
}
