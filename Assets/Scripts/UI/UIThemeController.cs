using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VisnetXR.UI
{
    public class UIThemeController : MonoBehaviour
    {
        [Header("Scene")]
        [SerializeField] private Transform themeRoot;
        [SerializeField] private Button toggleButton;
        [SerializeField] private Vector2 togglePosition = new(506f, 258f);
        [SerializeField] private Vector2 toggleSize = new(48f, 48f);
        [SerializeField] private Vector2 iconSize = new(25f, 25f);
        [SerializeField] private bool createToggleIfMissing = true;
        [SerializeField] private bool startInLightMode;

        [Header("Dark Theme")]
        [SerializeField] private Color darkShell = new(0.043f, 0.063f, 0.094f, 0.92f);
        [SerializeField] private Color darkSurface = new(0.118f, 0.161f, 0.231f, 0.96f);
        [SerializeField] private Color darkInputSurface = new(0.118f, 0.161f, 0.231f, 1f);
        [SerializeField] private Color darkButton = new(0.231f, 0.510f, 0.965f, 1f);
        [SerializeField] private Color darkText = new(0.973f, 0.980f, 0.988f, 1f);

        [Header("Light Theme")]
        [SerializeField] private Color lightShell = new(0.855f, 0.890f, 0.945f, 0.98f);
        [SerializeField] private Color lightSurface = new(0.965f, 0.976f, 0.992f, 1f);
        [SerializeField] private Color lightInputSurface = new(0.812f, 0.855f, 0.925f, 1f);
        [SerializeField] private Color lightButton = new(0.145f, 0.388f, 0.922f, 1f);
        [SerializeField] private Color lightText = new(0.043f, 0.063f, 0.094f, 1f);

        private bool isLightMode;
        private Sprite generatedCircleSprite;

        private void Awake()
        {
            themeRoot ??= transform;
            UseReadableLightPalette();
            EnsureToggleButton();

            if (toggleButton != null)
            {
                toggleButton.onClick.AddListener(ToggleTheme);
            }

            NormalizeToggleIcon();
            ApplyTheme(startInLightMode);
        }

        private void UseReadableLightPalette()
        {
            // Scene values can be stale after quick UI iterations, so normalize contrast at runtime.
            lightShell = new Color(0.855f, 0.890f, 0.945f, 0.98f);
            lightSurface = new Color(0.965f, 0.976f, 0.992f, 1f);
            lightInputSurface = new Color(0.812f, 0.855f, 0.925f, 1f);
            lightButton = new Color(0.145f, 0.388f, 0.922f, 1f);
            lightText = new Color(0.043f, 0.063f, 0.094f, 1f);
        }

        private void EnsureToggleButton()
        {
            if (!createToggleIfMissing || toggleButton != null)
            {
                return;
            }

            GameObject buttonObject = new("ThemeToggleButton", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(XRButtonFeedback));
            buttonObject.transform.SetParent(themeRoot, false);

            RectTransform rect = buttonObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = togglePosition;
            rect.sizeDelta = toggleSize;

            Image image = buttonObject.GetComponent<Image>();
            image.sprite = GetCircleSprite();
            image.type = Image.Type.Simple;
            image.raycastTarget = true;

            toggleButton = buttonObject.GetComponent<Button>();
            toggleButton.targetGraphic = image;

            GameObject labelObject = new("BulbIcon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            labelObject.transform.SetParent(buttonObject.transform, false);

            RectTransform labelRect = labelObject.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            Image icon = labelObject.GetComponent<Image>();
            icon.color = Color.white;
            icon.preserveAspect = true;
            icon.raycastTarget = false;
            NormalizeToggleIcon();
        }

        private Sprite GetCircleSprite()
        {
            if (generatedCircleSprite != null)
            {
                return generatedCircleSprite;
            }

            const int size = 64;
            Texture2D texture = new(size, size, TextureFormat.RGBA32, false);
            Color clear = new(1f, 1f, 1f, 0f);
            Color solid = Color.white;
            Vector2 center = new((size - 1) * 0.5f, (size - 1) * 0.5f);
            float radius = size * 0.48f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    bool inside = Vector2.Distance(new Vector2(x, y), center) <= radius;
                    texture.SetPixel(x, y, inside ? solid : clear);
                }
            }

            texture.Apply();
            generatedCircleSprite = Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100f);
            return generatedCircleSprite;
        }

        public void ToggleTheme()
        {
            ApplyTheme(!isLightMode);
        }

        public void ApplyTheme(bool lightMode)
        {
            isLightMode = lightMode;
            ApplyImages(lightMode);
            ApplyText(lightMode);
        }

        private void ApplyImages(bool lightMode)
        {
            Color shell = lightMode ? lightShell : darkShell;
            Color surface = lightMode ? lightSurface : darkSurface;
            Color inputSurface = lightMode ? lightInputSurface : darkInputSurface;
            Color button = lightMode ? lightButton : darkButton;

            foreach (Image image in themeRoot.GetComponentsInChildren<Image>(true))
            {
                if (IsFloorCenterElement(image.transform))
                {
                    continue;
                }

                Button parentButton = image.GetComponentInParent<Button>();
                if (parentButton != null)
                {
                    image.color = parentButton.targetGraphic == image ? button : (lightMode ? new Color(0.043f, 0.063f, 0.094f, 1f) : Color.white);
                    continue;
                }

                // Names keep scene styling stable without a heavy theme framework.
                string lowerName = image.gameObject.name.ToLowerInvariant();
                if (lowerName.Contains("input") || lowerName.Contains("search"))
                {
                    image.color = inputSurface;
                }
                else
                {
                    image.color = lowerName.Contains("shell") || lowerName.Contains("background") ? shell : surface;
                }
            }
        }

        private void NormalizeToggleIcon()
        {
            if (toggleButton == null)
            {
                return;
            }

            RectTransform buttonRect = toggleButton.GetComponent<RectTransform>();
            if (buttonRect != null)
            {
                buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
                buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
                buttonRect.pivot = new Vector2(0.5f, 0.5f);
                buttonRect.anchoredPosition = togglePosition;
                buttonRect.sizeDelta = toggleSize;
            }

            foreach (Image image in toggleButton.GetComponentsInChildren<Image>(true))
            {
                if (toggleButton.targetGraphic == image)
                {
                    continue;
                }

                RectTransform iconRect = image.GetComponent<RectTransform>();
                iconRect.anchorMin = new Vector2(0.5f, 0.5f);
                iconRect.anchorMax = new Vector2(0.5f, 0.5f);
                iconRect.pivot = new Vector2(0.5f, 0.5f);
                iconRect.anchoredPosition = Vector2.zero;
                iconRect.sizeDelta = iconSize;
                image.preserveAspect = true;
                image.raycastTarget = false;
            }
        }

        private void ApplyText(bool lightMode)
        {
            Color textColor = lightMode ? lightText : darkText;
            Color buttonTextColor = Color.white;
            Color placeholderColor = lightMode ? new Color(0.226f, 0.275f, 0.357f, 1f) : new Color(0.682f, 0.706f, 0.761f, 1f);

            foreach (TMP_Text text in themeRoot.GetComponentsInChildren<TMP_Text>(true))
            {
                if (IsFloorCenterElement(text.transform))
                {
                    continue;
                }

                bool isButtonText = text.GetComponentInParent<Button>() != null;
                text.color = isButtonText ? buttonTextColor : textColor;
                text.alpha = 1f;
            }

            foreach (TMP_InputField input in themeRoot.GetComponentsInChildren<TMP_InputField>(true))
            {
                input.customCaretColor = true;
                input.caretColor = textColor;
                input.selectionColor = lightMode ? new Color(0.231f, 0.510f, 0.965f, 0.28f) : new Color(0.357f, 0.549f, 1f, 0.35f);

                if (input.textComponent != null)
                {
                    input.textComponent.color = textColor;
                }

                if (input.placeholder is TMP_Text placeholder)
                {
                    placeholder.color = placeholderColor;
                }
            }
        }

        private static bool IsFloorCenterElement(Transform transform)
        {
            while (transform != null)
            {
                string name = transform.name;
                if (name == "FloorDropdownHeader" || name == "FloorListRoot" || name == "FloorButtonTemplate")
                {
                    return true;
                }

                transform = transform.parent;
            }

            return false;
        }
    }
}
