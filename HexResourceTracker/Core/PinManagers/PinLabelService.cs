using HarmonyLib;
using System.Reflection;

namespace HexResourceTracker.Core.PinManagers
{
    internal static class PinLabelService
    {
        private static readonly FieldInfo PinUpdateRequired = AccessTools.Field(typeof(Minimap), "m_pinUpdateRequired");

        private static readonly MethodInfo DestroyMapMarker = AccessTools.Method(typeof(Minimap.PinNameData), "DestroyMapMarker");

        internal static void SetLabel(Minimap.PinData pin, string label)
        {
            if (pin == null)
            {
                return;
            }

            pin.m_name = label ?? string.Empty;
            if (string.IsNullOrEmpty(pin.m_name))
            {
                // Remove the label object before releasing its owner so it cannot linger on the map.
                if (pin.m_NamePinData != null)
                {
                    DestroyMapMarker.Invoke(pin.m_NamePinData, null);
                }
                pin.m_NamePinData = null;
            }
            else
            {
                pin.m_NamePinData = pin.m_NamePinData ?? new Minimap.PinNameData(pin);
                if (pin.m_NamePinData.PinNameText != null)
                {
                    pin.m_NamePinData.PinNameText.text = pin.m_name;
                }
            }
        }

        internal static void RefreshLabels()
        {
            if (Minimap.instance != null)
            {
                PinUpdateRequired.SetValue(Minimap.instance, true);
            }
        }
    }
}
