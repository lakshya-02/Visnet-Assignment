# Unity Scene Hierarchy and Inspector Setup

This project uses a scene-authored Meta Quest / Horizon-style world-space dashboard. You do not need to manually build every panel again unless the scene is damaged.

Use this scene:

```text
Assets/Scenes/MainScene.unity
```

## Current Scene

Keep these objects. Do not duplicate them:

```text
XR Origin (XR Rig)
Input Action Manager
EventSystem
WorldSpaceCanvas
AppManager
```

The UI is under:

```text
WorldSpaceCanvas
|-- LoginPanel
|-- ProjectListPanel
|-- FloorSelectionPanel
|-- SummaryPanel
|-- ToastPanel
```

Initial active state:

```text
LoginPanel: active
ProjectListPanel: inactive
FloorSelectionPanel: inactive
SummaryPanel: inactive
ToastPanel: inactive
```

## Scene UI Rule

Keep the dashboard as normal Unity UI objects under `WorldSpaceCanvas`.
If a panel is damaged, restore the scene from Git or recreate the panel manually using this hierarchy.

## Required Packages

Install/keep these packages:

```text
OpenXR Plugin
XR Plugin Management
XR Interaction Toolkit
Input System
TextMeshPro
UGUI / Unity UI
Universal Render Pipeline
```

You do not need Unity Cloud Services for this assignment.

## WorldSpaceCanvas Inspector

Set `WorldSpaceCanvas` like this:

```text
Render Mode: World Space
Event Camera: Main Camera
Width: 1200
Height: 720
Position: X 0, Y 1.5, Z 2.05
Rotation: X 0, Y 0, Z 0
Scale: X 0.0017, Y 0.0017, Z 0.0017
```

Keep `XR Origin (XR Rig)` at:

```text
Position: X 0, Y 0, Z 0
Rotation: X 0, Y 0, Z 0
Scale: X 1, Y 1, Z 1
```

Required components:

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

## EventSystem

Use the existing `EventSystem`.

It should have one UI input module, usually:

```text
Input System UI Input Module
```

or:

```text
XR UI Input Module
```

Do not create a second EventSystem.

## Input Action Manager

Use the existing `Input Action Manager`.

It should reference:

```text
Assets/Samples/XR Interaction Toolkit/3.0.10/Starter Assets/XRI Default Input Actions.inputactions
```

## XR Origin

Use the XRI Starter Assets rig:

```text
Assets/Samples/XR Interaction Toolkit/3.0.10/Starter Assets/Prefabs/XR Origin (XR Rig).prefab
```

Do not manually add old deprecated `XR Ray Interactor` components. XRI 3 uses the newer Starter Assets setup.

## AppManager

`AppManager` should have:

```text
Manual Xr App Controller
UI Theme Controller, optional polish
Navigation Manager
Session Manager
Project API
Auth API
API Manager
```

The controller uses Inspector references saved on `AppManager`.
Before building, confirm the login, project, floor, summary, and toast UI fields are assigned.

## Visual Style

The dashboard style is:

```text
Dark translucent rounded shell
Left navigation column
Center content area
Right details panel
Large XR-readable buttons
Blue selected state
Floating toast pill
```

Main colors:

```text
Shell: #0B1018
Nav: #111827
Surface: #1E293B
Primary blue: #3B82F6
Selected blue: #2563EB
Text: #F8FAFC
Muted text: #94A3B8
```

Recommended font direction:

```text
Best: Inter
Also good: Roboto
Current safe fallback: TextMeshPro default / Liberation Sans
```

Do not import a full font family with dozens of files before submission. If you want Inter, add only the specific TMP font assets you use.

## Optional Theme Toggle

For a light/dark polish control:

```text
Add UIThemeController to WorldSpaceCanvas or AppManager
Theme Root: WorldSpaceCanvas
Toggle Button: your bulb/icon button
Button OnClick: UIThemeController.ToggleTheme()
```

Keep the button small, for example in the top-right area of the dashboard.

## Test Order

Use this order before building the APK:

```text
1. Press Play in Unity.
2. Login with testuser / 123456.
3. Confirm ProjectListPanel opens.
4. Select a project.
5. Press Continue and confirm FloorSelectionPanel opens.
6. Select a floor and confirm it turns blue.
7. Press Continue.
8. Confirm SummaryPanel shows user, project, and floor.
9. Test Back and Start Again.
10. Test XR controller ray click on Quest 2 / Quest 3 / Quest Pro.
```

Backend URL:

```text
https://visnet-assignment.vercel.app
```

## Simulator vs Quest Testing

Use simulator testing only for fast editor checks:

```text
Unity Play Mode + XR Device Simulator
Meta XR Simulator
```

For the strongest submission, do the final test on the real headset:

```text
Meta Quest 2 / Quest 3 / Quest Pro APK install
```

The project remains an OpenXR + Unity XR Interaction Toolkit app. Meta XR Simulator is a testing option, not the main framework.

## APK Build

Build from Unity's normal Android build window:

```text
File > Build Profiles / Build Settings > Android > Build
```

Use OpenXR, IL2CPP, ARM64, and ASTC texture compression for the Quest APK.
