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

            var prefabName = __instance.gameObject.name.Replace("(Clone)", string.Empty).Trim();

            switch (prefabName)
            {
                case "Deer_0":
                    prefabName = "Deer";
                    break;
                case "Boar_0":
                    prefabName = "Boar";
                    break;
                case "Hare_0":
                    prefabName = "Hare";
                    break;
            }

            if (prefabName != "Deer" && prefabName != "Boar" && prefabName != "Hare")
            {
                return;
            }

            TrackedCreature.TryAdd(__instance, prefabName);
        }
    }
}
