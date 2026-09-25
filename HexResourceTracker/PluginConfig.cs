using BepInEx.Configuration;
using HexResourceTracker.Core;
using HexResourceTracker.Core.PinManagers;
using HexResourceTracker.Core.Tracking;
using HexResourceTracker.Models;
using HexResourceTracker.UI;
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

        private const int HideLabelOrder = 1000;
        private const int LabelOrder = 900;

        internal static ConfigEntry<bool> IsModEnabled { get; private set; }
        internal static ConfigEntry<float> TrackingRange { get; private set; }
        internal static ConfigEntry<TrackingModeEnum> TrackingMode { get; private set; }

        internal static ConfigEntry<bool> HideDungeonLabels { get; private set; }
        internal static ConfigEntry<bool> HideDepositLabels { get; private set; }

        internal static readonly Dictionary<string, ConfigEntry<bool>> ResourceConfigs = new Dictionary<string, ConfigEntry<bool>>();
        internal static readonly Dictionary<Room.Theme, ConfigEntry<bool>> DungeonConfigs = new Dictionary<Room.Theme, ConfigEntry<bool>>();
        internal static readonly Dictionary<string, ConfigEntry<string>> DepositLabelConfigs = new Dictionary<string, ConfigEntry<string>>();
        internal static readonly Dictionary<Room.Theme, ConfigEntry<string>> DungeonLabelConfigs = new Dictionary<Room.Theme, ConfigEntry<string>>();

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
                new ConfigDescription(
                    "Hide labels on tracked dungeon pins.",
                    null,
                    new ConfigurationManagerAttributes
                    {
                        Order = HideLabelOrder
                    }));

            HideDungeonLabels.SettingChanged += delegate
            {
                DungeonPinManager.UpdateDungeonLabels();
            };

            BindDungeonLabel(config, Room.Theme.ForestCrypt, "Burial Chamber");
            BindDungeonLabel(config, Room.Theme.SunkenCrypt, "Sunken Crypt");
            BindDungeonLabel(config, Room.Theme.Cave, "Frost Cave");
            BindDungeonLabel(config, Room.Theme.DvergerTown, "Infested Mine");
            BindDungeonLabel(config, Room.Theme.MorkHalla, "Morkhalla");
            BindDungeonLabel(config, Room.Theme.Hole, "Winding Tunnel");

            HideDepositLabels = config.Bind(
                DepositLabelsSection,
                "Hide Deposit Labels",
                false,
                new ConfigDescription(
                    "Hide labels on tracked deposit pins.",
                    null,
                    new ConfigurationManagerAttributes
                    {
                        Order = HideLabelOrder
                    }));

            HideDepositLabels.SettingChanged += delegate
            {
                ResourcePinManager.UpdateDepositLabels();
            };

            foreach (TrackedResourceDefinition resource in TrackedResources.AllTrackedResources.OrderBy(resource => resource.SortOrder))
            {
                BindResource(config, resource.ResourcePrefabName, resource.DisplayName);

                if (resource.ResourceType == TrackedResourceTypeEnum.Deposit)
                {
                    BindDepositLabel(config, resource);
                }
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

        internal static string GetDepositLabel(string prefabName)
        {
            if (HideDepositLabels.Value)
            {
                return string.Empty;
            }

            if (!DepositLabelConfigs.TryGetValue(prefabName, out ConfigEntry<string> labelConfig))
            {
                return string.Empty;
            }

            return labelConfig.Value;
        }

        internal static string GetDungeonLabel(Room.Theme theme)
        {
            if (HideDungeonLabels.Value)
            {
                return string.Empty;
            }

            if (!DungeonLabelConfigs.TryGetValue(theme, out ConfigEntry<string> labelConfig))
            {
                return string.Empty;
            }

            return labelConfig.Value;
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

        private static void BindDepositLabel(ConfigFile config, TrackedResourceDefinition resource)
        {
            ConfigEntry<string> entry = config.Bind(
                DepositLabelsSection,
                $"{resource.DisplayName} Label",
                resource.DisplayName,
                new ConfigDescription(
                    $"Label displayed for {resource.DisplayName} deposits.",
                    null,
                    new ConfigurationManagerAttributes
                    {
                        Order = LabelOrder
                    }));

            entry.SettingChanged += delegate
            {
                ResourcePinManager.UpdateDepositLabels();
            };

            DepositLabelConfigs[resource.ResourcePrefabName] = entry;
        }

        private static void BindDungeonLabel(ConfigFile config, Room.Theme theme, string displayName)
        {
            ConfigEntry<string> entry = config.Bind(
                DungeonLabelsSection,
                $"{displayName} Label",
                displayName,
                new ConfigDescription(
                    $"Label displayed for {displayName}.",
                    null,
                    new ConfigurationManagerAttributes
                    {
                        Order = LabelOrder
                    }));

            entry.SettingChanged += delegate
            {
                DungeonPinManager.UpdateDungeonLabels();
            };

            DungeonLabelConfigs[theme] = entry;
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

        private sealed class ConfigurationManagerAttributes
        {
            public int? Order;
        }
    }
}