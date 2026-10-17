
using HexResourceTracker.Core;
using HexResourceTracker.Models;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HexResourceTracker.UI
{
    internal static class ResourceTrackerMapOverlay
    {
        private const float PanelWidth = 370f;
        private const float ColumnWidth = 180f;
        private const float ColumnSpacing = 6f;
        private const float HeaderHeight = 28f;
        private const float SectionHeight = 24f;
        private const float ToggleHeight = 24f;
        private const float RowSpacing = 1f;
        private const float SectionSpacing = 6f;
        private const float ContentPadding = 6f;

        private static GameObject _panel;
        private static GameObject _content;
        private static RectTransform _panelRect;
        private static TextMeshProUGUI _collapseText;
        private static bool _isCollapsed;

        private static readonly Dictionary<string, Toggle> ResourceToggles = new Dictionary<string, Toggle>();
        private static readonly Dictionary<Room.Theme, Toggle> DungeonToggles = new Dictionary<Room.Theme, Toggle>();
        private static readonly Dictionary<string, Toggle> CreatureToggles = new Dictionary<string, Toggle>();

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

            ResourceToggles.Clear();
            DungeonToggles.Clear();
            CreatureToggles.Clear();

            _isCollapsed = false;

            _panel = new GameObject("HexResourceTrackerOverlay");
            _panel.transform.SetParent(Minimap.instance.m_largeRoot.transform, false);

            _panelRect = _panel.AddComponent<RectTransform>();
            _panelRect.anchorMin = new Vector2(1f, 1f);
            _panelRect.anchorMax = new Vector2(1f, 1f);
            _panelRect.pivot = new Vector2(1f, 1f);
            _panelRect.anchoredPosition = new Vector2(-20f, -50f);
            _panelRect.sizeDelta = new Vector2(PanelWidth, HeaderHeight);

            Image background = _panel.AddComponent<Image>();
            background.color = new Color(0.22f, 0.16f, 0.10f, 0.75f);

            AddTitle();

            _content = new GameObject("Content");
            _content.transform.SetParent(_panel.transform, false);

            RectTransform contentRect = _content.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0f, 1f);
            contentRect.anchorMax = new Vector2(1f, 1f);
            contentRect.pivot = new Vector2(0.5f, 1f);
            contentRect.anchoredPosition = new Vector2(0f, -HeaderHeight);
            contentRect.sizeDelta = new Vector2(0f, 0f);

            GameObject leftColumn = CreateColumn("ResourcesColumn", ContentPadding);
            GameObject rightColumn = CreateColumn("TrackingColumn", ContentPadding + ColumnWidth + ColumnSpacing);

            BuildResources(leftColumn);
            BuildDeposits(rightColumn);
            BuildDungeons(rightColumn);
            BuildCreatures(rightColumn);

            LayoutRebuilder.ForceRebuildLayoutImmediate(leftColumn.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(rightColumn.GetComponent<RectTransform>());

            float leftHeight = LayoutUtility.GetPreferredHeight(leftColumn.GetComponent<RectTransform>());
            float rightHeight = LayoutUtility.GetPreferredHeight(rightColumn.GetComponent<RectTransform>());

            float contentHeight = Mathf.Max(leftHeight, rightHeight) + ContentPadding * 2f;

            contentRect.sizeDelta = new Vector2(0f, contentHeight);
            _panelRect.sizeDelta = new Vector2(PanelWidth, HeaderHeight + contentHeight);
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

        internal static void HandleCreatureTrackingChanged(string prefabName, bool isEnabled)
        {
            if (CreatureToggles.TryGetValue(prefabName, out Toggle toggle) && toggle != null)
            {
                toggle.SetIsOnWithoutNotify(isEnabled);
            }
        }

        private static GameObject CreateColumn(string name, float xPosition)
        {
            GameObject column = new GameObject(name);
            column.transform.SetParent(_content.transform, false);

            RectTransform rect = column.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 1f);
            rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(xPosition, -ContentPadding);
            rect.sizeDelta = new Vector2(ColumnWidth, 0f);

            VerticalLayoutGroup layout = column.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.UpperLeft;
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.spacing = RowSpacing;
            layout.padding = new RectOffset(0, 0, 0, 0);

            ContentSizeFitter fitter = column.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            return column;
        }

        private static void BuildResources(GameObject parent)
        {
            AddSectionHeader(parent, "Resources");

            foreach (TrackedResourceDefinitionModel resource in TrackedResources.AllTrackedResources
                .Where(resource => resource.ResourceType != TrackedResourceTypeEnum.Deposit)
                .OrderBy(resource => resource.SortOrder))
            {
                AddResourceToggle(parent, resource.ResourcePrefabName, resource.DisplayName);
            }
        }

        private static void BuildDeposits(GameObject parent)
        {
            AddSectionHeader(parent, "Deposits");

            foreach (TrackedResourceDefinitionModel resource in TrackedResources.AllTrackedResources
                .Where(resource => resource.ResourceType == TrackedResourceTypeEnum.Deposit)
                .OrderBy(resource => resource.SortOrder))
            {
                AddResourceToggle(parent, resource.ResourcePrefabName, resource.DisplayName);
            }
        }

        private static void BuildDungeons(GameObject parent)
        {
            AddSectionHeader(parent, "Dungeons", SectionSpacing);

            AddDungeonToggle(parent, Room.Theme.ForestCrypt, "Burial Chambers");
            AddDungeonToggle(parent, Room.Theme.SunkenCrypt, "Sunken Crypts");
            AddDungeonToggle(parent, Room.Theme.Cave, "Frost Caves");
            AddDungeonToggle(parent, Room.Theme.DvergerTown, "Infested Mines");
            AddDungeonToggle(parent, Room.Theme.MorkHalla, "Morkhalla");
            AddDungeonToggle(parent, Room.Theme.Hole, "Winding Tunnels");
        }

        private static void BuildCreatures(GameObject parent)
        {
            AddSectionHeader(parent, "Creatures", SectionSpacing);

            foreach (TrackedCreatureDefinitionModel creature in TrackedCreatures.AllTrackedCreatures.OrderBy(creature => creature.SortOrder))
            {
                AddCreatureToggle(parent, creature.PrefabName, creature.DisplayName);
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
            titleRect.sizeDelta = new Vector2(0f, HeaderHeight);

            Image dragTarget = titleObject.AddComponent<Image>();
            dragTarget.color = new Color(0.10f, 0.07f, 0.04f, 0.95f);
            dragTarget.raycastTarget = true;

            titleObject.AddComponent<ResourceTrackerDragHandler>();

            GameObject titleTextObject = new GameObject("TitleText");
            titleTextObject.transform.SetParent(titleObject.transform, false);

            RectTransform titleTextRect = titleTextObject.AddComponent<RectTransform>();
            titleTextRect.anchorMin = Vector2.zero;
            titleTextRect.anchorMax = Vector2.one;
            titleTextRect.offsetMin = new Vector2(30f, 0f);
            titleTextRect.offsetMax = new Vector2(-30f, 0f);

            TextMeshProUGUI title = titleTextObject.AddComponent<TextMeshProUGUI>();
            title.font = Minimap.instance.m_biomeNameLarge.font;
            title.text = "Map Tracking";
            title.fontSize = 12f;
            title.alignment = TextAlignmentOptions.Center;
            title.color = Color.white;
            title.raycastTarget = false;

            AddCollapseButton(titleObject);
            AddSeparator(titleObject);
        }

        private static void AddCollapseButton(GameObject parent)
        {
            GameObject buttonObject = new GameObject("CollapseButton");
            buttonObject.transform.SetParent(parent.transform, false);

            RectTransform buttonRect = buttonObject.AddComponent<RectTransform>();
            buttonRect.anchorMin = new Vector2(1f, 0.5f);
            buttonRect.anchorMax = new Vector2(1f, 0.5f);
            buttonRect.pivot = new Vector2(1f, 0.5f);
            buttonRect.anchoredPosition = new Vector2(-4f, 0f);
            buttonRect.sizeDelta = new Vector2(26f, 24f);

            Image buttonImage = buttonObject.AddComponent<Image>();
            buttonImage.color = new Color(0.25f, 0.19f, 0.12f, 0.95f);

            Button button = buttonObject.AddComponent<Button>();
            button.targetGraphic = buttonImage;
            button.onClick.AddListener(ToggleCollapsed);

            GameObject textObject = new GameObject("Arrow");
            textObject.transform.SetParent(buttonObject.transform, false);

            RectTransform textRect = textObject.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            _collapseText = textObject.AddComponent<TextMeshProUGUI>();
            _collapseText.font = Minimap.instance.m_biomeNameLarge.font;
            _collapseText.text = "−";
            _collapseText.fontSize = 18f;
            _collapseText.alignment = TextAlignmentOptions.Center;
            _collapseText.color = Color.white;
            _collapseText.raycastTarget = false;
        }

        private static void ToggleCollapsed()
        {
            _isCollapsed = !_isCollapsed;

            if (_content != null)
            {
                _content.SetActive(!_isCollapsed);
            }

            if (_panelRect != null)
            {
                float contentHeight = _content.GetComponent<RectTransform>().sizeDelta.y;
                _panelRect.sizeDelta = new Vector2(PanelWidth, _isCollapsed ? HeaderHeight : HeaderHeight + contentHeight);
            }

            if (_collapseText != null)
            {
                _collapseText.text = _isCollapsed ? "+" : "−";
            }
        }

        private static void AddSectionHeader(GameObject parent, string text, float topSpacing = 0f)
        {
            GameObject headerObject = new GameObject($"{text}Header");
            headerObject.transform.SetParent(parent.transform, false);

            RectTransform headerRect = headerObject.AddComponent<RectTransform>();
            headerRect.sizeDelta = new Vector2(ColumnWidth, SectionHeight);

            LayoutElement layoutElement = headerObject.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = SectionHeight;
            layoutElement.minHeight = SectionHeight;

            if (topSpacing > 0f)
            {
                layoutElement.minHeight += topSpacing;
                layoutElement.preferredHeight += topSpacing;
            }

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
            headerText.raycastTarget = false;

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

        private static void AddResourceToggle(GameObject parent, string prefabName, string displayName)
        {
            Toggle toggle = CreateToggle(parent, $"{prefabName}_Toggle", displayName);
            bool isEnabled = PluginConfig.ResourceConfigs[prefabName].Value;

            toggle.SetIsOnWithoutNotify(isEnabled);
            ResourceToggles[prefabName] = toggle;

            toggle.onValueChanged.AddListener(delegate (bool value)
            {
                PluginConfig.ResourceConfigs[prefabName].Value = value;
            });
        }

        private static void AddDungeonToggle(GameObject parent, Room.Theme theme, string displayName)
        {
            Toggle toggle = CreateToggle(parent, $"{theme}_Toggle", displayName);
            bool isEnabled = PluginConfig.DungeonConfigs[theme].Value;

            toggle.SetIsOnWithoutNotify(isEnabled);
            DungeonToggles[theme] = toggle;

            toggle.onValueChanged.AddListener(delegate (bool value)
            {
                PluginConfig.DungeonConfigs[theme].Value = value;
            });
        }

        private static void AddCreatureToggle(GameObject parent, string prefabName, string displayName)
        {
            Toggle toggle = CreateToggle(parent, $"{prefabName}_Toggle", displayName);
            bool isEnabled = PluginConfig.CreatureConfigs[prefabName].Value;

            toggle.SetIsOnWithoutNotify(isEnabled);
            CreatureToggles[prefabName] = toggle;

            toggle.onValueChanged.AddListener(delegate (bool value)
            {
                PluginConfig.CreatureConfigs[prefabName].Value = value;
            });
        }

        private static Toggle CreateToggle(GameObject parent, string objectName, string displayName)
        {
            GameObject toggleObject = new GameObject(objectName);
            toggleObject.transform.SetParent(parent.transform, false);

            RectTransform toggleRect = toggleObject.AddComponent<RectTransform>();
            toggleRect.sizeDelta = new Vector2(ColumnWidth, ToggleHeight);

            LayoutElement layoutElement = toggleObject.AddComponent<LayoutElement>();
            layoutElement.preferredHeight = ToggleHeight;
            layoutElement.minHeight = ToggleHeight;

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
            label.raycastTarget = false;

            return toggle;
        }
    }
}
