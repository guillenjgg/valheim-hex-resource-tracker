using System.Collections.Generic;
using UnityEngine;
using HexResourceTracker.Models;

namespace HexResourceTracker.Core
{
    internal static class PickableResourcePinService
    {
        private const float ClusterRadius = 25f;

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

            if (!IsTrackedPickablePrefab(pickablePrefabName))
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
                pickable.transform.position,
                ClusterRadius));
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

        private static bool IsTrackedPickablePrefab(string pickablePrefabName)
        {
            return PluginConfig.IsResourceTrackingEnabled(pickablePrefabName);
        }
    }
}
