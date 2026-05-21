# Deploying to DigitalOcean App Platform

This repository now includes a DigitalOcean App Platform spec at `.do/app.yaml`.

## What was changed

- The app now binds to `PORT` when provided by the platform (falls back to `5000` locally).
- The Docker image now exposes `8080`.
- App Platform spec includes:
  - Dockerfile-based deploy from `Memtly.Community/Dockerfile`
  - Health check at `/`
  - Persistent volumes for `/app/config` and `/app/wwwroot/uploads`

## Before you deploy

1. Ensure the `Memtly.Core` submodule is available in your repository.
2. Update `.do/app.yaml` and set `services[0].github.repo` and `services[0].github.branch` for your repo.
3. Review and set production app settings as environment variables in App Platform (admin credentials, encryption keys, SMTP config, etc.).

## Deploy with doctl

1. Install and authenticate `doctl`.
2. From the repository root run:

```bash
doctl apps create --spec .do/app.yaml
```

3. For updates after editing the spec:

```bash
doctl apps update <APP_ID> --spec .do/app.yaml
```

## Deploy with DigitalOcean UI

1. Create App from GitHub repository.
2. Select Dockerfile source and point to `Memtly.Community/Dockerfile`.
3. Set HTTP port to `8080`.
4. Add two volumes:
   - `/app/config` (at least 2 GB)
   - `/app/wwwroot/uploads` (size based on expected media)
5. Add runtime env vars, including:
   - `ASPNETCORE_ENVIRONMENT=Production`
   - `PORT=8080`

## Notes

- If your root path requires authentication in production, adjust health check path in `.do/app.yaml` to a public endpoint.
- This setup keeps SQLite and uploaded files across deploys by using App Platform volumes.
