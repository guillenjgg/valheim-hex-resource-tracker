using System;
using System.Collections.Generic;
using HexResourceTracker.Models;

namespace HexResourceTracker.Core
{
    [Obsolete("Delete this class after refactor")]
    internal static class TrackedResourceDefinitions
    {
        internal static readonly Dictionary<string, ResourceDefinitionModel> DestructibleResourcesByPrefabName =
            new Dictionary<string, ResourceDefinitionModel>
            {
                { "MineRock_Tin", new ResourceDefinitionModel("MineRock_Tin", "TinOre") },
                { "rock4_copper", new ResourceDefinitionModel("rock4_copper", "CopperOre") },
                { "silvervein", new ResourceDefinitionModel("silvervein", "SilverOre") },
                { "giant_skull", new ResourceDefinitionModel("giant_skull", "Softtissue") },
                { "MineRock_Obsidian", new ResourceDefinitionModel("MineRock_Obsidian", "Obsidian") },
                { "mudpile", new ResourceDefinitionModel("mudpile", "IronScrap") },
                { "mudpile2", new ResourceDefinitionModel("mudpile", "IronScrap") },
                { "mudpile_beacon", new ResourceDefinitionModel("mudpile", "IronScrap") },
                { "GuckSack", new ResourceDefinitionModel("GuckSack", "Guck") },
                { "GuckSack_small", new ResourceDefinitionModel("GuckSack", "Guck") },
                { "Spawner_GreydwarfNest", new ResourceDefinitionModel("Spawner_GreydwarfNest", "AncientSeed") },
                { "giant_sword1", new ResourceDefinitionModel("giant_sword1", "IronScrap") },
                { "giant_sword2", new ResourceDefinitionModel("giant_sword1", "IronScrap") },
                { "giant_helmet1", new ResourceDefinitionModel("giant_helmet1", "IronScrap") },
                { "giant_helmet2", new ResourceDefinitionModel("giant_helmet1", "IronScrap") }
            };

        internal static readonly Dictionary<string, ResourceDefinitionModel> MineRock5ResourcesByName =
            new Dictionary<string, ResourceDefinitionModel>
            {
                { "$piece_deposit_copper", new ResourceDefinitionModel("rock4_copper", "CopperOre") },
                { "$piece_deposit_silvervein", new ResourceDefinitionModel("silvervein", "SilverOre") },
                { "$piece_giant_bone", new ResourceDefinitionModel("giant_skull", "Softtissue") }
            };

        internal static readonly Dictionary<string, ResourceDefinitionModel> MineRockResourcesByName =
            new Dictionary<string, ResourceDefinitionModel>
            {
                { "$item_flametalore", new ResourceDefinitionModel("LeviathanLava", "FlametalOreNew") }
            };
    }
}