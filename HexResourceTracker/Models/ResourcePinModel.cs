using UnityEngine;
using static Minimap;

namespace HexResourceTracker.Models
{
    internal class ResourcePinModel
    {
        internal ZDOID ZdoId { get; }
        internal TrackedResourceDefinition ResourceDefinition { get; }
        internal Vector3 Position { get; }
        internal PinData Pin { get; set; }
        internal RectTransform LastSizedUiElement { get; set; }
        internal float LastAppliedSize { get; set; } = -1f;

        internal ResourcePinModel(ZDOID zdoId, TrackedResourceDefinition resourceDefinition, Vector3 position)
        {
            ZdoId = zdoId;
            ResourceDefinition = resourceDefinition;
            Position = position;
        }
    }
}