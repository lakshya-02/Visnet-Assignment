using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VisnetXR.API;
using VisnetXR.Managers;

namespace VisnetXR.UI
{
    public class ManualXrAppController : MonoBehaviour
    {
        private const string LoginPanel = "login";
        private const string ProjectsPanel = "projects";
        private const string FloorsPanel = "floors";
        private const string SummaryPanel = "summary";

        [Header("Services")]
        [SerializeField] private AuthAPI authAPI;
        [SerializeField] private ProjectAPI projectAPI;
        [SerializeField] private SessionManager sessionManager;
        [SerializeField] private NavigationManager navigationManager;

        [Header("UI Screens")]
        [SerializeField] private LoginUI loginUI;
        [SerializeField] private ProjectListUI projectListUI;
        [SerializeField] private FloorDropdownUI floorDropdownUI;
        [SerializeField] private SummaryUI summaryUI;
        [SerializeField] private ToastUI toastUI;

        [Header("Selection Colors")]
        [SerializeField] private Color normalItemColor = new(0.105f, 0.118f, 0.145f, 0.96f);
        [SerializeField] private Color selectedItemColor = new(0.357f, 0.549f, 1f, 1f);
        [SerializeField] private Color mutedTextColor = new(0.392f, 0.455f, 0.545f, 1f);
        [SerializeField] private Color successColor = new(0.341f, 0.839f, 0.553f, 1f);
        [SerializeField] private Color errorColor = new(1f, 0.361f, 0.478f, 1f);

        [Header("Floor Center Style")]
        [SerializeField] private Color floorDropdownColor = new(0.30f, 0.30f, 0.30f, 0.96f);
        [SerializeField] private Color floorRowColor = new(0.075f, 0.075f, 0.075f, 0.98f);
        [SerializeField] private Color floorHoverColor = new(0.18f, 0.18f, 0.18f, 1f);
        [SerializeField] private Color floorTextColor = new(0.94f, 0.94f, 0.94f, 1f);

        private ProjectData selectedProject;
        private Sprite roundedSprite;

        private void Awake()
        {
            NormalizeRuntimeColors();
            ResolveSceneServices();
            RegisterPanels();
            WireStaticButtons();
            HideTemplates();
        }

        private void NormalizeRuntimeColors()
        {
            // Keep status text readable in both dark and light mode even if scene values are stale.
            mutedTextColor = new Color(0.392f, 0.455f, 0.545f, 1f);
            floorDropdownColor = new Color(0.30f, 0.30f, 0.30f, 0.96f);
            floorRowColor = new Color(0.075f, 0.075f, 0.075f, 0.98f);
            floorHoverColor = new Color(0.18f, 0.18f, 0.18f, 1f);
            floorTextColor = new Color(0.94f, 0.94f, 0.94f, 1f);
        }

        private void Start()
        {
            sessionManager.Clear();
            PrefillDemoCredentials();
            StyleFloorCenterPanel();
            navigationManager.ResetTo(LoginPanel);
        }

        private void PrefillDemoCredentials()
        {
            // Keeps Quest recordings moving when the XR keyboard is unavailable.
            if (loginUI.usernameInput != null && string.IsNullOrWhiteSpace(loginUI.usernameInput.text))
            {
                loginUI.usernameInput.text = "testuser";
            }

            if (loginUI.passwordInput != null && string.IsNullOrWhiteSpace(loginUI.passwordInput.text))
            {
                loginUI.passwordInput.text = "123456";
            }
        }

        private void ResolveSceneServices()
        {
            authAPI ??= GetComponent<AuthAPI>();
            projectAPI ??= GetComponent<ProjectAPI>();
            sessionManager ??= GetComponent<SessionManager>();
            navigationManager ??= GetComponent<NavigationManager>();
        }

        private void RegisterPanels()
        {
            navigationManager.RegisterPanel(LoginPanel, loginUI.panel);
            navigationManager.RegisterPanel(ProjectsPanel, projectListUI.panel);
            navigationManager.RegisterPanel(FloorsPanel, floorDropdownUI.panel);
            navigationManager.RegisterPanel(SummaryPanel, summaryUI.panel);
        }

