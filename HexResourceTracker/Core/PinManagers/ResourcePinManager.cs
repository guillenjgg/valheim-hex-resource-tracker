using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using HexResourceTracker.Core.Tracking;
using HexResourceTracker.Models;
using UnityEngine;
using UnityEngine.UI;
using static Minimap;

namespace HexResourceTracker.Core.PinManagers
{
    internal static class ResourcePinManager
    {
        private const float ResourcePinSize = 20f;
        private const float ClusterRadius = 25f;
        private const float OnionSeedsBorderPadding = -16f;
        private const string OnionSeedsPrefabName = "OnionSeeds";
        private const string OnionSeedsBorderName = "OnionSeedsBorder";

        private static readonly Color OnionSeedsBorderColor = new Color(1f, 0.75f, 0.15f, 1f);

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
            { "Softtissue", 32f },
            { OnionSeedsPrefabName, 32f }
        };

        internal static bool TryAddResourcePin(ResourcePinModel model)
        {
            if (model == null ||
                model.ResourceDefinition == null ||
                Minimap.instance == null ||
                model.ZdoId == ZDOID.None)
            {
                return false;
            }

            if (!TrackingRangeService.IsWithinTrackingRange(model.Position))
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

            Sprite sprite = GetSprite(model.ResourceDefinition.IconItemPrefabName);

            if (sprite != null)
            {
                pin.m_icon = sprite;
            }

            model.Pin = pin;
            ResourcePinByZdoId[model.ZdoId] = model;

            SetPinUpdateRequired();

            return true;
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

                string iconItemPrefabName = model.ResourceDefinition.IconItemPrefabName;
                float size = GetResourcePinSize(iconItemPrefabName);

                if (model.LastSizedUiElement == pin.m_uiElement && model.LastAppliedSize == size)
                {
                    continue;
                }

                pin.m_uiElement.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
                pin.m_uiElement.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);

                if (model.ResourceDefinition.ResourcePrefabName == OnionSeedsPrefabName)
                {
                    AddOnionSeedsBorder(pin.m_uiElement, size);
                }

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

        internal static void RemoveResourcePins(string prefabName)
        {
            if (string.IsNullOrWhiteSpace(prefabName))
            {
                return;
            }

            var zdoIdsToRemove = new List<ZDOID>();

            foreach (KeyValuePair<ZDOID, ResourcePinModel> entry in ResourcePinByZdoId)
            {
                if (entry.Value.ResourceDefinition.ResourcePrefabName == prefabName)
                {
                    zdoIdsToRemove.Add(entry.Key);
                }
            }

            foreach (ZDOID zdoId in zdoIdsToRemove)
            {
                RemoveResourcePin(zdoId);
            }
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

        internal static bool RemoveClosestResourcePin(string prefabName, Vector3 position, float radius)
        {
            ResourcePinModel closestModel = null;
            float closestDistanceSqr = radius * radius;

            foreach (ResourcePinModel model in ResourcePinByZdoId.Values)
            {
                if (model.ResourceDefinition.ResourcePrefabName != prefabName)
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

        internal static bool HasResourcePin(ZDOID zdoId)
        {
            return zdoId != ZDOID.None && ResourcePinByZdoId.ContainsKey(zdoId);
        }

        private static bool HasNearbyResourcePin(ResourcePinModel model)
        {
            float radiusSqr = ClusterRadius * ClusterRadius;

            foreach (ResourcePinModel existingModel in ResourcePinByZdoId.Values)
            {
                if (existingModel.ResourceDefinition.ResourcePrefabName != model.ResourceDefinition.ResourcePrefabName)
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

            var prefab = ObjectDB.instance.GetItemPrefab(prefabName);

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

        private static float GetResourcePinSize(string iconItemPrefabName)
        {
            if (!string.IsNullOrWhiteSpace(iconItemPrefabName) &&
                ResourceIconSizeOverrides.TryGetValue(iconItemPrefabName, out float size))
            {
                return size;
            }

            return ResourcePinSize;
        }

        private static void AddOnionSeedsBorder(RectTransform pinUiElement, float iconSize)
        {
            Transform existingBorder = pinUiElement.Find(OnionSeedsBorderName);

            if (existingBorder != null)
            {
                return;
            }

            var borderObject = new GameObject(OnionSeedsBorderName);
            borderObject.transform.SetParent(pinUiElement, false);
            borderObject.transform.SetAsFirstSibling();

            var borderRect = borderObject.AddComponent<RectTransform>();
            borderRect.anchorMin = new Vector2(0.5f, 0.5f);
            borderRect.anchorMax = new Vector2(0.5f, 0.5f);
            borderRect.pivot = new Vector2(0.5f, 0.5f);
            borderRect.anchoredPosition = Vector2.zero;

            float borderSize = iconSize + OnionSeedsBorderPadding;
            borderRect.sizeDelta = new Vector2(borderSize, borderSize);

            AddBorderEdge(borderRect, "Top", new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -2f), new Vector2(0f, 2f));
            AddBorderEdge(borderRect, "Bottom", new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, 0f), new Vector2(0f, 2f));
            AddBorderEdge(borderRect, "Left", new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(0f, 0f), new Vector2(2f, 0f));
            AddBorderEdge(borderRect, "Right", new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(-2f, 0f), new Vector2(2f, 0f));
        }

        private static void AddBorderEdge(RectTransform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            var edgeObject = new GameObject(name);
            edgeObject.transform.SetParent(parent, false);

            var edgeRect = edgeObject.AddComponent<RectTransform>();
            edgeRect.anchorMin = anchorMin;
            edgeRect.anchorMax = anchorMax;
            edgeRect.pivot = new Vector2(0.5f, 0.5f);
            edgeRect.anchoredPosition = anchoredPosition;
            edgeRect.sizeDelta = sizeDelta;

            var edgeImage = edgeObject.AddComponent<Image>();
            edgeImage.color = OnionSeedsBorderColor;
            edgeImage.raycastTarget = false;
        }

        private static void SetPinUpdateRequired()
        {
            if (Minimap.instance == null)
            {
                return;
            }

            MPinUpdateRequired.SetValue(Minimap.instance, true);
        }
    }
}