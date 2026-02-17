# ✅ Workflow Database Configuration - DefaultRepo

**وضعیت**: ✅ **تنظیم شد برای استفاده از DefaultRepo**

---

## 📊 خلاصه تغییرات

### ❌ قبل:
```json
"ConnectionStrings": {
  "DefaultConnection": "AppEnd database",
  "ElsaWorkflows": "الیسا جداگانه در دیتابیس جدید"
}
```

### ✅ بعد:
```json
"ConnectionStrings": {
  "DefaultConnection": "AppEnd database (همان DefaultRepo)"
}
```

**نتیجه**: Elsa tables در همان **AppEnd database** ایجاد می‌شوند (نه database جدا)

---

## 🔌 Connection Flow

```
DefaultRepo (AppEnd Database)
    ↓
DefaultConnection (appsettings.json)
    ↓
Program.cs (ConfigServices)
    ↓
WorkflowServices.AddAppEndWorkflows()
    ↓
Elsa.UseEntityFrameworkPersistence(ef => ef.UseSqlServer(sqlConnectionString))
    ↓
AppEnd Database (same place where AppEnd data is stored)
```

---

## 📁 تغییرات فایل‌ها

### 1. appsettings.json ✅
```json
// ElsaWorkflows connection string حذف شد
// اضافه شد: "UseDefaultRepository": true در Workflows config

"ConnectionStrings": {
  "DefaultConnection": "Data Source=.\\SQL2025;Initial Catalog=AppEnd;..."
}

"AppEnd": {
  "Workflows": {
    "UseDefaultRepository": true,
    "Persistence": {
      "UseDefaultConnection": true
    }
  }
}
```

### 2. Program.cs ✅
```csharp
// تغییر شد:
var workflowDbConnection = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=localhost;Database=AppEnd;Integrated Security=true;";

// (قبل: ElsaWorkflows یا DefaultConnection)
```

### 3. WorkflowServices.cs ✅
```csharp
// Documentation اپدیت شد:
// "Database: AppEnd (DefaultRepo) - same database as AppEnd application"
// "Connection String: From Program.cs (appsettings.json DefaultConnection)"
```

---

## 📊 Database Structure

### قبل:
```
SQL Server
├── AppEnd Database (DefaultRepo)
│   ├── BaseActivityLog
│   ├── BasePersons
│   └── ...
└── ElsaWorkflows Database (جدا)
    ├── ElsaWorkflowDefinitions
    ├── ElsaWorkflowInstances
    └── ...
```

### بعد:
```
SQL Server
└── AppEnd Database (DefaultRepo)
    ├── BaseActivityLog
    ├── BasePersons
    ├── ElsaWorkflowDefinitions
    ├── ElsaWorkflowInstances
    ├── ElsaActivityExecutions
    └── ... (14 Elsa tables in same database)
```

---

## ✅ مزایا

1. **یک دیتابیس**: همه data در یک جا
2. **ساده‌تر**: نیازی به مدیریت دو database نیست
3. **Performance**: یک connection pool
4. **Transactions**: می‌توانیم AppEnd و Workflow transactions را یکجا manage کنیم
5. **Backup**: یک database = یک backup

---

## 🚀 اگر بعداً نیاز به جداگانگی باشد

اگر بعداً می‌خواهید Elsa tables در database جداگانه باشند:

### گزینه 1: Separate Database
```json
"ConnectionStrings": {
  "DefaultConnection": "AppEnd database",
  "ElsaWorkflows": "جدید database"
}
```
سپس Program.cs را برگردانید

### گزینه 2: Schema Separation (SQL Server)
```csharp
// Elsa tables در schema "Workflows" قرار گیرند
elsa.UseEntityFrameworkPersistence(ef =>
{
    ef.UseSqlServer(sqlConnectionString, options =>
    {
        options.MigrationsHistoryTable("__EFMigrationsHistory", "Workflows");
    });
});
```

---

## 📌 Important Notes

1. **No separate database needed**: الیسا و AppEnd در یک database هستند
2. **Migration**: وقتی Elsa packages نصب شود، migrations 14 table اضافی می‌افزایند
3. **Naming**: جداول الیسا دارای prefix "Elsa" هستند (مثل `ElsaWorkflowDefinitions`)
4. **Compatibility**: هیچ conflict با AppEnd tables نیست (different schema/prefix)

---

## ✨ Status

✅ Build: SUCCESS  
✅ Configuration: DefaultConnection (DefaultRepo)  
✅ Database: AppEnd (no separate database)  
✅ Ready for Phase 2  

---

**خلاصه**: Elsa اکنون از **DefaultConnection** استفاده می‌کند که همان **DefaultRepo** است و همان **AppEnd database**!
