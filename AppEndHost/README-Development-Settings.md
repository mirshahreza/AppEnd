# Development Settings Configuration

This project supports environment-specific configuration files for local development without modifying the main `appsettings.json` file.

## How It Works

When running in **Development** environment (set via `ASPNETCORE_ENVIRONMENT=Development`), the application will:

1. First load `appsettings.json` (base configuration for server/production)
2. Then automatically merge `appsettings.Development.json` if it exists (local development overrides)
3. Development settings override matching properties in the base configuration

## Setup Instructions

### Step 1: Create Your Local Development Settings

Copy the example file to create your local development settings:

```bash
copy appsettings.Development.json.example appsettings.Development.json
```

Or manually create `appsettings.Development.json` in the `AppEndHost` folder.

### Step 2: Configure Your Local Settings

Edit `appsettings.Development.json` and customize it for your local development environment:

- **Database Connection**: Update `ConnectionString` with your local SQL Server instance
- **Secret**: Change to your local development secret (different from production)
- **API Keys**: Add your local API keys (AI services, etc.)
- **Any other settings**: Override only the settings you need for local development

### Step 3: Verify Environment Variable

Make sure `ASPNETCORE_ENVIRONMENT` is set to `Development`. This is usually configured in:

- **Visual Studio**: `Properties/launchSettings.json` (already configured)
- **Command Line**: Set the environment variable before running
- **IIS/Production**: Set to `Production` or leave unset to use base `appsettings.json`

## Example Configuration

Your `appsettings.Development.json` should look like:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AppEnd": {
    "DbServers": [
      {
        "Name": "DefaultRepo",
        "ServerType": "MsSql",
        "ConnectionString": "Data Source=.\\LOCALHOST;Initial Catalog=AppEndLearning;..."
      }
    ],
    "Secret": "MyLocalDevSecret123",
    "Ai": {
      "GitHub": {
        "ApiKey": "your-local-api-key"
      }
    }
  }
}
```

## Important Notes

1. **Git Ignore**: `appsettings.Development.json` is in `.gitignore` and will **NOT** be committed to the repository
2. **Base Configuration**: The main `appsettings.json` remains unchanged and is used for server/production
3. **Selective Override**: You only need to include settings you want to override - all other settings will use values from `appsettings.json`
4. **Merge Behavior**: 
   - Simple properties: Development values replace base values
   - Nested objects: Merged recursively (Development properties override base properties)
   - Arrays: Development arrays completely replace base arrays

## Troubleshooting

- **Settings not loading?** Check that `ASPNETCORE_ENVIRONMENT=Development` is set
- **File not found?** Ensure `appsettings.Development.json` exists in the `AppEndHost` folder
- **Changes not taking effect?** Restart the application after modifying the Development settings file
