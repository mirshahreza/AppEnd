# 📋 خلاصه - قبل از فاز 2 چه کار لازم است؟

---

## 🔴 5 کار حتمی (ترتیب اهمیت)

### 1️⃣ **NuGet Packages شامل Elsa**
```bash
dotnet add package Elsa --version 3.0.0
dotnet add package Elsa.Persistence.EntityFrameworkCore.SqlServer --version 3.0.0
dotnet add package Microsoft.EntityFrameworkCore.SqlServer --version 8.0.0
dotnet add package Microsoft.EntityFrameworkCore.Tools --version 8.0.0
```
⏱️ 5 دقیقه

---

### 2️⃣ **Database ایجاد کنید** (ElsaWorkflows)
```bash
sqlcmd -S localhost -U sa -P "password" -Q "CREATE DATABASE ElsaWorkflows;"
```
⏱️ 2 دقیقه

---

### 3️⃣ **Program.cs را اپدیت کنید**

**فایل**: `AppEndHost/Program.cs`

**اضافه کنید در بالا**:
```csharp
using AppEndServer.Workflows;
```

**در ConfigServices() اضافه کنید**:
```csharp
var workflowDbConnection = builder.Configuration.GetConnectionString("ElsaWorkflows")
    ?? AppEndSettings.GetConnStr("DefaultConnection");

builder.Services.AddAppEndWorkflows(workflowDbConnection, builder.Configuration);
```

⏱️ 3 دقیقه

---

### 4️⃣ **appsettings.json را اپدیت کنید**

**فایل**: `AppEndHost/appsettings.json`

**اضافه کنید**:
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

⏱️ 2 دقیقه

---

### 5️⃣ **Database Migrations را اجرا کنید**
```bash
cd AppEndHost
dotnet ef migrations add "InitializeElsaWorkflows" -p ../AppEndServer/AppEndServer.csproj
dotnet ef database update -p ../AppEndServer/AppEndServer.csproj
```
⏱️ 3 دقیقه

---

## ✅ تست کنید

```bash
# Build کنید
dotnet build
# باید موفق شود

# Application را شروع کنید
dotnet run --project AppEndHost/AppEndHost.csproj
# باید بدون خطا شروع شود
```

---

## 📊 کل زمان لازم

| کار | زمان |
|-----|------|
| NuGet packages | 5 دقیقه |
| Database | 2 دقیقه |
| Program.cs | 3 دقیقه |
| appsettings | 2 دقیقه |
| Migrations | 3 دقیقه |
| تست | 5 دقیقه |
| **جمع** | **~20 دقیقه** |

---

## 📚 راهنمای کامل

- `CRITICAL_TASKS_BEFORE_PHASE2.md` - 5 کار حتمی (دقیق)
- `PRE_PHASE2_CHECKLIST.md` - چک لیست کامل

---

## 🎯 بعد از تکمیل

وقتی همه 5 کار تمام شد و تست موفق بود:
✅ **آماده برای فاز 2 هستید!**

---

**وقت تقریبی**: 20-30 دقیقه
**سختی**: آسان (فقط کلیپ و پیست و کمند)
**خطرناک**: نه، تمام کارها safe و reversible هستند

---

## 🚀 بعدی؟

وقتی این 5 کار تمام شد، بگویید تا فاز 2 را شروع کنیم!

فاز 2 شامل:
- 🔄 Scheduler Integration
- 🔄 Event System Hooks
- 🔄 RPC Endpoints
- 🔄 Workflow Execution

---

**حاضرید شروع کنید؟** 💪
