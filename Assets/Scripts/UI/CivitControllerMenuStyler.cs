using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VisnetXR.UI
{
    [DefaultExecutionOrder(-8500)]
    public sealed class CivitControllerMenuStyler : MonoBehaviour
    {
        private static readonly Color Shell = Hex("343434", 0.92f);
        private static readonly Color Panel = Hex("252525", 0.94f);
        private static readonly Color Pill = Hex("555555", 0.96f);
        private static readonly Color DarkPill = Hex("151515", 0.98f);
        private static readonly Color Text = Hex("F4F4F4", 1f);
        private static readonly Color Muted = Hex("C8C8C8", 1f);
        private static readonly Color Blue = Hex("3B82F6", 1f);

        private Sprite roundedSprite;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if (FindAnyObjectByType<CivitControllerMenuStyler>() != null)
            {
                return;
            }

            new GameObject("Civit Controller Menu Styler").AddComponent<CivitControllerMenuStyler>();
        }

        private IEnumerator Start()
        {
            yield return null;
            Apply();
        }

        private void Apply()
        {
            Canvas canvas = FindCanvas();
            if (canvas == null)
            {
                return;
            }

            roundedSprite = FindRoundedSprite(canvas.transform);
            SetRect(canvas.transform, 0f, 0f, 980f, 520f);

            StyleLogin(canvas.transform.Find("LoginPanel"));
            StyleProjectPanel(canvas.transform.Find("ProjectListPanel"));
            StyleFloorPanel(canvas.transform.Find("FloorSelectionPanel"));
            StyleSummary(canvas.transform.Find("SummaryPanel"));
            StyleToast(canvas.transform.Find("ToastPanel"));
        }

        private void StyleLogin(Transform panel)
        {
            if (panel == null)
            {
                return;
            }

            SetRect(panel, 0f, 0f, 760f, 440f);
            StyleImage(FindChild(panel, "ShellBackground"), Shell, false);
            StyleImage(FindChild(panel, "LeftBrandPanel"), Panel, false);
            StyleImage(FindChild(panel, "RightInfoPanel"), Panel, false);

            SetRect(FindChild(panel, "LeftBrandPanel"), -250f, 0f, 220f, 400f);
            SetRect(FindChild(panel, "LoginContent"), 35f, 0f, 420f, 360f);
            SetRect(FindChild(panel, "RightInfoPanel"), 275f, 0f, 180f, 400f);

            SetText(FindChild(panel, "BrandText"), "ViSNET XR", 30, Text);
            SetText(FindChild(panel, "SmallLabelText"), "Controller Menu", 17, Muted);
            SetText(FindChild(panel, "LoginTitleText"), "Sign in", 34, Text);
            StyleInput(FindChild(panel, "UsernameInput"), "Username", -20f);
            StyleInput(FindChild(panel, "PasswordInput"), "Password", -95f);
            StyleButton(FindChild(panel, "LoginButton"), "Continue", 0f, -175f, 360f, 58f, Blue);
            SetText(FindChild(panel, "InfoTitleText"), "Quest Flow", 21, Text);
            SetText(FindChild(panel, "InfoBodyText"), "Sign in, choose project, then select floors.", 16, Muted);
        }

        private void StyleProjectPanel(Transform panel)
        {
            if (panel == null)
            {
                return;
            }

            StyleControllerShell(panel);
            Transform left = FindChild(panel, "LeftNav");
            Transform center = FindChild(panel, "CenterArea");
            Transform right = FindChild(panel, "RightDetails");

            StyleLeftActions(left, "Civit Actions");
            StyleCenterArea(center, "Projects", "Select Project");
            StyleRightDetails(right, "Selection", "No project selected");

            SetText(FindChild(panel, "SearchText"), "Projects", 20, Text);
            StyleImage(FindChild(panel, "SearchBarVisual"), Pill, false);
            SetRect(FindChild(panel, "SearchBarVisual"), 0f, 170f, 360f, 58f);
            SetRect(FindChild(panel, "HeroCard"), 0f, 95f, 360f, 80f);
            StyleImage(FindChild(panel, "HeroCard"), DarkPill, false);
            SetText(FindChild(panel, "HeroTitleText"), "Active Projects", 23, Text);

            Transform listRoot = FindChild(panel, "ProjectListRoot");
            SetRect(listRoot, 0f, -65f, 360f, 245f);
            EnsureVerticalLayout(listRoot, 12f, 78f);
            StyleButton(FindChild(panel, "ProjectButtonTemplate"), "Project", 0f, 0f, 330f, 72f, DarkPill);

            StyleButton(FindChild(right, "ContinueButton"), "Continue", 0f, -190f, 180f, 54f, Blue);
        }

        private void StyleFloorPanel(Transform panel)
        {
            if (panel == null)
            {
                return;
            }

            StyleControllerShell(panel);
            Transform left = FindChild(panel, "LeftNav");
            Transform center = FindChild(panel, "CenterArea");
            Transform right = FindChild(panel, "RightDetails");

            StyleLeftActions(left, "Civit Actions");
            StyleCenterArea(center, "Floors", "Floors");
            StyleRightDetails(right, "Selection", "No floor selected");

            SetText(FindChild(panel, "ProjectTitleText"), "Floors", 20, Muted);
            SetRect(FindChild(panel, "ProjectTitleText"), 0f, 150f, 360f, 36f);

            Transform listRoot = FindChild(panel, "FloorListRoot");
            SetRect(listRoot, 0f, -30f, 360f, 300f);
            EnsureVerticalLayout(listRoot, 14f, 74f);
            StyleButton(FindChild(panel, "FloorButtonTemplate"), "Ground Floor", 0f, 0f, 330f, 72f, DarkPill);

            StyleButton(FindChild(right, "ContinueButton"), "Continue", 0f, -150f, 180f, 54f, Blue);
            StyleButton(FindChild(right, "BackButton"), "Back", 0f, -212f, 180f, 46f, Pill);
        }

        private void StyleSummary(Transform panel)
        {
            if (panel == null)
            {
                return;
            }

            SetRect(panel, 0f, 0f, 780f, 430f);
            StyleImage(FindChild(panel, "ShellBackground"), Shell, false);
            Transform card = FindChild(panel, "ContentCard");
            SetRect(card, 0f, 0f, 700f, 360f);
            StyleImage(card, Panel, false);

            SetText(FindChild(panel, "TitleText"), "Configuration Complete", 30, Text);
            SetText(FindChild(panel, "UserText"), "User:", 22, Text);
            SetText(FindChild(panel, "ProjectText"), "Project:", 22, Text);
            SetText(FindChild(panel, "FloorText"), "Floor:", 22, Text);
            SetText(FindChild(panel, "StatusText"), "Status:", 20, Muted);
            StyleButton(FindChild(panel, "BackButton"), "Back", -110f, -135f, 180f, 52f, Pill);
            StyleButton(FindChild(panel, "RestartButton"), "Start Again", 110f, -135f, 180f, 52f, Blue);
        }

        private void StyleControllerShell(Transform panel)
        {
            SetRect(panel, 0f, 0f, 980f, 520f);
            StyleImage(FindChild(panel, "ShellBackground"), Shell, false);
            SetRect(FindChild(panel, "ShellBackground"), 0f, 0f, 980f, 520f);
            SetRect(FindChild(panel, "LeftNav"), -330f, 0f, 310f, 490f);
            SetRect(FindChild(panel, "CenterArea"), 30f, 0f, 380f, 490f);
            SetRect(FindChild(panel, "RightDetails"), 365f, 0f, 230f, 490f);
            StyleImage(FindChild(panel, "LeftNav"), Panel, false);
            StyleImage(FindChild(panel, "RightDetails"), Panel, false);
        }

        private void StyleLeftActions(Transform left, string title)
        {
            if (left == null)
            {
                return;
            }

            SetText(FindChild(left, "NavTitleText"), title, 27, Text);
            SetRect(FindChild(left, "NavTitleText"), 35f, 205f, 210f, 44f);

            Button backButton = FindChild(left, "BackToProjectsButton")?.GetComponent<Button>();
            if (backButton != null)
            {
                StyleButton(backButton.transform, "<", -105f, 205f, 70f, 38f, Pill);
            }

            Transform first = FindChild(left, "ProjectsNavButton") ?? FindChild(left, "FloorsNavButton");
            StyleButton(first, "New/Rescan", 0f, 135f, 250f, 58f, Pill);
            HideUnusedNavButtons(left, first);
            StyleButton(EnsureAction(left, "SaveScanAction"), "Save Scan (.fbx)", 0f, 62f, 250f, 58f, Pill);
            StyleButton(EnsureAction(left, "UploadAction"), "Upload to Civit", 0f, -11f, 250f, 58f, Pill);
            StyleButton(EnsureAction(left, "ShowHideAction"), "Show/hide", 0f, -84f, 250f, 58f, Pill);
            StyleButton(EnsureAction(left, "ToggleMeshAction"), "Toggle Mesh", 0f, -157f, 250f, 58f, Pill);

            Transform logout = FindChild(left, "LogoutButton") ?? FindChild(left, "SummaryNavButton");
            StyleButton(logout, "Logout", 0f, -220f, 190f, 38f, DarkPill);
        }

        private static void HideUnusedNavButtons(Transform left, Transform visibleFirstAction)
        {
            HideIfUnused(FindChild(left, "FloorsNavButton"), visibleFirstAction);
            HideIfUnused(FindChild(left, "SummaryNavButton"), visibleFirstAction);
        }

        private static void HideIfUnused(Transform transform, Transform visibleFirstAction)
        {
            if (transform != null && transform != visibleFirstAction)
            {
                transform.gameObject.SetActive(false);
            }
        }

        private void StyleCenterArea(Transform center, string title, string header)
        {
            if (center == null)
            {
                return;
            }

            SetText(FindChild(center, "HeaderText"), title, 28, Text);
            SetRect(FindChild(center, "HeaderText"), 0f, 205f, 360f, 52f);
            Transform headerPill = EnsureAction(center, title + "HeaderPill");
            StyleImage(headerPill, Pill, false);
            SetRect(headerPill, 0f, 165f, 360f, 58f);
            TMP_Text headerText = EnsureText(headerPill, "Label");
            headerText.text = header + "  v";
            headerText.fontSize = 22;
            headerText.color = Text;
            headerText.alignment = TextAlignmentOptions.MidlineLeft;
            SetRect(headerText.transform, 18f, 0f, 310f, 58f);
        }

        private void StyleRightDetails(Transform right, string title, string emptyText)
        {
            if (right == null)
            {
                return;
            }

            SetText(FindChild(right, "DetailsTitleText"), title, 24, Text);
            SetRect(FindChild(right, "DetailsTitleText"), 0f, 195f, 190f, 42f);
            SetText(FindChild(right, "SelectedProjectText"), emptyText, 19, Muted);
            SetText(FindChild(right, "SelectedFloorText"), emptyText, 19, Muted);
            SetText(FindChild(right, "DetailsHintText"), "Use the right controller ray to select.", 16, Muted);
            SetText(FindChild(right, "StatusText"), string.Empty, 16, Muted);
        }

        private void StyleToast(Transform panel)
        {
            if (panel == null)
            {
                return;
            }

            SetRect(panel, 0f, -285f, 540f, 62f);
            StyleImage(FindChild(panel, "ToastBackground"), DarkPill, false);
            SetText(FindChild(panel, "ToastText"), string.Empty, 20, Text);
        }

        private Transform EnsureAction(Transform parent, string name)
        {
            Transform existing = FindChild(parent, name);
            if (existing != null)
            {
                return existing;
            }

            GameObject obj = new(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button), typeof(XRButtonFeedback));
            obj.transform.SetParent(parent, false);
            return obj.transform;
        }

        private static TMP_Text EnsureText(Transform parent, string name)
        {
            Transform existing = FindDirectChild(parent, name);
            if (existing != null && existing.TryGetComponent(out TMP_Text existingText))
            {
                return existingText;
            }

            GameObject obj = new(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            obj.transform.SetParent(parent, false);
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            return obj.GetComponent<TMP_Text>();
        }

        private void StyleInput(Transform inputTransform, string placeholder, float y)
        {
            if (inputTransform == null)
            {
                return;
            }

            SetRect(inputTransform, 0f, y, 360f, 58f);
            StyleImage(inputTransform, DarkPill, true);

            TMP_InputField input = inputTransform.GetComponent<TMP_InputField>();
            if (input != null)
            {
                if (input.placeholder is TMP_Text placeholderText)
                {
                    placeholderText.text = placeholder;
                    placeholderText.color = Muted;
                }

                if (input.textComponent != null)
                {
                    input.textComponent.color = Text;
                    input.textComponent.fontSize = 20;
                }
            }
        }

        private void StyleButton(Transform buttonTransform, string label, float x, float y, float width, float height, Color color)
        {
            if (buttonTransform == null)
            {
                return;
            }

            SetRect(buttonTransform, x, y, width, height);
            StyleImage(buttonTransform, color, true);

            Button button = buttonTransform.GetComponent<Button>();
            if (button != null)
            {
                Image image = buttonTransform.GetComponent<Image>();
                button.targetGraphic = image;
                ColorBlock colors = button.colors;
                colors.normalColor = color;
                colors.highlightedColor = Color.Lerp(color, Color.white, 0.18f);
                colors.pressedColor = Color.Lerp(color, Color.black, 0.18f);
                colors.selectedColor = Blue;
                colors.disabledColor = Color.Lerp(color, Color.black, 0.35f);
                colors.fadeDuration = 0.08f;
                button.colors = colors;
            }

            TMP_Text text = EnsureText(buttonTransform, "Label");
            text.text = label;
            text.fontSize = height < 45f ? 17 : 21;
            text.color = Text;
            text.alignment = TextAlignmentOptions.Center;
            text.raycastTarget = false;
        }

        private void StyleImage(Transform transform, Color color, bool raycast)
        {
            if (transform == null)
            {
                return;
            }

            Image image = transform.GetComponent<Image>() ?? transform.gameObject.AddComponent<Image>();
            if (image.sprite == null)
            {
                image.sprite = roundedSprite;
            }

            image.type = image.sprite != null ? Image.Type.Sliced : Image.Type.Simple;
            image.color = color;
            image.raycastTarget = raycast;
        }

        private static void SetText(Transform transform, string text, int size, Color color)
        {
            if (transform == null || !transform.TryGetComponent(out TMP_Text label))
            {
                return;
            }

            if (!string.IsNullOrEmpty(text))
            {
                label.text = text;
            }

            label.fontSize = size;
            label.color = color;
            label.enableWordWrapping = true;
        }

        private static void SetRect(Transform transform, float x, float y, float width, float height)
        {
            if (transform == null || !transform.TryGetComponent(out RectTransform rect))
            {
                return;
            }

            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(x, y);
            rect.sizeDelta = new Vector2(width, height);
        }

        private static void EnsureVerticalLayout(Transform root, float spacing, float itemHeight)
        {
            if (root == null)
            {
                return;
            }

            VerticalLayoutGroup layout = root.GetComponent<VerticalLayoutGroup>() ?? root.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.UpperCenter;
            layout.spacing = spacing;
            layout.padding = new RectOffset(0, 0, 0, 0);
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            foreach (LayoutElement element in root.GetComponentsInChildren<LayoutElement>(true))
            {
                element.preferredHeight = itemHeight;
            }
        }

        private static Canvas FindCanvas()
        {
            foreach (Canvas canvas in FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (canvas.renderMode == RenderMode.WorldSpace && canvas.name == "WorldSpaceCanvas")
                {
                    return canvas;
                }
            }

            return null;
        }

        private static Transform FindChild(Transform root, string name)
        {
            if (root == null)
            {
                return null;
            }

            foreach (Transform child in root.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == name)
                {
                    return child;
                }
            }

            return null;
        }

        private static Transform FindDirectChild(Transform root, string name)
        {
            if (root == null)
            {
                return null;
            }

            for (int i = 0; i < root.childCount; i++)
            {
                Transform child = root.GetChild(i);
                if (child.name == name)
                {
                    return child;
                }
            }

            return null;
        }

        private static Sprite FindRoundedSprite(Transform root)
        {
            foreach (Image image in root.GetComponentsInChildren<Image>(true))
            {
                if (image.sprite != null)
                {
                    return image.sprite;
                }
            }

            return null;
        }

        private static Color Hex(string hex, float alpha)
        {
            ColorUtility.TryParseHtmlString("#" + hex, out Color color);
            color.a = alpha;
            return color;
        }
    }
}