        private void WireStaticButtons()
        {
            loginUI.loginButton.onClick.AddListener(OnLoginClicked);
            projectListUI.backButton.onClick.AddListener(OnLogoutClicked);
            projectListUI.continueButton.onClick.AddListener(ContinueWithSelectedProject);
            floorDropdownUI.backButton.onClick.AddListener(navigationManager.Back);
            if (floorDropdownUI.backToProjectsButton != null)
            {
                floorDropdownUI.backToProjectsButton.onClick.AddListener(navigationManager.Back);
            }

            if (floorDropdownUI.logoutButton != null)
            {
                floorDropdownUI.logoutButton.onClick.AddListener(OnLogoutClicked);
            }

            floorDropdownUI.continueButton.onClick.AddListener(ShowSummary);
            summaryUI.backButton.onClick.AddListener(navigationManager.Back);
            summaryUI.restartButton.onClick.AddListener(OnLogoutClicked);
        }

        private void HideTemplates()
        {
            projectListUI.itemButtonTemplate.gameObject.SetActive(false);
            floorDropdownUI.itemButtonTemplate.gameObject.SetActive(false);
        }

        private void OnLoginClicked()
        {
            StartCoroutine(LoginRoutine());
        }

        private IEnumerator LoginRoutine()
        {
            SetLoginBusy(true, "Signing in...");

            yield return authAPI.Login(loginUI.usernameInput.text, loginUI.passwordInput.text, result =>
            {
                SetLoginBusy(false, string.Empty);

                if (!result.IsSuccess || result.Data == null || !result.Data.success)
                {
                    SetStatus(loginUI.statusText, "Invalid credentials", errorColor);
                    toastUI.Show("Invalid Credentials");
                    return;
                }

                sessionManager.SetLogin(result.Data);
                SetStatus(loginUI.statusText, "Login successful", successColor);
                toastUI.Show("Login Successful");
                navigationManager.Show(ProjectsPanel);
                StartCoroutine(LoadProjectsRoutine());
            });
        }

        private IEnumerator LoadProjectsRoutine()
        {
            ClearDynamicItems(projectListUI.listRoot, projectListUI.itemButtonTemplate.transform);
            SetStatus(projectListUI.statusText, "Loading projects...", mutedTextColor);
            SetStatus(projectListUI.selectedProjectText, "No project selected", mutedTextColor);
            projectListUI.continueButton.interactable = false;
            selectedProject = null;

            yield return projectAPI.GetProjects(result =>
            {
                ClearDynamicItems(projectListUI.listRoot, projectListUI.itemButtonTemplate.transform);

                if (!result.IsSuccess || result.Data?.projects == null)
                {
                    SetStatus(projectListUI.statusText, "Unable to load projects", errorColor);
                    toastUI.Show("Project API error");
                    return;
                }

                SetStatus(projectListUI.statusText, "Select a project", mutedTextColor);
                foreach (ProjectData project in result.Data.projects)
                {
                    Button item = CreateItem(projectListUI.itemButtonTemplate, projectListUI.listRoot, project.name);
                    item.onClick.AddListener(() => OnProjectSelected(project, item));
                }
            });
        }

        private void OnProjectSelected(ProjectData project, Button selectedButton)
        {
            selectedProject = project;
            sessionManager.SetProject(project);
            HighlightSelected(projectListUI.listRoot, selectedButton);
            SetStatus(projectListUI.selectedProjectText, project.name, successColor);
            SetStatus(projectListUI.statusText, "Press Continue to load floors", mutedTextColor);
            projectListUI.continueButton.interactable = true;
            toastUI.Show($"Project Selected: {project.name}");
        }

        private void ContinueWithSelectedProject()
        {
            if (selectedProject == null)
            {
                SetStatus(projectListUI.statusText, "Select a project first", errorColor);
                toastUI.Show("Select a project first");
                return;
            }

            floorDropdownUI.projectTitle.text = selectedProject.name;
            SetStatus(floorDropdownUI.selectedFloorText, "No floor selected", mutedTextColor);
            floorDropdownUI.continueButton.interactable = false;
            navigationManager.Show(FloorsPanel);
            StartCoroutine(LoadFloorsRoutine(selectedProject.id));
        }

        private IEnumerator LoadFloorsRoutine(int projectId)
        {
            ClearDynamicItems(floorDropdownUI.listRoot, floorDropdownUI.itemButtonTemplate.transform);
            SetStatus(floorDropdownUI.statusText, "Loading floors...", mutedTextColor);

            yield return projectAPI.GetFloors(projectId, result =>
            {
                ClearDynamicItems(floorDropdownUI.listRoot, floorDropdownUI.itemButtonTemplate.transform);

                if (!result.IsSuccess || result.Data?.floors == null || result.Data.floors.Length == 0)
                {
                    SetStatus(floorDropdownUI.statusText, "No floors available", errorColor);
                    toastUI.Show("Floor API error");
                    return;
                }

                SetStatus(floorDropdownUI.statusText, "Select a floor", mutedTextColor);
                foreach (string floor in result.Data.floors)
                {
                    Button item = CreateItem(floorDropdownUI.itemButtonTemplate, floorDropdownUI.listRoot, floor);
                    StyleFloorButton(item);
                    item.onClick.AddListener(() => OnFloorSelected(floor, item));
                }
            });
        }

