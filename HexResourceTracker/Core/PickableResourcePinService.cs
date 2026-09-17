using HexResourceTracker.Core.Tracking;
using HexResourceTracker.Models;
using UnityEngine;

namespace HexResourceTracker.Core
{
    internal static class PickableResourcePinService
    {
        internal static bool TryAddResourcePinFromPickable(Pickable pickable)
        {
            if (pickable == null || Minimap.instance == null || pickable.m_itemPrefab == null)
            {
                return false;
            }

            ZNetView nview = pickable.GetComponent<ZNetView>();

            if (nview == null || !nview.IsValid())
            {
                return false;
            }

            if (!pickable.CanBePicked())
            {
                return false;
            }

            string pickablePrefabName = pickable.gameObject.name.Replace("(Clone)", string.Empty).Trim();

            if (!PluginConfig.IsResourceTrackingEnabled(pickablePrefabName))
            {
                return false;
            }

            ZDO zdo = nview.GetZDO();

            if (zdo == null)
            {
                return false;
            }

            return ResourcePinManager.TryAddResourcePin(new ResourcePinModel(
                zdo.m_uid,
                pickablePrefabName,
                pickable.m_itemPrefab.name,
                pickable.transform.position));
        }

        internal static void HandleResourceTrackingChanged(string pickablePrefabName, bool isEnabled)
        {
            if (string.IsNullOrWhiteSpace(pickablePrefabName))
            {
                return;
            }

            if (!isEnabled)
            {
                ResourcePinManager.RemoveResourcePins(pickablePrefabName);
                return;
            }

            if (!IsTrackedPickablePrefab(pickablePrefabName) || Minimap.instance == null)
            {
                return;
            }

            Pickable[] pickables = Object.FindObjectsByType<Pickable>(FindObjectsSortMode.None);

            foreach (Pickable pickable in pickables)
            {
                if (pickable == null)
                {
                    continue;
                }

                string prefabName = pickable.gameObject.name.Replace("(Clone)", string.Empty).Trim();

                if (prefabName != pickablePrefabName)
                {
                    continue;
                }

                TryAddResourcePinFromPickable(pickable);
            }
        }

        internal static void ReconcileTrackedPickable(TrackedMapObject trackedObject)
        {
            if (trackedObject == null)
            {
                return;
            }

            Pickable pickable = trackedObject.GetComponent<Pickable>();

            if (pickable == null)
            {
                return;
            }

            ZNetView nview = pickable.GetComponent<ZNetView>();

            if (nview == null || !nview.IsValid())
            {
                return;
            }

            ZDO zdo = nview.GetZDO();

            if (zdo == null)
            {
                return;
            }

            if (!trackedObject.IsInTrackingRange)
            {
                ResourcePinManager.RemoveResourcePin(zdo.m_uid);
                return;
            }

            TryAddResourcePinFromPickable(pickable);
        }

        private static bool IsTrackedPickablePrefab(string pickablePrefabName)
        {
            return PluginConfig.IsResourceTrackingEnabled(pickablePrefabName);
        }
    }
}
