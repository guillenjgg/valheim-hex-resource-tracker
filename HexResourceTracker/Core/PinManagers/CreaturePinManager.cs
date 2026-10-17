using HarmonyLib;
using HexResourceTracker.Assets.Assetbundles;
using HexResourceTracker.Core.Tracking;
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

            foreach (TrackedCreature creature in TrackedCreature.GetTrackedCreatures())
            {
                if (creature == null || creature.Character == null || creature.ZNetView == null)
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

                activeIds.Add(zdoId);

                if (!PluginConfig.IsCreatureTrackingEnabled(creature.PrefabName) ||
                    creature.Character.IsDead() ||
                    !TrackingRangeService.IsWithinTrackingRange(creature.transform.position))
                {
                    RemoveCreaturePin(zdoId);
                    continue;
                }

                if (CreaturePins.TryGetValue(zdoId, out CreaturePinModel model))
                {
                    if (model.Pin != null)
                    {
                        var position = creature.transform.position;

                        if ((model.Pin.m_pos - position).sqrMagnitude > 0.01f)
                        {
                            model.Pin.m_pos = position;
                            SetPinUpdateRequired();
                        }

                        continue;
                    }

                    CreaturePins.Remove(zdoId);
                }

                Sprite sprite = GetCreatureSprite(creature.PrefabName);

                if (sprite == null)
                {
                    continue;
                }

                PinData pin = Minimap.instance.AddPin(
                    creature.transform.position,
                    PinType.None,
                    string.Empty,
                    false,
                    false);

                if (pin == null)
                {
                    continue;
                }

                pin.m_icon = sprite;
                CreaturePins[zdoId] = new CreaturePinModel(zdoId, creature.PrefabName, pin);

                SetPinUpdateRequired();
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

            switch (prefabName)
            {
                case "Deer":
                    sprite = AssetBundleHelper.LoadSprite("HexDeerSprite_Golden_Bold");
                    break;
            }

            if (sprite == null)
            {
                string trophyPrefabName;

                switch (prefabName)
                {
                    case "Deer":
                        trophyPrefabName = "TrophyDeer";
                        break;
                    default:
                        return null;
                }

                if (ObjectDB.instance == null)
                {
                    return null;
                }

                var trophyPrefab = ObjectDB.instance.GetItemPrefab(trophyPrefabName);

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