        private void OnFloorSelected(string floor, Button selectedButton)
        {
            sessionManager.SetFloor(floor);
            HighlightSelected(floorDropdownUI.listRoot, selectedButton);
            SetStatus(floorDropdownUI.selectedFloorText, floor, successColor);
            floorDropdownUI.continueButton.interactable = true;
            toastUI.Show($"Selected Floor: {floor}");
        }

        private void ShowSummary()
        {
            summaryUI.userText.text = $"Welcome, {sessionManager.User?.name ?? "Test User"}";
            summaryUI.projectText.text = $"Selected Project: {sessionManager.SelectedProject?.name ?? "-"}";
            summaryUI.floorText.text = $"Selected Floor: {sessionManager.SelectedFloor}";
            summaryUI.statusText.text = "Status: Configuration Complete";
            navigationManager.Show(SummaryPanel);
        }

        private void OnLogoutClicked()
        {
            sessionManager.Clear();
            selectedProject = null;
            ClearDynamicItems(projectListUI.listRoot, projectListUI.itemButtonTemplate.transform);
            ClearDynamicItems(floorDropdownUI.listRoot, floorDropdownUI.itemButtonTemplate.transform);
            loginUI.passwordInput.text = string.Empty;
            SetStatus(projectListUI.selectedProjectText, "No project selected", mutedTextColor);
            SetStatus(floorDropdownUI.selectedFloorText, "No floor selected", mutedTextColor);
            SetStatus(loginUI.statusText, string.Empty, mutedTextColor);
            projectListUI.continueButton.interactable = false;
            floorDropdownUI.continueButton.interactable = false;
            toastUI.Show("Logged out");
            navigationManager.ResetTo(LoginPanel);
        }

        private void SetLoginBusy(bool isBusy, string message)
        {
            loginUI.loginButton.interactable = !isBusy;
            SetStatus(loginUI.statusText, message, mutedTextColor);
        }

        private static Button CreateItem(Button template, Transform parent, string label)
        {
            // Clone the hidden template for API-driven rows.
            Button item = Instantiate(template, parent);
            item.gameObject.SetActive(true);
            item.name = label;

            TMP_Text text = item.GetComponentInChildren<TMP_Text>();
            if (text != null)
            {
                text.text = label;
            }

            if (item.GetComponent<XRButtonFeedback>() == null)
            {
                item.gameObject.AddComponent<XRButtonFeedback>();
            }

            return item;
        }

        private void HighlightSelected(Transform root, Button selectedButton)
        {
            Color normalColor = root == floorDropdownUI.listRoot ? floorRowColor : normalItemColor;

            foreach (Transform child in root)
            {
                Button button = child.GetComponent<Button>();
                Image image = button != null ? GetButtonImage(button) : child.GetComponent<Image>();
                if (image != null)
                {
                    image.color = normalColor;
                }
            }

            Image selectedImage = GetButtonImage(selectedButton);
            if (selectedImage != null)
            {
                selectedImage.color = selectedItemColor;
            }
        }

        private static Image GetButtonImage(Button button)
        {
            if (button == null)
            {
                return null;
            }

            if (button.targetGraphic is Image targetImage)
            {
                return targetImage;
            }

            return button.GetComponent<Image>() ?? button.GetComponentInChildren<Image>();
        }

