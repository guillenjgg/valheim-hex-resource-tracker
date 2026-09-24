using HexResourceTracker.Models;
using UnityEngine;
using static Minimap;

internal class ResourcePinModel
{
    internal ZDOID ZdoId { get; }
    internal TrackedResourceDefinition ResourceDefinition { get; }
    internal string IconItemPrefabName { get; }
    internal Vector3 Position { get; }
    internal PinData Pin { get; set; }
    internal RectTransform LastSizedUiElement { get; set; }
    internal float LastAppliedSize { get; set; } = -1f;

    internal ResourcePinModel(ZDOID zdoId, TrackedResourceDefinition resourceDefinition, string iconItemPrefabName, Vector3 position)
    {
        ZdoId = zdoId;
        ResourceDefinition = resourceDefinition;
        IconItemPrefabName = iconItemPrefabName;
        Position = position;
    }
}