using HarmonyLib;
using HexResourceTracker.Core;

namespace HexResourceTracker.Patches
{
    [HarmonyPatch(typeof(ResourceRoot), nameof(ResourceRoot.Awake))]
    internal static class PatchResourceRootAwake
    {
        private static void Postfix(ResourceRoot __instance)
        {
            if (!PluginConfig.IsModEnabled.Value || __instance == null)
            {
                return;
            }

            YggdrasilRootPinService.TrackRoot(__instance);
        }
    }
}
