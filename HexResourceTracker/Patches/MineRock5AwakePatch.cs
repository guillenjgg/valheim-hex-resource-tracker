using HarmonyLib;
using System.Collections.Generic;

namespace HexResourceTracker.Patches
{
    [HarmonyPatch(typeof(MineRock5), nameof(MineRock5.Awake))]
    internal static class MineRock5AwakePatch
    {
        private sealed class MineRock5ResourceDefinition
        {
            internal readonly string ResourcePrefabName;
            internal readonly string ItemPrefabName;

            internal MineRock5ResourceDefinition(string resourcePrefabName, string itemPrefabName)
            {
                ResourcePrefabName = resourcePrefabName;
                ItemPrefabName = itemPrefabName;
            }
        }

        private static readonly Dictionary<string, MineRock5ResourceDefinition> ResourceDefinitionsByMineRockName =
            new Dictionary<string, MineRock5ResourceDefinition>
            {
                { "$piece_deposit_copper", new MineRock5ResourceDefinition("rock4_copper", "CopperOre") },
                { "$piece_deposit_silvervein", new MineRock5ResourceDefinition("silvervein", "SilverOre") },
                { "$piece_giant_bone", new MineRock5ResourceDefinition("giant_skull", "Softtissue") }
            };

        private static void Postfix(MineRock5 __instance)
        {
            if (!PluginConfig.IsModEnabled.Value || __instance == null)
            {
                return;
            }

            if (!ResourceDefinitionsByMineRockName.TryGetValue(__instance.m_name, out MineRock5ResourceDefinition definition))
            {
                return;
            }

            ResourcePinManager.TryAddOrRelinkResourcePinFromMineRock5Ore(
                __instance,
                definition.ResourcePrefabName,
                definition.ItemPrefabName);
        }
    }
}