using HarmonyLib;
using HexResourceTracker.Core;
using HexResourceTracker.Core.Tracking;
using HexResourceTracker.Models;

namespace HexResourceTracker.Patches
{
    [HarmonyPatch(typeof(MineRock), nameof(MineRock.Start))]
    internal static class PatchMineRockStart
    {
        private static void Postfix(MineRock __instance)
        {
            if (!PluginConfig.IsModEnabled.Value || __instance == null)
            {
                return;
            }

            if (!TrackedResourceDefinitions.MineRockResourcesByName.TryGetValue(__instance.m_name, out ResourceDefinitionModel definition))
            {
                return;
            }

            TrackedMapObject.TryAdd(__instance.gameObject, definition.ResourcePrefabName);

            if (PluginConfig.TrackingMode.Value == TrackingModeEnum.RangeScanner)
            {
                return;
            }

            OreResourcePinService.TryAddResourcePinFromMineRock(__instance);
        }
    }
}