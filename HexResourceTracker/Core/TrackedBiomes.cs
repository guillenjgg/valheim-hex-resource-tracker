using HexResourceTracker.Models;
using System.Collections.Generic;

namespace HexResourceTracker.Core
{
    // Which biomes each tracked item belongs to. The map overlay shows only the items of the player's current biome.
    // An item in several biomes shows in each; a shared toggle covers every biome its prefabs are in.
    internal static class TrackedBiomes
    {
        internal const Heightmap.Biome MemorialSiteSearch = Heightmap.Biome.DeepNorth;

        private static readonly Dictionary<Heightmap.Biome, string> Names = new Dictionary<Heightmap.Biome, string>
        {
            { Heightmap.Biome.Meadows, "Meadows" },
            { Heightmap.Biome.BlackForest, "Black Forest" },
            { Heightmap.Biome.Swamp, "Swamp" },
            { Heightmap.Biome.Mountain, "Mountains" },
            { Heightmap.Biome.Plains, "Plains" },
            { Heightmap.Biome.Mistlands, "Mistlands" },
            { Heightmap.Biome.AshLands, "Ashlands" },
            { Heightmap.Biome.DeepNorth, "Deep North" },
            { Heightmap.Biome.Ocean, "Ocean" }
        };

        // Biomes a resource is also found in beyond its biome name (seen in game).
        private static readonly Dictionary<string, Heightmap.Biome> AlsoFoundIn = new Dictionary<string, Heightmap.Biome>
        {
            { "Pickable_Mushroom", Heightmap.Biome.Swamp },
            { "Pickable_Thistle", Heightmap.Biome.Swamp },
            { "SpearWood", Heightmap.Biome.Swamp }
        };

        // Resources without a biome name (deposits).
        private static readonly Dictionary<string, Heightmap.Biome> Resources = new Dictionary<string, Heightmap.Biome>
        {
            { "Beehive", Heightmap.Biome.Meadows },
            { "rock4_copper", Heightmap.Biome.BlackForest },
            { "MineRock_Tin", Heightmap.Biome.BlackForest },
            { "mudpile", Heightmap.Biome.Swamp },
            { "silvervein", Heightmap.Biome.Mountain },
            { "MineRock_Obsidian", Heightmap.Biome.Mountain },
            { TarPitResourcePinService.ResourcePrefabName, Heightmap.Biome.Plains },
            { "giant_skull", Heightmap.Biome.Mistlands },
            { "LeviathanLava", Heightmap.Biome.AshLands },
            { "GuckSack", Heightmap.Biome.Swamp },
            { "Spawner_GreydwarfNest", Heightmap.Biome.BlackForest },
            { "giant_sword1", Heightmap.Biome.Mistlands },
            { "giant_helmet1", Heightmap.Biome.Mistlands },
            { YggdrasilRootPinService.ResourcePrefabName, Heightmap.Biome.Mistlands },
            { "Leviathan", Heightmap.Biome.Ocean }
        };

        private static readonly Dictionary<Room.Theme, Heightmap.Biome> Dungeons = new Dictionary<Room.Theme, Heightmap.Biome>
        {
            { Room.Theme.ForestCrypt, Heightmap.Biome.BlackForest },
            { Room.Theme.SunkenCrypt, Heightmap.Biome.Swamp },
            { Room.Theme.Cave, Heightmap.Biome.Mountain },
            { Room.Theme.DvergerTown, Heightmap.Biome.Mistlands },
            { Room.Theme.MorkHalla, Heightmap.Biome.DeepNorth },
            { Room.Theme.Hole, Heightmap.Biome.DeepNorth }
        };

