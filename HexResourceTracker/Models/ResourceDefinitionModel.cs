namespace HexResourceTracker.Models
{
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