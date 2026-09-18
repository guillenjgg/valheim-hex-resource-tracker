using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HexResourceTracker.Core
{
    internal static class ResourceTrackerMapOverlay
    {
        private static GameObject _panel;

        private static readonly Dictionary<string, Toggle> ResourceToggles = new Dictionary<string, Toggle>();
        private static readonly Dictionary<Room.Theme, Toggle> DungeonToggles = new Dictionary<Room.Theme, Toggle>();

        internal static void Create()
        {
            if (_panel != null)
            {
                return;
            }

            if (Minimap.instance == null || Minimap.instance.m_largeRoot == null)
            {
                return;
            }

            _panel = new GameObject("HexResourceTrackerOverlay");
            _panel.transform.SetParent(Minimap.instance.m_largeRoot.transform, false);

            RectTransform panelRect = _panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(1f, 1f);
            panelRect.anchorMax = new Vector2(1f, 1f);
            panelRect.pivot = new Vector2(1f, 1f);
            panelRect.anchoredPosition = new Vector2(-20f, -50f);
            panelRect.sizeDelta = new Vector2(180f, 860f);

            Image background = _panel.AddComponent<Image>();
            background.color = new Color(0.22f, 0.16f, 0.10f, 0.75f);

            AddTitle();

            // Resources
            AddSectionHeader("Resources", -32f);

            // Meadows
            AddResourceToggle("Pickable_Mushroom", "Mushrooms", -60f);
            AddResourceToggle("Pickable_Dandelion", "Dandelions", -85f);
            AddResourceToggle("RaspberryBush", "Raspberries", -110f);

            // Black Forest
            AddResourceToggle("BlueberryBush", "Blueberries", -135f);
            AddResourceToggle("Pickable_Thistle", "Thistle", -160f);
            AddResourceToggle("Pickable_SeedCarrot", "Carrot Seeds", -185f);

            // Swamp
            AddResourceToggle("Pickable_SeedTurnip", "Turnip Seeds", -210f);

            // Mountains
            AddResourceToggle("Pickable_DragonEgg", "Dragon Eggs", -235f);

            // Plains
            AddResourceToggle("Pickable_Flax_Wild", "Flax", -260f);
            AddResourceToggle("Pickable_Barley_Wild", "Barley", -285f);
            AddResourceToggle("CloudberryBush", "Cloudberries", -310f);

            // Mistlands
            AddResourceToggle("Pickable_Mushroom_JotunPuffs", "Jotun Puffs", -335f);
            AddResourceToggle("Pickable_Mushroom_Magecap", "Magecap", -360f);

            // Ashlands
            AddResourceToggle("VineAsh", "Vineberries", -385f);
            AddResourceToggle("Pickable_SmokePuff", "Smoke Puffs", -410f);
            AddResourceToggle("Pickable_Fiddlehead", "Fiddleheads", -435f);

            // Deep North
            AddResourceToggle("LingonberryBush", "Lingonberries", -460f);
            AddResourceToggle("Pickable_SeedKale", "Kale Seeds", -485f);

            // Deposits
            AddSectionHeader("Deposits", -515f);

            // Black Forest
            AddResourceToggle("rock4_copper", "Copper", -543f);

            // Mountains
            AddResourceToggle("silvervein", "Silver", -568f);

            // Mistlands
            AddResourceToggle("giant_skull", "Giant Skull", -593f);

            // Ashlands
            AddResourceToggle("LeviathanLava", "Flametal", -618f);

            // Dungeons
            AddSectionHeader("Dungeons", -648f);

            AddDungeonToggle(Room.Theme.ForestCrypt, "Burial Chambers", -676f);
            AddDungeonToggle(Room.Theme.SunkenCrypt, "Sunken Crypts", -701f);
            AddDungeonToggle(Room.Theme.Cave, "Frost Caves", -726f);
            AddDungeonToggle(Room.Theme.DvergerTown, "Infested Mines", -751f);
            AddDungeonToggle(Room.Theme.MorkHalla, "Morkhalla", -776f);
            AddDungeonToggle(Room.Theme.Hole, "Winding Tunnels", -801f);
        }

        internal static void HandleResourceTrackingChanged(string prefabName, bool isEnabled)
        {
            if (string.IsNullOrWhiteSpace(prefabName))
            {
                return;
            }

            if (ResourceToggles.TryGetValue(prefabName, out Toggle toggle) && toggle != null)
            {
                toggle.SetIsOnWithoutNotify(isEnabled);
            }
        }

        internal static void HandleDungeonTrackingChanged(Room.Theme theme, bool isEnabled)
        {
            if (DungeonToggles.TryGetValue(theme, out Toggle toggle) && toggle != null)
            {
                toggle.SetIsOnWithoutNotify(isEnabled);
            }
        }

        private static void AddTitle()
        {
            GameObject titleObject = new GameObject("Title");
            titleObject.transform.SetParent(_panel.transform, false);

            RectTransform titleRect = titleObject.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0f, 1f);
            titleRect.anchorMax = new Vector2(1f, 1f);
            titleRect.pivot = new Vector2(0.5f, 1f);
            titleRect.anchoredPosition = Vector2.zero;
            titleRect.sizeDelta = new Vector2(0f, 28f);

            Image dragTarget = titleObject.AddComponent<Image>();
            dragTarget.color = new Color(0.10f, 0.07f, 0.04f, 0.95f);
            dragTarget.raycastTarget = true;

            titleObject.AddComponent<ResourceTrackerDragHandler>();

            GameObject titleTextObject = new GameObject("TitleText");
            titleTextObject.transform.SetParent(titleObject.transform, false);

            RectTransform titleTextRect = titleTextObject.AddComponent<RectTransform>();
            titleTextRect.anchorMin = Vector2.zero;
            titleTextRect.anchorMax = Vector2.one;
            titleTextRect.offsetMin = Vector2.zero;
            titleTextRect.offsetMax = Vector2.zero;

            TextMeshProUGUI title = titleTextObject.AddComponent<TextMeshProUGUI>();
            title.font = Minimap.instance.m_biomeNameLarge.font;
            title.text = "Map Tracking";
            title.fontSize = 12f;
            title.alignment = TextAlignmentOptions.Center;
            title.color = Color.white;

            AddSeparator(titleObject);
        }

        private static void AddSectionHeader(string text, float yPosition)
        {
            GameObject headerObject = new GameObject($"{text}Header");
            headerObject.transform.SetParent(_panel.transform, false);

            RectTransform headerRect = headerObject.AddComponent<RectTransform>();
            headerRect.anchorMin = new Vector2(0f, 1f);
            headerRect.anchorMax = new Vector2(1f, 1f);
            headerRect.pivot = new Vector2(0.5f, 1f);
            headerRect.anchoredPosition = new Vector2(0f, yPosition);
            headerRect.sizeDelta = new Vector2(0f, 24f);

            Image headerBackground = headerObject.AddComponent<Image>();
            headerBackground.color = new Color(0.14f, 0.10f, 0.06f, 0.90f);
            headerBackground.raycastTarget = false;

            GameObject textObject = new GameObject("Text");
            textObject.transform.SetParent(headerObject.transform, false);

            RectTransform textRect = textObject.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            TextMeshProUGUI headerText = textObject.AddComponent<TextMeshProUGUI>();
            headerText.font = Minimap.instance.m_biomeNameLarge.font;
            headerText.text = text;
            headerText.fontSize = 12f;
            headerText.alignment = TextAlignmentOptions.Center;
            headerText.color = Color.white;

            AddSeparator(headerObject);
        }

        private static void AddSeparator(GameObject parent)
        {
            GameObject separator = new GameObject("Separator");
            separator.transform.SetParent(parent.transform, false);

            RectTransform separatorRect = separator.AddComponent<RectTransform>();
            separatorRect.anchorMin = new Vector2(0f, 0f);
            separatorRect.anchorMax = new Vector2(1f, 0f);
            separatorRect.pivot = new Vector2(0.5f, 0f);
            separatorRect.sizeDelta = new Vector2(0f, 2f);

            Image separatorImage = separator.AddComponent<Image>();
            separatorImage.color = new Color(0.6f, 0.5f, 0.3f, 0.8f);
            separatorImage.raycastTarget = false;
        }

        private static void AddResourceToggle(string prefabName, string displayName, float yPosition)
        {
            Toggle toggle = CreateToggle($"{prefabName}_Toggle", displayName, yPosition);
            bool isEnabled = PluginConfig.ResourceConfigs[prefabName].Value;

            toggle.SetIsOnWithoutNotify(isEnabled);
            ResourceToggles[prefabName] = toggle;

            toggle.onValueChanged.AddListener(delegate (bool value)
            {
                PluginConfig.ResourceConfigs[prefabName].Value = value;
            });
        }

        private static void AddDungeonToggle(Room.Theme theme, string displayName, float yPosition)
        {
            Toggle toggle = CreateToggle($"{theme}_Toggle", displayName, yPosition);
            bool isEnabled = PluginConfig.DungeonConfigs[theme].Value;

            toggle.SetIsOnWithoutNotify(isEnabled);
            DungeonToggles[theme] = toggle;

            toggle.onValueChanged.AddListener(delegate (bool value)
            {
                PluginConfig.DungeonConfigs[theme].Value = value;
            });
        }

        private static Toggle CreateToggle(string objectName, string displayName, float yPosition)
        {
            GameObject toggleObject = new GameObject(objectName);
            toggleObject.transform.SetParent(_panel.transform, false);

            RectTransform toggleRect = toggleObject.AddComponent<RectTransform>();
            toggleRect.anchorMin = new Vector2(0f, 1f);
            toggleRect.anchorMax = new Vector2(1f, 1f);
            toggleRect.pivot = new Vector2(0.5f, 1f);
            toggleRect.anchoredPosition = new Vector2(0f, yPosition);
            toggleRect.sizeDelta = new Vector2(0f, 24f);

            Toggle toggle = toggleObject.AddComponent<Toggle>();

            GameObject backgroundObject = new GameObject("Background");
            backgroundObject.transform.SetParent(toggleObject.transform, false);

            RectTransform backgroundRect = backgroundObject.AddComponent<RectTransform>();
            backgroundRect.anchorMin = new Vector2(0f, 0.5f);
            backgroundRect.anchorMax = new Vector2(0f, 0.5f);
            backgroundRect.pivot = new Vector2(0f, 0.5f);
            backgroundRect.anchoredPosition = new Vector2(12f, 0f);
            backgroundRect.sizeDelta = new Vector2(14f, 14f);

            Image backgroundImage = backgroundObject.AddComponent<Image>();
            backgroundImage.color = Color.white;

            GameObject checkmarkObject = new GameObject("Checkmark");
            checkmarkObject.transform.SetParent(backgroundObject.transform, false);

            RectTransform checkmarkRect = checkmarkObject.AddComponent<RectTransform>();
            checkmarkRect.anchorMin = Vector2.zero;
            checkmarkRect.anchorMax = Vector2.one;
            checkmarkRect.offsetMin = new Vector2(3f, 3f);
            checkmarkRect.offsetMax = new Vector2(-3f, -3f);

            Image checkmarkImage = checkmarkObject.AddComponent<Image>();
            checkmarkImage.color = Color.green;

            toggle.targetGraphic = backgroundImage;
            toggle.graphic = checkmarkImage;

            GameObject labelObject = new GameObject("Label");
            labelObject.transform.SetParent(toggleObject.transform, false);

            RectTransform labelRect = labelObject.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(34f, 0f);
            labelRect.offsetMax = new Vector2(-10f, 0f);

            TextMeshProUGUI label = labelObject.AddComponent<TextMeshProUGUI>();
            label.font = Minimap.instance.m_biomeNameLarge.font;
            label.text = displayName;
            label.fontSize = 14f;
            label.alignment = TextAlignmentOptions.Left;
            label.color = Color.white;

            return toggle;
        }
    }
}