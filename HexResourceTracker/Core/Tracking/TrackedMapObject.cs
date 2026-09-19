using System.Collections.Generic;
using UnityEngine;

namespace HexResourceTracker.Core.Tracking
{
    internal sealed class TrackedMapObject : MonoBehaviour
    {
        private static readonly HashSet<TrackedMapObject> TrackedObjects = new HashSet<TrackedMapObject>();

        internal string PrefabName { get; private set; }
        internal bool IsInTrackingRange { get; set; }

        internal Pickable Pickable { get; private set; }
        internal Destructible Destructible { get; private set; }
        internal MineRock5 MineRock5 { get; private set; }
        internal MineRock MineRock { get; private set; }
        internal ZNetView ZNetView { get; private set; }

        internal static IReadOnlyCollection<TrackedMapObject> GetTrackedObjects()
        {
            return TrackedObjects;
        }

        internal void Initialize(string prefabName)
        {
            PrefabName = prefabName;

            TryGetComponent(out Pickable pickable);
            TryGetComponent(out Destructible destructible);
            TryGetComponent(out MineRock5 mineRock5);
            TryGetComponent(out MineRock mineRock);
            TryGetComponent(out ZNetView zNetView);

            Pickable = pickable;
            Destructible = destructible;
            MineRock5 = mineRock5;
            MineRock = mineRock;
            ZNetView = zNetView;

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