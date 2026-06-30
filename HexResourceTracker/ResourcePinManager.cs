using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using static Minimap;

namespace HexResourceTracker
{
    internal static class ResourcePinManager
    {
        private const float ResourcePinSize = 20f;
        private const float ClusterRadius = 25f;

        private static readonly Dictionary<string, Sprite> ResourceSprites = new Dictionary<string, Sprite>();

        private static readonly Dictionary<ZDOID, ResourcePinModel> ResourcePinByZdoId = new Dictionary<ZDOID, ResourcePinModel>();

        private static readonly FieldInfo MPinUpdateRequired = AccessTools.Field(typeof(Minimap), "m_pinUpdateRequired");

        private static readonly Dictionary<string, float> ResourceIconSizeOverrides = new Dictionary<string, float>
        {
            { "TurnipSeeds", 32f },
            { "CarrotSeeds", 32f },
            { "Dandelion", 32f },
            { "CopperOre", 32f },
            { "SilverOre", 32f },
            { "Softtissue", 32f }
        };

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

            return TryAddResourcePin(new ResourcePinModel(
                zdo.m_uid,
                pickablePrefabName,
                pickable.m_itemPrefab.name,
                pickable.transform.position,
                ClusterRadius));
        }

        internal static bool TryAddResourcePin(ResourcePinModel model)
        {
            if (model == null || Minimap.instance == null || model.ZdoId == ZDOID.None)
            {
                return false;
            }

            if (ResourcePinByZdoId.ContainsKey(model.ZdoId))
            {
                return false;
            }

            if (HasNearbyResourcePin(model))
            {
                return false;
            }

            PinData pin = Minimap.instance.AddPin(
                model.Position,
                PinType.None,
                string.Empty,
                false,
                false);

            Sprite sprite = GetSprite(model.ItemPrefabName);

            if (sprite != null)
            {
                pin.m_icon = sprite;
            }

            model.Pin = pin;
            ResourcePinByZdoId[model.ZdoId] = model;

            SetPinUpdateRequired();

            return true;
        }

