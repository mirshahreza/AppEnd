# Configuration

Configure AppEnd via `appsettings.json`, environment variables, and secrets.

## AppEndHost settings
- ConnectionStrings: database connection string for SQL Server
- Logging: providers, levels
- Security: authentication options, token lifetimes (as applicable)
- Deployment: settings for single or multi?node deployments

## Environment?specific config
- Use `appsettings.Development.json` and `appsettings.Production.json`
- Override with environment variables for containerized deployments

## Secrets
- Do not commit credentials
- Use user secrets (Development) or Azure Key Vault/secure store (Production)

## Feature flags
- Toggle experimental features safely

See also [[Deployment]] and [[Security]].