using HexResourceTracker.Models;
using System.Collections.Generic;

namespace HexResourceTracker.Core
{
    internal static class TrackedResources
    {
        internal static readonly List<TrackedResourceDefinitionModel> AllTrackedResources = new List<TrackedResourceDefinitionModel>
        {
            // Meadows
            new TrackedResourceDefinitionModel("Pickable_Mushroom", "Mushrooms", 100, TrackedResourceTypeEnum.Pickable),
            new TrackedResourceDefinitionModel("Pickable_Dandelion", "Dandelions", 110, TrackedResourceTypeEnum.Pickable),
            new TrackedResourceDefinitionModel("RaspberryBush", "Raspberries", 120, TrackedResourceTypeEnum.Pickable),

            // Black Forest
            new TrackedResourceDefinitionModel("BlueberryBush", "Blueberries", 200, TrackedResourceTypeEnum.Pickable),
            new TrackedResourceDefinitionModel("Pickable_Thistle", "Thistle", 210, TrackedResourceTypeEnum.Pickable),
            new TrackedResourceDefinitionModel("Pickable_SeedCarrot", "Carrot Seeds", 220, TrackedResourceTypeEnum.Pickable),

            new TrackedResourceDefinitionModel(
                "rock4_copper",
                "Copper",
                230,
                TrackedResourceTypeEnum.Deposit,
                "CopperOre",
                "$piece_deposit_copper"),

            // Swamp
            new TrackedResourceDefinitionModel("Pickable_SeedTurnip", "Turnip Seeds", 300, TrackedResourceTypeEnum.Pickable),

            // Mountains
            new TrackedResourceDefinitionModel("Pickable_DragonEgg", "Dragon Eggs", 400, TrackedResourceTypeEnum.Pickable),
            new TrackedResourceDefinitionModel("OnionSeeds", "Onion Seeds", 410, TrackedResourceTypeEnum.Container),

            new TrackedResourceDefinitionModel(
                "silvervein",
                "Silver",
                420,
                TrackedResourceTypeEnum.Deposit,
                "SilverOre",
                "$piece_deposit_silvervein"),

            // Plains
            new TrackedResourceDefinitionModel("Pickable_Flax_Wild", "Flax", 500, TrackedResourceTypeEnum.Pickable),
            new TrackedResourceDefinitionModel("Pickable_Barley_Wild", "Barley", 510, TrackedResourceTypeEnum.Pickable),
            new TrackedResourceDefinitionModel("CloudberryBush", "Cloudberries", 520, TrackedResourceTypeEnum.Pickable),

            // Mistlands
            new TrackedResourceDefinitionModel("Pickable_Mushroom_JotunPuffs", "Jotun Puffs", 600, TrackedResourceTypeEnum.Pickable),
            new TrackedResourceDefinitionModel("Pickable_Mushroom_Magecap", "Magecap", 610, TrackedResourceTypeEnum.Pickable),

            new TrackedResourceDefinitionModel(
                "giant_skull",
                "Giant Skull",
                620,
                TrackedResourceTypeEnum.Deposit,
                "Softtissue",
                "$piece_giant_bone"),

            // Ashlands
            new TrackedResourceDefinitionModel("VineAsh", "Vineberries", 700, TrackedResourceTypeEnum.Pickable),
            new TrackedResourceDefinitionModel("Pickable_SmokePuff", "Smoke Puffs", 710, TrackedResourceTypeEnum.Pickable),
            new TrackedResourceDefinitionModel("Pickable_Fiddlehead", "Fiddleheads", 720, TrackedResourceTypeEnum.Pickable),

            new TrackedResourceDefinitionModel(
                "LeviathanLava",
                "Flametal",
                730,
                TrackedResourceTypeEnum.Deposit,
                "FlametalOreNew",
                null,
                "$item_flametalore"),

            // Deep North
            new TrackedResourceDefinitionModel("LingonberryBush", "Lingonberries", 800, TrackedResourceTypeEnum.Pickable),
            new TrackedResourceDefinitionModel("Pickable_SeedKale", "Kale Seeds", 810, TrackedResourceTypeEnum.Pickable)
        };

        private static readonly Dictionary<string, TrackedResourceDefinitionModel> ByPrefabName = new Dictionary<string, TrackedResourceDefinitionModel>();
        private static readonly Dictionary<string, TrackedResourceDefinitionModel> ByMineRock5Name = new Dictionary<string, TrackedResourceDefinitionModel>();
        private static readonly Dictionary<string, TrackedResourceDefinitionModel> ByMineRockName = new Dictionary<string, TrackedResourceDefinitionModel>();

        static TrackedResources()
        {
            BuildResourceLookups();
        }

        internal static bool TryGetByPrefabName(string prefabName, out TrackedResourceDefinitionModel definition)
        {
            return ByPrefabName.TryGetValue(prefabName, out definition);
        }

        internal static bool TryGetByMineRock5(string mineRockName, out TrackedResourceDefinitionModel definition)
        {
            return ByMineRock5Name.TryGetValue(mineRockName, out definition);
        }

        internal static bool TryGetByMineRock(string mineRockName, out TrackedResourceDefinitionModel definition)
        {
            return ByMineRockName.TryGetValue(mineRockName, out definition);
        }

        private static void BuildResourceLookups()
        {
            foreach (TrackedResourceDefinitionModel resource in AllTrackedResources)
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