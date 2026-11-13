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
        private static RectTransform _leftColumnRect;
        private static RectTransform _rightColumnRect;
        private static TextMeshProUGUI _collapseText;

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

            _collapseText = ResourceTrackerOverlayUiFactory.CreateTitle(_panel, HeaderHeight, ToggleCollapsed, () => SetAllSectionsCollapsed(false), () => SetAllSectionsCollapsed(true));

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
            BuildLocations(rightColumn);

            _leftColumnRect = leftColumn.GetComponent<RectTransform>();
            _rightColumnRect = rightColumn.GetComponent<RectTransform>();
            ApplyCollapsedState();
        }

        private static void UpdateLayout()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(_leftColumnRect);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_rightColumnRect);

            float leftHeight = LayoutUtility.GetPreferredHeight(_leftColumnRect);
            float rightHeight = LayoutUtility.GetPreferredHeight(_rightColumnRect);
            float contentHeight = Mathf.Max(leftHeight, rightHeight) + ResourceTrackerOverlayUiFactory.ContentPadding * 2f;

            RectTransform contentRect = _content.GetComponent<RectTransform>();
            contentRect.sizeDelta = new Vector2(0f, contentHeight);
            _panelRect.sizeDelta = new Vector2(PanelWidth, PluginConfig.OverlayCollapsed.Value ? HeaderHeight : HeaderHeight + contentHeight);
        }

        private static void BuildResources(GameObject parent)
        {
            GameObject sectionContent = ResourceTrackerOverlayUiFactory.CreateCollapsibleSection(parent, "Resources", UpdateLayout);

            var biomeGroups = TrackedResources.AllTrackedResources.Where(resource => !resource.ShowInLocations && resource.ResourceType != TrackedResourceTypeEnum.Deposit).OrderBy(resource => resource.SortOrder).GroupBy(resource => resource.BiomeName);

            foreach (var biomeGroup in biomeGroups)
            {
                GameObject biomeContent = ResourceTrackerOverlayUiFactory.CreateCollapsibleSection(sectionContent, biomeGroup.Key, UpdateLayout, SectionSpacing);

                foreach (TrackedResourceDefinitionModel resource in biomeGroup)
                {
                    AddResourceToggle(biomeContent, resource.ResourcePrefabName, resource.DisplayName);
                }
            }
        }

        private static void BuildDeposits(GameObject parent)
        {
            GameObject sectionContent = ResourceTrackerOverlayUiFactory.CreateCollapsibleSection(parent, "Deposits", UpdateLayout);

            foreach (TrackedResourceDefinitionModel resource in TrackedResources.AllTrackedResources
                .Where(resource => !resource.ShowInLocations && resource.ResourceType == TrackedResourceTypeEnum.Deposit)
                .OrderBy(resource => resource.SortOrder))
            {
                AddResourceToggle(sectionContent, resource.ResourcePrefabName, resource.DisplayName);
            }
        }

        private static void BuildDungeons(GameObject parent)
        {
            GameObject sectionContent = ResourceTrackerOverlayUiFactory.CreateCollapsibleSection(parent, "Dungeons", UpdateLayout, SectionSpacing);

            foreach (TrackedDungeonDefinitionModel dungeon in TrackedDungeons.AllTrackedDungeons.OrderBy(dungeon => dungeon.SortOrder))
            {
                AddDungeonToggle(sectionContent, dungeon.Theme, dungeon.UiDisplayName);
            }
        }

        private static void BuildCreatures(GameObject parent)
        {
            GameObject sectionContent = ResourceTrackerOverlayUiFactory.CreateCollapsibleSection(parent, "Creatures", UpdateLayout, SectionSpacing);

            foreach (TrackedCreatureDefinitionModel creature in TrackedCreatures.AllTrackedCreatures.OrderBy(creature => creature.SortOrder))
            {
                AddCreatureToggle(sectionContent, creature.PrefabName, creature.DisplayName);
            }
        }

        private static void BuildLocations(GameObject parent)
        {
            GameObject sectionContent = ResourceTrackerOverlayUiFactory.CreateCollapsibleSection(parent, "Locations", UpdateLayout, SectionSpacing);
            foreach (var location in TrackedLocations.All)
            {
                string prefabName = location.Key;
                Toggle toggle = ResourceTrackerOverlayUiFactory.CreateToggle(sectionContent, $"{prefabName}_Toggle", location.Value);
                toggle.SetIsOnWithoutNotify(PluginConfig.LocationConfigs[prefabName].Value);
                ResourceTrackerOverlayState.LocationToggles[prefabName] = toggle;
                toggle.onValueChanged.AddListener(value => PluginConfig.LocationConfigs[prefabName].Value = value);
            }
            foreach (TrackedResourceDefinitionModel resource in TrackedResources.AllTrackedResources.Where(resource => resource.ShowInLocations).OrderBy(resource => resource.SortOrder))
            {
                AddResourceToggle(sectionContent, resource.ResourcePrefabName, resource.DisplayName);
            }

            ResourceTrackerOverlayUiFactory.CreateActionButton(sectionContent, "FindNearestMemorialSite",
                "Find Nearest\nMemorial Site", LocationSearchService.FindNearestMemorialSite);
        }

        private static void SetAllSectionsCollapsed(bool collapsed)
        {
            foreach (var entry in PluginConfig.OverlaySectionCollapsedConfigs.Values)
            {
                entry.Value = collapsed;
            }

            PluginConfig.OverlayCollapsed.Value = false;
            ApplyCollapsedState();
        }

        private static void ToggleCollapsed()
        {
            PluginConfig.OverlayCollapsed.Value = !PluginConfig.OverlayCollapsed.Value;
        }

        internal static void ApplyCollapsedState()
        {
            if (_panel == null)
            {
                return;
            }

            if (_content == null)
            {
                return;
            }

            if (_leftColumnRect == null || _rightColumnRect == null)
            {
                return;
            }

            _content.SetActive(true);
            UpdateLayout();
            _content.SetActive(!PluginConfig.OverlayCollapsed.Value);
            _collapseText.text = PluginConfig.OverlayCollapsed.Value ? "+" : "−";
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