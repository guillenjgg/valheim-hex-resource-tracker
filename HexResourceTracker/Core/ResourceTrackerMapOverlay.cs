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
            panelRect.sizeDelta = new Vector2(180f, 410f);

            Image background = _panel.AddComponent<Image>();
            background.color = new Color(0.22f, 0.16f, 0.10f, 0.75f);

            AddTitle();

            AddResourceToggle("Pickable_Mushroom", "Mushrooms", -35f);
            AddResourceToggle("Pickable_Dandelion", "Dandelions", -60f);
            AddResourceToggle("RaspberryBush", "Raspberries", -85f);
            AddResourceToggle("BlueberryBush", "Blueberries", -110f);
            AddResourceToggle("Pickable_Thistle", "Thistle", -135f);
            AddResourceToggle("Pickable_SeedCarrot", "Carrot Seeds", -160f);
            AddResourceToggle("Pickable_SeedTurnip", "Turnip Seeds", -185f);
            AddResourceToggle("Pickable_Flax_Wild", "Flax", -210f);
            AddResourceToggle("Pickable_Barley_Wild", "Barley", -235f);
            AddResourceToggle("CloudberryBush", "Cloudberries", -260f);
            AddResourceToggle("Pickable_Mushroom_JotunPuffs", "Jotun Puffs", -285f);
            AddResourceToggle("Pickable_Mushroom_Magecap", "Magecap", -310f);
            AddResourceToggle("rock4_copper", "Copper", -335f);
            AddResourceToggle("silvervein", "Silver", -360f);
            AddResourceToggle("giant_skull", "Giant Skull", -385f);
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
            title.text = "Track Resources";
            title.fontSize = 12f;
            title.alignment = TextAlignmentOptions.Center;
            title.color = Color.white;

            GameObject separator = new GameObject("Separator");
            separator.transform.SetParent(titleObject.transform, false);

            RectTransform separatorRect = separator.AddComponent<RectTransform>();
            separatorRect.anchorMin = new Vector2(0f, 0f);
            separatorRect.anchorMax = new Vector2(1f, 0f);
            separatorRect.pivot = new Vector2(0.5f, 0f);
            separatorRect.sizeDelta = new Vector2(0f, 2f);

            Image separatorImage = separator.AddComponent<Image>();
            separatorImage.color = new Color(0.6f, 0.5f, 0.3f, 0.8f);
        }

        private static void AddResourceToggle(string prefabName, string displayName, float yPosition)
        {
            GameObject toggleObject = new GameObject($"{prefabName}_Toggle");
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
            labelRect.anchorMin = new Vector2(0f, 0f);
            labelRect.anchorMax = new Vector2(1f, 1f);
            labelRect.offsetMin = new Vector2(34f, 0f);
            labelRect.offsetMax = new Vector2(-10f, 0f);

            TextMeshProUGUI label = labelObject.AddComponent<TextMeshProUGUI>();
            label.font = Minimap.instance.m_biomeNameLarge.font;
            label.text = displayName;
            label.fontSize = 14f;
            label.alignment = TextAlignmentOptions.Left;
            label.color = Color.white;

            bool isEnabled = PluginConfig.ResourceConfigs[prefabName].Value;
            toggle.SetIsOnWithoutNotify(isEnabled);
            ResourceToggles[prefabName] = toggle;

            toggle.onValueChanged.AddListener(delegate (bool value)
            {
                PluginConfig.ResourceConfigs[prefabName].Value = value;
            });
        }
    }
}