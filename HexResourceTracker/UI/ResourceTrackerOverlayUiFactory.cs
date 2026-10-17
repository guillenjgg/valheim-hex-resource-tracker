using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace HexResourceTracker.UI
{
    internal static class ResourceTrackerOverlayUiFactory
    {
        private const float SectionHeight = 24f;
        private const float RowSpacing = 1f;
        private const float ToggleHeight = 24f;

        internal const float ColumnWidth = 180f;
        internal const float ContentPadding = 6f;

        internal static TextMeshProUGUI CreateTitle(GameObject parent, float headerHeight, UnityAction onCollapse)
        {
            GameObject titleObject = new GameObject("Title");
            titleObject.transform.SetParent(parent.transform, false);

            RectTransform titleRect = titleObject.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0f, 1f);
            titleRect.anchorMax = new Vector2(1f, 1f);
            titleRect.pivot = new Vector2(0.5f, 1f);
            titleRect.anchoredPosition = Vector2.zero;
            titleRect.sizeDelta = new Vector2(0f, headerHeight);

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

            TextMeshProUGUI collapseText = CreateCollapseButton(titleObject, onCollapse);
            AddSeparator(titleObject);

            return collapseText;
        }

        internal static TextMeshProUGUI CreateCollapseButton(GameObject parent, UnityAction onCollapse)
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
            button.onClick.AddListener(onCollapse);

            GameObject textObject = new GameObject("Arrow");
            textObject.transform.SetParent(buttonObject.transform, false);

            RectTransform textRect = textObject.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            TextMeshProUGUI collapseText = textObject.AddComponent<TextMeshProUGUI>();
            collapseText.font = Minimap.instance.m_biomeNameLarge.font;
            collapseText.text = "−";
            collapseText.fontSize = 18f;
            collapseText.alignment = TextAlignmentOptions.Center;
            collapseText.color = Color.white;
            collapseText.raycastTarget = false;

            return collapseText;
        }

        internal static void AddSectionHeader(GameObject parent, string text, float topSpacing = 0f)
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

        internal static GameObject CreateColumn(GameObject parent, string name, float xPosition)
        {
            GameObject column = new GameObject(name);
            column.transform.SetParent(parent.transform, false);

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

        internal static void AddSeparator(GameObject parent)
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

        internal static Toggle CreateToggle(GameObject parent, string objectName, string displayName)
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
