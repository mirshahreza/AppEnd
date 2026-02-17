# Phase 1: Foundation - Installation & Setup Guide

## Overview
This guide walks through the final installation steps to complete Phase 1 and prepare for Phase 2.

---

## What's Been Completed

✅ Service layer architecture (interfaces & implementations)  
✅ Dependency injection setup  
✅ Data transfer objects (DTOs)  
✅ Sample workflow templates  
✅ Complete documentation  

**Current Status**: Code compiles successfully but Elsa runtime is not yet integrated.

---

## Installation Steps

### Step 1: Add NuGet Packages

Update `AppEndServer.csproj` to include Elsa dependencies:

```xml
<ItemGroup>
    <!-- Elsa Workflow Engine (Core) -->
    <PackageReference Include="Elsa" Version="3.0.0" />
    
    <!-- Elsa Persistence with Entity Framework Core -->
    <PackageReference Include="Elsa.Persistence.EntityFrameworkCore" Version="3.0.0" />
    <PackageReference Include="Elsa.Persistence.EntityFrameworkCore.SqlServer" Version="3.0.0" />
    
    <!-- Elsa Web API and Dashboard -->
    <PackageReference Include="Elsa.Api.Endpoints" Version="3.0.0" />
    <PackageReference Include="Elsa.Management.Api.Endpoints" Version="3.0.0" />
    
    <!-- Optional: Logging Integration -->
    <PackageReference Include="Elsa.Logging.Serilog" Version="3.0.0" />
    
    <!-- Entity Framework Core -->
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0" />
</ItemGroup>
```

Then restore packages:
```bash
dotnet restore AppEndServer/AppEndServer.csproj
```

### Step 2: Update Program.cs

Add the following to `AppEndHost/Program.cs`:

**At the top, add using statement:**
```csharp
using AppEndServer.Workflows;
```

**In `ConfigServices()` method, add (after existing service registrations):**
```csharp
static WebApplicationBuilder ConfigServices(WebApplicationBuilder builder)
{
    // ... existing CORS, compression, scheduler registrations ...
    
    // NEW: Add Elsa Workflow Engine
    var workflowConnectionString = builder.Configuration.GetConnectionString("ElsaWorkflows")
        ?? AppEndSettings.GetConnStr("DefaultConnection"); // Fallback to default if not specified
    
    builder.Services.AddAppEndWorkflows(workflowConnectionString, builder.Configuration);
    
    return builder;
}
```

### Step 3: Configure appsettings.json

Add Elsa configuration to `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=AppEnd;Integrated Security=true;TrustServerCertificate=true;",
    "ElsaWorkflows": "Server=localhost;Database=ElsaWorkflows;Integrated Security=true;TrustServerCertificate=true;"
  },
  "Elsa": {
    "Features": {
      "EnableWorkflowDefinitions": true,
      "EnableWorkflowInstances": true,
      "EnableApprovals": true,
      "EnableMonitoring": true
    }
  }
}
```

Or use environment variables for production:
```bash
# Set in .env or deployment config:
ConnectionStrings__ElsaWorkflows=Server=prod-server;Database=ElsaWorkflows;User Id=sa;Password=xxxxx;
```

### Step 4: Create/Update Database

Run Entity Framework migrations to create Elsa tables:

```bash
# Create migration for Elsa
cd AppEndHost
dotnet ef migrations add "InitializeElsaWorkflows" -p ../AppEndServer/AppEndServer.csproj -c ElsaDbContext

# Apply migration to database
dotnet ef database update -p ../AppEndServer/AppEndServer.csproj -c ElsaDbContext
```

Or if using a separate ElsaWorkflows database:
```bash
# Create initial database
sqlcmd -S localhost -U sa -P "<your-password>" -Q "CREATE DATABASE ElsaWorkflows;"

# Apply migrations
dotnet ef database update -p ../AppEndServer/AppEndServer.csproj -c ElsaDbContext -s .
```

### Step 5: Verify Installation

Create a simple test to verify Elsa is registered:

**Create `TestWorkflowIntegration.cs` in AppEndHost for testing:**

```csharp
using AppEndServer.Workflows;
using Microsoft.Extensions.DependencyInjection;

// In a test or startup verification method:
var services = ServiceProvider.GetRequiredService<IWorkflowService>();
var definitions = services.Definitions;
var instances = services.Instances;

Console.WriteLine("✅ IWorkflowService registered successfully");
```

### Step 6: Build and Test

```bash
# Build solution
dotnet build

# Run tests
dotnet test

# Start application
dotnet run --project AppEndHost/AppEndHost.csproj
```

You should see in the logs:
```
[Information] Elsa 3.0 Workflow Engine initialized successfully
[Information] Workflow services registered
[Information] Database context configured with SQL Server
```

---

## Troubleshooting

### Issue: "IWorkflowService not registered"
**Solution**: Verify `AddAppEndWorkflows()` is called in `Program.cs` ConfigServices method.

### Issue: "Database migration failed"
**Solution**: 
1. Ensure connection string is correct
2. SQL Server is running and accessible
3. User has CREATE DATABASE permissions
4. Check firewall allows SQL Server port (1433)

