# ✅ Phase 1 → Phase 2 Transition Checklist

**هدف**: اطمینان از آمادگی کامل قبل از شروع فاز 2 (Integration)

---

## 🔍 Pre-Phase 2 Verification Checklist

### 1️⃣ Code Review & Architecture Approval
**Status**: ⏳ Required

- [ ] **Code Review**
  - [ ] Interfaces reviewed (IWorkflowService, etc.)
  - [ ] Service implementations reviewed
  - [ ] DTO models reviewed
  - [ ] No issues found

- [ ] **Architecture Approval**
  - [ ] Service layer pattern approved
  - [ ] Database design approved
  - [ ] DI registration approach approved
  - [ ] Multi-tenancy strategy approved

- [ ] **Documentation Verification**
  - [ ] All guides are clear and complete
  - [ ] Code examples are accurate
  - [ ] No typos or formatting issues

### 2️⃣ Project Setup
**Status**: ⏳ Required

- [ ] **Add NuGet Packages** to `AppEndServer.csproj`
  ```xml
  <PackageReference Include="Elsa" Version="3.0.0" />
  <PackageReference Include="Elsa.Persistence.EntityFrameworkCore.SqlServer" Version="3.0.0" />
  <PackageReference Include="Elsa.Api.Endpoints" Version="3.0.0" />
  <PackageReference Include="Elsa.Management.Api.Endpoints" Version="3.0.0" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
  <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0" />
  ```

- [ ] **Run Package Restore**
  ```bash
  dotnet restore AppEndServer/AppEndServer.csproj
  ```

- [ ] **Verify Build**
  ```bash
  dotnet build
  ```

### 3️⃣ Configuration Setup
**Status**: ⏳ Required

- [ ] **Update `appsettings.json`**
  ```json
  {
    "ConnectionStrings": {
      "DefaultConnection": "Server=localhost;Database=AppEnd;...",
      "ElsaWorkflows": "Server=localhost;Database=ElsaWorkflows;..."
    },
    "Elsa": {
      "Features": {
        "EnableWorkflowDefinitions": true,
        "EnableWorkflowInstances": true
      }
    }
  }
  ```

- [ ] **Verify Connection Strings**
  - [ ] DefaultConnection works
  - [ ] ElsaWorkflows connection works
  - [ ] SQL Server is accessible

### 4️⃣ Database Setup
**Status**: ⏳ Required

- [ ] **Create ElsaWorkflows Database**
  ```bash
  # Option 1: Using Azure Data Studio or SSMS
  CREATE DATABASE ElsaWorkflows;
  
  # Option 2: Using sqlcmd
  sqlcmd -S localhost -U sa -P "password" -Q "CREATE DATABASE ElsaWorkflows;"
  ```

- [ ] **Run Entity Framework Migrations**
  ```bash
  cd AppEndHost
  dotnet ef migrations add "InitializeElsaWorkflows" -p ../AppEndServer/AppEndServer.csproj
  dotnet ef database update -p ../AppEndServer/AppEndServer.csproj
  ```

- [ ] **OR Run SQL Script Directly**
  ```bash
  # If migrations don't work, run the SQL scripts:
  sqlcmd -S localhost -U sa -P "password" -d ElsaWorkflows -i Database/01_Elsa_Schema_Foundation.sql
  ```

- [ ] **Verify Table Creation**
  ```sql
  SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
  WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME LIKE 'Elsa%'
  ```

### 5️⃣ Program.cs Integration
**Status**: ⏳ Required

- [ ] **Add Using Statement**
  ```csharp
  using AppEndServer.Workflows;
  ```

- [ ] **Update ConfigServices Method**
  ```csharp
  static WebApplicationBuilder ConfigServices(WebApplicationBuilder builder)
  {
      // ... existing services ...
      
      // NEW: Add Elsa Workflow Engine
      var connStr = builder.Configuration.GetConnectionString("ElsaWorkflows")
          ?? AppEndSettings.GetConnStr("DefaultConnection");
      
      builder.Services.AddAppEndWorkflows(connStr, builder.Configuration);
      
      return builder;
  }
  ```

