using HarmonyLib;
using HexResourceTracker.Core;
using HexResourceTracker.Core.Tracking;
using System.Collections.Generic;
using UnityEngine;
using static Minimap;

namespace HexResourceTracker
{
    internal static class DungeonPinManager
    {
        private const float DuplicateRadius = 5f;

        private static readonly List<DungeonPinModel> DungeonPins = new List<DungeonPinModel>();
        private static readonly AccessTools.FieldRef<Minimap, List<PinData>> MinimapPins = AccessTools.FieldRefAccess<Minimap, List<PinData>>("m_pins");
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
            PinData existingPin = FindExistingDungeonPin(theme, position);

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

            List<PinData> minimapPins = MinimapPins(Minimap.instance);

            if (minimapPins != null)
            {
                for (int i = minimapPins.Count - 1; i >= 0; i--)
                {
                    PinData pin = minimapPins[i];

                    if (pin == null)
                    {
                        continue;
                    }

                    bool isDungeonPin = false;

                    foreach (KeyValuePair<Room.Theme, string> supportedDungeon in SupportedDungeons)
                    {
                        if (!IsMatchingDungeonPin(pin, supportedDungeon.Value))
                        {
                            continue;
                        }

                        isDungeonPin = true;
                        break;
                    }

                    if (!isDungeonPin)
                    {
                        continue;
                    }

                    if (TrackingRangeService.IsWithinTrackingRange(pin.m_pos))
                    {
                        continue;
                    }

                    Minimap.instance.RemovePin(pin);
                }
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

        private static bool CreatePin(DungeonPinModel model)
        {
            if (model == null || Minimap.instance == null)
            {
                return false;
            }

            PinData pin = Minimap.instance.AddPin(
                model.Position,
                PinType.Icon2,
                GetDungeonName(model.Theme),
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

        private static PinData FindExistingDungeonPin(Room.Theme theme, Vector3 position)
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

            string dungeonName = GetDungeonName(theme);
            float radiusSqr = DuplicateRadius * DuplicateRadius;

            foreach (PinData pin in minimapPins)
            {
                if (!IsMatchingDungeonPin(pin, dungeonName))
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

        private static int RemoveExistingDungeonPins(Room.Theme theme)
        {
            string dungeonName = GetDungeonName(theme);
            int removedCount = 0;

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
                removedCount++;
            }

            if (Minimap.instance == null)
            {
                return removedCount;
            }

            List<PinData> minimapPins = MinimapPins(Minimap.instance);

            if (minimapPins == null)
            {
                return removedCount;
            }

            for (int i = minimapPins.Count - 1; i >= 0; i--)
            {
                PinData pin = minimapPins[i];

                if (!IsMatchingDungeonPin(pin, dungeonName))
                {
                    continue;
                }

                Minimap.instance.RemovePin(pin);
                removedCount++;
            }

            return removedCount;
        }

        private static bool IsMatchingDungeonPin(PinData pin, string dungeonName)
        {
            if (pin == null || pin.m_name != dungeonName)
            {
                return false;
            }

            return pin.m_type == PinType.Icon2 ||
                   pin.m_type == PinType.Icon3;
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

        private static string GetDungeonName(Room.Theme theme)
        {
            if (SupportedDungeons.TryGetValue(theme, out string dungeonName))
            {
                return dungeonName;
            }

            return "Dungeon";
        }
    }
}