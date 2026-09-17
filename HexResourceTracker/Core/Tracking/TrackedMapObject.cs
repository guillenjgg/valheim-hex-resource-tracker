using System.Collections.Generic;
using UnityEngine;

namespace HexResourceTracker.Core.Tracking
{
    internal sealed class TrackedMapObject : MonoBehaviour
    {
        private static readonly HashSet<TrackedMapObject> TrackedObjects = new HashSet<TrackedMapObject>();

        internal string PrefabName { get; private set; }
        internal bool IsInTrackingRange { get; set; }

        internal static IReadOnlyCollection<TrackedMapObject> GetTrackedObjects()
        {
            return TrackedObjects;
        }

        internal void Initialize(string prefabName)
        {
            PrefabName = prefabName;
            TrackedObjects.Add(this);
        }

        private void OnDestroy()
        {
            TrackedObjects.Remove(this);
        }

        internal static void TryAdd(GameObject gameObject, string prefabName)
        {
            if (gameObject == null || string.IsNullOrWhiteSpace(prefabName))
            {
                return;
            }

            if (gameObject.TryGetComponent(out TrackedMapObject trackedMapObject))
            {
                return;
            }

            trackedMapObject = gameObject.AddComponent<TrackedMapObject>();
            trackedMapObject.Initialize(prefabName);
        }
    }
}