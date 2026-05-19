using TMPro;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Readers;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.UI;
using InputXRController = UnityEngine.InputSystem.XR.XRController;

namespace VisnetXR.UI
{
    /// <summary>
    /// Runtime guard for Quest input, hand-mounted canvas placement, and fallback editor keyboard support.
    /// </summary>
    [DefaultExecutionOrder(-10000)]
    public sealed class XrRuntimeStabilizer : MonoBehaviour, IXRInputButtonReader
    {
        private const string LeftControllerMenuAnchorName = "LeftControllerMenuAnchor";
        private const float PressThreshold = 0.2f;
        private const float NearDashboardDistance = 1.15f;
        private const float HandMountedDashboardScale = 0.0011f;
        private static readonly Vector3 LeftControllerMenuOffset = new(0f, 0.36f, 0.18f);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            if (FindAnyObjectByType<XrRuntimeStabilizer>() != null)
            {
                return;
            }

            new GameObject("XR Runtime Stabilizer").AddComponent<XrRuntimeStabilizer>();
        }

        private bool isPressed;
        private bool pressedThisFrame;
        private bool releasedThisFrame;
        private float pressValue;
        private static TMP_InputField activeInputField;
        private static TouchScreenKeyboard nativeKeyboard;
        private Canvas handMountedCanvas;
        private Transform leftControllerAnchor;
        private Transform menuAnchor;
        private bool isDashboardHandMounted;
        private float nextInteractorRefreshTime;

        private void Awake()
        {
            DisablePhysicsLocomotion();
            ConfigureEventSystem();
            ConfigureCanvases();
            ConfigureInteractors();
        }

        private void Update()
        {
            SyncNativeKeyboard();
            RefreshInteractorsForSimulator();

            bool currentPressed = ReadRawPress(out float currentValue);
            pressedThisFrame = !isPressed && currentPressed;
            releasedThisFrame = isPressed && !currentPressed;
            isPressed = currentPressed;
            pressValue = currentValue;
        }

        private void LateUpdate()
        {
            AttachDashboardToLeftController();
        }

        private static void DisablePhysicsLocomotion()
        {
            Physics.gravity = Vector3.zero;

            foreach (LocomotionProvider provider in FindObjectsByType<LocomotionProvider>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                provider.enabled = false;
            }

            foreach (CharacterController controller in FindObjectsByType<CharacterController>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                controller.enabled = false;
            }

            foreach (Rigidbody body in FindObjectsByType<Rigidbody>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                body.useGravity = false;
                body.isKinematic = true;
            }

            foreach (XROrigin origin in FindObjectsByType<XROrigin>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                // Keep the rig root stable while headset tracking continues to drive the camera pose.
                Vector3 position = origin.transform.position;
                origin.transform.position = new Vector3(position.x, 0f, position.z);
            }
        }

        private static void ConfigureEventSystem()
        {
            EventSystem eventSystem = EventSystem.current ?? FindAnyObjectByType<EventSystem>();
            if (eventSystem == null)
            {
                GameObject eventSystemObject = new("EventSystem");
                eventSystem = eventSystemObject.AddComponent<EventSystem>();
            }

            XRUIInputModule xrModule = eventSystem.GetComponent<XRUIInputModule>() ?? eventSystem.gameObject.AddComponent<XRUIInputModule>();
            xrModule.enabled = true;
            xrModule.enableXRInput = true;
            xrModule.enableMouseInput = true;
            xrModule.enableTouchInput = true;
            xrModule.activeInputMode = XRUIInputModule.ActiveInputMode.InputSystemActions;

            InputSystemUIInputModule inputSystemModule = eventSystem.GetComponent<InputSystemUIInputModule>();
            if (inputSystemModule != null)
            {
                inputSystemModule.enabled = false;
            }
        }

        private static void ConfigureCanvases()
        {
            Camera mainCamera = Camera.main;
            foreach (Canvas canvas in FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (canvas.renderMode != RenderMode.WorldSpace)
                {
                    continue;
                }

                if (canvas.worldCamera == null && mainCamera != null)
                {
                    canvas.worldCamera = mainCamera;
                }

                PlaceCanvasInFront(canvas, NearDashboardDistance);

                if (canvas.GetComponent<GraphicRaycaster>() == null)
                {
                    canvas.gameObject.AddComponent<GraphicRaycaster>();
                }

                if (canvas.GetComponent<TrackedDeviceGraphicRaycaster>() == null)
                {
                    canvas.gameObject.AddComponent<TrackedDeviceGraphicRaycaster>();
                }

                ConfigureKeyboard(canvas);
            }
        }

