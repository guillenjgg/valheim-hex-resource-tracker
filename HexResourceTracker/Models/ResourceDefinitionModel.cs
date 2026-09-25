using System;

namespace HexResourceTracker.Models
{
    [Obsolete("Delete this class after refactor")]
    internal class ResourceDefinitionModel
    {
        internal string ResourcePrefabName { get; }
        internal string ItemPrefabName { get; }

        internal ResourceDefinitionModel(string resourcePrefabName, string itemPrefabName)
        {
            ResourcePrefabName = resourcePrefabName;
            ItemPrefabName = itemPrefabName;
        }
    }
}
