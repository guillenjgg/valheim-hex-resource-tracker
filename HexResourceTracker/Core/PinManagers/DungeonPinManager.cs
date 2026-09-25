using HarmonyLib;
using HexResourceTracker.Core.Tracking;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static Minimap;

namespace HexResourceTracker.Core.PinManagers
{
    internal static class DungeonPinManager
    {
        private const float DuplicateRadius = 5f;
        private const float ExistingPinMatchRadius = 0.1f;

        private static readonly List<DungeonPinModel> DungeonPins = new List<DungeonPinModel>();
        private static readonly AccessTools.FieldRef<Minimap, List<PinData>> MinimapPins = AccessTools.FieldRefAccess<Minimap, List<PinData>>("m_pins");
        private static readonly FieldInfo MinimapPinUpdateRequiredField = AccessTools.Field(typeof(Minimap), "m_pinUpdateRequired");

        private static Minimap _trackedMinimap;

        private static readonly Dictionary<Room.Theme, string> SupportedDungeons = new Dictionary<Room.Theme, string>
        {
            { Room.Theme.ForestCrypt, "Burial Chamber" },
            { Room.Theme.SunkenCrypt, "Sunken Crypt" },
            { Room.Theme.Cave, "Frost Cave" },
            { Room.Theme.DvergerTown, "Infested Mine" },
            { Room.Theme.MorkHalla, "Morkhalla" },
            { Room.Theme.Hole, "Winding Tunnel" }
        };

        internal static bool TryAddDungeonPin(Location location)
        {
            if (location == null)
            {
                return false;
            }

            EnsureMinimapState();

            if (Minimap.instance == null)
            {
                return false;
            }

            if (!TryGetSupportedDungeonTheme(location, out Room.Theme theme))
            {
                return false;
            }

            string locationName = Utils.GetPrefabName(location.gameObject);
            Vector3 position = location.transform.position;

            if (!PluginConfig.IsDungeonTrackingEnabled(theme))
            {
                return false;
            }

            if (!TrackingRangeService.IsWithinTrackingRange(position))
            {
                return false;
            }

            if (HasDungeonPin(theme, position))
            {
                return false;
            }

            DungeonPinModel model = new DungeonPinModel(theme, locationName, position);
            PinData existingPin = FindExistingDungeonPin(position);

            if (existingPin != null)
            {
                model.Pin = existingPin;
                model.IsChecked = existingPin.m_checked;
            }
            else if (!CreatePin(model))
            {
                return false;
            }

            DungeonPins.Add(model);
            return true;
        }

        internal static void HandleDungeonTrackingChanged(Room.Theme theme, bool isEnabled)
        {
            EnsureMinimapState();

            RemoveExistingDungeonPins(theme);

            if (!isEnabled)
            {
                return;
            }

            AddLoadedDungeonPins(theme);
        }

        internal static void UpdateCheckedStates()
        {
            EnsureMinimapState();

            foreach (DungeonPinModel model in DungeonPins)
            {
                if (model.Pin == null || model.IsChecked == model.Pin.m_checked)
                {
                    continue;
                }

                model.IsChecked = model.Pin.m_checked;
            }
        }

        internal static void RedrawDungeonPins()
        {
            EnsureMinimapState();

            if (Minimap.instance == null)
            {
                return;
            }

            foreach (DungeonPinModel model in DungeonPins)
            {
                if (model.Pin != null)
                {
                    Minimap.instance.RemovePin(model.Pin);
                    model.Pin = null;
                }

                CreatePin(model);
            }
        }

        internal static void ReconcileTrackingRange()
        {
            EnsureMinimapState();

            if (Minimap.instance == null || Player.m_localPlayer == null)
            {
                return;
            }

            for (int i = DungeonPins.Count - 1; i >= 0; i--)
            {
                DungeonPinModel model = DungeonPins[i];

                if (TrackingRangeService.IsWithinTrackingRange(model.Position))
                {
                    continue;
                }

                if (model.Pin != null)
                {
                    Minimap.instance.RemovePin(model.Pin);
                    model.Pin = null;
                }

                DungeonPins.RemoveAt(i);
            }

            foreach (TrackedDungeonLocation trackedLocation in TrackedDungeonLocation.GetTrackedLocations())
            {
                if (trackedLocation == null || trackedLocation.Location == null)
                {
                    continue;
                }

                TryAddDungeonPin(trackedLocation.Location);
            }
        }

        internal static void AddLoadedDungeonPins()
        {
            EnsureMinimapState();

            if (Minimap.instance == null)
            {
                return;
            }

            foreach (TrackedDungeonLocation trackedLocation in TrackedDungeonLocation.GetTrackedLocations())
            {
                if (trackedLocation == null || trackedLocation.Location == null)
                {
                    continue;
                }

                TryAddDungeonPin(trackedLocation.Location);
            }
        }

