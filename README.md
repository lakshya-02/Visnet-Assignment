# ViSNET Meta Quest XR Assignment

This repository contains a Unity XR application and a Vercel-ready backend for the ViSNET Meta Quest development task.

## What It Builds

The app presents a Meta Quest-style world-space UI where a user can:

1. Log in with demo credentials.
2. Load projects from a backend API.
3. Select a project.
4. Load floors dynamically for that project.
5. Select a floor.
6. Review a final summary screen.

## Demo Credentials

```text
Username: testuser
Password: 123456
```

## Backend

Backend folder:

```text
backend/
```

Local commands:

```bash
cd backend
npm install
npm run dev
npm test
npm run build
```

Required API endpoints:

```text
POST /api/login
GET /api/projects
GET /api/projects/{id}/floors
```

After deploying the backend to Vercel, update:

```text
Assets/Scripts/API/APIConfig.cs
```

with the deployed backend URL.

The Unity app is not deployed to Vercel. Vercel hosts only the backend API. The Quest app is built directly from Unity as an Android APK.

## Unity XR App

Unity version used:

```text
Unity 6000.0.70f1
```

Important packages:

```text
OpenXR
Unity XR Interaction Toolkit
XR Management
Unity Input System
TextMeshPro
UGUI
URP
```

Meta XR SDK can remain installed for Quest simulator/support tooling, but the assignment app should be presented as an OpenXR + Unity XR Interaction Toolkit implementation.

Main implementation folders:

```text
Assets/Scripts/API
Assets/Scripts/Managers
Assets/Scripts/UI
```

Recommended manual scene setup:

```text
OpenXR + Unity XR Interaction Toolkit
XR Origin / XR Rig
Left and right XR Ray Interactors
EventSystem with XR UI Input Module
World-space Canvas
LoginPanel
ProjectListPanel
FloorSelectionPanel
SummaryPanel
ToastPanel
```

Create the scene manually using Unity XR Interaction Toolkit components, then wire the panel references into `ManualXrAppController`.

## Premium UI Direction

The UI uses:

```text
Dark translucent floating panels
Large VR-readable typography
Blue selected states
Rounded inputs and buttons
Hover/pressed feedback
XR world-space toast notifications
Final configuration summary screen
```

## Final Testing Checklist

```text
Correct login opens project list
Wrong login shows invalid credentials toast
Projects load from backend API
Project selection loads dynamic floors
Floor selection highlights the selected floor
Summary screen shows user, project, and floor
Back navigation works
Controller/ray interaction works in XR
Backend tests pass
Backend production build passes
APK builds for Android/Quest
```

## AI Usage Disclosure

AI assistance was used for planning, UI direction, documentation structuring, and implementation guidance. The final Unity project setup, integration, testing, and submission build were reviewed and assembled by the developer.
