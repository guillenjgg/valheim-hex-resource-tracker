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

            string locationName = Utils.GetPrefabName(location.gameObject);

            if (!TryGetSupportedDungeonTheme(location, out Room.Theme theme))
            {
                return false;
            }

            if (!PluginConfig.IsDungeonTrackingEnabled(theme))
            {
                return false;
            }

            if (Minimap.instance == null)
            {
                return false;
            }

            Vector3 position = location.transform.position;

            if (HasDungeonPin(theme, position))
            {
                return false;
            }

            string dungeonName = GetDungeonName(theme);

            PinData pin = Minimap.instance.AddPin(
                position,
                PinType.Icon2,// hammer icon
                dungeonName,
                false,
                false);

            if (pin == null)
            {
                #if DEBUG
                Plugin.Log.LogWarning($"[DungeonPins] Failed to create pin for {locationName}.");
                #endif
                return false;
            }

            DungeonPinModel model = new DungeonPinModel(theme, locationName, position)
            {
                Pin = pin
            };

            DungeonPins.Add(model);

            #if DEBUG
            Plugin.Log.LogInfo(
                $"[DungeonPins] Added {dungeonName} pin at " +
                $"X={position.x}, Y={position.y}, Z={position.z}.");
            #endif

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

            if (generator == null)
            {
                return false;
            }

            if (generator.m_algorithm != DungeonGenerator.Algorithm.Dungeon)
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

                float deltaX = model.Position.x - position.x;
                float deltaZ = model.Position.z - position.z;
                float distanceSqr = (deltaX * deltaX) + (deltaZ * deltaZ);

                if (distanceSqr <= radiusSqr)
                {
                    return true;
                }
            }

            return false;
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

            if(minimapPins == null)
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