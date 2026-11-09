using System.Collections.Generic;
using UnityEngine;

namespace HexResourceTracker.Core.Tracking.Monoscripts
{
    internal sealed class TrackedLocation : MonoBehaviour
    {
        internal static readonly HashSet<TrackedLocation> Loaded = new HashSet<TrackedLocation>();
        internal string PrefabName { get; private set; }

        internal static bool TryAdd(Location location)
        {
            string prefabName = Utils.GetPrefabName(location.gameObject);
            if (!TrackedLocations.All.ContainsKey(prefabName) || location.TryGetComponent(out TrackedLocation _))
            {
                return false;
            }

            var tracked = location.gameObject.AddComponent<TrackedLocation>();
            tracked.PrefabName = prefabName;
            Loaded.Add(tracked);
            return true;
        }

        private void OnDestroy()
        {
            Loaded.Remove(this);
        }
    }
}
