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
            int objectsInRange = 0;
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

                if (isInRange)
                {
                    objectsInRange++;
                }

                PickableResourcePinService.ReconcileTrackedPickable(trackedObject);
            }

#if DEBUG
            Plugin.Log.LogInfo($"Map tracking scan at {currentPosition}. Range: {trackingRange:F1}m. Registered objects: {TrackedMapObject.GetTrackedObjects().Count}. In range: {objectsInRange}");
#endif

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

#if DEBUG
                    Plugin.Log.LogInfo("Added MapTrackingScanner to local player.");
#endif
                }

                scanner.ForceScan();
                return;
            }

            if (player.TryGetComponent(out MapTrackingScanner existingScanner))
            {
#if DEBUG
                Plugin.Log.LogInfo("Removing MapTrackingScanner because tracking mode changed to ZoneBased.");
#endif

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

#if DEBUG
            Plugin.Log.LogInfo($"Tracking range changed. Forcing RangeScanner rescan. Range: {PluginConfig.TrackingRange.Value:F1}m.");
#endif

            scanner.ForceScan();
        }

        private void ForceScan()
        {
#if DEBUG
            Plugin.Log.LogInfo($"Forcing RangeScanner reconciliation. Range: {PluginConfig.TrackingRange.Value:F1}m.");
#endif

            _hasScanned = false;
            Scan(transform.position);
        }

        private static void RescanZoneBasedObjects()
        {
            int reconciledObjects = 0;

            foreach (TrackedMapObject trackedObject in TrackedMapObject.GetTrackedObjects())
            {
                if (trackedObject == null)
                {
                    continue;
                }

                Pickable pickable = trackedObject.GetComponent<Pickable>();

                if (pickable == null)
                {
                    continue;
                }

                PickableResourcePinService.TryAddResourcePinFromPickable(pickable);
                reconciledObjects++;
            }

#if DEBUG
            Plugin.Log.LogInfo($"ZoneBased rescan complete. Reconciled {reconciledObjects} currently loaded tracked objects.");
#endif
        }
    }
}