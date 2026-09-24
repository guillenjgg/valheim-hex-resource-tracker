using HexResourceTracker.Core.PinManagers;
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

            var nview = pickable.GetComponent<ZNetView>();

            if (nview == null || !nview.IsValid())
            {
                return false;
            }

            if (!pickable.CanBePicked())
            {
                return false;
            }

            string prefabName = pickable.gameObject.name.Replace("(Clone)", string.Empty).Trim();

            if (!TrackedResources.TryGetPrefabName(prefabName, out TrackedResourceDefinition definition) || definition.ResourceType != TrackedResourceTypeEnum.Pickable)
            {
                return false;
            }

            if (!PluginConfig.IsResourceTrackingEnabled(definition.ResourcePrefabName))
            {
                return false;
            }

            var zdo = nview.GetZDO();

            if (zdo == null)
            {
                return false;
            }

            return ResourcePinManager.TryAddResourcePin(new ResourcePinModel(
                zdo.m_uid,
                definition,
                pickable.transform.position));
        }

        internal static void HandleResourceTrackingChanged(string prefabName, bool isEnabled)
        {
            if (string.IsNullOrWhiteSpace(prefabName))
            {
                return;
            }

            if (!TrackedResources.TryGetPrefabName(prefabName, out TrackedResourceDefinition definition) || definition.ResourceType != TrackedResourceTypeEnum.Pickable)
            {
                return;
            }

            if (!isEnabled)
            {
                ResourcePinManager.RemoveResourcePins(definition.ResourcePrefabName);
                return;
            }

            if (Minimap.instance == null)
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

                string loadedPrefabName = pickable.gameObject.name.Replace("(Clone)", string.Empty).Trim();

                if (loadedPrefabName != definition.ResourcePrefabName)
                {
                    continue;
                }

                TryAddResourcePinFromPickable(pickable);
            }
        }

        internal static void ReconcileTrackedPickable(TrackedMapObject trackedObject)
        {
            if (trackedObject == null || trackedObject.Pickable == null)
            {
                return;
            }

            var nview = trackedObject.ZNetView;

            if (nview == null || !nview.IsValid())
            {
                return;
            }

            var zdo = nview.GetZDO();

            if (zdo == null)
            {
                return;
            }

            if (!trackedObject.IsInTrackingRange)
            {
                ResourcePinManager.RemoveResourcePin(zdo.m_uid);
                return;
            }

            TryAddResourcePinFromPickable(trackedObject.Pickable);
        }
    }
}