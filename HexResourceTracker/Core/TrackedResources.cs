using HexResourceTracker.Models;
using System.Collections.Generic;

namespace HexResourceTracker.Core
{
    internal static class TrackedResources
    {
        internal static readonly List<TrackedResourceDefinition> AllTrackedResources = new List<TrackedResourceDefinition>
        {
            // Meadows
            new TrackedResourceDefinition("Pickable_Mushroom", "Mushrooms", 100, TrackedResourceTypeEnum.Pickable),
            new TrackedResourceDefinition("Pickable_Dandelion", "Dandelions", 110, TrackedResourceTypeEnum.Pickable),
            new TrackedResourceDefinition("RaspberryBush", "Raspberries", 120, TrackedResourceTypeEnum.Pickable),

            // Black Forest
            new TrackedResourceDefinition("BlueberryBush", "Blueberries", 200, TrackedResourceTypeEnum.Pickable),
            new TrackedResourceDefinition("Pickable_Thistle", "Thistle", 210, TrackedResourceTypeEnum.Pickable),
            new TrackedResourceDefinition("Pickable_SeedCarrot", "Carrot Seeds", 220, TrackedResourceTypeEnum.Pickable),

            new TrackedResourceDefinition(
                "rock4_copper",
                "Copper",
                230,
                TrackedResourceTypeEnum.Deposit,
                "CopperOre",
                "$piece_deposit_copper"),

            // Swamp
            new TrackedResourceDefinition("Pickable_SeedTurnip", "Turnip Seeds", 300, TrackedResourceTypeEnum.Pickable),

            // Mountains
            new TrackedResourceDefinition("Pickable_DragonEgg", "Dragon Eggs", 400, TrackedResourceTypeEnum.Pickable),
            new TrackedResourceDefinition("OnionSeeds", "Onion Seeds", 410, TrackedResourceTypeEnum.Container),

            new TrackedResourceDefinition(
                "silvervein",
                "Silver",
                420,
                TrackedResourceTypeEnum.Deposit,
                "SilverOre",
                "$piece_deposit_silvervein"),

            // Plains
            new TrackedResourceDefinition("Pickable_Flax_Wild", "Flax", 500, TrackedResourceTypeEnum.Pickable),
            new TrackedResourceDefinition("Pickable_Barley_Wild", "Barley", 510, TrackedResourceTypeEnum.Pickable),
            new TrackedResourceDefinition("CloudberryBush", "Cloudberries", 520, TrackedResourceTypeEnum.Pickable),

            // Mistlands
            new TrackedResourceDefinition("Pickable_Mushroom_JotunPuffs", "Jotun Puffs", 600, TrackedResourceTypeEnum.Pickable),
            new TrackedResourceDefinition("Pickable_Mushroom_Magecap", "Magecap", 610, TrackedResourceTypeEnum.Pickable),

            new TrackedResourceDefinition(
                "giant_skull",
                "Giant Skull",
                620,
                TrackedResourceTypeEnum.Deposit,
                "Softtissue",
                "$piece_giant_bone"),

            // Ashlands
            new TrackedResourceDefinition("VineAsh", "Vineberries", 700, TrackedResourceTypeEnum.Pickable),
            new TrackedResourceDefinition("Pickable_SmokePuff", "Smoke Puffs", 710, TrackedResourceTypeEnum.Pickable),
            new TrackedResourceDefinition("Pickable_Fiddlehead", "Fiddleheads", 720, TrackedResourceTypeEnum.Pickable),

            new TrackedResourceDefinition(
                "LeviathanLava",
                "Flametal",
                730,
                TrackedResourceTypeEnum.Deposit,
                "FlametalOreNew",
                null,
                "$item_flametalore"),

            // Deep North
            new TrackedResourceDefinition("LingonberryBush", "Lingonberries", 800, TrackedResourceTypeEnum.Pickable),
            new TrackedResourceDefinition("Pickable_SeedKale", "Kale Seeds", 810, TrackedResourceTypeEnum.Pickable)
        };

        private static readonly Dictionary<string, TrackedResourceDefinition> ByPrefabName = new Dictionary<string, TrackedResourceDefinition>();
        private static readonly Dictionary<string, TrackedResourceDefinition> ByMineRock5Name = new Dictionary<string, TrackedResourceDefinition>();
        private static readonly Dictionary<string, TrackedResourceDefinition> ByMineRockName = new Dictionary<string, TrackedResourceDefinition>();

        static TrackedResources()
        {
            BuildResourceLookups();
        }

        internal static bool TryGetPrefabName(string prefabName, out TrackedResourceDefinition definition)
        {
            return ByPrefabName.TryGetValue(prefabName, out definition);
        }

        internal static bool TryGetMineRock5(string mineRockName, out TrackedResourceDefinition definition)
        {
            return ByMineRock5Name.TryGetValue(mineRockName, out definition);
        }

        internal static bool TryGetMineRock(string mineRockName, out TrackedResourceDefinition definition)
        {
            return ByMineRockName.TryGetValue(mineRockName, out definition);
        }

        private static void BuildResourceLookups()
        {
            foreach (TrackedResourceDefinition resource in AllTrackedResources)
            {
                ByPrefabName[resource.ResourcePrefabName] = resource;

                if (!string.IsNullOrWhiteSpace(resource.MineRock5Name))
                {
                    ByMineRock5Name[resource.MineRock5Name] = resource;
                }

                if (!string.IsNullOrWhiteSpace(resource.MineRockName))
                {
                    ByMineRockName[resource.MineRockName] = resource;
                }
            }
        }
    }
}