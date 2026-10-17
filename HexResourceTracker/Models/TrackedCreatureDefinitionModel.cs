namespace HexResourceTracker.Models
{
    internal sealed class TrackedCreatureDefinitionModel
    {
        internal string PrefabName { get; }
        internal string DisplayName { get; }
        internal string TrophyPrefabName { get; }
        internal string CustomSpriteName { get; }
        internal int SortOrder { get; }

        internal TrackedCreatureDefinitionModel(
            string prefabName,
            string displayName,
            string trophyPrefabName,
            int sortOrder,
            string customSpriteName = null)
        {
            PrefabName = prefabName;
            DisplayName = displayName;
            TrophyPrefabName = trophyPrefabName;
            SortOrder = sortOrder;
            CustomSpriteName = customSpriteName;
        }
    }
}