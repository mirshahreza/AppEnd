# Deployment

This page covers deployment options and steps for AppEnd.

## Targets
- Windows or Linux hosts
- Single?node or multi?node deployments
- Containers (Docker image planned)

## Build artifacts
- Publish the `AppEndHost` project
  - `dotnet publish AppEnd/AppEndHost/AppEndHost.csproj -c Release -o out`

## Configuration
- Provide environment?specific `appsettings.*.json`
- Set connection strings and secrets via environment variables or secret store

## Hosting
- Reverse proxy via IIS, Nginx, or Apache
- Enable HTTPS and HSTS
- Configure health checks if needed

## Database
- Run initialization scripts (Zzz_Deploy)
- Apply any module?specific migrations

## Monitoring
- Configure logging providers and retention
- Capture metrics and traces where available

See also [[Security]] and [[Configuration]].