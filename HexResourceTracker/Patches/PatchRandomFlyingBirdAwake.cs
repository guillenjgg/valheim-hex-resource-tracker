using HarmonyLib;
using HexResourceTracker.Core;
using HexResourceTracker.Core.Tracking.Monoscripts;

namespace HexResourceTracker.Patches
{
    // Crows, gulls and ash crows are not characters; they are found when they wake, like fish.
    [HarmonyPatch(typeof(RandomFlyingBird), nameof(RandomFlyingBird.Awake))]
    internal static class PatchRandomFlyingBirdAwake
    {
        private static void Postfix(RandomFlyingBird __instance)
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
