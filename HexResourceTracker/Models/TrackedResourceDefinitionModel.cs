namespace HexResourceTracker.Models
{
    internal sealed class TrackedResourceDefinitionModel
    {
        internal string ResourcePrefabName { get; }
        internal string DisplayName { get; }
        internal int SortOrder { get; }
        internal TrackedResourceTypeEnum ResourceType { get; }
        internal string IconItemPrefabName { get; }
        internal string MineRock5Name { get; }
        internal string MineRockName { get; }
        internal bool ShowInLocations { get; }
        internal string BiomeName { get; }

        internal TrackedResourceDefinitionModel(string resourcePrefabName, string displayName, int sortOrder, TrackedResourceTypeEnum resourceType, string iconItemPrefabName = null, string mineRock5Name = null, string mineRockName = null, bool showInLocations = false, string biomeName = null)
        {
            ResourcePrefabName = resourcePrefabName;
            DisplayName = displayName;
            SortOrder = sortOrder;
            ResourceType = resourceType;
            IconItemPrefabName = iconItemPrefabName;
            MineRock5Name = mineRock5Name;
            MineRockName = mineRockName;
            ShowInLocations = showInLocations;
            BiomeName = biomeName;
        }
    }
}