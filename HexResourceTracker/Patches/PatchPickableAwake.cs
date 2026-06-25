using HarmonyLib;

namespace HexResourceTracker.Patches
{
    [HarmonyPatch(typeof(Pickable), nameof(Pickable.Awake))]
    internal static class PatchPickableAwake
    {
        private const float ClusterRadius = 25f;

        private static void Postfix(Pickable __instance)
        {
            if (__instance == null || Minimap.instance == null)
            {
                return;
            }

            var nview = __instance.GetComponent<ZNetView>();

            if (nview == null || !nview.IsValid())
            {
                return;
            }

            if (!__instance.CanBePicked())
            {
                return;
            }

            string pickablePrefabName = __instance.gameObject.name.Replace("(Clone)", "").Trim();

            if (!ResourcePinManager.IsTrackedPickablePrefab(pickablePrefabName))
            {
                return;
            }

            if (__instance.m_itemPrefab == null)
            {
                return;
            }

            ZDO zdo = nview.GetZDO();

            if (zdo == null)
            {
                return;
            }

            var model = new ResourcePinModel(
                zdo.m_uid,
                pickablePrefabName,
                __instance.m_itemPrefab.name,
                __instance.transform.position,
                ClusterRadius);

            ResourcePinManager.TryAddResourcePin(model);
        }
    }
}