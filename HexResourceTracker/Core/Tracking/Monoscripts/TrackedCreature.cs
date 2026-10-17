using HexResourceTracker.Core.PinManagers;
using System.Collections.Generic;
using UnityEngine;

namespace HexResourceTracker.Core.Tracking.Monoscripts
{
    internal sealed class TrackedCreature : MonoBehaviour
    {
        private static readonly HashSet<TrackedCreature> TrackedCreatures = new HashSet<TrackedCreature>();

        internal Character Character { get; private set; }
        internal string PrefabName { get; private set; }
        internal ZNetView ZNetView { get; private set; }
        internal ZDOID ZdoId { get; private set; }

        internal static HashSet<TrackedCreature> GetTrackedCreatures()
        {
            return TrackedCreatures;
        }

        internal void Initialize(Character character, string prefabName)
        {
            Character = character;
            PrefabName = prefabName;
            ZNetView = character.GetComponent<ZNetView>();

            ZdoId = ZNetView != null && ZNetView.IsValid() ? ZNetView.GetZDO().m_uid : ZDOID.None;

            TrackedCreatures.Add(this);
        }

        internal static void TryAdd(Character character, string prefabName)
        {
            if (character == null || character.TryGetComponent(out TrackedCreature _))
            {
                return;
            }

            var trackedCreature = character.gameObject.AddComponent<TrackedCreature>();
            trackedCreature.Initialize(character, prefabName);
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
