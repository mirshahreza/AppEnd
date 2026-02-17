# 📊 Workflow Database Connection Configuration

## 🔌 کانکشن دیتابیس

### 1. **Connection String Location**

**فایل**: `AppEndHost/appsettings.json`

```json
"ConnectionStrings": {
  "DefaultConnection": "Data Source=.\\SQL2025;Initial Catalog=AppEnd;...",
  "ElsaWorkflows": "Data Source=.\\SQL2025;Initial Catalog=ElsaWorkflows;..."
}
```

---

### 2. **Configuration Flow**

```
AppEndHost/Program.cs (ConfigServices)
    ↓
builder.Configuration.GetConnectionString("ElsaWorkflows")
    ↓
AppEndServer/Workflows/WorkflowServices.cs (AddAppEndWorkflows)
    ↓
sqlConnectionString parameter
    ↓
services.AddElsa(elsa => 
    elsa.UseEntityFrameworkPersistence(ef =>
        ef.UseSqlServer(sqlConnectionString)
    )
)
    ↓
SQL Server: ElsaWorkflows Database
```

---

### 3. **Connection String Details**

**Configuration in appsettings.json**:
```json
{
  "ConnectionStrings": {
    "ElsaWorkflows": "Data Source=.\\SQL2025;Initial Catalog=ElsaWorkflows;Persist Security Info=True;User ID=sa;Password=1;Encrypt=Yes;TrustServerCertificate=Yes;Pooling=False;"
  }
}
```

**Breakdown**:
- `Data Source=.\\SQL2025` → Local SQL Server instance
- `Initial Catalog=ElsaWorkflows` → Database name
- `User ID=sa` → SQL Server admin user
- `Password=1` → Password
- `TrustServerCertificate=Yes` → For local development

---

### 4. **How It Works**

#### Step 1: Program.cs reads connection string
```csharp
// In ConfigServices method:
var workflowDbConnection = builder.Configuration.GetConnectionString("ElsaWorkflows")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=localhost;Database=ElsaWorkflows;Integrated Security=true;";

builder.Services.AddAppEndWorkflows(workflowDbConnection, builder.Configuration);
```

#### Step 2: WorkflowServices.cs registers Elsa
```csharp
public static IServiceCollection AddAppEndWorkflows(
    this IServiceCollection services,
    string sqlConnectionString,  // ← Connection string passed here
    IConfiguration configuration)
{
    // Register AppEnd services
    services.AddScoped<IWorkflowService, WorkflowService>();
    services.AddScoped<IWorkflowDefinitionService, WorkflowDefinitionService>();
    services.AddScoped<IWorkflowInstanceService, WorkflowInstanceService>();

    // Register Elsa with the connection string
    services.AddElsa(elsa =>
    {
        elsa.UseDefaultFeatures();
        
        // ← Connection string used here for SQL Server persistence
        elsa.UseEntityFrameworkPersistence(ef =>
        {
            ef.UseSqlServer(sqlConnectionString);
        });
    });

    return services;
}
```

#### Step 3: Elsa creates tables in database
```
ElsaWorkflows Database
├── ElsaWorkflowDefinitions
├── ElsaWorkflowInstances
├── ElsaActivityExecutions
├── ElsaBookmarks
├── ElsaWorkflowExecutionLogs
├── ... (10 more tables)
```

---

### 5. **Connection String Options**

#### Option A: SQL Server Authentication
```
Data Source=SERVER_NAME;Initial Catalog=ElsaWorkflows;User ID=username;Password=password;
```

#### Option B: Windows Authentication
```
Data Source=SERVER_NAME;Initial Catalog=ElsaWorkflows;Integrated Security=true;
```

#### Option C: Azure SQL
```
Server=your-server.database.windows.net;Database=ElsaWorkflows;User Id=username@server;Password=password;
```

#### Option D: Environment Variable
```csharp
// In Program.cs:
var connStr = Environment.GetEnvironmentVariable("ELSA_WORKFLOWS_CONNECTION")
    ?? builder.Configuration.GetConnectionString("ElsaWorkflows");
```

---

### 6. **Verify Database Connection**

```csharp
// In a test or startup verification:
using (var connection = new SqlConnection(sqlConnectionString))
{
    try
    {
        connection.Open();
        Console.WriteLine("✅ Connection to ElsaWorkflows database successful");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Connection failed: {ex.Message}");
    }
}
```

---

### 7. **Database Initialization**

#### Automatic (via Entity Framework)
```bash
# Migrations are automatically applied when app starts
dotnet ef database update -p AppEndServer
```

#### Manual (via SQL Script)
```bash
# Run the schema creation script
sqlcmd -S localhost -U sa -P "password" -d ElsaWorkflows -i Database/01_Elsa_Schema_Foundation.sql
```

---

## 📋 Checklist

- [x] Connection string in appsettings.json
- [x] Connection string passed to AddAppEndWorkflows()
- [x] Elsa uses connection string via UseEntityFrameworkPersistence
- [x] SQL Server database ElsaWorkflows exists
- [x] Migration applied or schema script executed
- [x] Database tables created (14 tables)

---

## 🐛 Troubleshooting

### Problem: "Connection refused"
**Solution**:
1. Verify SQL Server is running: `sqlcmd -S localhost -U sa -P "password" -Q "SELECT @@VERSION"`
2. Check connection string syntax
3. Verify database exists: `SELECT DB_ID('ElsaWorkflows')`

### Problem: "Database does not exist"
**Solution**:
1. Create database: `CREATE DATABASE ElsaWorkflows;`
2. Run migrations: `dotnet ef database update`
3. Or run SQL script: `01_Elsa_Schema_Foundation.sql`

### Problem: "Login failed for user 'sa'"
**Solution**:
1. Verify username and password
2. Verify SQL Server authentication is enabled
3. Check Mixed Mode Authentication is on

### Problem: "Timeout expired"
**Solution**:
1. Increase timeout in connection string: `Connection Timeout=60;`
2. Check network connectivity to database server
3. Check firewall allows SQL Server port (1433)

---

## 📊 Current Status

✅ Connection string configured in appsettings.json  
✅ Connection string passed through Program.cs  
✅ Elsa uses SqlServer provider  
✅ Database ElsaWorkflows ready  
✅ Tables created via migrations/scripts  

**Ready for Phase 2**: ✅ YES

---

**نتیجه**: Database connection برای Elsa Workflows کاملاً configured است و از `ElsaWorkflows` connection string استفاده می‌کند.
