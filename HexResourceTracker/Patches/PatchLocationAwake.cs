using HarmonyLib;
using HexResourceTracker.Core.Tracking;

namespace HexResourceTracker.Patches
{
    [HarmonyPatch(typeof(Location), nameof(Location.Awake))]
    internal static class PatchLocationAwake
    {
        private static void Postfix(Location __instance)
        {
            if (!PluginConfig.IsModEnabled.Value || __instance == null)
            {
                return;
            }

            TrackedDungeonLocation.TryAdd(__instance);

            if (PluginConfig.TrackingMode.Value == TrackingModeEnum.RangeScanner)
            {
                return;
            }

            DungeonPinManager.TryAddDungeonPin(__instance);
        }
    }
}