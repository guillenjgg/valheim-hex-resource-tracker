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
                new TrackedCreatureDefinitionModel("Hare", "Hare", "TrophyHare", 2),
                new TrackedCreatureDefinitionModel("Lox", "Lox", "TrophyLox", 3),

                // Off by default: players switch on what they hunt.
                new TrackedCreatureDefinitionModel("Neck", "Neck", "TrophyNeck", 10, enabledByDefault: false),
                new TrackedCreatureDefinitionModel("Bjorn", "Bear", "TrophyBjorn", 20, enabledByDefault: false, alsoPrefabNames: new[] { "Bjorn_sleeping" }),
                new TrackedCreatureDefinitionModel("Abomination", "Abomination", "TrophyAbomination", 30, enabledByDefault: false),
                new TrackedCreatureDefinitionModel("Wraith", "Wraith", "TrophyWraith", 31, enabledByDefault: false),
                new TrackedCreatureDefinitionModel("Writhan", "Writhan", "TrophyWrithan", 38, enabledByDefault: false),
                new TrackedCreatureDefinitionModel("Surtling", "Surtling", "TrophySurtling", 32, enabledByDefault: false),
                new TrackedCreatureDefinitionModel("Wolf", "Wolf", "TrophyWolf", 40, enabledByDefault: false),
                new TrackedCreatureDefinitionModel("Hatchling", "Drake", "TrophyHatchling", 41, enabledByDefault: false),
                new TrackedCreatureDefinitionModel("Deathsquito", "Deathsquito", "TrophyDeathsquito", 50, enabledByDefault: false),
                new TrackedCreatureDefinitionModel("Unbjorn", "Vile", "TrophyBjornUndead", 51, enabledByDefault: false),
                new TrackedCreatureDefinitionModel("Asksvin", "Asksvin", "TrophyAsksvin", 70, enabledByDefault: false),
                new TrackedCreatureDefinitionModel("FallenValkyrie", "Fallen Valkyrie", "TrophyFallenValkyrie", 71, enabledByDefault: false),
                new TrackedCreatureDefinitionModel("Morgen", "Morgen", "TrophyMorgen", 72, enabledByDefault: false, alsoPrefabNames: new[] { "Morgen_NonSleeping" }),
                new TrackedCreatureDefinitionModel("BonemawSerpent", "Bonemaw", "TrophyBonemawSerpent", 73, enabledByDefault: false),
                new TrackedCreatureDefinitionModel("Moose", "Moose", "TrophyMoose", 80, enabledByDefault: false),
                new TrackedCreatureDefinitionModel("Seal", "Seal", "TrophySeal", 81, enabledByDefault: false),
                // The live Gammeltroll has no trophy of its own; it borrows the Troll's.
                new TrackedCreatureDefinitionModel("TrollFrost", "Gammeltroll", "TrophyFrostTroll", 82, enabledByDefault: false),
                new TrackedCreatureDefinitionModel("Serpent", "Serpent", "TrophySerpent", 90, enabledByDefault: false),

                // Food and leather sources (Friday's drops report, 2026-11-26). Creatures without a trophy show the item they drop.
                new TrackedCreatureDefinitionModel("Troll", "Troll", "TrophyFrostTroll", 21, enabledByDefault: false, alsoPrefabNames: new[] { "Troll_sleeping" }),
                new TrackedCreatureDefinitionModel("Greydwarf", "Greydwarf", "TrophyGreydwarf", 22, enabledByDefault: false, alsoPrefabNames: new[] { "Greydwarf_Frozen" }),
                new TrackedCreatureDefinitionModel("Greydwarf_Elite", "Greydwarf Brute", "TrophyGreydwarfBrute", 23, enabledByDefault: false),
                new TrackedCreatureDefinitionModel("Greydwarf_Shaman", "Greydwarf Shaman", "TrophyGreydwarfShaman", 24, enabledByDefault: false, alsoPrefabNames: new[] { "Greydwarf_Shaman_Frozen" }),
                new TrackedCreatureDefinitionModel("Crow", "Crow", "Feathers", 25, enabledByDefault: false),
                new TrackedCreatureDefinitionModel("Draugr", "Draugr", "TrophyDraugr", 33, enabledByDefault: false, alsoPrefabNames: new[] { "Draugr_sleeping", "Draugr_Ranged", "Draugr_Ranged_sleeping" }),
                new TrackedCreatureDefinitionModel("Draugr_Elite", "Draugr Elite", "TrophyDraugrElite", 34, enabledByDefault: false, alsoPrefabNames: new[] { "Draugr_Elite_sleeping" }),
                new TrackedCreatureDefinitionModel("Leech", "Leech", "TrophyLeech", 35, enabledByDefault: false, alsoPrefabNames: new[] { "Leech_cave" }),
                new TrackedCreatureDefinitionModel("Blob", "Blob", "TrophyBlob", 36, enabledByDefault: false),
                new TrackedCreatureDefinitionModel("BlobElite", "Oozer", "TrophyBlob", 37, enabledByDefault: false),
                new TrackedCreatureDefinitionModel("GoblinShaman", "Fuling Shaman", "TrophyGoblinShaman", 52, enabledByDefault: false),
                new TrackedCreatureDefinitionModel("Seagal", "Gull", "Feathers", 53, enabledByDefault: false),
                new TrackedCreatureDefinitionModel("Seeker", "Seeker", "TrophySeeker", 60, enabledByDefault: false),
                new TrackedCreatureDefinitionModel("SeekerBrute", "Seeker Soldier", "TrophySeekerBrute", 61, enabledByDefault: false),
                new TrackedCreatureDefinitionModel("SeekerBrood", "Seeker Brood", "RoyalJelly", 62, enabledByDefault: false),
                new TrackedCreatureDefinitionModel("Tick", "Tick", "TrophyTick", 63, enabledByDefault: false),
                new TrackedCreatureDefinitionModel("Gjall", "Gjall", "TrophyGjall", 64, enabledByDefault: false),
                new TrackedCreatureDefinitionModel("Volture", "Volture", "TrophyVolture", 74, enabledByDefault: false),
                new TrackedCreatureDefinitionModel("AshCrow", "Ash Crow", "Feathers", 75, enabledByDefault: false),
                new TrackedCreatureDefinitionModel("Skeleton_DeepNorth", "Deep North Skeleton", "TrophySkeleton", 83, enabledByDefault: false),
                new TrackedCreatureDefinitionModel("JotunWarrior", "Krigen", "TrophyJotunWarrior", 84, enabledByDefault: false, alsoPrefabNames: new[] { "JotunWarriorDualWield" }),

                // Fish: each fish is its own item, so its icon is the fish itself.
                new TrackedCreatureDefinitionModel("Fish1", "Perch", "Fish1", 100, enabledByDefault: false),
                new TrackedCreatureDefinitionModel("Fish2", "Pike", "Fish2", 101, enabledByDefault: false),
                new TrackedCreatureDefinitionModel("Fish3", "Tuna", "Fish3", 102, enabledByDefault: false),
                new TrackedCreatureDefinitionModel("Fish5", "Trollfish", "Fish5", 104, enabledByDefault: false),
                new TrackedCreatureDefinitionModel("Fish6", "Giant Herring", "Fish6", 105, enabledByDefault: false),
                new TrackedCreatureDefinitionModel("Fish7", "Grouper", "Fish7", 106, enabledByDefault: false),
                new TrackedCreatureDefinitionModel("Fish8", "Coral Cod", "Fish8", 107, enabledByDefault: false),
                new TrackedCreatureDefinitionModel("Fish9", "Anglerfish", "Fish9", 108, enabledByDefault: false),
                new TrackedCreatureDefinitionModel("Fish10", "Northern Salmon", "Fish10", 109, enabledByDefault: false),
                new TrackedCreatureDefinitionModel("Fish11", "Magmafish", "Fish11", 110, enabledByDefault: false),
                new TrackedCreatureDefinitionModel("Fish12", "Pufferfish", "Fish12", 111, enabledByDefault: false)
            };

        private static readonly Dictionary<string, TrackedCreatureDefinitionModel> DefinitionsByPrefabName =
            new Dictionary<string, TrackedCreatureDefinitionModel>();

        static TrackedCreatures()
        {
            foreach (var definition in AllTrackedCreatures)
            {
                DefinitionsByPrefabName[definition.PrefabName] = definition;
                DefinitionsByPrefabName[definition.PrefabName + "_0"] = definition;

                foreach (string alsoPrefabName in definition.AlsoPrefabNames)
                {
                    DefinitionsByPrefabName[alsoPrefabName] = definition;
                }
            }
        }

        internal static bool TryGetDefinition(string prefabName, out TrackedCreatureDefinitionModel definition)
        {
            return DefinitionsByPrefabName.TryGetValue(prefabName, out definition);
        }
    }
}