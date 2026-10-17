using HarmonyLib;
using HexResourceTracker.Core;
using HexResourceTracker.Core.Tracking.Monoscripts;

namespace HexResourceTracker.Patches
{
    [HarmonyPatch(typeof(Character), nameof(Character.Awake))]
    internal static class PatchCharacterAwake
    {
        private static void Postfix(Character __instance)
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