        internal static bool TryGetSupportedDungeonTheme(Location location, out Room.Theme theme)
        {
            theme = Room.Theme.None;

            if (location == null || !location.m_hasInterior)
            {
                return false;
            }

            DungeonGenerator generator = location.GetComponentInChildren<DungeonGenerator>(true);

            if (generator == null || generator.m_algorithm != DungeonGenerator.Algorithm.Dungeon)
            {
                return false;
            }

            theme = generator.m_themes;

            return SupportedDungeons.ContainsKey(theme);
        }

        internal static void UpdateDungeonLabels()
        {
            EnsureMinimapState();

            if (Minimap.instance == null)
            {
                return;
            }

            foreach (DungeonPinModel model in DungeonPins)
            {
                if (model.Pin == null)
                {
                    continue;
                }

                string label = PluginConfig.GetDungeonLabel(model.Theme);

                model.Pin.m_name = label;

                if (model.Pin.m_NamePinData != null && model.Pin.m_NamePinData.PinNameText != null)
                {
                    model.Pin.m_NamePinData.PinNameText.text = label;
                }

                model.Pin.m_NamePinData = string.IsNullOrEmpty(label)
                    ? null
                    : model.Pin.m_NamePinData ?? new Minimap.PinNameData(model.Pin);
            }

            MinimapPinUpdateRequiredField.SetValue(Minimap.instance, true);
        }

        private static bool CreatePin(DungeonPinModel model)
        {
            if (model == null || Minimap.instance == null)
            {
                return false;
            }

            PinData pin = Minimap.instance.AddPin(
                model.Position,
                PinType.Icon2,
                PluginConfig.GetDungeonLabel(model.Theme),
                true,
                model.IsChecked);

            if (pin == null)
            {
                return false;
            }

            model.Pin = pin;
            return true;
        }

        private static void AddLoadedDungeonPins(Room.Theme theme)
        {
            if (Minimap.instance == null)
            {
                return;
            }

            foreach (TrackedDungeonLocation trackedLocation in TrackedDungeonLocation.GetTrackedLocations())
            {
                if (trackedLocation == null || trackedLocation.Location == null)
                {
                    continue;
                }

                Location location = trackedLocation.Location;

                if (!TryGetSupportedDungeonTheme(location, out Room.Theme locationTheme))
                {
                    continue;
                }

                if (locationTheme != theme)
                {
                    continue;
                }

                TryAddDungeonPin(location);
            }
        }

        private static bool HasDungeonPin(Room.Theme theme, Vector3 position)
        {
            float radiusSqr = DuplicateRadius * DuplicateRadius;

            foreach (DungeonPinModel model in DungeonPins)
            {
                if (model.Theme != theme)
                {
                    continue;
                }

                if (IsWithinRadius(model.Position, position, radiusSqr))
                {
                    return true;
                }
            }

            return false;
        }

        private static PinData FindExistingDungeonPin(Vector3 position)
        {
            if (Minimap.instance == null)
            {
                return null;
            }

            List<PinData> minimapPins = MinimapPins(Minimap.instance);

            if (minimapPins == null)
            {
                return null;
            }

            float radiusSqr = ExistingPinMatchRadius * ExistingPinMatchRadius;

            foreach (PinData pin in minimapPins)
            {
                if (pin == null || (pin.m_type != PinType.Icon2 && pin.m_type != PinType.Icon3))
                {
                    continue;
                }

                if (IsWithinRadius(pin.m_pos, position, radiusSqr))
                {
                    return pin;
                }
            }

            return null;
        }

        private static bool IsWithinRadius(Vector3 firstPosition, Vector3 secondPosition, float radiusSqr)
        {
            float deltaX = firstPosition.x - secondPosition.x;
            float deltaZ = firstPosition.z - secondPosition.z;
            float distanceSqr = (deltaX * deltaX) + (deltaZ * deltaZ);

            return distanceSqr <= radiusSqr;
        }

        private static void RemoveExistingDungeonPins(Room.Theme theme)
        {
            for (int i = DungeonPins.Count - 1; i >= 0; i--)
            {
                DungeonPinModel model = DungeonPins[i];

                if (model.Theme != theme)
                {
                    continue;
                }

                if (model.Pin != null && Minimap.instance != null)
                {
                    Minimap.instance.RemovePin(model.Pin);
                }

                DungeonPins.RemoveAt(i);
            }

            if (Minimap.instance == null)
            {
                return;
            }

            foreach (TrackedDungeonLocation trackedLocation in TrackedDungeonLocation.GetTrackedLocations())
            {
                if (trackedLocation == null || trackedLocation.Location == null)
                {
                    continue;
                }

                Location location = trackedLocation.Location;

                if (!TryGetSupportedDungeonTheme(location, out Room.Theme locationTheme) || locationTheme != theme)
                {
                    continue;
                }

                PinData pin = FindExistingDungeonPin(location.transform.position);

                if (pin != null)
                {
                    Minimap.instance.RemovePin(pin);
                }
            }
        }

        private static void EnsureMinimapState()
        {
            if (_trackedMinimap == Minimap.instance)
            {
                return;
            }

            DungeonPins.Clear();
            _trackedMinimap = Minimap.instance;
        }
    }
}