        private void AttachDashboardToLeftController()
        {
            handMountedCanvas ??= FindWorldSpaceCanvas();
            if (handMountedCanvas == null)
            {
                return;
            }

            if (!IsLeftControllerTracked())
            {
                UseCameraFallback(handMountedCanvas);
                return;
            }

            leftControllerAnchor ??= FindControllerTransform(true);
            if (leftControllerAnchor == null)
            {
                UseCameraFallback(handMountedCanvas);
                return;
            }

            menuAnchor = EnsureMenuAnchor(leftControllerAnchor, menuAnchor);
            FaceMenuTowardHeadset(menuAnchor);

            if (handMountedCanvas.transform.parent != menuAnchor)
            {
                handMountedCanvas.transform.SetParent(menuAnchor, false);
            }

            handMountedCanvas.transform.localPosition = Vector3.zero;
            handMountedCanvas.transform.localRotation = Quaternion.identity;
            handMountedCanvas.transform.localScale = Vector3.one * HandMountedDashboardScale;
            isDashboardHandMounted = true;
        }

        private void UseCameraFallback(Canvas canvas)
        {
            if (isDashboardHandMounted || canvas.transform.parent != null)
            {
                canvas.transform.SetParent(null, true);
                isDashboardHandMounted = false;
                leftControllerAnchor = null;
                menuAnchor = null;
            }

            PlaceCanvasInFront(canvas, NearDashboardDistance);
        }

        private static Transform EnsureMenuAnchor(Transform leftController, Transform existingAnchor)
        {
            if (existingAnchor != null && existingAnchor.parent == leftController)
            {
                existingAnchor.localPosition = LeftControllerMenuOffset;
                return existingAnchor;
            }

            Transform foundAnchor = leftController.Find(LeftControllerMenuAnchorName);
            if (foundAnchor == null)
            {
                foundAnchor = new GameObject(LeftControllerMenuAnchorName).transform;
                foundAnchor.SetParent(leftController, false);
            }

            foundAnchor.localPosition = LeftControllerMenuOffset;
            foundAnchor.localRotation = Quaternion.identity;
            foundAnchor.localScale = Vector3.one;
            return foundAnchor;
        }

