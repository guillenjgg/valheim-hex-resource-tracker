using HarmonyLib;
using HexResourceTracker.Core;

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

            ResourcePinManager.TryAddResourcePinFromDestructibleOre(__instance);
        }
    }
}