### Issue: "Elsa tables not created"
**Solution**:
1. Verify migration was applied: `dotnet ef database update`
2. Check ElsaWorkflows database exists
3. Run SQL script manually: `01_Elsa_Schema_Foundation.sql`

### Issue: "Connection string error"
**Solution**:
```csharp
// Verify in Program.cs:
var connStr = builder.Configuration.GetConnectionString("ElsaWorkflows");
if (string.IsNullOrEmpty(connStr))
    Console.WriteLine("Warning: Using default connection for Elsa");
```

---

## Verification Checklist

After installation, verify:

- [ ] **Build succeeds** without errors
- [ ] **NuGet packages installed** (check `.csproj` file or `obj/project.assets.json`)
- [ ] **appsettings.json** has ElsaWorkflows connection string
- [ ] **Program.cs** calls `AddAppEndWorkflows()`
- [ ] **Database migration** applied (check database schema)
- [ ] **Application starts** without Elsa-related errors
- [ ] **IWorkflowService** resolves from DI container
- [ ] **Logs show** Elsa initialization messages

---

## Next Steps: Phase 2

Once Phase 1 is verified working:

1. ✅ **Phase 1 Complete**: Foundation is ready
2. **Phase 2**: Integration with AppEnd's scheduler and events
   - Hook into `SchedulerService`
   - Listen to workflow events
   - Add RPC endpoints for management

---

## File Structure After Installation

```
AppEndServer/
├── Workflows/                           # Phase 1 deliverables
│   ├── IWorkflowService.cs              
│   ├── IWorkflowDefinitionService.cs    
│   ├── IWorkflowInstanceService.cs      
│   ├── WorkflowService.cs               
│   ├── WorkflowDefinitionService.cs     
│   ├── WorkflowInstanceService.cs       
│   ├── WorkflowServices.cs              # DI registration
│   ├── README.md                        
│   ├── PHASE_1_COMPLETION.md            
│   ├── PROGRAM_CS_INTEGRATION_INSTRUCTIONS.txt
│   ├── INSTALLATION_SETUP_GUIDE.md      # This file
│   └── Samples/
│       └── SimpleApprovalWorkflow.cs    # Workflow templates
│
├── AppEndServer.csproj                  # Updated with Elsa packages
└── ... (existing files)

AppEndHost/
├── Program.cs                           # Updated with Elsa services
├── appsettings.json                     # Updated with Elsa config
└── ... (existing files)

Database/
├── 01_Elsa_Schema_Foundation.sql        # Table definitions
├── 02_Elsa_System_Tables.sql            
├── 03_Elsa_Indexes_Constraints.sql      
└── 04_Elsa_Monitoring_Queries.sql       # Monitoring & management
```

---

## Configuration Reference

### AppSettings Sections

```json
{
  "Elsa": {
    "Version": "3.0.0",
    "Features": {
      "EnableWorkflowDefinitions": true,
      "EnableWorkflowInstances": true,
      "EnableApprovals": true,
      "EnableScheduling": true,
      "EnableMonitoring": true
    },
    "Persistence": {
      "Provider": "EntityFrameworkCore",
      "Database": "SqlServer"
    },
    "Security": {
      "RequireAuthentication": false,
      "TenantResolutionStrategy": "HeaderBased"
    }
  }
}
```

### Connection String Formats

**Local Development:**
```
Server=localhost;Database=ElsaWorkflows;Integrated Security=true;TrustServerCertificate=true;
```

**Docker:**
```
Server=elsa-db,1433;Database=ElsaWorkflows;User Id=sa;Password=YourStrong!Password;Encrypt=true;TrustServerCertificate=true;
```

**Azure SQL:**
```
Server=append.database.windows.net;Database=ElsaWorkflows;User Id=admin@append;Password=xxxxx;Encrypt=true;Connection Timeout=30;
```

---

## Performance Tuning (Optional)

For production deployments, consider:

1. **Connection Pooling** (via connection string):
   ```
   Min Pool Size=10;Max Pool Size=100;
   ```

2. **Command Timeout**:
   ```csharp
   context.Database.SetCommandTimeout(60);
   ```

3. **Query Optimization**:
   - Add indexes for common filters (TenantId, Status, CreatedAt)
   - Archive old workflow instances regularly
   - Implement soft-delete strategy

4. **Monitoring**:
   - Enable Elsa monitoring endpoints
   - Track workflow execution metrics
   - Monitor database performance

---

## Summary

You now have:
- ✅ All Phase 1 code files compiled and ready
- ✅ Service registration setup
- ✅ Database schema scripts
- ✅ Configuration templates
- ✅ Installation steps documented

**Ready to proceed to Phase 2: Integration** where we connect Elsa with AppEnd's existing infrastructure!

---

**Questions?** Refer to:
- `README.md` - Architecture overview
- `PROGRAM_CS_INTEGRATION_INSTRUCTIONS.txt` - Integration details  
- `PHASE_1_COMPLETION.md` - What was delivered
- Elsa Documentation: https://v3.elsaworkflows.io/
