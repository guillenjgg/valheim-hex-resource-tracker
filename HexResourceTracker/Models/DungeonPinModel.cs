using UnityEngine;
using static Minimap;

namespace HexResourceTracker
{
    internal class DungeonPinModel
    {
        internal Room.Theme Theme { get; }
        internal string LocationPrefabName { get; }
        internal Vector3 Position { get; }
        internal PinData Pin { get; set; }

        internal DungeonPinModel(Room.Theme theme, string locationPrefabName, Vector3 position)
        {
            Theme = theme;
            LocationPrefabName = locationPrefabName;
            Position = position;
        }
    }
}