        internal static void HandleResourceTrackingChanged(string pickablePrefabName, bool isEnabled)
        {
            if (string.IsNullOrWhiteSpace(pickablePrefabName))
            {
                return;
            }

            if (!isEnabled)
            {
                var zdoIdsToRemove = new List<ZDOID>();

                foreach (KeyValuePair<ZDOID, ResourcePinModel> entry in ResourcePinByZdoId)
                {
                    if (entry.Value.PickablePrefabName == pickablePrefabName)
                    {
                        zdoIdsToRemove.Add(entry.Key);
                    }
                }

                foreach (ZDOID zdoId in zdoIdsToRemove)
                {
                    RemoveResourcePin(zdoId);
                }

                return;
            }

            if (Minimap.instance == null)
            {
                return;
            }

            if (pickablePrefabName == "rock4_copper" || pickablePrefabName == "silvervein")
            {
                Destructible[] destructibles = Object.FindObjectsByType<Destructible>(FindObjectsSortMode.None);

                foreach (Destructible destructible in destructibles)
                {
                    if (destructible == null)
                    {
                        continue;
                    }

                    string prefabName = destructible.gameObject.name.Replace("(Clone)", string.Empty).Trim();

                    if (prefabName != pickablePrefabName)
                    {
                        continue;
                    }

                    if (prefabName == "rock4_copper")
                    {
                        TryAddResourcePinFromDestructibleOre(destructible, "rock4_copper", "CopperOre");
                    }
                    else if (prefabName == "silvervein")
                    {
                        TryAddResourcePinFromDestructibleOre(destructible, "silvervein", "SilverOre");
                    }
                }

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

        internal static void UpdateResourcePinVisuals(Minimap minimap)
        {
            if (minimap == null)
            {
                return;
            }

            foreach (ResourcePinModel model in ResourcePinByZdoId.Values)
            {
                PinData pin = model.Pin;

                if (pin == null || pin.m_uiElement == null)
                {
                    model.LastSizedUiElement = null;
                    continue;
                }

                float size = GetResourcePinSize(model.ItemPrefabName);

                if (model.LastSizedUiElement == pin.m_uiElement &&
                    model.LastAppliedSize == size)
                {
                    continue;
                }

                pin.m_uiElement.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
                pin.m_uiElement.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);

                model.LastSizedUiElement = pin.m_uiElement;
                model.LastAppliedSize = size;
            }
        }

        internal static bool RemoveResourcePin(ZDOID zdoId)
        {
            if (zdoId == ZDOID.None)
            {
                return false;
            }

            if (!ResourcePinByZdoId.TryGetValue(zdoId, out ResourcePinModel model))
            {
                return false;
            }

            if (model.Pin != null && Minimap.instance != null)
            {
                Minimap.instance.RemovePin(model.Pin);
            }

            ResourcePinByZdoId.Remove(zdoId);
            SetPinUpdateRequired();

            return true;
        }

        internal static bool RemoveClosestResourcePin(Vector3 position, float radius)
        {
            ResourcePinModel closestModel = null;
            float closestDistanceSqr = radius * radius;

            foreach (ResourcePinModel model in ResourcePinByZdoId.Values)
            {
                float deltaX = model.Position.x - position.x;
                float deltaZ = model.Position.z - position.z;
                float distanceSqr = (deltaX * deltaX) + (deltaZ * deltaZ);

                if (distanceSqr <= closestDistanceSqr)
                {
                    closestDistanceSqr = distanceSqr;
                    closestModel = model;
                }
            }

            if (closestModel == null)
            {
                return false;
            }

            return RemoveResourcePin(closestModel.ZdoId);
        }

        internal static bool TryAddResourcePinFromDestructibleOre(Destructible destructible, string orePrefabName, string itemPrefabName)
        {
            if (destructible == null || Minimap.instance == null || string.IsNullOrWhiteSpace(orePrefabName) || string.IsNullOrWhiteSpace(itemPrefabName))
            {
                return false;
            }

            if (!PluginConfig.IsResourceTrackingEnabled(orePrefabName))
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

            return TryAddResourcePin(new ResourcePinModel(
                zdo.m_uid,
                orePrefabName,
                itemPrefabName,
                destructible.transform.position,
                ClusterRadius));
        }

        internal static bool TryAddOrRelinkResourcePinFromMineRock5Ore(MineRock5 mineRock, string orePrefabName, string itemPrefabName)
        {
            if (mineRock == null || Minimap.instance == null)
            {
                return false;
            }

            if (!PluginConfig.IsResourceTrackingEnabled(orePrefabName))
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

            if (ResourcePinByZdoId.ContainsKey(zdo.m_uid))
            {
                return true;
            }

            RemoveClosestResourcePin(orePrefabName, mineRock.transform.position, 35f);

            return TryAddResourcePin(new ResourcePinModel(
                zdo.m_uid,
                orePrefabName,
                itemPrefabName,
                mineRock.transform.position,
                ClusterRadius));
        }

        private static bool IsTrackedPickablePrefab(string pickablePrefabName)
        {
            return PluginConfig.IsResourceTrackingEnabled(pickablePrefabName);
        }

        private static bool HasNearbyResourcePin(ResourcePinModel model)
        {
            float radiusSqr = model.ClusterRadius * model.ClusterRadius;

            foreach (ResourcePinModel existingModel in ResourcePinByZdoId.Values)
            {
                if (existingModel.PickablePrefabName != model.PickablePrefabName)
                {
                    continue;
                }

                float deltaX = existingModel.Position.x - model.Position.x;
                float deltaZ = existingModel.Position.z - model.Position.z;
                float distanceSqr = (deltaX * deltaX) + (deltaZ * deltaZ);

                if (distanceSqr <= radiusSqr)
                {
                    return true;
                }
            }

            return false;
        }

        private static Sprite GetSprite(string prefabName)
        {
            if (string.IsNullOrWhiteSpace(prefabName))
            {
                return null;
            }

            if (ResourceSprites.TryGetValue(prefabName, out Sprite cachedSprite))
            {
                return cachedSprite;
            }

            GameObject prefab = ObjectDB.instance.GetItemPrefab(prefabName);

            if (prefab == null)
            {
                return null;
            }

            var itemDrop = prefab.GetComponent<ItemDrop>();

            if (itemDrop == null)
            {
                return null;
            }

            Sprite sprite = itemDrop.m_itemData.GetIcon();

            ResourceSprites[prefabName] = sprite;

            return sprite;
        }

        private static void SetPinUpdateRequired()
        {
            if (Minimap.instance == null)
            {
                return;
            }

            MPinUpdateRequired.SetValue(Minimap.instance, true);
        }

        private static float GetResourcePinSize(string itemPrefabName)
        {
            if (ResourceIconSizeOverrides.TryGetValue(itemPrefabName, out float overrideSize))
            {
                return overrideSize;
            }

            return ResourcePinSize;
        }

        internal static bool RemoveClosestResourcePin(string prefabName, Vector3 position, float radius)
        {
            ResourcePinModel closestModel = null;
            float closestDistanceSqr = radius * radius;

            foreach (ResourcePinModel model in ResourcePinByZdoId.Values)
            {
                if (model.PickablePrefabName != prefabName)
                {
                    continue;
                }

                float deltaX = model.Position.x - position.x;
                float deltaZ = model.Position.z - position.z;
                float distanceSqr = (deltaX * deltaX) + (deltaZ * deltaZ);

                if (distanceSqr <= closestDistanceSqr)
                {
                    closestDistanceSqr = distanceSqr;
                    closestModel = model;
                }
            }

            if (closestModel == null)
            {
                return false;
            }

            return RemoveResourcePin(closestModel.ZdoId);
        }
    }
}
