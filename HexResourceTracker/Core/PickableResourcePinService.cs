using HexResourceTracker.Core.PinManagers;
using HexResourceTracker.Core.Tracking.Monoscripts;
using HexResourceTracker.Models;
using UnityEngine;

namespace HexResourceTracker.Core
{
    internal static class PickableResourcePinService
    {
        internal static bool TryGetResourceDefinition(Pickable pickable, out TrackedResourceDefinitionModel definition)
        {
            definition = null;

            if (pickable == null)
            {
                return false;
            }

            string prefabName = Utils.GetPrefabName(pickable.gameObject);

            if (TrackedResources.TryGetByPrefabName(prefabName, out definition))
            {
                return definition.ResourceType == TrackedResourceTypeEnum.Pickable;
            }

            if (pickable.m_itemPrefab == null)
            {
                return false;
            }

            string itemPrefabName = Utils.GetPrefabName(pickable.m_itemPrefab);

            if (!TrackedResources.TryGetByPrefabName(itemPrefabName, out definition))
            {
                return false;
            }

            return definition.ResourceType == TrackedResourceTypeEnum.Pickable;
        }

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

            if (!TryGetResourceDefinition(pickable, out TrackedResourceDefinitionModel definition))
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
                pickable.m_itemPrefab.name,
                pickable.transform.position));
        }

        internal static void HandleResourceTrackingChanged(string prefabName, bool isEnabled)
        {
            if (string.IsNullOrWhiteSpace(prefabName))
            {
                return;
            }

            if (!TrackedResources.TryGetByPrefabName(prefabName, out TrackedResourceDefinitionModel definition) ||
                definition.ResourceType != TrackedResourceTypeEnum.Pickable)
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

                if (!TryGetResourceDefinition(pickable, out TrackedResourceDefinitionModel loadedDefinition))
                {
                    continue;
                }

                if (loadedDefinition.ResourcePrefabName != definition.ResourcePrefabName)
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