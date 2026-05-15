# Unity Manual Scene Hierarchy and Inspector Setup

Use this for a clean OpenXR + Unity XR Interaction Toolkit scene. The goal is a polished VR dashboard, not a generated scene.

## Required Packages

Keep these installed:

```text
OpenXR Plugin                         com.unity.xr.openxr
XR Plugin Management                  com.unity.xr.management
XR Interaction Toolkit                com.unity.xr.interaction.toolkit
Input System                          com.unity.inputsystem
TextMeshPro                           com.unity.textmeshpro
UGUI / Unity UI                       com.unity.ugui
Universal Render Pipeline             com.unity.render-pipelines.universal
```

Meta XR SDK has been removed. If needed later, reinstall it only for simulator/support tooling.

## Scene Hierarchy

Create or rename your main scene as:

```text
Assets/Scenes/MainScene.unity
```

Recommended hierarchy:

```text
MainScene
├── XR Origin (XR Rig)
│   ├── Camera Offset
│   │   ├── Main Camera
│   │   ├── Left Controller
│   │   │   ├── XR Ray Interactor
│   │   │   └── XR Interactor Line Visual
│   │   └── Right Controller
│   │       ├── XR Ray Interactor
│   │       └── XR Interactor Line Visual
│   └── XR Interaction Manager
├── EventSystem
├── WorldSpaceCanvas
│   ├── SafeArea
│   │   ├── LoginPanel
│   │   ├── ProjectListPanel
│   │   ├── FloorSelectionPanel
│   │   ├── SummaryPanel
│   │   └── ToastPanel
├── AppManager
└── Environment
    ├── Directional Light
    ├── Backdrop Plane
    └── Accent Glow Panels
```

## XR Origin

Add through:

```text
GameObject > XR > XR Origin (Action-based)
```

Inspector:

```text
XR Origin
Tracking Origin Mode: Floor
Camera Y Offset: 0
```

For each controller:

```text
XR Controller (Action-based)
XR Ray Interactor
XR Interactor Line Visual
Line Renderer
```

Ray settings:

```text
XR Ray Interactor
Line Type: Straight Line
Max Raycast Distance: 10
Hit Detection Type: Raycast
Enable UI Interaction: On
```

Line visual:

```text
Valid Color Gradient: Blue to transparent
Invalid Color Gradient: White/gray to transparent
Line Width: 0.006 to 0.012
```

## EventSystem

Use:

```text
EventSystem
Input System UI Input Module
XR UI Input Module if available in your XRI setup
```

If XR UI clicks do not work, check:

```text
WorldSpaceCanvas has Tracked Device Graphic Raycaster
XR Ray Interactor has Enable UI Interaction enabled
EventSystem is using Input System / XR UI input
```

## WorldSpaceCanvas

Create:

```text
GameObject > UI > Canvas
```

Inspector:

```text
Render Mode: World Space
Position: X 0, Y 1.45, Z 2.0
Rotation: X 0, Y 0, Z 0
Scale: X 0.0017, Y 0.0017, Z 0.0017
Width: 1200
Height: 820
Graphic Raycaster: enabled
Tracked Device Graphic Raycaster: add if available
```

Canvas Scaler:

```text
UI Scale Mode: Constant Pixel Size
Dynamic Pixels Per Unit: 10 to 12
```

## Visual Style

Use this consistent style:

```text
Background: #05070C
Main Panel: #101216 with 88-94% alpha
Panel Border: #2F3440
Primary Blue: #5B8CFF
Text: #FFFFFF
Muted Text: #AEB4C2
Success: #57D68D
Error: #FF5C7A
```

Panel sizing:

```text
Main panel width: 850-950
Main panel height: 560-650
Corner radius: 18-28
Button height: 64-76
Input height: 64-76
Text size title: 44-52
Text size body/buttons: 24-30
```

Make panels feel spatial:

```text
Add subtle shadow image behind each panel
Use 1 large floating panel, not many small cards
Keep all controls inside readable central area
Use blue selected state for active project/floor
```

## UI Panel Details

### LoginPanel

Hierarchy:

```text
LoginPanel
├── TitleText                 "ViSNET XR"
├── SubtitleText              "Project access portal"
├── UsernameInput
├── PasswordInput
├── LoginButton
└── StatusText
```

Inspector:

```text
LoginPanel: add LoginUI
LoginUI.panel: LoginPanel
LoginUI.usernameInput: UsernameInput
LoginUI.passwordInput: PasswordInput
LoginUI.loginButton: LoginButton
LoginUI.statusText: StatusText
```

Input placeholders:

