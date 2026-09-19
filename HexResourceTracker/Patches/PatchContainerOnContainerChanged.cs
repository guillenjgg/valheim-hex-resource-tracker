using HarmonyLib;
using HexResourceTracker.Core;
using HexResourceTracker.Core.Tracking;

namespace HexResourceTracker.Patches
{
    [HarmonyPatch(typeof(Container), nameof(Container.OnContainerChanged))]
    internal static class PatchContainerOnContainerChanged
    {
        private const string MountainChestPrefabName = "TreasureChest_mountains";

        private static void Postfix(Container __instance)
        {
            if (!PluginConfig.IsModEnabled.Value || __instance == null)
            {
                return;
            }

            string prefabName = __instance.gameObject.name.Replace("(Clone)", string.Empty).Trim();

            if (prefabName != MountainChestPrefabName)
            {
                return;
            }

            if (__instance.gameObject.TryGetComponent(out TrackedMapObject trackedObject))
            {
                ContainerResourcePinService.ReconcileTrackedContainer(trackedObject);
            }
        }
    }
}