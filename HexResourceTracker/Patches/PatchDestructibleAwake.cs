using HarmonyLib;
using HexResourceTracker.Core;
using HexResourceTracker.Core.Tracking;

namespace HexResourceTracker.Patches
{
    [HarmonyPatch(typeof(Destructible), nameof(Destructible.Awake))]
    internal static class PatchDestructibleAwake
    {
        private static void Postfix(Destructible __instance)
        {
            if (!PluginConfig.IsModEnabled.Value || __instance == null)
            {
                return;
            }

            string prefabName = __instance.gameObject.name.Replace("(Clone)", string.Empty).Trim();

            if (!TrackedResourceDefinitions.DestructibleResourcesByPrefabName.ContainsKey(prefabName))
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
}