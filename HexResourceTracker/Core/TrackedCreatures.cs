using HexResourceTracker.Models;
using System.Collections.Generic;

namespace HexResourceTracker.Core
{
    internal static class TrackedCreatures
    {
        internal static readonly List<TrackedCreatureDefinitionModel> AllTrackedCreatures =
            new List<TrackedCreatureDefinitionModel>
            {
                new TrackedCreatureDefinitionModel("Deer", "Deer", "TrophyDeer", 0, "HexDeerSprite_Golden_Bold"),
                new TrackedCreatureDefinitionModel("Boar", "Boar", "TrophyBoar", 1),
                new TrackedCreatureDefinitionModel("Hare", "Hare", "TrophyHare", 2)
            };

        private static readonly Dictionary<string, TrackedCreatureDefinitionModel> DefinitionsByPrefabName =
            new Dictionary<string, TrackedCreatureDefinitionModel>();

        static TrackedCreatures()
        {
            foreach (var definition in AllTrackedCreatures)
            {
                DefinitionsByPrefabName[definition.PrefabName] = definition;
                DefinitionsByPrefabName[definition.PrefabName + "_0"] = definition;
            }
        }

        internal static bool TryGetDefinition(string prefabName, out TrackedCreatureDefinitionModel definition)
        {
            return DefinitionsByPrefabName.TryGetValue(prefabName, out definition);
        }
    }
}