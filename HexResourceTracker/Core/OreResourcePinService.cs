using UnityEngine;
using HexResourceTracker.Models;

namespace HexResourceTracker.Core
{
    internal static class OreResourcePinService
    {
        private const float ClusterRadius = 25f;
        private const float MineRockRelinkRadius = 35f;

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
                destructible.transform.position,
                ClusterRadius));
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
                mineRock.transform.position,
                ClusterRadius));
        }

        internal static void HandleResourceTrackingChanged(string prefabName, bool isEnabled)
        {
            if (string.IsNullOrWhiteSpace(prefabName))
            {
                return;
            }

            if (!TrackedResourceDefinitions.DestructibleResourcesByPrefabName.ContainsKey(prefabName))
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

            Destructible[] destructibles = UnityEngine.Object.FindObjectsByType<Destructible>(FindObjectsSortMode.None);

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

            MineRock5[] mineRocks = UnityEngine.Object.FindObjectsByType<MineRock5>(FindObjectsSortMode.None);

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
    }
}
