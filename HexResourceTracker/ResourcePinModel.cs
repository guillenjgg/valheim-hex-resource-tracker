using UnityEngine;
using static Minimap;

namespace HexResourceTracker
{
    internal class ResourcePinModel
    {
        internal ZDOID ZdoId { get; }
        internal string PickablePrefabName { get; }
        internal string ItemPrefabName { get; }
        internal Vector3 Position { get; }
        internal float ClusterRadius { get; }
        internal PinData Pin { get; set; }

        public ResourcePinModel(ZDOID zdoid, string pickablePrefabName, string itemPrefabName, Vector3 position, float clusterRadius)
        {
            ZdoId = zdoid;
            PickablePrefabName = pickablePrefabName;
            ItemPrefabName = itemPrefabName;
            Position = position;
            ClusterRadius = clusterRadius;
        }
    }
}
