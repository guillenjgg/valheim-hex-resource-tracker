namespace HexResourceTracker.Models
{
    internal sealed class TrackedDungeonDefinitionModel
    {
        internal Room.Theme Theme { get; }
        internal string ConfigKeyName { get; }
        internal string UiDisplayName { get; }
        internal string DefaultLabel { get; }
        internal int SortOrder { get; }

        internal TrackedDungeonDefinitionModel(
            Room.Theme theme,
            string configKeyName,
            string uiDisplayName,
            string defaultLabel,
            int sortOrder)
        {
            Theme = theme;
            ConfigKeyName = configKeyName;
            UiDisplayName = uiDisplayName;
            DefaultLabel = defaultLabel;
            SortOrder = sortOrder;
        }
    }
}
