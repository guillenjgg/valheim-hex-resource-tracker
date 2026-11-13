using BepInEx.Configuration;
using System;
using System.IO;

namespace HexResourceTracker
{
    internal static class Plugin
    {
        internal static BepInEx.Logging.ManualLogSource Log;
    }

    internal static class ConfigMigrationChecks
    {
        private static void Main()
        {
            string directory = Path.Combine(Path.GetTempPath(), "HexResourceTrackerConfigChecks", Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(directory);

            string path = Path.Combine(directory, "migration.cfg");
            File.WriteAllText(path, "[Resources To Track]\nTrack Beehives = false\nTrack Fuling Totem = false\n[Deposit Labels]\nBeehives Label = My Hive\n[Unrelated]\nKeep Me = custom\n");

            var config = new ConfigFile(path, false);
            config.SaveOnConfigSet = false;
            ConfigMigration.Migrate(config);

            Check(!config.Bind("Resources To Track", "Track Beehive", true).Value, "Beehive preference preserved");
            Check(!config.Bind("Locations To Track", "Track Fuling Totem", true).Value, "Totem preference moved");
            Check(config.Bind("Deposit Labels", "Beehive Label", "Beehive").Value == "My Hive", "Custom label preserved");
            Check(config.Bind("Map Overlay", "Overlay Collapsed", false).Value == false, "New setting receives default");
            config.Save();

            string saved = File.ReadAllText(path);
            Check(!saved.Contains("Track Beehives ="), "Old toggle removed");
            Check(!saved.Contains("Beehives Label ="), "Old label removed");
            Check(saved.Contains("Keep Me = custom"), "Unrelated entry retained");

            var reloaded = new ConfigFile(path, false);
            reloaded.SaveOnConfigSet = false;
            ConfigMigration.Migrate(reloaded);
            Check(!reloaded.Bind("Locations To Track", "Track Fuling Totem", true).Value, "Migration survives reload and repeat");

            string conflictPath = Path.Combine(directory, "conflict.cfg");
            File.WriteAllText(conflictPath, "[Resources To Track]\nTrack Beehives = false\nTrack Beehive = true\nTrack Fuling Totem = false\n[Locations To Track]\nTrack Fuling Totem = true\n");

            var conflicting = new ConfigFile(conflictPath, false);
            conflicting.SaveOnConfigSet = false;
            ConfigMigration.Migrate(conflicting);
            Check(conflicting.Bind("Resources To Track", "Track Beehive", false).Value, "New beehive preference wins");
            Check(conflicting.Bind("Locations To Track", "Track Fuling Totem", false).Value, "New totem preference wins");
            Console.WriteLine("All config migration checks passed.");
        }

        private static void Check(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }

            Console.WriteLine(message);
        }
    }
}
