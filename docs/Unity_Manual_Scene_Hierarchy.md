# Unity Manual Scene Hierarchy and Inspector Setup

This guide matches the current project direction: OpenXR + Unity XR Interaction Toolkit, with a manually built world-space UI. As of the latest scene check, `MainScene.unity` already contains the XRI rig/prefab, `Input Action Manager`, `EventSystem`, and a world-space canvas object named `WorldSpace`. The remaining work is to build the UI panels and wire the app scripts.

## Required Packages

```text
OpenXR Plugin                         com.unity.xr.openxr
XR Plugin Management                  com.unity.xr.management
XR Interaction Toolkit                com.unity.xr.interaction.toolkit
Input System                          com.unity.inputsystem
TextMeshPro                           com.unity.textmeshpro
UGUI / Unity UI                       com.unity.ugui
Universal Render Pipeline             com.unity.render-pipelines.universal
```

## Current Scene Status

Already present in the saved scene:

```text
XR Origin (XR Rig)
Input Action Manager
EventSystem
WorldSpace
Main Camera / Directional Light / Global Volume
```

Build Settings has been updated to use:

```text
Assets/Scenes/MainScene.unity
```

Still to create:

```text
LoginPanel
ProjectListPanel
FloorSelectionPanel
SummaryPanel
ToastPanel
AppManager
```

## Build Order From Here

1. Rename `WorldSpace` to `WorldSpaceCanvas`.
2. Configure `WorldSpaceCanvas` as a world-space canvas.
3. Create all UI panels directly under `WorldSpaceCanvas`.
4. Create `AppManager`.
5. Add app scripts to `AppManager`.
6. Wire all Inspector references.
7. Test with mouse first, then XR ray, then APK.

## Full Build Order If Starting From Empty

1. Import XRI Starter Assets and XR Device Simulator.
2. Add XR Origin, XR Interaction Manager, Input Action Manager, and EventSystem.
3. Add WorldSpaceCanvas.
4. Build Login, Project List, Floor Selection, Summary, and Toast panels.
5. Add AppManager and wire all script references.
6. Test UI with mouse first, then XR ray, then APK.

## Scene Hierarchy

```text
MainScene
|-- XR Origin (XR Rig)
|   |-- Camera Offset
|   |   |-- Main Camera
|   |   |-- Left Controller
|   |   |-- Right Controller
|-- XR Interaction Manager
|-- Input Action Manager
|-- EventSystem
|-- WorldSpaceCanvas
|   |-- LoginPanel
|   |-- ProjectListPanel
|   |-- FloorSelectionPanel
|   |-- SummaryPanel
|   |-- ToastPanel
|-- AppManager
|-- Environment
```

## Input Action Manager

Create empty GameObject:

```text
Input Action Manager
```

Add component:

```text
Input Action Manager
```

Inspector:

```text
Action Assets Size: 1
Element 0: XRI Default Input Actions
```

Use:

```text
Assets/Samples/XR Interaction Toolkit/3.0.10/Starter Assets/XRI Default Input Actions.inputactions
```

If `Input Action Manager` already exists, only verify that `XRI Default Input Actions` is assigned. Do not create a duplicate.

## XR Origin and Controller Setup

Use the imported XRI Starter Assets prefab:

```text
Assets/Samples/XR Interaction Toolkit/3.0.10/Starter Assets/Prefabs/XR Origin (XR Rig).prefab
```

Do not manually add old `XR Ray Interactor` or `XR Interactor Line Visual` components. The XRI 3 Starter Assets rig already contains the correct controller/interactor setup.

Verify on the XR Origin:

```text
Tracking Origin Mode: Floor
Camera Y Offset: 0
Main Camera exists under Camera Offset
```

If controller UI clicks do not work, check these first:

```text
Input Action Manager has XRI Default Input Actions
EventSystem exists
WorldSpaceCanvas has Tracked Device Graphic Raycaster
Canvas Render Mode is World Space
```

If the prefab has `Near-Far Interactor` components, leave them enabled. They replace the older manual ray setup in XRI 3.

## EventSystem

Use:

```text
EventSystem
Input System UI Input Module
```

If XR UI clicks do not work, add/use:

```text
XR UI Input Module
```

