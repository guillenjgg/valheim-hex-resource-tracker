using System.Collections.Generic;
using HexResourceTracker.Core.Tracking;
using HexResourceTracker.Core.Tracking.Monoscripts;
using UnityEngine;

namespace HexResourceTracker.Core.PinManagers
{
    internal static class LocationPinManager
    {
        private sealed class LocationPin
        {
            internal string PrefabName;
            internal Vector3 Position;
            internal Minimap.PinData Pin;
            internal bool IsChecked;
            internal bool IsSearchResult;
        }

        private static readonly List<LocationPin> KnownLocations = new List<LocationPin>();
        private static Minimap _trackedMinimap;

        internal static void UpdateLocationLabels()
        {
            SavedPinLabelService.Reconcile(Minimap.instance);
            // Configuration changes apply to the session cache, even when no location objects are loaded.
            if (!EnsureMinimapState())
            {
                return;
            }

            ReconcileKnownPins();
        }

        private static bool EnsureMinimapState()
        {
            if (_trackedMinimap != Minimap.instance)
            {
                KnownLocations.Clear();
                _trackedMinimap = Minimap.instance;
            }

            return Minimap.instance != null;
        }

        internal static void RegisterSavedLocation(Minimap.PinData pin, string prefabName)
        {
            if (pin == null || !EnsureMinimapState())
            {
                return;
            }

            foreach (LocationPin model in KnownLocations)
            {
                if (model.PrefabName == prefabName &&
                    new Vector2(model.Position.x - pin.m_pos.x, model.Position.z - pin.m_pos.z).sqrMagnitude <= 25f)
                {
                    // Restored map data may replace a pin while this session cache already exists.
                    if (model.Pin != null && model.Pin != pin)
                    {
                        Minimap.instance.RemovePin(model.Pin);
                    }
                    model.Pin = pin;
                    model.IsChecked = pin.m_checked;
                    return;
                }
            }

            KnownLocations.Add(new LocationPin
            {
                PrefabName = prefabName,
                Position = pin.m_pos,
                Pin = pin,
                IsChecked = pin.m_checked
            });
        }

        internal static void RevealLocation(string prefabName, Vector3 position)
        {
            if (!EnsureMinimapState())
            {
                return;
            }

            LocationPin result = null;
            foreach (LocationPin model in KnownLocations)
            {
                if (model.PrefabName != prefabName)
                {
                    continue;
                }
                model.IsSearchResult = false;
                if (new Vector2(model.Position.x - position.x, model.Position.z - position.z).sqrMagnitude <= 25f)
                {
                    result = model;
                }
            }
            if (result == null)
            {
                result = new LocationPin { PrefabName = prefabName, Position = position };
                KnownLocations.Add(result);
            }
            result.IsSearchResult = true;
            ReconcileKnownPins();
        }

        internal static void ReconcilePins()
        {
            if (!EnsureMinimapState())
            {
                return;
            }

            foreach (TrackedLocation location in TrackedLocation.Loaded)
            {
                if (location == null)
                {
                    continue;
                }

                Vector3 position = location.transform.position;
                bool exists = KnownLocations.Exists(model => model.PrefabName == location.PrefabName &&
                    new Vector2(model.Position.x - position.x, model.Position.z - position.z).sqrMagnitude <= 25f);
                if (!exists)
                {
                    KnownLocations.Add(new LocationPin { PrefabName = location.PrefabName, Position = position });
                }
            }

            ReconcileKnownPins();
        }

        private static void ReconcileKnownPins()
        {
            foreach (LocationPin model in KnownLocations)
            {
                if (model.Pin != null)
                {
                    model.IsChecked = model.Pin.m_checked;
                }

                bool enabled = PluginConfig.IsLocationTrackingEnabled(model.PrefabName);
                if (!enabled)
                {
                    model.IsSearchResult = false;
                }
                bool visible = enabled && (model.IsSearchResult || TrackingRangeService.IsWithinTrackingRange(model.Position));
                if (!visible)
                {
                    if (model.Pin != null)
                    {
                        Minimap.instance.RemovePin(model.Pin);
                        model.Pin = null;
                    }
                    continue;
                }

                if (model.Pin == null)
                {
                    model.Pin = Minimap.instance.AddPin(model.Position, TrackedLocations.GetPinType(model.PrefabName),
                        PluginConfig.GetLocationLabel(model.PrefabName), true, model.IsChecked);
                    SavedPinLabelService.RegisterLocation(model.Pin, model.PrefabName);
                }
                else
                {
                    PinLabelService.SetLabel(model.Pin, PluginConfig.GetLocationLabel(model.PrefabName));
                }
                LocationPinIconService.Apply(model.Pin, model.PrefabName);
            }

            PinLabelService.RefreshLabels();
        }
    }
}
