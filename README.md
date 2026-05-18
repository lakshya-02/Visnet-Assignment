# ViSNET Meta Quest XR Assignment

Unity XR assignment project for a Meta Quest-style application with login, combined project/floor selection, backend API integration, XR ray interaction, toast feedback, and a final summary screen.

The project is built as:

```text
Unity Quest APK
  -> UnityWebRequest
Vercel Serverless Backend
```

The Unity application is not deployed to Vercel. Vercel hosts only the backend APIs.

## Current Stack

```text
Unity: 6000.0.70f1
XR Runtime: OpenXR
Interaction: Unity XR Interaction Toolkit
Input: Unity Input System
UI: UGUI + TextMeshPro
Rendering: URP
Backend: Next.js API routes on Vercel
```

Meta XR SDK has been removed. The app should be presented as an **OpenXR + Unity XR Interaction Toolkit** implementation.

## Live Links

```text
Backend URL: https://visnet-assignment.vercel.app
GitHub: https://github.com/lakshya-02/Visnet-Assignment
```

## Demo Credentials

```text
Username: testuser
Password: 123456
```

Only this demo user is required by the assignment PDF. Invalid credentials should simply show the failure toast.

## Required Backend APIs

```text
POST /api/login
GET /api/projects
GET /api/projects/{id}/floors
```

Test URLs:

```text
https://visnet-assignment.vercel.app/api/projects
https://visnet-assignment.vercel.app/api/projects/1/floors
```

Unity backend config:

```text
Assets/Scripts/API/APIConfig.cs
```

## Important Folders

```text
Assets/Scripts/API        UnityWebRequest API clients and response models
Assets/Scripts/Managers   Session and panel navigation state
Assets/Scripts/UI         UI binding scripts, theme toggle, and app controller
backend/                  Vercel/Next.js backend
docs/                     Report and manual scene hierarchy guide
```

## Scene Setup

`Assets/Scenes/MainScene.unity` contains the scene-authored Quest-style world-space dashboard under `WorldSpaceCanvas`.
Keep the main panels and `AppManager` references in the scene so the submission opens directly into the XR UI.

## Start From Empty Hierarchy

If starting from a fully empty Unity scene, rebuild the XR foundation in this order first.

### 1. Import XRI Samples

In Unity:

```text
Window > Package Manager > XR Interaction Toolkit > Samples
```

Import:

```text
Starter Assets
XR Device Simulator
```

After import, you should have:

```text
Assets/Samples/XR Interaction Toolkit/3.0.10/Starter Assets
Assets/Samples/XR Interaction Toolkit/3.0.10/XR Device Simulator
```

### 2. Create Core XR Objects

Create:

```text
XR Origin (XR Rig)
XR Interaction Manager
Input Action Manager
EventSystem
WorldSpaceCanvas
AppManager
Environment
```

For `Input Action Manager`:

```text
Action Assets Size: 1
Element 0: XRI Default Input Actions
```

Use the input actions from:

```text
Assets/Samples/XR Interaction Toolkit/3.0.10/Starter Assets/XRI Default Input Actions.inputactions
```

### 3. Recommended Scene Hierarchy

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
|   |-- FloorSelectionPanel
|   |-- SummaryPanel
|   |-- ToastPanel
|-- AppManager
|-- Environment
```

Full Inspector setup is documented in:

```text
docs/Unity_Manual_Scene_Hierarchy.md
```

## World Space Canvas Values

```text
Render Mode: World Space
Width: 1200
Height: 720
Position: X 0, Y 1.5, Z 2.05
Rotation: X 0, Y 0, Z 0
Scale: X 0.0017, Y 0.0017, Z 0.0017
```

Canvas components:

```text
Canvas
Canvas Scaler
Graphic Raycaster
Tracked Device Graphic Raycaster
```

Canvas Scaler:

```text
UI Scale Mode: Constant Pixel Size
Dynamic Pixels Per Unit: 10
```

Use the XRI 3 Starter Assets `XR Origin (XR Rig)` prefab as-is. Do not manually add old `XR Ray Interactor` or `XR Interactor Line Visual` components. For UI clicking, the important object is the canvas: it must have `Canvas`, `Canvas Scaler`, `Graphic Raycaster`, and `Tracked Device Graphic Raycaster`.

## UI Panels

These panels exist directly under `WorldSpaceCanvas`:

```text
LoginPanel
FloorSelectionPanel
SummaryPanel
ToastPanel
```

Attached scripts:

```text
LoginPanel -> LoginUI
FloorSelectionPanel -> FloorDropdownUI
SummaryPanel -> SummaryUI
ToastPanel -> ToastUI + CanvasGroup
```

## AppManager Setup

Create an empty GameObject:

```text
AppManager
```

Add components:

```text
APIManager
AuthAPI
ProjectAPI
SessionManager
NavigationManager
ManualXrAppController
```

In `AuthAPI`:

```text
API Manager: AppManager / APIManager
```

In `ProjectAPI`:

```text
API Manager: AppManager / APIManager
```

`ManualXrAppController` uses the Inspector references saved in `MainScene`.
Before building, select `AppManager` and confirm the login, project, floor, summary, and toast UI fields are assigned.

## UI Style

Use a clean Meta Quest-inspired VR dashboard style:

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

Typography recommendation:

```text
Best font if you have time: Inter
Good Android/Quest alternative: Roboto
Safe current fallback: TextMeshPro default / Liberation Sans
```

For final submission, it is better to keep the safe TMP fallback than to import a large unused font folder. If you import Inter, import only one regular/medium font asset and one bold font asset.

Optional theme toggle:

```text
Add UIThemeController to WorldSpaceCanvas or AppManager
Set Theme Root to WorldSpaceCanvas
Create a small bulb/icon Button
Wire Button OnClick -> UIThemeController.ToggleTheme()
```

This gives a dark/light presentation option without changing the app flow.

Recommended sizes:

```text
Main panel width: 850-950
Main panel height: 560-650
Button height: 64-76
Input height: 64-76
Title text: 44-52
Button/body text: 24-30
```

## Backend Development

```bash
cd backend
npm install
npm run dev
npm test
npm run build
```

Deploy the `backend` folder to Vercel.

## Final Testing Checklist

```text
Login with testuser / 123456 works
Invalid login shows toast
Projects load from Vercel API
Project selection highlights the card and Continue loads floors dynamically
Floor selection highlights selected floor
Summary screen shows user, project, and floor
Back navigation works
XR ray can hover and click UI
Toast fades out
APK builds for Android/Quest
```

Testing target:

```text
Editor smoke test: Unity Play Mode with Meta XR Simulator
Final required proof: install and run the APK on Meta Quest 2 / Quest 3 / Quest Pro
```

The app itself is OpenXR + Unity XR Interaction Toolkit. Meta XR Simulator is a required testing option, not the main XR framework.

## Build APK

Use Unity's normal Android build flow:

```text
Platform: Android
Texture Compression: ASTC
Scripting Backend: IL2CPP
Target Architecture: ARM64
XR Plug-in Management: OpenXR enabled
```

Recommended output name:

```text
ViSNET_Meta_Quest_XR.apk
```

## AI Usage Disclosure

AI assistance was used for planning, UI direction, documentation structuring, and implementation guidance. The final Unity project setup, integration, testing, and submission build were reviewed and assembled by the developer.
