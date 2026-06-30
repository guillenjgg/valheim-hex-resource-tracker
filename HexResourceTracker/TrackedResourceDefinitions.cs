using HexResourceTracker.Models;
using System.Collections.Generic;

namespace HexResourceTracker
{
    internal static class TrackedResourceDefinitions
    {
        internal static readonly Dictionary<string, ResourceDefinitionModel> DestructibleResourcesByPrefabName =
            new Dictionary<string, ResourceDefinitionModel>
            {
                { "rock4_copper", new ResourceDefinitionModel("rock4_copper", "CopperOre") },
                { "silvervein", new ResourceDefinitionModel("silvervein", "SilverOre") },
                { "giant_skull", new ResourceDefinitionModel("giant_skull", "Softtissue") }
            };

        internal static readonly Dictionary<string, ResourceDefinitionModel> MineRock5ResourcesByName =
            new Dictionary<string, ResourceDefinitionModel>
            {
                { "$piece_deposit_copper", new ResourceDefinitionModel("rock4_copper", "CopperOre") },
                { "$piece_deposit_silvervein", new ResourceDefinitionModel("silvervein", "SilverOre") },
                { "$piece_giant_bone", new ResourceDefinitionModel("giant_skull", "Softtissue") }
            };
    }
}