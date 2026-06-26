using HarmonyLib;

namespace HexResourceTracker.Patches
{
    [HarmonyPatch(typeof(Pickable), nameof(Pickable.SetPicked))]
    internal static class PatchPickableSetPicked
    {
        private static void Prefix(Pickable __instance, bool picked, ref ZDOID __state)
        {
            if (!PluginConfig.IsModEnabled.Value)
            {
                return;
            }

            __state = ZDOID.None;

            if (!picked || __instance == null)
            {
                return;
            }

            ZNetView nview = __instance.GetComponent<ZNetView>();

            if (nview == null)
            {
                return;
            }

            ZDO zdo = nview.GetZDO();

            if (zdo == null)
            {
                return;
            }

            __state = zdo.m_uid;
        }

        private static void Postfix(Pickable __instance, bool picked, ZDOID __state)
        {
            if (!PluginConfig.IsModEnabled.Value)
            {
                return;
            }

            if (!picked || __state == ZDOID.None)
            {
                return;
            }

            ResourcePinManager.RemoveResourcePin(__state);
        }
    }
}