- [ ] **Update appsettings for Development**
  - [ ] Set correct connection string
  - [ ] Enable logging if needed

### 6️⃣ Build & Runtime Verification
**Status**: ⏳ Required

- [ ] **Clean Build**
  ```bash
  dotnet clean
  dotnet build
  ```

- [ ] **Verify Build Output**
  - [ ] 0 errors
  - [ ] 0 warnings
  - [ ] All projects built

- [ ] **Run Application**
  ```bash
  dotnet run --project AppEndHost/AppEndHost.csproj
  ```

- [ ] **Check Startup Logs**
  - [ ] Application starts without errors
  - [ ] Elsa services registered
  - [ ] Database connection successful
  - [ ] No Elsa-related exceptions

- [ ] **Verify DI Resolution**
  ```csharp
  // In a test or controller:
  var service = serviceProvider.GetRequiredService<IWorkflowService>();
  Assert.NotNull(service);
  ```

### 7️⃣ Documentation & Team Readiness
**Status**: ⏳ Required

- [ ] **Team Onboarding**
  - [ ] All developers reviewed QUICK_START.md
  - [ ] All developers understand the architecture
  - [ ] Q&A session completed

- [ ] **Repository Preparation**
  - [ ] Code committed to feature branch
  - [ ] Create pull request
  - [ ] Code review completed
  - [ ] Approved and merged to develop

- [ ] **Phase 2 Planning**
  - [ ] Phase 2 tasks defined
  - [ ] Scheduler integration requirements clear
  - [ ] Event system integration requirements clear
  - [ ] RPC endpoints defined

### 8️⃣ Backup & Safety
**Status**: ⏳ Recommended

- [ ] **Database Backup**
  - [ ] Existing AppEnd database backed up
  - [ ] New ElsaWorkflows database backed up

- [ ] **Code Backup**
  - [ ] All changes committed to Git
  - [ ] Branch is pushed to remote
  - [ ] Main branch is protected

---

## 📋 Detailed Tasks

### Task 1: Install NuGet Packages
```bash
# Navigate to AppEndServer directory
cd AppEndServer

# Add required packages
dotnet add package Elsa --version 3.0.0
dotnet add package Elsa.Persistence.EntityFrameworkCore.SqlServer --version 3.0.0
dotnet add package Elsa.Api.Endpoints --version 3.0.0
dotnet add package Elsa.Management.Api.Endpoints --version 3.0.0
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.0

# Restore all packages
cd ..
dotnet restore
```

### Task 2: Create Database
```bash
# Option A: Using SSMS or Azure Data Studio
# Connect to SQL Server
# Right-click Databases → New Database
# Name: ElsaWorkflows
# Click OK

# Option B: Using sqlcmd
sqlcmd -S localhost -U sa -P "YourPassword" -Q "CREATE DATABASE ElsaWorkflows;"

# Option C: Using SQL Script
# Open Database/01_Elsa_Schema_Foundation.sql in SSMS
# Connect to ElsaWorkflows database
# Execute script
```

### Task 3: Apply Entity Framework Migrations
```bash
# From AppEndHost directory
cd AppEndHost

# Create migration
dotnet ef migrations add "InitializeElsaWorkflows" -p ../AppEndServer/AppEndServer.csproj --verbose

# Update database
dotnet ef database update -p ../AppEndServer/AppEndServer.csproj --verbose

# If issues occur, check the migration file:
# AppEndServer/Migrations/[Timestamp]_InitializeElsaWorkflows.cs
```

