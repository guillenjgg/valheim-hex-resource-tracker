using System.Collections.Generic;
using HexResourceTracker.Core.PinManagers;
using HexResourceTracker.Models;
using UnityEngine;

namespace HexResourceTracker.Core
{
    // Debug logging for mineable objects, behind the "Log Deposit Changes" setting (off by default).
    // Writes one line per first hit, break and mine-out, so new deposits can be identified from the log.
    internal static class DepositChangeLog
    {
        private const string Prefix = "[DepositLog]";

        private static readonly HashSet<ZDOID> LoggedFirstHits = new HashSet<ZDOID>();
        private static readonly HashSet<ZDOID> LoggedMinedOut = new HashSet<ZDOID>();

        private static bool IsEnabled =>
            PluginConfig.IsModEnabled.Value && PluginConfig.LogDepositChanges != null && PluginConfig.LogDepositChanges.Value;

        internal static void LogFirstHit(Component component, string kind, string nameToken)
        {
            if (!IsEnabled || component == null)
            {
                return;
            }

            ZDOID zdoId = GetZdoId(component);

            if (zdoId != ZDOID.None && !LoggedFirstHits.Add(zdoId))
            {
                return;
            }

            Plugin.Log.LogInfo($"{Prefix} first hit: {Describe(component, kind, nameToken, zdoId)}");
        }

        internal static void LogDestructibleDestroyed(Destructible destructible)
        {
            if (!IsEnabled || destructible == null)
            {
                return;
            }

            ZDOID zdoId = GetZdoId(destructible);
            GameObject spawn = destructible.m_spawnWhenDestroyed;
            string turnsInto = "nothing";

            if (spawn != null)
            {
                MineRock5 mineRock5 = spawn.GetComponent<MineRock5>();
                MineRock mineRock = spawn.GetComponent<MineRock>();
                Destructible nextDestructible = spawn.GetComponent<Destructible>();

                string spawnKind = mineRock5 != null ? "MineRock5" :
                    mineRock != null ? "MineRock" :
                    nextDestructible != null ? "Destructible" : "other";
                string spawnToken = mineRock5 != null ? mineRock5.m_name :
                    mineRock != null ? mineRock.m_name : string.Empty;

                turnsInto = $"'{spawn.name}' ({spawnKind}, token '{spawnToken}')";
            }

            Plugin.Log.LogInfo($"{Prefix} broken: {Describe(destructible, "Destructible", string.Empty, zdoId)}, turns into {turnsInto}");
        }

        internal static void LogMinedOut(Component component, string nameToken)
        {
            if (!IsEnabled || component == null)
            {
                return;
            }

            ZDOID zdoId = GetZdoId(component);

            // AllDestroyed is a check the game can call more than once; log the mine-out once per object.
            if (zdoId != ZDOID.None && !LoggedMinedOut.Add(zdoId))
            {
                return;
            }

            string kind = component is MineRock5 ? "MineRock5" : "MineRock";
            Plugin.Log.LogInfo($"{Prefix} mined out: {Describe(component, kind, nameToken, zdoId)}");
        }

        private static string Describe(Component component, string kind, string nameToken, ZDOID zdoId)
        {
            string prefabName = component.gameObject.name.Replace("(Clone)", string.Empty).Trim();
            bool isTracked = TrackedResources.TryGetByPrefabName(prefabName, out TrackedResourceDefinitionModel _) ||
                (!string.IsNullOrEmpty(nameToken) && TrackedResources.TryGetByMineRock5(nameToken, out TrackedResourceDefinitionModel _)) ||
                (!string.IsNullOrEmpty(nameToken) && TrackedResources.TryGetByMineRock(nameToken, out TrackedResourceDefinitionModel _));
            bool hasPin = ResourcePinManager.HasResourcePin(zdoId);
            Vector3 position = component.transform.position;

            return $"'{prefabName}' ({kind}, token '{nameToken}'), tracked {isTracked}, pin {hasPin}, at ({position.x:F0}, {position.z:F0})";
        }

        private static ZDOID GetZdoId(Component component)
        {
            ZNetView nview = component.GetComponent<ZNetView>();

            if (nview == null || !nview.IsValid())
            {
                return ZDOID.None;
            }

            ZDO zdo = nview.GetZDO();
            return zdo != null ? zdo.m_uid : ZDOID.None;
        }
    }
}
