using UnityEngine;

namespace HexResourceTracker.Core.Tracking
{
    internal static class TrackingRangeService
    {
        internal static bool IsWithinTrackingRange(Vector3 position)
        {
            if (PluginConfig.TrackingMode.Value == TrackingModeEnum.ZoneBased)
            {
                return true;
            }

            if (Player.m_localPlayer == null)
            {
                return false;
            }

            float range = PluginConfig.TrackingRange.Value;
            Vector3 playerPosition = Player.m_localPlayer.transform.position;

            float deltaX = playerPosition.x - position.x;
            float deltaZ = playerPosition.z - position.z;
            float distanceSqr = (deltaX * deltaX) + (deltaZ * deltaZ);

            return distanceSqr <= range * range;
        }
    }
}