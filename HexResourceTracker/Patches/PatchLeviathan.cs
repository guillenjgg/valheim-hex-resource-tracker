using HarmonyLib;
using HexResourceTracker.Core;
using HexResourceTracker.Core.Tracking;
using HexResourceTracker.Core.Tracking.Monoscripts;

namespace HexResourceTracker.Patches
{
    [HarmonyPatch(typeof(Leviathan), nameof(Leviathan.Awake))]
    internal static class PatchLeviathanAwake
    {
        private static void Postfix(Leviathan __instance)
        {
            if (!PluginConfig.IsModEnabled.Value || __instance == null)
            {
                return;
            }

            string prefabName = __instance.gameObject.name.Replace("(Clone)", string.Empty).Trim();

            if (!TrackedResources.TryGetByPrefabName(prefabName, out _))
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

    [HarmonyPatch(typeof(Leviathan), nameof(Leviathan.RPC_Left))]
    internal static class PatchLeviathanLeft
    {
        private static void Postfix(Leviathan __instance)
        {
            if (!PluginConfig.IsModEnabled.Value || __instance == null)
            {
                return;
            }

            OreResourcePinService.TryRemoveResourcePinFromDestructibleOre(__instance);
        }
    }
}
