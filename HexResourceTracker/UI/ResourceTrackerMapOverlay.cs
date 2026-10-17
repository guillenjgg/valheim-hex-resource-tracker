using HexResourceTracker.Core;
using HexResourceTracker.Models;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HexResourceTracker.UI
{
    internal static class ResourceTrackerMapOverlay
    {
        private const float PanelWidth = 370f;
        private const float ColumnSpacing = 6f;
        private const float HeaderHeight = 28f;
        private const float SectionSpacing = 6f;

        private static GameObject _panel;
        private static GameObject _content;
        private static RectTransform _panelRect;
        private static TextMeshProUGUI _collapseText;
        private static bool _isCollapsed;

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

            ResourceTrackerOverlayState.Clear();

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

            _collapseText = ResourceTrackerOverlayUiFactory.CreateTitle(_panel, HeaderHeight, ToggleCollapsed);

            _content = new GameObject("Content");
            _content.transform.SetParent(_panel.transform, false);

            RectTransform contentRect = _content.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0f, 1f);
            contentRect.anchorMax = new Vector2(1f, 1f);
            contentRect.pivot = new Vector2(0.5f, 1f);
            contentRect.anchoredPosition = new Vector2(0f, -HeaderHeight);
            contentRect.sizeDelta = new Vector2(0f, 0f);

            float columnWidth = ResourceTrackerOverlayUiFactory.ColumnWidth;
            float contentPadding = ResourceTrackerOverlayUiFactory.ContentPadding;

            GameObject leftColumn = ResourceTrackerOverlayUiFactory.CreateColumn(
                _content, "ResourcesColumn", contentPadding);

            GameObject rightColumn = ResourceTrackerOverlayUiFactory.CreateColumn(
                _content, "TrackingColumn", contentPadding + columnWidth + ColumnSpacing);

            BuildResources(leftColumn);
            BuildDeposits(rightColumn);
            BuildDungeons(rightColumn);
            BuildCreatures(rightColumn);

            LayoutRebuilder.ForceRebuildLayoutImmediate(leftColumn.GetComponent<RectTransform>());
            LayoutRebuilder.ForceRebuildLayoutImmediate(rightColumn.GetComponent<RectTransform>());

            float leftHeight = LayoutUtility.GetPreferredHeight(leftColumn.GetComponent<RectTransform>());
            float rightHeight = LayoutUtility.GetPreferredHeight(rightColumn.GetComponent<RectTransform>());

            float contentHeight = Mathf.Max(leftHeight, rightHeight) + contentPadding * 2f;

            contentRect.sizeDelta = new Vector2(0f, contentHeight);
            _panelRect.sizeDelta = new Vector2(PanelWidth, HeaderHeight + contentHeight);
        }

        private static void BuildResources(GameObject parent)
        {
            ResourceTrackerOverlayUiFactory.AddSectionHeader(parent, "Resources");

            foreach (TrackedResourceDefinitionModel resource in TrackedResources.AllTrackedResources
                .Where(resource => resource.ResourceType != TrackedResourceTypeEnum.Deposit)
                .OrderBy(resource => resource.SortOrder))
            {
                AddResourceToggle(parent, resource.ResourcePrefabName, resource.DisplayName);
            }
        }

        private static void BuildDeposits(GameObject parent)
        {
            ResourceTrackerOverlayUiFactory.AddSectionHeader(parent, "Deposits");

            foreach (TrackedResourceDefinitionModel resource in TrackedResources.AllTrackedResources
                .Where(resource => resource.ResourceType == TrackedResourceTypeEnum.Deposit)
                .OrderBy(resource => resource.SortOrder))
            {
                AddResourceToggle(parent, resource.ResourcePrefabName, resource.DisplayName);
            }
        }

        private static void BuildDungeons(GameObject parent)
        {
            ResourceTrackerOverlayUiFactory.AddSectionHeader(parent, "Dungeons", SectionSpacing);

            AddDungeonToggle(parent, Room.Theme.ForestCrypt, "Burial Chambers");
            AddDungeonToggle(parent, Room.Theme.SunkenCrypt, "Sunken Crypts");
            AddDungeonToggle(parent, Room.Theme.Cave, "Frost Caves");
            AddDungeonToggle(parent, Room.Theme.DvergerTown, "Infested Mines");
            AddDungeonToggle(parent, Room.Theme.MorkHalla, "Morkhalla");
            AddDungeonToggle(parent, Room.Theme.Hole, "Winding Tunnels");
        }

        private static void BuildCreatures(GameObject parent)
        {
            ResourceTrackerOverlayUiFactory.AddSectionHeader(parent, "Creatures", SectionSpacing);

            foreach (TrackedCreatureDefinitionModel creature in TrackedCreatures.AllTrackedCreatures.OrderBy(creature => creature.SortOrder))
            {
                AddCreatureToggle(parent, creature.PrefabName, creature.DisplayName);
            }
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

        private static void AddResourceToggle(GameObject parent, string prefabName, string displayName)
        {
            Toggle toggle = ResourceTrackerOverlayUiFactory.CreateToggle(parent, $"{prefabName}_Toggle", displayName);
            bool isEnabled = PluginConfig.ResourceConfigs[prefabName].Value;

            toggle.SetIsOnWithoutNotify(isEnabled);
            ResourceTrackerOverlayState.ResourceToggles[prefabName] = toggle;

            toggle.onValueChanged.AddListener(delegate (bool value)
            {
                PluginConfig.ResourceConfigs[prefabName].Value = value;
            });
        }

        private static void AddDungeonToggle(GameObject parent, Room.Theme theme, string displayName)
        {
            Toggle toggle = ResourceTrackerOverlayUiFactory.CreateToggle(parent, $"{theme}_Toggle", displayName);
            bool isEnabled = PluginConfig.DungeonConfigs[theme].Value;

            toggle.SetIsOnWithoutNotify(isEnabled);
            ResourceTrackerOverlayState.DungeonToggles[theme] = toggle;

            toggle.onValueChanged.AddListener(delegate (bool value)
            {
                PluginConfig.DungeonConfigs[theme].Value = value;
            });
        }

        private static void AddCreatureToggle(GameObject parent, string prefabName, string displayName)
        {
            Toggle toggle = ResourceTrackerOverlayUiFactory.CreateToggle(parent, $"{prefabName}_Toggle", displayName);
            bool isEnabled = PluginConfig.CreatureConfigs[prefabName].Value;

            toggle.SetIsOnWithoutNotify(isEnabled);
            ResourceTrackerOverlayState.CreatureToggles[prefabName] = toggle;

            toggle.onValueChanged.AddListener(delegate (bool value)
            {
                PluginConfig.CreatureConfigs[prefabName].Value = value;
            });
        }
    }
}