        private static void FaceMenuTowardHeadset(Transform anchor)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                return;
            }

            Vector3 lookDirection = anchor.position - mainCamera.transform.position;
            lookDirection.y = 0f;
            if (lookDirection.sqrMagnitude < 0.001f)
            {
                return;
            }

            anchor.rotation = Quaternion.LookRotation(lookDirection.normalized, Vector3.up);
        }

        private static Canvas FindWorldSpaceCanvas()
        {
            Canvas fallbackCanvas = null;
            foreach (Canvas canvas in FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (canvas.renderMode != RenderMode.WorldSpace)
                {
                    continue;
                }

                if (canvas.name == "WorldSpaceCanvas")
                {
                    return canvas;
                }

                fallbackCanvas ??= canvas;
            }

            return fallbackCanvas;
        }

        private static bool IsLeftControllerTracked()
        {
            InputXRController leftController = InputSystem.GetDevice<InputXRController>(CommonUsages.LeftHand);
            if (leftController == null || !leftController.enabled)
            {
                return false;
            }

            ButtonControl isTracked = leftController.TryGetChildControl<ButtonControl>("isTracked");
            if (isTracked != null)
            {
                return isTracked.isPressed;
            }

            IntegerControl trackingState = leftController.TryGetChildControl<IntegerControl>("trackingState");
            if (trackingState != null)
            {
                return trackingState.ReadValue() != 0;
            }

            return true;
        }

        private static void ConfigureKeyboard(Canvas canvas)
        {
            Transform existingKeyboard = canvas.transform.Find("XRKeyboardPanel");
            GameObject keyboardPanel = existingKeyboard != null ? existingKeyboard.gameObject : CreateKeyboardPanel(canvas.transform);
            keyboardPanel.SetActive(false);

            foreach (TMP_InputField input in canvas.GetComponentsInChildren<TMP_InputField>(true))
            {
                input.onSelect.AddListener(_ =>
                {
                    activeInputField = input;
                    keyboardPanel.SetActive(true);
                    OpenNativeKeyboard(input);
                });
            }
        }

        private static GameObject CreateKeyboardPanel(Transform canvasTransform)
        {
            GameObject panel = new("XRKeyboardPanel", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            panel.transform.SetParent(canvasTransform, false);

            RectTransform panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0.5f, 0.5f);
            panelRect.anchorMax = new Vector2(0.5f, 0.5f);
            panelRect.pivot = new Vector2(0.5f, 0.5f);
            panelRect.anchoredPosition = new Vector2(0f, -240f);
            panelRect.sizeDelta = new Vector2(820f, 240f);

            Image panelImage = panel.GetComponent<Image>();
            panelImage.color = new Color(0.043f, 0.063f, 0.094f, 0.96f);
            panelImage.raycastTarget = false;

            AddKeyboardRow(panel.transform, "1234567890", 72f, 10);
            AddKeyboardRow(panel.transform, "qwertyuiop", 25f, 10);
            AddKeyboardRow(panel.transform, "asdfghjkl", -22f, 9);
            AddKeyboardRow(panel.transform, "zxcvbnm", -69f, 7);

            CreateKeyboardButton(panel.transform, "Demo", new Vector2(-310f, -112f), new Vector2(120f, 38f), () => FillDemoCredentials(panel.GetComponentInParent<Canvas>()));
            CreateKeyboardButton(panel.transform, "Space", new Vector2(-145f, -112f), new Vector2(180f, 38f), () => InsertText(" "));
            CreateKeyboardButton(panel.transform, "Back", new Vector2(30f, -112f), new Vector2(100f, 38f), Backspace);
            CreateKeyboardButton(panel.transform, "Clear", new Vector2(145f, -112f), new Vector2(100f, 38f), ClearActiveInput);
            CreateKeyboardButton(panel.transform, "Hide", new Vector2(265f, -112f), new Vector2(100f, 38f), HideKeyboard);

            return panel;
        }

        public static void HideKeyboard()
        {
            if (activeInputField != null)
            {
                activeInputField.DeactivateInputField();
                activeInputField = null;
            }

            if (nativeKeyboard != null)
            {
                nativeKeyboard.active = false;
                nativeKeyboard = null;
            }

            EventSystem.current?.SetSelectedGameObject(null);

            foreach (Canvas canvas in FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                Transform keyboard = canvas.transform.Find("XRKeyboardPanel");
                if (keyboard != null)
                {
                    keyboard.gameObject.SetActive(false);
                }
            }
        }

        private static void AddKeyboardRow(Transform parent, string letters, float y, int count)
        {
            const float keyWidth = 54f;
            const float spacing = 7f;
            float startX = -((count - 1) * (keyWidth + spacing)) * 0.5f;

            for (int index = 0; index < letters.Length; index++)
            {
                string value = letters[index].ToString();
                CreateKeyboardButton(parent, value, new Vector2(startX + index * (keyWidth + spacing), y), new Vector2(keyWidth, 38f), () => InsertText(value));
            }
        }

        private static Button CreateKeyboardButton(Transform parent, string label, Vector2 position, Vector2 size, UnityEngine.Events.UnityAction onClick)
        {
            Button button = CreateUiButton(parent, label, position, size, 20);
            button.onClick.AddListener(onClick);
            return button;
        }

        private static Button CreateUiButton(Transform parent, string label, Vector2 position, Vector2 size, int fontSize)
        {
            GameObject buttonObject = new(label + "Button", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
            buttonObject.transform.SetParent(parent, false);

            RectTransform rect = buttonObject.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            Image image = buttonObject.GetComponent<Image>();
            image.color = new Color(0.145f, 0.388f, 0.922f, 0.96f);
            image.raycastTarget = true;

            Button button = buttonObject.GetComponent<Button>();
            button.targetGraphic = image;

            GameObject textObject = new("Label", typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            textObject.transform.SetParent(buttonObject.transform, false);

            RectTransform textRect = textObject.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = Vector2.zero;
            textRect.offsetMax = Vector2.zero;

            TextMeshProUGUI text = textObject.GetComponent<TextMeshProUGUI>();
            text.text = label;
            text.fontSize = fontSize;
            text.color = Color.white;
            text.alignment = TextAlignmentOptions.Center;
            text.raycastTarget = false;

            return button;
        }

        private static void PlaceCanvasInFront(Canvas canvas, float distance)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
            {
                return;
            }

            Vector3 forward = mainCamera.transform.forward;
            forward.y = 0f;
            if (forward.sqrMagnitude < 0.01f)
            {
                forward = Vector3.forward;
            }

            forward.Normalize();
            canvas.transform.position = mainCamera.transform.position + forward * distance + new Vector3(0f, -0.05f, 0f);
            canvas.transform.rotation = Quaternion.Euler(0f, mainCamera.transform.eulerAngles.y, 0f);
        }

        private static void FillDemoCredentials(Canvas canvas)
        {
            if (canvas == null)
            {
                return;
            }

            foreach (TMP_InputField input in canvas.GetComponentsInChildren<TMP_InputField>(true))
            {
                string lowerName = input.gameObject.name.ToLowerInvariant();
                if (lowerName.Contains("username"))
                {
                    input.text = "testuser";
                }
                else if (lowerName.Contains("password"))
                {
                    input.text = "123456";
                }
            }
        }

        private static void InsertText(string value)
        {
            if (activeInputField == null)
            {
                return;
            }

            activeInputField.text += value;
            activeInputField.caretPosition = activeInputField.text.Length;
        }

        private static void Backspace()
        {
            if (activeInputField == null || string.IsNullOrEmpty(activeInputField.text))
            {
                return;
            }

            activeInputField.text = activeInputField.text[..^1];
            activeInputField.caretPosition = activeInputField.text.Length;
        }

        private static void ClearActiveInput()
        {
            if (activeInputField != null)
            {
                activeInputField.text = string.Empty;
            }
        }

        private static void OpenNativeKeyboard(TMP_InputField input)
        {
            if (!TouchScreenKeyboard.isSupported)
            {
                return;
            }

            bool isPassword = input.contentType == TMP_InputField.ContentType.Password;
            nativeKeyboard = TouchScreenKeyboard.Open(input.text, TouchScreenKeyboardType.Default, false, false, isPassword);
        }

        private static void SyncNativeKeyboard()
        {
            if (nativeKeyboard == null || activeInputField == null)
            {
                return;
            }

            activeInputField.text = nativeKeyboard.text;
            if (nativeKeyboard.status != TouchScreenKeyboard.Status.Visible)
            {
                nativeKeyboard = null;
            }
        }

        private void RefreshInteractorsForSimulator()
        {
            if (Time.unscaledTime < nextInteractorRefreshTime)
            {
                return;
            }

            nextInteractorRefreshTime = Time.unscaledTime + 0.75f;
            ConfigureInteractors();
        }

        private void ConfigureInteractors()
        {
            XRRayInteractor[] rays = FindObjectsByType<XRRayInteractor>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            NearFarInteractor[] nearFars = FindObjectsByType<NearFarInteractor>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            bool hasRightHandInteractor = HasRightHandInteractor(rays, nearFars);

            foreach (XRRayInteractor ray in rays)
            {
                bool enableInteractor = hasRightHandInteractor ? IsRightHandInteractor(ray) : true;
                ray.enableUIInteraction = enableInteractor;
                if (enableInteractor)
                {
                    ray.uiPressInput.bypass = this;
                }

                XRInteractorLineVisual lineVisual = ray.GetComponent<XRInteractorLineVisual>();
                if (lineVisual != null)
                {
                    lineVisual.enabled = enableInteractor;
                }
            }

            foreach (NearFarInteractor nearFar in nearFars)
            {
                bool enableInteractor = hasRightHandInteractor ? IsRightHandInteractor(nearFar) : true;
                nearFar.enableUIInteraction = enableInteractor;
                if (enableInteractor)
                {
                    nearFar.uiPressInput.bypass = this;
                }
            }
        }

        private static bool HasRightHandInteractor(XRRayInteractor[] rays, NearFarInteractor[] nearFars)
        {
            foreach (XRRayInteractor ray in rays)
            {
                if (IsRightHandInteractor(ray))
                {
                    return true;
                }
            }

            foreach (NearFarInteractor nearFar in nearFars)
            {
                if (IsRightHandInteractor(nearFar))
                {
                    return true;
                }
            }

            return false;
        }

        private static Transform FindControllerTransform(bool leftHand)
        {
            string desired = leftHand ? "left" : "right";
            foreach (Transform transform in FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                string lowerName = transform.name.ToLowerInvariant();
                if (lowerName.Contains(desired) && lowerName.Contains("controller"))
                {
                    return transform;
                }
            }

            foreach (XRBaseInteractor interactor in FindObjectsByType<XRBaseInteractor>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (leftHand && !IsLeftHandInteractor(interactor))
                {
                    continue;
                }

                if (!leftHand && !IsRightHandInteractor(interactor))
                {
                    continue;
                }

                return FindNamedControllerParent(interactor.transform, desired) ?? interactor.transform;
            }

            return null;
        }

        private static Transform FindNamedControllerParent(Transform start, string desired)
        {
            Transform current = start;
            while (current != null)
            {
                string lowerName = current.name.ToLowerInvariant();
                if (lowerName.Contains(desired) && lowerName.Contains("controller"))
                {
                    return current;
                }

                current = current.parent;
            }

            return null;
        }

        private static bool IsRightHandInteractor(XRBaseInteractor interactor)
        {
            return IsHandInteractor(interactor, InteractorHandedness.Right);
        }

        private static bool IsLeftHandInteractor(XRBaseInteractor interactor)
        {
            return IsHandInteractor(interactor, InteractorHandedness.Left);
        }

        private static bool IsHandInteractor(XRBaseInteractor interactor, InteractorHandedness handedness)
        {
            if (interactor.handedness == handedness)
            {
                return true;
            }

            InteractorHandedness oppositeHand = handedness == InteractorHandedness.Right ? InteractorHandedness.Left : InteractorHandedness.Right;
            if (interactor.handedness == oppositeHand)
            {
                return false;
            }

            string hierarchyName = GetHierarchyName(interactor.transform);
            string desired = handedness == InteractorHandedness.Right ? "right" : "left";
            string opposite = handedness == InteractorHandedness.Right ? "left" : "right";

            if (hierarchyName.Contains(desired))
            {
                return true;
            }

            return !hierarchyName.Contains(opposite) && handedness == InteractorHandedness.Right;
        }

        private static string GetHierarchyName(Transform transform)
        {
            string names = string.Empty;
            Transform current = transform;
            while (current != null)
            {
                names += current.name.ToLowerInvariant();
                current = current.parent;
            }

            return names;
        }

        public bool ReadIsPerformed()
        {
            return isPressed;
        }

        public bool ReadWasPerformedThisFrame()
        {
            return pressedThisFrame;
        }

        public bool ReadWasCompletedThisFrame()
        {
            return releasedThisFrame;
        }

        public float ReadValue()
        {
            return pressValue;
        }

        public bool TryReadValue(out float value)
        {
            value = pressValue;
            return true;
        }

        private static bool ReadRawPress(out float value)
        {
            value = 0f;

            if (Mouse.current?.leftButton.isPressed == true)
            {
                value = 1f;
                return true;
            }

            if (Keyboard.current?.spaceKey.isPressed == true)
            {
                value = 1f;
                return true;
            }

            InputXRController rightController = InputSystem.GetDevice<InputXRController>(CommonUsages.RightHand);
            if (rightController != null)
            {
                return TryReadDevicePress(rightController, ref value);
            }

            bool foundRightNamedController = false;
            foreach (InputDevice device in InputSystem.devices)
            {
                if (!IsRightHandDevice(device))
                {
                    continue;
                }

                foundRightNamedController = true;
                if (TryReadDevicePress(device, ref value))
                {
                    return true;
                }
            }

            if (foundRightNamedController)
            {
                return false;
            }

            foreach (InputDevice device in InputSystem.devices)
            {
                if (TryReadDevicePress(device, ref value))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool TryReadDevicePress(InputDevice device, ref float value)
        {
            AxisControl trigger = device.TryGetChildControl<AxisControl>("trigger")
                ?? device.TryGetChildControl<AxisControl>("indexTrigger");

            ButtonControl triggerPressedControl = device.TryGetChildControl<ButtonControl>("triggerPressed")
                ?? device.TryGetChildControl<ButtonControl>("triggerButton")
                ?? device.TryGetChildControl<ButtonControl>("select")
                ?? device.TryGetChildControl<ButtonControl>("selectButton")
                ?? device.TryGetChildControl<ButtonControl>("primaryButton");

            float triggerValue = trigger?.ReadValue() ?? 0f;
            bool triggerPressed = triggerPressedControl?.isPressed == true || triggerValue > PressThreshold;
            value = Mathf.Max(value, triggerValue);

            if (!triggerPressed)
            {
                return false;
            }

            value = Mathf.Max(value, 1f);
            return true;
        }

        private static bool IsRightHandDevice(InputDevice device)
        {
            if (HasUsage(device, CommonUsages.RightHand))
            {
                return true;
            }

            if (HasUsage(device, CommonUsages.LeftHand))
            {
                return false;
            }

            string deviceName = $"{device.name} {device.displayName} {device.description.product}".ToLowerInvariant();
            if (deviceName.Contains("right"))
            {
                return true;
            }

            return !deviceName.Contains("left");
        }

        private static bool HasUsage(InputDevice device, InternedString usage)
        {
            foreach (InternedString deviceUsage in device.usages)
            {
                if (deviceUsage == usage)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