Also check:

```text
WorldSpaceCanvas has Tracked Device Graphic Raycaster
Input Action Manager has XRI Default Input Actions
```

## WorldSpaceCanvas

Rename the current `WorldSpace` object to:

```text
WorldSpaceCanvas
```

Then set:

```text
Render Mode: World Space
Event Camera: Main Camera
Plane Distance: 1
Width: 1200
Height: 820
Position: X 0, Y 1.45, Z 2
Rotation: X 0, Y 0, Z 0
Scale: X 0.0017, Y 0.0017, Z 0.0017
```

Components:

```text
Canvas
Canvas Scaler
Graphic Raycaster
Tracked Device Graphic Raycaster
```

Canvas Scaler:

```text
UI Scale Mode: Constant Pixel Size
Scale Factor: 1
Reference Pixels Per Unit: 100
Dynamic Pixels Per Unit: 10
```

Graphic Raycaster:

```text
Ignore Reversed Graphics: On
Blocking Objects: None
Blocking Mask: Everything
```

Tracked Device Graphic Raycaster:

```text
Ignore Reversed Graphics: On
Check For 2D Occlusion: Off
Check For 3D Occlusion: Off
Blocking Mask: Everything
Raycast Trigger Interaction: Use Global
```

If you cannot find this component, use Add Component search:

```text
Tracked Device Graphic Raycaster
```

Hierarchy under canvas:

```text
WorldSpaceCanvas
|-- LoginPanel
|-- ProjectListPanel
|-- FloorSelectionPanel
|-- SummaryPanel
|-- ToastPanel
```

Do not create any extra wrapper object between the canvas and panels.

## Visual Style

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

Sizing:

```text
Main panel width: 850-950
Main panel height: 560-650
Button height: 64-76
Input height: 64-76
Title text: 44-52
Body/button text: 24-30
```

## LoginPanel

```text
LoginPanel
|-- TitleText                 "ViSNET XR"
|-- SubtitleText              "Project access portal"
|-- UsernameInput
|-- PasswordInput
|-- LoginButton
|-- StatusText
```

Panel RectTransform:

```text
Width: 850
Height: 560
Position: X 0, Y 0
Anchor Min: X 0.5, Y 0.5
Anchor Max: X 0.5, Y 0.5
Pivot: X 0.5, Y 0.5
```

Recommended layout:

```text
Vertical Layout Group
Padding Left/Right: 56
Padding Top: 46
Padding Bottom: 42
Spacing: 18
Child Control Width: On
Child Control Height: Off
Child Force Expand Width: On
Child Force Expand Height: Off
```

Attach:

```text
LoginPanel -> LoginUI
```

Wire:

```text
panel: LoginPanel
usernameInput: UsernameInput
passwordInput: PasswordInput
loginButton: LoginButton
statusText: StatusText
```

## ProjectListPanel

```text
ProjectListPanel
|-- TitleText                 "Select Project"
|-- StatusText
|-- ProjectListRoot
|   |-- ProjectButtonTemplate
|-- BackButton                "Logout"
```

Panel RectTransform:

```text
Width: 850
Height: 560
Position: X 0, Y 0
Anchor Min: X 0.5, Y 0.5
Anchor Max: X 0.5, Y 0.5
Pivot: X 0.5, Y 0.5
Initially Active: Off
```

Attach:

```text
ProjectListPanel -> ProjectListUI
```

Wire:

```text
panel: ProjectListPanel
listRoot: ProjectListRoot
itemButtonTemplate: ProjectButtonTemplate
backButton: BackButton
statusText: StatusText
```

Important:

```text
ProjectButtonTemplate: disabled in scene
ProjectButtonTemplate: must contain TMP_Text child
ProjectListRoot: Vertical Layout Group
```

## FloorSelectionPanel

```text
FloorSelectionPanel
|-- ProjectTitleText
|-- SubtitleText              "Select a floor"
|-- StatusText
|-- FloorListRoot
|   |-- FloorButtonTemplate
|-- ContinueButton            "Continue"
|-- BackButton                "Back"
```

Panel RectTransform:

