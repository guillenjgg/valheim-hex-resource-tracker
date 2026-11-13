using BepInEx.Configuration;
using System.Collections.Generic;
using System.Reflection;

namespace HexResourceTracker
{
    internal static class ConfigMigration
    {
        private static readonly PropertyInfo OrphanedEntriesProperty = typeof(ConfigFile).GetProperty("OrphanedEntries", BindingFlags.Instance | BindingFlags.NonPublic);

        internal static void Migrate(ConfigFile config)
        {
            if (OrphanedEntriesProperty == null)
            {
                Plugin.Log?.LogWarning("Could not migrate renamed config settings because BepInEx does not expose unbound entries. Existing settings have been preserved.");
                return;
            }

            var entries = OrphanedEntriesProperty.GetValue(config, null) as IDictionary<ConfigDefinition, string>;

            if (entries == null)
            {
                Plugin.Log?.LogWarning("Could not read unbound config entries for migration. Existing settings have been preserved.");
                return;
            }

            MoveEntry(config, entries, "Resources To Track", "Track Beehives", "Resources To Track", "Track Beehive");
            MoveEntry(config, entries, "Deposit Labels", "Beehives Label", "Deposit Labels", "Beehive Label");
            MoveEntry(config, entries, "Resources To Track", "Track Fuling Totem", "Locations To Track", "Track Fuling Totem");
        }

        private static void MoveEntry(ConfigFile config, IDictionary<ConfigDefinition, string> entries, string oldSection, string oldKey, string newSection, string newKey)
        {
            var oldDefinition = new ConfigDefinition(oldSection, oldKey);

            if (!entries.TryGetValue(oldDefinition, out string value))
            {
                return;
            }

            var newDefinition = new ConfigDefinition(newSection, newKey);

            if (!entries.ContainsKey(newDefinition) && !config.ContainsKey(newDefinition))
            {
                entries[newDefinition] = value;
            }

            entries.Remove(oldDefinition);
        }
    }
}
