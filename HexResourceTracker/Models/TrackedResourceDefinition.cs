namespace HexResourceTracker.Models
{
    internal sealed class TrackedResourceDefinition
    {
        internal string ResourcePrefabName { get; }
        internal string DisplayName { get; }
        internal int SortOrder { get; }
        internal TrackedResourceTypeEnum ResourceType { get; }
        internal string IconItemPrefabName { get; }
        internal string MineRock5Name { get; }
        internal string MineRockName { get; }

        internal TrackedResourceDefinition(
            string resourcePrefabName,
            string displayName,
            int sortOrder,
            TrackedResourceTypeEnum resourceType,
            string iconItemPrefabName = null,
            string mineRock5Name = null,
            string mineRockName = null)
        {
            ResourcePrefabName = resourcePrefabName;
            DisplayName = displayName;
            SortOrder = sortOrder;
            ResourceType = resourceType;
            IconItemPrefabName = iconItemPrefabName;
            MineRock5Name = mineRock5Name;
            MineRockName = mineRockName;
        }
    }
}