```text
Width: 850
Height: 560
Position: X 0, Y 0
Anchor Min: X 0.5, Y 0.5
Anchor Max: X 0.5, Y 0.5
Pivot: X 0.5, Y 0.5
Initially Active: Off
```

Attach:

```text
FloorSelectionPanel -> FloorDropdownUI
```

Wire:

```text
panel: FloorSelectionPanel
projectTitle: ProjectTitleText
listRoot: FloorListRoot
itemButtonTemplate: FloorButtonTemplate
continueButton: ContinueButton
backButton: BackButton
statusText: StatusText
```

Important:

```text
FloorButtonTemplate: disabled in scene
FloorButtonTemplate: must contain TMP_Text child
FloorListRoot: Vertical Layout Group
ContinueButton: can start disabled
```

## SummaryPanel

```text
SummaryPanel
|-- TitleText                 "Configuration Complete"
|-- UserText
|-- ProjectText
|-- FloorText
|-- StatusText
|-- BackButton                "Back"
|-- RestartButton             "Start Again"
```

Panel RectTransform:

```text
Width: 850
Height: 560
Position: X 0, Y 0
Anchor Min: X 0.5, Y 0.5
Anchor Max: X 0.5, Y 0.5
Pivot: X 0.5, Y 0.5
Initially Active: Off
```

Attach:

```text
SummaryPanel -> SummaryUI
```

Wire:

```text
panel: SummaryPanel
userText: UserText
projectText: ProjectText
floorText: FloorText
statusText: StatusText
backButton: BackButton
restartButton: RestartButton
```

## ToastPanel

```text
ToastPanel
|-- BackgroundImage
|-- ToastText
```

Toast RectTransform:

```text
Anchor Min: X 0.5, Y 0
Anchor Max: X 0.5, Y 0
Pivot: X 0.5, Y 0
Width: 650
Height: 72
Position: X 0, Y 56
Initially Active: Off
```

Attach:

```text
ToastPanel -> CanvasGroup
ToastPanel -> ToastUI
```

Wire:

```text
canvasGroup: ToastPanel CanvasGroup
label: ToastText
visibleSeconds: 2.2
fadeSeconds: 0.25
```

## AppManager

Create this as a root-level empty GameObject, not inside the canvas.

```text
AppManager
|-- APIManager
|-- AuthAPI
|-- ProjectAPI
|-- SessionManager
|-- NavigationManager
|-- ManualXrAppController
```

AuthAPI:

```text
API Manager: AppManager/APIManager
```

ProjectAPI:

```text
API Manager: AppManager/APIManager
```

ManualXrAppController:

```text
Auth API: AuthAPI
Project API: ProjectAPI
Session Manager: SessionManager
Navigation Manager: NavigationManager
Login UI: LoginUI
Project List UI: ProjectListUI
Floor Dropdown UI: FloorDropdownUI
Summary UI: SummaryUI
Toast UI: ToastUI
```

Important:

```text
AuthAPI.API Manager -> AppManager/APIManager
ProjectAPI.API Manager -> AppManager/APIManager
ManualXrAppController.Login UI -> LoginPanel/LoginUI
ManualXrAppController.Project List UI -> ProjectListPanel/ProjectListUI
ManualXrAppController.Floor Dropdown UI -> FloorSelectionPanel/FloorDropdownUI
ManualXrAppController.Summary UI -> SummaryPanel/SummaryUI
ManualXrAppController.Toast UI -> ToastPanel/ToastUI
```

## Common Mistakes To Avoid

```text
Do not leave Build Settings pointing to deleted ViSNET_XR_Assignment scene.
Do not create two EventSystems.
Do not create two Input Action Managers.
Do not forget Tracked Device Graphic Raycaster on the canvas.
Do not manually add old XR Ray Interactor or Line Visual components when using the XRI 3 Starter Assets rig.
Do not forget to disable ProjectButtonTemplate and FloorButtonTemplate.
Do not put AppManager inside a disabled UI panel.
Do not forget to save MainScene before testing or committing.
```

## Test Order

```text
1. Play Mode: click LoginButton with mouse
2. Play Mode: login with testuser / 123456
3. Confirm project API loads
4. Confirm floors load after selecting a project
5. Confirm summary screen updates
6. Test XRI controller hover/click
7. Build APK and test on Quest
```
