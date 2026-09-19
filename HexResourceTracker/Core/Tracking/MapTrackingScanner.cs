using HexResourceTracker.Core.Tracking;
using UnityEngine;

namespace HexResourceTracker.Core
{
    internal sealed class MapTrackingScanner : MonoBehaviour
    {
        private const float RescanDistance = 25f;
        private const float RescanDistanceSquared = RescanDistance * RescanDistance;

        private Vector3 _lastScanPosition;
        private bool _hasScanned;

        private void Update()
        {
            if (!PluginConfig.IsModEnabled.Value)
            {
                return;
            }

            Vector3 currentPosition = transform.position;

            if (_hasScanned && !HasMovedEnough(currentPosition))
            {
                return;
            }

            Scan(currentPosition);
        }

        private bool HasMovedEnough(Vector3 currentPosition)
        {
            float deltaX = currentPosition.x - _lastScanPosition.x;
            float deltaZ = currentPosition.z - _lastScanPosition.z;

            return (deltaX * deltaX) + (deltaZ * deltaZ) >= RescanDistanceSquared;
        }

        private void Scan(Vector3 currentPosition)
        {
            float trackingRange = PluginConfig.TrackingRange.Value;
            float trackingRangeSquared = trackingRange * trackingRange;

            foreach (TrackedMapObject trackedObject in TrackedMapObject.GetTrackedObjects())
            {
                if (trackedObject == null)
                {
                    continue;
                }

                Vector3 objectPosition = trackedObject.transform.position;

                float deltaX = objectPosition.x - currentPosition.x;
                float deltaZ = objectPosition.z - currentPosition.z;

                bool isInRange = (deltaX * deltaX) + (deltaZ * deltaZ) <= trackingRangeSquared;

                trackedObject.IsInTrackingRange = isInRange;

                if (trackedObject.Container != null)
                {
                    ContainerResourcePinService.ReconcileTrackedContainer(trackedObject);
                }
                else if (trackedObject.Pickable != null)
                {
                    PickableResourcePinService.ReconcileTrackedPickable(trackedObject);
                }
                else
                {
                    OreResourcePinService.ReconcileTrackedOre(trackedObject);
                }
            }

            DungeonPinManager.ReconcileTrackingRange();

            _lastScanPosition = currentPosition;
            _hasScanned = true;
        }

        internal static void HandleTrackingModeChanged()
        {
            Player player = Player.m_localPlayer;

            if (player == null)
            {
                return;
            }

            if (PluginConfig.TrackingMode.Value == TrackingModeEnum.RangeScanner)
            {
                if (!player.TryGetComponent(out MapTrackingScanner scanner))
                {
                    scanner = player.gameObject.AddComponent<MapTrackingScanner>();
                }

                scanner.ForceScan();
                return;
            }

            if (player.TryGetComponent(out MapTrackingScanner existingScanner))
            {
                Destroy(existingScanner);
            }

            RescanZoneBasedObjects();
        }

        internal static void ForceRescan()
        {
            if (PluginConfig.TrackingMode.Value != TrackingModeEnum.RangeScanner)
            {
                return;
            }

            Player player = Player.m_localPlayer;

            if (player == null || !player.TryGetComponent(out MapTrackingScanner scanner))
            {
                return;
            }

            scanner.ForceScan();
        }

        private void ForceScan()
        {
            _hasScanned = false;
            Scan(transform.position);
        }

        private static void RescanZoneBasedObjects()
        {
            foreach (TrackedMapObject trackedObject in TrackedMapObject.GetTrackedObjects())
            {
                if (trackedObject == null)
                {
                    continue;
                }

                if (trackedObject.Container != null)
                {
                    ContainerResourcePinService.ReconcileTrackedContainer(trackedObject);
                    continue;
                }

                if (trackedObject.Pickable != null)
                {
                    PickableResourcePinService.TryAddResourcePinFromPickable(trackedObject.Pickable);
                    continue;
                }

                if (trackedObject.Destructible != null)
                {
                    OreResourcePinService.TryAddResourcePinFromDestructibleOre(trackedObject.Destructible);
                    continue;
                }

                if (trackedObject.MineRock5 != null)
                {
                    OreResourcePinService.TryAddOrRelinkResourcePinFromMineRock5Ore(trackedObject.MineRock5);
                    continue;
                }

                if (trackedObject.MineRock != null)
                {
                    OreResourcePinService.TryAddResourcePinFromMineRock(trackedObject.MineRock);
                }
            }

            DungeonPinManager.AddLoadedDungeonPins();
        }
    }
}