using HexResourceTracker.Core.PinManagers;
using HexResourceTracker.Core.Tracking.Monoscripts;
using HexResourceTracker.Models;
using UnityEngine;

namespace HexResourceTracker.Core
{
    internal static class TarPitResourcePinService
    {
        internal const string ResourcePrefabName = "TarLiquid";

        internal static void TrackTarPit(LiquidVolume liquidVolume)
        {
            if (liquidVolume == null)
            {
                return;
            }

            if (liquidVolume.m_liquidType != LiquidType.Tar)
            {
                return;
            }

            TrackedMapObject.TryAdd(liquidVolume.gameObject, ResourcePrefabName);
            TryAddResourcePin(liquidVolume);
        }

        internal static void ReconcileTrackedTarPit(TrackedMapObject trackedObject)
        {
            if (trackedObject == null)
            {
                return;
            }

            LiquidVolume liquidVolume = trackedObject.LiquidVolume;

            if (liquidVolume == null)
            {
                return;
            }

            ZNetView nview = trackedObject.ZNetView;

            if (nview == null)
            {
                return;
            }

            if (!nview.IsValid())
            {
                return;
            }

            ZDO zdo = nview.GetZDO();

            if (zdo == null)
            {
                return;
            }

            if (!Tracking.TrackingRangeService.IsWithinTrackingRange(liquidVolume.transform.position))
            {
                ResourcePinManager.RemoveResourcePin(zdo.m_uid);
                return;
            }

            TryAddResourcePin(liquidVolume);
        }

        internal static void HandleResourceTrackingChanged(string prefabName, bool isEnabled)
        {
            if (prefabName != ResourcePrefabName)
            {
                return;
            }

            if (!isEnabled)
            {
                ResourcePinManager.RemoveResourcePins(ResourcePrefabName);
                return;
            }

            LiquidVolume[] liquidVolumes = Object.FindObjectsByType<LiquidVolume>(FindObjectsSortMode.None);

            foreach (LiquidVolume liquidVolume in liquidVolumes)
            {
                TrackTarPit(liquidVolume);
            }
        }

        private static void TryAddResourcePin(LiquidVolume liquidVolume)
        {
            if (liquidVolume.m_liquidType != LiquidType.Tar)
            {
                return;
            }

            if (!PluginConfig.IsResourceTrackingEnabled(ResourcePrefabName))
            {
                return;
            }

            if (!TrackedResources.TryGetByPrefabName(ResourcePrefabName, out TrackedResourceDefinitionModel definition))
            {
                return;
            }

            ZNetView nview = liquidVolume.GetComponent<ZNetView>();

            if (nview == null)
            {
                return;
            }

            if (!nview.IsValid())
            {
                return;
            }

            ZDO zdo = nview.GetZDO();

            if (zdo == null)
            {
                return;
            }

            ResourcePinManager.TryAddResourcePin(new ResourcePinModel(zdo.m_uid, definition, definition.IconItemPrefabName, liquidVolume.transform.position));
        }
    }
}
