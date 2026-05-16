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
        [SerializeField] private bool startInLightMode;

        [Header("Dark Theme")]
        [SerializeField] private Color darkShell = new(0.043f, 0.063f, 0.094f, 0.92f);
        [SerializeField] private Color darkSurface = new(0.118f, 0.161f, 0.231f, 0.96f);
        [SerializeField] private Color darkButton = new(0.231f, 0.510f, 0.965f, 1f);
        [SerializeField] private Color darkText = new(0.973f, 0.980f, 0.988f, 1f);

        [Header("Light Theme")]
        [SerializeField] private Color lightShell = new(0.933f, 0.957f, 0.988f, 0.94f);
        [SerializeField] private Color lightSurface = new(1f, 1f, 1f, 0.98f);
        [SerializeField] private Color lightButton = new(0.145f, 0.388f, 0.922f, 1f);
        [SerializeField] private Color lightText = new(0.059f, 0.091f, 0.165f, 1f);

        private bool isLightMode;

        private void Awake()
        {
            themeRoot ??= transform;

            if (toggleButton != null)
            {
                toggleButton.onClick.AddListener(ToggleTheme);
            }

            ApplyTheme(startInLightMode);
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
            Color button = lightMode ? lightButton : darkButton;

            foreach (Image image in themeRoot.GetComponentsInChildren<Image>(true))
            {
                if (image.GetComponentInParent<Button>() != null)
                {
                    image.color = button;
                    continue;
                }

                // Names keep scene styling stable without a heavy theme framework.
                string lowerName = image.gameObject.name.ToLowerInvariant();
                image.color = lowerName.Contains("shell") || lowerName.Contains("background") ? shell : surface;
            }
        }

        private void ApplyText(bool lightMode)
        {
            Color textColor = lightMode ? lightText : darkText;

            foreach (TMP_Text text in themeRoot.GetComponentsInChildren<TMP_Text>(true))
            {
                text.color = textColor;
            }

            foreach (TMP_InputField input in themeRoot.GetComponentsInChildren<TMP_InputField>(true))
            {
                if (input.textComponent != null)
                {
                    input.textComponent.color = textColor;
                }

                if (input.placeholder is TMP_Text placeholder)
                {
                    placeholder.color = lightMode ? new Color(0.392f, 0.455f, 0.545f, 1f) : new Color(0.682f, 0.706f, 0.761f, 1f);
                }
            }
        }
    }
}
