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
        // Other prefab names for the same resource (variants); they share its toggle, config and label.
        internal string[] AlsoPrefabNames { get; }

        internal TrackedResourceDefinitionModel(string resourcePrefabName, string displayName, int sortOrder, TrackedResourceTypeEnum resourceType, string iconItemPrefabName = null, string mineRock5Name = null, string mineRockName = null, bool showInLocations = false, string biomeName = null, string[] alsoPrefabNames = null)
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
            AlsoPrefabNames = alsoPrefabNames ?? new string[0];
        }
    }
}
