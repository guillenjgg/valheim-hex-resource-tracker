using HarmonyLib;

namespace HexResourceTracker.Patches
{
    [HarmonyPatch(typeof(Pickable), nameof(Pickable.Awake))]
    internal static class PatchPickableAwake
    {
        private static void Postfix(Pickable __instance)
        {
            if (!PluginConfig.IsModEnabled.Value)
            {
                return;
            }

            ResourcePinManager.TryAddResourcePinFromPickable(__instance);
        }
    }
}