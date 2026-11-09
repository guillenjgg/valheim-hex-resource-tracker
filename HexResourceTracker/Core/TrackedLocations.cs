using System.Collections.Generic;

namespace HexResourceTracker.Core
{
    internal static class TrackedLocations
    {
        internal static readonly Dictionary<string, string> All = new Dictionary<string, string>
        {
            { "NorthMemorialPlace", "Memorial Site" },
            { "CharredFortress", "Charred Fortress" }
        };

        internal static Minimap.PinType GetPinType(string prefabName)
        {
            return prefabName == "CharredFortress" ? Minimap.PinType.Icon2 : Minimap.PinType.Icon3;
        }
    }
}
