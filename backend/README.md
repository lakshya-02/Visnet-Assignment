# ViSNET XR Backend

Serverless backend for the Meta Quest XR assignment.

## Endpoints

### `POST /api/login`

Request:

```json
{
  "username": "testuser",
  "password": "123456"
}
```

Success:

```json
{
  "success": true,
  "token": "demo_jwt_token_123",
  "user": {
    "id": 1,
    "name": "Test User"
  }
}
```

Failure:

```json
{
  "success": false,
  "message": "Invalid credentials"
}
```

### `GET /api/projects`

Returns four demo projects.

### `GET /api/projects/{id}/floors`

Returns floors for the selected project.

## Local Development

```bash
npm install
npm run dev
npm test
```

## Deploy

Import the `backend` folder into Vercel or run:

```bash
npx vercel
```

After deployment, set the same URL in Unity at `Assets/Scripts/API/APIConfig.cs`.

The Unity app is not deployed to Vercel. Vercel only hosts these backend APIs; the Quest application is built directly from Unity as an Android APK.
