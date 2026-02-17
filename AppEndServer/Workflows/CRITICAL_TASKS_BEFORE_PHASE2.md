# ⚠️ Critical Tasks Before Phase 2

**Priority**: 🔴 HIGH - Must Complete Before Proceeding

---

## 🎯 5 Critical Items (Do These First)

### 1. ❌ → ✅ Add NuGet Packages

**Why**: Code references Elsa types that don't exist without packages

**Action**:
```bash
cd AppEndServer
dotnet add package Elsa --version 3.0.0
dotnet add package Elsa.Persistence.EntityFrameworkCore.SqlServer --version 3.0.0
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.0
cd ..
dotnet restore
```

**Verify**: `dotnet build` should succeed

---

### 2. ❌ → ✅ Create SQL Database

**Why**: Workflow data needs a place to store

**Action**:
```bash
# Option 1: Using Azure Data Studio
# 1. Connect to SQL Server
# 2. Right-click Databases → New Database
# 3. Name: ElsaWorkflows
# 4. Click Create

# Option 2: Using command line
sqlcmd -S localhost -U sa -P "YourPassword" -Q "CREATE DATABASE ElsaWorkflows;"
```

**Verify**: 
```sql
SELECT DB_ID('ElsaWorkflows')  -- Should return non-null
```

---

### 3. ❌ → ✅ Update Program.cs

**Why**: Services need to be registered

**File**: `AppEndHost/Program.cs`

**Add at top**:
```csharp
using AppEndServer.Workflows;
```

**In ConfigServices() method, add** (after existing services):
```csharp
// NEW: Add Elsa Workflow Engine
var workflowDbConnection = builder.Configuration.GetConnectionString("ElsaWorkflows")
    ?? AppEndSettings.GetConnStr("DefaultConnection");

builder.Services.AddAppEndWorkflows(workflowDbConnection, builder.Configuration);
```

**Before return builder;**

---

### 4. ❌ → ✅ Update appsettings.json

**Why**: Database connection string needed

**File**: `AppEndHost/appsettings.json`

**Add**:
```json
"ConnectionStrings": {
  "ElsaWorkflows": "Server=localhost;Database=ElsaWorkflows;Integrated Security=true;TrustServerCertificate=true;"
},
"Elsa": {
  "Features": {
    "EnableWorkflowDefinitions": true,
    "EnableWorkflowInstances": true
  }
}
```

**Note**: Change `localhost` if using remote server

---

### 5. ❌ → ✅ Run Database Migrations

**Why**: Tables need to be created

**Action**:
```bash
cd AppEndHost

# Create migration
dotnet ef migrations add "InitializeElsaWorkflows" -p ../AppEndServer/AppEndServer.csproj

# Apply migration
dotnet ef database update -p ../AppEndServer/AppEndServer.csproj
```

**Verify**:
```sql
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_SCHEMA = 'dbo' 
AND TABLE_NAME LIKE 'Elsa%'
```

Should show 14 tables

---

## 🔍 Verification Steps

After completing the 5 items above:

### Step 1: Build
```bash
dotnet clean
dotnet build
```
✅ Should show: `Build succeeded`

### Step 2: Run Application
```bash
dotnet run --project AppEndHost/AppEndHost.csproj
```
✅ Should show application starting

### Step 3: Check Logs
Look for messages like:
```
[Information] Workflow services registered
[Information] Database connected successfully
[Information] Application started
```

### Step 4: Verify Database
```sql
-- In ElsaWorkflows database
SELECT COUNT(*) FROM ElsaWorkflowDefinitions;  -- Should return 0
SELECT COUNT(*) FROM ElsaWorkflowInstances;     -- Should return 0
```

### Step 5: Test DI Resolution
In a test or controller:
```csharp
var workflows = serviceProvider.GetRequiredService<IWorkflowService>();
Assert.IsNotNull(workflows);  // Should pass
```

---

## ⏱️ Time Estimate

| Task | Time |
|------|------|
| Add NuGet packages | 5 min |
| Create database | 2 min |
| Update Program.cs | 3 min |
| Update appsettings | 2 min |
| Run migrations | 3 min |
| Verification | 5 min |
| **TOTAL** | **~20 min** |

---

## 🚨 If Something Fails

### Build fails?
1. Did you run `dotnet restore`?
2. Are packages actually installed? `dotnet list AppEndServer package | grep Elsa`
3. Try clean: `dotnet clean && dotnet build`

### Database fails?
1. Is SQL Server running? `sqlcmd -S localhost -Q "SELECT @@VERSION"`
2. Did you create ElsaWorkflows database?
3. Is connection string correct in appsettings.json?

### Migration fails?
1. Delete migration: `dotnet ef migrations remove -p AppEndServer`
2. Try again: `dotnet ef migrations add InitializeElsaWorkflows -p AppEndServer`
3. Or run SQL manually: `01_Elsa_Schema_Foundation.sql`

### App won't start?
1. Check appsettings.json for JSON syntax errors
2. Verify connection string
3. Check Program.cs for syntax errors
4. Review application logs

---

## ✅ Checklist Before Declaring "Ready for Phase 2"

- [ ] All 5 critical items complete
- [ ] `dotnet build` succeeds
- [ ] Application starts without errors
- [ ] Database has 14 Elsa tables
- [ ] appsettings.json configured
- [ ] Program.cs integrated
- [ ] Logs show successful startup
- [ ] DI container resolves IWorkflowService

---

## 📌 Important Notes

1. **Don't skip steps** - They're in order of dependency
2. **Save connection strings** - You'll need them for Phase 2
3. **Backup database** - Good practice before migrations
4. **Commit to Git** - Track all changes
5. **Document any issues** - For troubleshooting

---

## 🎯 After Completion

Once all 5 items and verification steps are done:
1. ✅ Phase 1 pre-requisites complete
2. 🚀 Ready to start Phase 2: Integration

**Total time to Phase 2 ready**: ~20-30 minutes

---

**Status**: ⏳ **WAITING FOR COMPLETION**

When done, let me know and we'll start Phase 2! 🚀
