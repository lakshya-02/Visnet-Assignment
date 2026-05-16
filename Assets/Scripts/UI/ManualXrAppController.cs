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
        [SerializeField] private Color mutedTextColor = new(0.682f, 0.706f, 0.761f, 1f);
        [SerializeField] private Color successColor = new(0.341f, 0.839f, 0.553f, 1f);
        [SerializeField] private Color errorColor = new(1f, 0.361f, 0.478f, 1f);

        private ProjectData selectedProject;

        private void Awake()
        {
            ResolveSceneServices();
            RegisterPanels();
            WireStaticButtons();
            HideTemplates();
        }

        private void Start()
        {
            sessionManager.Clear();
            navigationManager.ResetTo(LoginPanel);
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
            foreach (Transform child in root)
            {
                Button button = child.GetComponent<Button>();
                Image image = button != null ? GetButtonImage(button) : child.GetComponent<Image>();
                if (image != null)
                {
                    image.color = normalItemColor;
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
    }
}
