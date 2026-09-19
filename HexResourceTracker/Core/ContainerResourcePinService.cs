using HexResourceTracker.Core.Tracking;
using HexResourceTracker.Models;

namespace HexResourceTracker.Core
{
    internal static class ContainerResourcePinService
    {
        private const string OnionSeedsPrefabName = "OnionSeeds";

        internal static void ReconcileTrackedContainer(TrackedMapObject trackedObject)
        {
            if (trackedObject == null || trackedObject.Container == null || trackedObject.ZNetView == null)
            {
                return;
            }

            ZDO zdo = trackedObject.ZNetView.GetZDO();

            if (zdo == null)
            {
                return;
            }

            if (!PluginConfig.IsResourceTrackingEnabled(OnionSeedsPrefabName) ||
                !ContainsOnionSeeds(trackedObject.Container))
            {
                ResourcePinManager.RemoveResourcePin(zdo.m_uid);
                return;
            }

            var model = new ResourcePinModel(
                zdo.m_uid,
                OnionSeedsPrefabName,
                trackedObject.transform.position);

            ResourcePinManager.TryAddResourcePin(model);
        }

        internal static void HandleResourceTrackingChanged(string prefabName, bool enabled)
        {
            if (prefabName != OnionSeedsPrefabName)
            {
                return;
            }

            if (!enabled)
            {
                ResourcePinManager.RemoveResourcePins(OnionSeedsPrefabName);
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

        private static bool ContainsOnionSeeds(Container container)
        {
            Inventory inventory = container.GetInventory();

            if (inventory == null)
            {
                return false;
            }

            foreach (ItemDrop.ItemData item in inventory.GetAllItems())
            {
                if (item.m_dropPrefab != null && item.m_dropPrefab.name == OnionSeedsPrefabName)
                {
                    return true;
                }
            }

            return false;
        }
    }
}