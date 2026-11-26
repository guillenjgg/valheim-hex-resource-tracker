using HarmonyLib;
using HexResourceTracker.Assets.Assetbundles;
using HexResourceTracker.Core.Tracking;
using HexResourceTracker.Core.Tracking.Monoscripts;
using HexResourceTracker.Models;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using static Minimap;

namespace HexResourceTracker.Core.PinManagers
{
    internal static class CreaturePinManager
    {
        private static readonly Dictionary<ZDOID, CreaturePinModel> CreaturePins = new Dictionary<ZDOID, CreaturePinModel>();
        private static readonly Dictionary<string, Sprite> CreatureSprites = new Dictionary<string, Sprite>();
        private static readonly FieldInfo MPinUpdateRequired = AccessTools.Field(typeof(Minimap), "m_pinUpdateRequired");

        private static Minimap _trackedMinimap;

        internal static void ReconcileCreaturePins()
        {
            if (_trackedMinimap != Minimap.instance)
            {
                CreaturePins.Clear();
                _trackedMinimap = Minimap.instance;
            }

            if (Minimap.instance == null)
            {
                return;
            }

            var activeIds = new HashSet<ZDOID>();
            var shownByPrefab = new Dictionary<string, List<TrackedCreature>>();

            foreach (TrackedCreature creature in TrackedCreature.GetTrackedCreatures())
            {
                if (creature == null)
                {
                    continue;
                }

                if (creature.Body == null)
                {
                    continue;
                }

                if (creature.ZNetView == null)
                {
                    continue;
                }

                if (!creature.ZNetView.IsValid())
                {
                    continue;
                }

                var zdoId = creature.ZdoId;

                if (zdoId == ZDOID.None)
                {
                    continue;
                }

                if (!PluginConfig.IsCreatureTrackingEnabled(creature.PrefabName))
                {
                    continue;
                }

                if (creature.Character != null && creature.Character.IsDead())
                {
                    continue;
                }

                // A caught fish lying on land is an item now, not a fish to find.
                if (creature.Body is Fish fish && fish.IsOutOfWater())
                {
                    continue;
                }

                if (!TrackingRangeService.IsWithinTrackingRange(creature.transform.position))
                {
                    continue;
                }

                if (!shownByPrefab.TryGetValue(creature.PrefabName, out List<TrackedCreature> shown))
                {
                    shown = new List<TrackedCreature>();
                    shownByPrefab[creature.PrefabName] = shown;
                }

                shown.Add(creature);
            }

            float packRadius = PluginConfig.GroupCreaturePacks.Value ? PluginConfig.CreaturePackRadius.Value : 0f;

            foreach (KeyValuePair<string, List<TrackedCreature>> entry in shownByPrefab)
            {
                // A stable order keeps each pack's first member, and so its pin, the same from pass to pass.
                entry.Value.Sort((a, b) => a.ZdoId.CompareTo(b.ZdoId));

                foreach (List<TrackedCreature> pack in CreaturePacks.Group(entry.Value, c => c.transform.position.x, c => c.transform.position.z, packRadius))
                {
                    Vector3 position = Vector3.zero;

                    foreach (TrackedCreature member in pack)
                    {
                        position += member.transform.position;
                    }

                    position /= pack.Count;

                    ZDOID zdoId = pack[0].ZdoId;
                    activeIds.Add(zdoId);
                    ShowCreaturePin(zdoId, entry.Key, position);
                }
            }

            var idsToRemove = new List<ZDOID>();

            foreach (ZDOID zdoId in CreaturePins.Keys)
            {
                if (!activeIds.Contains(zdoId))
                {
                    idsToRemove.Add(zdoId);
                }
            }

            foreach (ZDOID zdoId in idsToRemove)
            {
                RemoveCreaturePin(zdoId);
            }
        }
        
        private static void ShowCreaturePin(ZDOID zdoId, string prefabName, Vector3 position)
        {
            if (CreaturePins.TryGetValue(zdoId, out CreaturePinModel model))
            {
                if (model.Pin != null && model.PrefabName == prefabName)
                {
                    if ((model.Pin.m_pos - position).sqrMagnitude > 0.01f)
                    {
                        model.Pin.m_pos = position;
                        SetPinUpdateRequired();
                    }

                    return;
                }

                RemoveCreaturePin(zdoId);
            }

            Sprite sprite = GetCreatureSprite(prefabName);

            if (sprite == null)
            {
                return;
            }

            PinData pin = Minimap.instance.AddPin(
                position,
                PinType.None,
                string.Empty,
                false,
                false);

            if (pin == null)
            {
                return;
            }

            pin.m_icon = sprite;
            CreaturePins[zdoId] = new CreaturePinModel(zdoId, prefabName, pin);

            SetPinUpdateRequired();
        }

        internal static void RemoveCreaturePin(ZDOID zdoId)
        {
            if (!CreaturePins.TryGetValue(zdoId, out CreaturePinModel model))
            {
                return;
            }

            if (model.Pin != null && Minimap.instance != null)
            {
                Minimap.instance.RemovePin(model.Pin);
            }

            CreaturePins.Remove(zdoId);
            SetPinUpdateRequired();
        }

        internal static void RemoveCreaturePins(string prefabName)
        {
            var idsToRemove = new List<ZDOID>();

            foreach (KeyValuePair<ZDOID, CreaturePinModel> entry in CreaturePins)
            {
                if (entry.Value.PrefabName == prefabName)
                {
                    idsToRemove.Add(entry.Key);
                }
            }

            foreach (ZDOID zdoId in idsToRemove)
            {
                RemoveCreaturePin(zdoId);
            }
        }

        private static Sprite GetCreatureSprite(string prefabName)
        {
            if (CreatureSprites.TryGetValue(prefabName, out Sprite sprite))
            {
                return sprite;
            }

            if (!TrackedCreatures.TryGetDefinition(prefabName, out var definition))
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(definition.CustomSpriteName))
            {
                sprite = AssetBundleHelper.LoadSprite(definition.CustomSpriteName);
            }

            if (sprite == null && !string.IsNullOrWhiteSpace(definition.TrophyPrefabName))
            {
                if (ObjectDB.instance == null)
                {
                    return null;
                }

                var trophyPrefab = ObjectDB.instance.GetItemPrefab(definition.TrophyPrefabName);

                if (trophyPrefab == null)
                {
                    return null;
                }

                var itemDrop = trophyPrefab.GetComponent<ItemDrop>();

                if (itemDrop == null)
                {
                    return null;
                }

                sprite = itemDrop.m_itemData.GetIcon();
            }

            if (sprite != null)
            {
                CreatureSprites[prefabName] = sprite;
            }

            return sprite;
        }

        private static void SetPinUpdateRequired()
        {
            if (Minimap.instance != null)
            {
                MPinUpdateRequired.SetValue(Minimap.instance, true);
            }
        }
    }
}
