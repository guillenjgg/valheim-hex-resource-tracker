using HarmonyLib;
using HexResourceTracker.Models;
using System;
using System.Collections.Generic;
using static Minimap;

namespace HexResourceTracker.Core.PinManagers
{
    // Restored pins exist before their world objects load. Keep label identity independently of those objects.
    internal static class SavedPinLabelService
    {
        private sealed class LabelBinding
        {
            internal string DefaultLabel;
            internal string LocationPrefabName;
            internal Func<string> GetLabel;
        }

        private static readonly Dictionary<PinData, LabelBinding> Bindings = new Dictionary<PinData, LabelBinding>();
        private static readonly AccessTools.FieldRef<Minimap, List<PinData>> MapPins = AccessTools.FieldRefAccess<Minimap, List<PinData>>("m_pins");
        private static Minimap _minimap;

        private static void EnsureMap(Minimap minimap)
        {
            if (!ReferenceEquals(_minimap, minimap))
            {
                Bindings.Clear();
                _minimap = minimap;
            }
        }

        internal static void RegisterDungeon(PinData pin, Room.Theme theme)
        {
            EnsureMap(Minimap.instance);
            foreach (TrackedDungeonDefinitionModel definition in TrackedDungeons.AllTrackedDungeons)
            {
                if (definition.Theme == theme)
                {
                    Register(pin, definition.DefaultLabel, () => PluginConfig.GetDungeonLabel(theme));
                    return;
                }
            }
        }

        internal static void RegisterLocation(PinData pin, string prefabName)
        {
            EnsureMap(Minimap.instance);
            if (TrackedLocations.All.TryGetValue(prefabName, out string defaultLabel))
            {
                Register(pin, defaultLabel, () => PluginConfig.GetLocationLabel(prefabName), prefabName);
                LocationPinManager.RegisterSavedLocation(pin, prefabName);
            }
        }

        private static void Register(PinData pin, string defaultLabel, Func<string> getLabel, string locationPrefabName = null)
        {
            if (pin == null || !pin.m_save)
            {
                return;
            }

            Bindings[pin] = new LabelBinding { DefaultLabel = defaultLabel, GetLabel = getLabel, LocationPrefabName = locationPrefabName };
            if (locationPrefabName != null)
            {
                LocationPinIconService.Apply(pin, locationPrefabName);
            }
            PinLabelService.SetLabel(pin, getLabel());
        }

        internal static void Reconcile(Minimap minimap)
        {
            EnsureMap(minimap);
            if (minimap == null || !PluginConfig.IsModEnabled.Value)
            {
                return;
            }

            List<PinData> pins = MapPins(minimap);
            var removed = new List<PinData>();
            foreach (PinData pin in Bindings.Keys)
            {
                if (!pins.Contains(pin))
                {
                    removed.Add(pin);
                }
            }
            foreach (PinData pin in removed)
            {
                Bindings.Remove(pin);
            }

            foreach (PinData pin in new List<PinData>(pins))
            {
                if (!pin.m_save)
                {
                    continue;
                }

                if (!Bindings.ContainsKey(pin))
                {
                    MatchRestoredPin(pin);
                }
                if (Bindings.TryGetValue(pin, out LabelBinding binding))
                {
                    PinLabelService.SetLabel(pin, binding.GetLabel());
                    if (binding.LocationPrefabName != null)
                    {
                        LocationPinIconService.Apply(pin, binding.LocationPrefabName);
                    }
                }
            }
            PinLabelService.RefreshLabels();
        }

        private static bool Matches(string name, string defaultLabel, string configuredLabel)
        {
            return !string.IsNullOrEmpty(name) &&
                (name == defaultLabel || (!string.IsNullOrEmpty(configuredLabel) && name == configuredLabel));
        }

        private static void MatchRestoredPin(PinData pin)
        {
            if (pin.m_type != PinType.Icon2 && pin.m_type != PinType.Icon3)
            {
                return;
            }

            foreach (TrackedDungeonDefinitionModel definition in TrackedDungeons.AllTrackedDungeons)
            {
                PluginConfig.DungeonLabelConfigs.TryGetValue(definition.Theme, out var configured);
                if (Matches(pin.m_name, definition.DefaultLabel, configured?.Value))
                {
                    RegisterDungeon(pin, definition.Theme);
                    return;
                }
            }
            foreach (var definition in TrackedLocations.All)
            {
                PluginConfig.LocationLabelConfigs.TryGetValue(definition.Key, out var configured);
                if (Matches(pin.m_name, definition.Value, configured?.Value))
                {
                    RegisterLocation(pin, definition.Key);
                    return;
                }
            }
        }

        internal static Dictionary<PinData, string> PrepareForSave(Minimap minimap)
        {
            Reconcile(minimap);
            var labels = new Dictionary<PinData, string>();
            foreach (var entry in Bindings)
            {
                labels[entry.Key] = entry.Key.m_name;
                // Save a stable identity even if the displayed label is hidden or customized.
                entry.Key.m_name = entry.Value.DefaultLabel;
            }
            return labels;
        }

        internal static void RestoreAfterSave(Dictionary<PinData, string> labels)
        {
            if (labels == null)
            {
                return;
            }
            foreach (var entry in labels)
            {
                entry.Key.m_name = entry.Value;
            }
        }
    }
}
