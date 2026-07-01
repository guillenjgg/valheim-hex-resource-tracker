using BepInEx.Configuration;
using HexResourceTracker.Core;
using System.Collections.Generic;

namespace HexResourceTracker
{
    internal static class PluginConfig
    {
        private const string GeneralSection = "General";
        private const string ResourcesSection = "Resources To Track";

        internal static ConfigEntry<bool> IsModEnabled { get; private set; }

        internal static readonly Dictionary<string, ConfigEntry<bool>> ResourceConfigs = new Dictionary<string, ConfigEntry<bool>>();

        internal static void Initialize(ConfigFile config)
        {
            IsModEnabled = config.Bind(
                GeneralSection,
                "Enable",
                true,
                "Enable or disable the HexResourceTracker mod.");

            BindResource(config, "Pickable_Mushroom", "Mushrooms");
            BindResource(config, "Pickable_Dandelion", "Dandelions");
            BindResource(config, "RaspberryBush", "Raspberries");
            BindResource(config, "BlueberryBush", "Blueberries");
            BindResource(config, "Pickable_Thistle", "Thistle");
            BindResource(config, "Pickable_SeedCarrot", "Carrot Seeds");
            BindResource(config, "Pickable_SeedTurnip", "Turnip Seeds");
            BindResource(config, "Pickable_Flax_Wild", "Flax");
            BindResource(config, "Pickable_Barley_Wild", "Barley");
            BindResource(config, "CloudberryBush", "Cloudberries");
            BindResource(config, "Pickable_Mushroom_JotunPuffs", "Jotun Puffs");
            BindResource(config, "Pickable_Mushroom_Magecap", "Magecap");
            BindResource(config, "rock4_copper", "Copper");
            BindResource(config, "silvervein", "Silver");
            BindResource(config, "giant_skull", "Giant Skull");
            BindResource(config, "LeviathanLava", "Flametal");
            BindResource(config, "VineAsh", "Vineberries");
            BindResource(config, "Pickable_SmokePuff", "Smoke Puffs");
            BindResource(config, "Pickable_Fiddlehead", "Fiddleheads");
        }

        internal static bool IsResourceTrackingEnabled(string prefabName)
        {
            return IsModEnabled.Value &&
                   ResourceConfigs.TryGetValue(prefabName, out ConfigEntry<bool> config) &&
                   config.Value;
        }

        private static void BindResource(ConfigFile config, string prefabName, string displayName)
        {
            ConfigEntry<bool> entry = config.Bind(
                ResourcesSection,
                $"Track {displayName}",
                true,
                $"Enable or disable tracking for {displayName}.");

            entry.SettingChanged += delegate
            {
                ResourceTrackerMapOverlay.HandleResourceTrackingChanged(prefabName, entry.Value);
                PickableResourcePinService.HandleResourceTrackingChanged(prefabName, entry.Value);
                OreResourcePinService.HandleResourceTrackingChanged(prefabName, entry.Value);
            };

            ResourceConfigs[prefabName] = entry;
        }
    }
}