using HexResourceTracker.Core.Tracking;
using HexResourceTracker.Core.Tracking.Monoscripts;
using UnityEngine;

namespace HexResourceTracker.Core
{
    // Yggdrasil roots carry a ResourceRoot component (the sap a Sap Extractor draws). They are found by that
    // component, so they share one key instead of a prefab name.
    internal static class YggdrasilRootPinService
    {
        internal const string ResourcePrefabName = "YggdrasilRoot";

        internal static void TrackRoot(ResourceRoot root)
        {
            if (root == null)
            {
                return;
            }

            if (PluginConfig.LogDepositChanges.Value)
            {
                string prefabName = root.gameObject.name.Replace("(Clone)", string.Empty).Trim();
                Debug.Log($"[HexResourceTracker] [DepositLog] Yggdrasil root found: '{prefabName}' (ResourceRoot, token '{root.m_name}') at ({root.transform.position.x:0}, {root.transform.position.z:0})");
            }

            TrackedMapObject.TryAdd(root.gameObject, ResourcePrefabName);

            if (PluginConfig.TrackingMode.Value == TrackingModeEnum.RangeScanner)
            {
                return;
            }

            OreResourcePinService.TryAddResourcePin(root, ResourcePrefabName);
        }
    }
}
