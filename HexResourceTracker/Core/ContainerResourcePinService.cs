using HexResourceTracker.Core.PinManagers;
using HexResourceTracker.Core.Tracking;
using HexResourceTracker.Models;

namespace HexResourceTracker.Core
{
    internal static class ContainerResourcePinService
    {
        internal static void ReconcileTrackedContainer(TrackedMapObject trackedObject)
        {
            if (trackedObject == null || trackedObject.Container == null || trackedObject.ZNetView == null)
            {
                return;
            }

            var zdo = trackedObject.ZNetView.GetZDO();

            if (zdo == null)
            {
                return;
            }

            if (!TryGetTrackedContainerResource(trackedObject.Container, out TrackedResourceDefinition definition) ||
                !PluginConfig.IsResourceTrackingEnabled(definition.ResourcePrefabName))
            {
                ResourcePinManager.RemoveResourcePin(zdo.m_uid);
                return;
            }

            var model = new ResourcePinModel(
                zdo.m_uid,
                definition,
                trackedObject.transform.position);

            ResourcePinManager.TryAddResourcePin(model);
        }

        internal static void HandleResourceTrackingChanged(string prefabName, bool isEnabled)
        {
            if (!TrackedResources.TryGetPrefabName(prefabName, out TrackedResourceDefinition definition) ||
                definition.ResourceType != TrackedResourceTypeEnum.Container)
            {
                return;
            }

            if (!isEnabled)
            {
                ResourcePinManager.RemoveResourcePins(definition.ResourcePrefabName);
                return;
            }

            foreach (TrackedMapObject trackedObject in TrackedMapObject.GetTrackedObjects())
            {
                if (trackedObject == null || trackedObject.Container == null)
                {
                    continue;
                }

                ReconcileTrackedContainer(trackedObject);
            }
        }

        private static bool TryGetTrackedContainerResource(Container container, out TrackedResourceDefinition definition)
        {
            var inventory = container.GetInventory();

            if (inventory == null)
            {
                definition = null;
                return false;
            }

            foreach (ItemDrop.ItemData item in inventory.GetAllItems())
            {
                if (item.m_dropPrefab == null)
                {
                    continue;
                }

                if (!TrackedResources.TryGetPrefabName(item.m_dropPrefab.name, out definition))
                {
                    continue;
                }

                if (definition.ResourceType == TrackedResourceTypeEnum.Container)
                {
                    return true;
                }
            }

            definition = null;
            return false;
        }
    }
}