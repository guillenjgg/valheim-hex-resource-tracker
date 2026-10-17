using HarmonyLib;
using HexResourceTracker.Core.Tracking.Monoscripts;

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
            CreatureTrackingUpdater.EnsureCreated();
        }
    }
}
