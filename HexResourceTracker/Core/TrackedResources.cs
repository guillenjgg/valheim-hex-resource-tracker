using HexResourceTracker.Models;
using System.Collections.Generic;

namespace HexResourceTracker.Core
{
    internal static class TrackedResources
    {
        internal static readonly List<TrackedResourceDefinitionModel> AllTrackedResources = new List<TrackedResourceDefinitionModel>
        {
            // Meadows
            new TrackedResourceDefinitionModel("Pickable_Mushroom", "Mushrooms", 100, TrackedResourceTypeEnum.Pickable, biomeName: "Meadows"),
            new TrackedResourceDefinitionModel("Pickable_Dandelion", "Dandelions", 110, TrackedResourceTypeEnum.Pickable, biomeName: "Meadows"),
            new TrackedResourceDefinitionModel("RaspberryBush", "Raspberries", 120, TrackedResourceTypeEnum.Pickable, biomeName: "Meadows"),

            new TrackedResourceDefinitionModel("Beehive", "Beehive", 130, TrackedResourceTypeEnum.Deposit, "Honey"),

            // Black Forest
            new TrackedResourceDefinitionModel("BlueberryBush", "Blueberries", 200, TrackedResourceTypeEnum.Pickable, biomeName: "Black Forest"),
            new TrackedResourceDefinitionModel("Pickable_Thistle", "Thistle", 210, TrackedResourceTypeEnum.Pickable, biomeName: "Black Forest"),
            new TrackedResourceDefinitionModel("Pickable_SeedCarrot", "Carrot Seeds", 220, TrackedResourceTypeEnum.Pickable, biomeName: "Black Forest"),

            new TrackedResourceDefinitionModel(
                "rock4_copper",
                "Copper",
                230,
                TrackedResourceTypeEnum.Deposit,
                "CopperOre",
                "$piece_deposit_copper"),

            new TrackedResourceDefinitionModel("MineRock_Tin", "Tin", 240, TrackedResourceTypeEnum.Deposit, "TinOre"),
            new TrackedResourceDefinitionModel("Spawner_GreydwarfNest", "Greydwarf Nest", 250, TrackedResourceTypeEnum.Deposit, "AncientSeed"),

            // Swamp
            new TrackedResourceDefinitionModel("Pickable_SeedTurnip", "Turnip Seeds", 300, TrackedResourceTypeEnum.Pickable, biomeName: "Swamp"),

            new TrackedResourceDefinitionModel(
                "mudpile",
                "Muddy Scrap Pile",
                310,
                TrackedResourceTypeEnum.Deposit,
                "IronScrap",
                alsoPrefabNames: new[] { "mudpile2", "mudpile_beacon" }),

            // Mountains
            new TrackedResourceDefinitionModel("Pickable_DragonEgg", "Dragon Eggs", 400, TrackedResourceTypeEnum.Pickable, biomeName: "Mountains"),
            new TrackedResourceDefinitionModel("OnionSeeds", "Onion Seeds", 410, TrackedResourceTypeEnum.Container, biomeName: "Mountains"),

            new TrackedResourceDefinitionModel(
                "silvervein",
                "Silver",
                420,
                TrackedResourceTypeEnum.Deposit,
                "SilverOre",
                "$piece_deposit_silvervein"),

            new TrackedResourceDefinitionModel("MineRock_Obsidian", "Obsidian", 430, TrackedResourceTypeEnum.Deposit, "Obsidian"),

            new TrackedResourceDefinitionModel("GuckSack", "Guck Sack", 320, TrackedResourceTypeEnum.Deposit, "Guck", alsoPrefabNames: new[] { "GuckSack_small" }),

            // Plains
            new TrackedResourceDefinitionModel("Pickable_Flax_Wild", "Flax", 500, TrackedResourceTypeEnum.Pickable, biomeName: "Plains"),
            new TrackedResourceDefinitionModel("Pickable_Barley_Wild", "Barley", 510, TrackedResourceTypeEnum.Pickable, biomeName: "Plains"),
            new TrackedResourceDefinitionModel("CloudberryBush", "Cloudberries", 520, TrackedResourceTypeEnum.Pickable, biomeName: "Plains"),

            new TrackedResourceDefinitionModel(TarPitResourcePinService.ResourcePrefabName, "Tar Pit", 530, TrackedResourceTypeEnum.Deposit, "Tar"),

            new TrackedResourceDefinitionModel("GoblinTotem", "Fuling Totem", 540, TrackedResourceTypeEnum.Pickable, showInLocations: true, biomeName: "Plains"),

            // Mistlands
            new TrackedResourceDefinitionModel("Pickable_Mushroom_JotunPuffs", "Jotun Puffs", 600, TrackedResourceTypeEnum.Pickable, biomeName: "Mistlands"),
            new TrackedResourceDefinitionModel("Pickable_Mushroom_Magecap", "Magecap", 610, TrackedResourceTypeEnum.Pickable, biomeName: "Mistlands"),

            new TrackedResourceDefinitionModel(
                "giant_skull",
                "Giant Skull",
                620,
                TrackedResourceTypeEnum.Deposit,
                "Softtissue",
                "$piece_giant_bone"),

            new TrackedResourceDefinitionModel("giant_sword1", "Ancient Sword", 630, TrackedResourceTypeEnum.Deposit, "IronScrap", alsoPrefabNames: new[] { "giant_sword2" }),
            // Found through their ResourceRoot component (the roots a Sap Extractor taps), not by prefab name.
            new TrackedResourceDefinitionModel(YggdrasilRootPinService.ResourcePrefabName, "Yggdrasil Root", 650, TrackedResourceTypeEnum.Deposit, "Sap"),
            new TrackedResourceDefinitionModel("giant_helmet1", "Ancient Armour", 640, TrackedResourceTypeEnum.Deposit, "IronScrap", alsoPrefabNames: new[] { "giant_helmet2" }),

            // Ashlands
            new TrackedResourceDefinitionModel("VineAsh", "Vineberries", 700, TrackedResourceTypeEnum.Pickable, biomeName: "Ashlands"),
            new TrackedResourceDefinitionModel("Pickable_SmokePuff", "Smoke Puffs", 710, TrackedResourceTypeEnum.Pickable, biomeName: "Ashlands"),
            new TrackedResourceDefinitionModel("Pickable_Fiddlehead", "Fiddleheads", 720, TrackedResourceTypeEnum.Pickable, biomeName: "Ashlands"),

            new TrackedResourceDefinitionModel(
                "LeviathanLava",
                "Flametal",
                730,
                TrackedResourceTypeEnum.Deposit,
                "FlametalOreNew",
                null,
                "$item_flametalore"),

            // Deep North
            new TrackedResourceDefinitionModel("LingonberryBush", "Lingonberries", 800, TrackedResourceTypeEnum.Pickable, biomeName: "Deep North"),
            new TrackedResourceDefinitionModel("Pickable_SeedKale", "Kale Seeds", 810, TrackedResourceTypeEnum.Pickable, biomeName: "Deep North"),
            new TrackedResourceDefinitionModel("OatSeeds", "Oat Seeds", 820, TrackedResourceTypeEnum.Container, biomeName: "Deep North"),
            new TrackedResourceDefinitionModel("PoteitrSeeds", "Potato Seeds", 830, TrackedResourceTypeEnum.Container, biomeName: "Deep North"),
            // Chest-only loot (Friday's chest-only report, 2026-11-24; Markus, 2026-11-26): a chest is pinned while it holds one.
            new TrackedResourceDefinitionModel("AxeHead1", "Curious Axe Head", 140, TrackedResourceTypeEnum.Container, biomeName: "Meadows"),
            new TrackedResourceDefinitionModel("AxeHead2", "Mysterious Axe Head", 141, TrackedResourceTypeEnum.Container, biomeName: "Meadows"),
            new TrackedResourceDefinitionModel("AxeWood", "Wooden Axe", 150, TrackedResourceTypeEnum.Container, biomeName: "Meadows"),
            new TrackedResourceDefinitionModel("KnifeWood", "Wooden Knife", 151, TrackedResourceTypeEnum.Container, biomeName: "Meadows"),
            new TrackedResourceDefinitionModel("MaceWood", "Wooden Mace", 152, TrackedResourceTypeEnum.Container, biomeName: "Meadows"),
            new TrackedResourceDefinitionModel("SpearWood", "Wooden Spear", 260, TrackedResourceTypeEnum.Container, biomeName: "Black Forest"),
            new TrackedResourceDefinitionModel("SledgeWood", "Wooden Sledge", 330, TrackedResourceTypeEnum.Container, biomeName: "Swamp"),
            new TrackedResourceDefinitionModel("BattleaxeWood", "Wooden Battleaxe", 440, TrackedResourceTypeEnum.Container, biomeName: "Mountains"),
            new TrackedResourceDefinitionModel("AtgeirWood", "Wooden Atgeir", 550, TrackedResourceTypeEnum.Container, biomeName: "Plains"),
            new TrackedResourceDefinitionModel("THSwordWood", "Wooden Greatsword", 655, TrackedResourceTypeEnum.Container, biomeName: "Mistlands"),

            // Every mould (Nord weapons, armour, the Intricate Key) shares one toggle; the pin shows the mould found.
            new TrackedResourceDefinitionModel(MouldsKey, "Moulds", 840, TrackedResourceTypeEnum.Container, biomeName: "Deep North"),

            // Ocean
            new TrackedResourceDefinitionModel("Leviathan", "Leviathan", 900, TrackedResourceTypeEnum.Deposit, "Chitin")
        };

        // Every world-placed container (chests, barrels, granaries) is looked inside for tracked items; player-built
        // chests and graves never are. This replaced a list of two chest prefabs, which missed the chest-only loot.
        internal static bool IsWorldContainer(bool isPlayerBuilt, bool isGrave)
        {
            return !isPlayerBuilt && !isGrave;
        }

        internal const string MouldsKey = "Moulds";
        private const string MouldTokenPrefix = "$item_mold_";

        // The tracked resource a container item counts towards: its own prefab name, or Moulds for any mould.
        internal static string ContainerKeyForItem(string itemPrefabName, string itemNameToken)
        {
            if (!string.IsNullOrEmpty(itemPrefabName) && ByPrefabName.ContainsKey(itemPrefabName))
            {
                return itemPrefabName;
            }

            if (!string.IsNullOrEmpty(itemNameToken) && itemNameToken.StartsWith(MouldTokenPrefix))
            {
                return MouldsKey;
            }

            return null;
        }

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

                foreach (string alsoPrefabName in resource.AlsoPrefabNames)
                {
                    ByPrefabName[alsoPrefabName] = resource;
                }

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