### Task 4: Update Program.cs
```csharp
// File: AppEndHost/Program.cs

// Add at top:
using AppEndServer.Workflows;

// In ConfigServices method, add:
static WebApplicationBuilder ConfigServices(WebApplicationBuilder builder)
{
    builder.Services.AddCors(...);
    builder.Services.Configure(...);
    builder.Services.AddResponseCompression(...);
    
    // Add Elsa services
    var workflowDbConnection = builder.Configuration.GetConnectionString("ElsaWorkflows")
        ?? AppEndSettings.GetConnStr("DefaultConnection");
    builder.Services.AddAppEndWorkflows(workflowDbConnection, builder.Configuration);
    
    builder.Services.AddSingleton<SchedulerService>();
    builder.Services.AddSingleton<SchedulerManager>();
    builder.Services.AddHostedService(sp => sp.GetRequiredService<SchedulerService>());
    
    return builder;
}
```

### Task 5: Update appsettings.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Elsa": "Information"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=AppEnd;Integrated Security=true;TrustServerCertificate=true;",
    "ElsaWorkflows": "Server=localhost;Database=ElsaWorkflows;Integrated Security=true;TrustServerCertificate=true;"
  },
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
    }
  }
}
```

### Task 6: Verify Installation
```bash
# Clean and build
dotnet clean
dotnet build

# Check for errors
echo "Build completed. Check for any errors above."

# Start application
dotnet run --project AppEndHost/AppEndHost.csproj

# Look for messages like:
# [Information] Workflow services registered
# [Information] Database connected to Elsa
# [Information] Application started successfully
```

---

## 🐛 Common Issues & Solutions

### Issue: "IWorkflowRuntime not registered"
**Solution**:
1. Verify NuGet packages are installed
2. Check Program.cs has AddAppEndWorkflows() call
3. Ensure using statement is present

### Issue: "Database connection failed"
**Solution**:
1. Verify ElsaWorkflows database exists
2. Check connection string is correct
3. Verify SQL Server is running: `sqlcmd -S localhost -U sa -P "password" -Q "SELECT 1"`

### Issue: "Migration failed"
**Solution**:
1. Delete any partial migrations
2. Run: `dotnet ef database drop` (careful!)
3. Run: `dotnet ef database update`
4. Or run SQL script manually

### Issue: "Type not found: WorkflowBase"
**Solution**:
1. NuGet packages not installed yet
2. Install: `dotnet add package Elsa`
3. Run: `dotnet restore`

---

## ✅ Pre-Phase 2 Sign-Off

**Before starting Phase 2, verify all checklist items are complete:**

- [ ] All code reviews passed
- [ ] Architecture approved
- [ ] NuGet packages installed
- [ ] Database created and initialized
- [ ] Program.cs updated
- [ ] appsettings.json configured
- [ ] Build successful (0 errors)
- [ ] Application runs without errors
- [ ] DI container resolves IWorkflowService
- [ ] Team is ready

**If all items are checked**: ✅ **Ready for Phase 2!**

---

## 📞 Troubleshooting Support

**Common Commands:**

```bash
# Check NuGet packages
dotnet list AppEndServer package

# Verify build
dotnet build --verbose

# Test database connection
sqlcmd -S localhost -U sa -P "password" -Q "SELECT @@VERSION"

# List Entity Framework migrations
dotnet ef migrations list -p AppEndServer

# Rollback migration
dotnet ef migrations remove -p AppEndServer

# Check service registration
dotnet add package Microsoft.Extensions.DependencyInjection
```

---

## 🎯 Phase 2 Requirements (Once Phase 1 Pre-Reqs Complete)

Phase 2 will require:
1. ✅ All Phase 1 items complete (from above)
2. 🔄 Scheduler service integration
3. 🔄 Event system hookup
4. 🔄 RPC endpoint creation
5. 🔄 Workflow execution logic

---

**Status**: ⏳ **AWAITING COMPLETION OF CHECKLIST**

Once all items above are completed, Phase 2 Integration is ready to start!

---

**Next**: Complete this checklist, then we proceed to Phase 2! 🚀
