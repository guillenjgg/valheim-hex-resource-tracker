using HarmonyLib;
using HexResourceTracker.Core.Tracking;

namespace HexResourceTracker.Patches
{
    [HarmonyPatch(typeof(Location), nameof(Location.Awake))]
    internal static class PatchLocationAwake
    {
        private static void Postfix(Location __instance)
        {
            if (!PluginConfig.IsModEnabled.Value)
            {
                return;
            }

            if (__instance == null)
            {
                return;
            }

            TrackedDungeonLocation.TryAdd(__instance);
            DungeonPinManager.TryAddDungeonPin(__instance);
        }
    }
}