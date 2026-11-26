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
        private const float ColumnSpacing = 6f;
        private const float HeaderHeight = 28f;
        private const float SectionSpacing = 6f;
        private const string Title = "Map Tracking";

        private static GameObject _panel;
        private static GameObject _content;
        private static RectTransform _panelRect;
        private static RectTransform _leftColumnRect;
        private static RectTransform _rightColumnRect;
        private static TextMeshProUGUI _collapseText;
        private static TextMeshProUGUI _titleText;
        private static GameObject _emptyText;

        // The panel shows only the rows of the player's current biome; a section with no rows showing is hidden.
        private static readonly List<BiomeSection> Sections = new List<BiomeSection>();
        private static Heightmap.Biome _shownBiome;

        private sealed class BiomeSection
        {
            internal GameObject Header;
            internal GameObject Body;
            internal readonly List<KeyValuePair<GameObject, Heightmap.Biome>> Rows = new List<KeyValuePair<GameObject, Heightmap.Biome>>();
        }

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
            Sections.Clear();
            _shownBiome = Heightmap.Biome.None;

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

            _collapseText = ResourceTrackerOverlayUiFactory.CreateTitle(_panel, HeaderHeight, ToggleCollapsed, out _titleText);

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

            _emptyText = ResourceTrackerOverlayUiFactory.CreateNote(leftColumn, "EmptyNote", "Nothing tracked here yet");

            BuildResources(leftColumn);
            BuildDeposits(leftColumn);
            BuildDungeons(rightColumn);
            BuildCreatures(rightColumn);
            BuildLocations(rightColumn);

            _leftColumnRect = leftColumn.GetComponent<RectTransform>();
            _rightColumnRect = rightColumn.GetComponent<RectTransform>();
            ApplyBiome();
        }

        // Called every frame from the minimap; redraws the panel only when the player's biome changes.
        internal static void UpdateBiome()
        {
            if (_panel == null || Player.m_localPlayer == null)
            {
                return;
            }

            Heightmap.Biome biome = Player.m_localPlayer.GetCurrentBiome();
            if (biome == _shownBiome)
            {
                return;
            }

            _shownBiome = biome;
            ApplyBiome();
        }

        private static void ApplyBiome()
        {
            bool anyShown = false;
            foreach (BiomeSection section in Sections)
            {
                bool sectionShown = false;
                foreach (var row in section.Rows)
                {
                    bool shown = TrackedBiomes.IsShown(row.Value, _shownBiome);
                    row.Key.SetActive(shown);
                    sectionShown |= shown;
                }

                section.Header.SetActive(sectionShown);
                section.Body.SetActive(sectionShown);
                anyShown |= sectionShown;
            }

            _emptyText.SetActive(!anyShown);
            string biomeName = TrackedBiomes.DisplayName(_shownBiome);
            _titleText.text = string.IsNullOrEmpty(biomeName) ? Title : $"{Title}: {biomeName}";
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

        private static BiomeSection CreateSection(GameObject parent, string title, float topSpacing = 0f)
        {
            GameObject header = ResourceTrackerOverlayUiFactory.AddSectionHeader(parent, title, topSpacing);
            BiomeSection section = new BiomeSection
            {
                Header = header,
                Body = ResourceTrackerOverlayUiFactory.CreateSectionBody(parent, title)
            };
            Sections.Add(section);
            return section;
        }

        private static void BuildResources(GameObject parent)
        {
            BiomeSection section = CreateSection(parent, "Resources");

            foreach (TrackedResourceDefinitionModel resource in TrackedResources.AllTrackedResources
                .Where(resource => !resource.ShowInLocations && resource.ResourceType != TrackedResourceTypeEnum.Deposit)
                .OrderBy(resource => resource.SortOrder))
            {
                AddResourceToggle(section, resource);
            }
        }

        private static void BuildDeposits(GameObject parent)
        {
            BiomeSection section = CreateSection(parent, "Deposits", SectionSpacing);

            foreach (TrackedResourceDefinitionModel resource in TrackedResources.AllTrackedResources
                .Where(resource => !resource.ShowInLocations && resource.ResourceType == TrackedResourceTypeEnum.Deposit)
                .OrderBy(resource => resource.SortOrder))
            {
                AddResourceToggle(section, resource);
            }
        }

        private static void BuildDungeons(GameObject parent)
        {
            BiomeSection section = CreateSection(parent, "Dungeons");

            foreach (TrackedDungeonDefinitionModel dungeon in TrackedDungeons.AllTrackedDungeons.OrderBy(dungeon => dungeon.SortOrder))
            {
                AddDungeonToggle(section, dungeon.Theme, dungeon.UiDisplayName);
            }
        }

        private static void BuildCreatures(GameObject parent)
        {
            BiomeSection section = CreateSection(parent, "Creatures", SectionSpacing);

            foreach (TrackedCreatureDefinitionModel creature in TrackedCreatures.AllTrackedCreatures.OrderBy(creature => creature.SortOrder))
            {
                AddCreatureToggle(section, creature.PrefabName, creature.DisplayName);
            }
        }

        private static void BuildLocations(GameObject parent)
        {
            BiomeSection section = CreateSection(parent, "Locations", SectionSpacing);
            foreach (var location in TrackedLocations.Groups())
            {
                string firstPrefabName = location.Value[0];
                Toggle toggle = ResourceTrackerOverlayUiFactory.CreateToggle(section.Body, $"{firstPrefabName}_Toggle", location.Key);
                toggle.SetIsOnWithoutNotify(PluginConfig.LocationConfigs[firstPrefabName].Value);
                foreach (string prefabName in location.Value)
                {
                    ResourceTrackerOverlayState.LocationToggles[prefabName] = toggle;
                }

                toggle.onValueChanged.AddListener(value => PluginConfig.LocationConfigs[firstPrefabName].Value = value);
                section.Rows.Add(new KeyValuePair<GameObject, Heightmap.Biome>(toggle.gameObject, TrackedBiomes.ForLocations(location.Value)));
            }
            foreach (TrackedResourceDefinitionModel resource in TrackedResources.AllTrackedResources.Where(resource => resource.ShowInLocations).OrderBy(resource => resource.SortOrder))
            {
                AddResourceToggle(section, resource);
            }

            Button button = ResourceTrackerOverlayUiFactory.CreateActionButton(section.Body, "FindNearestMemorialSite",
                "Find Nearest\nMemorial Site", LocationSearchService.FindNearestMemorialSite);
            section.Rows.Add(new KeyValuePair<GameObject, Heightmap.Biome>(button.gameObject, TrackedBiomes.MemorialSiteSearch));
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

        private static void AddResourceToggle(BiomeSection section, TrackedResourceDefinitionModel resource)
        {
            string prefabName = resource.ResourcePrefabName;
            Toggle toggle = ResourceTrackerOverlayUiFactory.CreateToggle(section.Body, $"{prefabName}_Toggle", resource.DisplayName);
            bool isEnabled = PluginConfig.ResourceConfigs[prefabName].Value;

            toggle.SetIsOnWithoutNotify(isEnabled);
            ResourceTrackerOverlayState.ResourceToggles[prefabName] = toggle;

            toggle.onValueChanged.AddListener(delegate (bool value)
            {
                PluginConfig.ResourceConfigs[prefabName].Value = value;
            });

            section.Rows.Add(new KeyValuePair<GameObject, Heightmap.Biome>(toggle.gameObject, TrackedBiomes.ForResource(resource)));
        }

        private static void AddDungeonToggle(BiomeSection section, Room.Theme theme, string displayName)
        {
            Toggle toggle = ResourceTrackerOverlayUiFactory.CreateToggle(section.Body, $"{theme}_Toggle", displayName);
            bool isEnabled = PluginConfig.DungeonConfigs[theme].Value;

            toggle.SetIsOnWithoutNotify(isEnabled);
            ResourceTrackerOverlayState.DungeonToggles[theme] = toggle;

            toggle.onValueChanged.AddListener(delegate (bool value)
            {
                PluginConfig.DungeonConfigs[theme].Value = value;
            });

            section.Rows.Add(new KeyValuePair<GameObject, Heightmap.Biome>(toggle.gameObject, TrackedBiomes.ForDungeon(theme)));
        }

        private static void AddCreatureToggle(BiomeSection section, string prefabName, string displayName)
        {
            Toggle toggle = ResourceTrackerOverlayUiFactory.CreateToggle(section.Body, $"{prefabName}_Toggle", displayName);
            bool isEnabled = PluginConfig.CreatureConfigs[prefabName].Value;

            toggle.SetIsOnWithoutNotify(isEnabled);
            ResourceTrackerOverlayState.CreatureToggles[prefabName] = toggle;

            toggle.onValueChanged.AddListener(delegate (bool value)
            {
                PluginConfig.CreatureConfigs[prefabName].Value = value;
            });

            section.Rows.Add(new KeyValuePair<GameObject, Heightmap.Biome>(toggle.gameObject, TrackedBiomes.ForCreature(prefabName)));
        }
    }
}