        private static readonly Dictionary<string, Heightmap.Biome> Creatures = new Dictionary<string, Heightmap.Biome>
        {
            { "Deer", Heightmap.Biome.Meadows | Heightmap.Biome.BlackForest },
            { "Boar", Heightmap.Biome.Meadows },
            { "Hare", Heightmap.Biome.Mistlands },
            { "Lox", Heightmap.Biome.Plains },
            { "Neck", Heightmap.Biome.Meadows },
            { "Bjorn", Heightmap.Biome.BlackForest },
            { "Abomination", Heightmap.Biome.Swamp },
            { "Wraith", Heightmap.Biome.Swamp },
            { "Writhan", Heightmap.Biome.Swamp },
            { "Surtling", Heightmap.Biome.Swamp },
            { "Wolf", Heightmap.Biome.Mountain },
            { "Hatchling", Heightmap.Biome.Mountain },
            { "Deathsquito", Heightmap.Biome.Plains },
            { "Unbjorn", Heightmap.Biome.Plains },
            { "Asksvin", Heightmap.Biome.AshLands },
            { "FallenValkyrie", Heightmap.Biome.AshLands },
            { "Morgen", Heightmap.Biome.AshLands },
            { "BonemawSerpent", Heightmap.Biome.AshLands },
            { "Moose", Heightmap.Biome.DeepNorth },
            { "Seal", Heightmap.Biome.DeepNorth },
            { "TrollFrost", Heightmap.Biome.DeepNorth },
            { "Serpent", Heightmap.Biome.Ocean },
            { "Troll", Heightmap.Biome.BlackForest },
            { "Greydwarf", Heightmap.Biome.Meadows | Heightmap.Biome.BlackForest | Heightmap.Biome.DeepNorth },
            { "Greydwarf_Elite", Heightmap.Biome.BlackForest },
            { "Greydwarf_Shaman", Heightmap.Biome.BlackForest | Heightmap.Biome.DeepNorth },
            { "Crow", Heightmap.Biome.BlackForest },
            { "Draugr", Heightmap.Biome.Swamp | Heightmap.Biome.Meadows },
            { "Draugr_Elite", Heightmap.Biome.Swamp },
            { "Leech", Heightmap.Biome.Swamp },
            { "Blob", Heightmap.Biome.Swamp },
            { "BlobElite", Heightmap.Biome.Swamp },
            { "GoblinShaman", Heightmap.Biome.Plains },
            { "Seagal", Heightmap.Biome.Meadows | Heightmap.Biome.BlackForest | Heightmap.Biome.Plains | Heightmap.Biome.Ocean },
            { "Seeker", Heightmap.Biome.Mistlands },
            { "SeekerBrute", Heightmap.Biome.Mistlands },
            { "SeekerBrood", Heightmap.Biome.Mistlands },
            { "Tick", Heightmap.Biome.Mistlands },
            { "Gjall", Heightmap.Biome.Mistlands },
            { "Volture", Heightmap.Biome.AshLands },
            { "AshCrow", Heightmap.Biome.AshLands },
            { "Skeleton_DeepNorth", Heightmap.Biome.DeepNorth },
            { "JotunWarrior", Heightmap.Biome.DeepNorth },
            // Fish, by where their schools spawn (Valheim Wiki, Fish).
            { "Fish1", Heightmap.Biome.Meadows | Heightmap.Biome.BlackForest },
            { "Fish2", Heightmap.Biome.Meadows | Heightmap.Biome.BlackForest },
            { "Fish3", Heightmap.Biome.Ocean },
            { "Fish5", Heightmap.Biome.BlackForest },
            { "Fish6", Heightmap.Biome.Swamp },
            { "Fish7", Heightmap.Biome.Plains },
            { "Fish8", Heightmap.Biome.Ocean },
            { "Fish9", Heightmap.Biome.Mistlands },
            { "Fish10", Heightmap.Biome.DeepNorth },
            { "Fish11", Heightmap.Biome.AshLands },
            { "Fish12", Heightmap.Biome.Ocean | Heightmap.Biome.Mistlands }
        };