        private static void ClearDynamicItems(Transform root, Transform template)
        {
            for (int index = root.childCount - 1; index >= 0; index--)
            {
                Transform child = root.GetChild(index);
                if (child != template)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        private static void SetStatus(TMP_Text label, string message, Color color)
        {
            if (label == null)
            {
                return;
            }

            label.text = message;
            label.color = color;
        }

        private void StyleFloorCenterPanel()
        {
            if (floorDropdownUI?.panel == null)
            {
                return;
            }

            roundedSprite ??= FindRoundedSprite(floorDropdownUI.panel.transform);
            Transform centerArea = FindChild(floorDropdownUI.panel.transform, "CenterArea");
            if (centerArea == null)
            {
                return;
            }

            SetRect(centerArea, -20f, 0f, 610f, 560f);

            Transform headerText = FindChild(centerArea, "HeaderText");
            if (headerText != null)
            {
                headerText.gameObject.SetActive(false);
            }

            if (floorDropdownUI.projectTitle != null)
            {
                floorDropdownUI.projectTitle.gameObject.SetActive(false);
            }

            Transform dropdownHeader = EnsureDropdownHeader(centerArea);
            SetRect(dropdownHeader, 0f, 200f, 360f, 58f);

            if (floorDropdownUI.listRoot != null)
            {
                SetRect(floorDropdownUI.listRoot.transform, 0f, -5f, 360f, 330f);
                ConfigureFloorLayout(floorDropdownUI.listRoot);
            }

            StyleFloorButton(floorDropdownUI.itemButtonTemplate);
        }

        private Transform EnsureDropdownHeader(Transform centerArea)
        {
            Transform header = FindDirectChild(centerArea, "FloorDropdownHeader");
            if (header == null)
            {
                GameObject headerObject = new("FloorDropdownHeader", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                headerObject.transform.SetParent(centerArea, false);
                header = headerObject.transform;
            }

            Image headerImage = header.GetComponent<Image>() ?? header.gameObject.AddComponent<Image>();
            headerImage.sprite = roundedSprite;
            headerImage.type = roundedSprite != null ? Image.Type.Sliced : Image.Type.Simple;
            headerImage.color = floorDropdownColor;
            headerImage.raycastTarget = false;

            TMP_Text label = EnsureText(header, "Label");
            label.text = "Floors";
            label.fontSize = 24f;
            label.color = floorTextColor;
            label.alignment = TextAlignmentOptions.MidlineLeft;
            SetRect(label.transform, -25f, 0f, 250f, 58f);

            TMP_Text caret = EnsureText(header, "Caret");
            caret.text = "v";
            caret.fontSize = 28f;
            caret.color = floorTextColor;
            caret.alignment = TextAlignmentOptions.Center;
            SetRect(caret.transform, 145f, 1f, 40f, 58f);

            return header;
        }

        private void ConfigureFloorLayout(RectTransform listRoot)
        {
            GridLayoutGroup grid = listRoot.GetComponent<GridLayoutGroup>() ?? listRoot.gameObject.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(350f, 78f);
            grid.spacing = new Vector2(0f, 16f);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 1;
            grid.childAlignment = TextAnchor.UpperCenter;
            grid.startAxis = GridLayoutGroup.Axis.Vertical;
            grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
            grid.padding = new RectOffset(0, 0, 0, 0);
        }

        private void StyleFloorButton(Button button)
        {
            if (button == null)
            {
                return;
            }

            RectTransform rect = button.GetComponent<RectTransform>();
            if (rect != null)
            {
                rect.sizeDelta = new Vector2(350f, 78f);
            }

            Image image = GetButtonImage(button);
            if (image != null)
            {
                image.sprite = roundedSprite;
                image.type = roundedSprite != null ? Image.Type.Sliced : Image.Type.Simple;
                image.color = floorRowColor;
                image.raycastTarget = true;
                button.targetGraphic = image;
            }

            ColorBlock colors = button.colors;
            colors.normalColor = floorRowColor;
            colors.highlightedColor = floorHoverColor;
            colors.pressedColor = Color.Lerp(floorRowColor, Color.black, 0.2f);
            colors.selectedColor = selectedItemColor;
            colors.disabledColor = new Color(0.12f, 0.12f, 0.12f, 0.45f);
            colors.fadeDuration = 0.08f;
            button.colors = colors;

            TMP_Text label = button.GetComponentInChildren<TMP_Text>(true);
            if (label != null)
            {
                label.color = floorTextColor;
                label.fontSize = 23f;
                label.alignment = TextAlignmentOptions.MidlineLeft;
                RectTransform labelRect = label.GetComponent<RectTransform>();
                if (labelRect != null)
                {
                    labelRect.anchorMin = Vector2.zero;
                    labelRect.anchorMax = Vector2.one;
                    labelRect.offsetMin = new Vector2(34f, 0f);
                    labelRect.offsetMax = new Vector2(-24f, 0f);
                }
            }
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

            for (int index = 0; index < root.childCount; index++)
            {
                Transform child = root.GetChild(index);
                if (child.name == name)
                {
                    return child;
                }
            }

            return null;
        }

        private static TMP_Text EnsureText(Transform parent, string name)
        {
            Transform existing = FindDirectChild(parent, name);
            if (existing != null && existing.TryGetComponent(out TMP_Text existingText))
            {
                return existingText;
            }

            GameObject textObject = new(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            textObject.transform.SetParent(parent, false);
            TMP_Text text = textObject.GetComponent<TMP_Text>();
            text.raycastTarget = false;
            return text;
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
    }
}
