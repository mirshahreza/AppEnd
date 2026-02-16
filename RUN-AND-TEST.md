# 🚀 Next Steps - Testing & Running Workflow Engine

**وضعیت:** ✅ 100% Configured & Ready  
**Build:** ✅ Successful (No Errors)

---

## ⚡ فوری اجرا کردن

### 1️⃣ برنامه را اجرا کنید
```bash
cd AppEndHost
dotnet run
```

**انتظار:** `Application started. Press Ctrl+C to shut down.`

---

## 🧪 تست API

### آپشن 1: PowerShell Script (سریع‌ترین)
```bash
.\test-workflow-api.ps1
```

**کاری که می‌کند:**
1. ✅ Workflow definitions را می‌خواند
2. ✅ Pending tasks را می‌خواند
3. ✅ اول task را complete می‌کند
4. ✅ نتایج updated را نشان می‌دهد

---

### آپشن 2: Browser Console (F12)

**ابتدا SQL میں تسک درج کنید:**
```sql
USE AppEnd
GO

INSERT INTO [dbo].[WorkflowTasks] 
(
    [WorkflowInstanceId], 
    [WorkflowDefinitionId], 
    [Title], 
    [Description],
    [AssignedTo],
    [Priority],
    [Status],
    [DueDate],
    [CreatedAt],
    [CreatedBy],
    [ContextData]
)
VALUES 
(
    NEWID(), 
    'order-approval', 
    'تایید سفارش #12345',
    'سفارشی به مبلغ ۲۵ میلیون تومان',
    'admin',
    'High',
    'Pending',
    DATEADD(DAY, 3, GETUTCDATE()),
    GETUTCDATE(),
    'system',
    '{"orderId": 12345, "amount": 25000000}'
)
GO
```

**سپس در Browser (F12):**
```javascript
// Get tasks
rpcAEP("GetMyWorkflowTasks", { 
    Status: "Pending",
    Page: 1,
    PageSize: 25
}, (result) => {
    console.log("✅ Tasks:", result);
});
```

**انتظار:**
```json
{
  "success": true,
  "tasks": [
    {
      "taskId": "xxx",
      "title": "تایید سفارش #12345",
      "status": "Pending",
      "priority": "High",
      ...
    }
  ],
  "totalCount": 1
}
```

---

## 📋 Configuration مختصر

### appsettings.json
```json
{
  "AppEnd": {
    "DefaultDbConfName": "DefaultRepo",
    "DbServers": [
      {
        "Name": "DefaultRepo",
        "ServerType": "MsSql",
        "ConnectionString": "Data Source=.\\SQL2025;Initial Catalog=AppEnd;..."
      }
    ],
    "Workflow": {
      "Features": { ... },
      "Persistence": {
        "ConnectionStringName": "DefaultRepo"
      }
    }
  },
  "Logging": { ... }
}
```

### ElsaSetup.cs
```csharp
public static IServiceCollection AddAppEndWorkflow(
    this IServiceCollection services, 
    IConfiguration? configuration = null)
{
    // خوانده می‌شود: AppEnd.Workflow.Persistence.ConnectionStringName
    var connectionStringName = configuration?["AppEnd:Workflow:Persistence:ConnectionStringName"] 
        ?? "DefaultRepo";
    
    var dbConf = DbConf.FromSettings(connectionStringName);
    var connectionString = dbConf.ConnectionString;
    
    // ... Elsa configuration ...
}
```

---

## 📂 UI Components قابل استفاده

### 1. WorkflowInbox.vue ✅
**محل:** `AppEndHost\workspace\client\a.SharedComponents\WorkflowInbox.vue`

**کاری که می‌کند:**
- ✅ Pending tasks را نشان می‌دهد
- ✅ Task details را باز می‌کند
- ✅ Approve/Reject می‌کند
- ✅ Auto-refresh (15s)

**نحوه استفاده:**
```vue
<WorkflowInbox :cid="componentId" />
```

### 2. WorkflowInstances.vue ✅
**محل:** `AppEndHost\workspace\client\AppEndStudio\components\WorkflowInstances.vue`

**کاری که می‌کند:**
- ✅ Running instances را نشان می‌دهد
- ✅ Instance details
- ✅ Cancel instance
- ✅ Filter by status

---

## 🔄 API Methods

### GetMyWorkflowTasks
```javascript
rpcAEP("GetMyWorkflowTasks", {
    Status: "Pending",    // optional
    Page: 1,              // default: 1
    PageSize: 25          // default: 25
}, callback)
```

### CompleteWorkflowTask
```javascript
rpcAEP("CompleteWorkflowTask", {
    TaskId: "uuid",
    Outcome: "Approve" | "Reject",
    OutputParams: { comment: "..." }
}, callback)
```

### GetWorkflowDefinitions
```javascript
rpcAEP("GetWorkflowDefinitions", {}, callback)
```

### ExecuteWorkflow
```javascript
rpcAEP("ExecuteWorkflow", {
    WorkflowId: "order-approval",
    InputParams: { orderId: "123", amount: 1000 }
}, callback)
```

---

## 🗄️ Database

**Connection String:**
```
Data Source=.\SQL2025
Initial Catalog=AppEnd
User ID=sa
Password=1
```

**جدول اصلی:** `WorkflowTasks`
**Stored Procedures:**
- `ElsaGetMyWorkflowTasks`
- `ElsaCompleteWorkflowTask`

---

## 🐛 Troubleshooting

| مشکل | حل |
|------|-----|
| "Connection timeout" | `sqlcmd -S .\SQL2025 -U sa -P 1` |
| "Task not found" | SQL میں تسک درج کنید |
| "RPC method not found" | Build را دوباره اجرا کنید |
| "Port 5000 in use" | `netstat -ano \| findstr :5000` |

---

## 📊 Architecture

```
┌─────────────────────────────────────┐
│ Browser (Vue.js)                     │
│ WorkflowInbox.vue                    │
└────────────────┬────────────────────┘
                 │ rpcAEP()
┌────────────────▼────────────────────┐
│ Zzz.AppEndProxy.Workflow.cs          │
│ GetMyWorkflowTasks()                 │
│ CompleteWorkflowTask()               │
└────────────────┬────────────────────┘
                 │
┌────────────────▼────────────────────┐
│ AppEndWorkflow.WorkflowServices      │
│ GetMyWorkflowTasks()                 │
│ CompleteWorkflowTask()               │
└────────────────┬────────────────────┘
                 │ SQL
┌────────────────▼────────────────────┐
│ SQL Server - AppEnd Database         │
│ WorkflowTasks Table                  │
│ Stored Procedures                    │
└─────────────────────────────────────┘
```

---

## 🎯 Next Steps

### فوری (15 دقیقه)
1. ✅ برنامه را اجرا کنید
2. ✅ PowerShell script اجرا کنید یا Browser console
3. ✅ Tasks را test کنید

### بعد از تست (اختیاری)
- 🔧 Custom Activities (`PHASE7-CUSTOM-ACTIVITIES.md`)
- 🎨 UI Enhancements
- 📊 Workflow Analytics

---

**شروع کنید:**
```bash
dotnet run
```

یا اگر PowerShell script دارید:
```bash
.\test-workflow-api.ps1
```

🚀 **Let's go!**
