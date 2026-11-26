namespace HexResourceTracker.Models
{
    internal sealed class TrackedCreatureDefinitionModel
    {
        internal string PrefabName { get; }
        internal string DisplayName { get; }
        internal string TrophyPrefabName { get; }
        internal string CustomSpriteName { get; }
        internal int SortOrder { get; }
        internal bool EnabledByDefault { get; }
        // Other prefab names for the same creature (for example a sleeping variant); they share its toggle.
        internal string[] AlsoPrefabNames { get; }

        internal TrackedCreatureDefinitionModel(
            string prefabName,
            string displayName,
            string trophyPrefabName,
            int sortOrder,
            string customSpriteName = null,
            bool enabledByDefault = true,
            string[] alsoPrefabNames = null)
        {
            PrefabName = prefabName;
            DisplayName = displayName;
            TrophyPrefabName = trophyPrefabName;
            SortOrder = sortOrder;
            CustomSpriteName = customSpriteName;
            EnabledByDefault = enabledByDefault;
            AlsoPrefabNames = alsoPrefabNames ?? new string[0];
        }
    }
}