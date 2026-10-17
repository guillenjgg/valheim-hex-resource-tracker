
using HexResourceTracker.Core.PinManagers;
using UnityEngine;

namespace HexResourceTracker.Core.Tracking
{
    internal sealed class CreatureTrackingUpdater : MonoBehaviour
    {
        private const float UpdateInterval = 0.25f;

        private float _nextUpdateTime;

        private void Update()
        {
            if (Time.time < _nextUpdateTime)
            {
                return;
            }

            _nextUpdateTime = Time.time + UpdateInterval;

            if (!PluginConfig.IsModEnabled.Value)
            {
                return;
            }

            CreaturePinManager.ReconcileCreaturePins();
        }

        internal static void EnsureCreated()
        {
            Player player = Player.m_localPlayer;

            if (player == null || player.TryGetComponent(out CreatureTrackingUpdater _))
            {
                return;
            }

            player.gameObject.AddComponent<CreatureTrackingUpdater>();
        }
    }
}
