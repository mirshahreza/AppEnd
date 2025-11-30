# Architecture

AppEnd is organized into logical layers and projects to keep concerns separated and enable extensibility.

## Solution structure
- `AppEndCommon`: Shared models, utilities, and abstractions
- `AppEndDbIO`: Database access layer and data utilities
- `AppEndServer`: Core application services, business logic, and API endpoints
- `AppEndDynaCode`: Dynamic code handling for low?code scenarios
- `AppEndHost`: Host project (web entry), configuration, DI, and runtime

## High-level components
- API Layer: ASP.NET Core controllers/endpoints hosted in `AppEndHost` using services from `AppEndServer`
- Service Layer: Business logic, validation, authorization checks
- Data Layer: Database access via `AppEndDbIO`, stored procedures, and scripts
- UI Layer: SPA based on Bootstrap + Vue.js 3, consuming AppEnd APIs
- Extensibility: Hooks to inject custom code both server?side and client?side

## Cross?cutting concerns
- Configuration via `appsettings.json` and environment variables
- Dependency Injection via built?in ASP.NET Core DI
- Logging and diagnostics (provider?based)
- Localization and multi?language support (client)
- Security: authentication/authorization, role and permission checks

## Request flow
1. Client calls API endpoint
2. Host routes request to controller
3. Controller delegates to service in `AppEndServer`
4. Service interacts with `AppEndDbIO` for data
5. Response mapped and returned to client

## Extensibility points
- Custom services registered in DI
- Middleware for cross?cutting needs
- Custom Vue components and pages
- Event hooks for CRUD lifecycle (pre/post create/update/delete)

See [[API Development]] and [[UI Development]] for details.