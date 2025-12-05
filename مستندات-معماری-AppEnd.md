<!--
Document Direction: Right-to-Left (RTL)
Language: Persian (Farsi)
This document should be displayed with RTL direction
-->

<div dir="rtl" lang="fa">

# مستندات جامع معماری و روش‌های کار AppEnd Framework

## فهرست مطالب
1. [مقدمه](#مقدمه)
2. [معرفی AppEnd](#معرفی-append)
3. [معماری کلی سیستم](#معماری-کلی-سیستم)
4. [ماژول‌های پروژه](#ماژولهای-پروژه)
5. [جریان کار (Workflow)](#جریان-کار-workflow)
6. [ویژگی‌های کلیدی](#ویژگیهای-کلیدی)
7. [نحوه ایجاد یک اپلیکیشن](#نحوه-ایجاد-یک-اپلیکیشن)
8. [نحوه توسعه به عنوان توسعه‌دهنده](#نحوه-توسعه-به-عنوان-توسعهدهنده)
9. [دیباگ و تغییرات](#دیباگ-و-تغییرات)
10. [ساختار فایل‌ها و پوشه‌ها](#ساختار-فایلها-و-پوشهها)
11. [تنظیمات کامل (Configuration Reference)](#تنظیمات-کامل-configuration-reference)
12. [UI Widgets و Components](#ui-widgets-و-components)
13. [Compare Operators](#compare-operators)
14. [Relations (ارتباطات بین جداول)](#relations-ارتباطات-بین-جداول)
15. [ValueSharp System](#valuesharp-system)
16. [Package Manager](#package-manager)
17. [Multiple Database Support](#multiple-database-support)
18. [RPC Client Library (append-client.js)](#rpc-client-library-append-clientjs)
19. [Frontend Application Structure](#frontend-application-structure)
20. [BuildInfo و Template System](#buildinfo-و-template-system)
21. [Database Schema Utilities](#database-schema-utilities)
22. [Security و Best Practices](#security-و-best-practices)
23. [Pagination و Sorting](#pagination-و-sorting)
24. [DbQueryColumn و انتخاب ستون‌ها](#dbquerycolumn-و-انتخاب-ستونها)
25. [Aggregations](#aggregations)
26. [History Tables](#history-tables)
27. [ClientUI Structure](#clientui-structure)
28. [FAQ (سوالات متداول)](#faq-سوالات-متداول)
29. [مثال‌های کاربردی](#مثالهای-کاربردی)
30. [Glossary (واژه‌نامه)](#glossary-واژهنامه)
31. [Frontend Architecture - معماری Frontend](#frontend-architecture---معماری-frontend)

---

## مقدمه

AppEnd یک فریم‌ورک Low-Code و Rapid Application Development (RAD) است که به شما کمک می‌کند به سرعت اپلیکیشن‌های کامل Full-Stack بسازید. این فریم‌ورک بر اساس معماری ساده و ماژولار طراحی شده و قابلیت توسعه و سفارشی‌سازی بالایی دارد.

---

## معرفی AppEnd

### AppEnd چیست؟

AppEnd یک پلتفرم Low-Code است که امکان ایجاد سریع APIها، رابط‌های کاربری و مدیریت سطوح دسترسی را فراهم می‌کند. این فریم‌ورک برای ساخت اپلیکیشن‌های مبتنی بر پایگاه داده طراحی شده است.

### مزایای استفاده از AppEnd

- **منبع باز و رایگان**: کاملاً Open Source است
- **یادگیری آسان**: منحنی یادگیری پایینی دارد
- **معماری تمیز و ساده**: ساختار ماژولار و قابل فهم
- **قابلیت اجرا در Linux و Windows**
- **فقط برای Database IO نیست**: یک پلتفرم کامل برای توسعه اپلیکیشن‌ها
- **سازگار با استانداردهای توسعه**
- **قابلیت تزریق کد سفارشی**: در بخش‌های کلاینت و سرور
- **رابط‌های کاربری قابل سفارشی‌سازی**
- **پشتیبانی از چندزبانه**
- **تولید خودکار CRUD**: با چند کلیک ساده

### تکنولوژی‌های استفاده شده

- **Host**: Linux یا Windows
- **Application Server**: .NET Core / C#
- **Database**: MS SQL Server
- **Client**: SPA بر پایه Bootstrap & Vue.js 3

---

## معماری کلی سیستم

### معماری کلی

AppEnd از معماری سه لایه استفاده می‌کند:

```
┌─────────────────────────────────────────┐
│         Frontend (Vue.js + Bootstrap)   │
│  - SPA Application                      │
│  - Components & Templates               │
└─────────────────┬───────────────────────┘
                  │ RPC (JSON over HTTP)
┌─────────────────▼───────────────────────┐
│         AppEndHost (ASP.NET Core)       │
│  - HTTP Server                          │
│  - RPC Endpoint Handler                 │
│  - Static File Server                   │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│      AppEndServer (Business Logic)      │
│  - RpcNet (RPC Handler)                 │
│  - Dynamic Code Execution               │
│  - Service Layer                        │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│     AppEndDbIO (Data Access Layer)      │
│  - Database Connection                  │
│  - Query Builder                        │
│  - CRUD Operations                      │
└─────────────────┬───────────────────────┘
                  │
┌─────────────────▼───────────────────────┐
│         SQL Server Database             │
└─────────────────────────────────────────┘
```

### ارتباط بین لایه‌ها

#### 1. ارتباط Frontend به Backend

Frontend از طریق **RPC (Remote Procedure Call)** با Backend ارتباط برقرار می‌کند:

- **Endpoint**: `/talk-to-me` (قابل تنظیم در appsettings.json)
- **Method**: POST
- **Format**: JSON
- **Authentication**: Token-based (در Header)

#### 2. پردازش درخواست‌ها

1. Frontend درخواست RPC را ارسال می‌کند
2. `RpcNet` درخواست را دریافت می‌کند
3. `DynaCode` متد مربوطه را پیدا و اجرا می‌کند
4. متد می‌تواند از `DbIO` برای دسترسی به دیتابیس استفاده کند
5. نتیجه به Frontend برگردانده می‌شود

---

## ماژول‌های پروژه

پروژه AppEnd به چندین ماژول تقسیم شده است:

### 1. AppEndCommon

**مسئولیت**: کلاس‌ها و توابع مشترک و ابزارهای کمکی

**فایل‌های مهم**:
- `AppEndClass.cs`: کلاس‌های پایه برای تولید کد C#
- `AppEndSettings.cs`: تنظیمات سیستم
- `AppEndUser.cs`: مدل کاربر
- `AppEndException.cs`: کلاس Exception سفارشی
- `ExtensionsFor*.cs`: Extension Methods برای انواع مختلف

**ویژگی‌های کلیدی**:
- Extension Methods برای String، DateTime، Json، و ...
- مدیریت تنظیمات از طریق `appsettings.json`
- کلاس‌های کمکی برای لاگ و مدیریت خطا

### 2. AppEndDbIO

**مسئولیت**: دسترسی به پایگاه داده و ساخت کوئری‌ها

**کلاس‌های اصلی**:
- `DbIO.cs`: کلاس پایه برای ارتباط با دیتابیس
  - `DbIOMsSql.cs`: پیاده‌سازی برای SQL Server
- `ClientQuery.cs`: ساخت و اجرای کوئری‌ها از JSON
- `DbQuery.cs`: تعریف کوئری‌های از پیش تعریف شده
- `DbDialog.cs`: تعریف متادیتای یک جدول/View
- `DbDialogFactory.cs`: Factory برای ایجاد DbDialog

**انواع QueryType**:
- `Create`: برای Insert
- `ReadList`: برای SELECT لیستی
- `ReadByKey`: برای SELECT یک رکورد
- `UpdateByKey`: برای Update
- `DeleteByKey`: برای Delete
- `AggregatedReadList`: برای SELECT با Aggregate Functions
- `Procedure`: برای اجرای Stored Procedure
- `ScalarFunction`: برای اجرای Scalar Function
- `TableFunction`: برای اجرای Table Function

**مثال استفاده**:
```csharp
// ایجاد یک ClientQuery از JSON
ClientQuery cq = ClientQuery.GetInstanceByQueryJson(jsonElement, userContext);
object result = cq.Exec(); // اجرای کوئری
```

### 3. AppEndDynaCode

**مسئولیت**: کامپایل و اجرای داینامیک کد C#

**کلاس اصلی**: `DynaCode.cs`

**ویژگی‌ها**:
- کامپایل فایل‌های `.cs` موجود در `workspace/server`
- ایجاد Assembly داینامیک
- Invoke متدها به صورت داینامیک
- مدیریت دسترسی (Access Control)
- Cache کردن نتایج
- Logging خودکار

**جریان کار**:
1. همه فایل‌های `.cs` در پوشه `workspace/server` خوانده می‌شوند
2. با استفاده از Roslyn کامپایل می‌شوند
3. Assembly داینامیک ایجاد می‌شود
4. متدها از طریق Reflection فراخوانی می‌شوند

**مثال**:
```csharp
// فراخوانی یک متد
CodeInvokeResult result = DynaCode.InvokeByJsonInputs(
    "DefaultRepo.Products.ReadList",
    jsonInputs,
    actor
);
```

### 4. AppEndServer

**مسئولیت**: سرویس‌های سرور و Business Logic

**کلاس‌های اصلی**:
- `RpcNet.cs`: Middleware برای مدیریت RPC
- `DbServices.cs`: سرویس‌های مربوط به دیتابیس
- `DbDialogServices.cs`: سرویس‌های مربوط به DbDialog
- `TemplateServices.cs`: سرویس تولید UI از Template
- `ActorServices.cs`: سرویس‌های احراز هویت
- `CacheServices.cs`: مدیریت Cache
- `FileServices.cs`: مدیریت فایل‌ها

**ویژگی‌ها**:
- مدیریت RPC Requests
- Authentication & Authorization
- تولید خودکار UI از Templates
- مدیریت Cache
- Logging

### 5. AppEndHost

**مسئولیت**: میزبانی اپلیکیشن و HTTP Server

**فایل‌های مهم**:
- `Program.cs`: نقطه شروع اپلیکیشن

**مسئولیت‌ها**:
- راه‌اندازی HTTP Server
- پیکربندی Middleware
- سرو فایل‌های استاتیک (Frontend)
- پیکربندی CORS
- Response Compression

### 6. Frontend (workspace/client)

**مسئولیت**: رابط کاربری

**ساختار**:
- `a..lib/`: کتابخانه‌های JavaScript
  - `append-client.js`: کلاینت RPC اصلی
  - `jquery/`, `bootstrap/`, `vue/`: کتابخانه‌های مورد نیاز
- `a..templates/`: Template‌های Razor برای تولید UI
- `a.Components/`: کامپوننت‌های Vue تولید شده
- `a.SharedComponents/`: کامپوننت‌های مشترک Vue
- `a.Layouts/`: Layout‌های Vue
- `AppEndStudio/`: اپلیکیشن Studio برای مدیریت
- `[ApplicationName]/`: اپلیکیشن‌های ایجاد شده

---

## جریان کار (Workflow)

### 1. جریان یک درخواست از Frontend تا Database

```
1. کاربر یک Action در Frontend انجام می‌دهد
   ↓
2. Frontend یک RPC Request ایجاد می‌کند
   {
     "Id": "unique-id",
     "Method": "DefaultRepo.Products.ReadList",
     "Inputs": { ... }
   }
   ↓
3. Request به /talk-to-me ارسال می‌شود
   ↓
4. RpcNet درخواست را دریافت می‌کند
   ↓
5. Actor (کاربر) از Token استخراج می‌شود
   ↓
6. DynaCode متد را پیدا و بررسی دسترسی می‌کند
   ↓
7. متد اجرا می‌شود (ممکن است از Cache استفاده کند)
   ↓
8. متد ممکن است ClientQuery ایجاد و اجرا کند
   ↓
9. DbIO کوئری SQL را اجرا می‌کند
   ↓
10. نتیجه از Database برگردانده می‌شود
   ↓
11. نتیجه پردازش و به JSON تبدیل می‌شود
   ↓
12. Response به Frontend برگردانده می‌شود
   ↓
13. Frontend UI را به‌روزرسانی می‌کند
```

### 2. جریان ایجاد یک DbObject (جدول) جدید

```
1. توسعه‌دهنده یک جدول جدید در Database ایجاد می‌کند
   ↓
2. در AppEndStudio، جدول را انتخاب می‌کند
   ↓
3. روی "Create Server Objects" کلیک می‌کند
   ↓
4. DbDialogFactory.CreateServerObjectsFor() فراخوانی می‌شود
   ↓
5. DbDialog JSON ایجاد می‌شود (workspace/server/DefaultRepo.TableName.dbdialog.json)
   ↓
6. کلاس C# ایجاد می‌شود (workspace/server/DefaultRepo.TableName.cs)
   ↓
7. Query‌های پیش‌فرض ایجاد می‌شوند (Create, ReadList, ReadByKey, UpdateByKey, DeleteByKey)
   ↓
8. ClientUI‌ها تعریف می‌شوند
   ↓
9. DynaCode.Refresh() فراخوانی می‌شود (کامپایل مجدد)
   ↓
10. می‌توان UI را از Template تولید کرد
```

### 3. جریان تولید UI از Template

```
1. توسعه‌دهنده روی "Build UI" کلیک می‌کند
   ↓
2. DbDialogServices.BuildUiForDbObject() فراخوانی می‌شود
   ↓
3. برای هر ClientUI:
   - Template مربوطه خوانده می‌شود (a..templates/TemplateName.cshtml)
   - با استفاده از RazorEngine کامپایل می‌شود
   - BuildInfo به Template پاس داده می‌شود
   ↓
4. Vue Component ایجاد می‌شود (.vue)
   ↓
5. فایل در a.Components/ ذخیره می‌شود
```

---

## ویژگی‌های کلیدی

### 1. Authentication & Authorization

**Authentication**:
- Token-based Authentication
- Token در Header با نام `token` ارسال می‌شود
- Token شامل اطلاعات کاربر است (Id, UserName, Roles)
- Token با Secret Key encode/decode می‌شود

**Authorization**:
- سطح Method: هر متد می‌تواند Access Rules داشته باشد
- سطح Role: دسترسی بر اساس Role
- سطح User: دسترسی بر اساس UserName
- Public Methods: متدهای عمومی که نیاز به احراز هویت ندارند

**مثال تنظیمات دسترسی**:
```json
{
  "AccessRules": {
    "AllowedRoles": ["admin", "user"],
    "AllowedUsers": ["john"],
    "DeniedUsers": []
  }
}
```

### 2. Dynamic Code Compilation

- کدهای C# در `workspace/server` به صورت داینامیک کامپایل می‌شوند
- از Roslyn برای کامپایل استفاده می‌شود
- Assembly داینامیک ایجاد می‌شود
- امکان Refresh بدون Restart اپلیکیشن

### 3. Query Builder

- ساخت کوئری SQL از JSON
- پشتیبانی از:
  - WHERE clauses (با CompareOperators مختلف)
  - ORDER BY
  - Pagination
  - Aggregations
  - Relations (JOIN)
  - Sub Queries

### 4. Template Engine

- استفاده از RazorEngine برای تولید UI
- Template‌های Razor در `a..templates/`
- تولید خودکار Vue Components
- Template‌های موجود:
  - `ReadList.cshtml`: لیست داده‌ها
  - `Create.cshtml`: فرم ایجاد
  - `UpdateByKey.cshtml`: فرم ویرایش
  - `ReadByKey.cshtml`: نمایش یک رکورد

### 5. Caching

- Memory Cache برای نتایج متدها
- Cache Policy قابل تنظیم:
  - `None`: بدون Cache
  - `Global`: Cache برای همه
  - `PerUser`: Cache برای هر کاربر
- TTL قابل تنظیم

### 6. Logging

- Logging خودکار همه متدها
- ذخیره در Database (BaseActivityLog)
- شامل اطلاعات:
  - Method Name
  - Inputs
  - Output
  - Duration
  - User
  - IP Address
  - Success/Failure

### 7. File Management

- مدیریت فایل‌های استاتیک
- پشتیبانی از Upload/Download
- مدیریت تصاویر و فایل‌های باینری

---

## نحوه ایجاد یک اپلیکیشن

### مرحله 1: راه‌اندازی اولیه

1. **راه‌اندازی Database**:
   - یک Database خالی در SQL Server ایجاد کنید
   - فایل `Zzz_Deploy.sql` را اجرا کنید
   - دستورات زیر را اجرا کنید:
   ```sql
   EXEC Zzz_Deploy
   EXEC Zzz_Deploy 'AppEnd'
   ```

2. **تنظیمات appsettings.json**:
   ```json
   {
     "AppEnd": {
       "DefaultDbConfName": "DefaultRepo",
       "TalkPoint": "talk-to-me",
       "Secret": "YourSecretKey",
       "DbServers": [
         {
           "Name": "DefaultRepo",
           "ServerType": "MsSql",
           "ConnectionString": "YourConnectionString"
         }
       ]
     }
   }
   ```

3. **اجرای اپلیکیشن**:
   - پروژه `AppEndHost` را اجرا کنید
   - به آدرس `http://localhost:5000` بروید
   - با Username: `Admin` و Password: `P#ssw0rd` لاگین کنید

### مرحله 2: ایجاد یک جدول جدید

1. **ایجاد جدول در Database**:
   ```sql
   CREATE TABLE Products (
       Id INT PRIMARY KEY IDENTITY(1,1),
       Name NVARCHAR(200) NOT NULL,
       Price DECIMAL(18,2),
       CreatedOn DATETIME DEFAULT GETDATE()
   )
   ```

2. **ایجاد Server Objects**:
   - وارد AppEndStudio شوید
   - به بخش "DbObjects" بروید
   - جدول `Products` را پیدا کنید
   - روی "Create Server Objects" کلیک کنید
   - این کار ایجاد می‌کند:
     - `DefaultRepo.Products.dbdialog.json`
     - `DefaultRepo.Products.cs`

3. **تنظیم DbDialog**:
   - فایل `DefaultRepo.Products.dbdialog.json` را باز کنید
   - می‌توانید Column Properties، Relations، و UI Props را تنظیم کنید

### مرحله 3: ایجاد Query‌ها

1. **ایجاد Query‌های پیش‌فرض**:
   - در Designer، می‌توانید Query‌های مختلف ایجاد کنید:
     - `ReadList`: برای نمایش لیست
     - `Create`: برای ایجاد رکورد جدید
     - `ReadByKey`: برای نمایش یک رکورد
     - `UpdateByKey`: برای ویرایش
     - `DeleteByKey`: برای حذف

2. **سفارشی‌سازی Query**:
   - Columns را انتخاب کنید
   - Relations را تنظیم کنید
   - Aggregations را اضافه کنید
   - WHERE clauses را تنظیم کنید

### مرحله 4: تولید UI

1. **ایجاد ClientUI**:
   - برای هر Query که نیاز به UI دارد، یک ClientUI ایجاد کنید
   - Template را انتخاب کنید (ReadList, Create, UpdateByKey, ...)

2. **Build UI**:
   - روی "Build UI" کلیک کنید
   - Vue Components در `a.Components/` ایجاد می‌شوند

### مرحله 5: ایجاد Frontend Application

1. **ایجاد پوشه Application**:
   - یک پوشه جدید در `workspace/client/` ایجاد کنید (مثلاً `MyApp`)

2. **ایجاد app.json**:
   ```json
   {
     "title": "My App",
     "sub-title": "Application",
     "dir": "ltr",
     "lang": "fa",
     "defaultComponent": "components/Home",
     "navigation": [
       {
         "title": "Products",
         "icon": "fa-box",
         "component": "a.Components/Products_ReadList"
       }
     ]
   }
   ```

3. **دسترسی به Application**:
   - به آدرس `http://localhost:5000/client/MyApp/index.html` بروید

---

## نحوه توسعه به عنوان توسعه‌دهنده

### 1. ایجاد یک متد جدید (Not Mapped)

متدهایی که مستقیماً به Database مربوط نیستند:

```csharp
// فایل: workspace/server/MyNamespace.MyClass.cs
namespace MyNamespace
{
    public static class MyClass
    {
        public static object? MyCustomMethod(AppEndUser? Actor, string Parameter1)
        {
            // کد شما
            return new { Result = "Success" };
        }
    }
}
```

**تنظیمات دسترسی** (فایل `.settings.json`):
```json
{
  "MyNamespace.MyClass.MyCustomMethod": {
    "AccessRules": {
      "AllowedRoles": ["admin"],
      "AllowedUsers": []
    },
    "CachePolicy": {
      "CacheLevel": "None"
    },
    "LogPolicy": "Full"
  }
}
```

### 2. ایجاد یک DbDialog Method

متدهایی که از DbDialog استفاده می‌کنند:

```csharp
namespace DefaultRepo
{
    public static class Products
    {
        public static object? ReadList(JsonElement ClientQueryJE, AppEndUser? Actor)
        {
            return ClientQuery.GetInstanceByQueryJson(ClientQueryJE, Actor?.ContextInfo).Exec();
        }
    }
}
```

### 3. سفارشی‌سازی یک متد

می‌توانید متدهای تولید شده را ویرایش کنید:

```csharp
public static object? ReadList(JsonElement ClientQueryJE, AppEndUser? Actor)
{
    // قبل از اجرای Query
    ClientQuery cq = ClientQuery.GetInstanceByQueryJson(ClientQueryJE, Actor?.ContextInfo);
    
    // اضافه کردن WHERE clause
    if (cq.Where == null) cq.Where = new Where();
    cq.Where.And("IsActive", CompareOperator.Equal, true);
    
    // اجرای Query
    object result = cq.Exec();
    
    // پردازش نتیجه
    // ...
    
    return result;
}
```

### 4. ایجاد یک کامپوننت Vue سفارشی

1. **ایجاد فایل Vue**:
   ```vue
   <!-- workspace/client/MyApp/components/MyComponent.vue -->
   <template>
     <div>
       <h1>My Custom Component</h1>
     </div>
   </template>

   <script>
   export default {
     name: 'MyComponent',
     data() {
       return {
         // data
       }
     },
     methods: {
       // methods
     }
   }
   </script>
   ```

2. **استفاده در app.json**:
   ```json
   {
     "navigation": [
       {
         "title": "My Component",
         "component": "components/MyComponent"
       }
     ]
   }
   ```

### 5. ایجاد یک Template جدید

1. **ایجاد فایل Template**:
   ```razor
   @* workspace/client/a..templates/MyTemplate.cshtml *@
   <template>
     <div>
       <h1>@Model.DbDialog.ObjectName</h1>
       <!-- ... -->
     </div>
   </template>
   ```

2. **استفاده در ClientUI**:
   - در DbDialog، ClientUI را با TemplateName = "MyTemplate" تنظیم کنید

### 6. کار با Cache

```csharp
public static object? GetCachedData(AppEndUser? Actor)
{
    string cacheKey = "MyCacheKey";
    
    if (SV.SharedMemoryCache.TryGetValue(cacheKey, out object? cached))
    {
        return cached;
    }
    
    // محاسبه داده
    object data = ComputeData();
    
    // ذخیره در Cache
    MemoryCacheEntryOptions options = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
    };
    SV.SharedMemoryCache.ToCache(cacheKey, data, options);
    
    return data;
}
```

### 7. کار با Logging

```csharp
// Log Error
LogMan.LogError("Error message");

// Log Activity (به صورت خودکار توسط DynaCode انجام می‌شود)
LogMan.LogActivity(
    namespaceName,
    className,
    methodName,
    recordId,
    isSucceeded,
    fromCache,
    inputs,
    response,
    duration,
    clientIp,
    clientAgent,
    userId,
    userName
);
```

---

## دیباگ و تغییرات

### 1. دیباگ Backend

**استفاده از Visual Studio**:
1. نقطه Breakpoint قرار دهید
2. پروژه `AppEndHost` را در حالت Debug اجرا کنید
3. درخواست از Frontend بفرستید
4. Breakpoint فعال می‌شود

**لاگ‌ها**:
- لاگ‌ها در `log/` ذخیره می‌شوند
- همچنین در Database (BaseActivityLog) ذخیره می‌شوند

**مشاهده Exception‌ها**:
- در Log Files
- در Database Activity Log
- در Console Output

### 2. دیباگ Frontend

**استفاده از Browser DevTools**:
1. F12 را بزنید
2. به Console بروید
3. خطاها و Log‌ها را مشاهده کنید

**مشاهده RPC Requests**:
1. به Network Tab بروید
2. درخواست به `/talk-to-me` را پیدا کنید
3. Request و Response را مشاهده کنید

### 3. تغییر در کد Backend

1. **تغییر در کدهای Dynamic**:
   - فایل `.cs` را در `workspace/server/` ویرایش کنید
   - DynaCode به صورت خودکار تغییرات را تشخیص می‌دهد
   - می‌توانید `DynaCode.Refresh()` را فراخوانی کنید

2. **تغییر در Framework**:
   - فایل‌های Framework را ویرایش کنید
   - اپلیکیشن را Restart کنید

### 4. تغییر در Frontend

1. **تغییر در Components**:
   - فایل `.vue` را ویرایش کنید
   - صفحه را Refresh کنید (Hot Reload ممکن است موجود باشد)

2. **تغییر در Templates**:
   - Template را ویرایش کنید
   - UI را دوباره Build کنید

### 5. Troubleshooting

**مشکل: متد پیدا نمی‌شود**
- بررسی کنید که فایل `.cs` در `workspace/server/` باشد
- Namespace و Class Name را بررسی کنید
- `DynaCode.Refresh()` را فراخوانی کنید
- خطاهای کامپایل را در Log بررسی کنید

**مشکل: دسترسی رد می‌شود**
- تنظیمات AccessRules را بررسی کنید
- Role کاربر را بررسی کنید
- PublicMethods را بررسی کنید

**مشکل: Query اجرا نمی‌شود**
- Connection String را بررسی کنید
- ساختار Query را بررسی کنید
- لاگ‌های Database را بررسی کنید

---

## ساختار فایل‌ها و پوشه‌ها

### ساختار کلی

```
AppEnd/
├── AppEndCommon/          # ماژول مشترک
├── AppEndDbIO/            # ماژول Database IO
├── AppEndDynaCode/        # ماژول Dynamic Code
├── AppEndServer/          # ماژول Server Services
├── AppEndHost/            # ماژول Host
│   ├── workspace/
│   │   ├── server/        # کدهای C# Dynamic
│   │   │   ├── DefaultRepo.Products.cs
│   │   │   ├── DefaultRepo.Products.dbdialog.json
│   │   │   └── DefaultRepo.Products.settings.json
│   │   └── client/        # Frontend Files
│   │       ├── a..lib/    # کتابخانه‌ها
│   │       ├── a..templates/  # Template‌های Razor
│   │       ├── a.Components/  # کامپوننت‌های Vue تولید شده
│   │       ├── a.SharedComponents/  # کامپوننت‌های مشترک
│   │       ├── a.Layouts/  # Layout‌ها
│   │       └── [AppName]/  # اپلیکیشن‌ها
│   └── appsettings.json   # تنظیمات
└── README.md
```

### فایل‌های مهم

#### Backend

- **workspace/server/DefaultRepo.Products.cs**:
  - کد C# برای متدهای مربوط به Products

- **workspace/server/DefaultRepo.Products.dbdialog.json**:
  - متادیتای جدول Products
  - Columns, Relations, Queries, ClientUIs

- **workspace/server/DefaultRepo.Products.settings.json**:
  - تنظیمات دسترسی و Cache برای متدها

#### Frontend

- **workspace/client/[AppName]/app.json**:
  - تنظیمات اپلیکیشن
  - Navigation
  - Themes

- **workspace/client/a.Components/Products_ReadList.vue**:
  - کامپوننت Vue تولید شده برای لیست Products

- **workspace/client/a..templates/ReadList.cshtml**:
  - Template Razor برای تولید ReadList Components

### پوشه‌های Reserved

این پوشه‌ها توسط Framework استفاده می‌شوند:
- `a..lib/`: کتابخانه‌های JavaScript
- `a..templates/`: Template‌های Razor
- `a.Components/`: کامپوننت‌های تولید شده
- `a.SharedComponents/`: کامپوننت‌های مشترک
- `a.Layouts/`: Layout‌ها
- `appendstudio/`: اپلیکیشن Studio

---

## خلاصه و نکات نهایی

### نکات مهم برای توسعه‌دهندگان

1. **همیشه DynaCode.Refresh() را بعد از تغییر کدهای Dynamic فراخوانی کنید**
2. **از Cache برای بهبود عملکرد استفاده کنید**
3. **لاگ‌ها را بررسی کنید تا مشکلات را پیدا کنید**
4. **از AccessRules برای امنیت استفاده کنید**
5. **Template‌ها را برای تولید UI سفارشی کنید**
6. **از Relations برای پیاده‌سازی ارتباطات بین جداول استفاده کنید**

### بهترین روش‌ها

1. **کدهای Business Logic را در متدهای Not Mapped قرار دهید**
2. **از DbDialog برای Query‌های ساده استفاده کنید**
3. **UI را از Template تولید کنید تا قابلیت نگهداری بالا باشد**
4. **از Namespace برای سازماندهی کدها استفاده کنید**
5. **تنظیمات را در appsettings.json قرار دهید**

### منابع و مراجع

- README.md: راهنمای اولیه
- Wiki: مستندات بیشتر (https://github.com/mirshahreza/AppEnd/wiki)
- کدهای موجود: بهترین منبع برای یادگیری

---

## تنظیمات کامل (Configuration Reference)

### تنظیمات appsettings.json

فایل `appsettings.json` در پوشه `AppEndHost` قرار دارد و شامل تنظیمات زیر است:

```json
{
  "AppEnd": {
    "TalkPoint": "talk-to-me",              // Endpoint برای RPC
    "DefaultDbConfName": "DefaultRepo",      // نام پیش‌فرض دیتابیس
    "LogDbConfName": "DefaultRepo",          // دیتابیس برای لاگ‌ها
    "LoginDbConfName": "DefaultRepo",        // دیتابیس برای احراز هویت
    "Secret": "YourSecretKey",               // Secret Key برای Token
    "PublicKeyRole": "admin",                // Role با دسترسی کامل
    "PublicKeyUser": "admin",                // User با دسترسی کامل
    "IsDevelopment": false,                  // حالت Development
    "EnableFileLogging": true,               // فعال‌سازی File Logging
    "LogLevel": "Information",               // سطح لاگ (Debug, Information, Warning, Error)
    "LogsPath": "log",                       // مسیر ذخیره لاگ‌ها
    "MaxLogFileSizeBytes": 2048,             // حداکثر اندازه فایل لاگ (KB)
    "LogWriterQueueCap": 5,                  // ظرفیت صف لاگ‌نویس
    "PublicMethods": [                       // لیست متدهای عمومی
      "Zzz.AppEndProxy.PingMe",
      "Zzz.AppEndProxy.Login"
    ],
    "Serilog": {                             // تنظیمات Serilog
      "TableName": "BaseActivityLog",
      "Connection": "DefaultRepo",
      "BatchPostingLimit": 3,
      "BatchPeriodSeconds": 15
    },
    "DbServers": [                           // لیست دیتابیس‌ها
      {
        "Name": "DefaultRepo",
        "ServerType": "MsSql",
        "ConnectionString": "YourConnectionString"
      }
    ]
  }
}
```

### تنظیمات Method Settings

هر متد می‌تواند تنظیمات خود را در فایل `.settings.json` داشته باشد:

```json
{
  "Namespace.ClassName.MethodName": {
    "AccessRules": {
      "AllowedRoles": ["admin", "user"],
      "AllowedUsers": ["john"],
      "DeniedUsers": []
    },
    "CachePolicy": {
      "CacheLevel": "PerUser",
      "AbsoluteExpirationSeconds": 3600
    },
    "LogPolicy": "Full"
  }
}
```

**CacheLevel**:
- `None`: بدون Cache
- `PerUser`: Cache برای هر کاربر جداگانه
- `AllUsers`: Cache مشترک برای همه کاربران

**LogPolicy**:
- `IgnoreLogging`: بدون لاگ
- `TrimInputs`: لاگ با Inputs کوتاه شده
- `Full`: لاگ کامل با تمام Inputs

---

## UI Widgets و Components

### لیست کامل UI Widgets

AppEnd از Widget‌های زیر پشتیبانی می‌کند:

#### Single Line Inputs
- **Textbox**: ورودی متن تک خط
- **DisabledTextbox**: ورودی غیرفعال (برای نمایش)
- **Sliderbox**: اسلایدر عددی

#### Multi Line Inputs
- **MultilineTextbox**: ورودی متن چند خط
- **Htmlbox**: ویرایشگر HTML
- **CodeEditorbox**: ویرایشگر کد

#### Select Inputs
- **Combo**: Dropdown List
- **Radio**: Radio Buttons
- **ObjectPicker**: انتخاب شی از جدول دیگر

#### Date & Time
- **DatePicker**: انتخاب تاریخ
- **DateTimePicker**: انتخاب تاریخ و زمان
- **TimePicker**: انتخاب زمان

#### Binary
- **ImageView**: نمایش و آپلود تصویر
- **FileView**: نمایش و آپلود فایل

#### Other
- **Checkbox**: چک باکس
- **ColorPicker**: انتخاب رنگ
- **NoWidget**: بدون Widget (مخفی)

### Search Types

برای هر ستون می‌توان نوع جستجو را تنظیم کرد:

- **None**: بدون جستجو
- **Fast**: جستجوی سریع (برای Combo و Radio)
- **Expandable**: جستجوی قابل گسترش

### Auto Widget Selection

فریم‌ورک به صورت خودکار Widget مناسب را بر اساس نوع ستون انتخاب می‌کند:

- `INT IDENTITY` → `NoWidget`
- `BIT` → `Checkbox`
- `DATETIME` → `DateTimePicker`
- `DATE` → `DatePicker`
- `IMAGE` (Picture) → `ImageView`
- `IMAGE` (Other) → `FileView`
- `TEXT/NTEXT` → `MultilineTextbox`
- `NVARCHAR > 160` → `MultilineTextbox`
- `FK` → `Combo`
- Default → `Textbox`

---

## Compare Operators

برای ساخت WHERE clauses از Operator‌های زیر استفاده می‌شود:

- **Equal**: `=`
- **NotEqual**: `!=`
- **Contains**: `LIKE '%...%'`
- **StartsWith**: `LIKE '...%'`
- **EndsWith**: `LIKE '%...'`
- **MoreThan**: `>`
- **MoreThanOrEqual**: `>=`
- **LessThan**: `<`
- **LessThanOrEqual**: `<=`
- **In**: `IN (...)`
- **NotIn**: `NOT IN (...)`
- **IsNull**: `IS NULL`
- **IsNotNull**: `IS NOT NULL`

### مثال استفاده از WHERE

```json
{
  "Where": {
    "ConjunctiveOperator": "AND",
    "CompareClauses": [
      {
        "Name": "IsActive",
        "CompareOperator": "Equal",
        "Value": true
      },
      {
        "Name": "Name",
        "CompareOperator": "Contains",
        "Value": "test"
      }
    ]
  }
}
```

---

## Relations (ارتباطات بین جداول)

### انواع Relations

#### 1. OneToMany
ارتباط یک به چند (Parent-Child)

```json
{
  "RelationName": "To_Orders_On_CustomerId",
  "RelationTable": "Orders",
  "RelationPkColumn": "Id",
  "RelationFkColumn": "CustomerId",
  "RelationType": "OneToMany",
  "CreateQuery": "Create",
  "ReadListQuery": "ReadList",
  "RelationUiWidget": "Grid"
}
```

#### 2. ManyToMany
ارتباط چند به چند (با جدول واسط)

```json
{
  "RelationName": "To_Tags_On_TagId",
  "RelationTable": "Tags",
  "RelationType": "ManyToMany",
  "LinkingTargetTable": "ProductTags",
  "LinkingColumnInManyToMany": "TagId",
  "RelationUiWidget": "CheckboxList"
}
```

### Relation UI Widgets

- **Grid**: نمایش در قالب Grid
- **Cards**: نمایش در قالب Card
- **CheckboxList**: لیست Checkbox (برای ManyToMany)
- **AddableList**: لیست قابل افزودن

### File Centric Relations

برای Relation‌های مرتبط با فایل (Image, File):
- `IsFileCentric: true`
- استفاده از Widget `Cards`

---

## ValueSharp System

ValueSharp یک سیستم برای تعیین مقادیر پیش‌فرض و پردازش شده در Parameters است.

### ValueSharp Expressions

#### 1. #Now
زمان فعلی:

```json
{
  "Name": "CreatedOn",
  "ValueSharp": "#Now"
}
```

#### 2. #Context:Key
مقدار از User Context:

```json
{
  "Name": "CreatedBy",
  "ValueSharp": "#Context:UserId"
}
```

#### 3. #Resize:ColumnName,Size
تغییر اندازه تصویر:

```json
{
  "Name": "Picture_FileBody_xs",
  "ValueSharp": "#Resize:Picture_FileBody,75"
}
```

#### 4. #ToMD5:Value
تبدیل به MD5 Hash:

```json
{
  "Name": "Password",
  "ValueSharp": "#ToMD5:Password"
}
```

#### 5. #ToMD4:Value
تبدیل به MD4 Hash:

```json
{
  "Name": "Password",
  "ValueSharp": "#ToMD4:Password"
}
```

### مثال استفاده در DbParam

```json
{
  "Params": [
    {
      "Name": "UpdatedBy",
      "DbType": "INT",
      "AllowNull": true,
      "ValueSharp": "#Context:UserId"
    },
    {
      "Name": "UpdatedOn",
      "DbType": "DATETIME",
      "AllowNull": true,
      "ValueSharp": "#Now"
    },
    {
      "Name": "Picture_FileBody_xs",
      "DbType": "IMAGE",
      "AllowNull": true,
      "ValueSharp": "#Resize:Picture_FileBody,75"
    }
  ]
}
```

---

## Package Manager

AppEnd شامل یک سیستم Package Manager برای بسته‌بندی و به‌اشتراک‌گذاری ماژول‌ها است.

### ساختار Package

یک Package شامل:
- `info.json`: اطلاعات Package
- `install.sql`: اسکریپت نصب
- `uninstall.sql`: اسکریپت حذف
- فایل‌های دیگر (server objects, client components, ...)

### فیلدهای Package

```json
{
  "Name": "MyPackage",
  "Title": "My Package Title",
  "Note": "Description",
  "Version": "1.0.0",
  "Url": "https://...",
  "CreatedBy": "admin",
  "CreatedOn": "2024-01-01",
  "UpdatedBy": "admin",
  "UpdatedOn": "2024-01-01",
  "InstallSql": "CREATE TABLE ...",
  "UnInstallSql": "DROP TABLE ...",
  "Installed": false,
  "MenuItems": []
}
```

### کار با Packages

1. **ایجاد Package**:
   - در AppEndStudio به "Package Manager" بروید
   - روی "Create Package" کلیک کنید
   - اطلاعات Package را وارد کنید

2. **Export Package**:
   - Package را انتخاب کنید
   - Export کنید (فایل `.aepkg`)

3. **Import Package**:
   - روی "Upload Package" کلیک کنید
   - فایل `.aepkg` را انتخاب کنید

4. **Install Package**:
   - Package را انتخاب کنید
   - روی "Install" کلیک کنید
   - اسکریپت SQL اجرا می‌شود

---

## Multiple Database Support

AppEnd از چند دیتابیس پشتیبانی می‌کند.

### تنظیم چند دیتابیس

```json
{
  "DbServers": [
    {
      "Name": "DefaultRepo",
      "ServerType": "MsSql",
      "ConnectionString": "Server=...;Database=MainDB;..."
    },
    {
      "Name": "ArchiveRepo",
      "ServerType": "MsSql",
      "ConnectionString": "Server=...;Database=ArchiveDB;..."
    }
  ]
}
```

### استفاده از دیتابیس‌های مختلف

در نام Query از فرمت زیر استفاده کنید:

```
{DbConfName}.{ObjectName}.{MethodName}
```

مثال:
- `DefaultRepo.Products.ReadList`
- `ArchiveRepo.Orders.ReadList`

### انتخاب دیتابیس در DbIO

```csharp
// استفاده از دیتابیس پیش‌فرض
DbIO dbIO = DbIO.Instance();

// استفاده از دیتابیس خاص
DbConf dbConf = DbConf.FromSettings("ArchiveRepo");
DbIO dbIO = DbIO.Instance(dbConf);
```

---

## RPC Client Library (append-client.js)

کتابخانه کلاینت برای ارتباط با Backend.

### متدهای اصلی

#### rpc(options)
ارسال درخواست RPC (Async):

```javascript
rpc({
  requests: [
    {
      Id: "req1",
      Method: "DefaultRepo.Products.ReadList",
      Inputs: {
        Pagination: { PageIndex: 0, PageSize: 10 }
      }
    }
  ],
  onDone: function(responses) {
    console.log(responses);
  },
  onFail: function(error) {
    console.error(error);
  },
  loadingModel: "..."  // نمایش Loading
});
```

#### rpcSync(options)
ارسال درخواست RPC (Sync):

```javascript
let responses = rpcSync({
  requests: [...]
});
```

#### rpcAEP(method, inputs, onDone, onFail)
فراخوانی ساده یک متد:

```javascript
rpcAEP("DefaultRepo.Products.ReadList", 
  { Pagination: { PageIndex: 0, PageSize: 10 } },
  function(result) {
    console.log(result);
  }
);
```

### Cache در Client

کلاینت به صورت خودکار Response‌ها را Cache می‌کند:
- اگر Request مشابهی ارسال شود
- از Cache استفاده می‌شود
- نیازی به ارسال Request نیست

---

## Frontend Application Structure

### ساختار app.json

```json
{
  "title": "Application Title",
  "sub-title": "Subtitle",
  "dir": "ltr",                    // ltr یا rtl
  "lang": "en",                    // زبان پیش‌فرض
  "calendar": "Gregorian",         // تقویم
  "defaultComponent": "components/Home",
  "navigation": [
    {
      "title": "Section Title",
      "icon": "fa-solid fa-fw fa-icon",
      "items": [
        {
          "title": "Item Title",
          "icon": "fa-solid fa-fw fa-icon",
          "component": "components/MyComponent"
        }
      ]
    }
  ],
  "themes": {
    "default": "theme-name"
  }
}
```

### Layouts

دو Layout اصلی وجود دارد:

1. **BO.vue**: Back Office Layout
   - Sidebar Menu
   - Header
   - Content Area

2. **FO.vue**: Front Office Layout
   - بدون Sidebar
   - Header ساده
   - Content Area

### Shared Components

کامپوننت‌های مشترک در `a.SharedComponents/`:

- **BaseComponentLoader**: لودر کامپوننت
- **BaseContent**: محتوای اصلی
- **BaseFileEditor**: ویرایشگر فایل
- **BaseJsonView**: نمایش JSON
- **BaseConfirm**: Confirm Dialog
- **BasePrompt**: Prompt Dialog
- **Login**: صفحه لاگین
- **MyProfile**: پروفایل کاربر
- **SideMenu**: منوی کناری
- و ...

---

## BuildInfo و Template System

### BuildInfo

کلاس `BuildInfo` شامل اطلاعات لازم برای کامپایل Template است:

```csharp
public class BuildInfo
{
    public DbDialog DbDialog { get; set; }
    public ClientUI ClientUI { get; set; }
    public Dictionary<string, object> Parameters { get; set; }
}
```

### Template Compilation

Template‌های Razor کامپایل می‌شوند و Vue Component تولید می‌کنند:

```
Template (Razor) → Compile → Vue Component
```

### Template Helpers

در Template‌ها می‌توان از Helper Methods استفاده کرد:

- `GetDisplayColumns()`: ستون‌های نمایشی
- `GetTargetPkColumn()`: ستون Primary Key
- `GetFirstFileFieldName()`: نام فیلد فایل
- و ...

---

## Database Schema Utilities

### ایجاد و تغییر Schema

```csharp
DbSchemaUtils dbSchemaUtils = new("DefaultRepo");

// دریافت لیست جداول
List<DbTable> tables = dbSchemaUtils.GetTables();

// دریافت ستون‌های یک جدول
List<DbColumn> columns = dbSchemaUtils.GetTableViewColumns("Products");

// ایجاد یا تغییر جدول
dbSchemaUtils.CreateOrAlterTable(dbTable);

// حذف جدول
dbSchemaUtils.DropTable("Products");

// ایجاد View
dbSchemaUtils.CreateEmptyView("MyView");

// ایجاد Procedure
dbSchemaUtils.CreateEmptyProcedure("MyProcedure");
```

---

## Security و Best Practices

### امنیت

1. **Secret Key**:
   - همیشه Secret را تغییر دهید
   - از کلید قوی استفاده کنید

2. **Access Rules**:
   - برای هر متد Access Rules تعیین کنید
   - از Public Methods با احتیاط استفاده کنید

3. **SQL Injection**:
   - همه Query‌ها Parameterized هستند
   - از ClientQuery استفاده کنید

4. **Token Security**:
   - Token در Header ارسال می‌شود
   - Token شامل اطلاعات کاربر است

### Best Practices

1. **Organize Code**:
   - از Namespace استفاده کنید
   - کدها را ماژولار کنید

2. **Error Handling**:
   - Exception‌ها را Handle کنید
   - پیام‌های خطای واضح بدهید

3. **Performance**:
   - از Cache استفاده کنید
   - Query‌ها را بهینه کنید
   - Pagination استفاده کنید

4. **Maintainability**:
   - از Template Engine استفاده کنید
   - UI را خودکار Build کنید
   - Documentation بنویسید

---

## Pagination و Sorting

### Pagination

برای صفحه‌بندی نتایج از کلاس `Pagination` استفاده می‌شود:

```json
{
  "Pagination": {
    "PageNumber": 1,
    "PageSize": 10
  }
}
```

**SQL Generation**:
```sql
OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY
```

### Order Clauses

برای مرتب‌سازی از `OrderClauses` استفاده می‌شود:

```json
{
  "OrderClauses": [
    {
      "Name": "CreatedOn",
      "OrderDirection": "DESC"
    },
    {
      "Name": "Name",
      "OrderDirection": "ASC"
    }
  ]
}
```

**OrderDirection**:
- `ASC`: صعودی
- `DESC`: نزولی

### Order SQL Statement

می‌توان مستقیماً SQL Order Statement داد:

```json
{
  "OrderSqlStatement": "ORDER BY [Products].[Price] DESC, [Products].[Name] ASC"
}
```

---

## DbQueryColumn و انتخاب ستون‌ها

### انواع ستون‌ها در Query

#### 1. ستون ساده (Name)
```json
{
  "Name": "ProductName"
}
```

#### 2. ستون با Alias (Phrase)
```json
{
  "Phrase": "UPPER([Products].[Name])",
  "As": "NameUpperCase"
}
```

#### 3. ستون با RefTo (Relation)
```json
{
  "Name": "CategoryId",
  "RefTo": {
    "RelationName": "To_Categories",
    "Columns": ["Name", "Code"]
  }
}
```

### Containment System

برای کنترل ستون‌های برگردانده شده:

**ColumnsContainment**:
- `IncludeAll`: شامل همه ستون‌ها (پیش‌فرض)
- `IncludeIndicatedItems`: فقط ستون‌های مشخص شده
- `ExcludeAll`: هیچ ستونی
- `ExcludeIndicatedItems`: همه به جز موارد مشخص شده

**مثال**:
```json
{
  "ColumnsContainment": "IncludeIndicatedItems",
  "ClientIndicatedColumns": ["Id", "Name", "Price"]
}
```

---

## Aggregations

برای محاسبه Aggregate Functions:

```json
{
  "Aggregations": [
    {
      "Name": "TotalPrice",
      "AggregationFunction": "SUM",
      "ColumnName": "Price"
    },
    {
      "Name": "AvgPrice",
      "AggregationFunction": "AVG",
      "ColumnName": "Price"
    }
  ]
}
```

**Aggregation Functions**:
- `SUM`: مجموع
- `AVG`: میانگین
- `COUNT`: تعداد
- `MIN`: حداقل
- `MAX`: حداکثر

---

## History Tables

برای ذخیره تاریخچه تغییرات:

```json
{
  "HistoryTable": "Products_History"
}
```

وقتی `UpdateByKey` اجرا می‌شود:
1. نسخه قبلی در History Table ذخیره می‌شود
2. سپس Update انجام می‌شود

---

## ClientUI Structure

### فیلدهای ClientUI

```json
{
  "FileName": "Products_ReadList",
  "TemplateName": "ReadList",
  "LoadAPI": "ReadList",
  "SubmitAPI": "",
  "PreventReBuilding": false
}
```

**FileName**: نام فایل Vue Component  
**TemplateName**: نام Template برای Build  
**LoadAPI**: API برای بارگذاری داده  
**SubmitAPI**: API برای Submit  
**PreventReBuilding**: جلوگیری از Build مجدد

---

## FAQ (سوالات متداول)

### چگونه یک متد جدید اضافه کنم؟

1. فایل `.cs` را در `workspace/server/` ایجاد کنید
2. Namespace و Class را تعریف کنید
3. متد Static Public را اضافه کنید
4. `DynaCode.Refresh()` را فراخوانی کنید

### چگونه UI را سفارشی کنم؟

1. Template را در `a..templates/` ویرایش کنید
2. یا کامپوننت Vue را مستقیماً ویرایش کنید (اگر `PreventReBuilding` true است)

### چگونه Cache را Clear کنم؟

```csharp
CacheServices.RemoveAllCacheItems();
```

یا در AppEndStudio به بخش Cache بروید.

### چگونه لاگ‌ها را مشاهده کنم؟

1. در AppEndStudio به بخش "Log" بروید
2. یا در Database: جدول `BaseActivityLog`
3. یا در فایل‌های لاگ در `log/`

### چگونه یک Relation ایجاد کنم؟

1. در DbDialog Designer بروید
2. بخش Relations را باز کنید
3. Relation جدید را اضافه کنید
4. Foreign Key را تنظیم کنید

### چگونه از چند دیتابیس استفاده کنم؟

1. در `appsettings.json` دیتابیس‌های جدید را اضافه کنید
2. در Query Full Name از فرمت `{DbConfName}.{ObjectName}.{MethodName}` استفاده کنید

### چگونه یک Package ایجاد کنم؟

1. به Package Manager بروید
2. "Create Package" را کلیک کنید
3. اطلاعات Package را وارد کنید
4. فایل‌ها را اضافه کنید
5. Export کنید

---

## مثال‌های کاربردی

### مثال 1: ایجاد یک Query با Filter و Pagination

```json
{
  "QueryFullName": "DefaultRepo.Products.ReadList",
  "Where": {
    "ConjunctiveOperator": "AND",
    "CompareClauses": [
      {
        "Name": "IsActive",
        "CompareOperator": "Equal",
        "Value": true
      },
      {
        "Name": "Price",
        "CompareOperator": "MoreThan",
        "Value": 100
      }
    ]
  },
  "OrderClauses": [
    {
      "Name": "CreatedOn",
      "OrderDirection": "DESC"
    }
  ],
  "Pagination": {
    "PageNumber": 1,
    "PageSize": 20
  }
}
```

### مثال 2: ایجاد یک Record با Relations

```json
{
  "QueryFullName": "DefaultRepo.Products.Create",
  "Params": [
    {
      "Name": "Name",
      "Value": "Product Name"
    },
    {
      "Name": "Price",
      "Value": 99.99
    }
  ],
  "Relations": {
    "To_ProductImages_On_ProductId": [
      [
        {
          "Name": "ImageUrl",
          "Value": "image1.jpg"
        }
      ],
      [
        {
          "Name": "ImageUrl",
          "Value": "image2.jpg"
        }
      ]
    ]
  }
}
```

### مثال 3: Update با History

```json
{
  "QueryFullName": "DefaultRepo.Products.UpdateByKey",
  "Params": [
    {
      "Name": "Id",
      "Value": 1
    },
    {
      "Name": "Price",
      "Value": 149.99
    }
  ]
}
```

اگر `HistoryTable` تنظیم شده باشد، نسخه قبلی ذخیره می‌شود.

---

## Glossary (واژه‌نامه)

- **DbDialog**: متادیتای یک جدول/View شامل Columns، Relations، Queries
- **DbQuery**: تعریف یک Query خاص (ReadList، Create، ...)
- **ClientQuery**: اجرای یک Query از Client با پارامترهای داینامیک
- **ClientUI**: تعریف یک UI Component که باید Build شود
- **ValueSharp**: سیستم تعیین مقادیر پیش‌فرض و پردازش شده
- **RPC**: Remote Procedure Call - روش ارتباط Frontend و Backend
- **DynaCode**: سیستم کامپایل و اجرای داینامیک کد C#
- **BuildInfo**: اطلاعات لازم برای Build Template
- **Containment**: سیستم کنترل ستون‌های برگردانده شده
- **Relation**: ارتباط بین دو جدول (OneToMany، ManyToMany)

---

## Frontend Architecture - معماری Frontend

این بخش به صورت جامع و کامل به معماری، ساختار و نحوه کار Frontend می‌پردازد.

### ساختار کلی پوشه‌های Frontend

```
workspace/client/
├── index.html                    # صفحه اصلی (Redirect به AppEndStudio)
├── favicon.ico                   # Favicon
├── manifest.json                 # PWA Manifest
├── serviceWorker.js              # Service Worker برای PWA
│
├── a..lib/                       # پوشه کتابخانه‌ها (Reserved)
│   ├── append-all.js            # فایل JavaScript اصلی (Bundle)
│   ├── append-all-ltr.css       # فایل CSS LTR (Bundle)
│   ├── append-all-rtl.css       # فایل CSS RTL (Bundle)
│   ├── append-client.js         # کلاینت RPC اصلی
│   ├── append-client.css        # استایل کلاینت
│   ├── append-helpers.js        # توابع کمکی
│   ├── append-jQuery-plugins.js # پلاگین‌های jQuery سفارشی
│   │
│   ├── jquery/                  # jQuery 3.7.1
│   ├── bootstrap/               # Bootstrap (CSS & JS)
│   ├── vue/                     # Vue.js 3
│   │   ├── vue.global.js
│   │   └── vue3-sfc-loader.js   # Loader برای Vue SFC
│   │
│   ├── ace/                     # Ace Editor (Code Editor)
│   │   ├── src-min/             # Source files
│   │   └── css/                 # Themes و Styles
│   │
│   ├── fontawesome-free/        # Font Awesome Icons
│   ├── Trumbowyg/               # WYSIWYG Editor
│   ├── croppie/                 # Image Cropper
│   ├── jstree/                  # Tree View
│   ├── draganddrop/             # Drag & Drop
│   ├── flex-splitter/           # Resizable Panels
│   ├── OverlayScrollbars/       # Custom Scrollbars
│   │
│   ├── misc/                    # سایر کتابخانه‌ها
│   │   ├── lodash.js            # Lodash Utility Library
│   │   ├── moment.js            # Date/Time Library
│   │   ├── jalaali.js           # Jalali Calendar
│   │   └── animate.min.css      # CSS Animations
│   │
│   ├── bsDateTimePicker/        # Bootstrap DateTime Picker
│   ├── images/                  # تصاویر مشترک
│   └── mode-*.js                # Syntax Highlighters برای Ace
│
├── a..templates/                 # Template‌های Razor (Reserved)
│   ├── ReadList.cshtml          # Template لیست
│   ├── Create.cshtml            # Template ایجاد
│   ├── UpdateByKey.cshtml       # Template ویرایش
│   ├── ReadByKey.cshtml         # Template نمایش
│   ├── ReadTreeList.cshtml      # Template لیست درختی
│   └── FormColumnContent.cshtml # محتوای ستون‌های فرم
│
├── a.Components/                 # کامپوننت‌های Vue تولید شده (Reserved)
│   └── [TableName]_[QueryType].vue
│
├── a.SharedComponents/           # کامپوننت‌های مشترک (Reserved)
│   ├── BaseComponentLoader.vue  # لودر کامپوننت
│   ├── BaseContent.vue          # نمایش محتوا
│   ├── BaseFileEditor.vue       # ویرایشگر فایل
│   ├── BaseJsonView.vue         # نمایش JSON
│   ├── BaseConfirm.vue          # Confirm Dialog
│   ├── BasePrompt.vue           # Prompt Dialog
│   ├── Login.vue                # صفحه لاگین
│   ├── SideMenu.vue             # منوی کناری ساده
│   ├── SideMenu2Level.vue       # منوی دو سطحه
│   └── ... (سایر کامپوننت‌ها)
│
├── a.Layouts/                    # Layout‌ها (Reserved)
│   ├── BO.vue                   # Back Office Layout
│   └── FO.vue                   # Front Office Layout
│
├── AppEndStudio/                 # اپلیکیشن Studio
│   ├── index.html               # صفحه اصلی
│   ├── app.json                 # تنظیمات اپلیکیشن
│   ├── assets/                  # Asset‌های مخصوص Studio
│   └── components/              # کامپوننت‌های Studio
│
└── [ApplicationName]/            # اپلیکیشن‌های ایجاد شده
    ├── index.html
    ├── app.json
    ├── assets/
    └── components/
```

### فایل‌های HTML

#### 1. index.html (Root)

فایل اصلی که به `AppEndStudio` redirect می‌کند:

```html
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <title>...</title>
    <script>
        window.location = "/AppEndStudio/";
    </script>
</head>
<body>...</body>
</html>
```

#### 2. Application index.html

ساختار کلی یک `index.html` اپلیکیشن:

```html
<!DOCTYPE html>
<html lang="en" spellcheck="false">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <link rel="manifest" href="/manifest.json">
    <link rel="icon" href="assets/Logo-Only.png">
    
    <title>...</title>
    
    <!-- Service Worker Registration -->
    <script>
        (function () {
            if ('serviceWorker' in navigator) 
                navigator.serviceWorker.register('/serviceWorker.js', { scope: '/' });
            
            // Session Storage Sync
            if (!sessionStorage.length) 
                localStorage.setItem('getSessionStorage', Date.now());
                
            window.addEventListener('storage', function (event) {
                // Sync session storage between tabs
            });
        })();
    </script>
    
    <!-- CSS Files -->
    <link href="/a..lib/append-all-ltr.css" rel="stylesheet" />
    <link href="assets/custom.css" rel="stylesheet" />
</head>
<body class="bg-light bg-gradient">
    <!-- Vue App Container -->
    <div id="app" class="h-100">
        <component-loader 
            v-if="shared.isLogedIn() === true" 
            src="/a.Layouts/BO" 
            cid="studiEnv">
        </component-loader>
        <component-loader 
            v-else 
            src="/a.SharedComponents/Login" 
            cid="loginEnv" />
    </div>
    
    <!-- Loading Screen -->
    <div class="static-working-cover">...</div>
    
    <!-- JavaScript Files -->
    <script src="/a..lib/append-all.js"></script>
    <script src="assets/custom.js"></script>
    
    <!-- Vue App Initialization -->
    <script>
        $(document).ready(function () {
            initPage();
            let wId = showWorking(shared.heavyWorkingCover);
            
            // Create Vue App
            vApp = Vue.createApp();
            vApp.config.globalProperties.shared = shared;
            vApp.config.warnHandler = () => null;
            
            // Register Component Loader
            vApp.component('component-loader', 
                loadVM("/a.SharedComponents/BaseComponentLoader.vue"));
            
            // Mount App
            vInstance = vApp.mount("#app");
            
            hideWorking(wId);
        });
    </script>
</body>
</html>
```

### کتابخانه‌های JavaScript

#### append-all.js

این فایل Bundle شامل همه کتابخانه‌های مورد نیاز است. به صورت خودکار ساخته می‌شود و شامل:

- jQuery
- Bootstrap
- Vue.js
- Lodash
- و سایر کتابخانه‌ها

#### append-client.js

**کلاینت RPC اصلی** - این فایل شامل تمام توابع لازم برای ارتباط با Backend است.

**توابع اصلی**:

1. **RPC Functions**:
   - `rpc(options)`: ارسال درخواست RPC (Async)
   - `rpcSync(options)`: ارسال درخواست RPC (Sync)
   - `rpcAEP(method, inputs, onDone, onFail)`: فراخوانی ساده

2. **Component Management**:
   - `openComponent(src, options)`: باز کردن کامپوننت
   - `closeComponent(cid)`: بستن کامپوننت
   - `loadVM(componentPath)`: لود کردن Vue Component

3. **Authentication**:
   - `login(loginInfo)`: لاگین
   - `logout(callback)`: لاگ اوت
   - `isLogedIn()`: بررسی وضعیت لاگین
   - `getUserToken()`: دریافت Token
   - `getUserObject()`: دریافت اطلاعات کاربر

4. **Navigation**:
   - `getAppConfig()`: دریافت تنظیمات اپلیکیشن
   - `getAppNav()`: دریافت Navigation
   - `getQueryString(name)`: دریافت Query Parameter
   - `setQueryString(name, value)`: تنظیم Query Parameter

5. **UI Utilities**:
   - `showWorking(cover)`: نمایش Loading
   - `hideWorking(id)`: پنهان کردن Loading
   - `showMessage(options)`: نمایش پیام
   - `showConfirm(options)`: نمایش Confirm Dialog
   - `showPrompt(options)`: نمایش Prompt Dialog

6. **Formatting**:
   - `formatDate(date)`: فرمت تاریخ
   - `formatDateTime(dateTime)`: فرمت تاریخ و زمان
   - `formatNumber(number)`: فرمت عدد

#### append-helpers.js

**توابع کمکی** شامل:

- `getSessionItemSync()`: دریافت از Session Storage
- `formatDate()`: فرمت تاریخ
- `formatDateTime()`: فرمت تاریخ و زمان
- `parseJwt()`: Parse کردن JWT Token
- `decodeB64Unicode()`: Decode Base64
- و ...

### Component Loader System

#### BaseComponentLoader.vue

این کامپوننت اصلی برای لود کردن سایر کامپوننت‌ها استفاده می‌شود.

**نحوه کار**:
1. کامپوننت Path را از Prop دریافت می‌کند
2. با استفاده از `vue3-sfc-loader` کامپوننت را لود می‌کند
3. کامپوننت را Render می‌کند

**استفاده**:
```vue
<component-loader 
    src="/a.Components/Products_ReadList" 
    cid="productsList" 
    uid="uniqueId"
    ismodal="false" />
```

**Path Formats**:
- `/a.Components/ComponentName`: مسیر کامل
- `components/ComponentName`: مسیر نسبی
- `qs:c`: از Query String دریافت می‌کند

#### Vue Component Loading

کامپوننت‌های Vue به صورت **Dynamic** لود می‌شوند:

```javascript
function loadVM(componentPath) {
    const { loadModule } = window["vue3-sfc-loader"];
    const options = {
        moduleCache: { vue: Vue },
        getFile(url) {
            return fetch(url).then(resp => resp.text());
        },
        addStyle(styleStr) {
            const style = document.createElement("style");
            style.textContent = styleStr;
            document.head.appendChild(style);
        }
    };
    return Vue.defineAsyncComponent(() => loadModule(componentPath, options));
}
```

### Routing System

AppEnd از **Query String Routing** استفاده می‌کند.

#### URL Format

```
http://localhost:5000/AppEndStudio/?c=components/BaseHome
```

- `c`: Parameter برای Component Name
- می‌توان Parameters اضافی اضافه کرد: `&param1=value1&param2=value2`

#### Navigation

Navigation از فایل `app.json` خوانده می‌شود:

```javascript
function getAppNav() {
    let nav = getAppConfig()["navigation"];
    return nav || [];
}
```

**مثال Navigation Item**:
```json
{
  "title": "Products",
  "icon": "fa-box",
  "component": "a.Components/Products_ReadList",
  "params": "&filter=active"
}
```

**لینک در Navigation**:
```vue
<a href="?c=components/BaseHome">Home</a>
```

#### Component Routing در Layout

در Layout از `component-loader` با `qs:c` استفاده می‌شود:

```vue
<component-loader src="qs:c" cid="dynamicContent" />
```

این کار Component را از Query String می‌گیرد و نمایش می‌دهد.

### Shared Object (shared)

یک Object Global که در همه جا در دسترس است:

```javascript
var shared = {
    // Authentication
    isLogedIn() { ... },
    getUserObject() { ... },
    
    // Component Management
    openComponent(src, options) { ... },
    closeComponent(cid) { ... },
    
    // Navigation
    getAppConfig() { ... },
    getAppNav() { ... },
    
    // RPC
    talkPoint: "/talk-to-me/",
    
    // Utilities
    translate(key) { ... },
    formatDate(date) { ... },
    formatDateTime(dateTime) { ... },
    
    // ...
};
```

**استفاده در Vue Components**:
```vue
<script>
export default {
    methods: {
        myMethod() {
            this.shared.isLogedIn();
            this.shared.openComponent(...);
        }
    }
}
</script>
```

### Layouts

#### BO.vue (Back Office)

Layout اصلی برای اپلیکیشن‌های مدیریتی:

**ساختار**:
```
┌─────────────────────────────────────┐
│         Header (Logo, Title)        │
├──────────┬──────────────────────────┤
│          │                          │
│ Sidebar  │     Content Area         │
│ (Menu)   │     (Dynamic Component)  │
│          │                          │
└──────────┴──────────────────────────┘
```

**ویژگی‌ها**:
- Sidebar با Menu
- Responsive (Sidebar در موبایل مخفی می‌شود)
- Header با User Menu
- Breadcrumb
- Dynamic Content Area

**کد ساختار**:
```vue
<template>
    <div class="d-flex flex-column h-100">
        <!-- Header -->
        <div class="header">...</div>
        
        <!-- Main Content -->
        <div class="d-flex flex-grow-1">
            <!-- Sidebar -->
            <div class="sidebar">
                <component-loader 
                    src="/a.SharedComponents/SideMenu2Level.vue" 
                    uid="sideMenu" />
            </div>
            
            <!-- Content -->
            <main>
                <component-loader 
                    src="qs:c" 
                    cid="dynamicContent" />
            </main>
        </div>
    </div>
</template>
```

#### FO.vue (Front Office)

Layout برای اپلیکیشن‌های عمومی:

**ساختار**:
```
┌─────────────────────────────────────┐
│         Navigation Bar              │
├─────────────────────────────────────┤
│                                     │
│         Content Area                │
│                                     │
├─────────────────────────────────────┤
│            Footer                   │
└─────────────────────────────────────┘
```

### Shared Components

#### BaseComponentLoader.vue

**مسئولیت**: لود کردن داینامیک Vue Components

**Props**:
- `src`: مسیر کامپوننت
- `cid`: Component ID
- `uid`: Unique ID
- `ismodal`: آیا Modal است

#### BaseContent.vue

**مسئولیت**: نمایش محتوای HTML

**Props**:
- `content`: Object با Title و ContentBody

#### BaseFileEditor.vue

**مسئولیت**: ویرایشگر فایل با Syntax Highlighting

**ویژگی‌ها**:
- پشتیبانی از Syntax Highlighting (Ace Editor)
- پشتیبانی از انواع فایل (C#, SQL, JSON, ...)
- ذخیره خودکار

#### BaseJsonView.vue

**مسئولیت**: نمایش JSON به صورت Tree View

#### BaseConfirm.vue

**مسئولیت**: نمایش Confirm Dialog

**استفاده**:
```javascript
shared.showConfirm({
    title: "Confirm",
    message: "Are you sure?",
    callback: function(confirmed) {
        if (confirmed) {
            // Do something
        }
    }
});
```

#### BasePrompt.vue

**مسئولیت**: نمایش Prompt Dialog

**استفاده**:
```javascript
shared.showPrompt({
    title: "Enter Name",
    message1: "Enter your name:",
    callback: function(value) {
        console.log(value);
    }
});
```

#### Login.vue

**مسئولیت**: صفحه لاگین

**ویژگی‌ها**:
- فرم لاگین
- Remember Me
- Validation
- Error Handling

#### SideMenu.vue

**مسئولیت**: منوی کناری ساده (یک سطح)

**نحوه کار**:
- Navigation را از `app.json` می‌خواند
- Menu Items را نمایش می‌دهد
- Active Item را Highlight می‌کند

#### SideMenu2Level.vue

**مسئولیت**: منوی دو سطحه

**ویژگی‌ها**:
- Level 1: Menu Groups
- Level 2: Menu Items
- Collapsible
- Active State

#### MyProfile.vue

**مسئولیت**: نمایش و ویرایش پروفایل کاربر

#### ImageEditor.vue

**مسئولیت**: ویرایشگر تصویر (Crop, Resize)

### Application Structure

#### app.json

فایل تنظیمات هر اپلیکیشن:

```json
{
  "title": "Application Title",
  "sub-title": "Subtitle",
  "dir": "ltr",                    // ltr یا rtl
  "lang": "en",                    // زبان پیش‌فرض
  "calendar": "Gregorian",         // تقویم (Gregorian, Jalali)
  "defaultComponent": "components/Home",
  "translation": {                 // ترجمه‌ها
    "Home": "خانه",
    "Products": "محصولات"
  },
  "navigation": [
    {
      "title": "Section",
      "icon": "fa-icon",
      "items": [
        {
          "title": "Item",
          "icon": "fa-icon",
          "component": "components/MyComponent",
          "params": "&param=value"
        }
      ]
    }
  ]
}
```

**فیلدهای مهم**:
- `title`: عنوان اپلیکیشن
- `sub-title`: زیر عنوان
- `dir`: جهت متن (ltr/rtl)
- `lang`: زبان
- `calendar`: نوع تقویم
- `defaultComponent`: کامپوننت پیش‌فرض
- `translation`: دیکشنری ترجمه
- `navigation`: ساختار Navigation

### Component Structure

#### ساختار یک Vue Component

```vue
<template>
    <!-- HTML Template -->
    <div>
        <h1>{{ title }}</h1>
    </div>
</template>

<script>
let _this = {
    cid: "",
    c: null,
    inputs: {},
    local: {
        // Local Data
    }
};

export default {
    props: {
        cid: String,
        // Other Props
    },
    setup(props) {
        _this.cid = props.cid;
        _this.inputs = shared["params_" + _this.cid] || {};
    },
    data() {
        return _this;
    },
    created() {
        _this.c = this;
    },
    mounted() {
        // Initialization
        initVueComponent(_this);
    },
    methods: {
        // Methods
    }
}
</script>

<style scoped>
/* Styles */
</style>
```

#### دریافت Parameters

Parameters از Query String یا هنگام باز کردن Component:

```javascript
// از Query String
let param = shared.getQueryString('paramName');

// از inputs (وقتی Component باز می‌شود)
_this.inputs = shared["params_" + _this.cid];
```

### RPC Client Library (جزئیات)

#### ساختار RPC Request

```javascript
{
    Id: "unique-id",
    Method: "DefaultRepo.Products.ReadList",
    Inputs: {
        ClientQueryJE: {
            QueryFullName: "DefaultRepo.Products.ReadList",
            Pagination: {
                PageNumber: 1,
                PageSize: 10
            },
            Where: {
                CompareClauses: [...]
            }
        }
    }
}
```

#### ساختار RPC Response

```javascript
{
    Id: "unique-id",
    Duration: 125,
    IsSucceeded: true,
    FromCache: false,
    Result: {
        Master: [
            { Id: 1, Name: "Product 1" },
            { Id: 2, Name: "Product 2" }
        ],
        Aggregations: {...}
    }
}
```

#### Cache در Client

کلاینت به صورت خودکار Response‌ها را Cache می‌کند:

```javascript
function analyzeRequests(requests) {
    let cached = [];
    let todo = [];
    
    requests.forEach(req => {
        let cacheKey = generateCacheKey(req);
        if (cache.has(cacheKey)) {
            cached.push(cache.get(cacheKey));
        } else {
            todo.push(req);
        }
    });
    
    return {
        cachedResponses: cached,
        todoRequests: todo
    };
}
```

### Templates System

#### ساختار Template

Template‌ها فایل‌های Razor (.cshtml) هستند که Vue Component تولید می‌کنند.

**مثال Template**:
```razor
@{
    string objectName = Model.DbDialog.ObjectName;
    string loadAPI = Model.ClientUI.LoadAPI;
}
<template>
    <div class="card">
        <div class="card-header">
            <h1>@objectName</h1>
        </div>
        <div class="card-body">
            <!-- Vue Template Code -->
            <div v-for="row in data">
                {{ row.Name }}
            </div>
        </div>
    </div>
</template>

<script>
export default {
    data() {
        return {
            data: []
        }
    },
    mounted() {
        // Load data
    }
}
</script>
```

#### Template Variables

در Template می‌توان از `Model` (BuildInfo) استفاده کرد:

- `Model.DbDialog`: DbDialog Object
- `Model.ClientUI`: ClientUI Object
- `Model.Parameters`: Dictionary اضافی

#### Template Helpers

Helper Methods موجود در Template:

- `Model.GetDisplayColumns(column)`: ستون‌های نمایشی
- `Model.GetTargetPkColumn(column)`: PK Column
- `Model.GetFirstFileFieldName()`: نام فیلد فایل

### Translation System

#### نحوه کار

ترجمه‌ها در فایل `app.json` در بخش `translation` ذخیره می‌شوند:

```json
{
  "translation": {
    "Home": "خانه",
    "Products": "محصولات",
    "Create": "ایجاد"
  }
}
```

#### استفاده در Components

```vue
<template>
    <div>
        <h1>{{ shared.translate("Home") }}</h1>
    </div>
</template>
```

```javascript
shared.translate("Products"); // "محصولات"
```

#### Extract Translation Keys

می‌توانید به صورت خودکار کلیدهای ترجمه را از Components استخراج کنید:
- در AppEndStudio به "Themes" → "Translation Management" بروید
- روی "Extract Keys" کلیک کنید
- کلیدهای موجود در Components استخراج می‌شوند

### PWA Support

#### Service Worker

AppEnd از Service Worker برای PWA پشتیبانی می‌کند:

```javascript
// serviceWorker.js
self.addEventListener('install', evt => {
    // Cache assets
});

self.addEventListener('fetch', evt => {
    // Serve from cache
});
```

#### Manifest

فایل `manifest.json`:

```json
{
  "short_name": "AppEnd",
  "name": "AppEnd",
  "icons": [...],
  "start_url": ".",
  "display": "standalone",
  "theme_color": "#000000",
  "background_color": "#ffffff"
}
```

### Styling System

#### CSS Files

1. **append-all-ltr.css**: Bundle CSS برای LTR
2. **append-all-rtl.css**: Bundle CSS برای RTL
3. **append-client.css**: استایل‌های مخصوص کلاینت
4. **custom.css**: استایل‌های سفارشی هر اپلیکیشن

#### Bootstrap Integration

AppEnd از Bootstrap استفاده می‌کند:
- Classes: `btn`, `card`, `form-control`, ...
- Utilities: `text-center`, `mb-3`, `d-flex`, ...
- Components: Modal, Toast, Dropdown, ...

#### Custom CSS Variables

می‌توان از CSS Variables استفاده کرد:

```css
:root {
    --bs-primary: #0d6efd;
    --bs-primary-rgb: 13, 110, 253;
}
```

### Widget System

#### Data Attributes

می‌توان از Data Attributes برای فعال‌سازی Widget‌ها استفاده کرد:

```html
<div data-ae-widget="datepicker" 
     data-ae-widget-options='{"format":"YYYY-MM-DD"}'>
</div>
```

**Widgets موجود**:
- `datepicker`: Date Picker
- `datetimepicker`: DateTime Picker
- `select2`: Enhanced Select
- `fileupload`: File Upload
- و ...

### Modal System

#### باز کردن Modal

```javascript
shared.openComponent("/a.Components/MyComponent", {
    title: "Modal Title",
    modalSize: "modal-lg",
    showHeader: true,
    showFooter: false,
    resizable: true,
    draggable: true,
    closeByOverlay: true,
    params: {
        param1: "value1"
    }
});
```

**Options**:
- `title`: عنوان Modal
- `modalSize`: اندازه (`modal-sm`, `modal-lg`, `modal-xl`, `modal-fullscreen`)
- `showHeader`: نمایش Header
- `showFooter`: نمایش Footer
- `resizable`: قابل تغییر اندازه
- `draggable`: قابل Drag
- `closeByOverlay`: بستن با کلیک روی Overlay
- `params`: پارامترها برای کامپوننت

### File Structure Details

#### a..lib/ (کتابخانه‌ها)

این پوشه شامل تمام کتابخانه‌های JavaScript و CSS است.

**کتابخانه‌های اصلی**:
- **jQuery 3.7.1**: کتابخانه اصلی DOM Manipulation
- **Bootstrap 5**: Framework CSS
- **Vue.js 3**: Framework JavaScript
- **Lodash**: Utility Functions
- **Ace Editor**: Code Editor
- **Font Awesome**: Icons
- **Moment.js**: Date/Time Library
- **Jalaali.js**: Jalali Calendar
- **Trumbowyg**: WYSIWYG Editor
- **Croppie**: Image Cropper
- **jstree**: Tree View

#### a..templates/ (Template‌ها)

Template‌های Razor که Vue Components تولید می‌کنند:

1. **ReadList.cshtml**: لیست داده‌ها
2. **Create.cshtml**: فرم ایجاد
3. **UpdateByKey.cshtml**: فرم ویرایش
4. **ReadByKey.cshtml**: نمایش یک رکورد
5. **ReadTreeList.cshtml**: لیست درختی
6. **BaseEmptyComponent.cshtml**: کامپوننت خالی
7. **FormColumnContent.cshtml**: محتوای ستون فرم
8. **FormRelation*.cshtml**: Relation Forms

#### a.Components/ (کامپوننت‌های تولید شده)

کامپوننت‌های Vue که از Template‌ها تولید شده‌اند:

**Naming Convention**:
- `{ObjectName}_{QueryType}.vue`
- مثال: `Products_ReadList.vue`, `Orders_Create.vue`

#### a.SharedComponents/ (کامپوننت‌های مشترک)

کامپوننت‌هایی که در همه جا قابل استفاده هستند:

1. **BaseComponentLoader.vue**: لودر کامپوننت
2. **BaseContent.vue**: نمایش محتوا
3. **BaseFileEditor.vue**: ویرایشگر فایل
4. **BaseJsonView.vue**: نمایش JSON
5. **BaseConfirm.vue**: Confirm Dialog
6. **BasePrompt.vue**: Prompt Dialog
7. **Login.vue**: صفحه لاگین
8. **SideMenu.vue**: منوی کناری
9. **SideMenu2Level.vue**: منوی دو سطحه
10. **MyProfile.vue**: پروفایل کاربر
11. **ImageEditor.vue**: ویرایشگر تصویر
12. **DbObjectPicker.vue**: انتخاب DbObject
13. و ...

#### a.Layouts/ (Layout‌ها)

Layout‌های اصلی:

1. **BO.vue**: Back Office Layout (با Sidebar)
2. **FO.vue**: Front Office Layout (ساده)

### Component Communication

#### Parent-Child Communication

از Props و Events استفاده می‌شود:

```vue
<!-- Parent -->
<component-loader 
    src="components/Child" 
    cid="child"
    :someProp="value" />

<!-- Child -->
<script>
export default {
    props: {
        someProp: String
    },
    methods: {
        notifyParent() {
            this.$emit('event-name', data);
        }
    }
}
</script>
```

#### Global Communication

از `shared` object استفاده می‌شود:

```javascript
// Set global data
shared.globalData = { ... };

// Get global data
let data = shared.globalData;
```

#### Session Storage

برای ذخیره داده‌ها در Session:

```javascript
sessionStorage.setItem('key', JSON.stringify(data));
let data = JSON.parse(sessionStorage.getItem('key'));
```

### Form Validation

#### Validation System

از Data Attributes برای Validation استفاده می‌شود:

```html
<input 
    type="text" 
    data-ae-validation-required="true"
    data-ae-validation-rule=":=s(0,100)" />
```

**Validation Rules**:
- `required`: الزامی
- `rule`: Rule خاص (مثلاً `:=s(0,100)` برای String با طول 0-100)

#### Inputs Regulator

```javascript
this.regulator = $(`#${this.cid}`).inputsRegulator();

// Validate
if (this.regulator.isValid()) {
    // Submit
}
```

### Image Handling

#### نمایش تصاویر

```javascript
// از Bytes به URI
let imageUri = shared.getImageURI(imageBytes);

// در Template
<img :src="shared.getImageURI(row.Picture)" />
```

#### Image Editor

استفاده از `ImageEditor.vue`:

```javascript
shared.openComponent("/a.SharedComponents/ImageEditor", {
    params: {
        imageBytes: imageData,
        callback: function(editedImage) {
            // Handle edited image
        }
    }
});
```

### Calendar System

#### پشتیبانی از تقویم‌های مختلف

- **Gregorian**: تقویم میلادی
- **Jalali**: تقویم شمسی

**استفاده**:
```javascript
// با توجه به تنظیمات اپلیکیشن
shared.formatDateL(date, getAppConfig()["calendar"]);
```

### Keyboard Shortcuts

#### مدیریت Shortcuts

```javascript
// دریافت Shortcuts
let shortcuts = shared.getUserShortcuts();

// تنظیم Shortcut
shared.setUserShortcuts([
    { key: "Ctrl+S", action: "save" }
]);
```

### Working Indicators

#### Loading States

```javascript
// نمایش Loading
let workingId = shared.showWorking(shared.heavyWorkingCover);

// پنهان کردن Loading
shared.hideWorking(workingId);
```

**انواع Loading Cover**:
- `shared.heavyWorkingCover`: Loading کامل صفحه
- `shared.notHeavyWorkingCover`: Loading شفاف
- `shared.miniHeavyWorkingCover`: Loading کوچک

### Toast Notifications

#### نمایش پیام‌ها

```javascript
// Success
shared.showSuccess("Operation completed!");

// Error
shared.showError("Something went wrong!");

// Warning
shared.showWarning("Please check!");

// Info
shared.showInfo("Information message");
```

### Component Lifecycle

#### Lifecycle Hooks

```vue
<script>
export default {
    setup(props) {
        // Setup Phase
    },
    created() {
        // Component Created
    },
    mounted() {
        // Component Mounted
        initVueComponent(this);
    },
    beforeUnmount() {
        // Cleanup
    }
}
</script>
```

### Best Practices برای Frontend

1. **استفاده از Shared Components**: تا حد ممکن از Shared Components استفاده کنید
2. **Organize Code**: کد را ماژولار کنید
3. **Use Translation**: همیشه از `shared.translate()` استفاده کنید
4. **Handle Errors**: Error Handling مناسب داشته باشید
5. **Optimize Images**: تصاویر را بهینه کنید
6. **Cache Management**: از Cache به درستی استفاده کنید
7. **Responsive Design**: طراحی Responsive داشته باشید

---

---

## خلاصه نهایی و راهنمای سریع

### راهنمای استفاده از مستندات

این مستندات شامل **31 بخش اصلی** است که همه جنبه‌های AppEnd Framework را پوشش می‌دهد:

#### بخش‌های اولیه (1-5)
- **مقدمه و معرفی**: آشنایی با AppEnd
- **معماری کلی**: ساختار سه لایه
- **ماژول‌ها**: 6 ماژول اصلی
- **جریان کار**: 3 Workflow مهم

#### بخش‌های توسعه (6-12)
- **ویژگی‌های کلیدی**: 7 ویژگی اصلی
- **ایجاد اپلیکیشن**: راهنمای گام به گام
- **توسعه**: نحوه توسعه
- **دیباگ**: Troubleshooting

#### بخش‌های فنی (13-22)
- **ساختار فایل‌ها**: پوشه‌ها و فایل‌ها
- **تنظیمات**: Configuration Reference
- **UI Widgets**: لیست کامل Widget‌ها
- **Query Builder**: Compare Operators
- **Relations**: ارتباطات بین جداول
- **ValueSharp**: سیستم مقادیر پیش‌فرض
- **Package Manager**: بسته‌بندی
- **Multiple Databases**: پشتیبانی چند دیتابیس

#### بخش‌های Frontend (23-31)
- **Frontend Architecture**: معماری کامل Frontend
- **RPC Client**: کتابخانه کلاینت
- **Routing & Navigation**: سیستم مسیریابی
- **Components**: کامپوننت‌های Vue
- **Templates**: سیستم Template
- **Translation**: چندزبانه

### نکات کلیدی برای شروع سریع

1. **برای شروع کار**:
   - بخش "نحوه ایجاد یک اپلیکیشن" را بخوانید
   - مراحل راه‌اندازی را دنبال کنید

2. **برای توسعه**:
   - بخش "نحوه توسعه به عنوان توسعه‌دهنده" را مطالعه کنید
   - مثال‌های کاربردی را بررسی کنید

3. **برای درک Frontend**:
   - بخش "Frontend Architecture" را کامل بخوانید
   - ساختار پوشه‌ها را بررسی کنید

4. **برای رفع مشکل**:
   - بخش "دیباگ و تغییرات" را ببینید
   - FAQ را بررسی کنید

### ساختار مستندات

```
مستندات (3073 خط)
├── معرفی و معماری (1-5)
├── راهنمای توسعه (6-12)
├── جزئیات فنی (13-22)
└── Frontend کامل (23-31)
```

### نمایه موضوعی

**Authentication & Authorization**: بخش 6.1, 9.1  
**Database Operations**: بخش 2.2, 4.3, 13  
**Dynamic Code**: بخش 2.3, 6.2  
**Frontend Components**: بخش 31 (Frontend Architecture)  
**Templates**: بخش 6.4, 20  
**RPC Communication**: بخش 3.1, 18  
**Query Builder**: بخش 13, 23  
**UI Widgets**: بخش 12  

---

**نسخه مستندات**: 2.1  
**تاریخ**: 2024  
**تعداد بخش‌ها**: 31  
**تعداد خطوط**: 3073+  
**نگارنده**: AI Assistant

</div>

