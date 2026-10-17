using HexResourceTracker.Models;
using System.Collections.Generic;

namespace HexResourceTracker.Core
{
    internal static class TrackedDungeons
    {
        internal static readonly List<TrackedDungeonDefinitionModel> AllTrackedDungeons = new List<TrackedDungeonDefinitionModel>
        {
            new TrackedDungeonDefinitionModel(Room.Theme.ForestCrypt, "Burial Chambers", "Burial Chambers", "Burial Chamber", 100),
            new TrackedDungeonDefinitionModel(Room.Theme.SunkenCrypt, "Sunken Crypts", "Sunken Crypts", "Sunken Crypt", 200),
            new TrackedDungeonDefinitionModel(Room.Theme.Cave, "Frost Caves", "Frost Caves", "Frost Cave", 300),
            new TrackedDungeonDefinitionModel(Room.Theme.DvergerTown, "Infested Mines", "Infested Mines", "Infested Mine", 400),
            new TrackedDungeonDefinitionModel(Room.Theme.MorkHalla, "Morkhalla", "Morkhalla", "Morkhalla", 500),
            new TrackedDungeonDefinitionModel(Room.Theme.Hole, "Winding Tunnel", "Winding Tunnels", "Winding Tunnel", 600)
        };
    }
}
