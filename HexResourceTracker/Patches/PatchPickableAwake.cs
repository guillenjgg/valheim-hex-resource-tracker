using HarmonyLib;
using HexResourceTracker.Core;

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

            string prefabName = __instance.gameObject.name.Replace("(Clone)", string.Empty).Trim();

            if (PluginConfig.ResourceConfigs.ContainsKey(prefabName))
            {
                TrackedMapObject.TryAdd(__instance.gameObject, prefabName);
            }

            PickableResourcePinService.TryAddResourcePinFromPickable(__instance);
        }
    }
}