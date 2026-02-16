# ⚙️ Configuration Setup - Production Ready

**مقصد:** Application کو Production کے لیے تیار کریں  
**وقت:** 30 منٹ  
**دشکلی:** درمیانی ⭐⭐⭐

---

## مرحلہ 1: Elsa Configuration

### AppEndHost/Program.cs میں چیک کریں:

```csharp
// دیکھیں کہ یہ موجود ہے:
builder.Services.AddElsa(elsa => elsa
    .UseWorkflowServer(options =>
    {
        options.ConfigureWorkflowServer = workflowServerOptions =>
        {
            workflowServerOptions.DisableAutoMigrations = false; // ✅ Auto migrations ON
        };
    })
);
```

### Options:

```csharp
// اختیار 1: Auto Migrations ON (شروع کے لیے بہترین)
workflowServerOptions.DisableAutoMigrations = false;

// اختیار 2: Auto Migrations OFF (Production میں بہتر)
workflowServerOptions.DisableAutoMigrations = true;
// پھر migration manually کریں:
// dotnet ef database update --project AppEndWorkflow
```

---

## مرحلہ 2: AppSettings Configuration

### appsettings.json میں شامل کریں:

```json
{
  "Elsa": {
    "Features": {
      "WorkflowDefinitionStore": "DatabaseWorkflowDefinitionStore",
      "WorkflowInstanceStore": "DatabaseWorkflowInstanceStore",
      "WorkflowExecutionLogStore": "DatabaseWorkflowExecutionLogStore",
      "BookmarkStore": "DatabaseBookmarkStore"
    },
    "DisableAutoMigrations": false,
    "ActivityTypeCache": {
      "Enabled": true
    }
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Elsa": "Information",
      "AppEndWorkflow": "Information"
    }
  }
}
```

---

## مرحلہ 3: Database Connection

### appsettings.json میں:

```json
{
  "ConnectionStrings": {
    "AppEndDB": "Server=YOUR_SERVER;Database=AppEndDB;Integrated Security=true;TrustServerCertificate=true;",
    "Elsa": "Server=YOUR_SERVER;Database=AppEndDB;Integrated Security=true;TrustServerCertificate=true;"
  }
}
```

### اگر Azure SQL ہے:
```json
{
  "ConnectionStrings": {
    "AppEndDB": "Server=tcp:yourserver.database.windows.net,1433;Initial Catalog=AppEndDB;Persist Security Info=False;User ID=yourusername;Password=yourpassword;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}
```

---

## مرحلہ 4: Logging Configuration

### appsettings.Development.json:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft": "Information",
      "Elsa": "Debug",
      "AppEndWorkflow": "Debug"
    }
  }
}
```

### appsettings.Production.json:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Elsa": "Information",
      "AppEndWorkflow": "Information"
    }
  }
}
```

---

## مرحلہ 5: Email Configuration (اگر SendEmail استعمال کریں)

### appsettings.json میں:

```json
{
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "SmtpUsername": "your-email@gmail.com",
    "SmtpPassword": "your-app-password",
    "FromAddress": "noreply@yourapp.com",
    "FromName": "AppEnd Workflow",
    "EnableSsl": true
  }
}
```

### C# میں استعمال:

```csharp
var email = configuration["Email:SmtpServer"];
var port = int.Parse(configuration["Email:SmtpPort"]);
var username = configuration["Email:SmtpUsername"];
var password = configuration["Email:SmtpPassword"];
```

---

## مرحلہ 6: Workflow Services Setup

### C# میں Program.cs:

```csharp
// AppEnd Workflow setup
services.AddSingleton(sp => WorkflowServiceProvider.Create(sp));

// Set service provider for WorkflowServices
var serviceProvider = services.BuildServiceProvider();
AppEndWorkflow.WorkflowServices.SetServiceProvider(serviceProvider);

// Log configuration
LogMan.LogConsole("✅ Workflow services initialized");
```

---

## مرحلہ 7: Security Configuration

### CORS اگر صررت ہو:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

