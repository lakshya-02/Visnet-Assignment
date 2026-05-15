# Unity Manual Scene Hierarchy

Use this hierarchy for the final Quest scene. The scripts already expect these objects to be wired manually through the Inspector.

```text
ViSNET_XR_Assignment
├── XR Origin (XR Rig)
│   ├── Camera Offset
│   │   ├── Main Camera
│   │   ├── Left Controller
│   │   │   ├── XR Ray Interactor
│   │   │   └── Line Renderer / XR Interactor Line Visual
│   │   └── Right Controller
│   │       ├── XR Ray Interactor
│   │       └── Line Renderer / XR Interactor Line Visual
│   └── XR Interaction Manager
├── EventSystem
│   ├── Input System UI Input Module or XR UI Input Module
│   └── Tracked Device Graphic Raycaster support
├── WorldSpaceCanvas
│   ├── LoginPanel
│   │   ├── TitleText
│   │   ├── UsernameInput
│   │   ├── PasswordInput
│   │   ├── LoginButton
│   │   └── StatusText
│   ├── ProjectListPanel
│   │   ├── TitleText
│   │   ├── ProjectListRoot
│   │   ├── ProjectButtonTemplate
│   │   ├── BackButton
│   │   └── StatusText
│   ├── FloorSelectionPanel
│   │   ├── ProjectTitleText
│   │   ├── FloorListRoot
│   │   ├── FloorButtonTemplate
│   │   ├── ContinueButton
│   │   ├── BackButton
│   │   └── StatusText
│   ├── SummaryPanel
│   │   ├── UserText
│   │   ├── ProjectText
│   │   ├── FloorText
│   │   ├── StatusText
│   │   ├── BackButton
│   │   └── RestartButton
│   └── ToastPanel
│       ├── CanvasGroup
│       └── ToastText
├── AppManager
│   ├── APIManager
│   ├── AuthAPI
│   ├── ProjectAPI
│   ├── SessionManager
│   ├── NavigationManager
│   └── ManualXrAppController
└── Environment
    ├── Soft Area Light
    └── Optional dark spatial background
```

## Inspector Wiring

Attach these components:

```text
LoginPanel -> LoginUI
ProjectListPanel -> ProjectListUI
FloorSelectionPanel -> FloorDropdownUI
SummaryPanel -> SummaryUI
ToastPanel -> ToastUI
AppManager -> APIManager, AuthAPI, ProjectAPI, SessionManager, NavigationManager, ManualXrAppController
```

Wire `ManualXrAppController` with:

```text
AuthAPI
ProjectAPI
SessionManager
NavigationManager
LoginUI
ProjectListUI
FloorDropdownUI
SummaryUI
ToastUI
```

Keep `ProjectButtonTemplate` and `FloorButtonTemplate` disabled in the scene. The controller clones them at runtime.

## XR Setup Notes

Use OpenXR as the active XR loader. Use Unity XR Interaction Toolkit for the rig, ray interactors, interaction manager, and UI interaction. Meta XR SDK can remain installed for Quest support and simulator tooling, but the implemented interaction path should be OpenXR + XR Interaction Toolkit.
