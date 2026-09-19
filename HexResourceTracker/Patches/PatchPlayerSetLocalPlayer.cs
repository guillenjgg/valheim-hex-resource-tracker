using HarmonyLib;
using HexResourceTracker.Core;

namespace HexResourceTracker.Patches
{
    [HarmonyPatch(typeof(Player), nameof(Player.SetLocalPlayer))]
    internal static class PatchPlayerSetLocalPlayer
    {
        private static void Postfix()
        {
            if (!PluginConfig.IsModEnabled.Value)
            {
                return;
            }

            MapTrackingScanner.HandleTrackingModeChanged();
        }
    }
}