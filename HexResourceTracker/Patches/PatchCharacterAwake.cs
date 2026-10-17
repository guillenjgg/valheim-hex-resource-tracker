
using HarmonyLib;
using HexResourceTracker.Core.Tracking;

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

            string prefabName = __instance.gameObject.name.Replace("(Clone)", string.Empty).Trim();

            if (prefabName == "Deer_0")
            {
                prefabName = "Deer";
            }

            if (prefabName != "Deer")
            {
                return;
            }

            TrackedCreature.TryAdd(__instance, prefabName);
        }
    }
}
