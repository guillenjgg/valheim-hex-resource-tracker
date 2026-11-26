using HexResourceTracker.Core.PinManagers;
using HexResourceTracker.Core.Tracking.Monoscripts;
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

            bool found = TryGetTrackedContainerResource(trackedObject.Container, out TrackedResourceDefinitionModel definition, out string iconItemPrefabName);

            if (PluginConfig.LogDepositChanges.Value)
            {
                var inventory = trackedObject.Container.GetInventory();
                var items = new System.Collections.Generic.List<string>();
                if (inventory != null)
                {
                    foreach (ItemDrop.ItemData item in inventory.GetAllItems())
                    {
                        items.Add((item.m_dropPrefab != null ? item.m_dropPrefab.name : "<no prefab>") + " " + item.m_shared?.m_name);
                    }
                }

                UnityEngine.Debug.Log($"[HexResourceTracker] [DepositLog] container '{trackedObject.PrefabName}' at ({trackedObject.transform.position.x:0}, {trackedObject.transform.position.z:0}): {items.Count} item(s) [{string.Join(", ", items)}], tracked {found}");
            }

            if (!found || !PluginConfig.IsResourceTrackingEnabled(definition.ResourcePrefabName))
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
            if (!TrackedResources.TryGetByPrefabName(prefabName, out TrackedResourceDefinitionModel definition) ||
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

        private static bool TryGetTrackedContainerResource(Container container, out TrackedResourceDefinitionModel definition, out string iconItemPrefabName)
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
                string resourceKey = TrackedResources.ContainerKeyForItem(itemPrefabName, item.m_shared?.m_name);

                if (resourceKey == null ||
                    !TrackedResources.TryGetByPrefabName(resourceKey, out definition) ||
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