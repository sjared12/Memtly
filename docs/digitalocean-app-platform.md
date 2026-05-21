# Deploying to DigitalOcean App Platform

This repository includes a DigitalOcean App Platform spec at `.do/app.yaml`.

## What was changed

- The app now binds to `PORT` when provided by the platform (falls back to `5000` locally).
- The Docker image now exposes `8080`.
- App Platform spec configured to use Dockerfile-based deployment:
  - Build from `Memtly.Community/Dockerfile` (not buildpack)
  - Health check at `/`
  - Persistent volumes for `/app/config` (2 GB) and `/app/wwwroot/uploads` (20 GB)

## Critical: Use Dockerfile, not buildpack

The `Memtly.Core.csproj` runs `npm ci` during build. The Dockerfile handles this by pre-installing Node.js. If the buildpack is used instead, the build will fail with `npm: not found`.

The `.do/app.yaml` is configured to force Dockerfile usage. However:
- If you created the app before this update, **you must delete and recreate it** using the new app.yaml
- Or manually configure it in the App Platform UI to use Dockerfile source

## Before you deploy

1. **Delete the existing app** on DigitalOcean (if you have one from the earlier failed build).
2. Ensure the `Memtly.Core` submodule uses the correct GitHub URL (we fixed this—verify `.gitmodules` has `https://github.com/Memtly/memtly.core.git`).
3. Update `.do/app.yaml` and set `services[0].github.repo` and `services[0].github.branch` to your repo.
4. Review and set production app settings as environment variables in App Platform (admin credentials, encryption keys, SMTP config, etc.).

## Deploy with doctl

1. Install and authenticate `doctl`.
2. **Delete the old app** (if any): `doctl apps delete <APP_ID>`
3. From the repository root, create the app with the updated spec:

```bash
doctl apps create --spec .do/app.yaml
```

4. For future updates to the spec:

```bash
doctl apps update <APP_ID> --spec .do/app.yaml
```

## Deploy with DigitalOcean UI

1. **Delete the existing app** (if any).
2. Click "Create App" → select "GitHub" source
3. Choose your repository and branch
4. When prompted for build settings:
   - **Source Type:** Dockerfile
   - **Dockerfile:** `Memtly.Community/Dockerfile`
   - **Port:** `8080`
5. Add runtime environment variables:
   - `ASPNETCORE_ENVIRONMENT=Production`
   - `PORT=8080`
6. Add two volumes in the service configuration:
   - `/app/config` (at least 2 GB)
   - `/app/wwwroot/uploads` (size based on expected media)
7. Review health check (should default to `/`) and create the app.

## Troubleshooting

- **npm: not found error**: App is using buildpack instead of Dockerfile. Delete and recreate the app with the Dockerfile source.
- **Submodule clone fails**: Verify `.gitmodules` contains `https://github.com/Memtly/memtly.core.git` (not a relative path).
