using HarmonyLib;
using HexResourceTracker.Core;

namespace HexResourceTracker.Patches
{
    [HarmonyPatch(typeof(Destructible), nameof(Destructible.Destroy))]
    internal static class PatchDestructibleDestroy
    {
        private static void Prefix(Destructible __instance)
        {
            if (!PluginConfig.IsModEnabled.Value || __instance == null)
            {
                return;
            }

            OreResourcePinService.TryRemoveResourcePinFromDestructibleOre(__instance);
        }
    }
}