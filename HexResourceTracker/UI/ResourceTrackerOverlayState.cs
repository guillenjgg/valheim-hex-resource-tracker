using System.Collections.Generic;
using UnityEngine.UI;

namespace HexResourceTracker.UI
{
    internal static class ResourceTrackerOverlayState
    {
        internal static readonly Dictionary<string, Toggle> ResourceToggles = new Dictionary<string, Toggle>();
        internal static readonly Dictionary<Room.Theme, Toggle> DungeonToggles = new Dictionary<Room.Theme, Toggle>();
        internal static readonly Dictionary<string, Toggle> CreatureToggles = new Dictionary<string, Toggle>();

        internal static void HandleResourceTrackingChanged(string prefabName, bool isEnabled)
        {
            if (string.IsNullOrWhiteSpace(prefabName))
            {
                return;
            }

            if (ResourceToggles.TryGetValue(prefabName, out Toggle toggle) && toggle != null)
            {
                toggle.SetIsOnWithoutNotify(isEnabled);
            }
        }

        internal static void HandleDungeonTrackingChanged(Room.Theme theme, bool isEnabled)
        {
            if (DungeonToggles.TryGetValue(theme, out Toggle toggle) && toggle != null)
            {
                toggle.SetIsOnWithoutNotify(isEnabled);
            }
        }

        internal static void HandleCreatureTrackingChanged(string prefabName, bool isEnabled)
        {
            if (CreatureToggles.TryGetValue(prefabName, out Toggle toggle) && toggle != null)
            {
                toggle.SetIsOnWithoutNotify(isEnabled);
            }
        }

        internal static void Clear()
        {
            ResourceToggles.Clear();
            DungeonToggles.Clear();
            CreatureToggles.Clear();
        }
    }
}
