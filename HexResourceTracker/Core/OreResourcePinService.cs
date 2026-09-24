using HarmonyLib;
using HexResourceTracker.Core.PinManagers;
using HexResourceTracker.Core.Tracking;
using HexResourceTracker.Models;
using System.Reflection;
using UnityEngine;

namespace HexResourceTracker.Core
{
    internal static class OreResourcePinService
    {
        private const float ResourcePinRadius = 35f;

        private static readonly MethodInfo MineRockAllDestroyedMethod = AccessTools.Method(typeof(MineRock), "AllDestroyed");

        internal static bool TryAddResourcePinFromDestructibleOre(Destructible destructible)
        {
            if (destructible == null || Minimap.instance == null)
            {
                return false;
            }

            string prefabName = destructible.gameObject.name.Replace("(Clone)", string.Empty).Trim();

            if (!TrackedResources.TryGetPrefabName(prefabName, out TrackedResourceDefinition definition) ||
                definition.ResourceType != TrackedResourceTypeEnum.Deposit)
            {
                return false;
            }

            if (!PluginConfig.IsResourceTrackingEnabled(definition.ResourcePrefabName))
            {
                return false;
            }

            ZNetView nview = destructible.GetComponent<ZNetView>();

            if (nview == null || !nview.IsValid())
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
                definition,
                destructible.transform.position));
        }

        internal static bool TryAddOrRelinkResourcePinFromMineRock5Ore(MineRock5 mineRock)
        {
            if (mineRock == null || Minimap.instance == null)
            {
                return false;
            }

            if (!TrackedResources.TryGetMineRock5(mineRock.m_name, out TrackedResourceDefinition definition) ||
                definition.ResourceType != TrackedResourceTypeEnum.Deposit)
            {
                return false;
            }

            if (!PluginConfig.IsResourceTrackingEnabled(definition.ResourcePrefabName))
            {
                return false;
            }

            ZNetView nview = mineRock.GetComponent<ZNetView>();

            if (nview == null || !nview.IsValid())
            {
                return false;
            }

            ZDO zdo = nview.GetZDO();

            if (zdo == null)
            {
                return false;
            }

            if (ResourcePinManager.HasResourcePin(zdo.m_uid))
            {
                return true;
            }

            ResourcePinManager.RemoveClosestResourcePin(
                definition.ResourcePrefabName,
                mineRock.transform.position,
                ResourcePinRadius);

            return ResourcePinManager.TryAddResourcePin(new ResourcePinModel(
                zdo.m_uid,
                definition,
                mineRock.transform.position));
        }

        internal static bool TryAddResourcePinFromMineRock(MineRock mineRock)
        {
            if (mineRock == null || Minimap.instance == null)
            {
                return false;
            }

            if (!TrackedResources.TryGetMineRock(mineRock.m_name, out TrackedResourceDefinition definition) ||
                definition.ResourceType != TrackedResourceTypeEnum.Deposit)
            {
                return false;
            }

            if (!PluginConfig.IsResourceTrackingEnabled(definition.ResourcePrefabName))
            {
                return false;
            }

            ZNetView nview = mineRock.GetComponent<ZNetView>();

            if (nview == null || !nview.IsValid())
            {
                return false;
            }

            ZDO zdo = nview.GetZDO();

            if (zdo == null)
            {
                return false;
            }

            if (IsMineRockFullyDestroyed(mineRock))
            {
                ResourcePinManager.RemoveResourcePin(zdo.m_uid);
                return false;
            }

            return ResourcePinManager.TryAddResourcePin(new ResourcePinModel(
                zdo.m_uid,
                definition,
                mineRock.transform.position));
        }

        internal static bool TryRemoveResourcePinFromDestructibleOre(Destructible destructible)
        {
            if (destructible == null)
            {
                return false;
            }

            string prefabName = destructible.gameObject.name.Replace("(Clone)", string.Empty).Trim();

            if (!TrackedResources.TryGetPrefabName(prefabName, out TrackedResourceDefinition definition) ||
                definition.ResourceType != TrackedResourceTypeEnum.Deposit)
            {
                return false;
            }

            ZNetView nview = destructible.GetComponent<ZNetView>();

            if (nview == null || !nview.IsValid())
            {
                return false;
            }

            ZDO zdo = nview.GetZDO();

            if (zdo == null)
            {
                return false;
            }

            return ResourcePinManager.RemoveResourcePin(zdo.m_uid);
        }

        internal static bool TryRemoveResourcePinFromMineRock(MineRock mineRock)
        {
            if (mineRock == null)
            {
                return false;
            }

            if (!TrackedResources.TryGetMineRock(mineRock.m_name, out TrackedResourceDefinition definition) ||
                definition.ResourceType != TrackedResourceTypeEnum.Deposit)
            {
                return false;
            }

            ZNetView nview = mineRock.GetComponent<ZNetView>();

            if (nview == null || !nview.IsValid())
            {
                return false;
            }

            ZDO zdo = nview.GetZDO();

            if (zdo == null)
            {
                return false;
            }

            return ResourcePinManager.RemoveResourcePin(zdo.m_uid);
        }

        internal static void HandleResourceTrackingChanged(string prefabName, bool isEnabled)
        {
            if (string.IsNullOrWhiteSpace(prefabName))
            {
                return;
            }

            if (!TrackedResources.TryGetPrefabName(prefabName, out TrackedResourceDefinition definition) ||
                definition.ResourceType != TrackedResourceTypeEnum.Deposit)
            {
                return;
            }

            if (!isEnabled)
            {
                ResourcePinManager.RemoveResourcePins(prefabName);
                return;
            }

            if (Minimap.instance == null)
            {
                return;
            }

            RescanLoadedDestructibles(prefabName);
            RescanLoadedMineRock5(prefabName);
            RescanLoadedMineRock(prefabName);
        }

        internal static void ReconcileTrackedOre(TrackedMapObject trackedObject)
        {
            if (trackedObject == null)
            {
                return;
            }

            ZNetView nview = trackedObject.ZNetView;

            if (nview == null || !nview.IsValid())
            {
                return;
            }

            ZDO zdo = nview.GetZDO();

            if (zdo == null)
            {
                return;
            }

            if (trackedObject.Destructible != null)
            {
                if (!trackedObject.IsInTrackingRange)
                {
                    ResourcePinManager.RemoveResourcePin(zdo.m_uid);
                    return;
                }

                TryAddResourcePinFromDestructibleOre(trackedObject.Destructible);
                return;
            }

            if (trackedObject.MineRock5 != null)
            {
                if (!trackedObject.IsInTrackingRange)
                {
                    ResourcePinManager.RemoveResourcePin(zdo.m_uid);
                    return;
                }

                TryAddOrRelinkResourcePinFromMineRock5Ore(trackedObject.MineRock5);
                return;
            }

            if (trackedObject.MineRock != null)
            {
                if (!trackedObject.IsInTrackingRange)
                {
                    ResourcePinManager.RemoveResourcePin(zdo.m_uid);
                    return;
                }

                TryAddResourcePinFromMineRock(trackedObject.MineRock);
            }
        }

        private static void RescanLoadedDestructibles(string prefabName)
        {
            Destructible[] destructibles = Object.FindObjectsByType<Destructible>(FindObjectsSortMode.None);

            foreach (Destructible destructible in destructibles)
            {
                if (destructible == null)
                {
                    continue;
                }

                string destructiblePrefabName = destructible.gameObject.name.Replace("(Clone)", string.Empty).Trim();

                if (destructiblePrefabName != prefabName)
                {
                    continue;
                }

                TryAddResourcePinFromDestructibleOre(destructible);
            }
        }

        private static void RescanLoadedMineRock5(string prefabName)
        {
            MineRock5[] mineRocks = Object.FindObjectsByType<MineRock5>(FindObjectsSortMode.None);

            foreach (MineRock5 mineRock in mineRocks)
            {
                if (mineRock == null)
                {
                    continue;
                }

                if (!TrackedResources.TryGetMineRock5(mineRock.m_name, out TrackedResourceDefinition definition))
                {
                    continue;
                }

                if (definition.ResourcePrefabName != prefabName)
                {
                    continue;
                }

                TryAddOrRelinkResourcePinFromMineRock5Ore(mineRock);
            }
        }

        private static void RescanLoadedMineRock(string prefabName)
        {
            MineRock[] mineRocks = Object.FindObjectsByType<MineRock>(FindObjectsSortMode.None);

            foreach (MineRock mineRock in mineRocks)
            {
                if (mineRock == null)
                {
                    continue;
                }

                if (!TrackedResources.TryGetMineRock(mineRock.m_name, out TrackedResourceDefinition definition))
                {
                    continue;
                }

                if (definition.ResourcePrefabName != prefabName)
                {
                    continue;
                }

                TryAddResourcePinFromMineRock(mineRock);
            }
        }

        private static bool IsMineRockFullyDestroyed(MineRock mineRock)
        {
            if (mineRock == null || MineRockAllDestroyedMethod == null)
            {
                return false;
            }

            return MineRockAllDestroyedMethod.Invoke(mineRock, null) is bool isDestroyed && isDestroyed;
        }
    }
}