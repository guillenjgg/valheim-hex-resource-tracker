using HarmonyLib;
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

            if (!PluginConfig.IsDungeonTrackingEnabled(theme))
            {
                return false;
            }

            string locationName = Utils.GetPrefabName(location.gameObject);
            Vector3 position = location.transform.position;

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

#if DEBUG
                Plugin.Log.LogInfo(
                    $"[DungeonPins] Reconnected existing {GetDungeonName(theme)} pin at " +
                    $"X={position.x}, Y={position.y}, Z={position.z}. " +
                    $"Checked={model.IsChecked}.");
#endif
            }
            else if (!CreatePin(model))
            {
#if DEBUG
                Plugin.Log.LogWarning($"[DungeonPins] Failed to create pin for {locationName}.");
#endif
                return false;
            }
            else
            {
#if DEBUG
                Plugin.Log.LogInfo(
                    $"[DungeonPins] Added {GetDungeonName(theme)} pin at " +
                    $"X={position.x}, Y={position.y}, Z={position.z}.");
#endif
            }

            DungeonPins.Add(model);
            return true;
        }

        internal static void HandleDungeonTrackingChanged(Room.Theme theme, bool isEnabled)
        {
            EnsureMinimapState();

            int removedCount = RemoveExistingDungeonPins(theme);

            if (!isEnabled)
            {
#if DEBUG
                Plugin.Log.LogInfo(
                    $"[DungeonPins] Disabled {GetDungeonName(theme)} tracking. " +
                    $"Removed {removedCount} dungeon pin(s).");
#endif
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

#if DEBUG
                Plugin.Log.LogInfo(
                    $"[DungeonPins] Updated {GetDungeonName(model.Theme)} checked state: " +
                    $"{model.IsChecked}.");
#endif
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

                if (!CreatePin(model))
                {
#if DEBUG
                    Plugin.Log.LogWarning(
                        $"[DungeonPins] Failed to redraw {GetDungeonName(model.Theme)} pin at " +
                        $"X={model.Position.x}, Y={model.Position.y}, Z={model.Position.z}.");
#endif
                }
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

            Location[] locations = Object.FindObjectsByType<Location>(
                FindObjectsInactive.Include,
                FindObjectsSortMode.None);

            int addedCount = 0;

            foreach (Location location in locations)
            {
                if (!TryGetSupportedDungeonTheme(location, out Room.Theme locationTheme))
                {
                    continue;
                }

                if (locationTheme != theme)
                {
                    continue;
                }

                if (TryAddDungeonPin(location))
                {
                    addedCount++;
                }
            }

#if DEBUG
            Plugin.Log.LogInfo(
                $"[DungeonPins] Enabled {GetDungeonName(theme)} tracking. " +
                $"Added {addedCount} loaded dungeon pin(s).");
#endif
        }

        private static bool TryGetSupportedDungeonTheme(Location location, out Room.Theme theme)
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
            return IsSupportedDungeon(theme);
        }

        private static bool IsSupportedDungeon(Room.Theme theme)
        {
            switch (theme)
            {
                case Room.Theme.ForestCrypt:
                case Room.Theme.SunkenCrypt:
                case Room.Theme.Cave:
                case Room.Theme.DvergerTown:
                    return true;
                default:
                    return false;
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
            switch (theme)
            {
                case Room.Theme.ForestCrypt:
                    return "Burial Chamber";
                case Room.Theme.SunkenCrypt:
                    return "Sunken Crypt";
                case Room.Theme.Cave:
                    return "Frost Cave";
                case Room.Theme.DvergerTown:
                    return "Infested Mine";
                default:
                    return "Dungeon";
            }
        }
    }
}