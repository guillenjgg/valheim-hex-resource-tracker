using HexResourceTracker.Core.PinManagers;
using UnityEngine;

namespace HexResourceTracker.Core
{
    internal static class LocationSearchService
    {
        internal static void FindNearestMemorialSite()
        {
            FindNearest("NorthMemorialPlace");
        }

        private static void FindNearest(string prefabName)
        {
            Player player = Player.m_localPlayer;
            if (player == null || Minimap.instance == null || !PluginConfig.IsModEnabled.Value)
            {
                return;
            }
            if (!PluginConfig.IsLocationTrackingEnabled(prefabName))
            {
                player.Message(MessageHud.MessageType.Center, "Enable Memorial Site tracking first.");
                return;
            }
            if (ZoneSystem.instance == null || ZNet.instance == null)
            {
                player.Message(MessageHud.MessageType.Center, "The world is not ready to search yet.");
                return;
            }
            // Remote clients do not receive the server's undiscovered location list.
            if (!ZNet.instance.IsServer())
            {
                player.Message(MessageHud.MessageType.Center, "Finding unvisited sites is available in single player or to the world host.");
                return;
            }

            Vector3 origin = player.transform.position;
            if (!ZoneSystem.instance.FindClosestLocation(prefabName, origin, out ZoneSystem.LocationInstance location))
            {
                player.Message(MessageHud.MessageType.Center, "No Memorial Site found in this world's location list. Try again once the world finishes loading.");
                return;
            }

            LocationPinManager.RevealLocation(prefabName, location.m_position);
            Minimap.instance.ShowPointOnMap(location.m_position);
            float deltaX = origin.x - location.m_position.x;
            float deltaZ = origin.z - location.m_position.z;
            float distance = Mathf.Sqrt(deltaX * deltaX + deltaZ * deltaZ);
            player.Message(MessageHud.MessageType.Center, $"Nearest Memorial Site: {distance:0} m away.");
        }
    }
}
