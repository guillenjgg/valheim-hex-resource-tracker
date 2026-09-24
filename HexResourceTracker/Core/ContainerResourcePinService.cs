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

            if (!TryGetTrackedContainerResource(trackedObject.Container, out TrackedResourceDefinition definition, out string iconItemPrefabName) ||
                !PluginConfig.IsResourceTrackingEnabled(definition.ResourcePrefabName))
            {
                ResourcePinManager.RemoveResourcePin(zdo.m_uid);
                return;
            }

            var model = new ResourcePinModel(
                zdo.m_uid,
                definition,
                iconItemPrefabName,
                trackedObject.transform.position);

            ResourcePinManager.TryAddResourcePin(model);
        }

        internal static void HandleResourceTrackingChanged(string prefabName, bool isEnabled)
        {
            if (!TrackedResources.TryGetByPrefabName(prefabName, out TrackedResourceDefinition definition) ||
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

        private static bool TryGetTrackedContainerResource(Container container, out TrackedResourceDefinition definition, out string iconItemPrefabName)
        {
            var inventory = container.GetInventory();

            if (inventory == null)
            {
                definition = null;
                iconItemPrefabName = null;
                return false;
            }

            foreach (ItemDrop.ItemData item in inventory.GetAllItems())
            {
                if (item.m_dropPrefab == null)
                {
                    continue;
                }

                string itemPrefabName = item.m_dropPrefab.name;

                if (!TrackedResources.TryGetByPrefabName(itemPrefabName, out definition) ||
                    definition.ResourceType != TrackedResourceTypeEnum.Container)
                {
                    continue;
                }

                iconItemPrefabName = itemPrefabName;
                return true;
            }

            definition = null;
            iconItemPrefabName = null;
            return false;
        }
    }
}