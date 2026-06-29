using HarmonyLib;

namespace HexResourceTracker.Patches
{
    [HarmonyPatch(typeof(MineRock5), nameof(MineRock5.Awake))]
    internal static class MineRock5AwakePatch
    {
        private const string CopperDepositName = "$piece_deposit_copper";
        private const string SilverDepositName = "$piece_deposit_silvervein";
        private const string GiantSkullName = "$piece_giant_bone";

        private static void Postfix(MineRock5 __instance)
        {
            if (!PluginConfig.IsModEnabled.Value || __instance == null)
            {
                return;
            }

            if (__instance.m_name == CopperDepositName)
            {
                ResourcePinManager.TryAddOrRelinkResourcePinFromMineRock5Ore(
                    __instance,
                    "rock4_copper",
                    "CopperOre");

                return;
            }

            if (__instance.m_name == SilverDepositName)
            {
                ResourcePinManager.TryAddOrRelinkResourcePinFromMineRock5Ore(
                    __instance,
                    "silvervein",
                    "SilverOre");

                return;
            }

            if (__instance.m_name == GiantSkullName)
            {
                ResourcePinManager.TryAddOrRelinkResourcePinFromMineRock5Ore(
                    __instance,
                    "giant_skull",
                    "Softtissue");

                return;
            }
        }
    }
}