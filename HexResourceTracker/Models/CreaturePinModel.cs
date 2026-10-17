using static Minimap;

namespace HexResourceTracker.Models
{
    internal sealed class CreaturePinModel
    {
        internal ZDOID ZdoId { get; }
        internal string PrefabName { get; }
        internal PinData Pin { get; set; }

        internal CreaturePinModel(ZDOID zdoId, string prefabName, PinData pin)
        {
            ZdoId = zdoId;
            PrefabName = prefabName;
            Pin = pin;
        }
    }
}
