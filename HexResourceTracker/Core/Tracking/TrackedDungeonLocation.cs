using HexResourceTracker.Core.PinManagers;
using System.Collections.Generic;
using UnityEngine;

namespace HexResourceTracker.Core.Tracking
{
    internal sealed class TrackedDungeonLocation : MonoBehaviour
    {
        private static readonly HashSet<TrackedDungeonLocation> TrackedLocations = new HashSet<TrackedDungeonLocation>();

        internal Location Location { get; private set; }

        internal static IReadOnlyCollection<TrackedDungeonLocation> GetTrackedLocations()
        {
            return TrackedLocations;
        }

        internal void Initialize(Location location)
        {
            Location = location;
            TrackedLocations.Add(this);
        }

        private void OnDestroy()
        {
            TrackedLocations.Remove(this);
        }

        internal static void TryAdd(Location location)
        {
            if (location == null)
            {
                return;
            }

            if (!DungeonPinManager.TryGetSupportedDungeonTheme(location, out _))
            {
                return;
            }

            if (location.TryGetComponent(out TrackedDungeonLocation trackedLocation))
            {
                return;
            }

            trackedLocation = location.gameObject.AddComponent<TrackedDungeonLocation>();
            trackedLocation.Initialize(location);
        }
    }
}