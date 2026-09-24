using BepInEx.Configuration;
using HexResourceTracker.Core;
using HexResourceTracker.Core.Tracking;
using HexResourceTracker.Models;
using System.Collections.Generic;
using System.Linq;

namespace HexResourceTracker
{
    internal static class PluginConfig
    {
        private const string GeneralSection = "General";
        private const string ResourcesSection = "Resources To Track";
        private const string DungeonsSection = "Dungeons To Track";
        private const string TrackingModeSection = "Tracking Mode";
        private const string DungeonLabelsSection = "Dungeon Labels";
        private const string DepositLabelsSection = "Deposit Labels";

        internal static ConfigEntry<bool> IsModEnabled { get; private set; }
        internal static ConfigEntry<float> TrackingRange { get; private set; }
        internal static ConfigEntry<TrackingModeEnum> TrackingMode { get; private set; }

        internal static ConfigEntry<bool> HideDungeonLabels { get; private set; }
        internal static ConfigEntry<string> BurialChamberLabel { get; private set; }
        internal static ConfigEntry<string> SunkenCryptLabel { get; private set; }
        internal static ConfigEntry<string> FrostCaveLabel { get; private set; }
        internal static ConfigEntry<string> InfestedMineLabel { get; private set; }
        internal static ConfigEntry<string> MorkhallaLabel { get; private set; }
        internal static ConfigEntry<string> WindingTunnelLabel { get; private set; }

        internal static ConfigEntry<bool> HideDepositLabels { get; private set; }
        internal static ConfigEntry<string> CopperLabel { get; private set; }
        internal static ConfigEntry<string> SilverLabel { get; private set; }
        internal static ConfigEntry<string> GiantSkullLabel { get; private set; }
        internal static ConfigEntry<string> FlametalLabel { get; private set; }

        internal static readonly Dictionary<string, ConfigEntry<bool>> ResourceConfigs = new Dictionary<string, ConfigEntry<bool>>();
        internal static readonly Dictionary<Room.Theme, ConfigEntry<bool>> DungeonConfigs = new Dictionary<Room.Theme, ConfigEntry<bool>>();

        internal static void Initialize(ConfigFile config)
        {
            IsModEnabled = config.Bind(
                GeneralSection,
                "Enable",
                true,
                "Enable or disable the HexResourceTracker mod.");

            TrackingMode = config.Bind(
                TrackingModeSection,
                "Tracking Mode",
                TrackingModeEnum.ZoneBased,
                "Controls how tracked map objects are discovered. ZoneBased adds pins as Valheim loads zones. RangeScanner only displays tracked objects within the configured range of the player.");

            TrackingMode.SettingChanged += delegate
            {
                MapTrackingScanner.HandleTrackingModeChanged();
            };

            TrackingRange = config.Bind(
                TrackingModeSection,
                "Tracking Range",
                50f,
                new ConfigDescription(
                    "Maximum distance in meters from the player to track resources and dungeons when using RangeScanner mode.",
                    new AcceptableValueRange<float>(50f, 2000f)));

            TrackingRange.SettingChanged += delegate
            {
                MapTrackingScanner.ForceRescan();
            };

            HideDungeonLabels = config.Bind(
                DungeonLabelsSection,
                "Hide Dungeon Labels",
                false,
                "Hide labels on tracked dungeon pins.");

            BurialChamberLabel = config.Bind(
                DungeonLabelsSection,
                "Burial Chamber Label",
                "Burial Chamber",
                "Label displayed for Burial Chambers.");

            SunkenCryptLabel = config.Bind(
                DungeonLabelsSection,
                "Sunken Crypt Label",
                "Sunken Crypt",
                "Label displayed for Sunken Crypts.");

            FrostCaveLabel = config.Bind(
                DungeonLabelsSection,
                "Frost Cave Label",
                "Frost Cave",
                "Label displayed for Frost Caves.");

            InfestedMineLabel = config.Bind(
                DungeonLabelsSection,
                "Infested Mine Label",
                "Infested Mine",
                "Label displayed for Infested Mines.");

            MorkhallaLabel = config.Bind(
                DungeonLabelsSection,
                "Morkhalla Label",
                "Morkhalla",
                "Label displayed for Morkhalla.");

            WindingTunnelLabel = config.Bind(
                DungeonLabelsSection,
                "Winding Tunnel Label",
                "Winding Tunnel",
                "Label displayed for Winding Tunnels.");

            HideDepositLabels = config.Bind(
                DepositLabelsSection,
                "Hide Deposit Labels",
                false,
                "Hide labels on tracked deposit pins.");

            CopperLabel = config.Bind(
                DepositLabelsSection,
                "Copper Label",
                "Copper",
                "Label displayed for Copper deposits.");

            SilverLabel = config.Bind(
                DepositLabelsSection,
                "Silver Label",
                "Silver",
                "Label displayed for Silver deposits.");

            GiantSkullLabel = config.Bind(
                DepositLabelsSection,
                "Giant Skull Label",
                "Giant Skull",
                "Label displayed for Giant Skulls.");

            FlametalLabel = config.Bind(
                DepositLabelsSection,
                "Flametal Label",
                "Flametal",
                "Label displayed for Flametal deposits.");

            foreach (TrackedResourceDefinition resource in TrackedResources.AllTrackedResources.OrderBy(resource => resource.SortOrder))
            {
                BindResource(config, resource.ResourcePrefabName, resource.DisplayName);
            }

            BindDungeon(config, Room.Theme.ForestCrypt, "Burial Chambers");
            BindDungeon(config, Room.Theme.SunkenCrypt, "Sunken Crypts");
            BindDungeon(config, Room.Theme.Cave, "Frost Caves");
            BindDungeon(config, Room.Theme.DvergerTown, "Infested Mines");
            BindDungeon(config, Room.Theme.MorkHalla, "Morkhalla");
            BindDungeon(config, Room.Theme.Hole, "Winding Tunnel");
        }

        internal static bool IsResourceTrackingEnabled(string prefabName)
        {
            return IsModEnabled.Value &&
                   ResourceConfigs.TryGetValue(prefabName, out ConfigEntry<bool> config) &&
                   config.Value;
        }

        internal static bool IsDungeonTrackingEnabled(Room.Theme theme)
        {
            return IsModEnabled.Value &&
                   DungeonConfigs.TryGetValue(theme, out ConfigEntry<bool> config) &&
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
                ContainerResourcePinService.HandleResourceTrackingChanged(prefabName, entry.Value);
            };

            ResourceConfigs[prefabName] = entry;
        }

        private static void BindDungeon(ConfigFile config, Room.Theme theme, string displayName)
        {
            ConfigEntry<bool> entry = config.Bind(
                DungeonsSection,
                $"Track {displayName}",
                true,
                $"Enable or disable tracking for {displayName}.");

            entry.SettingChanged += delegate
            {
                ResourceTrackerMapOverlay.HandleDungeonTrackingChanged(theme, entry.Value);
                DungeonPinManager.HandleDungeonTrackingChanged(theme, entry.Value);
            };

            DungeonConfigs[theme] = entry;
        }
    }
}