```text
UsernameInput placeholder: Username
PasswordInput placeholder: Password
PasswordInput Content Type: Password
```

Optional default text for testing:

```text
UsernameInput: testuser
PasswordInput: 123456
```

### ProjectListPanel

Hierarchy:

```text
ProjectListPanel
├── TitleText                 "Select Project"
├── StatusText
├── ProjectListRoot
│   └── ProjectButtonTemplate
└── BackButton                "Logout"
```

Inspector:

```text
ProjectListPanel: add ProjectListUI
ProjectListUI.panel: ProjectListPanel
ProjectListUI.listRoot: ProjectListRoot
ProjectListUI.itemButtonTemplate: ProjectButtonTemplate
ProjectListUI.backButton: BackButton
ProjectListUI.statusText: StatusText
```

Important:

```text
ProjectButtonTemplate should be disabled in the scene
ProjectButtonTemplate must contain a TMP_Text child
ProjectListRoot should use Vertical Layout Group
```

### FloorSelectionPanel

Hierarchy:

```text
FloorSelectionPanel
├── ProjectTitleText
├── SubtitleText              "Select a floor"
├── StatusText
├── FloorListRoot
│   └── FloorButtonTemplate
├── ContinueButton            "Continue"
└── BackButton                "Back"
```

Inspector:

```text
FloorSelectionPanel: add FloorDropdownUI
FloorDropdownUI.panel: FloorSelectionPanel
FloorDropdownUI.projectTitle: ProjectTitleText
FloorDropdownUI.listRoot: FloorListRoot
FloorDropdownUI.itemButtonTemplate: FloorButtonTemplate
FloorDropdownUI.continueButton: ContinueButton
FloorDropdownUI.backButton: BackButton
FloorDropdownUI.statusText: StatusText
```

Important:

```text
FloorButtonTemplate should be disabled in the scene
ContinueButton can start disabled
FloorListRoot should use Vertical Layout Group
```

### SummaryPanel

Hierarchy:

```text
SummaryPanel
├── TitleText                 "Configuration Complete"
├── UserText
├── ProjectText
├── FloorText
├── StatusText
├── BackButton                "Back"
└── RestartButton             "Start Again"
```

Inspector:

```text
SummaryPanel: add SummaryUI
SummaryUI.panel: SummaryPanel
SummaryUI.userText: UserText
SummaryUI.projectText: ProjectText
SummaryUI.floorText: FloorText
SummaryUI.statusText: StatusText
SummaryUI.backButton: BackButton
SummaryUI.restartButton: RestartButton
```

### ToastPanel

Hierarchy:

```text
ToastPanel
├── BackgroundImage
└── ToastText
```

Inspector:

```text
ToastPanel: add CanvasGroup
ToastPanel: add ToastUI
ToastUI.canvasGroup: ToastPanel CanvasGroup
ToastUI.label: ToastText
Visible Seconds: 2.2
Fade Seconds: 0.25
```

Place ToastPanel:

```text
Anchored Position: X 0, Y -310
Width: 600-700
Height: 64-76
```

## AppManager Wiring

Create empty object:

```text
AppManager
```

Add these components:

```text
APIManager
AuthAPI
ProjectAPI
SessionManager
NavigationManager
ManualXrAppController
```

ManualXrAppController Inspector:

```text
Auth API: AuthAPI component
Project API: ProjectAPI component
Session Manager: SessionManager component
Navigation Manager: NavigationManager component
Login UI: LoginPanel/LoginUI
Project List UI: ProjectListPanel/ProjectListUI
Floor Dropdown UI: FloorSelectionPanel/FloorDropdownUI
Summary UI: SummaryPanel/SummaryUI
Toast UI: ToastPanel/ToastUI
```

AuthAPI and ProjectAPI:

```text
API Manager: AppManager/APIManager
```

## Backend URL

The Unity API config is:

```text
Assets/Scripts/API/APIConfig.cs
```

Current URL:

```text
https://visnet-assignment.vercel.app
```

## Build Settings

Use:

```text
Platform: Android
Texture Compression: ASTC
Minimum API Level: Android 10 or higher
Scripting Backend: IL2CPP
Target Architectures: ARM64
XR Plug-in Management: OpenXR enabled
```

## Final Play Test

Test in this order:

```text
Editor Play Mode: login and API calls
XR Device Simulator: ray interaction
Quest APK: final hardware test
```

Required flow:

```text
Login with testuser / 123456
Project list loads
Project selection opens floors
Floor options change by project
Floor selection highlights
Summary screen shows user/project/floor
Back buttons work
Toast messages fade
```
