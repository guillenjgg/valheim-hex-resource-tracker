using HarmonyLib;
using HexResourceTracker.Models;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace HexResourceTracker.Core
{
    internal static class OreResourcePinService
    {
        private const float MineRockRelinkRadius = 35f;

        private static readonly MethodInfo MineRockAllDestroyedMethod = AccessTools.Method(typeof(MineRock), "AllDestroyed");

        internal static bool TryAddResourcePinFromDestructibleOre(Destructible destructible)
        {
            if (destructible == null || Minimap.instance == null)
            {
                return false;
            }

            string prefabName = destructible.gameObject.name.Replace("(Clone)", string.Empty).Trim();

            if (!TrackedResourceDefinitions.DestructibleResourcesByPrefabName.TryGetValue(prefabName, out ResourceDefinitionModel definition))
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
                definition.ResourcePrefabName,
                definition.ItemPrefabName,
                destructible.transform.position));
        }

        internal static bool TryAddOrRelinkResourcePinFromMineRock5Ore(MineRock5 mineRock)
        {
            if (mineRock == null || Minimap.instance == null)
            {
                return false;
            }

            if (!TrackedResourceDefinitions.MineRock5ResourcesByName.TryGetValue(mineRock.m_name, out ResourceDefinitionModel definition))
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
                MineRockRelinkRadius);

            return ResourcePinManager.TryAddResourcePin(new ResourcePinModel(
                zdo.m_uid,
                definition.ResourcePrefabName,
                definition.ItemPrefabName,
                mineRock.transform.position));
        }

        internal static bool TryAddResourcePinFromMineRock(MineRock mineRock)
        {
            if (mineRock == null || Minimap.instance == null)
            {
                return false;
            }

            if (!TrackedResourceDefinitions.MineRockResourcesByName.TryGetValue(mineRock.m_name, out ResourceDefinitionModel definition))
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
                definition.ResourcePrefabName,
                definition.ItemPrefabName,
                mineRock.transform.position));
        }

        internal static bool TryRemoveResourcePinFromDestructibleOre(Destructible destructible)
        {
            if (destructible == null)
            {
                return false;
            }

            string prefabName = destructible.gameObject.name.Replace("(Clone)", string.Empty).Trim();

            if (!TrackedResourceDefinitions.DestructibleResourcesByPrefabName.ContainsKey(prefabName))
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

            if (!TrackedResourceDefinitions.MineRockResourcesByName.ContainsKey(mineRock.m_name))
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

            bool isTrackedOre =
                TrackedResourceDefinitions.DestructibleResourcesByPrefabName.ContainsKey(prefabName) ||
                TrackedResourceDefinitions.MineRockResourcesByName.Values.Any(definition => definition.ResourcePrefabName == prefabName) ||
                TrackedResourceDefinitions.MineRock5ResourcesByName.Values.Any(definition => definition.ResourcePrefabName == prefabName);

            if (!isTrackedOre)
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

                if (!TrackedResourceDefinitions.MineRock5ResourcesByName.TryGetValue(mineRock.m_name, out ResourceDefinitionModel definition))
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

                if (!TrackedResourceDefinitions.MineRockResourcesByName.TryGetValue(mineRock.m_name, out ResourceDefinitionModel definition))
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