app.UseCors("AllowAll");
```

---

## مرحلہ 8: Startup Verification

### Program.cs میں startup check شامل کریں:

```csharp
// After app is built
using (var scope = app.Services.CreateScope())
{
    var dbIO = scope.ServiceProvider.GetRequiredService<DbIO>();
    
    // Check WorkflowTasks table
    try
    {
        var result = dbIO.ToScalar("SELECT COUNT(*) FROM WorkflowTasks");
        LogMan.LogConsole($"✅ WorkflowTasks table ready. Total: {result}");
    }
    catch (Exception ex)
    {
        LogMan.LogError($"❌ WorkflowTasks table not found: {ex.Message}");
    }
}
```

---

## مرحلہ 9: Monitoring & Health Check

### Health Check endpoint شامل کریں:

```csharp
builder.Services.AddHealthChecks()
    .AddDbContextCheck<YourDbContext>()
    .AddCheck("WorkflowTasks", () =>
    {
        try
        {
            var dbIO = new DbIoMsSql(DbConf.FromSettings("default"));
            var count = dbIO.ToScalar("SELECT COUNT(*) FROM WorkflowTasks");
            return count != null ? HealthStatus.Healthy : HealthStatus.Unhealthy;
        }
        catch
        {
            return HealthStatus.Unhealthy;
        }
    });

app.MapHealthChecks("/health");
```

---

## مرحلہ 10: Final Verification

```
☑️  Database connection working
☑️  WorkflowTasks table accessible
☑️  Stored procedures accessible
☑️  Elsa migrations done (if enabled)
☑️  Logging configured
☑️  Email configuration (if used)
☑️  Health checks passing
☑️  RPC endpoints responding
☑️  No startup errors
☑️  Application logs are clean
```

---

## 🚀 Startup Command

### Development:
```bash
cd C:\Workspace\Projects\AppEnd
dotnet run --project AppEndHost --configuration Development
```

### Production:
```bash
dotnet run --project AppEndHost --configuration Release
```

### Docker (اگر container میں)
```bash
docker build -t append:latest .
docker run -e ASPNETCORE_ENVIRONMENT=Production -p 5000:5000 append:latest
```

---

## 📊 Configuration Checklist

```
Elsa:
☑️  Auto-migrations configured
☑️  Workflow stores configured
☑️  Bookmark store configured
☑️  Activity type cache enabled

Database:
☑️  Connection string valid
☑️  Database accessible
☑️  Tables created
☑️  Stored procedures created

Logging:
☑️  Log level set correctly
☑️  Different levels for Dev/Prod
☑️  Workflow logging enabled
☑️  Error logs working

Security:
☑️  CORS configured (if needed)
☑️  Connection encryption enabled
☑️  Credentials in secrets manager
☑️  No hardcoded passwords

Health:
☑️  Health check endpoint working
☑️  Database health check passing
☑️  Startup checks passing
☑️  No initialization errors
```

---

## ⚠️ Production Checklist

```
Before Going Live:
☑️  DisableAutoMigrations = true (نہ کہ false)
☑️  Logging level = Information (نہ کہ Debug)
☑️  Database backed up
☑️  Error monitoring configured
☑️  Health checks deployed
☑️  SSL/TLS enabled
☑️  Secrets manager configured
☑️  Rate limiting enabled (optional)
☑️  Request logging enabled (optional)
☑️  Performance monitoring enabled
```

---

## 🐛 Common Configuration Issues

### مسئلہ: "Connection string not found"
```
حل: appsettings.json میں ConnectionStrings شامل کریں
```

### مسئلہ: "Elsa migrations failing"
```
حل: DisableAutoMigrations = false رکھیں شروع میں
```

### مسئلہ: "WorkflowServices not initialized"
```
حل: Program.cs میں SetServiceProvider() کال کریں
```

### مسئلہ: "RPC calls timing out"
```
حل: Logging enable کریں
     Server logs میں bottleneck دیکھیں
```

---

## 📞 اگلے مرحلے

**اگر Configuration مکمل ہو:**
→ Custom Activities شروع کریں (Phase 7)

**یا اگر مسائل ہوں:**
→ TESTING-GUIDE.md میں troubleshooting دیکھیں

---

یہ مرحلے مکمل ہوں تو بتائیں! ✅
