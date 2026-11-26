using HexResourceTracker.Core.PinManagers;
using System.Collections.Generic;
using UnityEngine;

namespace HexResourceTracker.Core.Tracking.Monoscripts
{
    internal sealed class TrackedCreature : MonoBehaviour
    {
        private static readonly HashSet<TrackedCreature> TrackedCreatures = new HashSet<TrackedCreature>();

        // The creature's own component: a Character for creatures, a Fish for fish.
        internal Component Body { get; private set; }
        internal Character Character { get; private set; }
        internal string PrefabName { get; private set; }
        internal ZNetView ZNetView { get; private set; }
        internal ZDOID ZdoId { get; private set; }

        internal static HashSet<TrackedCreature> GetTrackedCreatures()
        {
            return TrackedCreatures;
        }

        internal void Initialize(Component body, string prefabName)
        {
            Body = body;
            Character = body as Character;
            PrefabName = prefabName;
            ZNetView = body.GetComponent<ZNetView>();

            ZdoId = ZNetView != null && ZNetView.IsValid() ? ZNetView.GetZDO().m_uid : ZDOID.None;

            TrackedCreatures.Add(this);
        }

        internal static void TryAdd(Component body, string prefabName)
        {
            if (body == null || body.TryGetComponent(out TrackedCreature _))
            {
                return;
            }

            var trackedCreature = body.gameObject.AddComponent<TrackedCreature>();
            trackedCreature.Initialize(body, prefabName);
        }

        private void OnDestroy()
        {
            if (ZNetView != null && ZNetView.IsValid())
            {
                CreaturePinManager.RemoveCreaturePin(ZNetView.GetZDO().m_uid);
            }

            TrackedCreatures.Remove(this);
        }
    }
}