        private static readonly Dictionary<string, Heightmap.Biome> Locations = new Dictionary<string, Heightmap.Biome>
        {
            { "NorthMemorialPlace", Heightmap.Biome.DeepNorth },
            { "CharredFortress", Heightmap.Biome.AshLands },
            { "TrollCave02", Heightmap.Biome.BlackForest },
            { "GoblinCamp2", Heightmap.Biome.Plains },
            { "GoblinCamp2_1", Heightmap.Biome.Plains },
            { "Vendor_BlackForest", Heightmap.Biome.BlackForest },
            { "Hildir_camp", Heightmap.Biome.Meadows },
            { "BogWitch_Camp", Heightmap.Biome.Swamp },
            { "Eikthyrnir", Heightmap.Biome.Meadows },
            { "GDKing", Heightmap.Biome.BlackForest },
            { "Bonemass", Heightmap.Biome.Swamp },
            { "Dragonqueen", Heightmap.Biome.Mountain },
            { "GoblinKing", Heightmap.Biome.Plains },
            { "Mistlands_DvergrBossEntrance1", Heightmap.Biome.Mistlands },
            { "FaderLocation", Heightmap.Biome.AshLands },
            { "DN_Bossroom", Heightmap.Biome.DeepNorth },
            { "Mistlands_GuardTower1_new", Heightmap.Biome.Mistlands },
            { "Mistlands_GuardTower2_new", Heightmap.Biome.Mistlands },
            { "Mistlands_GuardTower3_new", Heightmap.Biome.Mistlands },
            { "Mistlands_Lighthouse1_new", Heightmap.Biome.Mistlands },
            { "Mistlands_Harbour1", Heightmap.Biome.Mistlands },
            { "Mistlands_Excavation1", Heightmap.Biome.Mistlands },
            { "Mistlands_Excavation2", Heightmap.Biome.Mistlands },
            { "Mistlands_Excavation3", Heightmap.Biome.Mistlands },
            { "Mistlands_GuardTower1_ruined_new", Heightmap.Biome.Mistlands },
            { "Mistlands_GuardTower1_ruined_new2", Heightmap.Biome.Mistlands },
            { "Mistlands_GuardTower3_ruined_new", Heightmap.Biome.Mistlands },
            { "Hildir_crypt", Heightmap.Biome.BlackForest },
            { "Hildir_cave", Heightmap.Biome.Mountain },
            { "Hildir_plainsfortress", Heightmap.Biome.Plains },
            { "BearCave", Heightmap.Biome.BlackForest },
            { "FireHole", Heightmap.Biome.Swamp },
            { "MorgenHole1", Heightmap.Biome.AshLands },
            { "MorgenHole2", Heightmap.Biome.AshLands },
            { "MorgenHole3", Heightmap.Biome.AshLands },
            { "NorthVillage", Heightmap.Biome.DeepNorth },
            { "AncientUpgradeStation", Heightmap.Biome.Mountain }
        };

        internal static bool IsShown(Heightmap.Biome itemBiomes, Heightmap.Biome currentBiome)
        {
            return (itemBiomes & currentBiome) != Heightmap.Biome.None;
        }

        internal static string DisplayName(Heightmap.Biome biome)
        {
            return Names.TryGetValue(biome, out string name) ? name : string.Empty;
        }

        internal static Heightmap.Biome ForResource(TrackedResourceDefinitionModel resource)
        {
            AlsoFoundIn.TryGetValue(resource.ResourcePrefabName, out Heightmap.Biome also);

            if (!string.IsNullOrWhiteSpace(resource.BiomeName))
            {
                foreach (var name in Names)
                {
                    if (name.Value == resource.BiomeName)
                    {
                        return name.Key | also;
                    }
                }
            }

            return (Resources.TryGetValue(resource.ResourcePrefabName, out Heightmap.Biome biome) ? biome : Heightmap.Biome.None) | also;
        }

        internal static Heightmap.Biome ForDungeon(Room.Theme theme)
        {
            return Dungeons.TryGetValue(theme, out Heightmap.Biome biome) ? biome : Heightmap.Biome.None;
        }

        internal static Heightmap.Biome ForCreature(string prefabName)
        {
            return Creatures.TryGetValue(prefabName, out Heightmap.Biome biome) ? biome : Heightmap.Biome.None;
        }

        internal static Heightmap.Biome ForLocations(IEnumerable<string> prefabNames)
        {
            Heightmap.Biome biomes = Heightmap.Biome.None;
            foreach (string prefabName in prefabNames)
            {
                if (Locations.TryGetValue(prefabName, out Heightmap.Biome biome))
                {
                    biomes |= biome;
                }
            }

            return biomes;
        }
    }
}
