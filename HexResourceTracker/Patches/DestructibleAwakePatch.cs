using HarmonyLib;

namespace HexResourceTracker.Patches
{
    [HarmonyPatch(typeof(Destructible), nameof(Destructible.Awake))]
    internal static class DestructibleAwakePatch
    {
        private const string CopperPrefabName = "rock4_copper";
        private const string SilverPrefabName = "silvervein";
        private const string GiantSkull = "giant_skull";

        private static void Postfix(Destructible __instance)
        {
            if (!PluginConfig.IsModEnabled.Value || __instance == null)
            {
                return;
            }

            string prefabName = __instance.gameObject.name.Replace("(Clone)", string.Empty).Trim();

            if (prefabName == CopperPrefabName)
            {
                ResourcePinManager.TryAddResourcePinFromDestructibleOre(
                    __instance,
                    CopperPrefabName,
                    "CopperOre");

                return;
            }

            if (prefabName == SilverPrefabName)
            {
                ResourcePinManager.TryAddResourcePinFromDestructibleOre(
                    __instance,
                    SilverPrefabName,
                    "SilverOre");

                return;
            }

            if (prefabName == GiantSkull)
            {
                ResourcePinManager.TryAddResourcePinFromDestructibleOre(
                    __instance,
                    GiantSkull,
                    "Softtissue");

                return;
            }
        }
    }
}