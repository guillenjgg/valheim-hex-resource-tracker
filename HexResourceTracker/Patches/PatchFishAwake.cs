using HarmonyLib;
using HexResourceTracker.Core;
using HexResourceTracker.Core.Tracking.Monoscripts;

namespace HexResourceTracker.Patches
{
    [HarmonyPatch(typeof(Fish), nameof(Fish.Awake))]
    internal static class PatchFishAwake
    {
        private static void Postfix(Fish __instance)
        {
            if (!PluginConfig.IsModEnabled.Value || __instance == null)
            {
                return;
            }

            var prefabName = __instance.gameObject.name.Replace("(Clone)", string.Empty).Trim();

            if (!TrackedCreatures.TryGetDefinition(prefabName, out var definition))
            {
                return;
            }

            TrackedCreature.TryAdd(__instance, definition.PrefabName);
        }
    }
}
